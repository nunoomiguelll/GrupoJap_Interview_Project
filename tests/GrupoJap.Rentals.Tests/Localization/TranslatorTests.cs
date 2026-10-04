using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Tests.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace GrupoJap.Rentals.Tests.Localization;

public sealed class TranslatorTests : IDisposable
{
    private readonly ControllerTestContext _context = new();

    private Translator T => _context.T;

    /// <summary>Grava uma tradução na base de dados (como se tivesse sido editada no painel).</summary>
    private void SeedTranslation(string key, params (string Language, string Value)[] values) => _context.Seed(new Translation
    {
        Key = key,
        Category = "Teste",
        Values = values.Select(v => new TranslationValue { Language = v.Language, Value = v.Value }).ToList()
    });

    [Theory]
    [InlineData("pt-PT", "Veículos")]
    [InlineData("en-GB", "Vehicles")]
    [InlineData("es-ES", "Vehículos")]
    public void UsesTheCatalogInTheRequestLanguage(string culture, string expected)
    {
        using var _ = new CultureScope(culture);

        Assert.Equal(expected, T["nav.vehicles"].Value);
    }

    [Fact]
    public void UnsupportedLanguage_FallsBackToPortuguese()
    {
        using var _ = new CultureScope("fr-FR");

        Assert.Equal("Veículos", T["nav.vehicles"].Value);
    }

    [Fact]
    public void TextEditedInTheDatabase_WinsOverTheCatalog()
    {
        SeedTranslation("nav.vehicles", ("en", "Fleet"));
        using var _ = new CultureScope("en-GB");

        Assert.Equal("Fleet", T["nav.vehicles"].Value);
    }

    [Fact]
    public void EmptyValueInTheDatabase_CountsAsMissing()
    {
        SeedTranslation("nav.vehicles", ("en", ""));
        using var _ = new CultureScope("en-GB");

        Assert.Equal("Vehicles", T["nav.vehicles"].Value);
    }

    [Fact]
    public void KeyWithoutTextInTheCurrentLanguage_FallsBackToPortugueseFromTheDatabase()
    {
        SeedTranslation("custom.promo", ("pt", "Promoção de verão"));
        using var _ = new CultureScope("es-ES");

        Assert.Equal("Promoção de verão", T["custom.promo"].Value);
    }

    [Fact]
    public void UnknownKey_ReturnsTheKeyAndFlagsItAsNotFound()
    {
        var text = T["does.not.exist"];

        Assert.Equal("does.not.exist", text.Value);
        Assert.True(text.ResourceNotFound);
    }

    [Fact]
    public void PlainText_IsReturnedUnchanged()
    {
        // DataAnnotations sem chave (texto livre) passam pelo mesmo localizador e não podem ser estragadas.
        Assert.Equal("Texto qualquer.", T["Texto qualquer."].Value);
    }

    [Theory]
    [InlineData("pt-PT", "3 veículo(s) registado(s)")]
    [InlineData("en-GB", "3 vehicle(s) registered")]
    public void FormatsArguments(string culture, string expected)
    {
        using var _ = new CultureScope(culture);

        Assert.Equal(expected, T["vehicles.count", 3].Value);
    }

    [Theory]
    [InlineData("pt-PT", FuelType.Diesel, "Gasóleo")]
    [InlineData("en-GB", FuelType.Lpg, "LPG")]
    [InlineData("es-ES", FuelType.Electric, "Eléctrico")]
    public void Fuel_IsTranslated(string culture, FuelType fuel, string expected)
    {
        using var _ = new CultureScope(culture);

        Assert.Equal(expected, T.Fuel(fuel));
    }

    [Fact]
    public void SavedEdits_AppearAfterTheCacheIsInvalidated()
    {
        SeedTranslation("custom.title", ("pt", "Antes"));
        using var _ = new CultureScope("pt-PT");
        Assert.Equal("Antes", T["custom.title"].Value);

        _context.Query(db =>
        {
            db.TranslationValues.Single(v => v.Translation!.Key == "custom.title").Value = "Depois";
            return db.SaveChanges();
        });
        Assert.Equal("Antes", T["custom.title"].Value); // ainda em cache

        _context.Services.GetRequiredService<TranslationStore>().Invalidate();
        Assert.Equal("Depois", T["custom.title"].Value);
    }

    public void Dispose() => _context.Dispose();
}
