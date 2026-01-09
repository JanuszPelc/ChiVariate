using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.StudentT;

/// <summary>
///     Tests for Student's t distribution using ChiFixed type.
///     StudentT uses Normal and Chi-squared internally, testing the full Ziggurat chain.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(3.0)] // Has defined variance, heavy tails
    [InlineData(5.0)] // Moderate tails
    [InlineData(30.0)] // Approaches Normal(0, 1)
    public void Sample_WithVariousDoF_ProducesStudentTDistribution(double degreesOfFreedom)
    {
        var df = (ChiFixed)(decimal)degreesOfFreedom;

        var rng = new ChiRng(ChiSeed.Scramble("StudentTFixed", (long)degreesOfFreedom));
        var variance = degreesOfFreedom / (degreesOfFreedom - 2.0);
        var stdDev = Math.Sqrt(variance);
        var minBound = -8 * stdDev;
        var maxBound = 8 * stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);
        var sampler = new ChiFixedStudentTSampler(df);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedStudentTSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, $"StudentT(v={degreesOfFreedom}) using ChiFixed");
        histogram.AssertIsStudentT(degreesOfFreedom, 0.2);
    }

    [Fact]
    public void Sample_LargeDoF_ApproachesStandardNormal()
    {
        // StudentT(30) should be very close to Normal(0, 1)
        var df = (ChiFixed)30m;

        var rng = new ChiRng("StudentTLargeDoFFixed");
        var histogram = new Histogram(-4, 4, 100);
        var sampler = new ChiFixedStudentTSampler(df);

        histogram.Generate<ChiFixed, ChiRng, ChiFixedStudentTSampler>(ref rng, SampleCount, sampler);

        histogram.DebugPrint(testOutputHelper, "StudentT(v=30) using ChiFixed - should approach N(0,1)");

        var actualMean = histogram.CalculateMean();
        var actualStdDev = histogram.CalculateStdDev(actualMean);

        // Should be close to Normal(0, 1)
        Assert.InRange(actualMean, -0.05, 0.05);
        Assert.InRange(actualStdDev, 0.95, 1.10);
    }

    private readonly struct ChiFixedStudentTSampler(ChiFixed degreesOfFreedom) :
        IHistogramSamplerWithRange<ChiFixed, ChiRng>
    {
        public ChiFixed NextSample(ref ChiRng rng)
        {
            return rng.StudentT(degreesOfFreedom).Sample();
        }

        public double Normalize(ChiFixed value)
        {
            return double.CreateChecked(value);
        }
    }
}