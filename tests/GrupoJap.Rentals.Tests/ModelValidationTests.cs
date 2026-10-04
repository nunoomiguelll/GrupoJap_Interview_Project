using System.ComponentModel.DataAnnotations;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.ViewModels;

namespace GrupoJap.Rentals.Tests;

public class ModelValidationTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    private static Vehicle ValidVehicle() => new()
    {
        Brand = "Toyota", Model = "Corolla", Registration = "AA-00-BB",
        YearOfManufacture = 2020, FuelType = FuelType.Petrol
    };

    [Fact]
    public void Vehicle_Valid_HasNoErrors() => Assert.Empty(Validate(ValidVehicle()));

    [Fact]
    public void Vehicle_FutureYear_IsRejected()
    {
        var vehicle = ValidVehicle();
        vehicle.YearOfManufacture = DateTime.Today.Year + 1;
        Assert.Contains(Validate(vehicle), r => r.MemberNames.Contains(nameof(Vehicle.YearOfManufacture)));
    }

    [Fact]
    public void Vehicle_CurrentYear_IsAccepted()
    {
        var vehicle = ValidVehicle();
        vehicle.YearOfManufacture = DateTime.Today.Year;
        Assert.Empty(Validate(vehicle));
    }

    [Fact]
    public void Vehicle_UnspecifiedFuel_IsRejected()
    {
        var vehicle = ValidVehicle();
        vehicle.FuelType = FuelType.Unspecified;
        Assert.Contains(Validate(vehicle), r => r.MemberNames.Contains(nameof(Vehicle.FuelType)));
    }

    [Fact]
    public void Vehicle_MissingRequiredFields_AreRejected()
    {
        var errors = Validate(new Vehicle { YearOfManufacture = 2020, FuelType = FuelType.Diesel });
        Assert.Contains(errors, r => r.MemberNames.Contains(nameof(Vehicle.Brand)));
        Assert.Contains(errors, r => r.MemberNames.Contains(nameof(Vehicle.Model)));
        Assert.Contains(errors, r => r.MemberNames.Contains(nameof(Vehicle.Registration)));
    }

    [Theory]
    [InlineData("912345678", true)]
    [InlineData("961234567", true)]
    [InlineData("930000000", true)]
    [InlineData("212345678", false)]  // 9 dígitos mas não começa por 9
    [InlineData("812345678", false)]
    [InlineData("012345678", false)]
    [InlineData("91234567", false)]
    [InlineData("9123456789", false)]
    [InlineData("91234abcd", false)]
    [InlineData("912 345 678", false)]
    [InlineData("+35191234567", false)]
    public void Customer_Phone_MustBePortuguese_NineDigitsStartingWith9(string phone, bool valid)
    {
        var customer = new Customer { FullName = "Ana Silva", Email = "ana@example.com", Phone = phone, DrivingLicenseNumber = "L-123" };
        Assert.Equal(valid, !Validate(customer).Any(r => r.MemberNames.Contains(nameof(Customer.Phone))));
    }

    [Theory]
    [InlineData(null, true)]          // no perfil o telefone é opcional
    [InlineData("912345678", true)]
    [InlineData("212345678", false)]
    [InlineData("91234567", false)]
    public void Profile_Phone_FollowsTheSameRule(string? phone, bool valid)
    {
        var profile = new ProfileViewModel { FullName = "Ana Silva", PhoneNumber = phone };
        Assert.Equal(valid, !Validate(profile).Any(r => r.MemberNames.Contains(nameof(ProfileViewModel.PhoneNumber))));
    }

    [Fact]
    public void Customer_InvalidEmail_IsRejected()
    {
        var customer = new Customer { FullName = "Ana", Email = "not-an-email", Phone = "912345678", DrivingLicenseNumber = "L-1" };
        Assert.Contains(Validate(customer), r => r.MemberNames.Contains(nameof(Customer.Email)));
    }

    private static RentalContract Contract(DateOnly start, DateOnly end) => new()
    {
        CustomerId = 1, VehicleId = 1, StartDate = start, EndDate = end, InitialMileage = 100
    };

    [Fact]
    public void Rental_StartInPast_IsRejected()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var errors = Validate(Contract(today.AddDays(-1), today.AddDays(3)));
        Assert.Contains(errors, r => r.MemberNames.Contains(nameof(RentalContract.StartDate)));
    }

    [Fact]
    public void Rental_StartToday_IsAccepted()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        Assert.Empty(Validate(Contract(today, today.AddDays(1))));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Rental_EndNotAfterStart_IsRejected(int offsetDays)
    {
        var start = DateOnly.FromDateTime(DateTime.Today).AddDays(2);
        var errors = Validate(Contract(start, start.AddDays(offsetDays)));
        Assert.Contains(errors, r => r.MemberNames.Contains(nameof(RentalContract.EndDate)));
    }

    [Fact]
    public void Rental_NegativeMileage_IsRejected()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var contract = Contract(today, today.AddDays(1));
        contract.InitialMileage = -5;
        Assert.Contains(Validate(contract), r => r.MemberNames.Contains(nameof(RentalContract.InitialMileage)));
    }

    [Fact]
    public void Rental_MissingCustomerAndVehicle_AreRejected()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var errors = Validate(new RentalContract { StartDate = today, EndDate = today.AddDays(1), InitialMileage = 0 });
        Assert.Contains(errors, r => r.MemberNames.Contains(nameof(RentalContract.CustomerId)));
        Assert.Contains(errors, r => r.MemberNames.Contains(nameof(RentalContract.VehicleId)));
    }
}
