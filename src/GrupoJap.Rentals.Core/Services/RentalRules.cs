namespace GrupoJap.Rentals.Services;

/// <summary>Regras de negócio puras (sem acesso a dados) para alugueres. Datas inclusivas.</summary>
public static class RentalRules
{
    /// <summary>Dois períodos inclusivos sobrepõem-se quando inicioA &lt;= fimB e fimA &gt;= inicioB.</summary>
    public static bool PeriodsOverlap(DateOnly startA, DateOnly endA, DateOnly startB, DateOnly endB)
        => startA <= endB && endA >= startB;

    /// <summary>Um contrato está ativo no dia indicado quando inicio &lt;= dia &lt;= fim.</summary>
    public static bool IsActiveOn(DateOnly start, DateOnly end, DateOnly day)
        => start <= day && end >= day;

    public static DateOnly Today() => DateOnly.FromDateTime(DateTime.Today);

    public static RentalStatus GetStatus(DateOnly start, DateOnly end, DateOnly today)
        => today < start ? RentalStatus.Upcoming
         : today > end ? RentalStatus.Completed
         : RentalStatus.Active;
}

public enum RentalStatus { Upcoming, Active, Completed }
