using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Zipf;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(10, 1.0)] // Classic Zipf, small N
    [InlineData(100, 1.0)] // Classic Zipf, larger N
    [InlineData(100, 1.5)] // More skewed
    [InlineData(50, 0.8)] // Less skewed
    public void Sample_ProducesDistributionWithCorrectShape(int numElements, double exponent)
    {
        var rng = new ChiRng(ChiSeed.Scramble("Zipf", new ChiHash().Add(numElements).Add(exponent).Hash));
        var histogram = new Histogram(1, numElements + 1, numElements, true);

        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Zipf(numElements, exponent).Sample();
            histogram.AddSample(sample);
        }

        histogram.DebugPrint(testOutputHelper, $"Zipf(N={numElements}, s={exponent})");
        histogram.AssertIsZipf(numElements);
    }

    [Theory]
    [InlineData(0, 1.0)]
    [InlineData(-10, 1.0)]
    [InlineData(10, 0.0)]
    [InlineData(10, -1.0)]
    public void Zipf_WithInvalidParameters_ThrowsArgumentOutOfRangeException(int numElements, double exponent)
    {
        var rng = new ChiRng(0);

        Action act = () => rng.Zipf(numElements, exponent).Sample();

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples()
    {
        var rng = new ChiRng("ZipfSnapshot");
        _ = rng.Zipf(100, 1.0).Sample(rng.Chance().PickBetween(100, 1000)).ToList();

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 100; i++)
            rng.Zipf(100, 1.0).Sample().Should().Be(rngClone.Zipf(100, 1.0).Sample());
    }
}