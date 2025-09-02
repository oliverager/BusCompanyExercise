using BusPricing.Pricing;
using FluentAssertions;

namespace BusPricing.Tests.Pricing;

public class BusPriceCalculatorTests
{
    [Theory]
    [InlineData(0.0,   2500.0)]
    [InlineData(0.5,   2500.0 + 0.5 * 10.0)]
    [InlineData(1.0,   2500.0 + 1.0 * 10.0)]
    [InlineData(99.9,  2500.0 + 99.9 * 10.0)]
    [InlineData(100.0, 2500.0 + 100.0 * 10.0)] // boundary between tier1 and tier2
    public void Calculates_Tier1_Correctly(double km, double expected)
    {
        BusPriceCalculator.Calculate((decimal)km).Should().Be((decimal)expected);
    }

    [Theory]
    // just into tier2
    [InlineData(100.1, 2500.0 + 100.0 * 10.0 + 0.1 * 8.0)]
    [InlineData(150.0, 2500.0 + 100.0 * 10.0 + 50.0 * 8.0)]
    [InlineData(500.0, 2500.0 + 100.0 * 10.0 + 400.0 * 8.0)] // boundary between tier2 and tier3
    public void Calculates_Tier2_Correctly(double km, double expected)
    {
        BusPriceCalculator.Calculate((decimal)km).Should().Be((decimal)expected);
    }

    [Theory]
    // into tier3
    [InlineData(500.1,   2500.0 + 100.0 * 10.0 + 400.0 * 8.0 + 0.1 * 6.0)]
    [InlineData(750.0,   2500.0 + 100.0 * 10.0 + 400.0 * 8.0 + 250.0 * 6.0)]
    [InlineData(1234.56, 2500.0 + 100.0 * 10.0 + 400.0 * 8.0 + 734.56 * 6.0)]
    public void Calculates_Tier3_Correctly(double km, double expected)
    {
        BusPriceCalculator.Calculate((decimal)km).Should().Be((decimal)expected);
    }

    [Fact]
    public void NegativeDistance_Throws()
    {
        var act = () => BusPriceCalculator.Calculate(-0.01m);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("distanceKm");
    }

    [Fact]
    public void Rounds_To_2_Decimals_AwayFromZero()
    {
        // Construct a distance that yields a repeating decimal to exercise rounding policy.
        decimal km = 100m + (8m / 3m) / 8m; 
        var price = BusPriceCalculator.Calculate(km);

        price.Should().Be(decimal.Round(
            2500m + 100m * 10m + (1m/3m) * 8m,
            2, MidpointRounding.AwayFromZero));
    }

    [Theory]
    // A few sanity checks to ensure monotonicity and no tier regressions
    [InlineData(50.0,  100.0)]
    [InlineData(100.0, 150.0)]
    [InlineData(300.0, 600.0)]
    [InlineData(500.0, 800.0)]
    [InlineData(800.0, 1600.0)]
    public void Additional_Kilometers_Never_Decrease_Marginal_Cost(double baseKm, double extraKm)
    {
        var basePrice  = BusPriceCalculator.Calculate((decimal)baseKm);
        var extraPrice = BusPriceCalculator.Calculate((decimal)(baseKm + extraKm));
        extraPrice.Should().BeGreaterThan(basePrice);
    }
}
