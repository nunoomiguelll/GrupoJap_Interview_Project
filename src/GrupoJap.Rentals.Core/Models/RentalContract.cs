using System.ComponentModel.DataAnnotations;

namespace GrupoJap.Rentals.Models;

public sealed class RentalContract : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Seleciona um cliente.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleciona um cliente válido.")]
    [Display(Name = "Cliente")]
    public int? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [Required(ErrorMessage = "Seleciona um veículo.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleciona um veículo válido.")]
    [Display(Name = "Veículo")]
    public int? VehicleId { get; set; }

    public Vehicle? Vehicle { get; set; }

    [Required(ErrorMessage = "A data de início é obrigatória.")]
    [Display(Name = "Data de início")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "A data de fim é obrigatória.")]
    [Display(Name = "Data de fim")]
    public DateOnly? EndDate { get; set; }

    [Required(ErrorMessage = "A quilometragem inicial é obrigatória.")]
    [Range(0, int.MaxValue, ErrorMessage = "A quilometragem inicial não pode ser negativa.")]
    [Display(Name = "Quilometragem inicial")]
    public int? InitialMileage { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (StartDate is DateOnly startDate && startDate < today)
        {
            yield return new ValidationResult(
                "A data de início não pode ser anterior à data atual.",
                [nameof(StartDate)]);
        }

        if (StartDate is DateOnly start && EndDate is DateOnly end && end <= start)
        {
            yield return new ValidationResult(
                "A data de fim tem de ser posterior à data de início.",
                [nameof(EndDate)]);
        }
    }

    public bool Overlaps(DateOnly requestedStart, DateOnly requestedEnd)
    {
        return StartDate is DateOnly startDate
            && EndDate is DateOnly endDate
            && startDate <= requestedEnd
            && endDate >= requestedStart;
    }
}