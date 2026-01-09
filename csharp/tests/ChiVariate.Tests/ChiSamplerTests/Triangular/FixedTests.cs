using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Triangular;

/// <summary>
///     Tests for Triangular distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(0.0, 10.0, 5.0)] // Symmetric
    [InlineData(0.0, 10.0, 2.0)] // Left-skewed
    [InlineData(0.0, 10.0, 8.0)] // Right-skewed
    [InlineData(-10.0, 10.0, 0.0)] // Symmetric around zero
    public void Sample_WithVariousParameters_ProducesTriangularDistribution(double min, double max, double mode)
    {
        var mn = (ChiFixed)(decimal)min;
        var mx = (ChiFixed)(decimal)max;
        var md = (ChiFixed)(decimal)mode;

        var rng = new ChiRng(ChiSeed.Scramble("TriangularFixed", mode));
        var histogram = new Histogram(min, max, 100);
        var sampler = new ChiFixedTriangularSampler(mn, mx, md);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedTriangularSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"Triangular({min}, {max}, {mode}) using ChiFixed");
        histogram.AssertIsTriangular(min, max, mode, 0.05);
    }

    [Fact]
    public void Sample_SymmetricCase_HasModeAtCenter()
    {
        var min = ChiFixed.Zero;
        var max = (ChiFixed)10m;
        var mode = (ChiFixed)5m;

        var rng = new ChiRng("TriangularSymmetricFixed");
        var histogram = new Histogram(0, 10, 100);
        var sampler = new ChiFixedTriangularSampler(min, max, mode);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedTriangularSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "Triangular(0, 10, 5) using ChiFixed - symmetric");

        var actualMode = histogram.CalculateMode();
        Assert.InRange(actualMode, 4.5, 5.5);
    }

    private readonly struct ChiFixedTriangularSampler(ChiFixed min, ChiFixed max, ChiFixed mode) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.Triangular(min, max, mode).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}