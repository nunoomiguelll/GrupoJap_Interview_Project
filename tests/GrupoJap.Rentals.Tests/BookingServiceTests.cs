using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;
using GrupoJap.Rentals.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Tests;

public class BookingServiceTests
{
    private static readonly DateOnly Today = new(2030, 6, 10);

    private static ApplicationDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static (ApplicationDbContext Db, BookingService Service, Vehicle Vehicle) Setup()
    {
        var db = NewContext();
        var vehicle = new Vehicle { Brand = "Toyota", Model = "Yaris", Registration = "AA-00-AA", YearOfManufacture = 2022, FuelType = FuelType.Hybrid, Mileage = 12_345 };
        db.Vehicles.Add(vehicle);
        db.SaveChanges();
        return (db, new BookingService(db, new RentalAvailabilityService(db)), vehicle);
    }

    private static ApplicationUser User(string email = "ana@example.com") => new() { Email = email, UserName = email, FullName = "Ana Silva" };

    private static BookingFormViewModel Form(int vehicleId, int startOffset, int endOffset, string? phone = "912345678", string? license = "L-1") => new()
    {
        VehicleId = vehicleId,
        StartDate = Today.AddDays(startOffset),
        EndDate = Today.AddDays(endOffset),
        Phone = phone,
        DrivingLicenseNumber = license
    };

    [Fact]
    public async Task FirstBooking_CreatesCustomerAndContract_WithVehicleMileage()
    {
        var (db, service, vehicle) = Setup();

        var result = await service.CreateAsync(User(), Form(vehicle.Id, 1, 3), Today);

        Assert.True(result.Succeeded);
        var customer = Assert.Single(db.Customers);
        Assert.Equal("ana@example.com", customer.Email);
        Assert.Equal("Ana Silva", customer.FullName);
        var contract = Assert.Single(db.RentalContracts);
        Assert.Equal(12_345, contract.InitialMileage);
        Assert.Equal(customer.Id, contract.CustomerId);
    }

    [Fact]
    public async Task FirstBooking_WithoutPhoneOrLicense_IsRejected()
    {
        var (db, service, vehicle) = Setup();

        var result = await service.CreateAsync(User(), Form(vehicle.Id, 1, 3, phone: null, license: " "), Today);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Field == nameof(BookingFormViewModel.Phone));
        Assert.Contains(result.Errors, e => e.Field == nameof(BookingFormViewModel.DrivingLicenseNumber));
        Assert.Empty(db.RentalContracts);
    }

    [Fact]
    public async Task ExistingCustomer_IsReusedByEmail_CaseInsensitive()
    {
        var (db, service, vehicle) = Setup();
        db.Customers.Add(new Customer { FullName = "Ana", Email = "ana@example.com", Phone = "911111111", DrivingLicenseNumber = "X" });
        await db.SaveChangesAsync();

        var result = await service.CreateAsync(User("ANA@Example.com"), Form(vehicle.Id, 1, 2, phone: null, license: null), Today);

        Assert.True(result.Succeeded);
        Assert.Single(db.Customers);
    }

    [Theory]
    [InlineData(5, 8, false)]  // sobrepõe-se a 3..6
    [InlineData(6, 9, false)]  // partilha o dia 6 (datas inclusivas)
    [InlineData(1, 3, false)]  // partilha o dia 3
    [InlineData(7, 9, true)]   // começa no dia seguinte ao fim
    [InlineData(1, 2, true)]   // termina antes do início
    public async Task Overlap_IsCheckedWithInclusiveDates(int start, int end, bool expectedSuccess)
    {
        var (_, service, vehicle) = Setup();
        Assert.True((await service.CreateAsync(User(), Form(vehicle.Id, 3, 6), Today)).Succeeded);

        var result = await service.CreateAsync(User("bruno@example.com"), Form(vehicle.Id, start, end), Today);

        Assert.Equal(expectedSuccess, result.Succeeded);
    }

    [Fact]
    public async Task StartInPast_IsRejected()
    {
        var (_, service, vehicle) = Setup();
        var result = await service.CreateAsync(User(), Form(vehicle.Id, -1, 2), Today);
        Assert.Contains(result.Errors, e => e.Field == nameof(BookingFormViewModel.StartDate));
    }

    [Fact]
    public async Task StartToday_IsAccepted()
    {
        var (_, service, vehicle) = Setup();
        Assert.True((await service.CreateAsync(User(), Form(vehicle.Id, 0, 1), Today)).Succeeded);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task EndNotAfterStart_IsRejected(int lengthDays)
    {
        var (_, service, vehicle) = Setup();
        var result = await service.CreateAsync(User(), Form(vehicle.Id, 2, 2 + lengthDays), Today);
        Assert.Contains(result.Errors, e => e.Field == nameof(BookingFormViewModel.EndDate));
    }

    [Fact]
    public async Task UnknownVehicle_IsRejected()
    {
        var (_, service, _) = Setup();
        var result = await service.CreateAsync(User(), Form(999, 1, 2), Today);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Cancel_OwnUpcomingBooking_Succeeds()
    {
        var (db, service, vehicle) = Setup();
        var created = await service.CreateAsync(User(), Form(vehicle.Id, 2, 4), Today);

        Assert.True(await service.CancelAsync(User(), created.ContractId!.Value, Today));
        Assert.Empty(db.RentalContracts);
    }

    [Fact]
    public async Task Cancel_SomeoneElsesBooking_Fails()
    {
        var (db, service, vehicle) = Setup();
        var created = await service.CreateAsync(User(), Form(vehicle.Id, 2, 4), Today);

        Assert.False(await service.CancelAsync(User("intruso@example.com"), created.ContractId!.Value, Today));
        Assert.Single(db.RentalContracts);
    }

    [Fact]
    public async Task Cancel_BookingThatAlreadyStarted_Fails()
    {
        var (db, service, vehicle) = Setup();
        var created = await service.CreateAsync(User(), Form(vehicle.Id, 0, 3), Today);

        Assert.False(await service.CancelAsync(User(), created.ContractId!.Value, Today));
        Assert.Single(db.RentalContracts);
    }

    [Fact]
    public async Task AvailableBetween_ExcludesVehiclesWithOverlappingContracts()
    {
        var (db, service, busy) = Setup();
        var free = new Vehicle { Brand = "Renault", Model = "Clio", Registration = "BB-11-BB", YearOfManufacture = 2021, FuelType = FuelType.Petrol };
        db.Vehicles.Add(free);
        await db.SaveChangesAsync();
        await service.CreateAsync(User(), Form(busy.Id, 1, 5), Today);

        var availability = new RentalAvailabilityService(db);
        var ids = await availability.AvailableBetween(db.Vehicles, Today.AddDays(4), Today.AddDays(6)).Select(v => v.Id).ToListAsync();

        Assert.Equal([free.Id], ids);
        Assert.True(await availability.HasOverlapAsync(busy.Id, Today.AddDays(5), Today.AddDays(7)));
        Assert.False(await availability.HasOverlapAsync(busy.Id, Today.AddDays(6), Today.AddDays(7)));
    }
}
