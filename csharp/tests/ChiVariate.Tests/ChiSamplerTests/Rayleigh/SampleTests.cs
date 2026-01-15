using System.Globalization;
using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Rayleigh;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 200_000;

    [Theory]
    [InlineData(1.0)]
    [InlineData(5.0)]
    [InlineData(0.5)]
    public void Sample_WithCorrectProperties_ProducesDistribution(double sigma)
    {
        var rng = new ChiRng(ChiSeed.Scramble("Rayleigh", sigma));
        var histogram = new Histogram(0, sigma * 6, 100);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Rayleigh(sigma).Sample();
            histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper, $"Rayleigh(sigma={sigma})");
        histogram.AssertIsRayleigh(sigma, 0.09);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Rayleigh_WithInvalidSigma_ThrowsArgumentOutOfRangeException(double invalidSigma)
    {
        var rng = new ChiRng(0);

        Action act = () => rng.Rayleigh(invalidSigma).Sample();

        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("sigma");
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var warmupCount = rng.Chance().PickBetween(100, 1000);
        for (var i = 0; i < warmupCount; i++)
            _ = rng.Rayleigh(1.0).Sample();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 10_000; i++)
            rng.Rayleigh(1.0).Sample().Should().Be(rngClone.Rayleigh(1.0).Sample());
    }

    [Theory]
    [InlineData("1.0")]
    [InlineData("5.0")]
    public void Sample_Decimal_WithCorrectProperties_ProducesDistribution(string sigmaStr)
    {
        var sigma = decimal.Parse(sigmaStr, CultureInfo.InvariantCulture);
        var rng = new ChiRng(ChiSeed.Scramble("RayleighDecimal", (long)(sigma * 10)));
        var histogram = new Histogram(0, (double)sigma * 6, 100);
        var sampler = new DecimalRayleighSampler(sigma);

        histogram.Generate<decimal, ChiRng, DecimalRayleighSampler>(ref rng, 200_000, sampler);

        histogram.DebugPrint(testOutputHelper, $"Rayleigh(sigma={sigma})");
        histogram.AssertIsRayleigh(sigma, 0.07);
    }

    private readonly struct DecimalRayleighSampler(decimal sigma)
        : IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Rayleigh(sigma).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}