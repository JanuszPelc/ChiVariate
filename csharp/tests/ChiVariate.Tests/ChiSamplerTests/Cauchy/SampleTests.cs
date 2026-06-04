using System.Globalization;
using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Cauchy;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Cauchy
    [InlineData(10.0, 5.0)] // Shifted and scaled
    [InlineData(0.0, 0.2)] // Narrower peak
    public void Sample_AcrossLocationAndScale_MatchesCauchyDistribution(double location, double scale)
    {
        var rng = new ChiRng(ChiSeed.Scramble("Cauchy", new ChiHash().Add(location).Add(scale).Hash));
        var histogramRange = 10 * scale;
        var minBound = location - histogramRange;
        var maxBound = location + histogramRange;

        var histogram = new Histogram(minBound, maxBound, 201);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Cauchy(location, scale).Sample();
            histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper, $"Cauchy(x₀={location}, γ={scale})");
        histogram.AssertIsCauchy(location, 0.1);
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.0, -1.0)]
    public void Cauchy_WithInvalidScale_ThrowsArgumentOutOfRangeException(double location, double scale)
    {
        var rng = new ChiRng(0);

        Action act = () => rng.Cauchy(location, scale).Sample();

        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("scale");
    }

    [Theory]
    [InlineData("0.0", "1.0")] // Standard Cauchy
    [InlineData("10.0", "5.0")] // Shifted and scaled
    public void Sample_Decimal_MatchesCauchyDistribution(string locationStr, string scaleStr)
    {
        var location = decimal.Parse(locationStr, CultureInfo.InvariantCulture);
        var scale = decimal.Parse(scaleStr, CultureInfo.InvariantCulture);

        var rng = new ChiRng(ChiSeed.Scramble("CauchyDecimal", new ChiHash().Add(location).Add(scale).Hash));
        var histogramRange = (double)(10 * scale);
        var minBound = (double)location - histogramRange;
        var maxBound = (double)location + histogramRange;

        var histogram = new Histogram(minBound, maxBound, 201);
        var sampler = new DecimalCauchySampler(location, scale);

        histogram.Generate<decimal, ChiRng, DecimalCauchySampler>(ref rng, 100_000, sampler);

        histogram.DebugPrint(testOutputHelper, $"Cauchy(x₀={location}, γ={scale})");
        histogram.AssertIsCauchy((double)location, 0.15);
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var warmupCount = rng.Chance().PickBetween(100, 1000);
        for (var i = 0; i < warmupCount; i++)
            _ = rng.Cauchy(0.0, 1.0).Sample();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 10_000; i++)
            rng.Cauchy(0.0, 1.0).Sample().Should().Be(rngClone.Cauchy(0.0, 1.0).Sample());
    }

    private readonly struct DecimalCauchySampler(decimal location, decimal scale) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Cauchy(location, scale).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}