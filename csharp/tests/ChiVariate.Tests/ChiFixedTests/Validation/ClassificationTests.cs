using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Validation;

public class ClassificationTests
{
    [Fact]
    public void IsCanonical_VariousValues_ReturnsTrue()
    {
        Assert.True(ChiFixed.IsCanonical(ChiFixed.Zero));
        Assert.True(ChiFixed.IsCanonical(ChiFixed.One));
        Assert.True(ChiFixed.IsCanonical(ChiFixed.NegativeOne));
        Assert.True(ChiFixed.IsCanonical(ChiFixed.NaN));
        Assert.True(ChiFixed.IsCanonical(ChiFixed.PositiveInfinity));
    }

    [Fact]
    public void IsComplexNumber_VariousValues_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsComplexNumber(ChiFixed.Zero));
        Assert.False(ChiFixed.IsComplexNumber(ChiFixed.One));
        Assert.False(ChiFixed.IsComplexNumber(ChiFixed.NaN));
        Assert.False(ChiFixed.IsComplexNumber(ChiFixed.PositiveInfinity));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("2")]
    [InlineData("-4")]
    [InlineData("100")]
    public void IsEvenInteger_EvenIntegers_ReturnsTrue(string value)
    {
        Assert.True(ChiFixed.IsEvenInteger(ChiFixed.Parse(value)));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("-3")]
    [InlineData("1.5")]
    [InlineData("2.5")]
    public void IsEvenInteger_OddOrFractions_ReturnsFalse(string value)
    {
        Assert.False(ChiFixed.IsEvenInteger(ChiFixed.Parse(value)));
    }

    [Fact]
    public void IsFinite_FiniteValues_ReturnsTrue()
    {
        Assert.True(ChiFixed.IsFinite(ChiFixed.Zero));
        Assert.True(ChiFixed.IsFinite(ChiFixed.One));
        Assert.True(ChiFixed.IsFinite((ChiFixed)123.456m));
        Assert.True(ChiFixed.IsFinite(ChiFixed.MaxValue));
        Assert.True(ChiFixed.IsFinite(ChiFixed.MinValue));
    }

    [Fact]
    public void IsImaginaryNumber_VariousValues_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsImaginaryNumber(ChiFixed.Zero));
        Assert.False(ChiFixed.IsImaginaryNumber(ChiFixed.One));
        Assert.False(ChiFixed.IsImaginaryNumber(ChiFixed.NaN));
    }

    [Fact]
    public void IsInfinity_NonInfinityValues_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsInfinity(ChiFixed.Zero));
        Assert.False(ChiFixed.IsInfinity(ChiFixed.One));
        Assert.False(ChiFixed.IsInfinity(ChiFixed.NaN));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("100")]
    public void IsInteger_IntegerValues_ReturnsTrue(string value)
    {
        Assert.True(ChiFixed.IsInteger(ChiFixed.Parse(value)));
    }

    [Theory]
    [InlineData("0.5")]
    [InlineData("-1.25")]
    [InlineData("0.000000000001")]
    public void IsInteger_FractionalValues_ReturnsFalse(string value)
    {
        Assert.False(ChiFixed.IsInteger(ChiFixed.Parse(value)));
    }

    [Fact]
    public void IsNaN_NonNaN_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsNaN(ChiFixed.Zero));
        Assert.False(ChiFixed.IsNaN(ChiFixed.PositiveInfinity));
    }

    [Fact]
    public void IsNegative_NegativeValues_ReturnsTrue()
    {
        Assert.True(ChiFixed.IsNegative(ChiFixed.NegativeOne));
        Assert.True(ChiFixed.IsNegative((ChiFixed)(-0.5m)));
        Assert.True(ChiFixed.IsNegative(ChiFixed.NegativeInfinity));
    }

    [Fact]
    public void IsNegative_NonNegativeValues_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsNegative(ChiFixed.Zero));
        Assert.False(ChiFixed.IsNegative(ChiFixed.One));
        Assert.False(ChiFixed.IsNegative(ChiFixed.PositiveInfinity));
        Assert.False(ChiFixed.IsNegative(ChiFixed.NaN));
    }

    [Fact]
    public void IsNegativeInfinity_OtherValues_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsNegativeInfinity(ChiFixed.PositiveInfinity));
        Assert.False(ChiFixed.IsNegativeInfinity(ChiFixed.Zero));
        Assert.False(ChiFixed.IsNegativeInfinity(ChiFixed.NaN));
    }

    [Fact]
    public void IsNormal_NormalValues_ReturnsTrue()
    {
        Assert.True(ChiFixed.IsNormal(ChiFixed.One));
        Assert.True(ChiFixed.IsNormal(ChiFixed.NegativeOne));
        Assert.True(ChiFixed.IsNormal((ChiFixed)123.456m));
        Assert.True(ChiFixed.IsNormal(ChiFixed.MaxValue));
        Assert.True(ChiFixed.IsNormal(ChiFixed.MinValue));
        Assert.True(ChiFixed.IsNormal(ChiFixed.Epsilon));
    }

    [Theory]
    [InlineData("1")]
    [InlineData("-3")]
    [InlineData("99")]
    public void IsOddInteger_OddIntegers_ReturnsTrue(string value)
    {
        Assert.True(ChiFixed.IsOddInteger(ChiFixed.Parse(value)));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("2")]
    [InlineData("-4")]
    [InlineData("1.5")]
    public void IsOddInteger_EvenOrFractions_ReturnsFalse(string value)
    {
        Assert.False(ChiFixed.IsOddInteger(ChiFixed.Parse(value)));
    }

    [Fact]
    public void IsPositive_PositiveValues_ReturnsTrue()
    {
        Assert.True(ChiFixed.IsPositive(ChiFixed.One));
        Assert.True(ChiFixed.IsPositive((ChiFixed)0.5m));
        Assert.True(ChiFixed.IsPositive(ChiFixed.PositiveInfinity));
    }

    [Fact]
    public void IsPositive_NonPositiveValues_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsPositive(ChiFixed.Zero));
        Assert.False(ChiFixed.IsPositive(ChiFixed.NegativeOne));
        Assert.False(ChiFixed.IsPositive(ChiFixed.NegativeInfinity));
        Assert.False(ChiFixed.IsPositive(ChiFixed.NaN));
    }

    [Fact]
    public void IsPositiveInfinity_OtherValues_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsPositiveInfinity(ChiFixed.NegativeInfinity));
        Assert.False(ChiFixed.IsPositiveInfinity(ChiFixed.Zero));
        Assert.False(ChiFixed.IsPositiveInfinity(ChiFixed.NaN));
    }

    [Fact]
    public void IsRealNumber_RealNumbers_ReturnsTrue()
    {
        Assert.True(ChiFixed.IsRealNumber(ChiFixed.Zero));
        Assert.True(ChiFixed.IsRealNumber(ChiFixed.One));
        Assert.True(ChiFixed.IsRealNumber((ChiFixed)123.456m));
        Assert.True(ChiFixed.IsRealNumber(ChiFixed.PositiveInfinity));
        Assert.True(ChiFixed.IsRealNumber(ChiFixed.NegativeInfinity));
    }

    [Fact]
    public void IsSubnormal_VariousValues_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsSubnormal(ChiFixed.Zero));
        Assert.False(ChiFixed.IsSubnormal(ChiFixed.One));
        Assert.False(ChiFixed.IsSubnormal(ChiFixed.Epsilon));
        Assert.False(ChiFixed.IsSubnormal(ChiFixed.NaN));
        Assert.False(ChiFixed.IsSubnormal(ChiFixed.PositiveInfinity));
    }

    [Fact]
    public void IsZero_Zero_ReturnsTrue()
    {
        Assert.True(ChiFixed.IsZero(ChiFixed.Zero));
    }

    [Fact]
    public void NegativeZero_Compared_EqualsZero()
    {
        Assert.Equal(ChiFixed.Zero, ChiFixed.NegativeZero);
    }

    [Fact]
    public void NegativeZero_IsZero_ReturnsTrue()
    {
        Assert.True(ChiFixed.IsZero(ChiFixed.NegativeZero));
    }

    [Fact]
    public void NegativeZero_IsNegative_ReturnsFalse()
    {
        Assert.False(ChiFixed.IsNegative(ChiFixed.NegativeZero));
    }
}