using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Tests;

public class RentalAvailabilityServiceTests
{
    private static readonly DateOnly Today = new(2030, 6, 10);

    private static async Task<(RentalAvailabilityService Service, Vehicle Vehicle, RentalContract Contract)> SetupAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new ApplicationDbContext(options);

        var vehicle = new Vehicle { Brand = "Toyota", Model = "Yaris", Registration = "AA-00-AA", YearOfManufacture = 2022, FuelType = FuelType.Hybrid, Mileage = 12_345 };
        var customer = new Customer { FullName = "Ana Silva", Email = "ana@example.com", Phone = "912345678", DrivingLicenseNumber = "L-1" };
        var contract = new RentalContract { Vehicle = vehicle, Customer = customer, StartDate = Today.AddDays(1), EndDate = Today.AddDays(5), InitialMileage = 12_345 };
        db.RentalContracts.Add(contract);
        await db.SaveChangesAsync();

        return (new RentalAvailabilityService(db), vehicle, contract);
    }

    [Fact]
    public async Task HasOverlap_IsInclusiveOnBothEnds()
    {
        var (service, vehicle, _) = await SetupAsync();

        Assert.True(await service.HasOverlapAsync(vehicle.Id, Today.AddDays(5), Today.AddDays(7)));
        Assert.True(await service.HasOverlapAsync(vehicle.Id, Today.AddDays(-2), Today.AddDays(1)));
        Assert.False(await service.HasOverlapAsync(vehicle.Id, Today.AddDays(6), Today.AddDays(7)));
        Assert.False(await service.HasOverlapAsync(vehicle.Id, Today.AddDays(-3), Today));
    }

    [Fact]
    public async Task HasOverlap_IgnoresTheContractBeingEdited()
    {
        var (service, vehicle, contract) = await SetupAsync();

        Assert.False(await service.HasOverlapAsync(vehicle.Id, Today.AddDays(2), Today.AddDays(4), excludeContractId: contract.Id));
    }
}
