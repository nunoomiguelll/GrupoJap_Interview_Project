using GrupoJap.Rentals.Controllers;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GrupoJap.Rentals.Tests.Controllers;

public sealed class CustomersControllerTests : IDisposable
{
    private readonly ControllerTestContext _context = new();

    private CustomersController NewController() => _context.Setup(new CustomersController(_context.Db, _context.T));

    private async Task<IActionResult> PostCreate(Customer customer)
    {
        var controller = NewController();
        _context.Validate(controller, customer);
        return await controller.Create(customer);
    }

    [Fact]
    public async Task Create_Valid_SavesNormalizedAndRedirects()
    {
        var customer = TestData.Customer(email: " Ana.Silva@Example.COM ", name: "  Ana Silva ");
        customer.DrivingLicenseNumber = " l-123 ";

        var result = await PostCreate(customer);

        Assert.IsType<RedirectToActionResult>(result);
        var saved = Assert.Single(_context.Query(db => db.Customers.ToList()));
        Assert.Equal("ana.silva@example.com", saved.Email);
        Assert.Equal("Ana Silva", saved.FullName);
        Assert.Equal("L-123", saved.DrivingLicenseNumber);
    }

    [Fact]
    public async Task Create_DuplicateEmail_IsRejectedEvenWithDifferentCase()
    {
        _context.Seed(TestData.Customer(email: "ana@example.com"));

        var result = await PostCreate(TestData.Customer(email: "ANA@Example.com", name: "Outra Ana"));

        var view = Assert.IsType<ViewResult>(result);
        var error = Assert.Single(view.ViewData.ModelState[nameof(Customer.Email)]!.Errors);
        Assert.Equal(_context.T["validation.customer_duplicate"].Value, error.ErrorMessage);
        Assert.Equal(1, _context.Query(db => db.Customers.Count()));
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    public async Task Create_InvalidEmail_IsRejected(string email)
    {
        var result = await PostCreate(TestData.Customer(email: email));

        var view = Assert.IsType<ViewResult>(result);
        Assert.False(view.ViewData.ModelState.IsValid);
        Assert.Equal(0, _context.Query(db => db.Customers.Count()));
    }

    [Theory]
    [InlineData("91234567")]
    [InlineData("91234567a")]
    [InlineData("+351912345678")]
    public async Task Create_PhoneOutsideAgreedFormat_IsRejected(string phone)
    {
        var customer = TestData.Customer();
        customer.Phone = phone;

        var result = await PostCreate(customer);

        var view = Assert.IsType<ViewResult>(result);
        Assert.True(view.ViewData.ModelState[nameof(Customer.Phone)]!.Errors.Count > 0);
        Assert.Equal(0, _context.Query(db => db.Customers.Count()));
    }

    [Fact]
    public async Task Edit_KeepingItsOwnEmail_IsAllowed()
    {
        var customer = TestData.Customer(email: "ana@example.com");
        _context.Seed(customer);

        var changes = TestData.Customer(email: "ana@example.com", name: "Ana Maria Silva");
        changes.Id = customer.Id;
        var result = await NewController().Edit(customer.Id, changes);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Ana Maria Silva", _context.Query(db => db.Customers.Single().FullName));
    }

    [Fact]
    public async Task Edit_ToAnotherCustomersEmail_IsRejected()
    {
        var ana = TestData.Customer(email: "ana@example.com");
        var rui = TestData.Customer(email: "rui@example.com", name: "Rui Costa");
        _context.Seed(ana, rui);

        var changes = TestData.Customer(email: "ana@example.com", name: "Rui Costa");
        changes.Id = rui.Id;
        var result = await NewController().Edit(rui.Id, changes);

        var view = Assert.IsType<ViewResult>(result);
        Assert.True(view.ViewData.ModelState[nameof(Customer.Email)]!.Errors.Count > 0);
        Assert.Equal("rui@example.com", _context.Query(db => db.Customers.Single(c => c.Id == rui.Id).Email));
    }

    [Fact]
    public async Task Delete_WithContracts_IsBlocked()
    {
        var customer = TestData.Customer();
        _context.Seed(TestData.Contract(customer, TestData.Vehicle(), 2, 5));

        var controller = NewController();
        await controller.DeleteConfirmed(customer.Id);

        Assert.Equal(_context.T["msg.customer_delete_blocked"].Value, controller.TempData["ErrorMessage"]);
        Assert.Equal(1, _context.Query(db => db.Customers.Count()));
    }

    [Fact]
    public async Task Delete_WithoutContracts_RemovesTheCustomer()
    {
        var customer = TestData.Customer();
        _context.Seed(customer);

        var controller = NewController();
        await controller.DeleteConfirmed(customer.Id);

        Assert.Equal(_context.T["msg.customer_deleted"].Value, controller.TempData["SuccessMessage"]);
        Assert.Equal(0, _context.Query(db => db.Customers.Count()));
    }

    [Fact]
    public async Task Index_ShowsContractCountAndWhetherARentalIsActiveToday()
    {
        var active = TestData.Customer(email: "ativo@example.com", name: "A Ativo");
        var futureOnly = TestData.Customer(email: "futuro@example.com", name: "B Futuro");
        var none = TestData.Customer(email: "nenhum@example.com", name: "C Nenhum");
        _context.Seed(
            TestData.Contract(active, TestData.Vehicle(registration: "AA-00-01"), -1, 1),
            TestData.Contract(active, TestData.Vehicle(registration: "AA-00-02"), -20, -10),
            TestData.Contract(futureOnly, TestData.Vehicle(registration: "AA-00-03"), 3, 6),
            none);

        var view = Assert.IsType<ViewResult>(await NewController().Index());
        var rows = Assert.IsAssignableFrom<IEnumerable<CustomerListItemViewModel>>(view.Model).ToDictionary(c => c.Email);

        Assert.Equal((2, true), (rows["ativo@example.com"].ContractCount, rows["ativo@example.com"].HasActiveRental));
        Assert.Equal((1, false), (rows["futuro@example.com"].ContractCount, rows["futuro@example.com"].HasActiveRental));
        Assert.Equal((0, false), (rows["nenhum@example.com"].ContractCount, rows["nenhum@example.com"].HasActiveRental));
    }

    public void Dispose() => _context.Dispose();
}
