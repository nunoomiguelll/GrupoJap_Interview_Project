using System.ComponentModel.DataAnnotations;
using GrupoJap.Rentals.Models;

namespace GrupoJap.Rentals.ViewModels;

public sealed class ProfileViewModel
{
    [Display(Name = "field.email")]
    public string Email { get; set; } = string.Empty;

    public string? PhotoPath { get; set; }

    public string Initial { get; set; } = "?";

    public string RoleLabel { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.full_name_required")]
    [StringLength(150)]
    [Display(Name = "field.full_name")]
    public string FullName { get; set; } = string.Empty;

    [RegularExpression(PhoneNumbers.PortuguesePattern, ErrorMessage = "validation.phone_pt")]
    [Display(Name = "field.phone")]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    [Display(Name = "field.job_title")]
    public string? JobTitle { get; set; }

    [StringLength(80)]
    [Display(Name = "field.favorite_brand")]
    public string? FavoriteBrand { get; set; }

    [StringLength(500, ErrorMessage = "validation.bio_length")]
    [Display(Name = "field.bio")]
    public string? Bio { get; set; }

    [Display(Name = "field.photo")]
    public IFormFile? Photo { get; set; }

    [Display(Name = "field.remove_photo")]
    public bool RemovePhoto { get; set; }

    public IReadOnlyList<string> BrandSuggestions { get; set; } = [];
}

public sealed class ChangePasswordViewModel
{
    [Required(ErrorMessage = "validation.current_password_required")]
    [DataType(DataType.Password)]
    [Display(Name = "field.current_password")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.new_password_required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "password.error.too_short")]
    [DataType(DataType.Password)]
    [Display(Name = "field.new_password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.confirm_new_password")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "validation.password_mismatch")]
    [Display(Name = "field.confirm_new_password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
