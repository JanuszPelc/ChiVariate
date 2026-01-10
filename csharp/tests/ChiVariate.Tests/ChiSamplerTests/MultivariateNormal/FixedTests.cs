using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.MultivariateNormal;

/// <summary>
///     Tests for Multivariate Normal distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 20_000;

    [Fact]
    public void Sample_2D_WithPositiveCorrelation_IsCorrect()
    {
        var mean = ChiMatrix.With([(ChiFixed)5m, (ChiFixed)(-10m)]);
        var covariance = ChiMatrix.With(new[,]
        {
            { (ChiFixed)4m, (ChiFixed)3.2m },
            { (ChiFixed)3.2m, (ChiFixed)16m }
        });
        var rng = new ChiRng("MvNormal_Fixed_PositiveCorr");
        var multivariateNormal = rng.MultivariateNormal(mean, covariance);
        var samples = new List<double[]>(SampleCount);

        foreach (var resultMatrix in multivariateNormal.Sample(SampleCount))
            using (resultMatrix)
            {
                var arr = resultMatrix.VectorToArray();
                samples.Add(arr.Select(x => double.CreateChecked(x)).ToArray());
            }

        samples.AssertIsMultivariateNormal([5.0, -10.0], new[,] { { 4.0, 3.2 }, { 3.2, 16.0 } }, 0.15);
    }

    [Fact]
    public void Sample_2D_WithNegativeCorrelation_IsCorrect()
    {
        var mean = ChiMatrix.With([(ChiFixed)0m, (ChiFixed)0m]);
        var covariance = ChiMatrix.With(new[,]
        {
            { (ChiFixed)1m, (ChiFixed)(-0.6m) },
            { (ChiFixed)(-0.6m), (ChiFixed)1m }
        });
        var rng = new ChiRng("MvNormal_Fixed_NegativeCorr");
        var multivariateNormal = rng.MultivariateNormal(mean, covariance);
        var samples = new List<double[]>(SampleCount);

        for (var i = 0; i < SampleCount; i++)
        {
            using var destination = multivariateNormal.Sample();
            var arr = destination.VectorToArray();
            samples.Add(arr.Select(x => double.CreateChecked(x)).ToArray());
        }

        samples.AssertIsMultivariateNormal([0.0, 0.0], new[,] { { 1.0, -0.6 }, { -0.6, 1.0 } }, 0.15);
    }

    [Fact]
    public void Sample_3D_Uncorrelated_IsCorrect()
    {
        var mean = ChiMatrix.With([(ChiFixed)10m, (ChiFixed)20m, (ChiFixed)30m]);
        var covariance = ChiMatrix.With(
            [(ChiFixed)1m, ChiFixed.Zero, ChiFixed.Zero],
            [ChiFixed.Zero, (ChiFixed)4m, ChiFixed.Zero],
            [ChiFixed.Zero, ChiFixed.Zero, (ChiFixed)9m]
        );
        var rng = new ChiRng(12345);
        var multivariateNormal = rng.MultivariateNormal(mean, covariance);
        var samples = new List<double[]>(SampleCount);

        for (var i = 0; i < SampleCount; i++)
        {
            using var destination = multivariateNormal.Sample();
            var arr = destination.VectorToArray();
            samples.Add(arr.Select(x => double.CreateChecked(x)).ToArray());
        }

        samples.AssertIsMultivariateNormal(
            [10.0, 20.0, 30.0],
            new[,] { { 1.0, 0.0, 0.0 }, { 0.0, 4.0, 0.0 }, { 0.0, 0.0, 9.0 } },
            0.15);
    }

    [Fact]
    public void Sample_MarginalDistributions_AreCorrectlyNormal()
    {
        var mean = ChiMatrix.With([(ChiFixed)10m, (ChiFixed)20m, (ChiFixed)30m]);
        var covariance = ChiMatrix.With(new[,]
        {
            { (ChiFixed)1m, ChiFixed.Zero, ChiFixed.Zero },
            { ChiFixed.Zero, (ChiFixed)4m, ChiFixed.Zero },
            { ChiFixed.Zero, ChiFixed.Zero, (ChiFixed)9m }
        });

        var rng = new ChiRng("MvNormal_Fixed_MarginalCheck");

        var histogramX1 = new Histogram(5, 15, 100);
        var histogramX2 = new Histogram(10, 30, 100);
        var histogramX3 = new Histogram(15, 45, 100);

        for (var i = 0; i < SampleCount; i++)
        {
            using var destination = rng.MultivariateNormal(mean, covariance).Sample();

            histogramX1.AddSample(double.CreateChecked(destination[0]));
            histogramX2.AddSample(double.CreateChecked(destination[1]));
            histogramX3.AddSample(double.CreateChecked(destination[2]));
        }

        const string methodName = nameof(Sample_MarginalDistributions_AreCorrectlyNormal);

        histogramX1.DebugPrint(testOutputHelper, $"{methodName} - Marginal Distribution for X1 (ChiFixed)");
        histogramX1.AssertIsNormal(10.0, 1.0, 0.15);

        histogramX2.DebugPrint(testOutputHelper, $"{methodName} - Marginal Distribution for X2 (ChiFixed)");
        histogramX2.AssertIsNormal(20.0, 2.0, 0.15);

        histogramX3.DebugPrint(testOutputHelper, $"{methodName} - Marginal Distribution for X3 (ChiFixed)");
        histogramX3.AssertIsNormal(30.0, 3.0, 0.15);
    }
}