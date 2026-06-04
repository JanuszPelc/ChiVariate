using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiFixedTests.Variate;

/// <summary>
///     Tests that verify ChiFixed works correctly with distributions that historically
///     had overflow/underflow issues. These tests will fail if the algorithmic fixes
///     are removed, serving as regression tests.
/// </summary>
public class FixedPointEdgeCaseTests
{
    /// <summary>
    ///     Logistic distribution with ChiFixed must not overflow.
    ///     The inverse CDF formula log(u/(1-u)) can produce extreme values when u approaches 0 or 1.
    ///     This test generates enough samples to statistically hit such extreme values.
    /// </summary>
    [Fact]
    public void Sample_LogisticExtremeTail_ReturnsFiniteValues()
    {
        var rng = new ChiRng(42);
        var location = ChiFixed.Zero;
        var scale = ChiFixed.One;

        // 10M samples should statistically trigger extreme u values
        // Probability of u > 0.9999995 is ~1 in 2M, so 10M gives ~5 expected hits
        for (var i = 0; i < 10_000_000; i++)
        {
            var sample = rng.Logistic(location, scale).Sample();

            // Just verify we get a finite value without throwing
            Assert.True(ChiFixed.IsFinite(sample), $"Sample {i} produced non-finite value: {sample}");
        }
    }

    /// <summary>
    ///     Gamma distribution with ChiFixed must not throw due to underflow.
    ///     In Marsaglia-Tsang algorithm, when xCubed is tiny, x = xCubed³ can underflow to 0,
    ///     causing log(x) to fail. This test generates enough samples to trigger such cases.
    /// </summary>
    [Fact]
    public void Sample_GammaUnderflowEdgeCase_ReturnsPositiveFiniteValues()
    {
        var rng = new ChiRng(42);
        var shape = (ChiFixed)2m;
        var scale = ChiFixed.One;

        // Generate many samples to trigger the underflow edge case
        for (var i = 0; i < 10_000_000; i++)
        {
            var sample = rng.Gamma(shape, scale).Sample();

            Assert.True(sample > ChiFixed.Zero, $"Sample {i} must be positive: {sample}");
            Assert.True(ChiFixed.IsFinite(sample), $"Sample {i} produced non-finite value: {sample}");
        }
    }
}