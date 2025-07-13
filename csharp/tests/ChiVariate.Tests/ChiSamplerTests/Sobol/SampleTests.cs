using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Sobol;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void CanonicalSequence_WithFixedSeed_IsBitForBitDeterministic()
    {
        // Arrange
        var rng1 = new ChiRng("TestSeed");
        var rng2 = new ChiRng(rng1.Snapshot());

        var sampler1 = rng1.Sobol(2, ChiSequenceMode.Canonical).OfType<decimal>();
        var sampler2 = rng2.Sobol(2, ChiSequenceMode.Canonical).OfType<decimal>();

        // Act & Assert
        for (var i = 0; i < SampleCount; i++)
        {
            using var p1 = sampler1.Sample();
            using var p2 = sampler2.Sample();

            p1.Should().BeEquivalentTo(p2, "because canonical sequences must be perfectly deterministic.");
        }
    }

    [Fact]
    public void RandomizedSequence_WithFixedRngSeed_IsBitForBitDeterministic()
    {
        // Arrange
        var rng1 = new ChiRng("RandomSeedForSobol");
        var rng2 = new ChiRng("RandomSeedForSobol");

        var sampler1 = rng1.Sobol(2).OfType<float>();
        var sampler2 = rng2.Sobol(2).OfType<float>();

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
        var rng1 = new ChiRng("Seed");
        var rng2 = new ChiRng("Different Seed");

        var sampler1 = rng1.Sobol(2).OfType<double>();
        var sampler2 = rng2.Sobol(2).OfType<double>();

        var firstPoint1 = sampler1.Sample().ToArray();
        var firstPoint2 = sampler2.Sample().ToArray();

        // Act & Assert
        firstPoint1.Should().NotBeEquivalentTo(firstPoint2,
            "because different RNG seeds should produce different randomized sequences.");
    }

    [Fact]
    public void CanonicalSobol_1D_FirstFewPoints_MatchKnownValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            0.5, // 0.1_2
            0.75, // 0.11_2
            0.25, // 0.01_2
            0.375, // 0.011_2
            0.875 // 0.111_2
        };

        var rng = new ChiRng();
        var sampler = rng.Sobol(1, ChiSequenceMode.Canonical).OfType<double>();

        // Act & Assert
        for (var i = 0; i < expectedValues.Length; i++)
        {
            using var actualPoint = sampler.Sample();
            actualPoint[0].Should().BeApproximately(expectedValues[i], 1e-9,
                $"because Sobol 1D point {i + 1} should match the known canonical value.");
        }
    }

    [Fact]
    public void CanonicalSobol_2D_FirstFewPoints_MatchKnownValues()
    {
        // Arrange
        var expectedPoints = new[]
        {
            new[] { 0.5, 0.5 },
            new[] { 0.75, 0.25 },
            new[] { 0.25, 0.75 },
            new[] { 0.375, 0.375 },
            new[] { 0.875, 0.875 },
            new[] { 0.625, 0.125 },
            new[] { 0.125, 0.625 }
        };

        var rng = new ChiRng();
        var sampler = rng.Sobol(2, ChiSequenceMode.Canonical).OfType<double>();

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

    [Theory]
    [InlineData(ChiSequenceMode.Canonical, 0.001)]
    [InlineData(ChiSequenceMode.Randomized, 0.1)]
    public void SobolSequence_Marginals_ShowCorrectUniformity(ChiSequenceMode mode, double tolerance)
    {
        // Arrange
        const int dimensions = 1024;
        const int samples = 25_000;
        const int bins = 10;

        var rng = new ChiRng("SobolUniformity");
        var sampler = rng.Sobol(dimensions, mode).OfType<double>();

        var histograms = new Histogram[dimensions];
        for (var i = 0; i < dimensions; i++)
            histograms[i] = new Histogram(0.0, 1.0, bins);

        // Act
        foreach (var point in sampler.Sample(samples))
            using (point)
            {
                for (var d = 0; d < dimensions; d++)
                    histograms[d].AddSample(point[d]);
            }

        // Assert
        const double expectedSamplesPerBin = (double)samples / bins;
        var uniformityTolerance = expectedSamplesPerBin * tolerance;

        for (var d = 0; d < dimensions; d++)
        {
            testOutputHelper.WriteLine($"\n--- Sobol Dimension {d + 1} Uniformity ({mode}) ---");
            histograms[d].DebugPrint(testOutputHelper);

            foreach (var binCount in histograms[d].Bins)
                ((double)binCount).Should().BeApproximately(expectedSamplesPerBin,
                    uniformityTolerance,
                    $"because Sobol sequence should distribute samples extremely uniformly across dimension {d + 1} in {mode} mode.");
        }
    }

    [Fact]
    public void RandomizedSobol_FromSameRng_ProducesDifferentSequencesThanCanonical()
    {
        // Arrange
        const int sampleCount = 1000;
        const int dimensions = 4;

        var canonicalRng = new ChiRng("SobolComparisonSeed");
        var randomizedRng = new ChiRng("SobolComparisonSeed"); // Same seed for fair comparison

        var canonicalSampler = canonicalRng.Sobol(dimensions, ChiSequenceMode.Canonical).OfType<double>();
        var randomizedSampler = randomizedRng.Sobol(dimensions).OfType<double>();

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
            "because randomized Sobol should differ from canonical Sobol due to digital scrambling");

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
    public void Sobol_HighDimensional_ShowsPrecisionDifferencesInComplexFractions()
    {
        // Arrange - Use many dimensions to create complex fraction combinations
        const int dimensions = 50; // High dimensional stress test
        const int sampleCount = 100; // Fewer samples, but more complex ones

        var baseRng = new ChiRng("HighDimPrecisionTest");
        var doubleRng = new ChiRng(baseRng.Snapshot());
        var decimalRng = new ChiRng(baseRng.Snapshot());

        var doubleSampler = doubleRng.Sobol(dimensions, ChiSequenceMode.Canonical).OfType<double>();
        var decimalSampler = decimalRng.Sobol(dimensions, ChiSequenceMode.Canonical).OfType<decimal>();

        var totalDifference = 0m;
        var dimensionsWithDifferences = 0;
        var maxDifference = 0m;

        // Act - Check high-dimensional points for precision differences
        for (var sample = 0; sample < sampleCount; sample++)
        {
            using var doublePoint = doubleSampler.Sample();
            using var decimalPoint = decimalSampler.Sample();

            for (var dim = 0; dim < dimensions; dim++)
            {
                var doubleAsDecimal = decimal.CreateChecked(doublePoint[dim]);
                var nativeDecimal = decimalPoint[dim];
                var difference = Math.Abs(doubleAsDecimal - nativeDecimal);

                if (difference <= 0m) continue;

                totalDifference += difference;
                dimensionsWithDifferences++;

                if (difference <= maxDifference) continue;

                maxDifference = difference;
                testOutputHelper.WriteLine($"New max difference at sample {sample}, dim {dim}:");
                testOutputHelper.WriteLine($"  Double:  {doublePoint[dim]:G17}");
                testOutputHelper.WriteLine($"  Decimal: {nativeDecimal:G29}");
                testOutputHelper.WriteLine($"  Diff:    {difference:G10}");
            }
        }

        // Assert - High dimensions should reveal precision differences
        testOutputHelper.WriteLine($"Total samples checked: {sampleCount * dimensions:N0}");
        testOutputHelper.WriteLine($"Dimensions with differences: {dimensionsWithDifferences:N0}");
        testOutputHelper.WriteLine($"Total cumulative difference: {totalDifference:G15}");
        testOutputHelper.WriteLine($"Maximum single difference: {maxDifference:G15}");
        testOutputHelper.WriteLine(
            $"Average difference (when present): {(dimensionsWithDifferences > 0 ? totalDifference / dimensionsWithDifferences : 0):G10}");

        // We should find SOME precision differences in high-dimensional space
        (totalDifference > 0m || dimensionsWithDifferences > 0).Should().BeTrue(
            "because high-dimensional Sobol sequences should eventually reveal precision limits of double vs decimal");
    }

    [Theory]
    [InlineData(0)] // Zero dimensions
    [InlineData(int.MaxValue)] // Exceeds max supported dimensions for this implementation
    public void Sobol_WithInvalidDimensions_ThrowsArgumentOutOfRangeException(int invalidDimensions)
    {
        // Arrange
        var rng = new ChiRng();
        var act = () => { rng.Sobol(invalidDimensions).OfType<Half>(); };

        // Act & Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Sobol_FirstPoints_MatchReferenceImplementation()
    {
        // Arrange
        var expected2D = new[,]
        {
            { 0.5, 0.5 },
            { 0.75, 0.25 },
            { 0.25, 0.75 },
            { 0.375, 0.375 },
            { 0.875, 0.875 },
            { 0.625, 0.125 },
            { 0.125, 0.625 },
            { 0.1875, 0.3125 }
        };

        var rng = new ChiRng();
        var sampler = rng.Sobol(2, ChiSequenceMode.Canonical).OfType<double>();

        // Act & Assert
        for (var i = 0; i < 8; i++)
        {
            using var point = sampler.Sample();
            point[0].Should().BeApproximately(expected2D[i, 0], 1e-9);
            point[1].Should().BeApproximately(expected2D[i, 1], 1e-9);
        }
    }

    [Fact]
    public void Sobol_DifferentScrambles_ProduceDifferentButValidSequences()
    {
        // Arrange
        const int numSequences = 10;
        const int dimensions = 3;

        var firstPoints = new List<double[]>();
        for (var i = 0; i < numSequences; i++)
        {
            var rng = new ChiRng(i); // Different seed
            var sampler = rng.Sobol(dimensions).OfType<double>();
            using var firstPoint = sampler.Sample();
            firstPoints.Add(firstPoint.ToArray());
        }

        // Act & Assert
        for (var i = 0; i < numSequences - 1; i++)
        for (var j = i + 1; j < numSequences; j++)
            firstPoints[i].Should().NotBeEquivalentTo(firstPoints[j],
                "Different scrambles should produce different sequences");
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(500)]
    public void Sobol_HighDimensions_MaintainsValidRange(int dimensions)
    {
        // Arrange
        var rng = new ChiRng();
        var sampler = rng.Sobol(dimensions).OfType<double>();

        // Act & Assert
        const int testPoints = 1000;
        foreach (var point in sampler.Sample(testPoints))
            for (var d = 0; d < dimensions; d++)
                point[d].Should().BeInRange(0.0, 1.0,
                    $"All values should be in [0,1] even in dimension {d + 1} of {dimensions}");
    }
}