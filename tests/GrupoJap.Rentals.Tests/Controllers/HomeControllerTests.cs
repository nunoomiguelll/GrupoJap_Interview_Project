using GrupoJap.Rentals.Controllers;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GrupoJap.Rentals.Tests.Controllers;

public sealed class HomeControllerTests : IDisposable
{
    private readonly ControllerTestContext _context = new();

    private async Task<DashboardViewModel> Dashboard()
    {
        var controller = _context.Setup(new HomeController(_context.Db));
        var view = Assert.IsType<ViewResult>(await controller.Index());
        return Assert.IsType<DashboardViewModel>(view.Model);
    }

    [Fact]
    public async Task Index_EmptyFleet_ShowsZeroesWithoutDividingByZero()
    {
        var model = await Dashboard();

        Assert.Equal(0, model.VehicleCount);
        Assert.Equal(0, model.AvailabilityPercentage);
        Assert.Empty(model.RecentRentals);
    }

    [Fact]
    public async Task Index_CountsAvailabilityFromTodaysContracts()
    {
        var ana = TestData.Customer(email: "ana@example.com");
        var rui = TestData.Customer(email: "rui@example.com", name: "Rui Costa");
        var rented = TestData.Vehicle(registration: "AA-00-01");
        _context.Seed(
            // Dois contratos ativos hoje no mesmo veículo contam como um só veículo alugado.
            TestData.Contract(ana, rented, -2, 0),
            TestData.Contract(rui, rented, 0, 3),
            TestData.Contract(ana, TestData.Vehicle(registration: "AA-00-02"), 4, 6),
            TestData.Vehicle(registration: "AA-00-03"),
            TestData.Vehicle(registration: "AA-00-04"));

        var model = await Dashboard();

        Assert.Equal(4, model.VehicleCount);
        Assert.Equal(3, model.AvailableVehicleCount);
        Assert.Equal(2, model.ActiveRentalCount);
        Assert.Equal(2, model.CustomerCount);
        Assert.Equal(75, model.AvailabilityPercentage);
    }

    [Fact]
    public async Task Index_ShowsTheFiveMostRecentContractsWithTheirStatus()
    {
        var customer = TestData.Customer();
        var vehicle = TestData.Vehicle();
        _context.Seed(Enumerable.Range(0, 7)
            .Select(i => (object)TestData.Contract(customer, vehicle, i * 10 - 30, i * 10 - 25))
            .ToArray());

        var recent = (await Dashboard()).RecentRentals;

        Assert.Equal(5, recent.Count);
        Assert.Equal(recent.OrderByDescending(r => r.StartDate).Select(r => r.StartDate), recent.Select(r => r.StartDate));
        Assert.Equal(TestData.Today.AddDays(30), recent[0].StartDate);
        Assert.All(recent, r => Assert.Equal(r.StartDate > TestData.Today, r.IsUpcoming));
        Assert.All(recent, r => Assert.Equal(r.StartDate <= TestData.Today && r.EndDate >= TestData.Today, r.IsActive));
    }

    public void Dispose() => _context.Dispose();
}
