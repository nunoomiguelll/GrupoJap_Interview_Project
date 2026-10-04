using GrupoJap.Rentals.Controllers;
using GrupoJap.Rentals.Infrastructure;
using GrupoJap.Rentals.Tests.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GrupoJap.Rentals.Tests.Localization;

public sealed class LanguageControllerTests : IDisposable
{
    private readonly ControllerTestContext _context = new();

    private (LanguageController Controller, LocalRedirectResult Result) Set(string code, string? returnUrl)
    {
        var controller = _context.Setup(new LanguageController());
        var result = Assert.IsType<LocalRedirectResult>(controller.Set(code, returnUrl));
        return (controller, result);
    }

    private static string SetCookie(LanguageController controller) => controller.Response.Headers.SetCookie.ToString();

    [Theory]
    [InlineData("pt", "pt-PT")]
    [InlineData("en", "en-GB")]
    [InlineData("es", "es-ES")]
    public void SupportedLanguage_StoresTheCultureInTheLanguageCookie(string code, string culture)
    {
        var (controller, _) = Set(code, "/vehicles");

        var cookie = SetCookie(controller);
        Assert.StartsWith(RentalsHostingExtensions.LanguageCookieName + "=", cookie);
        Assert.Contains(Uri.EscapeDataString($"c={culture}|uic={culture}"), cookie);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ReturnsToThePageWhereTheUserWas()
    {
        var (_, result) = Set("en", "/vehicles?page=2");

        Assert.Equal("/vehicles?page=2", result.Url);
    }

    [Fact]
    public void UnknownLanguage_DoesNotChangeTheCookie()
    {
        var (controller, result) = Set("fr", "/vehicles");

        Assert.Empty(SetCookie(controller));
        Assert.Equal("/vehicles", result.Url);
    }

    [Theory]
    [InlineData("https://evil.example.com")]
    [InlineData("//evil.example.com")]
    [InlineData(null)]
    [InlineData("")]
    public void ExternalOrMissingReturnUrl_GoesToTheDashboard(string? returnUrl)
    {
        var (_, result) = Set("en", returnUrl);

        Assert.Equal("/", result.Url);
    }

    public void Dispose() => _context.Dispose();
}
