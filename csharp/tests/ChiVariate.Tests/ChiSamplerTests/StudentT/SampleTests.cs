using ChiVariate.Tests.TestInfrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.StudentT;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 50_000;

    [Theory]
    [InlineData(3.0)] // Has defined variance
    [InlineData(5.0)]
    [InlineData(30.0)] // Should look very similar to Normal(0,1)
    public void Sample_ProducesDistributionWithCorrectStatistics(double degreesOfFreedom)
    {
        // Arrange
        var rng = new ChiRng($"StudentT_v={degreesOfFreedom}");

        var variance = degreesOfFreedom / (degreesOfFreedom - 2.0);
        var stdDev = Math.Sqrt(variance);
        var minBound = -8 * stdDev;
        var maxBound = 8 * stdDev;
        var histogram = new Histogram(minBound, maxBound, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
            histogram.AddSample(rng.StudentT(degreesOfFreedom).Sample());

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Student's t (v={degreesOfFreedom}) Distribution");
        histogram.AssertIsStudentT(degreesOfFreedom, 0.17);
    }

    [Fact]
    public void Sample_With1DoF_MatchesCauchyDistribution()
    {
        // Arrange
        var tRng = new ChiRng("StudentT_vs_Cauchy");
        var cauchyRng = new ChiRng("StudentT_vs_Cauchy");

        var tHistogram = new Histogram(-20, 20, 200);
        var cauchyHistogram = new Histogram(-20, 20, 200);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            tHistogram.AddSample(tRng.StudentT(1.0).Sample());
            cauchyHistogram.AddSample(cauchyRng.Cauchy(0.0, 1.0).Sample());
        }

        // Assert
        var tMedian = tHistogram.CalculateMedian();
        var cauchyMedian = cauchyHistogram.CalculateMedian();
        tMedian.Should().BeApproximately(cauchyMedian, 0.2,
            "because a t(1) distribution should be equivalent to a Cauchy(0,1) distribution.");
    }

    [Theory]
    [InlineData("3.0")] // Has defined variance
    [InlineData("5.0")]
    public void Sample_Decimal_ProducesDistributionWithCorrectStatistics(string degreesOfFreedomStr)
    {
        // Arrange
        var degreesOfFreedom = decimal.Parse(degreesOfFreedomStr);
        var rng = new ChiRng(ChiSeed.Scramble("StudentTDecimal", (long)degreesOfFreedom));

        var v = (double)degreesOfFreedom;
        var variance = v / (v - 2.0);
        var stdDev = Math.Sqrt(variance);

        const double theoreticalMean = 0.0;
        const double rangeMultiplier = 15.0;
        var minBound = theoreticalMean - rangeMultiplier * stdDev;
        var maxBound = theoreticalMean + rangeMultiplier * stdDev;

        var histogram = new Histogram(minBound, maxBound, 100);
        var sampler = new DecimalStudentTSampler(degreesOfFreedom);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalStudentTSampler>(ref rng, 20_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Student's t (v={degreesOfFreedom}) Distribution");
        histogram.AssertIsStudentT(degreesOfFreedom, 0.2);
    }

    private readonly struct DecimalStudentTSampler(decimal degreesOfFreedom) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.StudentT(degreesOfFreedom).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}