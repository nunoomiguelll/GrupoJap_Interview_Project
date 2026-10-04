using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Models;

[Index(nameof(Email), IsUnique = true)]
public sealed class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    [StringLength(150)]
    [Display(Name = "Nome completo")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Indica um endereço de email válido.")]
    [StringLength(254)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [RegularExpression("^[0-9]{9}$", ErrorMessage = "O telefone deve conter 9 dígitos numéricos.")]
    [StringLength(9)]
    [Display(Name = "Telefone")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "A carta de condução é obrigatória.")]
    [StringLength(30)]
    [Display(Name = "Carta de condução")]
    public string DrivingLicenseNumber { get; set; } = string.Empty;

    public ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();
}