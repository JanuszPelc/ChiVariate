using System.Globalization;
using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Beta;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(1.0, 1.0)] // Uniform case
    [InlineData(5.0, 5.0)] // Symmetric, bell-shaped case
    [InlineData(2.0, 5.0)] // Skewed right
    [InlineData(5.0, 2.0)] // Skewed left
    [InlineData(0.5, 0.5)] // U-shaped case
    public void Sample_ProducesDistributionWithCorrectMean(double alpha, double beta)
    {
        var rng = new ChiRng(ChiSeed.Scramble("Beta", alpha * 100 + beta));
        var expectedMean = alpha / (alpha + beta);
        var histogram = new Histogram(0, 1, 100);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Beta(alpha, beta).Sample();
            histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper, $"Beta(alpha={alpha}, beta={beta})");
        histogram.AssertIsBeta(expectedMean, 0.10);

        if (Math.Abs(alpha - 1.0) < 0.000001 && Math.Abs(beta - 1.0) < 0.000001)
            histogram.AssertIsUniform(0.15);
    }

    [Theory]
    [InlineData(0.0, 1.0)]
    [InlineData(-1.0, 1.0)]
    [InlineData(1.0, 0.0)]
    [InlineData(1.0, -1.0)]
    public void Beta_WithInvalidParameters_ThrowsArgumentOutOfRangeException(double alpha, double beta)
    {
        var rng = new ChiRng(0);

        Action act = () => rng.Beta(alpha, beta).Sample();

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("2.0", "5.0")] // Skewed case
    [InlineData("0.5", "0.5")] // U-shaped case
    public void Sample_Decimal_ProducesDistributionWithCorrectMean(string alphaStr, string betaStr)
    {
        var alpha = decimal.Parse(alphaStr, CultureInfo.InvariantCulture);
        var beta = decimal.Parse(betaStr, CultureInfo.InvariantCulture);
        var expectedMean = (double)alpha / (double)(alpha + beta);

        var rng = new ChiRng(ChiSeed.Scramble("BetaDecimal", new ChiHash().Add(alpha).Add(beta).Hash));
        var histogram = new Histogram(0, 1, 100);
        var sampler = new DecimalBetaSampler(alpha, beta);

        histogram.Generate<decimal, ChiRng, DecimalBetaSampler>(ref rng, 20_000, sampler);

        histogram.DebugPrint(testOutputHelper, $"Beta(alpha={alpha}, beta={beta})");
        histogram.AssertIsBeta(expectedMean, 0.15);
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        _ = rng.Beta(2.0, 5.0).Sample(rng.Chance().PickBetween(100, 1000)).ToList();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 100; i++)
            rng.Beta(2.0, 5.0).Sample().Should().Be(rngClone.Beta(2.0, 5.0).Sample());
    }

    private readonly struct DecimalBetaSampler(decimal alpha, decimal beta) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Beta(alpha, beta).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}