using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Logistic;

/// <summary>
///     Tests for Logistic distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.0, 1.0)] // Standard Logistic
    [InlineData(5.0, 2.0)] // Shifted and scaled
    [InlineData(-10.0, 0.5)] // Shifted and narrow
    public void Sample_AcrossLocationAndScale_ProducesLogisticDistribution(double location, double scale)
    {
        var loc = (ChiFixed)(decimal)location;
        var sc = (ChiFixed)(decimal)scale;

        var rng = new ChiRng(ChiSeed.Scramble("LogisticFixed", location * 100 + scale));
        var stdDev = scale * Math.PI / Math.Sqrt(3.0);
        var minBound = location - 6 * stdDev;
        var maxBound = location + 6 * stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);
        var sampler = new ChiFixedLogisticSampler(loc, sc);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedLogisticSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Logistic(mu={location}, s={scale}) using ChiFixed");
        histogram.AssertIsLogistic(location, scale, 0.15);
    }

    [Fact]
    public void Sample_StandardLogistic_ProducesMeanNearZero()
    {
        var location = ChiFixed.Zero;
        var scale = ChiFixed.One;

        var rng = new ChiRng("LogisticStandardFixed");
        var histogram = new Histogram(-10, 10, 100);
        var sampler = new ChiFixedLogisticSampler(location, scale);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedLogisticSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Logistic(0, 1) using ChiFixed");

        var mean = histogram.CalculateMean();
        Assert.InRange(mean, -0.05, 0.05);
    }

    private readonly struct ChiFixedLogisticSampler(ChiFixed location, ChiFixed scale) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Logistic(location, scale).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}