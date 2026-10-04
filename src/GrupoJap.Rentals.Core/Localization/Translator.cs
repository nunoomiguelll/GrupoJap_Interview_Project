using System.Globalization;
using GrupoJap.Rentals.Models;
using Microsoft.Extensions.Localization;

namespace GrupoJap.Rentals.Localization;

/// <summary>
/// Devolve o texto de uma chave no idioma do pedido. Ordem de recurso:
/// base de dados (idioma atual) → catálogo (idioma atual) → base de dados (português) → catálogo (português) → a própria chave.
/// Também é o <see cref="IStringLocalizer"/> usado pelas DataAnnotations: um texto que não seja chave
/// (ex.: mensagens já escritas em português nos modelos da administração) é devolvido tal como está.
/// </summary>
public sealed class Translator(TranslationStore store) : IStringLocalizer
{
    public LocalizedString this[string name]
    {
        get
        {
            var value = Resolve(name, SiteLanguages.Current);
            return new LocalizedString(name, value ?? name, resourceNotFound: value is null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var template = this[name];
            return new LocalizedString(name, string.Format(CultureInfo.CurrentCulture, template.Value, arguments), template.ResourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var language = SiteLanguages.Current;
        return TranslationCatalog.Entries
            .Select(entry => entry.Key)
            .Concat(store.All(language).Keys)
            .Distinct()
            .Select(key => this[key]);
    }

    /// <summary>Nome traduzido de um combustível (chaves <c>fuel.*</c>).</summary>
    public string Fuel(FuelType fuel) => Resolve("fuel." + fuel, SiteLanguages.Current) ?? fuel.GetDisplayName();

    private string? Resolve(string key, string language)
        => store.Find(language, key)
           ?? TranslationCatalog.DefaultFor(key, language)
           ?? store.Find(SiteLanguages.Default, key)
           ?? TranslationCatalog.DefaultFor(key, SiteLanguages.Default);
}

/// <summary>Faz com que qualquer <see cref="IStringLocalizer"/> pedido pela framework use as traduções da base de dados.</summary>
public sealed class TranslationLocalizerFactory(Translator translator) : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource) => translator;

    public IStringLocalizer Create(string baseName, string location) => translator;
}
