using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Validation;

public class ClassificationTests
{
    [Fact]
    public void IsCanonical_AnyValue_ReturnsTrue()
    {
        ChiFixed.IsCanonical(ChiFixed.Zero).Should().BeTrue();
        ChiFixed.IsCanonical(ChiFixed.One).Should().BeTrue();
        ChiFixed.IsCanonical(ChiFixed.NegativeOne).Should().BeTrue();
        ChiFixed.IsCanonical(ChiFixed.NaN).Should().BeTrue();
        ChiFixed.IsCanonical(ChiFixed.PositiveInfinity).Should().BeTrue();
    }

    [Fact]
    public void IsComplexNumber_AnyValue_ReturnsFalse()
    {
        ChiFixed.IsComplexNumber(ChiFixed.Zero).Should().BeFalse();
        ChiFixed.IsComplexNumber(ChiFixed.One).Should().BeFalse();
        ChiFixed.IsComplexNumber(ChiFixed.NaN).Should().BeFalse();
        ChiFixed.IsComplexNumber(ChiFixed.PositiveInfinity).Should().BeFalse();
    }

    [Theory]
    [InlineData("0")]
    [InlineData("2")]
    [InlineData("-4")]
    [InlineData("100")]
    public void IsEvenInteger_EvenIntegers_ReturnsTrue(string value)
    {
        ChiFixed.IsEvenInteger(ChiFixed.Parse(value)).Should().BeTrue();
    }

    [Theory]
    [InlineData("1")]
    [InlineData("-3")]
    [InlineData("1.5")]
    [InlineData("2.5")]
    public void IsEvenInteger_OddOrFractions_ReturnsFalse(string value)
    {
        ChiFixed.IsEvenInteger(ChiFixed.Parse(value)).Should().BeFalse();
    }

    [Fact]
    public void IsFinite_FiniteValues_ReturnsTrue()
    {
        ChiFixed.IsFinite(ChiFixed.Zero).Should().BeTrue();
        ChiFixed.IsFinite(ChiFixed.One).Should().BeTrue();
        ChiFixed.IsFinite((ChiFixed)123.456m).Should().BeTrue();
        ChiFixed.IsFinite(ChiFixed.MaxValue).Should().BeTrue();
        ChiFixed.IsFinite(ChiFixed.MinValue).Should().BeTrue();
    }

    [Fact]
    public void IsImaginaryNumber_AnyValue_ReturnsFalse()
    {
        ChiFixed.IsImaginaryNumber(ChiFixed.Zero).Should().BeFalse();
        ChiFixed.IsImaginaryNumber(ChiFixed.One).Should().BeFalse();
        ChiFixed.IsImaginaryNumber(ChiFixed.NaN).Should().BeFalse();
    }

    [Fact]
    public void IsInfinity_NonInfinityValues_ReturnsFalse()
    {
        ChiFixed.IsInfinity(ChiFixed.Zero).Should().BeFalse();
        ChiFixed.IsInfinity(ChiFixed.One).Should().BeFalse();
        ChiFixed.IsInfinity(ChiFixed.NaN).Should().BeFalse();
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("100")]
    public void IsInteger_IntegerValues_ReturnsTrue(string value)
    {
        ChiFixed.IsInteger(ChiFixed.Parse(value)).Should().BeTrue();
    }

    [Theory]
    [InlineData("0.5")]
    [InlineData("-1.25")]
    [InlineData("0.000000001")]
    public void IsInteger_FractionalValues_ReturnsFalse(string value)
    {
        ChiFixed.IsInteger(ChiFixed.Parse(value)).Should().BeFalse();
    }

    [Fact]
    public void IsNaN_NonNaN_ReturnsFalse()
    {
        ChiFixed.IsNaN(ChiFixed.Zero).Should().BeFalse();
        ChiFixed.IsNaN(ChiFixed.PositiveInfinity).Should().BeFalse();
    }

    [Fact]
    public void IsNegative_NegativeValues_ReturnsTrue()
    {
        ChiFixed.IsNegative(ChiFixed.NegativeOne).Should().BeTrue();
        ChiFixed.IsNegative((ChiFixed)(-0.5m)).Should().BeTrue();
        ChiFixed.IsNegative(ChiFixed.NegativeInfinity).Should().BeTrue();
    }

    [Fact]
    public void IsNegative_NonNegativeValues_ReturnsFalse()
    {
        ChiFixed.IsNegative(ChiFixed.Zero).Should().BeFalse();
        ChiFixed.IsNegative(ChiFixed.One).Should().BeFalse();
        ChiFixed.IsNegative(ChiFixed.PositiveInfinity).Should().BeFalse();
        ChiFixed.IsNegative(ChiFixed.NaN).Should().BeFalse();
    }

    [Fact]
    public void IsNegativeInfinity_OtherValues_ReturnsFalse()
    {
        ChiFixed.IsNegativeInfinity(ChiFixed.PositiveInfinity).Should().BeFalse();
        ChiFixed.IsNegativeInfinity(ChiFixed.Zero).Should().BeFalse();
        ChiFixed.IsNegativeInfinity(ChiFixed.NaN).Should().BeFalse();
    }

    [Fact]
    public void IsNormal_NormalValues_ReturnsTrue()
    {
        ChiFixed.IsNormal(ChiFixed.One).Should().BeTrue();
        ChiFixed.IsNormal(ChiFixed.NegativeOne).Should().BeTrue();
        ChiFixed.IsNormal((ChiFixed)123.456m).Should().BeTrue();
        ChiFixed.IsNormal(ChiFixed.MaxValue).Should().BeTrue();
        ChiFixed.IsNormal(ChiFixed.MinValue).Should().BeTrue();
        ChiFixed.IsNormal(ChiFixed.Epsilon).Should().BeTrue();
    }

    [Theory]
    [InlineData("1")]
    [InlineData("-3")]
    [InlineData("99")]
    public void IsOddInteger_OddIntegers_ReturnsTrue(string value)
    {
        ChiFixed.IsOddInteger(ChiFixed.Parse(value)).Should().BeTrue();
    }

    [Theory]
    [InlineData("0")]
    [InlineData("2")]
    [InlineData("-4")]
    [InlineData("1.5")]
    public void IsOddInteger_EvenOrFractions_ReturnsFalse(string value)
    {
        ChiFixed.IsOddInteger(ChiFixed.Parse(value)).Should().BeFalse();
    }

    [Fact]
    public void IsPositive_PositiveValues_ReturnsTrue()
    {
        ChiFixed.IsPositive(ChiFixed.One).Should().BeTrue();
        ChiFixed.IsPositive((ChiFixed)0.5m).Should().BeTrue();
        ChiFixed.IsPositive(ChiFixed.PositiveInfinity).Should().BeTrue();
    }

    [Fact]
    public void IsPositive_NonPositiveValues_ReturnsFalse()
    {
        ChiFixed.IsPositive(ChiFixed.Zero).Should().BeFalse();
        ChiFixed.IsPositive(ChiFixed.NegativeOne).Should().BeFalse();
        ChiFixed.IsPositive(ChiFixed.NegativeInfinity).Should().BeFalse();
        ChiFixed.IsPositive(ChiFixed.NaN).Should().BeFalse();
    }

    [Fact]
    public void IsPositiveInfinity_OtherValues_ReturnsFalse()
    {
        ChiFixed.IsPositiveInfinity(ChiFixed.NegativeInfinity).Should().BeFalse();
        ChiFixed.IsPositiveInfinity(ChiFixed.Zero).Should().BeFalse();
        ChiFixed.IsPositiveInfinity(ChiFixed.NaN).Should().BeFalse();
    }

    [Fact]
    public void IsRealNumber_RealNumbers_ReturnsTrue()
    {
        ChiFixed.IsRealNumber(ChiFixed.Zero).Should().BeTrue();
        ChiFixed.IsRealNumber(ChiFixed.One).Should().BeTrue();
        ChiFixed.IsRealNumber((ChiFixed)123.456m).Should().BeTrue();
        ChiFixed.IsRealNumber(ChiFixed.PositiveInfinity).Should().BeTrue();
        ChiFixed.IsRealNumber(ChiFixed.NegativeInfinity).Should().BeTrue();
    }

    [Fact]
    public void IsSubnormal_AnyValue_ReturnsFalse()
    {
        ChiFixed.IsSubnormal(ChiFixed.Zero).Should().BeFalse();
        ChiFixed.IsSubnormal(ChiFixed.One).Should().BeFalse();
        ChiFixed.IsSubnormal(ChiFixed.Epsilon).Should().BeFalse();
        ChiFixed.IsSubnormal(ChiFixed.NaN).Should().BeFalse();
        ChiFixed.IsSubnormal(ChiFixed.PositiveInfinity).Should().BeFalse();
    }

    [Fact]
    public void IsZero_Zero_ReturnsTrue()
    {
        ChiFixed.IsZero(ChiFixed.Zero).Should().BeTrue();
    }

    [Fact]
    public void NegativeZero_Compared_EqualsZero()
    {
        ChiFixed.NegativeZero.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void IsZero_NegativeZero_ReturnsTrue()
    {
        ChiFixed.IsZero(ChiFixed.NegativeZero).Should().BeTrue();
    }

    [Fact]
    public void IsNegative_NegativeZero_ReturnsFalse()
    {
        ChiFixed.IsNegative(ChiFixed.NegativeZero).Should().BeFalse();
    }
}