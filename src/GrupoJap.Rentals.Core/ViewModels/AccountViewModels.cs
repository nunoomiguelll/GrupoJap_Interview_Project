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
