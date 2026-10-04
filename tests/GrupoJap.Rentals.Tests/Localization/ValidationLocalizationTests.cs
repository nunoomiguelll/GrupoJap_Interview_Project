using System.ComponentModel.DataAnnotations;
using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Tests.Controllers;

namespace GrupoJap.Rentals.Tests.Localization;

/// <summary>As mensagens de <see cref="IValidatableObject"/> não passam pelo localizador do MVC; os modelos traduzem-nas com <c>validationContext.Text</c>.</summary>
public sealed class ValidationLocalizationTests : IDisposable
{
    private readonly ControllerTestContext _context = new();

    private static Vehicle VehicleWithoutFuel() => new()
    {
        Brand = "Toyota", Model = "Corolla", Registration = "AA-00-BB", YearOfManufacture = 2020, FuelType = FuelType.Unspecified
    };

    private static string? FuelError(ValidationContext context)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(context.ObjectInstance, context, results, validateAllProperties: true);
        return results.SingleOrDefault(r => r.MemberNames.Contains(nameof(Vehicle.FuelType)))?.ErrorMessage;
    }

    [Theory]
    [InlineData("pt-PT", "Seleciona um tipo de combustível.")]
    [InlineData("en-GB", "Select a fuel type.")]
    [InlineData("es-ES", "Selecciona un tipo de combustible.")]
    public void InsideARequest_MessageIsTranslated(string culture, string expected)
    {
        using var _ = new CultureScope(culture);
        var vehicle = VehicleWithoutFuel();

        Assert.Equal(expected, FuelError(new ValidationContext(vehicle, _context.Services, items: null)));
    }

    [Fact]
    public void OutsideARequest_MessageIsTheKey()
    {
        var vehicle = VehicleWithoutFuel();

        Assert.Equal("validation.fuel_required", FuelError(new ValidationContext(vehicle)));
    }

    public void Dispose() => _context.Dispose();
}
