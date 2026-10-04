using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GrupoJap.Rentals.Models;

public sealed class ApplicationUser : IdentityUser
{
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Caminho público da foto de perfil (ex.: /uploads/avatars/xxx.jpg). Nulo se não houver foto.</summary>
    [StringLength(260)]
    public string? PhotoPath { get; set; }

    [StringLength(80)]
    public string? FavoriteBrand { get; set; }

    [StringLength(100)]
    public string? JobTitle { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    /// <summary>Nome a mostrar na interface: nome completo ou, se vazio, o email.</summary>
    public string DisplayName => string.IsNullOrWhiteSpace(FullName) ? (Email ?? UserName ?? string.Empty) : FullName;

    public string Initial => DisplayName.Length > 0 ? DisplayName[..1].ToUpperInvariant() : "?";
}
