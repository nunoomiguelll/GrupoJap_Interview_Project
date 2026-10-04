using GrupoJap.Rentals.Services;

namespace GrupoJap.Rentals.ViewModels;

public sealed class RentalListItemViewModel
{
    public int Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string VehicleName { get; init; } = string.Empty;
    public string Registration { get; init; } = string.Empty;
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public int InitialMileage { get; init; }
    public RentalStatus Status { get; init; }
}
