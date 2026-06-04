using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Sobol;

/// <summary>
///     Tests for Sobol quasi-random sequence using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 10_000;

    [Fact]
    public void Sample_CanonicalSequence_IsBitForBitDeterministic()
    {
        var rng1 = new ChiRng("SobolFixedSeed");
        var rng2 = new ChiRng(rng1.Snapshot());

        var sampler1 = rng1.Sobol(2, ChiSequenceMode.Canonical).OfType<ChiFixed>();
        var sampler2 = rng2.Sobol(2, ChiSequenceMode.Canonical).OfType<ChiFixed>();

        for (var i = 0; i < 1000; i++)
        {
            using var p1 = sampler1.Sample();
            using var p2 = sampler2.Sample();

            p1.Should().BeEquivalentTo(p2, "because canonical sequences must be perfectly deterministic.");
        }
    }

    [Fact]
    public void Sample_RandomizedSequence_IsBitForBitDeterministic()
    {
        var rng1 = new ChiRng("RandomSobolFixed");
        var rng2 = new ChiRng("RandomSobolFixed");

        var sampler1 = rng1.Sobol(2).OfType<ChiFixed>();
        var sampler2 = rng2.Sobol(2).OfType<ChiFixed>();

        for (var i = 0; i < 1000; i++)
        {
            using var p1 = sampler1.Sample();
            using var p2 = sampler2.Sample();

            p1.Should().BeEquivalentTo(p2,
                "because randomized sequences must also be deterministic given a fixed RNG seed.");
        }
    }

    [Fact]
    public void Sample_Canonical1D_MatchesKnownValues()
    {
        var expectedValues = new[]
        {
            0.0,
            0.5,
            0.75,
            0.25,
            0.375,
            0.875
        };

        var rng = new ChiRng();
        var sampler = rng.Sobol(1, ChiSequenceMode.Canonical).OfType<ChiFixed>();

        for (var i = 0; i < expectedValues.Length; i++)
        {
            using var actualPoint = sampler.Sample();
            double.CreateChecked(actualPoint[0]).Should().BeApproximately(expectedValues[i], 1e-9,
                $"because Sobol 1D point {i + 1} should match the known canonical value.");
        }
    }

    [Fact]
    public void Sample_Canonical2D_MatchesKnownValues()
    {
        var expectedPoints = new[]
        {
            new[] { 0.0, 0.0 },
            new[] { 0.5, 0.5 },
            new[] { 0.75, 0.25 },
            new[] { 0.25, 0.75 },
            new[] { 0.375, 0.375 },
            new[] { 0.875, 0.875 },
            new[] { 0.625, 0.125 },
            new[] { 0.125, 0.625 }
        };

        var rng = new ChiRng();
        var sampler = rng.Sobol(2, ChiSequenceMode.Canonical).OfType<ChiFixed>();

        foreach (var expected in expectedPoints)
        {
            using var actualPoint = sampler.Sample();
            for (var d = 0; d < 2; d++)
                double.CreateChecked(actualPoint[d]).Should().BeApproximately(expected[d], 1e-9,
                    $"because Sobol 2D point should match known values at dimension {d}.");
        }
    }

    [Fact]
    public void Sample_Marginals_AreUniform()
    {
        const int dimensions = 2;
        const int bins = 10;

        var rng = new ChiRng("SobolFixedUniformity");
        var sampler = rng.Sobol(dimensions, ChiSequenceMode.Canonical).OfType<ChiFixed>();

        var histograms = new Histogram[dimensions];
        for (var i = 0; i < dimensions; i++)
            histograms[i] = new Histogram(0.0, 1.0, bins);

        foreach (var point in sampler.Sample(SampleCount))
            using (point)
            {
                for (var d = 0; d < dimensions; d++)
                    histograms[d].AddSample(double.CreateChecked(point[d]));
            }

        const double expectedSamplesPerBin = (double)SampleCount / bins;
        const double uniformityTolerance = 0.01;

        for (var d = 0; d < dimensions; d++)
        {
            testOutputHelper.WriteLine($"\n--- Sobol ChiFixed Dimension {d + 1} Uniformity ---");
            histograms[d].DebugPrint(testOutputHelper);

            foreach (var binCount in histograms[d].Bins)
                ((double)binCount).Should().BeApproximately(expectedSamplesPerBin,
                    expectedSamplesPerBin * uniformityTolerance,
                    $"because Sobol sequence should distribute samples extremely uniformly across dimension {d + 1}.");
        }
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    public void Sample_HighDimensions_MaintainsValidRange(int dimensions)
    {
        var rng = new ChiRng();
        var sampler = rng.Sobol(dimensions).OfType<ChiFixed>();

        const int testPoints = 100;
        foreach (var point in sampler.Sample(testPoints))
            for (var d = 0; d < dimensions; d++)
            {
                var value = double.CreateChecked(point[d]);
                value.Should().BeInRange(0.0, 1.0,
                    $"All values should be in [0,1] even in dimension {d + 1} of {dimensions}");
            }
    }
}