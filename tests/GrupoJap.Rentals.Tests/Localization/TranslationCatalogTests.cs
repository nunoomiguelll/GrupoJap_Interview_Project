using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using GrupoJap.Rentals.Localization;

namespace GrupoJap.Rentals.Tests.Localization;

/// <summary>Garante que o catálogo está completo e coerente com o código que o usa.</summary>
public sealed partial class TranslationCatalogTests
{
    private static IEnumerable<CatalogEntry> Entries => TranslationCatalog.Entries;

    [Fact]
    public void KeysAreUnique()
    {
        var duplicates = Entries.GroupBy(e => e.Key).Where(g => g.Count() > 1).Select(g => g.Key);

        Assert.Empty(duplicates);
    }

    [Fact]
    public void EveryEntryHasTextInEveryLanguage()
    {
        var incomplete = Entries
            .Where(e => SiteLanguages.All.Any(language => string.IsNullOrWhiteSpace(e.For(language.Code))))
            .Select(e => e.Key);

        Assert.Empty(incomplete);
    }

    [Fact]
    public void PlaceholdersMatchAcrossLanguages()
    {
        // Ex.: "{0} veículo(s)" em PT tem de ter {0} também em EN e ES, senão o string.Format falha ou perde o valor.
        var mismatched = Entries
            .Where(e => SiteLanguages.All.Select(l => Placeholders(e.For(l.Code))).Distinct().Count() > 1)
            .Select(e => e.Key);

        Assert.Empty(mismatched);
    }

    [Fact]
    public void KeysFollowTheFormatAcceptedByTheTranslationsPage()
    {
        var invalid = Entries.Where(e => !KeyFormat().IsMatch(e.Key)).Select(e => e.Key);

        Assert.Empty(invalid);
    }

    [Fact]
    public void NoCatalogKeyIsMarkedAsRetired()
    {
        // Uma chave retirada seria apagada da base de dados a cada arranque.
        Assert.Empty(Entries.Where(e => TranslationCatalog.IsRetired(e.Key)).Select(e => e.Key));
    }

    [Fact]
    public void EveryKeyUsedInTheCodeExistsInTheCatalog()
    {
        // Procura nas vistas e no código C# as chaves usadas (T["..."], ErrorMessage = "...", Display(Name = "...")).
        var prefixes = Entries.Select(e => e.Key.Split('.')[0]).ToHashSet();
        var sourceFiles = Directory
            .EnumerateFiles(SourceRoot(), "*.*", SearchOption.AllDirectories)
            .Where(path => path.EndsWith(".cshtml") || path.EndsWith(".cs"))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                        && !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
                        && !path.EndsWith("TranslationCatalog.cs"));

        var missing = sourceFiles
            .SelectMany(path => KeyLiteral().Matches(File.ReadAllText(path))
                .Select(match => (Key: match.Groups[1].Value, File: Path.GetFileName(path))))
            .Where(found => prefixes.Contains(found.Key.Split('.')[0]) && TranslationCatalog.Find(found.Key) is null)
            .Select(found => $"{found.Key} ({found.File})")
            .Distinct()
            .ToList();

        Assert.Empty(missing);
    }

    private static string Placeholders(string text) => string.Join(",", PlaceholderPattern().Matches(text).Select(m => m.Value).Order());

    /// <summary>Pasta src da solução, encontrada a partir da localização deste ficheiro de código.</summary>
    private static string SourceRoot([CallerFilePath] string thisFile = "")
        => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "..", "..", "src"));

    [GeneratedRegex(@"\{\d+\}")]
    private static partial Regex PlaceholderPattern();

    // O mesmo formato validado em Translation.Key / TranslationFormViewModel.Key.
    [GeneratedRegex(@"^[a-z0-9]+([._-][a-zA-Z0-9]+)*$")]
    private static partial Regex KeyFormat();

    [GeneratedRegex("\"([a-z]+(?:\\.[a-zA-Z0-9_]+)+)\"")]
    private static partial Regex KeyLiteral();
}
