using System.ComponentModel.DataAnnotations;
using GrupoJap.Rentals.Localization;

namespace GrupoJap.Rentals.Models;

public sealed class RentalContract : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "validation.customer_required")]
    [Range(1, int.MaxValue, ErrorMessage = "validation.customer_invalid")]
    [Display(Name = "field.customer")]
    public int? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    [Required(ErrorMessage = "validation.vehicle_required")]
    [Range(1, int.MaxValue, ErrorMessage = "validation.vehicle_invalid")]
    [Display(Name = "field.vehicle")]
    public int? VehicleId { get; set; }

    public Vehicle? Vehicle { get; set; }

    [Required(ErrorMessage = "validation.start_required")]
    [Display(Name = "field.start_date")]
    public DateOnly? StartDate { get; set; }

    [Required(ErrorMessage = "validation.end_required")]
    [Display(Name = "field.end_date")]
    public DateOnly? EndDate { get; set; }

    [Required(ErrorMessage = "validation.initial_mileage_required")]
    [Range(0, int.MaxValue, ErrorMessage = "validation.initial_mileage_negative")]
    [Display(Name = "field.initial_mileage")]
    public int? InitialMileage { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (StartDate is DateOnly startDate && startDate < today)
        {
            yield return new ValidationResult(
                validationContext.Text("validation.start_past"),
                [nameof(StartDate)]);
        }

        if (StartDate is DateOnly start && EndDate is DateOnly end && end <= start)
        {
            yield return new ValidationResult(
                validationContext.Text("validation.end_after_start"),
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