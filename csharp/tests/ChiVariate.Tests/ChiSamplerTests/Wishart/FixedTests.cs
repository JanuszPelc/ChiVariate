using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Wishart;

/// <summary>
///     Tests for Wishart distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 5_000;

    [Fact]
    public void Sample_2D_IsCorrect()
    {
        const int degreesOfFreedom = 5;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { (ChiFixed)1m, (ChiFixed)0.5m },
            { (ChiFixed)0.5m, (ChiFixed)2m }
        });
        var rng = new ChiRng("Wishart_Fixed_2D");
        var wishart = rng.Wishart(degreesOfFreedom, scaleMatrix);
        var samples = new List<double[,]>(SampleCount);

        for (var i = 0; i < SampleCount; i++)
        {
            using var sample = wishart.Sample();
            var arr = sample.ToArray();
            var doubleArr = new double[arr.GetLength(0), arr.GetLength(1)];
            for (var r = 0; r < arr.GetLength(0); r++)
            for (var c = 0; c < arr.GetLength(1); c++)
                doubleArr[r, c] = double.CreateChecked(arr[r, c]);
            samples.Add(doubleArr);
        }

        samples.AssertIsWishart(degreesOfFreedom, new[,] { { 1.0, 0.5 }, { 0.5, 2.0 } }, 0.20);
    }

    [Fact]
    public void Sample_3D_WithIdentityScale_IsCorrect()
    {
        const int degreesOfFreedom = 10;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { (ChiFixed)1m, ChiFixed.Zero, ChiFixed.Zero },
            { ChiFixed.Zero, (ChiFixed)1m, ChiFixed.Zero },
            { ChiFixed.Zero, ChiFixed.Zero, (ChiFixed)1m }
        });
        var rng = new ChiRng("Wishart_Fixed_3D_Identity");
        var wishart = rng.Wishart(degreesOfFreedom, scaleMatrix);
        var samples = new List<double[,]>(SampleCount);

        for (var i = 0; i < SampleCount; i++)
        {
            using var sample = wishart.Sample();
            var arr = sample.ToArray();
            var doubleArr = new double[arr.GetLength(0), arr.GetLength(1)];
            for (var r = 0; r < arr.GetLength(0); r++)
            for (var c = 0; c < arr.GetLength(1); c++)
                doubleArr[r, c] = double.CreateChecked(arr[r, c]);
            samples.Add(doubleArr);
        }

        samples.AssertIsWishart(degreesOfFreedom,
            new[,] { { 1.0, 0.0, 0.0 }, { 0.0, 1.0, 0.0 }, { 0.0, 0.0, 1.0 } },
            0.20);
    }

    [Fact]
    public void Sample_DiagonalElements_FollowChiSquaredDistribution()
    {
        const int degreesOfFreedom = 10;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { (ChiFixed)1m, ChiFixed.Zero, ChiFixed.Zero },
            { ChiFixed.Zero, (ChiFixed)1m, ChiFixed.Zero },
            { ChiFixed.Zero, ChiFixed.Zero, (ChiFixed)1m }
        });
        var rng = new ChiRng(nameof(Sample_DiagonalElements_FollowChiSquaredDistribution) + "_Fixed");

        const double expectedMean = degreesOfFreedom;
        const double expectedVariance = 2.0 * degreesOfFreedom;
        var expectedStdDev = Math.Sqrt(expectedVariance);
        var maxBound = expectedMean + 8 * expectedStdDev;
        var histogram = new Histogram(0, maxBound, 100);

        for (var i = 0; i < SampleCount; i++)
        {
            using var sampleMatrix = rng.Wishart(degreesOfFreedom, scaleMatrix).Sample();
            histogram.AddSample(double.CreateChecked(sampleMatrix[0, 0]));
        }

        histogram.DebugPrint(testOutputHelper, "Wishart diagonal element (ChiFixed)");
        histogram.AssertIsChiSquared(degreesOfFreedom, 0.15, 0.20);
    }

    [Fact]
    public void Sample_IsSymmetric()
    {
        const int degreesOfFreedom = 5;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { (ChiFixed)1m, (ChiFixed)0.3m },
            { (ChiFixed)0.3m, (ChiFixed)1m }
        });
        var rng = new ChiRng("Wishart_Fixed_Symmetric");

        for (var i = 0; i < 100; i++)
        {
            using var sample = rng.Wishart(degreesOfFreedom, scaleMatrix).Sample();

            // Wishart samples should be symmetric matrices
            var arr = sample.ToArray();
            for (var r = 0; r < arr.GetLength(0); r++)
            for (var c = r + 1; c < arr.GetLength(1); c++)
            {
                var diff = Math.Abs(double.CreateChecked(arr[r, c]) - double.CreateChecked(arr[c, r]));
                Assert.True(diff < 1e-10, $"Wishart sample should be symmetric, but [{r},{c}] != [{c},{r}]");
            }
        }
    }
}