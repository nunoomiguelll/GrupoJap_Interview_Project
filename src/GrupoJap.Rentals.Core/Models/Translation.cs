using System.ComponentModel.DataAnnotations;

namespace GrupoJap.Rentals.Models;

/// <summary>Texto traduzível do painel, identificado por uma chave estável (ex.: <c>dash.hero.title</c>).</summary>
public sealed class Translation
{
    public int Id { get; set; }

    [Required(ErrorMessage = "validation.key_required")]
    [StringLength(200)]
    [RegularExpression(@"^[a-z0-9]+([._-][a-zA-Z0-9]+)*$", ErrorMessage = "validation.key_format")]
    [Display(Name = "field.key")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "validation.category_required")]
    [StringLength(100)]
    [Display(Name = "field.category")]
    public string Category { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "field.description")]
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
