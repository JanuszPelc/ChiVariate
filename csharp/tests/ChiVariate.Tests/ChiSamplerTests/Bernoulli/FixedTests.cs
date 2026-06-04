using AwesomeAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Bernoulli;

/// <summary>
///     Tests for Bernoulli distribution using ChiFixed probability parameter.
///     Bernoulli is unique among discrete distributions in supporting generic probability types.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.25)]
    [InlineData(0.50)]
    [InlineData(0.75)]
    public void Sample_WithFixedProbability_MatchesConfiguredProbability(double probability)
    {
        var p = (ChiFixed)(decimal)probability;

        var rng = new ChiRng(ChiSeed.Scramble("BernoulliFixed", probability));
        var successCount = 0;

        for (var i = 0; i < SampleCount; i++)
            successCount += rng.Bernoulli(p).Sample();

        var actualRate = (double)successCount / SampleCount;

        testOutputHelper.WriteLine($"Bernoulli(p={probability}) using ChiFixed");
        testOutputHelper.WriteLine($"Expected: {probability:P2}, Actual: {actualRate:P2}");

        actualRate.Should().BeApproximately(probability, 0.01,
            "because the success rate should match the ChiFixed probability.");
    }

    [Fact]
    public void Sample_WithProbabilityFromBeta_MatchesExpectedMean()
    {
        // Use ChiFixed Beta to generate ChiFixed probability for Bernoulli
        var alpha = (ChiFixed)2m;
        var beta = (ChiFixed)5m;
        var expectedMeanRate = 2.0 / (2.0 + 5.0); // alpha / (alpha + beta)

        var rng = new ChiRng("BernoulliBetaFixed");
        var successCount = 0;

        for (var i = 0; i < SampleCount; i++)
        {
            var p = rng.Beta(alpha, beta).Sample();
            successCount += rng.Bernoulli(p).Sample();
        }

        var actualRate = (double)successCount / SampleCount;

        testOutputHelper.WriteLine("Bernoulli with p ~ Beta(2, 5) using ChiFixed");
        testOutputHelper.WriteLine($"Expected mean rate: {expectedMeanRate:P2}, Actual: {actualRate:P2}");

        actualRate.Should().BeApproximately(expectedMeanRate, 0.02,
            "because the overall success rate should equal the mean of the Beta distribution.");
    }

    [Fact]
    public void Sample_ExtremeProbabilities_ProduceCertainOutcomes()
    {
        var rng = new ChiRng("BernoulliExtremeFixed");

        // Test p = 0 (always fail)
        var pZero = ChiFixed.Zero;
        var zeroSuccesses = 0;
        for (var i = 0; i < 1000; i++)
            zeroSuccesses += rng.Bernoulli(pZero).Sample();
        zeroSuccesses.Should().Be(0, "because p=0 should never succeed.");

        // Test p = 1 (always succeed)
        var pOne = ChiFixed.One;
        var oneSuccesses = 0;
        for (var i = 0; i < 1000; i++)
            oneSuccesses += rng.Bernoulli(pOne).Sample();
        oneSuccesses.Should().Be(1000, "because p=1 should always succeed.");
    }
}