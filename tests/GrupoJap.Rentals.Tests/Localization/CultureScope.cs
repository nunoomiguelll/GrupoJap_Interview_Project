using System.Globalization;

namespace GrupoJap.Rentals.Tests.Localization;

/// <summary>Muda o idioma do "pedido" durante um teste (o mesmo que o cookie de idioma faz na aplicação) e repõe-o no fim.</summary>
internal sealed class CultureScope : IDisposable
{
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _uiCulture = CultureInfo.CurrentUICulture;

    public CultureScope(string culture)
    {
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo(culture);
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _culture;
        CultureInfo.CurrentUICulture = _uiCulture;
    }
}
