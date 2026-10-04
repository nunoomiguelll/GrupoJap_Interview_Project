using GrupoJap.Rentals.Services;

namespace GrupoJap.Rentals.Tests;

public class RentalRulesTests
{
    private static DateOnly D(int day) => new(2030, 1, day);

    [Theory]
    [InlineData(1, 5, 6, 10, false)]   // consecutivos sem partilha de dias
    [InlineData(1, 5, 5, 10, true)]    // partilham o último dia (inclusivo)
    [InlineData(1, 10, 3, 4, true)]    // um contém o outro
    [InlineData(3, 4, 1, 10, true)]    // contido
    [InlineData(1, 5, 10, 12, false)]  // disjuntos
    [InlineData(10, 12, 1, 5, false)]
    public void PeriodsOverlap_UsesInclusiveDates(int sA, int eA, int sB, int eB, bool expected)
        => Assert.Equal(expected, RentalRules.PeriodsOverlap(D(sA), D(eA), D(sB), D(eB)));

    [Theory]
    [InlineData(5, 10, 5, true)]
    [InlineData(5, 10, 10, true)]
    [InlineData(5, 10, 7, true)]
    [InlineData(5, 10, 4, false)]
    [InlineData(5, 10, 11, false)]
    public void IsActiveOn_IsInclusive(int start, int end, int day, bool expected)
        => Assert.Equal(expected, RentalRules.IsActiveOn(D(start), D(end), D(day)));

    [Theory]
    [InlineData(10, 15, 5, RentalStatus.Upcoming)]
    [InlineData(10, 15, 12, RentalStatus.Active)]
    [InlineData(10, 15, 20, RentalStatus.Completed)]
    public void GetStatus_ReflectsToday(int start, int end, int today, RentalStatus expected)
        => Assert.Equal(expected, RentalRules.GetStatus(D(start), D(end), D(today)));
}
