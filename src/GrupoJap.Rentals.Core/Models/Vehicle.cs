using System.ComponentModel.DataAnnotations;
using GrupoJap.Rentals.Localization;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Models;

[Index(nameof(Registration), IsUnique = true)]
public sealed class Vehicle : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "validation.brand_required")]
    [StringLength(80)]
    [Display(Name = "field.brand")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.model_required")]
    [StringLength(80)]
    [Display(Name = "field.model")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.registration_required")]
    [StringLength(15)]
    [Display(Name = "field.registration")]
    public string Registration { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "validation.year_invalid")]
    [Display(Name = "field.year")]
    public int YearOfManufacture { get; set; }

    [Display(Name = "field.fuel_type")]
    public FuelType FuelType { get; set; }

    [Range(0, 2_000_000, ErrorMessage = "validation.mileage_range")]
    [Display(Name = "field.mileage")]
    public int Mileage { get; set; }

    public ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (YearOfManufacture > DateTime.Today.Year)
        {
            yield return new ValidationResult(
                validationContext.Text("validation.year_future"),
                [nameof(YearOfManufacture)]);
        }

        if (!Enum.IsDefined(FuelType) || FuelType == FuelType.Unspecified)
        {
            yield return new ValidationResult(
                validationContext.Text("validation.fuel_required"),
                [nameof(FuelType)]);
        }
    }
}