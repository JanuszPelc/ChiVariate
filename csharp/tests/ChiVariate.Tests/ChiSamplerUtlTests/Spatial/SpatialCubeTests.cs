using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Spatial;

public class SpatialCubeTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 150_000;

    [Fact]
    public void Sample_InCube_StaysWithinBounds()
    {
        const float extents = 10.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointInCubeBounds"));
        var sampler = rng.Spatial().InCube(extents);

        for (var i = 0; i < 1000; i++)
        {
            var p = sampler.Sample();
            p.X.Should().BeInRange(-extents, extents);
            p.Y.Should().BeInRange(-extents, extents);
            p.Z.Should().BeInRange(-extents, extents);
        }
    }

    [Fact]
    public void Sample_InCube_IsSpatiallyUniform()
    {
        // For a cube, uniform spatial distribution means the X, Y, and Z
        // components should all be independent uniform distributions.
        const float extents = 1.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointInCubeUniformity"));
        var sampler = rng.Spatial().InCube(extents);

        var histX = new Histogram(-extents, extents, 20);
        var histY = new Histogram(-extents, extents, 20);
        var histZ = new Histogram(-extents, extents, 20);

        foreach (var p in sampler.Sample(SampleCount))
        {
            histX.AddSample(p.X);
            histY.AddSample(p.Y);
            histZ.AddSample(p.Z);
        }

        histX.DebugPrint(testOutputHelper, "PointInCube " + "Marginal X");
        histX.AssertIsUniform(0.15);

        histY.DebugPrint(testOutputHelper, "PointInCube " + "Marginal Y");
        histY.AssertIsUniform(0.15);

        histZ.DebugPrint(testOutputHelper, "PointInCube " + "Marginal Z");
        histZ.AssertIsUniform(0.15);
    }

    [Fact]
    public void Sample_OnCube_LiesOnSurface()
    {
        const float extents = 5.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointOnCubeBounds"));
        var sampler = rng.Spatial().OnCube(extents);

        for (var i = 0; i < 1000; i++)
        {
            var p = sampler.Sample();
            // A point is on the surface if at least one of its coordinates is exactly at the boundary.
            var onSurface = Math.Abs(p.X) - extents > -1e-6f ||
                            Math.Abs(p.Y) - extents > -1e-6f ||
                            Math.Abs(p.Z) - extents > -1e-6f;

            onSurface.Should().BeTrue("because every point must lie exactly on one of the six faces.");
            p.X.Should().BeInRange(-extents, extents);
            p.Y.Should().BeInRange(-extents, extents);
            p.Z.Should().BeInRange(-extents, extents);
        }
    }

    [Fact]
    public void Sample_OnCube_IsSpatiallyUniform()
    {
        // A uniform distribution on the surface means each of the six faces
        // should receive approximately the same number of points.
        const float extents = 1.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointOnCubeUniformity"));
        var sampler = rng.Spatial().OnCube(extents);

        var faceCounts = new int[6]; // +X, -X, +Y, -Y, +Z, -Z

        foreach (var p in sampler.Sample(SampleCount))
            if (Math.Abs(p.X - extents) < 1e-6f) faceCounts[0]++;
            else if (Math.Abs(p.X + extents) < 1e-6f) faceCounts[1]++;
            else if (Math.Abs(p.Y - extents) < 1e-6f) faceCounts[2]++;
            else if (Math.Abs(p.Y + extents) < 1e-6f) faceCounts[3]++;
            else if (Math.Abs(p.Z - extents) < 1e-6f) faceCounts[4]++;
            else faceCounts[5]++;

        var expectedCount = SampleCount / 6.0;
        foreach (var count in faceCounts)
            ((double)count).Should().BeApproximately(expectedCount, expectedCount * 0.05,
                "each face should have ~16.7% of the points.");
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        var rng = new ChiRng(1337);

        var onP = rng.Spatial().OnCube(10.0f).Sample();
        var inP = rng.Spatial().InCube(10.0f).Sample();

        onP.X.Should().BeApproximately(-3.1642f, 0.0001f);
        onP.Y.Should().BeApproximately(10f, 0.0001f);
        onP.Z.Should().BeApproximately(-6.8629f, 0.0001f);

        inP.X.Should().BeApproximately(9.7652f, 0.0001f);
        inP.Y.Should().BeApproximately(-3.7893f, 0.0001f);
        inP.Z.Should().BeApproximately(-3.5208f, 0.0001f);
    }

    [Fact]
    public void InCube_WithInvalidExtents_ThrowsArgumentOutOfRangeException()
    {
        var rng = new ChiRng();
        var act = () => { rng.Spatial().InCube(-1.0f); };

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("extents");
    }
}