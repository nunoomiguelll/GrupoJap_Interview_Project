using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Controllers;

/// <summary>Site público: página inicial, frota, detalhe da viatura, reserva e reservas do cliente.
/// As páginas são públicas; só reservar e gerir reservas exige sessão ([Authorize] em cada ação).</summary>
public sealed class SiteController(
    ApplicationDbContext dbContext,
    RentalAvailabilityService availability,
    BookingService bookingService,
    UserManager<ApplicationUser> userManager,
    Translator T) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var today = RentalRules.Today();
        var vehicles = dbContext.Vehicles.AsNoTracking();

        var featured = await ToCards(
                availability.AvailableBetween(vehicles, today, today)
                    .OrderByDescending(v => v.YearOfManufacture)
                    .ThenBy(v => v.Brand)
                    .Take(12), // 3 páginas de 4 no carrossel
                today, today)
            .ToListAsync();

        var brands = await vehicles
            .GroupBy(v => v.Brand)
            .Select(g => new { Brand = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count).ThenBy(g => g.Brand)
            .ToListAsync();

        var fuels = await vehicles
            .GroupBy(v => v.FuelType)
            .Select(g => new { Fuel = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync();

        return View(new SiteHomeViewModel
        {
            VehicleCount = await vehicles.CountAsync(),
            AvailableTodayCount = await availability.AvailableBetween(vehicles, today, today).CountAsync(),
            DistinctBrandCount = await vehicles.Select(v => v.Brand).Distinct().CountAsync(),
            Featured = featured,
            Brands = brands.Select(b => new BrandCount(b.Brand, b.Count)).ToList(),
            Fuels = fuels.Select(f => new FuelCount(f.Fuel, f.Count)).ToList()
        });
    }

    [HttpGet("viaturas")]
    public async Task<IActionResult> Fleet([FromQuery] FleetViewModel filters)
    {
        var today = RentalRules.Today();
        var query = dbContext.Vehicles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filters.Q))
        {
            var term = filters.Q.Trim();
            query = query.Where(v => v.Brand.Contains(term) || v.Model.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(filters.Brand))
        {
            query = query.Where(v => v.Brand == filters.Brand);
        }

        if (filters.Fuel is FuelType fuel && fuel != FuelType.Unspecified)
        {
            query = query.Where(v => v.FuelType == fuel);
        }

        // Período pesquisado: só é aplicado se for válido; caso contrário a disponibilidade é a de hoje.
        if (filters.Start is not null || filters.End is not null)
        {
            if (filters.Start is null || filters.End is null)
            {
                filters.PeriodError = T["fleet.error.both_dates"];
            }
            else if (filters.Start < today)
            {
                filters.PeriodError = T["fleet.error.start_past"];
            }
            else if (filters.End <= filters.Start)
            {
                filters.PeriodError = T["booking.error.end_before_start"];
            }
        }

        var start = filters.HasPeriod ? filters.Start!.Value : today;
        var end = filters.HasPeriod ? filters.End!.Value : today;

        if (filters.OnlyAvailable)
        {
            query = availability.AvailableBetween(query, start, end);
        }

        query = filters.Sort switch
        {
            FleetSort.Newest => query.OrderByDescending(v => v.YearOfManufacture).ThenBy(v => v.Brand),
            FleetSort.LowestMileage => query.OrderBy(v => v.Mileage).ThenBy(v => v.Brand),
            _ => query.OrderBy(v => v.Brand).ThenBy(v => v.Model)
        };

        filters.TotalCount = await query.CountAsync();
        filters.Page = Math.Clamp(filters.Page, 1, filters.TotalPages);
        filters.Vehicles = await ToCards(
                query.Skip((filters.Page - 1) * FleetViewModel.PageSize).Take(FleetViewModel.PageSize),
                start, end)
            .ToListAsync();
        filters.BrandOptions = await dbContext.Vehicles.AsNoTracking()
            .Select(v => v.Brand).Distinct().OrderBy(b => b).ToListAsync();

        return View(filters);
    }

    [HttpGet("viaturas/{id:int}")]
    public async Task<IActionResult> Details(int id, DateOnly? start, DateOnly? end)
    {
        var model = await BuildDetailsAsync(id, new BookingFormViewModel { VehicleId = id, StartDate = start, EndDate = end });
        return model is null ? NotFound() : View(model);
    }

    [HttpPost("viaturas/{id:int}/reservar")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(int id, [Bind(Prefix = "Booking")] BookingFormViewModel form)
    {
        form.VehicleId = id;
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (ModelState.IsValid)
        {
            var result = await bookingService.CreateAsync(user, form, RentalRules.Today());
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = T["booking.success"].Value;
                return RedirectToAction(nameof(MyBookings));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.IsNullOrEmpty(error.Field) ? string.Empty : "Booking." + error.Field, T[error.Message]);
            }
        }

        var model = await BuildDetailsAsync(id, form);
        return model is null ? NotFound() : View(nameof(Details), model);
    }

    [HttpGet("conta/reservas")]
    [Authorize]
    public async Task<IActionResult> MyBookings()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var today = RentalRules.Today();
        var customer = await bookingService.FindCustomerAsync(user);
        var bookings = new List<MyBookingItemViewModel>();
        if (customer is not null)
        {
            var rows = await dbContext.RentalContracts
                .AsNoTracking()
                .Where(r => r.CustomerId == customer.Id)
                .OrderByDescending(r => r.StartDate)
                .Select(r => new
                {
                    r.Id,
                    VehicleId = r.VehicleId!.Value,
                    VehicleName = r.Vehicle!.Brand + " " + r.Vehicle.Model,
                    r.Vehicle.FuelType,
                    Start = r.StartDate!.Value,
                    End = r.EndDate!.Value
                })
                .ToListAsync();

            bookings = rows.Select(r => new MyBookingItemViewModel
            {
                Id = r.Id,
                VehicleId = r.VehicleId,
                VehicleName = r.VehicleName,
                FuelType = r.FuelType,
                StartDate = r.Start,
                EndDate = r.End,
                Status = RentalRules.GetStatus(r.Start, r.End, today)
            }).ToList();
        }

        return View(new MyBookingsViewModel
        {
            DisplayName = user.DisplayName,
            HasCustomerProfile = customer is not null,
            Bookings = bookings
        });
    }

    [HttpPost("conta/reservas/{id:int}/cancelar")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var cancelled = await bookingService.CancelAsync(user, id, RentalRules.Today());
        TempData[cancelled ? "SuccessMessage" : "ErrorMessage"] = cancelled
            ? T["booking.cancelled"].Value
            : T["booking.cancel_not_allowed"].Value;

        return RedirectToAction(nameof(MyBookings));
    }

    private async Task<VehicleDetailsViewModel?> BuildDetailsAsync(int id, BookingFormViewModel booking)
    {
        var today = RentalRules.Today();
        var vehicle = await ToCards(dbContext.Vehicles.AsNoTracking().Where(v => v.Id == id), today, today)
            .FirstOrDefaultAsync();
        if (vehicle is null)
        {
            return null;
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await userManager.GetUserAsync(User);
            booking.NeedsCustomerData = user is not null && await bookingService.FindCustomerAsync(user) is null;
        }

        var related = await ToCards(
                dbContext.Vehicles.AsNoTracking()
                    .Where(v => v.Id != id && (v.FuelType == vehicle.FuelType || v.Brand == vehicle.Brand))
                    .OrderByDescending(v => v.YearOfManufacture)
                    .Take(3),
                today, today)
            .ToListAsync();

        return new VehicleDetailsViewModel
        {
            Vehicle = vehicle,
            IsAvailableToday = vehicle.IsAvailable,
            BookedPeriods = await availability.BookedPeriodsAsync(id, today),
            Booking = booking,
            Related = related
        };
    }

    private static IQueryable<VehicleCardViewModel> ToCards(IQueryable<Vehicle> vehicles, DateOnly start, DateOnly end)
        => vehicles.Select(v => new VehicleCardViewModel
        {
            Id = v.Id,
            Brand = v.Brand,
            Model = v.Model,
            YearOfManufacture = v.YearOfManufacture,
            FuelType = v.FuelType,
            Mileage = v.Mileage,
            IsAvailable = !v.RentalContracts.Any(r => r.StartDate <= end && r.EndDate >= start)
        });
}
