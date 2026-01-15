using System.Globalization;
using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Weibull;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 150_000;

    [Theory]
    [InlineData(1.0, 5.0)] // k=1, identical to Exponential(1/5)
    [InlineData(0.5, 5.0)] // k<1, decreasing failure rate (L-shape)
    [InlineData(2.0, 5.0)] // k>1, increasing failure rate (Rayleigh-like)
    [InlineData(5.0, 10.0)] // k>>1, bell-shaped (approaching Normal)
    public void Sample_ProducesDistributionWithCorrectShape(double shape, double scale)
    {
        var rng = new ChiRng(ChiSeed.Scramble("Weibull", new ChiHash().Add(shape).Add(scale).Hash));
        var maxBound = scale * 10;
        var histogram = new Histogram(0, maxBound, 150);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Weibull(shape, scale).Sample();
            if (sample < maxBound) histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper, $"Weibull(k={shape}, λ={scale})");
        histogram.AssertIsWeibull(shape, scale, 0.15);
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(1.0, 0.0)]
    public void Weibull_WithInvalidParameters_ThrowsArgumentOutOfRangeException(double shape, double scale)
    {
        var rng = new ChiRng(0);

        Action act = () => rng.Weibull(shape, scale).Sample();

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var sampler = rng.Weibull(2.0, 5.0);
        _ = sampler.Sample(rng.Chance().PickBetween(100, 1000)).ToList();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);
        var samplerClone = rngClone.Weibull(2.0, 5.0);

        for (var i = 0; i < 10_000; i++)
            sampler.Sample().Should().Be(samplerClone.Sample());
    }

    [Theory]
    [InlineData("2.0", "5.0")] // k>1, increasing failure rate
    [InlineData("5.0", "10.0")] // k>>1, bell-shaped
    public void Sample_Decimal_ProducesDistributionWithCorrectShape(string shapeStr, string scaleStr)
    {
        var shape = decimal.Parse(shapeStr, CultureInfo.InvariantCulture);
        var scale = decimal.Parse(scaleStr, CultureInfo.InvariantCulture);

        var rng = new ChiRng(ChiSeed.Scramble("WeibullDecimal", new ChiHash().Add(shape).Add(scale).Hash));
        var maxBound = (double)scale * 4; // Weibull can have a long tail
        var histogram = new Histogram(0, maxBound, 150);
        var sampler = new DecimalWeibullSampler(shape, scale);

        histogram.Generate<decimal, ChiRng, DecimalWeibullSampler>(ref rng, 150_000, sampler);

        histogram.DebugPrint(testOutputHelper, $"Weibull(k={shape}, λ={scale})");
        histogram.AssertIsWeibull(shape, scale, 0.20);
    }

    private readonly struct DecimalWeibullSampler(decimal shape, decimal scale)
        : IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Weibull(shape, scale).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}