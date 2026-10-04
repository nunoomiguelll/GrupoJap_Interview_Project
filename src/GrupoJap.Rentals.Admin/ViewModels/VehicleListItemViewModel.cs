using GrupoJap.Rentals.Models;

namespace GrupoJap.Rentals.ViewModels;

public sealed class VehicleListItemViewModel
{
    public int Id { get; init; }

    public string Brand { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public string Registration { get; init; } = string.Empty;

    public int YearOfManufacture { get; init; }

    public FuelType FuelType { get; init; }

    public bool IsRented { get; init; }
}