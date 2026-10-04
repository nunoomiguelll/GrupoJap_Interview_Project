using GrupoJap.Rentals.Controllers;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GrupoJap.Rentals.Tests.Controllers;

public sealed class VehiclesControllerTests : IDisposable
{
    private readonly ControllerTestContext _context = new();

    private VehiclesController NewController() => _context.Setup(new VehiclesController(_context.Db, _context.T));

    private async Task<IActionResult> PostCreate(Vehicle vehicle)
    {
        var controller = NewController();
        _context.Validate(controller, vehicle);
        return await controller.Create(vehicle);
    }

    [Fact]
    public async Task Create_Valid_SavesNormalizedAndRedirects()
    {
        var result = await PostCreate(TestData.Vehicle(registration: "  aa-11-cc ", brand: " Renault ", model: " Clio "));

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(VehiclesController.Index), redirect.ActionName);
        var saved = Assert.Single(_context.Query(db => db.Vehicles.ToList()));
        Assert.Equal("AA-11-CC", saved.Registration);
        Assert.Equal("Renault", saved.Brand);
        Assert.Equal("Clio", saved.Model);
    }

    [Fact]
    public async Task Create_DuplicateRegistration_IsRejectedEvenWithDifferentCase()
    {
        _context.Seed(TestData.Vehicle(registration: "AA-00-BB"));

        var result = await PostCreate(TestData.Vehicle(registration: "aa-00-bb"));

        Assert.IsType<ViewResult>(result);
        Assert.Contains(nameof(Vehicle.Registration), ErrorKeys(result));
        Assert.Equal(1, _context.Query(db => db.Vehicles.Count()));
    }

    [Fact]
    public async Task Create_FutureYear_IsRejectedAndNotSaved()
    {
        var vehicle = TestData.Vehicle();
        vehicle.YearOfManufacture = DateTime.Today.Year + 1;

        var result = await PostCreate(vehicle);

        Assert.IsType<ViewResult>(result);
        Assert.Contains(nameof(Vehicle.YearOfManufacture), ErrorKeys(result));
        Assert.Equal(0, _context.Query(db => db.Vehicles.Count()));
    }

    [Fact]
    public async Task Edit_KeepingItsOwnRegistration_IsAllowed()
    {
        var vehicle = TestData.Vehicle(registration: "AA-00-BB");
        _context.Seed(vehicle);

        var changes = TestData.Vehicle(registration: "AA-00-BB", model: "Yaris");
        changes.Id = vehicle.Id;
        var controller = NewController();
        var result = await controller.Edit(vehicle.Id, changes);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Yaris", _context.Query(db => db.Vehicles.Single().Model));
    }

    [Fact]
    public async Task Edit_ToAnotherVehiclesRegistration_IsRejected()
    {
        var first = TestData.Vehicle(registration: "AA-00-BB");
        var second = TestData.Vehicle(registration: "CC-11-DD");
        _context.Seed(first, second);

        var changes = TestData.Vehicle(registration: "AA-00-BB");
        changes.Id = second.Id;
        var result = await NewController().Edit(second.Id, changes);

        Assert.IsType<ViewResult>(result);
        Assert.Contains(nameof(Vehicle.Registration), ErrorKeys(result));
        Assert.Equal("CC-11-DD", _context.Query(db => db.Vehicles.Single(v => v.Id == second.Id).Registration));
    }

    [Fact]
    public async Task Edit_UnknownOrMismatchedId_ReturnsNotFound()
    {
        var changes = TestData.Vehicle();
        changes.Id = 99;

        Assert.IsType<NotFoundResult>(await NewController().Edit(99, changes));
        Assert.IsType<NotFoundResult>(await NewController().Edit(1, changes));
    }

    [Fact]
    public async Task Delete_WithContracts_IsBlocked()
    {
        var vehicle = TestData.Vehicle();
        _context.Seed(TestData.Contract(TestData.Customer(), vehicle, -10, -5));

        var controller = NewController();
        var result = await controller.DeleteConfirmed(vehicle.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(_context.T["msg.vehicle_delete_blocked"].Value, controller.TempData["ErrorMessage"]);
        Assert.Equal(1, _context.Query(db => db.Vehicles.Count()));
    }

    [Fact]
    public async Task Delete_WithoutContracts_RemovesTheVehicle()
    {
        var vehicle = TestData.Vehicle();
        _context.Seed(vehicle);

        var controller = NewController();
        await controller.DeleteConfirmed(vehicle.Id);

        Assert.Equal(_context.T["msg.vehicle_deleted"].Value, controller.TempData["SuccessMessage"]);
        Assert.Equal(0, _context.Query(db => db.Vehicles.Count()));
    }

    [Fact]
    public async Task Index_MarksAsRentedOnlyVehiclesWithAContractToday()
    {
        var customer = TestData.Customer();
        var rentedNow = TestData.Vehicle(registration: "AA-00-01");
        var endsToday = TestData.Vehicle(registration: "AA-00-02");
        var futureOnly = TestData.Vehicle(registration: "AA-00-03");
        var pastOnly = TestData.Vehicle(registration: "AA-00-04");
        var neverRented = TestData.Vehicle(registration: "AA-00-05");
        _context.Seed(
            TestData.Contract(customer, rentedNow, -2, 2),
            TestData.Contract(customer, endsToday, -3, 0),
            TestData.Contract(customer, futureOnly, 1, 4),
            TestData.Contract(customer, pastOnly, -9, -1),
            neverRented);

        var view = Assert.IsType<ViewResult>(await NewController().Index());
        var rows = Assert.IsAssignableFrom<IEnumerable<VehicleListItemViewModel>>(view.Model).ToDictionary(v => v.Registration);

        Assert.True(rows["AA-00-01"].IsRented);
        Assert.True(rows["AA-00-02"].IsRented);
        Assert.False(rows["AA-00-03"].IsRented);
        Assert.False(rows["AA-00-04"].IsRented);
        Assert.False(rows["AA-00-05"].IsRented);
    }

    private static IEnumerable<string> ErrorKeys(IActionResult result)
        => Assert.IsType<ViewResult>(result).ViewData.ModelState.Where(e => e.Value!.Errors.Count > 0).Select(e => e.Key);

    public void Dispose() => _context.Dispose();
}
