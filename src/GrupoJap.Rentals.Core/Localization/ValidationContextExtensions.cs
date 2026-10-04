using System.ComponentModel.DataAnnotations;

namespace GrupoJap.Rentals.Localization;

public static class ValidationContextExtensions
{
    /// <summary>
    /// Traduz uma chave no idioma do pedido. O MVC não traduz as mensagens devolvidas por <see cref="IValidatableObject"/>,
    /// por isso os modelos usam este método. Fora de um pedido (ex.: testes) devolve a própria chave.
    /// </summary>
    public static string Text(this ValidationContext context, string key)
        => context.GetService(typeof(Translator)) is Translator translator ? translator[key].Value : key;
}
