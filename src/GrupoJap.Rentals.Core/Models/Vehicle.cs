using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Models;

[Index(nameof(Registration), IsUnique = true)]
public sealed class Vehicle : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A marca é obrigatória.")]
    [StringLength(80)]
    [Display(Name = "Marca")]
    public string Brand { get; set; } = string.Empty;

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    [StringLength(80)]
    [Display(Name = "Modelo")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "A matrícula é obrigatória.")]
    [StringLength(15)]
    [Display(Name = "Matrícula")]
    public string Registration { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Indica um ano de fabrico válido.")]
    [Display(Name = "Ano de fabrico")]
    public int YearOfManufacture { get; set; }

    [Display(Name = "Tipo de combustível")]
    public FuelType FuelType { get; set; }

    [Range(0, 2_000_000, ErrorMessage = "A quilometragem tem de estar entre 0 e 2 000 000 km.")]
    [Display(Name = "Quilometragem atual")]
    public int Mileage { get; set; }

    public ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (YearOfManufacture > DateTime.Today.Year)
        {
            yield return new ValidationResult(
                "O ano de fabrico não pode ser posterior ao ano atual.",
                [nameof(YearOfManufacture)]);
        }

        if (!Enum.IsDefined(FuelType) || FuelType == FuelType.Unspecified)
        {
            yield return new ValidationResult(
                "Seleciona um tipo de combustível.",
                [nameof(FuelType)]);
        }
    }
}