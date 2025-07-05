using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.MultivariateNormal;

#pragma warning disable CS9113 // Parameter is unread.
public class SampleTests(ITestOutputHelper testOutputHelper)
#pragma warning restore CS9113 // Parameter is unread.
{
    private const int SampleCount = 50_000;

    [Fact]
    public void Sample_2D_WithPositiveCorrelation_IsCorrect()
    {
        // Arrange
        // A 2D distribution where X and Y tend to move together.
        var mean = ChiMatrix.With([5.0, -10.0]);
        var covariance = ChiMatrix.With(new[,]
        {
            { 4.0, 3.2 }, // Var(X)=4, Var(Y)=16, Cov(X,Y)=3.2 => Corr=0.8
            { 3.2, 16.0 }
        });
        var rng = new ChiRng("MvNormal_PositiveCorr");
        var multivariateNormal = rng.MultivariateNormal(mean, covariance);
        var samples = new List<double[]>(SampleCount);

        // Act
        foreach (var resultMatrix in multivariateNormal.Sample(SampleCount))
            using (resultMatrix)
            {
                samples.Add(resultMatrix.VectorToArray());
            }

        // Assert
        samples.AssertIsMultivariateNormal(mean.VectorToArray(), covariance.ToArray(), 0.1);
    }

    [Fact]
    public void Sample_2D_WithNegativeCorrelation_IsCorrect()
    {
        // Arrange
        // A 2D distribution where X and Y tend to move opposite.
        var mean = ChiMatrix.With([0.0, 0.0]);
        var covariance = ChiMatrix.With(new[,]
        {
            { 1.0, -0.6 }, // Var(X)=1, Var(Y)=1, Cov(X,Y)=-0.6 => Corr=-0.6
            { -0.6, 1.0 }
        });
        var rng = new ChiRng("MvNormal_NegativeCorr");
        var multivariateNormal = rng.MultivariateNormal(mean, covariance);
        var samples = new List<double[]>(SampleCount);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            using var destination = multivariateNormal.Sample();
            samples.Add(destination.VectorToArray());
        }

        // Assert
        samples.AssertIsMultivariateNormal(mean.VectorToArray(), covariance.ToArray(), 0.1);
    }

    [Fact]
    public void Sample_3D_Uncorrelated_IsCorrect()
    {
        // Arrange
        var mean = ChiMatrix.With([10.0, 20.0, 30.0]);
        var covariance = ChiMatrix.With(
            [1.0, 0.0, 0.0],
            [0.0, 4.0, 0.0],
            [0.0, 0.0, 9.0]
        );
        var rng = new ChiRng("MvNormal_Uncorrelated");
        var multivariateNormal = rng.MultivariateNormal(mean, covariance);
        var samples = new List<double[]>(SampleCount);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            using var destination = multivariateNormal.Sample();
            samples.Add(destination.VectorToArray());
        }

        // Assert
        samples.AssertIsMultivariateNormal(mean.VectorToArray(), covariance.ToArray(), 0.1);
    }

    [Fact]
    public void Sample_WithInvalidMatrix_ThrowsException()
    {
        // Arrange
        var rng = new ChiRng();
        var mean = ChiMatrix.With([0.0]);
        var covariance = ChiMatrix.With(new[,] { { -1.0 } }); // Not positive-definite

        // Act
        var act = () => { _ = rng.MultivariateNormal(mean, covariance); };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*not positive-definite*");
    }

    [Fact]
    public void Sample_MarginalDistributions_AreCorrectlyNormal()
    {
        // Arrange: Use the 3D uncorrelated case as a good example.
        var mean = ChiMatrix.With([10.0, 20.0, 30.0]);
        var covariance = ChiMatrix.With(new[,]
        {
            { 1.0, 0.0, 0.0 }, // Var(X1)=1 -> StdDev=1
            { 0.0, 4.0, 0.0 }, // Var(X2)=4 -> StdDev=2
            { 0.0, 0.0, 9.0 } // Var(X3)=9 -> StdDev=3
        });

        var rng = new ChiRng("MvNormal_MarginalCheck");

        // Create one histogram for each dimension
        var histogramX1 = new Histogram(5, 15, 100); // Mean 10, StdDev 1
        var histogramX2 = new Histogram(10, 30, 100); // Mean 20, StdDev 2
        var histogramX3 = new Histogram(15, 45, 100); // Mean 30, StdDev 3

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            using var destination = rng.MultivariateNormal(mean, covariance).Sample();

            histogramX1.AddSample(destination[0]);
            histogramX2.AddSample(destination[1]);
            histogramX3.AddSample(destination[2]);
        }

        // Assert
        const string methodName = nameof(Sample_MarginalDistributions_AreCorrectlyNormal);

        histogramX1.DebugPrint(testOutputHelper, $"{methodName} - Marginal Distribution for X1");
        histogramX1.AssertIsNormal(10.0, 1.0, 0.1);

        histogramX1.DebugPrint(testOutputHelper, $"{methodName} - Marginal Distribution for X2");
        histogramX2.AssertIsNormal(20.0, 2.0, 0.1);

        histogramX1.DebugPrint(testOutputHelper, $"{methodName} - Marginal Distribution for X3");
        histogramX3.AssertIsNormal(30.0, 3.0, 0.1);
    }

    [Fact]
    public void Sample_Decimal_IsCorrect()
    {
        // Arrange
        // A 2D distribution where X and Y tend to move together.
        var mean = ChiMatrix.With([5.0m, -10.0m]);
        var covariance = ChiMatrix.With(new[,]
        {
            { 4.0m, 3.2m }, // Var(X)=4, Var(Y)=16, Cov(X,Y)=3.2 => Corr=0.8
            { 3.2m, 16.0m }
        });
        var rng = new ChiRng("MvNormal_Decimal_PositiveCorr");

        var samples = new List<decimal[]>(10_000);

        // Act
        for (var i = 0; i < SampleCount; i++)
        {
            using var destination = rng.MultivariateNormal(mean, covariance).Sample();
            samples.Add(destination.VectorToArray());
        }

        // Assert
        samples.AssertIsMultivariateNormal(mean.VectorToArray(), covariance.ToArray(), 0.20);
    }
}