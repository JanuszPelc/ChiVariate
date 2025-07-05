using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.ChiSquared;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(1)] // Looks like an Exponential distribution
    [InlineData(2)] // Is an Exponential distribution
    [InlineData(5)] // Skewed right
    [InlineData(10)] // Becoming more symmetric
    public void Sample_ProducesDistributionWithCorrectStatistics(int degreesOfFreedom)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("ChiSquared", degreesOfFreedom));

        var expectedMean = (double)degreesOfFreedom;
        var expectedVariance = 2.0 * degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(expectedVariance);

        var maxBound = expectedMean + 5 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 150);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.ChiSquared((double)degreesOfFreedom).Sample();
            if (sample < maxBound) histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper, $"ChiSquared(k={degreesOfFreedom})");
        var varianceTolerance = degreesOfFreedom == 1 ? 0.2 : 0.15;
        histogram.AssertIsChiSquared(degreesOfFreedom, 0.1, varianceTolerance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ChiSquared_WithInvalidDof_ThrowsArgumentOutOfRangeException(int degreesOfFreedom)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.ChiSquared((float)degreesOfFreedom).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("degreesOfFreedom");
    }

    [Theory]
    [InlineData("2")]
    [InlineData("5")]
    public void Sample_Decimal_ProducesDistributionWithCorrectStatistics(string degreesOfFreedomStr)
    {
        // Arrange
        var degreesOfFreedom = decimal.Parse(degreesOfFreedomStr);
        var intDof = (int)degreesOfFreedom;
        var rng = new ChiRng(ChiSeed.Scramble("ChiSquaredDecimal", (long)degreesOfFreedom));

        var expectedMean = (double)degreesOfFreedom;
        var expectedVariance = 2.0 * (double)degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(expectedVariance);

        var maxBound = expectedMean + 5 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 150);
        var sampler = new DecimalChiSquaredSampler(degreesOfFreedom);

        // Act
        histogram.Generate<decimal, ChiRng, DecimalChiSquaredSampler>(ref rng, 20_000, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"ChiSquared(k={degreesOfFreedom})");
        histogram.AssertIsChiSquared(intDof, 0.1, 0.2, 0.005);
    }

    private readonly struct DecimalChiSquaredSampler(decimal degreesOfFreedom) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.ChiSquared(degreesOfFreedom).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}