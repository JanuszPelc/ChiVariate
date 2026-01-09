using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Uniform;

/// <summary>
///     Tests for Uniform distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void Sample_StandardRange_ProducesUniformDistribution()
    {
        var min = ChiFixed.Zero;
        var max = ChiFixed.One;

        var rng = new ChiRng("UniformFixed");
        var histogram = new Histogram(0, 1, 10);
        var sampler = new ChiFixedUniformSampler(min, max);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedUniformSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Uniform(0, 1) using ChiFixed");
        histogram.AssertIsUniform(0.05);
    }

    [Fact]
    public void Sample_ShiftedRange_ProducesUniformDistribution()
    {
        var min = (ChiFixed)(-100m);
        var max = (ChiFixed)100m;

        var rng = new ChiRng("UniformShiftedFixed");
        var histogram = new Histogram(-100, 100, 20);
        var sampler = new ChiFixedUniformSampler(min, max);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedUniformSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Uniform(-100, 100) using ChiFixed");
        histogram.AssertIsUniform(0.10);
    }

    [Fact]
    public void Sample_AlwaysStaysWithinBounds()
    {
        var min = (ChiFixed)10m;
        var max = (ChiFixed)20m;

        var rng = new ChiRng("UniformBoundsFixed");

        for (var i = 0; i < 10000; i++)
        {
            var sample = rng.Uniform(min, max).Sample();
            sample.Should().BeGreaterThanOrEqualTo(min).And.BeLessThan(max);
        }
    }

    private readonly struct ChiFixedUniformSampler(ChiFixed min, ChiFixed max) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Uniform(min, max).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}