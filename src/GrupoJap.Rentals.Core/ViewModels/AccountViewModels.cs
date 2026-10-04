using System.ComponentModel.DataAnnotations;

namespace GrupoJap.Rentals.ViewModels;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "validation.email_required")]
    [EmailAddress(ErrorMessage = "account.error.invalid_email")]
    [Display(Name = "field.email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.password_required")]
    [DataType(DataType.Password)]
    [Display(Name = "field.password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "field.remember")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

public sealed class RegisterViewModel
{
    [Required(ErrorMessage = "validation.full_name_required")]
    [StringLength(150)]
    [Display(Name = "field.full_name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.email_required")]
    [EmailAddress(ErrorMessage = "account.error.invalid_email")]
    [StringLength(254)]
    [Display(Name = "field.email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.password_required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "password.error.too_short")]
    [DataType(DataType.Password)]
    [Display(Name = "field.password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.confirm_password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "validation.password_mismatch")]
    [Display(Name = "field.confirm_password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
