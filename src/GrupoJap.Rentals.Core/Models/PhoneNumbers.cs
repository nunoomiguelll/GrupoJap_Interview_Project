namespace GrupoJap.Rentals.Models;

/// <summary>Regras dos números de telefone aceites (formato português).</summary>
public static class PhoneNumbers
{
    /// <summary>9 dígitos, a começar por 9 (ex.: 912345678).</summary>
    public const string PortuguesePattern = "^9[0-9]{8}$";
}
