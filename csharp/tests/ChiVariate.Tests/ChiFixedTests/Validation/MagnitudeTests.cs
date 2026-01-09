using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Validation;

public class MagnitudeTests
{
    [Fact]
    public void MaxMagnitude_BothPositive_ReturnsLarger()
    {
        var result = ChiFixed.MaxMagnitude((ChiFixed)3m, (ChiFixed)5m);

        Assert.Equal((ChiFixed)5m, result);
    }

    [Fact]
    public void MaxMagnitude_BothNegative_ReturnsLargerMagnitude()
    {
        var result = ChiFixed.MaxMagnitude((ChiFixed)(-3m), (ChiFixed)(-5m));

        Assert.Equal((ChiFixed)(-5m), result);
    }

    [Fact]
    public void MaxMagnitude_MixedSigns_ReturnsLargerMagnitude()
    {
        var result = ChiFixed.MaxMagnitude((ChiFixed)3m, (ChiFixed)(-5m));

        Assert.Equal((ChiFixed)(-5m), result);
    }

    [Fact]
    public void MaxMagnitude_EqualMagnitudes_PrefersPositive()
    {
        var result = ChiFixed.MaxMagnitude((ChiFixed)5m, (ChiFixed)(-5m));

        Assert.Equal((ChiFixed)5m, result);
    }

    [Fact]
    public void MaxMagnitude_BothZero_ReturnsPositiveZero()
    {
        var result = ChiFixed.MaxMagnitude(ChiFixed.Zero, ChiFixed.Zero);

        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void MaxMagnitudeNumber_BothPositive_ReturnsLarger()
    {
        var result = ChiFixed.MaxMagnitudeNumber((ChiFixed)3m, (ChiFixed)5m);

        Assert.Equal((ChiFixed)5m, result);
    }

    [Fact]
    public void MaxMagnitudeNumber_EqualMagnitudes_PrefersPositive()
    {
        var result = ChiFixed.MaxMagnitudeNumber((ChiFixed)5m, (ChiFixed)(-5m));

        Assert.Equal((ChiFixed)5m, result);
    }

    [Fact]
    public void MinMagnitude_BothPositive_ReturnsSmaller()
    {
        var result = ChiFixed.MinMagnitude((ChiFixed)3m, (ChiFixed)5m);

        Assert.Equal((ChiFixed)3m, result);
    }

    [Fact]
    public void MinMagnitude_BothNegative_ReturnsSmallerMagnitude()
    {
        var result = ChiFixed.MinMagnitude((ChiFixed)(-3m), (ChiFixed)(-5m));

        Assert.Equal((ChiFixed)(-3m), result);
    }

    [Fact]
    public void MinMagnitude_MixedSigns_ReturnsSmallerMagnitude()
    {
        var result = ChiFixed.MinMagnitude((ChiFixed)3m, (ChiFixed)(-5m));

        Assert.Equal((ChiFixed)3m, result);
    }

    [Fact]
    public void MinMagnitude_EqualMagnitudes_PrefersNegative()
    {
        var result = ChiFixed.MinMagnitude((ChiFixed)5m, (ChiFixed)(-5m));

        Assert.Equal((ChiFixed)(-5m), result);
    }

    [Fact]
    public void MinMagnitude_WithInfinity_ReturnsFiniteValue()
    {
        var result = ChiFixed.MinMagnitude((ChiFixed)100m, ChiFixed.PositiveInfinity);

        Assert.Equal((ChiFixed)100m, result);
    }

    [Fact]
    public void MinMagnitude_BothZero_ReturnsNegativeZero()
    {
        var result = ChiFixed.MinMagnitude(ChiFixed.Zero, ChiFixed.Zero);

        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void MinMagnitudeNumber_BothPositive_ReturnsSmaller()
    {
        var result = ChiFixed.MinMagnitudeNumber((ChiFixed)3m, (ChiFixed)5m);

        Assert.Equal((ChiFixed)3m, result);
    }

    [Fact]
    public void MinMagnitudeNumber_EqualMagnitudes_PrefersNegative()
    {
        var result = ChiFixed.MinMagnitudeNumber((ChiFixed)5m, (ChiFixed)(-5m));

        Assert.Equal((ChiFixed)(-5m), result);
    }

    [Fact]
    public void MinMagnitudeNumber_WithInfinity_ReturnsFiniteValue()
    {
        var result = ChiFixed.MinMagnitudeNumber((ChiFixed)100m, ChiFixed.PositiveInfinity);

        Assert.Equal((ChiFixed)100m, result);
    }
}