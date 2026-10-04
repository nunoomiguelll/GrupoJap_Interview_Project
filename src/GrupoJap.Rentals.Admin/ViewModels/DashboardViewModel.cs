namespace GrupoJap.Rentals.ViewModels;

public sealed class DashboardViewModel
{
    public int VehicleCount { get; init; }

    public int AvailableVehicleCount { get; init; }

    public int ActiveRentalCount { get; init; }

    public int CustomerCount { get; init; }

    public IReadOnlyList<RecentRentalViewModel> RecentRentals { get; init; } = [];

    public int AvailabilityPercentage => VehicleCount == 0
        ? 0
        : (int)Math.Round(AvailableVehicleCount * 100d / VehicleCount);
}

public sealed class RecentRentalViewModel
{
    public int Id { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public string VehicleName { get; init; } = string.Empty;

    public string Registration { get; init; } = string.Empty;

    public DateOnly StartDate { get; init; }

    public DateOnly EndDate { get; init; }

    public bool IsActive { get; init; }

    public bool IsUpcoming { get; init; }
}