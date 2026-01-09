using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Weibull;

/// <summary>
///     Tests for Weibull distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(2.0, 5.0)] // k>1, increasing failure rate (Rayleigh-like)
    [InlineData(5.0, 10.0)] // k>>1, bell-shaped (approaching Normal)
    public void Sample_WithVariousParameters_ProducesWeibullDistribution(double shape, double scale)
    {
        var sh = (ChiFixed)(decimal)shape;
        var sc = (ChiFixed)(decimal)scale;

        var rng = new ChiRng(ChiSeed.Scramble("WeibullFixed", shape * 100 + scale));
        var maxBound = scale * 4;
        var histogram = new Histogram(0, maxBound, 150);
        var sampler = new ChiFixedWeibullSampler(sh, sc);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedWeibullSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Weibull(k={shape}, lambda={scale}) using ChiFixed");
        histogram.AssertIsWeibull(shape, scale, 0.20);
    }

    [Fact]
    public void Sample_ShapeTwo_HasRayleighLikeShape()
    {
        // Weibull(2, scale) is related to Rayleigh
        var shape = (ChiFixed)2m;
        var scale = (ChiFixed)5m;

        var rng = new ChiRng("WeibullRayleighFixed");
        var histogram = new Histogram(0, 15, 100);
        var sampler = new ChiFixedWeibullSampler(shape, scale);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedWeibullSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Weibull(k=2, lambda=5) using ChiFixed");

        // Mode should be positive (not at 0)
        var mode = histogram.CalculateMode();
        Assert.True(mode > 0, "Weibull with k>1 should have mode > 0");
    }

    private readonly struct ChiFixedWeibullSampler(ChiFixed shape, ChiFixed scale) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Weibull(shape, scale).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}