using GrupoJap.Rentals.Controllers;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Services;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GrupoJap.Rentals.Tests.Controllers;

public sealed class RentalsControllerTests : IDisposable
{
    private readonly ControllerTestContext _context = new();
    private readonly Customer _customer = TestData.Customer();
    private readonly Vehicle _vehicle = TestData.Vehicle(registration: "AA-00-01");
    private readonly Vehicle _otherVehicle = TestData.Vehicle(registration: "AA-00-02");

    public RentalsControllerTests() => _context.Seed(_customer, _vehicle, _otherVehicle);

    private RentalsController NewController() => _context.Setup(new RentalsController(_context.Db, _context.Availability, _context.T));

    private RentalContract Form(int startOffset, int endOffset, Vehicle? vehicle = null, int? id = null) => new()
    {
        Id = id ?? 0,
        CustomerId = _customer.Id,
        VehicleId = (vehicle ?? _vehicle).Id,
        StartDate = TestData.Today.AddDays(startOffset),
        EndDate = TestData.Today.AddDays(endOffset),
        InitialMileage = 10_000
    };

    private async Task<IActionResult> PostCreate(RentalContract form)
    {
        var controller = NewController();
        _context.Validate(controller, form);
        return await controller.Create(form);
    }

    private async Task<IActionResult> PostEdit(int id, RentalContract form)
    {
        var controller = NewController();
        _context.Validate(controller, form);
        return await controller.Edit(id, form);
    }

    /// <summary>Contrato já gravado para o veículo principal, entre os dias indicados (a contar de hoje).</summary>
    private RentalContract SeedContract(int startOffset, int endOffset)
    {
        var contract = new RentalContract
        {
            CustomerId = _customer.Id,
            VehicleId = _vehicle.Id,
            StartDate = TestData.Today.AddDays(startOffset),
            EndDate = TestData.Today.AddDays(endOffset),
            InitialMileage = 10_000
        };
        _context.Seed(contract);
        return contract;
    }

    private static string[] ErrorsFor(IActionResult result, string key)
        => Assert.IsType<ViewResult>(result).ViewData.ModelState[key]?.Errors.Select(e => e.ErrorMessage).ToArray() ?? [];

    [Fact]
    public async Task Create_Valid_SavesAndRedirects()
    {
        var result = await PostCreate(Form(0, 3));

        Assert.IsType<RedirectToActionResult>(result);
        var saved = Assert.Single(_context.Query(db => db.RentalContracts.ToList()));
        Assert.Equal(TestData.Today, saved.StartDate);
    }

    [Theory]
    [InlineData(5, 8)]   // começa no último dia do contrato existente (datas inclusivas)
    [InlineData(0, 2)]   // termina no primeiro dia do contrato existente
    [InlineData(3, 4)]   // fica dentro do contrato existente
    [InlineData(1, 9)]   // envolve o contrato existente
    public async Task Create_OverlappingPeriodOnSameVehicle_IsRejected(int start, int end)
    {
        SeedContract(2, 5);

        var result = await PostCreate(Form(start, end));

        Assert.Equal([_context.T["validation.rental_overlap"].Value], ErrorsFor(result, nameof(RentalContract.VehicleId)));
        Assert.Equal(1, _context.Query(db => db.RentalContracts.Count()));
    }

    [Theory]
    [InlineData(6, 8)]   // começa no dia seguinte ao fim
    [InlineData(0, 1)]   // termina na véspera do início
    public async Task Create_AdjacentPeriodOnSameVehicle_IsAccepted(int start, int end)
    {
        SeedContract(2, 5);

        Assert.IsType<RedirectToActionResult>(await PostCreate(Form(start, end)));
        Assert.Equal(2, _context.Query(db => db.RentalContracts.Count()));
    }

    [Fact]
    public async Task Create_SamePeriodOnAnotherVehicle_IsAccepted()
    {
        SeedContract(2, 5);

        Assert.IsType<RedirectToActionResult>(await PostCreate(Form(2, 5, _otherVehicle)));
    }

    [Fact]
    public async Task Create_UnknownCustomerAndVehicle_AreRejected()
    {
        var form = Form(0, 3);
        form.CustomerId = 999;
        form.VehicleId = 999;

        var result = await PostCreate(form);

        Assert.Contains(_context.T["validation.customer_missing"].Value, ErrorsFor(result, nameof(RentalContract.CustomerId)));
        Assert.Contains(_context.T["validation.vehicle_missing"].Value, ErrorsFor(result, nameof(RentalContract.VehicleId)));
        Assert.Equal(0, _context.Query(db => db.RentalContracts.Count()));
    }

    [Fact]
    public async Task Create_WithoutCustomerOrVehicle_IsRejected()
    {
        var form = Form(0, 3);
        form.CustomerId = null;
        form.VehicleId = null;

        var result = await PostCreate(form);

        Assert.NotEmpty(ErrorsFor(result, nameof(RentalContract.CustomerId)));
        Assert.NotEmpty(ErrorsFor(result, nameof(RentalContract.VehicleId)));
    }

    [Fact]
    public async Task Create_StartInThePast_IsRejected()
    {
        var result = await PostCreate(Form(-1, 3));

        Assert.NotEmpty(ErrorsFor(result, nameof(RentalContract.StartDate)));
        Assert.Equal(0, _context.Query(db => db.RentalContracts.Count()));
    }

    [Theory]
    [InlineData(3, 3)]
    [InlineData(3, 1)]
    public async Task Create_EndNotAfterStart_IsRejected(int start, int end)
    {
        var result = await PostCreate(Form(start, end));

        Assert.NotEmpty(ErrorsFor(result, nameof(RentalContract.EndDate)));
    }

    [Fact]
    public async Task Create_NegativeInitialMileage_IsRejected()
    {
        var form = Form(0, 3);
        form.InitialMileage = -1;

        var result = await PostCreate(form);

        Assert.NotEmpty(ErrorsFor(result, nameof(RentalContract.InitialMileage)));
    }

    [Fact]
    public async Task Create_Invalid_ReloadsTheSelectLists()
    {
        var result = await PostCreate(Form(-1, 3));

        var view = Assert.IsType<ViewResult>(result);
        Assert.NotNull(view.ViewData["Customers"]);
        Assert.NotNull(view.ViewData["Vehicles"]);
    }

    [Fact]
    public async Task Edit_DoesNotConflictWithItself()
    {
        var contract = SeedContract(2, 5);

        var result = await PostEdit(contract.Id, Form(2, 7, id: contract.Id));

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(TestData.Today.AddDays(7), _context.Query(db => db.RentalContracts.Single().EndDate));
    }

    [Fact]
    public async Task Edit_IntoAnotherContractsPeriod_IsRejected()
    {
        SeedContract(10, 12);
        var contract = SeedContract(2, 5);

        var result = await PostEdit(contract.Id, Form(2, 10, id: contract.Id));

        Assert.Contains(_context.T["validation.rental_overlap"].Value, ErrorsFor(result, nameof(RentalContract.VehicleId)));
    }

    [Fact]
    public async Task Edit_StartedContract_KeepingTheOriginalStartDate_IsAllowed()
    {
        var contract = SeedContract(-3, 2);

        var result = await PostEdit(contract.Id, Form(-3, 4, id: contract.Id));

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(TestData.Today.AddDays(4), _context.Query(db => db.RentalContracts.Single().EndDate));
    }

    [Fact]
    public async Task Edit_StartedContract_MovingTheStartToAnotherPastDate_IsRejected()
    {
        var contract = SeedContract(-3, 2);

        var result = await PostEdit(contract.Id, Form(-5, 2, id: contract.Id));

        Assert.NotEmpty(ErrorsFor(result, nameof(RentalContract.StartDate)));
        Assert.Equal(TestData.Today.AddDays(-3), _context.Query(db => db.RentalContracts.Single().StartDate));
    }

    [Fact]
    public async Task Delete_RemovesTheContract()
    {
        var contract = SeedContract(2, 5);

        var controller = NewController();
        await controller.DeleteConfirmed(contract.Id);

        Assert.Equal(_context.T["msg.rental_deleted"].Value, controller.TempData["SuccessMessage"]);
        Assert.Equal(0, _context.Query(db => db.RentalContracts.Count()));
    }

    [Fact]
    public async Task Index_ComputesTheStatusFromToday()
    {
        var past = SeedContract(-10, -5);
        var current = SeedContract(-1, 1);
        var upcoming = SeedContract(5, 8);

        var view = Assert.IsType<ViewResult>(await NewController().Index());
        var rows = Assert.IsAssignableFrom<IEnumerable<RentalListItemViewModel>>(view.Model).ToDictionary(r => r.Id);

        Assert.Equal(RentalStatus.Completed, rows[past.Id].Status);
        Assert.Equal(RentalStatus.Active, rows[current.Id].Status);
        Assert.Equal(RentalStatus.Upcoming, rows[upcoming.Id].Status);
    }

    public void Dispose() => _context.Dispose();
}
