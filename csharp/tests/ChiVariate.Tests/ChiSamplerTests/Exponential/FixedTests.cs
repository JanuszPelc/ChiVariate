using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Exponential;

/// <summary>
///     Tests for Exponential distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.5)] // Mean = 2.0
    [InlineData(1.0)] // Mean = 1.0
    [InlineData(2.0)] // Mean = 0.5
    public void Sample_WithVariousRates_ProducesExponentialDistribution(double rate)
    {
        var r = (ChiFixed)(decimal)rate;

        var rng = new ChiRng(ChiSeed.Scramble("ExponentialFixed", rate));
        var expectedMean = 1.0 / rate;
        var histogram = new Histogram(0, expectedMean * 6, 100);
        var sampler = new ChiFixedExponentialSampler(r);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedExponentialSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Exponential(rate={rate}) using ChiFixed");
        histogram.AssertIsExponential(expectedMean, 0.05);
    }

    [Fact]
    public void Sample_RateOne_HasMeanOfOne()
    {
        var rate = ChiFixed.One;

        var rng = new ChiRng("ExponentialRateOneFixed");
        var histogram = new Histogram(0, 10, 100);
        var sampler = new ChiFixedExponentialSampler(rate);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedExponentialSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Exponential(rate=1) using ChiFixed");
        histogram.AssertIsExponential(1.0, 0.05);
    }

    private readonly struct ChiFixedExponentialSampler(ChiFixed rate) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Exponential(rate).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}