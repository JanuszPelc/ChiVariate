using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Wishart;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 10_000;

    [Fact]
    public void Sample_Dim2_MatchesWishartDistribution()
    {
        const int degreesOfFreedom = 5;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { 1.0, 0.5 },
            { 0.5, 2.0 }
        });
        var rng = new ChiRng("Wishart_2D");
        var wishart = rng.Wishart(degreesOfFreedom, scaleMatrix);
        var samples = new List<double[,]>(SampleCount);

        for (var i = 0; i < SampleCount; i++)
            samples.Add(wishart.Sample().ToArray());

        samples.AssertIsWishart(degreesOfFreedom, scaleMatrix.ToArray(), 0.15);
    }

    [Fact]
    public void Sample_Dim3IdentityScale_MatchesWishartDistribution()
    {
        const int degreesOfFreedom = 10;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { 1.0, 0.0, 0.0 },
            { 0.0, 1.0, 0.0 },
            { 0.0, 0.0, 1.0 }
        });
        var rng = new ChiRng("Wishart_3D_Identity");
        var wishart = rng.Wishart(degreesOfFreedom, scaleMatrix);

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

        samples.AssertIsWishart(degreesOfFreedom, scaleMatrix.ToArray(), 0.15);
    }

    [Fact]
    public void Sample_DiagonalElements_FollowChiSquaredDistribution()
    {
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

        for (var i = 0; i < SampleCount; i++)
        {
            var sampleMatrix = rng.Wishart(degreesOfFreedom, scaleMatrix).Sample();
            histogram.AddSample(sampleMatrix[0, 0]);
        }

        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsChiSquared(degreesOfFreedom, 0.1, 0.15);
    }

    [Fact]
    public void Sample_Decimal_MatchesWishartDistribution()
    {
        const int degreesOfFreedom = 5;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { 1.0m, 0.5m },
            { 0.5m, 2.0m }
        });
        var rng = new ChiRng("Wishart_Decimal_2D");

        var samples = new List<decimal[,]>(SampleCount);

        for (var i = 0; i < 1_000; i++)
            samples.Add(rng.Wishart(degreesOfFreedom, scaleMatrix).Sample().ToArray());

        samples.AssertIsWishart(degreesOfFreedom, scaleMatrix.ToArray(), 0.25);
    }

    [Theory]
    [InlineData("Deterministic")]
    [InlineData("Randomized")]
    public void Snapshot_WithRestoredState_ProducesIdenticalSamples(string seed)
    {
        const int degreesOfFreedom = 5;
        var scaleMatrix = ChiMatrix.With(new[,]
        {
            { 1.0, 0.5 },
            { 0.5, 2.0 }
        });
        var rng = seed == "Randomized" ? new ChiRng() : new ChiRng(seed);
        var warmupCount = rng.Chance().PickBetween(100, 1000);
        for (var i = 0; i < warmupCount; i++)
        {
            using var sample = rng.Wishart(degreesOfFreedom, scaleMatrix).Sample();
        }

        var rngSnapshot = rng.Snapshot();

        var rngClone = new ChiRng(rngSnapshot);

        for (var i = 0; i < 10_000; i++)
        {
            using var sample1 = rng.Wishart(degreesOfFreedom, scaleMatrix).Sample();
            using var sample2 = rngClone.Wishart(degreesOfFreedom, scaleMatrix).Sample();
            sample1.ToArray().Should().BeEquivalentTo(sample2.ToArray());
        }
    }
}