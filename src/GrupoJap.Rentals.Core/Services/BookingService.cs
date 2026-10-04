using System.Data;
using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Services;

/// <summary>Erro de reserva; <c>Message</c> é uma chave de tradução (ex.: booking.error.overlap).</summary>
public sealed record BookingError(string Field, string Message);

public sealed record BookingResult(int? ContractId, IReadOnlyList<BookingError> Errors)
{
    public bool Succeeded => ContractId is not null;

    public static BookingResult Fail(string field, string message) => new(null, [new BookingError(field, message)]);
}

/// <summary>
/// Reserva online: cria (se preciso) a ficha de cliente do utilizador e um contrato de aluguer,
/// aplicando as mesmas regras do backoffice (datas e não sobreposição).
/// </summary>
public sealed class BookingService(ApplicationDbContext dbContext, RentalAvailabilityService availability)
{
    public const string OverlapMessage = "booking.error.overlap";

    /// <summary>A ficha de cliente de um utilizador é a que tem o mesmo email.</summary>
    public Task<Customer?> FindCustomerAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(user.Email);
        return dbContext.Customers.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<BookingResult> CreateAsync(ApplicationUser user, BookingFormViewModel form, DateOnly today, CancellationToken cancellationToken = default)
    {
        var errors = new List<BookingError>();

        if (form.StartDate is not DateOnly start || form.EndDate is not DateOnly end)
        {
            return BookingResult.Fail(nameof(form.StartDate), "booking.error.dates_required");
        }

        if (start < today)
        {
            errors.Add(new(nameof(form.StartDate), "booking.error.start_past"));
        }

        if (end <= start)
        {
            errors.Add(new(nameof(form.EndDate), "booking.error.end_before_start"));
        }

        var vehicle = await dbContext.Vehicles.FirstOrDefaultAsync(v => v.Id == form.VehicleId, cancellationToken);
        if (vehicle is null)
        {
            return BookingResult.Fail(string.Empty, "booking.error.vehicle_missing");
        }

        var customer = await FindCustomerAsync(user, cancellationToken);
        if (customer is null)
        {
            if (string.IsNullOrWhiteSpace(form.Phone))
            {
                errors.Add(new(nameof(form.Phone), "booking.error.phone_required"));
            }

            if (string.IsNullOrWhiteSpace(form.DrivingLicenseNumber))
            {
                errors.Add(new(nameof(form.DrivingLicenseNumber), "booking.error.license_required"));
            }
        }

        if (errors.Count > 0)
        {
            return new BookingResult(null, errors);
        }

        // Serializable: a verificação de sobreposição e a inserção ficam atómicas,
        // evitando que duas reservas simultâneas ocupem o mesmo período.
        var supportsTransactions = dbContext.Database.IsRelational();
        await using var transaction = supportsTransactions
            ? await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken)
            : null;

        if (await availability.HasOverlapAsync(vehicle.Id, start, end, cancellationToken: cancellationToken))
        {
            return BookingResult.Fail(nameof(form.StartDate), OverlapMessage);
        }

        if (customer is null)
        {
            customer = new Customer
            {
                FullName = string.IsNullOrWhiteSpace(user.FullName) ? (user.Email ?? string.Empty) : user.FullName.Trim(),
                Email = NormalizeEmail(user.Email),
                Phone = form.Phone!.Trim(),
                DrivingLicenseNumber = form.DrivingLicenseNumber!.Trim().ToUpperInvariant()
            };
            dbContext.Customers.Add(customer);
        }

        var contract = new RentalContract
        {
            Customer = customer,
            VehicleId = vehicle.Id,
            StartDate = start,
            EndDate = end,
            InitialMileage = vehicle.Mileage
        };
        dbContext.RentalContracts.Add(contract);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
        }
        catch (DbUpdateException)
        {
            return BookingResult.Fail(string.Empty, "booking.error.generic");
        }

        return new BookingResult(contract.Id, []);
    }

    /// <summary>Cancela uma reserva futura do próprio utilizador. Devolve false se não existir, não for dele ou já tiver começado.</summary>
    public async Task<bool> CancelAsync(ApplicationUser user, int contractId, DateOnly today, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(user.Email);
        var contract = await dbContext.RentalContracts
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == contractId && r.Customer!.Email == email, cancellationToken);

        if (contract?.StartDate is not DateOnly start || start <= today)
        {
            return false;
        }

        dbContext.RentalContracts.Remove(contract);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string NormalizeEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();
}
