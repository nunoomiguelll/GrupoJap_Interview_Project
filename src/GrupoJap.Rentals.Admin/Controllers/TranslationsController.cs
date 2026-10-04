using GrupoJap.Rentals.Data;
using GrupoJap.Rentals.Localization;
using GrupoJap.Rentals.Models;
using GrupoJap.Rentals.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GrupoJap.Rentals.Controllers;

/// <summary>
/// Gestão dos textos do site público em PT/EN/ES. Ao guardar, a cache de traduções é descartada;
/// o site (outro processo) recarrega-a sozinho em poucos segundos (Translations:CacheSeconds).
/// </summary>
[Authorize(Roles = AppRoles.Admin)]
[Route("translations")]
public sealed class TranslationsController(ApplicationDbContext dbContext, TranslationStore store) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] TranslationIndexViewModel filters)
    {
        var languageCount = SiteLanguages.All.Count;
        var query = dbContext.Translations.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filters.Q))
        {
            var term = filters.Q.Trim();
            query = query.Where(t => t.Key.Contains(term)
                || (t.Description != null && t.Description.Contains(term))
                || t.Values.Any(v => v.Value.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(filters.Category))
        {
            query = query.Where(t => t.Category == filters.Category);
        }

        if (filters.OnlyMissing)
        {
            query = query.Where(t => t.Values.Count(v => v.Value != "") < languageCount);
        }

        filters.TotalCount = await query.CountAsync();
        filters.Page = Math.Clamp(filters.Page, 1, filters.TotalPages);
        var rows = await query
            .OrderBy(t => t.Category).ThenBy(t => t.Key)
            .Skip((filters.Page - 1) * TranslationIndexViewModel.PageSize)
            .Take(TranslationIndexViewModel.PageSize)
            .Include(t => t.Values)
            .ToListAsync();
        filters.Items = rows.Select(t => new TranslationListItemViewModel
        {
            Id = t.Id,
            Key = t.Key,
            Category = t.Category,
            Description = t.Description,
            UpdatedAt = t.UpdatedAt,
            Values = t.Values.ToDictionary(v => v.Language, v => v.Value)
        }).ToList();

        filters.Categories = await CategoriesAsync();
        filters.MissingCount = await dbContext.Translations.CountAsync(t => t.Values.Count(v => v.Value != "") < languageCount);
        return View(filters);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(string? category)
        => View("Form", await PrepareAsync(new TranslationFormViewModel { Category = category ?? string.Empty }));

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TranslationFormViewModel model)
    {
        model.Key = model.Key.Trim();
        ValidateValues(model);
        if (ModelState.IsValid && await dbContext.Translations.AnyAsync(t => t.Key == model.Key))
        {
            ModelState.AddModelError(nameof(model.Key), "Já existe uma tradução com esta chave.");
        }

        if (!ModelState.IsValid)
        {
            return View("Form", await PrepareAsync(model));
        }

        var now = DateTime.UtcNow;
        var translation = new Translation
        {
            Key = model.Key,
            Category = model.Category.Trim(),
            Description = NullIfBlank(model.Description),
            CreatedAt = now,
            UpdatedAt = now
        };
        ApplyValues(translation, model, now);
        dbContext.Translations.Add(translation);
        await dbContext.SaveChangesAsync();
        store.Invalidate();

        TempData["SuccessMessage"] = $"Tradução \"{translation.Key}\" criada. Usa @T[\"{translation.Key}\"] numa vista para a mostrar no site.";
        return RedirectToAction(nameof(Index), new { category = translation.Category });
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, string? returnUrl)
    {
        var translation = await dbContext.Translations.AsNoTracking().Include(t => t.Values).FirstOrDefaultAsync(t => t.Id == id);
        if (translation is null)
        {
            return NotFound();
        }

        ViewData["ReturnUrl"] = SafeReturnUrl(returnUrl);
        return View("Form", await PrepareAsync(new TranslationFormViewModel
        {
            Id = translation.Id,
            Key = translation.Key,
            Category = translation.Category,
            Description = translation.Description,
            Values = translation.Values.ToDictionary(v => v.Language, v => (string?)v.Value)
        }));
    }

    [HttpPost("{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TranslationFormViewModel model, string? returnUrl)
    {
        var translation = await dbContext.Translations.Include(t => t.Values).FirstOrDefaultAsync(t => t.Id == id);
        if (translation is null)
        {
            return NotFound();
        }

        // A chave é usada no código das vistas, por isso não é editável.
        model.Id = id;
        model.Key = translation.Key;
        ModelState.Remove(nameof(model.Key));
        ValidateValues(model);

        if (!ModelState.IsValid)
        {
            ViewData["ReturnUrl"] = SafeReturnUrl(returnUrl);
            return View("Form", await PrepareAsync(model));
        }

        var now = DateTime.UtcNow;
        translation.Category = model.Category.Trim();
        translation.Description = NullIfBlank(model.Description);
        translation.UpdatedAt = now;
        ApplyValues(translation, model, now);
        await dbContext.SaveChangesAsync();
        store.Invalidate();

        TempData["SuccessMessage"] = $"Tradução \"{translation.Key}\" atualizada. O site mostra o novo texto em poucos segundos.";
        return SafeReturnUrl(returnUrl) is { } url ? LocalRedirect(url) : RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var translation = await dbContext.Translations.AsNoTracking().Include(t => t.Values).FirstOrDefaultAsync(t => t.Id == id);
        if (translation is null)
        {
            return NotFound();
        }

        ViewBag.IsCatalogKey = TranslationCatalog.Find(translation.Key) is not null;
        return View(translation);
    }

    [HttpPost("{id:int}/delete"), ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var translation = await dbContext.Translations.FirstOrDefaultAsync(t => t.Id == id);
        if (translation is null)
        {
            return NotFound();
        }

        dbContext.Translations.Remove(translation);
        await dbContext.SaveChangesAsync();
        store.Invalidate();

        TempData["SuccessMessage"] = TranslationCatalog.Find(translation.Key) is not null
            ? $"Tradução \"{translation.Key}\" eliminada. O site volta a usar o texto original, que será reposto na base de dados no próximo arranque."
            : $"Tradução \"{translation.Key}\" eliminada.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateValues(TranslationFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Values.GetValueOrDefault(SiteLanguages.Default)))
        {
            ModelState.AddModelError($"Values[{SiteLanguages.Default}]", "O texto em português é obrigatório (é o idioma de recurso).");
        }
    }

    private static void ApplyValues(Translation translation, TranslationFormViewModel model, DateTime now)
    {
        foreach (var language in SiteLanguages.All)
        {
            // Texto vazio = sem tradução: o site usa o texto original desse idioma ou, na falta dele, o português.
            var text = model.Values.GetValueOrDefault(language.Code)?.Trim() ?? string.Empty;
            var value = translation.Values.FirstOrDefault(v => v.Language == language.Code);
            if (value is null)
            {
                translation.Values.Add(new TranslationValue { Language = language.Code, Value = text, UpdatedAt = now });
            }
            else if (value.Value != text)
            {
                value.Value = text;
                value.UpdatedAt = now;
            }
        }
    }

    private async Task<TranslationFormViewModel> PrepareAsync(TranslationFormViewModel model)
    {
        model.Categories = await CategoriesAsync();
        if (TranslationCatalog.Find(model.Key) is { } entry)
        {
            model.Defaults = SiteLanguages.All.ToDictionary(l => l.Code, l => entry.For(l.Code));
        }

        return model;
    }

    private Task<List<string>> CategoriesAsync()
        => dbContext.Translations.AsNoTracking().Select(t => t.Category).Distinct().OrderBy(c => c).ToListAsync();

    private string? SafeReturnUrl(string? returnUrl)
        => !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : null;

    private static string? NullIfBlank(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
