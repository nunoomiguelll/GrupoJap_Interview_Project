using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;

namespace GrupoJap.Rentals.Tests.Controllers;

/// <summary>Entidades válidas por omissão; cada teste altera só o que interessa.</summary>
internal static class TestData
{
    public static DateOnly Today => RentalRules.Today();

    public static Vehicle Vehicle(string registration = "AA-00-BB", string brand = "Toyota", string model = "Corolla") => new()
    {
        Brand = brand,
        Model = model,
        Registration = registration,
        YearOfManufacture = 2020,
        FuelType = FuelType.Petrol,
        Mileage = 10_000
    };

    public static Customer Customer(string email = "ana@example.com", string name = "Ana Silva") => new()
    {
        FullName = name,
        Email = email,
        Phone = "912345678",
        DrivingLicenseNumber = "L-123"
    };

    /// <summary>Contrato entre os dias <paramref name="startOffset"/> e <paramref name="endOffset"/> a contar de hoje.</summary>
    public static RentalContract Contract(Customer customer, Vehicle vehicle, int startOffset, int endOffset) => new()
    {
        Customer = customer,
        Vehicle = vehicle,
        StartDate = Today.AddDays(startOffset),
        EndDate = Today.AddDays(endOffset),
        InitialMileage = 10_000
    };
}
