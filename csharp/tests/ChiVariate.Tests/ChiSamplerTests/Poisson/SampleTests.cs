using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Poisson;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.5)]
    [InlineData(5.0)]
    [InlineData(20.0)]
    public void Sample_WithCorrectMean_ProducesDistribution(double mean)
    {
        var rng = new ChiRng(ChiSeed.Scramble("Poisson", mean));
        var maxBound = (int)Math.Ceiling(mean + 5 * Math.Sqrt(mean));
        var histogram = new Histogram(0, maxBound, maxBound, true);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = (double)rng.Poisson(mean).Sample();
            histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsPoisson(mean, 0.05);
    }

    [Theory]
    [InlineData(-double.Epsilon)]
    [InlineData(-10.0)]
    public void Poisson_WithInvalidMean_ThrowsArgumentOutOfRangeException(double invalidMean)
    {
        var rng = new ChiRng(0);

        Action act = () => rng.Poisson(invalidMean).Sample();

        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("mean");
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var warmupCount = rng.Chance().PickBetween(100, 1000);
        for (var i = 0; i < warmupCount; i++)
            _ = rng.Poisson(5.0).Sample();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 10_000; i++)
            rng.Poisson(5.0).Sample().Should().Be(rngClone.Poisson(5.0).Sample());
    }
}