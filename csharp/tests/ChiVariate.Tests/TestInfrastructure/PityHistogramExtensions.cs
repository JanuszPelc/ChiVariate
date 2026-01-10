using AwesomeAssertions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.TestInfrastructure;

public static class PityHistogramExtensions
{
    /// <summary>
    ///     Asserts that the histogram represents a valid Pity (PRD) distribution
    ///     of "trials until success".
    /// </summary>
    /// <param name="histogram">Histogram of trial counts until success.</param>
    /// <param name="baseProbability">Base probability of success.</param>
    /// <param name="hardCap">Hard pity cap (0 means no cap).</param>
    /// <param name="tolerance">Relative tolerance for mean comparison.</param>
    public static void AssertIsPity(
        this Histogram histogram,
        double baseProbability,
        int hardCap,
        double tolerance)
    {
        histogram.TotalSamples.Should().BeGreaterThan(0,
            "because samples must be generated before asserting.");

        var actualMean = histogram.CalculateMean();
        var geometricMean = 1.0 / baseProbability;

        // With escalating probability, mean should be less than pure geometric
        actualMean.Should().BeLessThan(geometricMean,
            "because PRD escalation should reduce average trials compared to pure geometric.");

        // Hard cap validation
        if (hardCap > 0)
        {
            // All samples should be <= hard cap (bins are 0-indexed, so bin[hardCap] = value hardCap+1)
            for (var i = hardCap; i < histogram.Bins.Length; i++)
                histogram.Bins[i].Should().Be(0,
                    $"because hard cap at {hardCap} should prevent values greater than {hardCap}.");

            // Mean should be significantly less than hard cap
            actualMean.Should().BeLessThan(hardCap * 0.9,
                "because most samples should succeed before hard cap.");
        }

        // Variance should be lower than geometric (PRD reduces streaks)
        var actualStdDev = histogram.CalculateStdDev(actualMean);
        var geometricStdDev = Math.Sqrt((1 - baseProbability) / (baseProbability * baseProbability));

        actualStdDev.Should().BeLessThan(geometricStdDev * (1 + tolerance),
            "because PRD should have lower variance than pure geometric distribution.");
    }

    /// <summary>
    ///     Asserts that the histogram shows proper hard cap behavior.
    /// </summary>
    public static void AssertHardCapEnforced(this Histogram histogram, int hardCap)
    {
        histogram.TotalSamples.Should().BeGreaterThan(0,
            "because samples must be generated before asserting.");

        // Find max value in histogram by finding last non-zero bin
        var maxBin = -1;
        for (var i = histogram.Bins.Length - 1; i >= 0; i--)
            if (histogram.Bins[i] > 0)
            {
                maxBin = i;
                break;
            }

        // For discrete histogram starting at 1, bin index i = value i+1
        // So max value = maxBin + 1
        var maxValue = maxBin + 1;
        maxValue.Should().BeLessThan(hardCap + 1,
            $"because hard cap at {hardCap} should guarantee success.");
    }
}