using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Models;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Localization;

/// <summary>
/// Insere na base de dados as chaves e os idiomas do catálogo que ainda não existem (idempotente).
/// Nunca altera textos existentes, para não apagar o que foi editado na administração.
/// </summary>
public static class TranslationSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var existing = await db.Translations
            .Include(t => t.Values)
            .ToDictionaryAsync(t => t.Key, StringComparer.Ordinal);

        var now = DateTime.UtcNow;
        foreach (var entry in TranslationCatalog.Entries)
        {
            if (!existing.TryGetValue(entry.Key, out var translation))
            {
                translation = new Translation
                {
                    Key = entry.Key,
                    Category = entry.Category,
                    Description = entry.Description,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                db.Translations.Add(translation);
            }

            foreach (var language in SiteLanguages.All)
            {
                if (translation.Values.All(v => v.Language != language.Code))
                {
                    translation.Values.Add(new TranslationValue { Language = language.Code, Value = entry.For(language.Code), UpdatedAt = now });
                }
            }
        }

        if (!db.ChangeTracker.HasChanges())
        {
            return;
        }

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            // O site e a administração podem arrancar ao mesmo tempo: se a outra aplicação já inseriu
            // as mesmas chaves, a violação do índice único é esperada e o catálogo já está na base de dados.
            services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(TranslationSeeder))
                .LogInformation(exception, "Traduções já inseridas por outra instância; a ignorar.");
            db.ChangeTracker.Clear();
        }
    }
}
