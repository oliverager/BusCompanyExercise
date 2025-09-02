namespace BusPricing.Pricing;

public static class BusPriceCalculator
{
    // Recommendation: keep prices as decimal for currency math.
    private const decimal InitialFee = 2500m;
    private const decimal Tier1Rate = 10m; // 0–100 km (first 100 km)
    private const decimal Tier2Rate = 8m;  // 100–500 km (next 400 km)
    private const decimal Tier3Rate = 6m;  // >500 km

    /// <summary>
    /// Calculates the total price in kroner (kr) for a one-day bus trip with chauffeur.
    /// Pricing model:
    /// - Initial fee: 2500 kr
    /// - First 100 km: 10 kr/km
    /// - Next 400 km (km 100–500): 8 kr/km
    /// - Beyond 500 km: 6 kr/km
    /// Distances may be fractional (e.g., 100.5 km).
    /// </summary>
    /// <param name="distanceKm">Trip distance in kilometers. Must be >= 0.</param>
    /// <returns>Total price in kroner (kr).</returns>
    /// <exception cref="ArgumentOutOfRangeException">If distanceKm is negative.</exception>
    public static decimal Calculate(decimal distanceKm)
    {
        if (distanceKm < 0)
            throw new ArgumentOutOfRangeException(nameof(distanceKm), "Distance must be non-negative.");

        // Piecewise, cumulative (stepped) pricing
        var tier1Km = Math.Min(distanceKm, 100m);
        var tier2Km = Math.Min(Math.Max(distanceKm - 100m, 0m), 400m);   // up to 400 km in this tier
        var tier3Km = Math.Max(distanceKm - 500m, 0m);

        var price =
            InitialFee +
            tier1Km * Tier1Rate +
            tier2Km * Tier2Rate +
            tier3Km * Tier3Rate;

        return decimal.Round(price, 2, MidpointRounding.AwayFromZero);
    }
}