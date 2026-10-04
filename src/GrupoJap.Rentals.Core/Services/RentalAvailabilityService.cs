using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Models;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Services;

/// <summary>
/// Consultas de disponibilidade partilhadas pelo backoffice e pelo site público,
/// para que a regra de não sobreposição (datas inclusivas) exista num só sítio.
/// </summary>
public sealed class RentalAvailabilityService(ApplicationDbContext dbContext)
{
    /// <summary>Existe algum contrato do veículo que se sobreponha ao período? Ignora o contrato <paramref name="excludeContractId"/>.</summary>
    public Task<bool> HasOverlapAsync(int vehicleId, DateOnly start, DateOnly end, int? excludeContractId = null, CancellationToken cancellationToken = default)
        => dbContext.RentalContracts.AnyAsync(r =>
                r.VehicleId == vehicleId
                && (excludeContractId == null || r.Id != excludeContractId)
                && r.StartDate <= end
                && r.EndDate >= start,
            cancellationToken);

    /// <summary>Veículos sem contratos que se sobreponham ao período indicado.</summary>
    public IQueryable<Vehicle> AvailableBetween(IQueryable<Vehicle> vehicles, DateOnly start, DateOnly end)
        => vehicles.Where(v => !v.RentalContracts.Any(r => r.StartDate <= end && r.EndDate >= start));

    /// <summary>Períodos já ocupados (presentes e futuros) de um veículo, por ordem cronológica.</summary>
    public async Task<List<(DateOnly Start, DateOnly End)>> BookedPeriodsAsync(int vehicleId, DateOnly from, CancellationToken cancellationToken = default)
    {
        var rows = await dbContext.RentalContracts
            .AsNoTracking()
            .Where(r => r.VehicleId == vehicleId && r.EndDate >= from)
            .OrderBy(r => r.StartDate)
            .Select(r => new { Start = r.StartDate!.Value, End = r.EndDate!.Value })
            .ToListAsync(cancellationToken);

        return rows.Select(r => (r.Start, r.End)).ToList();
    }
}
