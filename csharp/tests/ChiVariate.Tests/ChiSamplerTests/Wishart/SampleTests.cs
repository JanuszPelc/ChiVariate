using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Wishart;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 10_000;

    [Fact]
    public void Sample_2D_IsCorrect()
    {
        // Arrange
        const int degreesOfFreedom = 5;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { 1.0, 0.5 },
            { 0.5, 2.0 }
        });
        var rng = new ChiRng("Wishart_2D");
        var wishart = rng.Wishart(degreesOfFreedom, scaleMatrix);
        var samples = new List<double[,]>(SampleCount);

        // Act
        for (var i = 0; i < SampleCount; i++)
            samples.Add(wishart.Sample().ToArray());

        // Assert
        samples.AssertIsWishart(degreesOfFreedom, scaleMatrix.ToArray(), 0.15);
    }

    [Fact]
    public void Sample_3D_WithIdentityScale_IsCorrect()
    {
        // Arrange
        const int degreesOfFreedom = 10;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { 1.0, 0.0, 0.0 },
            { 0.0, 1.0, 0.0 },
            { 0.0, 0.0, 1.0 }
        });
        var rng = new ChiRng("Wishart_3D_Identity");
        var wishart = rng.Wishart(degreesOfFreedom, scaleMatrix);

        // Act
        var samples = wishart
            .Sample(SampleCount)
            .Select(matrix =>
            {
                using (matrix)
                {
                    return matrix.ToArray();
                }
            })
            .ToList();

        // Assert
        samples.AssertIsWishart(degreesOfFreedom, scaleMatrix.ToArray(), 0.15);
    }

    [Fact]
    public void Sample_DiagonalElements_FollowChiSquaredDistribution()
    {
        // Arrange
        const int degreesOfFreedom = 10;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { 1.0, 0.0, 0.0 },
            { 0.0, 1.0, 0.0 },
            { 0.0, 0.0, 1.0 }
        });
        var rng = new ChiRng(nameof(Sample_DiagonalElements_FollowChiSquaredDistribution));

        const double expectedMean = degreesOfFreedom;
        const double expectedVariance = 2.0 * degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var maxBound = expectedMean + 8 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            var sampleMatrix = rng.Wishart(degreesOfFreedom, scaleMatrix).Sample();
            histogram.AddSample(sampleMatrix[0, 0]);
        }

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsChiSquared(degreesOfFreedom, 0.1, 0.15);
    }

    [Fact]
    public void Sample_Decimal_IsCorrect()
    {
        // Arrange
        const int degreesOfFreedom = 5;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { 1.0m, 0.5m },
            { 0.5m, 2.0m }
        });
        var rng = new ChiRng("Wishart_Decimal_2D");

        var samples = new List<decimal[,]>(SampleCount);

        // Act
        for (var i = 0; i < 1_000; i++)
            samples.Add(rng.Wishart(degreesOfFreedom, scaleMatrix).Sample().ToArray());

        // Assert
        samples.AssertIsWishart(degreesOfFreedom, scaleMatrix.ToArray(), 0.25);
    }
}