using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Spatial;

public class SpatialSphereTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Fact]
    public void PointOnSphere_SamplesAreCorrectlyNormalized()
    {
        // Arrange
        const float radius = 5.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointOnSphereNormalization"));
        var sampler = rng.Spatial().OnSphere(radius);

        // Act & Assert
        for (var i = 0; i < 1000; i++)
        {
            var p = sampler.Sample();
            var lengthSquared = p.X * p.X + p.Y * p.Y + p.Z * p.Z;
            lengthSquared.Should().BeApproximately(radius * radius, 1e-5f,
                "because every point on the surface of a sphere must have a magnitude equal to the radius.");
        }
    }

    [Fact]
    public void PointOnSphere_DistributionIsSpatiallyUniform()
    {
        var rng = new ChiRng(ChiSeed.Scramble("PointOnSphereUniformity"));
        var sampler = rng.Spatial().OnSphere(1.0);

        double sumX = 0, sumY = 0, sumZ = 0;
        var histX = new Histogram(-1, 1, 50);
        var histY = new Histogram(-1, 1, 50);
        var histZ = new Histogram(-1, 1, 50);

        // Act
        foreach (var p in sampler.Sample(SampleCount))
        {
            sumX += p.X;
            sumY += p.Y;
            sumZ += p.Z;
            histX.AddSample(p.X);
            histY.AddSample(p.Y);
            histZ.AddSample(p.Z);
        }

        // Assert
        (sumX / SampleCount).Should().BeApproximately(0.0, 0.01);
        (sumY / SampleCount).Should().BeApproximately(0.0, 0.01);
        (sumZ / SampleCount).Should().BeApproximately(0.0, 0.01);

        var meanX = histX.CalculateMean();
        var meanY = histY.CalculateMean();
        var meanZ = histZ.CalculateMean();

        histX.CalculateStdDev(meanX).Should().BeApproximately(histY.CalculateStdDev(meanY), 0.05);
        histX.CalculateStdDev(meanX).Should().BeApproximately(histZ.CalculateStdDev(meanZ), 0.05);

        histX.DebugPrint(testOutputHelper, "PointOnSphere " + "Marginal X");
    }


    [Fact]
    public void PointInSphere_SamplesAreWithinBounds()
    {
        // Arrange
        const float radius = 10.0f;
        var rng = new ChiRng(ChiSeed.Scramble("PointInSphereBounds"));
        var sampler = rng.Spatial().InSphere(radius);
        var rSquared = radius * radius;

        // Act & Assert
        for (var i = 0; i < 1000; i++)
        {
            var p = sampler.Sample();
            var lengthSquared = p.X * p.X + p.Y * p.Y + p.Z * p.Z;

            (lengthSquared <= rSquared).Should().BeTrue(
                "because every point must be within or on the boundary of the sphere; expected {0} <= {1}",
                lengthSquared, rSquared);
        }
    }

    [Fact]
    public void PointInSphere_DistributionIsSpatiallyUniform()
    {
        var rng = new ChiRng(ChiSeed.Scramble("PointInSphereUniformity"));
        var sampler = rng.Spatial().InSphere(1.0f);

        var radiusHistogram = new Histogram(0, 1, 50);

        // Act
        foreach (var p in sampler.Sample(SampleCount))
        {
            var r = MathF.Sqrt(p.X * p.X + p.Y * p.Y + p.Z * p.Z);
            radiusHistogram.AddSample(r);
        }

        // Assert
        radiusHistogram.DebugPrint(testOutputHelper, "Radii of " + "Points In Sphere");

        var firstBinCount = radiusHistogram.Bins[0];
        var lastBinCount = radiusHistogram.Bins[^1];

        ((double)lastBinCount / firstBinCount).Should().BeGreaterThan(10.0,
            "because the PDF of the radius should be proportional to r^2, thus heavily weighted towards the outer edge.");
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        var rng = new ChiRng(1337);

        var onP = rng.Spatial().OnSphere(10.0).Sample();
        var inP = rng.Spatial().InSphere(10.0).Sample();

        onP.X.Should().BeApproximately(-1.1006, 0.0001);
        onP.Y.Should().BeApproximately(-9.9283, 0.0001);
        onP.Z.Should().BeApproximately(0.4642, 0.0001);

        inP.X.Should().BeApproximately(-5.3724, 0.0001);
        inP.Y.Should().BeApproximately(6.6621, 0.0001);
        inP.Z.Should().BeApproximately(2.4115, 0.0001);
    }

    [Fact]
    public void PointSphere_WithInvalidRadius_ThrowsArgumentOutOfRangeException()
    {
        var rng = new ChiRng();
        var act = () => { rng.Spatial().InSphere(-1.0f); };

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("radius");
    }
}