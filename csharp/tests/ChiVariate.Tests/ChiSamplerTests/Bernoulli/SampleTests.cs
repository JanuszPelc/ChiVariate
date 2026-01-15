using System.Globalization;
using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Bernoulli;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.25)]
    [InlineData(0.50)]
    [InlineData(0.75)]
    [InlineData(0.99)]
    public void Sample_WithFixedProbability_HasCorrectMean(double probability)
    {
        var rng = new ChiRng(ChiSeed.Scramble("BernoulliFixed", probability));
        var successCount = 0;

        for (var i = 0; i < SampleCount; i++) successCount += rng.Bernoulli(probability).Sample();

        var actualMean = (double)successCount / SampleCount;
        actualMean.Should().BeApproximately(probability, 0.01,
            "because the mean of a Bernoulli distribution (the frequency of successes) should be equal to p.");
    }

    [Theory]
    [InlineData(2.0, 2.0)] // Symmetric Beta, expected mean = 0.5
    [InlineData(5.0, 2.0)] // Skewed Beta, expected mean = 5/7 ≈ 0.714
    [InlineData(1.0, 9.0)] // Heavily skewed Beta, expected mean = 0.1
    public void Sample_WithVaryingProbabilityFromBeta_MatchesExpectedMean(double alpha, double beta)
    {
        var rng = new ChiRng(ChiSeed.Scramble("BernoulliVarying", alpha + beta * 100));
        var expectedMeanSuccessRate = alpha / (alpha + beta);

        var histogram = new Histogram(0, 1, 20);
        var successCount = 0;

        for (var i = 0; i < SampleCount; i++)
        {
            var p = rng.Beta(alpha, beta).Sample();
            histogram.AddSample(p);
            successCount += rng.Bernoulli(p).Sample();
        }

        histogram.DebugPrint(testOutputHelper, $"Input Probabilities from Beta({alpha}, {beta})");

        var actualMeanSuccessRate = (double)successCount / SampleCount;
        actualMeanSuccessRate.Should().BeApproximately(expectedMeanSuccessRate, 0.01,
            "because the overall success rate should equal the mean of the input Beta distribution.");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(double.NaN)]
    public void Bernoulli_WithInvalidProbability_ThrowsArgumentOutOfRangeException(double invalidProbability)
    {
        var rng = new ChiRng(0);

        Action act = () => rng.Bernoulli(invalidProbability).Sample();

        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("probability");
    }

    [Theory]
    [InlineData("0.25")]
    [InlineData("0.75")]
    public void Sample_Decimal_ProducesDistributionWithCorrectMean(string probabilityStr)
    {
        var probability = decimal.Parse(probabilityStr, CultureInfo.InvariantCulture);
        var rng = new ChiRng(ChiSeed.Scramble("BernoulliDecimal", (double)probability));
        var successCount = 0;

        for (var i = 0; i < SampleCount; i++) successCount += rng.Bernoulli(probability).Sample();

        var actualMean = (double)successCount / SampleCount;
        actualMean.Should().BeApproximately((double)probability, 0.01,
            "because the mean should be correct for high-precision decimal probabilities.");
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var warmupCount = rng.Chance().PickBetween(100, 1000);
        for (var i = 0; i < warmupCount; i++)
            _ = rng.Bernoulli(0.5).Sample();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 10_000; i++)
            rng.Bernoulli(0.5).Sample().Should().Be(rngClone.Bernoulli(0.5).Sample());
    }
}