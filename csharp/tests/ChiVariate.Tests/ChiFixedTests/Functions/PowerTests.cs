using System.Globalization;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Functions;

public class PowerTests
{
    private const decimal RelativeTolerance = 0.001m;
    private const decimal MinAbsoluteTolerance = 0.0001m;

    [Fact]
    public void Pow_AnyBaseToZero_ReturnsOne()
    {
        var baseVal = (ChiFixed)5.0m;
        var result = ChiFixed.Pow(baseVal, ChiFixed.Zero);
        AssertEqual((ChiFixed)1.0m, result);
    }

    [Fact]
    public void Pow_AnyBaseToOne_ReturnsSameValue()
    {
        var baseVal = (ChiFixed)5.0m;
        var result = ChiFixed.Pow(baseVal, ChiFixed.One);
        AssertEqual(baseVal, result);
    }

    [Fact]
    public void Pow_OneToAnyPower_ReturnsOne()
    {
        var exponent = (ChiFixed)5.0m;
        var result = ChiFixed.Pow(ChiFixed.One, exponent);
        AssertEqual(ChiFixed.One, result);
    }

    [Fact]
    public void Pow_ZeroToZero_ReturnsOne()
    {
        var result = ChiFixed.Pow(ChiFixed.Zero, ChiFixed.Zero);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("2", "3", "8")]
    [InlineData("3", "2", "9")]
    [InlineData("10", "2", "100")]
    [InlineData("5", "3", "125")]
    public void Pow_PositiveIntegerExponents_ReturnsCorrectValue(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Pow(baseVal, exponent);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("2", "-1", "0.5")]
    [InlineData("2", "-2", "0.25")]
    [InlineData("10", "-1", "0.1")]
    [InlineData("5", "-2", "0.04")]
    public void Pow_NegativeIntegerExponents_ReturnsCorrectValue(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Pow(baseVal, exponent);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("4", "0.5", "2")]
    [InlineData("9", "0.5", "3")]
    [InlineData("8", "0.333333333", "2")]
    [InlineData("27", "0.333333333", "3")]
    public void Pow_FractionalExponents_ReturnsCorrectValue(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Pow(baseVal, exponent);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-2", "3", "-8")]
    [InlineData("-2", "2", "4")]
    [InlineData("-3", "3", "-27")]
    [InlineData("-5", "2", "25")]
    public void Pow_NegativeBaseIntegerExponent_ReturnsCorrectValue(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Pow(baseVal, exponent);
        AssertEqual(expected, result);
    }

    [Fact]
    public void Pow_NegativeBaseFractionalExponent_ThrowsException()
    {
        var baseVal = (ChiFixed)(-4.0m);
        var exponent = (ChiFixed)0.5m;
        Assert.Throws<ArgumentException>(() => ChiFixed.Pow(baseVal, exponent));
    }

    [Theory]
    [InlineData("2", "10", "1024")]
    [InlineData("3", "5", "243")]
    public void Pow_LargerExponents_ReturnsCorrectValue(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Pow(baseVal, exponent);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("2.5", "2", "6.25")]
    [InlineData("1.5", "3", "3.375")]
    [InlineData("0.5", "2", "0.25")]
    public void Pow_DecimalBase_ReturnsCorrectValue(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Pow(baseVal, exponent);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("2", "0.5", "1.414213562")]
    [InlineData("10", "0.5", "3.162277660")]
    public void Pow_SquareRoot_ReturnsCorrectValue(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Pow(baseVal, exponent);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("8", "0.333333333", "2")]
    [InlineData("27", "0.333333333", "3")]
    public void Pow_CubeRoot_ReturnsCorrectValue(string baseStr, string expStr, string expectedStr)
    {
        var baseVal = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var exponent = (ChiFixed)decimal.Parse(expStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Pow(baseVal, exponent);
        AssertEqual(expected, result);
    }

    private static void AssertEqual(ChiFixed expected, ChiFixed actual)
    {
        var expectedDecimal = (decimal)expected;
        var actualDecimal = (decimal)actual;
        var difference = Math.Abs(expectedDecimal - actualDecimal);
        var tolerance = Math.Max(MinAbsoluteTolerance, Math.Abs(expectedDecimal) * RelativeTolerance);
        Assert.True(difference < tolerance,
            $"Expected: {expectedDecimal}, Got: {actualDecimal}, Diff: {difference}, RelErr: {(expectedDecimal != 0 ? difference / Math.Abs(expectedDecimal) : difference):P4}");
    }
}