using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Exponential;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void Sample_WithRateOne_ProducesExpectedMean()
    {
        // Arrange
        const double rateLambda = 1.0;
        const double expectedMean = 1.0 / rateLambda;
        var rng = new ChiRng(ChiHashObsolete.Hash("Exponential_Rate1"));
        var histogram = new Histogram(0.0, 10.0, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Exponential(rateLambda).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsExponential(expectedMean, 0.05);
    }

    [Fact]
    public void Sample_WithDifferentRate_ProducesExpectedMean()
    {
        // Arrange
        const double rateLambda = 0.5;
        const double expectedMean = 1.0 / rateLambda;
        var rng = new ChiRng(ChiHashObsolete.Hash("Exponential_Rate0.5"));
        var histogram = new Histogram(0.0, 20.0, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sample = rng.Exponential(rateLambda).Sample();
            histogram.AddSample(sample);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsExponential(expectedMean, 0.05);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Exponential_WithInvalidRate_ThrowsArgumentOutOfRangeException(double invalidRate)
    {
        // Arrange
        var rng = new ChiRng(0);

        // Act
        Action act = () => rng.Exponential(invalidRate).Sample();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().And.ParamName.Should().Be("rateLambda");
    }

    [Fact]
    public void Sample_Decimal_ProducesExpectedMean()
    {
        // Arrange
        const decimal rateLambda = 1.0m;
        const double expectedMean = 1.0 / (double)rateLambda;

        var rng = new ChiRng(ChiHashObsolete.Hash("Exponential_Decimal_Rate1"));
        var histogram = new Histogram(0.0, 10.0, 100);
        var sampler = new DecimalExponentialSampler(rateLambda); // Use our new sampler

        // Act
        histogram.Generate<decimal, ChiRng, DecimalExponentialSampler>(ref rng, SampleCount, sampler);

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsExponential(expectedMean, 0.05);
    }

    private readonly struct DecimalExponentialSampler(decimal rateLambda) :
        IHistogramSamplerWithRange<decimal, ChiRng>
    {
        public decimal NextSample(ref ChiRng rng)
        {
            return rng.Exponential(rateLambda).Sample();
        }

        public double Normalize(decimal value)
        {
            return (double)value;
        }
    }
}