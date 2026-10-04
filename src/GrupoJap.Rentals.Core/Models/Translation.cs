using System.ComponentModel.DataAnnotations;

namespace GrupoJap.Rentals.Models;

/// <summary>Texto traduzível do site, identificado por uma chave estável (ex.: <c>home.hero.title</c>).</summary>
public sealed class Translation
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A chave é obrigatória.")]
    [StringLength(200)]
    [RegularExpression(@"^[a-z0-9]+([._-][a-zA-Z0-9]+)*$", ErrorMessage = "Usa letras minúsculas, números e pontos (ex.: home.hero.title).")]
    [Display(Name = "Chave")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [StringLength(100)]
    [Display(Name = "Categoria")]
    public string Category { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Descrição")]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<TranslationValue> Values { get; set; } = [];
}

/// <summary>Valor de uma tradução num idioma (chave composta: tradução + idioma).</summary>
public sealed class TranslationValue
{
    public int TranslationId { get; set; }

    public Translation? Translation { get; set; }

    [StringLength(10)]
    public string Language { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}
