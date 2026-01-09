using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Dirichlet;

/// <summary>
///     Tests for Dirichlet distribution using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 20_000;

    [Fact]
    public void Sample_Symmetric_IsCorrect()
    {
        var alpha = new[] { (ChiFixed)1m, (ChiFixed)1m, (ChiFixed)1m };
        var rng = new ChiRng("Dirichlet_Fixed_Symmetric");
        var samples = new List<double[]>(SampleCount);

        for (var i = 0; i < SampleCount; i++)
        {
            using var probabilities = rng.Dirichlet(alpha).Sample();
            samples.Add(probabilities.ToArray().Select(x => double.CreateChecked(x)).ToArray());
        }

        samples.AssertIsDirichlet([1.0, 1.0, 1.0], 0.15);
    }

    [Fact]
    public void Sample_Asymmetric_IsCorrect()
    {
        var alpha = new[] { (ChiFixed)5m, (ChiFixed)10m, (ChiFixed)15m };
        var rng = new ChiRng("Dirichlet_Fixed_Asymmetric");
        var samples = new List<double[]>(SampleCount);

        for (var i = 0; i < SampleCount; i++)
        {
            using var probabilities = rng.Dirichlet(alpha).Sample();
            samples.Add(probabilities.ToArray().Select(x => double.CreateChecked(x)).ToArray());
        }

        samples.AssertIsDirichlet([5.0, 10.0, 15.0], 0.15);
    }

    [Fact]
    public void Sample_OutputVector_SumsToOne()
    {
        var alpha = new[] { (ChiFixed)2m, (ChiFixed)2m, (ChiFixed)2m, (ChiFixed)2m };
        var rng = new ChiRng("Dirichlet_Fixed_SumCheck");

        for (var i = 0; i < 1000; i++)
        {
            using var probabilities = rng.Dirichlet(alpha).Sample();
            var arr = probabilities.ToArray();
            var sum = ChiFixed.Zero;
            foreach (var p in arr)
                sum += p;
            double.CreateChecked(sum).Should()
                .BeApproximately(1.0, 1e-6, "because the components of a Dirichlet vector must sum to 1.");
        }
    }

    [Fact]
    public void Sample_MarginalDistribution_IsBetaDistributed()
    {
        var alpha = new[] { (ChiFixed)2m, (ChiFixed)3m, (ChiFixed)5m };
        var rng = new ChiRng("Dirichlet_Fixed_BetaCheck");

        const double expectedBetaAlpha = 2.0;
        const double expectedBetaBeta = 3.0 + 5.0;
        const double expectedMean = expectedBetaAlpha / (expectedBetaAlpha + expectedBetaBeta);

        var histogram = new Histogram(0, 1, 100);

        for (var i = 0; i < SampleCount; i++)
        {
            using var probabilities = rng.Dirichlet(alpha).Sample();
            histogram.AddSample(double.CreateChecked(probabilities[0]));
        }

        histogram.DebugPrint(testOutputHelper, "Dirichlet marginal (ChiFixed)");
        histogram.AssertIsBeta(expectedMean, 0.15);
    }

    [Fact]
    public void Sample_HighConcentration_ProducesConcentratedOutput()
    {
        // High alpha values should produce samples close to the mean
        var alpha = new[] { (ChiFixed)100m, (ChiFixed)100m, (ChiFixed)100m };
        var rng = new ChiRng("Dirichlet_Fixed_HighConcentration");

        for (var i = 0; i < 100; i++)
        {
            using var probabilities = rng.Dirichlet(alpha).Sample();
            for (var j = 0; j < probabilities.Length; j++)
            {
                var value = double.CreateChecked(probabilities[j]);
                // With symmetric high alpha, all components should be close to 1/3
                value.Should().BeApproximately(1.0 / 3.0, 0.1,
                    $"because high concentration Dirichlet should produce values near the mean at component {j}.");
            }
        }
    }
}