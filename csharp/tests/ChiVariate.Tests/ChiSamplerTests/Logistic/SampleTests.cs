using System.Globalization;
using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Logistic;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 250_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Logistic
    [InlineData(5.0, 2.0)] // Shifted and scaled
    [InlineData(-10.0, 0.5)] // Shifted and narrow
    public void Sample_ProducesDistributionWithCorrectStatistics(double location, double scale)
    {
        var rng = new ChiRng($"Logistic_loc={location}_scale={scale}");

        var stdDev = scale * Math.PI / Math.Sqrt(3.0);
        var minBound = location - 6 * stdDev;
        var maxBound = location + 6 * stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);

        for (var i = 0; i < SampleCount; i++)
            histogram.AddSample(rng.Logistic(location, scale).Sample());

        histogram.DebugPrint(testOutputHelper, $"Logistic(μ={location}, s={scale}) Distribution");
        histogram.AssertIsLogistic(location, scale, 0.1);
    }

    [Theory]
    [InlineData("5.0", "2.0")] // Shifted and scaled
    [InlineData("-10.0", "0.5")] // Shifted and narrow
    public void Sample_Decimal_ProducesDistributionWithCorrectStatistics(string locationStr, string scaleStr)
    {
        var location = decimal.Parse(locationStr, CultureInfo.InvariantCulture);
        var scale = decimal.Parse(scaleStr, CultureInfo.InvariantCulture);

        var rng = new ChiRng(ChiSeed.Scramble("LogisticDecimal", new ChiHash().Add(location).Add(scale).Hash));

        var stdDev = (double)(scale * (decimal)Math.PI / (decimal)Math.Sqrt(3.0));
        var minBound = (double)location - 6 * stdDev;
        var maxBound = (double)location + 6 * stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);
        var sampler = new DecimalLogisticSampler(location, scale);

        histogram.Generate<decimal, ChiRng, DecimalLogisticSampler>(ref rng, 50_000, sampler);

        histogram.DebugPrint(testOutputHelper, $"Logistic(μ={location}, s={scale}) Distribution");
        histogram.AssertIsLogistic(location, scale, 0.15);
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var sampler = rng.Logistic(0.0, 1.0);
        _ = sampler.Sample(rng.Chance().PickBetween(100, 1000)).ToList();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);
        var samplerClone = rngClone.Logistic(0.0, 1.0);

        for (var i = 0; i < 10_000; i++)
            sampler.Sample().Should().Be(samplerClone.Sample());
    }

    private readonly struct DecimalLogisticSampler(decimal location, decimal scale) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Logistic(location, scale).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}