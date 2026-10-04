using GrupoJap.Rentals.Data;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Localization;

/// <summary>
/// Cache em memória de todas as traduções da base de dados (idioma → chave → texto).
/// É recarregada quando expira (<c>Translations:CacheSeconds</c>, 10 s por omissão) ou quando a
/// administração guarda uma alteração, por isso o site reflete as edições sem reiniciar.
/// </summary>
public sealed class TranslationStore(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<TranslationStore> logger)
{
    private readonly Lock _reloadLock = new();
    private readonly TimeSpan _maxAge = TimeSpan.FromSeconds(Math.Max(0, configuration.GetValue("Translations:CacheSeconds", 10)));
    private volatile Snapshot? _snapshot;

    private sealed record Snapshot(IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Values, DateTime LoadedAt);

    public string? Find(string language, string key)
    {
        var values = GetSnapshot().Values;
        return values.TryGetValue(language, out var byKey) && byKey.TryGetValue(key, out var value) ? value : null;
    }

    public IReadOnlyDictionary<string, string> All(string language)
        => GetSnapshot().Values.TryGetValue(language, out var byKey) ? byKey : new Dictionary<string, string>();

    /// <summary>Descarta a cache; a próxima leitura volta a carregar da base de dados.</summary>
    public void Invalidate() => _snapshot = null;

    private Snapshot GetSnapshot()
    {
        var current = _snapshot;
        if (current is not null && DateTime.UtcNow - current.LoadedAt < _maxAge)
        {
            return current;
        }

        lock (_reloadLock)
        {
            current = _snapshot;
            if (current is not null && DateTime.UtcNow - current.LoadedAt < _maxAge)
            {
                return current;
            }

            _snapshot = Load(current);
            return _snapshot;
        }
    }

    private Snapshot Load(Snapshot? previous)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var rows = db.TranslationValues
                .AsNoTracking()
                .Where(v => v.Value != "")
                .Select(v => new { v.Translation!.Key, v.Language, v.Value })
                .ToList();

            var values = rows
                .GroupBy(r => r.Language)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyDictionary<string, string>)g.ToDictionary(r => r.Key, r => r.Value, StringComparer.Ordinal));

            return new Snapshot(values, DateTime.UtcNow);
        }
        catch (Exception exception)
        {
            // Sem base de dados (ou sem a migration aplicada) o site continua a funcionar com os textos do catálogo.
            logger.LogWarning(exception, "Não foi possível carregar as traduções; a usar a cache anterior ou os textos por omissão.");
            return new Snapshot(previous?.Values ?? new Dictionary<string, IReadOnlyDictionary<string, string>>(), DateTime.UtcNow);
        }
    }
}
