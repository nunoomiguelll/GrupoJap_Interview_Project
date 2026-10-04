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
    [Display(Name = "Descrição (onde aparece / notas)")]
    public string? Description { get; set; }

    /// <summary>Texto por idioma (pt, en, es). O português é obrigatório por ser o idioma de recurso.</summary>
    public Dictionary<string, string?> Values { get; set; } = [];

    /// <summary>Preenchido quando a chave existe no catálogo do código (texto original de cada idioma).</summary>
    public IReadOnlyDictionary<string, string>? Defaults { get; set; }

    public IReadOnlyList<string> Categories { get; set; } = [];
}
