using System.Globalization;

namespace GrupoJap.Rentals.Localization;

public sealed record SiteLanguage(string Code, string Culture, string Name);

/// <summary>Idiomas suportados pelo site. O português é o idioma base e o de recurso.</summary>
public static class SiteLanguages
{
    public const string Default = "pt";

    public static readonly IReadOnlyList<SiteLanguage> All =
    [
        new("pt", "pt-PT", "Português"),
        new("en", "en-GB", "English"),
        new("es", "es-ES", "Español")
    ];

    public static bool IsSupported(string? code) => All.Any(language => language.Code == code);

    /// <summary>Código do idioma do pedido atual (pt, en ou es).</summary>
    public static string Current
    {
        get
        {
            var code = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            return IsSupported(code) ? code : Default;
        }
    }

    public static SiteLanguage CurrentLanguage => All.First(language => language.Code == Current);
}
