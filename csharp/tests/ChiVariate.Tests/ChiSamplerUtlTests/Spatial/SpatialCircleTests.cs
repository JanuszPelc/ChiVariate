using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Spatial;

public class SpatialCircleTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void Sample_OnCircle_ProducesPointsAtRadius()
    {
        const float radius = 15.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointOnCircleNormalization"));
        var sampler = rng.Spatial().OnCircle(radius);

        for (var i = 0; i < 1000; i++)
        {
            var p = sampler.Sample();
            var lengthSquared = p.X * p.X + p.Y * p.Y;
            lengthSquared.Should().BeApproximately(radius * radius, 1e-4f,
                "because every point on the perimeter of a circle must have a magnitude equal to the radius.");
        }
    }

    [Fact]
    public void Sample_OnCircle_IsCenteredAtOrigin()
    {
        var rng = new ChiRng(ChiSeed.Scramble("PointOnCircleUniformity"));
        var sampler = rng.Spatial().OnCircle(1.0);

        double sumX = 0, sumY = 0;

        foreach (var p in sampler.Sample(SampleCount))
        {
            sumX += p.X;
            sumY += p.Y;
        }

        (sumX / SampleCount).Should()
            .BeApproximately(0.0, 0.01, "because the distribution should be centered at the origin.");
        (sumY / SampleCount).Should()
            .BeApproximately(0.0, 0.01, "because the distribution should be centered at the origin.");
    }

    [Fact]
    public void Sample_InCircle_ProducesPointsWithinRadius()
    {
        const float radius = 25.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointInCircleBounds"));
        var sampler = rng.Spatial().InCircle(radius);
        var rSquared = radius * radius;

        for (var i = 0; i < 1000; i++)
        {
            var p = sampler.Sample();
            var lengthSquared = p.X * p.X + p.Y * p.Y;
            (lengthSquared <= rSquared).Should().BeTrue(
                "because every point must be within or on the boundary of the circle; expected {0} <= {1}",
                lengthSquared, rSquared);
        }
    }

    [Fact]
    public void Sample_InCircle_ProducesUniformDiskMeanRadius()
    {
        var rng = new ChiRng(ChiSeed.Scramble("PointInCircleUniformity"));
        var sampler = rng.Spatial().InCircle(1.0f); // Unit circle

        var radiusHistogram = new Histogram(0, 1, 50);

        foreach (var p in sampler.Sample(SampleCount))
        {
            var r = MathF.Sqrt(p.X * p.X + p.Y * p.Y);
            radiusHistogram.AddSample(r);
        }

        radiusHistogram.DebugPrint(testOutputHelper, "Radii of Points" + " In Circle");

        const double expectedMeanRadius = 2.0 / 3.0;
        var actualMeanRadius = radiusHistogram.CalculateMean();

        actualMeanRadius.Should().BeApproximately(expectedMeanRadius, 0.01,
            "because the mean radius for a uniform 2D disk is 2/3 * R.");
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        var rng = new ChiRng(1337);

        var onP = rng.Spatial().OnCircle(10.0).Sample();
        var inP = rng.Spatial().InCircle(10.0).Sample();

        onP.X.Should().BeApproximately(-9.7156, 0.0001);
        onP.Y.Should().BeApproximately(2.3675, 0.0001);

        inP.X.Should().BeApproximately(-1.4702, 0.0001);
        inP.Y.Should().BeApproximately(3.6774, 0.0001);
    }

    [Fact]
    public void InCircle_WithInvalidRadius_ThrowsArgumentOutOfRangeException()
    {
        var rng = new ChiRng();
        var act = () => { rng.Spatial().InCircle(-1.0f); };

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("radius");
    }
}