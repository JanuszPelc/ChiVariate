using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Uniform;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void Sample_WithStandardRange_ProducesUniformDistribution()
    {
        var rng = new ChiRng("BoundedUniform_Standard");
        const double min = 0.0;
        const double max = 1.0;
        var histogram = new Histogram(min, max, 10);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Uniform(min, max).Sample();
            histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsUniform(0.05);
    }

    [Fact]
    public void Sample_WithShiftedRange_ProducesUniformDistribution()
    {
        var rng = new ChiRng("BoundedUniform_Shifted");
        const float min = -100.0f;
        const float max = 100.0f;
        var histogram = new Histogram(min, max, 20);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Uniform(min, max).Sample();
            histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsUniform(0.10);
    }

    [Fact]
    public void Sample_AcrossBoundedRange_StaysWithinBounds()
    {
        var rng = new ChiRng("BoundedUniform_BoundsCheck");
        const double min = 10.0;
        const double max = 20.0;

        for (var i = 0; i < 10000; i++)
        {
            var sample = rng.Uniform(min, max).Sample();
            sample.Should().BeGreaterThanOrEqualTo(min).And.BeLessThan(max);
        }
    }

    [Fact]
    public void Uniform_WithInvalidRange_ThrowsArgumentOutOfRangeException()
    {
        var rng = new ChiRng(0);

        Action act = () => rng.Uniform(10.0, 5.0).Sample();

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
            _ = rng.Uniform(1, 101).Sample();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 10_000; i++)
            rng.Uniform(1, 101).Sample().Should().Be(rngClone.Uniform(1, 101).Sample());
    }
}