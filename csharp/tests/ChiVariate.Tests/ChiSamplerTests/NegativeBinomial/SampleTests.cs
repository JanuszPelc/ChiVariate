using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.NegativeBinomial;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(5, 0.5)] // Expected mean = 10
    [InlineData(10, 0.25)] // Expected mean = 40
    [InlineData(3, 0.8)] // Expected mean = 3.75
    public void Sample_AcrossSuccessesAndProbability_MatchesNegativeBinomialDistribution(int numSuccesses,
        double probability)
    {
        var rng = new ChiRng(
            ChiSeed.Scramble("NegativeBinomial", new ChiHash().Add(numSuccesses).Add(probability).Hash));
        var expectedMean = numSuccesses / probability;
        var maxBound = (int)Math.Ceiling(expectedMean * 3);
        var histogram = new Histogram(numSuccesses, maxBound, maxBound - numSuccesses, true);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.NegativeBinomial(numSuccesses, probability).Sample();
            histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper, $"NegativeBinomial(r={numSuccesses}, p={probability})");
        histogram.AssertIsNegativeBinomial(numSuccesses, probability, 0.1);
    }

    [Fact]
    public void Sample_WhenNumSuccessesIsOne_MatchesGeometric()
    {
        const double probability = 0.3;
        var negBinRng = new ChiRng(123);
        var geoRng = new ChiRng(123);

        var negBinHistogram = new Histogram(1, 50, 49, true);
        var geoHistogram = new Histogram(1, 50, 49, true);

        for (var i = 0; i < SampleCount; i++)
        {
            negBinHistogram.AddSample(negBinRng.NegativeBinomial(1, probability).Sample());
            geoHistogram.AddSample(geoRng.Geometric(probability).Sample());
        }

        var negBinMean = negBinHistogram.CalculateMean();
        var geoMean = geoHistogram.CalculateMean();

        negBinMean.Should().BeApproximately(geoMean, 0.01,
            "because a Negative Binomial with r=1 must be statistically equivalent to a Geometric distribution.");
    }

    [Theory]
    [InlineData(0, 0.5)]
    [InlineData(-1, 0.5)]
    [InlineData(5, 0.0)]
    [InlineData(5, 1.1)]
    public void NegativeBinomial_WithInvalidParameters_ThrowsArgumentOutOfRangeException(int numSuccesses,
        double probability)
    {
        var rng = new ChiRng(0);

        Action act = () => rng.NegativeBinomial(numSuccesses, probability).Sample();

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var warmupCount = rng.Chance().PickBetween(100, 1000);
        for (var i = 0; i < warmupCount; i++)
            _ = rng.NegativeBinomial(5, 0.5).Sample();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 10_000; i++)
            rng.NegativeBinomial(5, 0.5).Sample().Should().Be(rngClone.NegativeBinomial(5, 0.5).Sample());
    }
}