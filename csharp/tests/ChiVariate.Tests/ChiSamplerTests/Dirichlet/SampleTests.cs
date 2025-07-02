using ChiVariate.Tests.TestInfrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Dirichlet;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 20_000;

    [Fact]
    public void Sample_Symmetric_IsCorrect()
    {
        // Arrange
        // Symmetric case, all components should have the same mean.
        var alpha = new[] { 1.0, 1.0, 1.0 }; // Expected means: [1/3, 1/3, 1/3]
        var rng = new ChiRng("Dirichlet_Symmetric");
        var samples = new List<double[]>(SampleCount);

        // Act
        for (var i = 0; i < SampleCount; i++)
            samples.Add(rng.Dirichlet(alpha).Sample().ToArray());

        // Assert
        samples.AssertIsDirichlet(alpha, 0.1);
    }

    [Fact]
    public void Sample_Asymmetric_IsCorrect()
    {
        // Arrange
        // Asymmetric case, means should be proportional to alpha values.
        var alpha = new[] { 5.0, 10.0, 15.0 }; // Total=30, Expected means: [5/30, 10/30, 15/30]
        var rng = new ChiRng("Dirichlet_Asymmetric");
        var samples = new List<double[]>(SampleCount);

        samples.AddRange(rng
            .Dirichlet(alpha)
            .Sample(SampleCount)
            .Select(probabilities => probabilities.ToArray()));

        // Act

        // Assert
        samples.AssertIsDirichlet(alpha, 0.1);
    }

    [Fact]
    public void Sample_OutputVector_SumsToOne()
    {
        // Arrange
        var alpha = new[] { 2.0, 2.0, 2.0, 2.0 };
        var rng = new ChiRng("Dirichlet_SumCheck");

        // Act & Assert
        for (var i = 0; i < 1000; i++)
        {
            var probabilities = rng.Dirichlet(alpha).Sample();
            probabilities.ToArray().Sum().Should()
                .BeApproximately(1.0, 1e-9, "because the components of a Dirichlet vector must sum to 1.");
        }
    }

    [Fact]
    public void Sample_MarginalDistribution_IsBetaDistributed()
    {
        // Arrange
        var alpha = new[] { 2.0, 3.0, 5.0 }; // Asymmetric case
        var rng = new ChiRng("Dirichlet_BetaCheck");

        var expectedBetaAlpha = alpha[0];
        var expectedBetaBeta = alpha[1] + alpha[2];
        var expectedMean = expectedBetaAlpha / (expectedBetaAlpha + expectedBetaBeta);

        var histogram = new Histogram(0, 1, 100);

        // Act
        foreach (var probabilities in rng.Dirichlet(alpha).Sample(SampleCount))
            histogram.AddSample(probabilities[0]);

        // Assert
        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsBeta(expectedMean, 0.1);
    }

    [Fact]
    public void Sample_Decimal_IsCorrect()
    {
        // Arrange
        var alpha = new[] { 5.0m, 10.0m, 15.0m }; // Asymmetric case
        var rng = new ChiRng("Dirichlet_Decimal_Asymmetric");
        var samples = new List<decimal[]>(20_000);

        // Act
        for (var i = 0; i < 20_000; i++)
        {
            using var probabilities = rng.Dirichlet(alpha).Sample();
            samples.Add(probabilities.ToArray());
        }

        // Assert
        samples.AssertIsDirichlet(alpha, 0.15);
    }
}