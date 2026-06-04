using System.Globalization;
using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Laplace;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 50_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Laplace
    [InlineData(10.0, 5.0)] // Shifted and scaled
    [InlineData(-5.0, 0.5)] // Shifted and narrow
    public void Sample_AcrossLocationAndScale_MatchesLaplaceDistribution(double location, double scale)
    {
        var rng = new ChiRng($"Laplace_loc={location}_scale={scale}");

        var maxBound = location + 10 * scale;
        var minBound = location - 10 * scale;
        var histogram = new Histogram(minBound, maxBound, 100);

        for (var i = 0; i < SampleCount; i++) histogram.AddSample(rng.Laplace(location, scale).Sample());

        histogram.DebugPrint(testOutputHelper, $"Laplace(μ={location}, b={scale}) Distribution");
        histogram.AssertIsLaplace(location, scale, 0.1);
    }

    [Theory]
    [InlineData("10.0", "5.0")] // Shifted and scaled
    [InlineData("-5.0", "0.5")] // Shifted and narrow
    public void Sample_Decimal_MatchesLaplaceDistribution(string locationStr, string scaleStr)
    {
        var location = decimal.Parse(locationStr, CultureInfo.InvariantCulture);
        var scale = decimal.Parse(scaleStr, CultureInfo.InvariantCulture);

        var rng = new ChiRng(ChiSeed.Scramble("LaplaceDecimal", new ChiHash().Add(location).Add(scale).Hash));

        var stdDev = (double)scale * Math.Sqrt(2.0);
        var minBound = (double)location - 8 * stdDev;
        var maxBound = (double)location + 8 * stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);
        var sampler = new DecimalLaplaceSampler(location, scale);

        histogram.Generate<decimal, ChiRng, DecimalLaplaceSampler>(ref rng, 50_000, sampler);

        histogram.DebugPrint(testOutputHelper, $"Laplace(μ={location}, b={scale}) Distribution");
        histogram.AssertIsLaplace(location, scale, 0.15);
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var warmupCount = rng.Chance().PickBetween(100, 1000);
        for (var i = 0; i < warmupCount; i++)
            _ = rng.Laplace(0.0, 1.0).Sample();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 10_000; i++)
            rng.Laplace(0.0, 1.0).Sample().Should().Be(rngClone.Laplace(0.0, 1.0).Sample());
    }

    private readonly struct DecimalLaplaceSampler(decimal location, decimal scale) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Laplace(location, scale).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}