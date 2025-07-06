using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Normal;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void Sample_UsingPreconstructedSampler_ProducesNormalDistribution()
    {
        // Arrange
        var rng = new ChiRng(42);
        var sampler = rng.Normal(0.0, 1.0);
        var histogram = new Histogram(-4.0, 4.0, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = sampler.Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsNormal(0.0, 1.0, 0.005);
    }

    [Fact]
    public void Sample_UsingFluentInterface_ProducesNormalDistribution()
    {
        // Arrange
        var rng = new ChiRng(42);
        var histogram = new Histogram(-4.0, 4.0, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Normal(0.0, 1.0).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsNormal(0.0, 1.0, 0.02);
    }

    [Fact]
    public void Sample_ShiftedDistribution_IsCorrectlyCentered()
    {
        // Arrange
        const double expectedMean = 50.0;
        const double expectedStdDev = 1.0;
        var rng = new ChiRng(ChiHashObsolete.Hash("ShiftedNormal"));
        var histogram = new Histogram(45.0, 55.0, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Normal(expectedMean, expectedStdDev).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsNormal(expectedMean, expectedStdDev, 0.01);
    }

    [Fact]
    public void Sample_WiderDistribution_HasCorrectSpread()
    {
        // Arrange
        const double expectedMean = 0.0;
        const double expectedStdDev = 15.0;
        var rng = new ChiRng(ChiHashObsolete.Hash("WiderNormal"));
        var histogram = new Histogram(-60.0, 60.0, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Normal(expectedMean, expectedStdDev).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsNormal(expectedMean, expectedStdDev, 0.01);
    }

    [Theory]
    [InlineData("50.0", "1.0")] // Shifted
    [InlineData("0.0", "15.0")] // Wider
    public void Sample_Decimal_ProducesNormalDistribution(string meanStr, string stdDevStr)
    {
        // Arrange
        var mean = decimal.Parse(meanStr);
        var stdDev = decimal.Parse(stdDevStr);

        var rng = new ChiRng(ChiSeed.Scramble("NormalDecimal", ChiHashObsolete.Hash(mean, stdDev)));

        var minBound = (double)mean - 4 * (double)stdDev;
        var maxBound = (double)mean + 4 * (double)stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);
        var sampler = new DecimalNormalSampler(mean, stdDev);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalNormalSampler>(ref rng, 200_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Normal(μ={mean}, σ={stdDev})");
        histogram.AssertIsNormal((double)mean, (double)stdDev, 0.02);
    }

    private readonly struct DecimalNormalSampler(decimal mean, decimal stdDev) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Normal(mean, stdDev).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}