using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Gamma;

/// <summary>
///     Tests for Gamma distribution using ChiFixed type.
///     Gamma uses Normal sampling internally via the Marsaglia-Tsang method.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.5, 2.0)] // Shape < 1, uses transformation
    [InlineData(2.0, 2.0)] // Shape = 2, small integer
    [InlineData(9.0, 2.0)] // Shape > 1, well-behaved case
    public void Sample_WithVariousShapes_ProducesGammaDistribution(double shape, double scale)
    {
        var s = (ChiFixed)(decimal)shape;
        var sc = (ChiFixed)(decimal)scale;

        var rng = new ChiRng(ChiSeed.Scramble("GammaFixed", shape * 100 + scale));
        var expectedMean = shape * scale;
        var expectedStdDev = Math.Sqrt(shape) * scale;
        var maxBound = expectedMean + 6 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);
        var sampler = new ChiFixedGammaSampler(s, sc);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedGammaSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Gamma(shape={shape}, scale={scale}) using ChiFixed");
        histogram.AssertIsGamma(expectedMean, expectedStdDev, 0.1);
    }

    [Fact]
    public void Sample_LargeShape_ApproachesNormalBehavior()
    {
        // For large shape, Gamma approaches Normal distribution
        var shape = (ChiFixed)20m;
        var scale = (ChiFixed)2m;

        var rng = new ChiRng("GammaLargeShapeFixed");
        var expectedMean = 40.0; // shape * scale = 20 * 2
        var expectedStdDev = Math.Sqrt(20.0) * 2.0; // sqrt(shape) * scale
        var histogram = new Histogram(0, expectedMean + 5 * expectedStdDev, 100);
        var sampler = new ChiFixedGammaSampler(shape, scale);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedGammaSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Gamma(20, 2) using ChiFixed - approaches Normal");

        var actualMean = histogram.CalculateMean();
        Assert.InRange(actualMean, expectedMean * 0.95, expectedMean * 1.05);
    }

    private readonly struct ChiFixedGammaSampler(ChiFixed shape, ChiFixed scale) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Gamma(shape, scale).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}