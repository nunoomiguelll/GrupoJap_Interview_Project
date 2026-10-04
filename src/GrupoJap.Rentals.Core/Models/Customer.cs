using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Models;

[Index(nameof(Email), IsUnique = true)]
public sealed class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "validation.full_name_required")]
    [StringLength(150)]
    [Display(Name = "field.full_name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.email_required")]
    [EmailAddress(ErrorMessage = "account.error.invalid_email")]
    [StringLength(254)]
    [Display(Name = "field.email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.phone_required")]
    [RegularExpression("^[0-9]{9}$", ErrorMessage = "validation.phone_format")]
    [StringLength(9)]
    [Display(Name = "field.phone")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.license_required")]
    [StringLength(30)]
    [Display(Name = "field.driving_license")]
    public string DrivingLicenseNumber { get; set; } = string.Empty;

    public ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();
}