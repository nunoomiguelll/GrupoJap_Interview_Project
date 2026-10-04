using System.ComponentModel.DataAnnotations;

namespace GrupoJap.Rentals.ViewModels;

public sealed class TranslationListItemViewModel
{
    public int Id { get; init; }
    public string Key { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IReadOnlyDictionary<string, string> Values { get; init; } = new Dictionary<string, string>();
    public DateTime UpdatedAt { get; init; }
}

public sealed class TranslationIndexViewModel
{
    public const int PageSize = 25;

    // Filtros (query string)
    public string? Q { get; set; }
    public string? Category { get; set; }
    public bool OnlyMissing { get; set; }
    public int Page { get; set; } = 1;

    // Resultados
    public IReadOnlyList<TranslationListItemViewModel> Items { get; set; } = [];
    public IReadOnlyList<string> Categories { get; set; } = [];
    public int TotalCount { get; set; }
    public int MissingCount { get; set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}

public sealed class TranslationFormViewModel
{
    public int? Id { get; set; }

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
    [Display(Name = "field.translation_notes")]
    public string? Description { get; set; }

    /// <summary>Texto por idioma (pt, en, es). O português é obrigatório por ser o idioma de recurso.</summary>
    public Dictionary<string, string?> Values { get; set; } = [];

    /// <summary>Preenchido quando a chave existe no catálogo do código (texto original de cada idioma).</summary>
    public IReadOnlyDictionary<string, string>? Defaults { get; set; }

    public IReadOnlyList<string> Categories { get; set; } = [];
}
