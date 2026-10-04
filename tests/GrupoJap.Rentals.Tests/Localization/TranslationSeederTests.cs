using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.Tests.Controllers;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Tests.Localization;

public sealed class TranslationSeederTests : IDisposable
{
    private readonly ControllerTestContext _context = new();

    private Task Seed() => TranslationSeeder.SeedAsync(_context.Services);

    private string? StoredValue(string key, string language) => _context.Query(db => db.TranslationValues
        .Where(v => v.Translation!.Key == key && v.Language == language)
        .Select(v => v.Value)
        .SingleOrDefault());

    private static Translation Row(string key, params (string Language, string Value)[] values) => new()
    {
        Key = key,
        Category = "Teste",
        Values = values.Select(v => new TranslationValue { Language = v.Language, Value = v.Value }).ToList()
    };

    [Fact]
    public async Task EmptyDatabase_ReceivesTheWholeCatalogInEveryLanguage()
    {
        await Seed();

        Assert.Equal(TranslationCatalog.Entries.Count, _context.Query(db => db.Translations.Count()));
        Assert.Equal(TranslationCatalog.Entries.Count * SiteLanguages.All.Count, _context.Query(db => db.TranslationValues.Count()));
        Assert.Equal("Vehicles", StoredValue("nav.vehicles", "en"));
    }

    [Fact]
    public async Task TextsEditedInThePanel_AreKept()
    {
        _context.Seed(Row("nav.vehicles", ("pt", "Frota"), ("en", "Fleet"), ("es", "Flota")));

        await Seed();

        Assert.Equal("Fleet", StoredValue("nav.vehicles", "en"));
        Assert.Equal("Frota", StoredValue("nav.vehicles", "pt"));
    }

    [Fact]
    public async Task MissingLanguage_IsAddedWithoutTouchingTheOthers()
    {
        _context.Seed(Row("nav.vehicles", ("pt", "Frota")));

        await Seed();

        Assert.Equal("Frota", StoredValue("nav.vehicles", "pt"));
        Assert.Equal("Vehicles", StoredValue("nav.vehicles", "en"));
        Assert.Equal("Vehículos", StoredValue("nav.vehicles", "es"));
    }

    [Fact]
    public async Task RetiredKeysOfTheOldWebsite_AreRemoved_CustomKeysAreKept()
    {
        _context.Seed(
            Row("home.hero.title", ("pt", "Encontra a viatura certa")),
            Row("nav.my_bookings", ("pt", "As minhas reservas")),
            Row("custom.promo", ("pt", "Promoção")));

        await Seed();

        Assert.Null(StoredValue("home.hero.title", "pt"));
        Assert.Null(StoredValue("nav.my_bookings", "pt"));
        Assert.Equal("Promoção", StoredValue("custom.promo", "pt"));
        // Os valores das chaves apagadas saem em cascata.
        Assert.Equal(0, _context.Query(db => db.TranslationValues.Count(v => v.Translation == null)));
    }

    [Fact]
    public async Task RunningTwice_ChangesNothing()
    {
        await Seed();
        var before = _context.Query(db => db.TranslationValues.AsNoTracking().Select(v => new { v.TranslationId, v.Language, v.Value, v.UpdatedAt }).ToList());

        await Seed();
        var after = _context.Query(db => db.TranslationValues.AsNoTracking().Select(v => new { v.TranslationId, v.Language, v.Value, v.UpdatedAt }).ToList());

        Assert.Equal(before.OrderBy(v => v.TranslationId).ThenBy(v => v.Language), after.OrderBy(v => v.TranslationId).ThenBy(v => v.Language));
    }

    public void Dispose() => _context.Dispose();
}
