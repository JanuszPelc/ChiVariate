using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Geometric;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 200_000;

    [Theory]
    [InlineData(0.5)] // Expected mean = 2
    [InlineData(0.25)] // Expected mean = 4
    [InlineData(0.1)] // Expected mean = 10
    public void Sample_ProducesDistributionWithCorrectMean(double probability)
    {
        var rng = new ChiRng(ChiSeed.Scramble("Geometric", probability));
        var expectedMean = 1.0 / probability;
        var maxBound = (int)Math.Ceiling(expectedMean * 15);
        var histogram = new Histogram(1, maxBound, maxBound - 1, true);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Geometric(probability).Sample();
            histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper, $"Geometric(p={probability})");
        histogram.AssertIsGeometric(probability, 0.05);
    }

    [Fact]
    public void Geometric_WithProbabilityOne_AlwaysReturnsOne()
    {
        var rng = new ChiRng(0);
        for (var i = 0; i < 100; i++) rng.Geometric(1.0).Sample().Should().Be(1);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void Geometric_WithInvalidProbability_ThrowsArgumentOutOfRangeException(double invalidProbability)
    {
        var rng = new ChiRng(0);

        Action act = () => rng.Geometric(invalidProbability).Sample();

        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("probability");
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var sampler = rng.Geometric(0.3);
        _ = sampler.Sample(rng.Chance().PickBetween(100, 1000)).ToList();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);
        var samplerClone = rngClone.Geometric(0.3);

        for (var i = 0; i < 10_000; i++)
            sampler.Sample().Should().Be(samplerClone.Sample());
    }
}