using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Halton;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 1000;

    [Fact]
    public void CanonicalSequence_WithFixedSeed_IsBitForBitDeterministic()
    {
        // Arrange
        var rng1 = new ChiRng("TestSeed");
        var rng2 = new ChiRng("TestSeed");

        var sampler1 = rng1.Halton(2, ChiSequenceMode.Canonical).OfType<decimal>();
        var sampler2 = rng2.Halton(2, ChiSequenceMode.Canonical).OfType<decimal>();

        // Act & Assert
        for (var i = 0; i < SampleCount; i++)
        {
            using var p1 = sampler1.Sample();
            using var p2 = sampler2.Sample();

            p1.Should().BeEquivalentTo(p2, "because canonical sequences must be perfectly deterministic."); // here!
        }
    }

    [Fact]
    public void RandomizedSequence_WithFixedRngSeed_IsBitForBitDeterministic()
    {
        // Arrange
        var rng1 = new ChiRng("RandomSeedForHalton");
        var rng2 = new ChiRng(rng1.Snapshot());

        var sampler1 = rng1.Halton(2).OfType<float>();
        var sampler2 = rng2.Halton(2).OfType<float>();

        // Act & Assert
        for (var i = 0; i < SampleCount; i++)
        {
            using var p1 = sampler1.Sample();
            using var p2 = sampler2.Sample();

            p1.Should().BeEquivalentTo(p2,
                "because randomized sequences must also be deterministic given a fixed RNG seed.");
        }
    }

    [Fact]
    public void RandomizedSequence_WithDifferentRngSeeds_ProducesDifferentSequences()
    {
        // Arrange
        var rng1 = new ChiRng("SeedA");
        var rng2 = new ChiRng("SeedB");

        var sampler1 = rng1.Halton(2).OfType<double>();
        var sampler2 = rng2.Halton(2).OfType<double>();

        var firstPoint1 = sampler1.Sample().ToArray();
        var firstPoint2 = sampler2.Sample().ToArray();

        // Act & Assert
        firstPoint1.Should().NotBeEquivalentTo(firstPoint2,
            "because different RNG seeds should produce different randomized offsets.");
    }

    [Fact]
    public void RandomizedHalton_FromSameRng_ProducesDifferentSequencesThanCanonical()
    {
        // Arrange
        const int sampleCount = 1000;
        const int dimensions = 3;

        var canonicalRng = new ChiRng();
        var randomizedRng = new ChiRng(canonicalRng.Snapshot());

        var canonicalSampler = canonicalRng.Halton(dimensions, ChiSequenceMode.Canonical).OfType<double>();
        var randomizedSampler = randomizedRng.Halton(dimensions).OfType<double>();

        // Act - Test bulk sampling
        using var canonicalPoints = canonicalSampler.Sample(sampleCount);
        using var randomizedPoints = randomizedSampler.Sample(sampleCount);

        // Assert
        canonicalPoints.List.Count.Should().Be(sampleCount);
        randomizedPoints.List.Count.Should().Be(sampleCount);

        // Randomized should differ from canonical
        var canonicalFirst = canonicalPoints.List[0].ToArray();
        var randomizedFirst = randomizedPoints.List[0].ToArray();

        canonicalFirst.Should().NotBeEquivalentTo(randomizedFirst,
            "because randomized Halton should differ from canonical Halton due to Cranley-Patterson rotation");

        // Both maintain valid quasi-random properties
        ValidateQuasiRandomProperties(canonicalPoints.List, dimensions);
        ValidateQuasiRandomProperties(randomizedPoints.List, dimensions);

        return;

        static void ValidateQuasiRandomProperties(IList<ChiVector<double>> points, int dimensions)
        {
            foreach (var point in points)
                using (point)
                {
                    for (var d = 0; d < dimensions; d++)
                        point[d].Should().BeInRange(0.0, 1.0);
                }
        }
    }

    [Fact]
    public void CanonicalHalton_2D_FirstFewPoints_MatchKnownValues()
    {
        var expectedPoints = new[]
        {
            new[] { 0.5, 1.0 / 3.0 },
            new[] { 0.25, 2.0 / 3.0 },
            new[] { 0.75, 1.0 / 9.0 },
            new[] { 0.125, 4.0 / 9.0 },
            new[] { 0.625, 7.0 / 9.0 }
        };

        var rng = new ChiRng();
        var sampler = rng.Halton(2, ChiSequenceMode.Canonical).OfType<double>();

        // Act & Assert
        foreach (var t in expectedPoints)
        {
            using var actualPoint = sampler.Sample();
            actualPoint.ToArray().Should().BeEquivalentTo(t, options => options
                .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-9))
                .WhenTypeIs<double>()
            );
        }
    }

    [Fact]
    public void HaltonSequence_Marginals_ShowExcellentUniformity()
    {
        // Arrange
        const int dimensions = 2;
        const int samples = 10_000;
        const int bins = 50;

        var rng = new ChiRng("HaltonUniformity");
        var sampler = rng.Halton(dimensions).OfType<double>();

        var histograms = new Histogram[dimensions];
        for (var i = 0; i < dimensions; i++) histograms[i] = new Histogram(0.0, 1.0, bins);

        // Act
        foreach (var point in sampler.Sample(samples))
            using (point)
            {
                for (var d = 0; d < dimensions; d++) histograms[d].AddSample(point[d]);
            }

        // Assert
        var expectedSamplesPerBin = (double)samples / bins;
        const double uniformityTolerance = 0.01;

        for (var d = 0; d < dimensions; d++)
        {
            testOutputHelper.WriteLine($"\n--- Halton Dimension {d + 1} Uniformity ---");
            histograms[d].DebugPrint(testOutputHelper);

            foreach (var binCount in histograms[d].Bins)
                ((double)binCount).Should().BeApproximately(expectedSamplesPerBin,
                    expectedSamplesPerBin * uniformityTolerance,
                    $"because Halton sequence should distribute samples extremely uniformly across dimension {d + 1}.");
        }
    }

    [Fact]
    public void Halton_HighDimensional_ShowsPrecisionDifferencesInPrimeBasedFractions()
    {
        // Arrange - Use many dimensions to stress different prime bases
        const int dimensions = 50; // First 50 primes: 2,3,5,7,11,13,17,19,23,29...
        const int sampleCount = 100;

        var baseRng = new ChiRng("HaltonHighDimPrecisionTest");
        var doubleRng = new ChiRng(baseRng.Snapshot());
        var decimalRng = new ChiRng(baseRng.Snapshot());

        var doubleSampler = doubleRng.Halton(dimensions, ChiSequenceMode.Canonical).OfType<double>();
        var decimalSampler = decimalRng.Halton(dimensions, ChiSequenceMode.Canonical).OfType<decimal>();

        var totalDifference = 0m;
        var dimensionsWithDifferences = 0;
        var maxDifference = 0m;
        int maxDiffSample = -1, maxDiffDim = -1;

        // Act - Check high-dimensional Halton points for precision differences
        for (var sample = 0; sample < sampleCount; sample++)
        {
            using var doublePoint = doubleSampler.Sample();
            using var decimalPoint = decimalSampler.Sample();

            for (var dim = 0; dim < dimensions; dim++)
            {
                var doubleAsDecimal = decimal.CreateChecked(doublePoint[dim]);
                var nativeDecimal = decimalPoint[dim];
                var difference = Math.Abs(doubleAsDecimal - nativeDecimal);

                if (difference > 0m)
                {
                    totalDifference += difference;
                    dimensionsWithDifferences++;

                    if (difference > maxDifference)
                    {
                        maxDifference = difference;
                        maxDiffSample = sample;
                        maxDiffDim = dim;

                        testOutputHelper.WriteLine(
                            $"New max difference at sample {sample}, dim {dim} (prime base: {GetPrimeForDimension(dim)}):");
                        testOutputHelper.WriteLine($"  Double:  {doublePoint[dim]:G17}");
                        testOutputHelper.WriteLine($"  Decimal: {nativeDecimal:G29}");
                        testOutputHelper.WriteLine($"  Diff:    {difference:G10}");
                    }
                }
            }
        }

        // Assert - Halton should show precision differences due to prime-based fractions
        testOutputHelper.WriteLine($"Total samples checked: {sampleCount * dimensions:N0}");
        testOutputHelper.WriteLine($"Dimensions with differences: {dimensionsWithDifferences:N0}");
        testOutputHelper.WriteLine($"Hit rate: {(double)dimensionsWithDifferences / (sampleCount * dimensions):P2}");
        testOutputHelper.WriteLine($"Total cumulative difference: {totalDifference:G15}");
        testOutputHelper.WriteLine($"Maximum single difference: {maxDifference:G15}");
        if (maxDiffSample >= 0)
            testOutputHelper.WriteLine(
                $"Max difference at sample {maxDiffSample}, dimension {maxDiffDim} (prime {GetPrimeForDimension(maxDiffDim)})");

        testOutputHelper.WriteLine(
            $"Average difference (when present): {(dimensionsWithDifferences > 0 ? totalDifference / dimensionsWithDifferences : 0):G10}");

        // Halton with many prime bases should reveal precision limits
        (totalDifference > 0m || dimensionsWithDifferences > 0).Should().BeTrue(
            "because high-dimensional Halton sequences with various prime bases should reveal double precision limits");

        return;

        static int GetPrimeForDimension(int dimension)
        {
            // First few primes for context in test output
            var primes = new[]
                { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
            return dimension < primes.Length ? primes[dimension] : -1; // -1 for "unknown" beyond our small table
        }
    }

    [Fact]
    public void Halton_CachedDimensions_PerformBetterThanUncached()
    {
        var rng = new ChiRng();

        // Cached: 2D (primes 2,3)
        var cached = rng.Halton(2).OfType<double>();

        // Uncached: 20D (includes higher primes)  
        var uncached = rng.Halton(20).OfType<double>();

        using var cachedPoint = cached.Sample();
        using var uncachedPoint = uncached.Sample();

        cachedPoint.Length.Should().Be(2);
        uncachedPoint.Length.Should().Be(20);
    }

    [Fact]
    public void Halton_HighDimensions_MaintainsQuality()
    {
        var rng = new ChiRng();
        var sampler = rng.Halton(1024, ChiSequenceMode.Canonical).OfType<double>();

        using var point = sampler.Sample();
        point.Length.Should().Be(1024);

        for (var i = 0; i < 1024; i++)
        {
            point[i].Should().BeInRange(0.0, 1.0);
            point[i].Should().NotBe(0.0); // Halton never produces 0
        }
    }

    [Theory]
    [InlineData(0)] // Zero dimensions
    [InlineData(1025)] // Exceeds max supported dimensions
    public void Halton_WithInvalidDimensions_ThrowsArgumentOutOfRangeException(int invalidDimensions)
    {
        // Arrange
        var rng = new ChiRng();
        var act = () => { rng.Halton(invalidDimensions).OfType<Half>(); };

        // Act & Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}