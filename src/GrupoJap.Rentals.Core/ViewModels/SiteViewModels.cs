using System.ComponentModel.DataAnnotations;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;

namespace GrupoJap.Rentals.ViewModels;

public sealed class VehicleCardViewModel
{
    public int Id { get; init; }
    public string Brand { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public int YearOfManufacture { get; init; }
    public FuelType FuelType { get; init; }
    public int Mileage { get; init; }

    /// <summary>Disponível no período pesquisado (ou hoje, se não houver período).</summary>
    public bool IsAvailable { get; init; }
}

public sealed class SiteHomeViewModel
{
    public int VehicleCount { get; init; }
    public int AvailableTodayCount { get; init; }
    public int DistinctBrandCount { get; init; }
    public IReadOnlyList<VehicleCardViewModel> Featured { get; init; } = [];
    public IReadOnlyList<BrandCount> Brands { get; init; } = [];
    public IReadOnlyList<FuelCount> Fuels { get; init; } = [];
}

public sealed record BrandCount(string Brand, int Count);

public sealed record FuelCount(FuelType FuelType, int Count);

public enum FleetSort
{
    [Display(Name = "Marca (A-Z)")] Brand = 0,
    [Display(Name = "Mais recentes")] Newest = 1,
    [Display(Name = "Menos quilómetros")] LowestMileage = 2
}

public sealed class FleetViewModel
{
    public const int PageSize = 12;

    // Filtros (query string)
    public string? Q { get; set; }
    public string? Brand { get; set; }
    public FuelType? Fuel { get; set; }
    public DateOnly? Start { get; set; }
    public DateOnly? End { get; set; }
    public bool OnlyAvailable { get; set; }
    public FleetSort Sort { get; set; }
    public int Page { get; set; } = 1;

    // Resultados
    public IReadOnlyList<VehicleCardViewModel> Vehicles { get; set; } = [];
    public IReadOnlyList<string> BrandOptions { get; set; } = [];
    public int TotalCount { get; set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public string? PeriodError { get; set; }

    /// <summary>Período efetivamente usado para calcular disponibilidade.</summary>
    public bool HasPeriod => Start is not null && End is not null && PeriodError is null;
}

public sealed class BookingFormViewModel
{
    public int VehicleId { get; set; }

    [Required(ErrorMessage = "validation.start_required")]
    [Display(Name = "field.start_date")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "validation.end_required")]
    [Display(Name = "field.end_date")]
    public DateOnly? EndDate { get; set; }

    // Só pedidos quando o utilizador ainda não tem ficha de cliente.
    [RegularExpression("^[0-9]{9}$", ErrorMessage = "validation.phone_format")]
    [Display(Name = "field.phone")]
    public string? Phone { get; set; }

    [StringLength(30)]
    [Display(Name = "field.driving_license")]
    public string? DrivingLicenseNumber { get; set; }

    public bool NeedsCustomerData { get; set; }
}

public sealed class VehicleDetailsViewModel
{
    public VehicleCardViewModel Vehicle { get; init; } = new();
    public IReadOnlyList<(DateOnly Start, DateOnly End)> BookedPeriods { get; init; } = [];
    public bool IsAvailableToday { get; init; }
    public BookingFormViewModel Booking { get; init; } = new();
    public IReadOnlyList<VehicleCardViewModel> Related { get; init; } = [];
}

public sealed class MyBookingItemViewModel
{
    public int Id { get; init; }
    public int VehicleId { get; init; }
    public string VehicleName { get; init; } = string.Empty;
    public FuelType FuelType { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public RentalStatus Status { get; init; }
    public int Days => EndDate.DayNumber - StartDate.DayNumber + 1;
    public bool CanCancel => Status == RentalStatus.Upcoming;
}

public sealed class MyBookingsViewModel
{
    public string DisplayName { get; init; } = string.Empty;
    public bool HasCustomerProfile { get; init; }
    public IReadOnlyList<MyBookingItemViewModel> Bookings { get; init; } = [];
}
