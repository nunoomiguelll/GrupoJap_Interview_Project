using GrupoJap.Rentals.Infrastructure;
using GrupoJap.Rentals.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace GrupoJap.Rentals.Controllers;

/// <summary>Muda o idioma do painel: guarda a escolha num cookie (1 ano) e volta à página onde o utilizador estava.</summary>
[AllowAnonymous]
[Route("language")]
public sealed class LanguageController : Controller
{
    [HttpGet("{code}")]
    public IActionResult Set(string code, string? returnUrl)
    {
        var language = SiteLanguages.All.FirstOrDefault(l => l.Code == code);
        if (language is not null)
        {
            Response.Cookies.Append(
                RentalsHostingExtensions.LanguageCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language.Culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true, HttpOnly = true, SameSite = SameSiteMode.Lax });
        }

        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/");
    }
}
