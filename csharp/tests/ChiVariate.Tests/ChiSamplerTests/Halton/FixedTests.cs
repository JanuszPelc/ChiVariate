using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Halton;

/// <summary>
///     Tests for Halton quasi-random sequence using ChiFixed type.
/// </summary>
public class FixedTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 10_000;

    [Fact]
    public void Sample_CanonicalChiFixed_IsBitForBitDeterministic()
    {
        var rng1 = new ChiRng("HaltonFixedSeed");
        var rng2 = new ChiRng("HaltonFixedSeed");

        var sampler1 = rng1.Halton(2, ChiSequenceMode.Canonical).OfType<ChiFixed>();
        var sampler2 = rng2.Halton(2, ChiSequenceMode.Canonical).OfType<ChiFixed>();

        for (var i = 0; i < 1000; i++)
        {
            using var p1 = sampler1.Sample();
            using var p2 = sampler2.Sample();

            p1.Should().BeEquivalentTo(p2, "because canonical sequences must be perfectly deterministic.");
        }
    }

    [Fact]
    public void Sample_RandomizedChiFixed_IsBitForBitDeterministic()
    {
        var rng1 = new ChiRng("RandomHaltonFixed");
        var rng2 = new ChiRng(rng1.Snapshot());

        var sampler1 = rng1.Halton(2).OfType<ChiFixed>();
        var sampler2 = rng2.Halton(2).OfType<ChiFixed>();

        for (var i = 0; i < 1000; i++)
        {
            using var p1 = sampler1.Sample();
            using var p2 = sampler2.Sample();

            p1.Should().BeEquivalentTo(p2,
                "because randomized sequences must also be deterministic given a fixed RNG seed.");
        }
    }

    [Fact]
    public void Sample_ChiFixed2D_ReturnsValuesInUnitInterval()
    {
        var rng = new ChiRng();
        var sampler = rng.Halton(2, ChiSequenceMode.Canonical).OfType<ChiFixed>();

        for (var i = 0; i < 10; i++)
        {
            using var point = sampler.Sample();
            point.Length.Should().Be(2);

            for (var d = 0; d < 2; d++)
                double.CreateChecked(point[d]).Should().BeInRange(0.0, 1.0,
                    $"because Halton values should be in [0, 1] range at dimension {d}.");
        }
    }

    [Fact]
    public void Sample_ChiFixedMarginals_AreUniformlyDistributed()
    {
        const int dimensions = 2;
        const int bins = 50;

        var rng = new ChiRng("HaltonFixedUniformity");
        var sampler = rng.Halton(dimensions).OfType<ChiFixed>();

        var histograms = new Histogram[dimensions];
        for (var i = 0; i < dimensions; i++) histograms[i] = new Histogram(0.0, 1.0, bins);

        foreach (var point in sampler.Sample(SampleCount))
            using (point)
            {
                for (var d = 0; d < dimensions; d++)
                    histograms[d].AddSample(double.CreateChecked(point[d]));
            }

        var expectedSamplesPerBin = (double)SampleCount / bins;
        const double uniformityTolerance = 0.02;

        for (var d = 0; d < dimensions; d++)
        {
            testOutputHelper.WriteLine($"\n--- Halton ChiFixed Dimension {d + 1} Uniformity ---");
            histograms[d].DebugPrint(testOutputHelper);

            foreach (var binCount in histograms[d].Bins)
                ((double)binCount).Should().BeApproximately(expectedSamplesPerBin,
                    expectedSamplesPerBin * uniformityTolerance,
                    $"because Halton sequence should distribute samples uniformly across dimension {d + 1}.");
        }
    }

    [Fact]
    public void Sample_ChiFixedHighDimensions_ReturnsNonZeroValuesInUnitInterval()
    {
        var rng = new ChiRng();
        var sampler = rng.Halton(100, ChiSequenceMode.Canonical).OfType<ChiFixed>();

        using var point = sampler.Sample();
        point.Length.Should().Be(100);

        for (var i = 0; i < 100; i++)
        {
            var value = double.CreateChecked(point[i]);
            value.Should().BeInRange(0.0, 1.0);
            value.Should().NotBe(0.0, "because Halton never produces 0");
        }
    }
}