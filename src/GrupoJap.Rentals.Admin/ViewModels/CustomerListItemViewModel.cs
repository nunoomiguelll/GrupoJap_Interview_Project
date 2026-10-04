namespace GrupoJap.Rentals.ViewModels;

public sealed class CustomerListItemViewModel
{
    public int Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string DrivingLicenseNumber { get; init; } = string.Empty;
    public int ContractCount { get; init; }
    public bool HasActiveRental { get; init; }
}
