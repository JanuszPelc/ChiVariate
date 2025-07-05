using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Spatial;

public class SpatialSquareTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void PointInSquare_SamplesAreWithinBounds()
    {
        // Arrange
        const float extents = 10.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointInSquareBounds"));
        var sampler = rng.Spatial().InSquare(extents);

        // Act & Assert
        for (var i = 0; i < 1000; i++)
        {
            var p = sampler.Sample();
            p.X.Should().BeInRange(-extents, extents);
            p.Y.Should().BeInRange(-extents, extents);
        }
    }

    [Fact]
    public void PointInSquare_DistributionIsSpatiallyUniform()
    {
        const float extents = 1.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointInSquareUniformity"));
        var sampler = rng.Spatial().InSquare(extents);

        var histX = new Histogram(-extents, extents, 20);
        var histY = new Histogram(-extents, extents, 20);

        // Act
        foreach (var p in sampler.Sample(SampleCount))
        {
            histX.AddSample(p.X);
            histY.AddSample(p.Y);
        }

        // Assert
        histX.DebugPrint(testOutputHelper, "PointInSquare Marginal" + " X");
        histY.DebugPrint(testOutputHelper, "PointInSquare Marginal" + " Y");

        histX.AssertIsUniform(0.15);
        histY.AssertIsUniform(0.15);
    }

    [Fact]
    public void PointOnSquare_SamplesAreOnThePerimeter()
    {
        // Arrange
        const float extents = 5.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointOnSquareBounds"));
        var sampler = rng.Spatial().OnSquare(extents);

        // Act & Assert
        for (var i = 0; i < 1000; i++)
        {
            var p = sampler.Sample();
            var onEdge = Math.Abs(p.X - extents) < 1e-6f || Math.Abs(p.X + extents) < 1e-6f ||
                         Math.Abs(p.Y - extents) < 1e-6f || Math.Abs(p.Y + extents) < 1e-6f;

            onEdge.Should().BeTrue("because every point must lie exactly on one of the four edges.");
            p.X.Should().BeInRange(-extents, extents);
            p.Y.Should().BeInRange(-extents, extents);
        }
    }

    [Fact]
    public void PointOnSquare_DistributionIsSpatiallyUniform()
    {
        const float extents = 1.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointOnSquareUniformity"));
        var sampler = rng.Spatial().OnSquare(extents);

        int topEdgeCount = 0, bottomEdgeCount = 0, leftEdgeCount = 0, rightEdgeCount = 0;

        // Act
        foreach (var p in sampler.Sample(SampleCount))
            if (Math.Abs(p.Y - extents) < 1e-6f) topEdgeCount++;
            else if (Math.Abs(p.Y + extents) < 1e-6f) bottomEdgeCount++;
            else if (Math.Abs(p.X - extents) < 1e-6f) rightEdgeCount++;
            else leftEdgeCount++;

        // Assert
        const double expectedCount = SampleCount / 4.0;
        ((double)topEdgeCount).Should()
            .BeApproximately(expectedCount, expectedCount * 0.05, "top edge should have ~25% of points.");
        ((double)bottomEdgeCount).Should().BeApproximately(expectedCount, expectedCount * 0.05,
            "bottom edge should have ~25% of points.");
        ((double)leftEdgeCount).Should()
            .BeApproximately(expectedCount, expectedCount * 0.05, "left edge should have ~25% of points.");
        ((double)rightEdgeCount).Should()
            .BeApproximately(expectedCount, expectedCount * 0.05, "right edge should have ~25% of points.");
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        // Arrange
        var rng = new ChiRng(1337);

        // Act
        var onP = rng.Spatial().OnSquare(10.0f).Sample();
        var inP = rng.Spatial().InSquare(10.0f).Sample();

        // Assert
        onP.X.Should().BeApproximately(10f, 0.0001f);
        onP.Y.Should().BeApproximately(-6.9566f, 0.0001f);

        inP.X.Should().BeApproximately(-3.1642f, 0.0001f);
        inP.Y.Should().BeApproximately(-6.8629f, 0.0001f);
    }

    [Fact]
    public void PointSquare_WithInvalidExtents_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var rng = new ChiRng();
        var act = () => { rng.Spatial().InSquare(-1.0f); };

        // Act & Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("extents");
    }
}