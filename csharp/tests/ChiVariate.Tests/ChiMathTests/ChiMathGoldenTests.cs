using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiMathTests;

public class ChiMathGoldenTests
{
    #region Phi Constants

    [Fact]
    public void Phi_Double_ReturnsGoldenRatio()
    {
        var phi = ChiMath.Const<double>.Phi;

        phi.Should().BeApproximately(1.6180339887498949, 1e-14);
    }

    [Fact]
    public void Phi_Float_ReturnsGoldenRatio()
    {
        var phi = ChiMath.Const<float>.Phi;

        phi.Should().BeApproximately(1.6180340f, 1e-6f);
    }

    [Fact]
    public void Phi_Decimal_ReturnsGoldenRatio()
    {
        var phi = ChiMath.Const<decimal>.Phi;

        phi.Should().BeApproximately(1.6180339887498948m, 1e-15m);
    }

    [Fact]
    public void Phi_ChiFixed_ReturnsGoldenRatio()
    {
        var phi = ChiMath.Const<ChiFixed>.Phi;

        ((decimal)phi).Should().BeApproximately(1.6180339887m, 1e-8m);
    }

    [Fact]
    public void PhiConjugate_Double_ReturnsGoldenRatioConjugate()
    {
        var phiConj = ChiMath.Const<double>.PhiConjugate;

        phiConj.Should().BeApproximately(0.6180339887498949, 1e-14);
    }

    [Fact]
    public void PhiConjugate_Double_EqualsPhiMinusOne()
    {
        var phi = ChiMath.Const<double>.Phi;
        var phiConj = ChiMath.Const<double>.PhiConjugate;

        phiConj.Should().BeApproximately(phi - 1.0, 1e-14);
    }

    [Fact]
    public void PhiConjugate_Double_EqualsOneOverPhi()
    {
        var phi = ChiMath.Const<double>.Phi;
        var phiConj = ChiMath.Const<double>.PhiConjugate;

        phiConj.Should().BeApproximately(1.0 / phi, 1e-14);
    }

    [Fact]
    public void Phi_Squared_EqualsPhiPlusOne()
    {
        // φ² = φ + 1
        var phi = ChiMath.Const<double>.Phi;

        var phiSquared = phi * phi;
        var phiPlusOne = phi + 1.0;

        phiSquared.Should().BeApproximately(phiPlusOne, 1e-14);
    }

    #endregion

    #region Golden Sequence Basic

    [Fact]
    public void Golden_IndexZero_ReturnsZero()
    {
        var result = ChiMath.Golden<double>(0);

        result.Should().Be(0.0);
    }

    [Fact]
    public void Golden_IndexOne_ReturnsPhiConjugate()
    {
        var result = ChiMath.Golden<double>(1);
        var expected = ChiMath.Const<double>.PhiConjugate;

        result.Should().BeApproximately(expected, 1e-14);
    }

    [Fact]
    public void Golden_AnyIndex_ReturnsValueInUnitInterval()
    {
        for (var i = 0; i < 1000; i++)
        {
            var result = ChiMath.Golden<double>(i);

            (result >= 0.0).Should().BeTrue();
            (result < 1.0).Should().BeTrue();
        }
    }

    [Fact]
    public void Golden_SameIndex_ReturnsSameValue()
    {
        var result1 = ChiMath.Golden<double>(42);
        var result2 = ChiMath.Golden<double>(42);

        result1.Should().Be(result2);
    }

    #endregion

    #region Golden Sequence with Seed

    [Fact]
    public void Golden_WithSeedAtIndexZero_ReturnsSeed()
    {
        const double seed = 0.3;
        var result = ChiMath.Golden(0, seed);

        result.Should().BeApproximately(0.3, 1e-14);
    }

    [Fact]
    public void Golden_WithSeed_ReturnsValueInUnitInterval()
    {
        var seed = 0.7;
        for (var i = 0; i < 100; i++)
        {
            var result = ChiMath.Golden(i, seed);

            (result >= 0.0).Should().BeTrue();
            (result < 1.0).Should().BeTrue();
        }
    }

    [Fact]
    public void Golden_DifferentSeeds_ProducesDifferentValues()
    {
        var result1 = ChiMath.Golden(5, 0.0);
        var result2 = ChiMath.Golden(5, 0.5);

        result1.Should().NotBe(result2);
    }

    #endregion

    #region Golden Sequence Distribution

    [Fact]
    public void Golden_TenPoints_HasBoundedGapRatio()
    {
        var points = new double[10];
        for (var i = 0; i < 10; i++)
            points[i] = ChiMath.Golden<double>(i);

        Array.Sort(points);

        var minGap = double.MaxValue;
        var maxGap = double.MinValue;

        for (var i = 1; i < points.Length; i++)
        {
            var gap = points[i] - points[i - 1];
            minGap = Math.Min(minGap, gap);
            maxGap = Math.Max(maxGap, gap);
        }

        var gapRatio = maxGap / minGap;
        (gapRatio < 3.0).Should().BeTrue($"Gap ratio {gapRatio} is too large");
    }

    [Fact]
    public void Golden_ThousandPoints_CoversAllBucketsEvenly()
    {
        const int buckets = 10;
        const int samples = 1000;
        var counts = new int[buckets];

        for (var i = 0; i < samples; i++)
        {
            var value = ChiMath.Golden<double>(i);
            var bucket = (int)(value * buckets);
            if (bucket == buckets) bucket = buckets - 1;
            counts[bucket]++;
        }

        var expected = samples / buckets;
        foreach (var count in counts)
        {
            (count > expected * 0.8).Should().BeTrue();
            (count < expected * 1.2).Should().BeTrue();
        }
    }

    #endregion

    #region Type Coverage

    [Fact]
    public void Golden_Float_ReturnsPhiConjugate()
    {
        var result = ChiMath.Golden<float>(1);
        var expected = ChiMath.Const<float>.PhiConjugate;

        result.Should().BeApproximately(expected, 1e-6f);
    }

    [Fact]
    public void Golden_Decimal_ReturnsPhiConjugate()
    {
        var result = ChiMath.Golden<decimal>(1);
        var expected = ChiMath.Const<decimal>.PhiConjugate;

        result.Should().BeApproximately(expected, 1e-20m);
    }

    [Fact]
    public void Golden_ChiFixed_ReturnsPhiConjugate()
    {
        var result = ChiMath.Golden<ChiFixed>(1);
        var expected = ChiMath.Const<ChiFixed>.PhiConjugate;

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-8m);
    }

    [Fact]
    public void Golden_ChiFixedAnyIndex_ReturnsValueInUnitInterval()
    {
        for (var i = 0; i < 100; i++)
        {
            var result = ChiMath.Golden<ChiFixed>(i);

            (result >= ChiFixed.Zero).Should().BeTrue();
            (result < ChiFixed.One).Should().BeTrue();
        }
    }

    #endregion
}