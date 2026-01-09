using System.Globalization;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Functions;

public class ExponentialTests
{
    private const decimal RelativeTolerance = 0.001m;
    private const decimal MinAbsoluteTolerance = 0.0001m;

    private static void AssertEqual(ChiFixed expected, ChiFixed actual)
    {
        var expectedDecimal = (decimal)expected;
        var actualDecimal = (decimal)actual;
        var difference = Math.Abs(expectedDecimal - actualDecimal);
        var tolerance = Math.Max(MinAbsoluteTolerance, Math.Abs(expectedDecimal) * RelativeTolerance);
        (difference < tolerance).Should().BeTrue(
            $"Expected: {expectedDecimal}, Got: {actualDecimal}, Diff: {difference}, RelErr: {(expectedDecimal != 0 ? difference / Math.Abs(expectedDecimal) : difference):P4}");
    }

    #region Exp Tests

    [Fact]
    public void Exp_Zero_ReturnsOne()
    {
        var result = ChiFixed.Exp(ChiFixed.Zero);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("1", "2.718281828")]
    [InlineData("2", "7.389056099")]
    [InlineData("0.5", "1.648721271")]
    [InlineData("0.1", "1.105170918")]
    public void Exp_PositiveValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-1", "0.367879441")]
    [InlineData("-2", "0.135335283")]
    [InlineData("-0.5", "0.606530660")]
    public void Exp_NegativeValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("5", "148.413159103")]
    [InlineData("10", "22026.465794807")]
    public void Exp_LargerValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("29")]
    [InlineData("50")]
    [InlineData("100")]
    public void Exp_LargePositiveValue_ReturnsSaturatedValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp(input);
        (result == ChiFixed.PositiveInfinity || (decimal)result > 1000000m).Should().BeTrue();
    }

    [Theory]
    [InlineData("-29")]
    [InlineData("-50")]
    [InlineData("-100")]
    public void Exp_LargeNegativeValue_ReturnsVerySmallValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp(input);
        ((decimal)result >= 0m).Should().BeTrue();
        ((decimal)result < 0.000001m).Should().BeTrue();
    }

    #endregion

    #region Exp2 Tests

    [Fact]
    public void Exp2_Zero_ReturnsOne()
    {
        var result = ChiFixed.Exp2(ChiFixed.Zero);
        AssertEqual(ChiFixed.One, result);
    }

    [Fact]
    public void Exp2_One_ReturnsTwo()
    {
        var result = ChiFixed.Exp2(ChiFixed.One);
        AssertEqual((ChiFixed)2m, result);
    }

    [Theory]
    [InlineData("2", "4")]
    [InlineData("3", "8")]
    [InlineData("4", "16")]
    [InlineData("5", "32")]
    [InlineData("10", "1024")]
    public void Exp2_IntegerPowers_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp2(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-1", "0.5")]
    [InlineData("-2", "0.25")]
    [InlineData("-3", "0.125")]
    [InlineData("-10", "0.0009765625")]
    public void Exp2_NegativeIntegerPowers_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp2(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.5", "1.414213562")]
    [InlineData("1.5", "2.828427125")]
    [InlineData("2.5", "5.656854249")]
    public void Exp2_FractionalPowers_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp2(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-0.5", "0.707106781")]
    [InlineData("-1.5", "0.353553391")]
    public void Exp2_NegativeFractionalPowers_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp2(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Exp10 Tests

    [Fact]
    public void Exp10_Zero_ReturnsOne()
    {
        var result = ChiFixed.Exp10(ChiFixed.Zero);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("1", "10")]
    [InlineData("2", "100")]
    [InlineData("3", "1000")]
    [InlineData("4", "10000")]
    public void Exp10_IntegerPowers_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp10(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-1", "0.1")]
    [InlineData("-2", "0.01")]
    [InlineData("-3", "0.001")]
    [InlineData("-4", "0.0001")]
    public void Exp10_NegativeIntegerPowers_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp10(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.5", "3.162277660")]
    [InlineData("1.5", "31.622776602")]
    [InlineData("0.25", "1.778279410")]
    public void Exp10_FractionalPowers_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp10(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-0.5", "0.316227766")]
    [InlineData("-1.5", "0.031622777")]
    public void Exp10_NegativeFractionalPowers_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp10(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.1", "1.258925412")]
    [InlineData("0.01", "1.023292992")]
    public void Exp10_SmallFractionalPowers_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Exp10(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Edge Cases and Relationships

    [Fact]
    public void Exp2_ValidInput_MatchesPow()
    {
        var x = (ChiFixed)3.5m;
        var exp2Result = ChiFixed.Exp2(x);
        var powResult = ChiFixed.Pow((ChiFixed)2m, x);
        AssertEqual(exp2Result, powResult);
    }

    [Fact]
    public void Exp10_ValidInput_MatchesPow()
    {
        var x = (ChiFixed)2.5m;
        var exp10Result = ChiFixed.Exp10(x);
        var powResult = ChiFixed.Pow((ChiFixed)10m, x);
        AssertEqual(exp10Result, powResult);
    }

    [Fact]
    public void Exp_ValidInput_MatchesPow()
    {
        var x = (ChiFixed)1.5m;
        var expResult = ChiFixed.Exp(x);
        var powResult = ChiFixed.Pow(ChiFixed.E, x);
        AssertEqual(expResult, powResult);
    }

    #endregion
}