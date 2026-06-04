using System.Globalization;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Functions;

public class NumericTests
{
    private const decimal Tolerance = 0.000000001m;

    [Fact]
    public void Abs_PositiveValue_ReturnsSameValue()
    {
        var value = (ChiFixed)123.45m;
        ChiFixed.Abs(value).Should().Be(value);
    }

    [Fact]
    public void Abs_NegativeValue_ReturnsPositiveValue()
    {
        var value = (ChiFixed)(-123.45m);
        var expected = (ChiFixed)123.45m;
        ChiFixed.Abs(value).Should().Be(expected);
    }

    [Fact]
    public void Abs_Zero_ReturnsZero()
    {
        ChiFixed.Abs(ChiFixed.Zero).Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Round_NegativeDigits_ThrowsArgumentOutOfRangeException()
    {
        var act = () => ChiFixed.Round((ChiFixed)1.23m, -1, MidpointRounding.ToEven);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("2.5", "2")]
    [InlineData("3.5", "4")]
    [InlineData("2.75", "3")]
    [InlineData("2.25", "2")]
    [InlineData("-2.5", "-2")]
    [InlineData("-3.5", "-4")]
    [InlineData("-2.75", "-3")]
    [InlineData("-2.25", "-2")]
    [InlineData("0", "0")]
    public void Round_DigitsZeroToEven_ReturnsExpectedResult(string input, string expected)
    {
        PerformRoundingTest(input, 0, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("2.5", "3")]
    [InlineData("3.5", "4")]
    [InlineData("2.25", "2")]
    [InlineData("-2.5", "-3")]
    [InlineData("-3.5", "-4")]
    [InlineData("-2.25", "-2")]
    public void Round_DigitsZeroAwayFromZero_ReturnsExpectedResult(string input, string expected)
    {
        PerformRoundingTest(input, 0, MidpointRounding.AwayFromZero, expected);
    }

    [Theory]
    [InlineData("2.75", "2")]
    [InlineData("2.25", "2")]
    [InlineData("-2.75", "-2")]
    [InlineData("-2.25", "-2")]
    public void Round_DigitsZeroToZero_ReturnsExpectedResult(string input, string expected)
    {
        PerformRoundingTest(input, 0, MidpointRounding.ToZero, expected);
    }

    [Theory]
    [InlineData("2.75", "2")]
    [InlineData("2.25", "2")]
    [InlineData("-2.75", "-3")]
    [InlineData("-2.25", "-3")]
    public void Round_DigitsZeroToNegativeInfinity_ReturnsExpectedResult(string input, string expected)
    {
        PerformRoundingTest(input, 0, MidpointRounding.ToNegativeInfinity, expected);
    }

    [Theory]
    [InlineData("2.75", "3")]
    [InlineData("2.25", "3")]
    [InlineData("-2.75", "-2")]
    [InlineData("-2.25", "-2")]
    public void Round_DigitsZeroToPositiveInfinity_ReturnsExpectedResult(string input, string expected)
    {
        PerformRoundingTest(input, 0, MidpointRounding.ToPositiveInfinity, expected);
    }

    [Theory]
    [InlineData("1.125", 2, "1.12")]
    [InlineData("2.375", 2, "2.38")]
    public void Round_PositiveDigitsToEven_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("1.125", 2, "1.13")]
    [InlineData("-1.125", 2, "-1.13")]
    public void Round_PositiveDigitsAwayFromZero_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.AwayFromZero, expected);
    }

    [Theory]
    [InlineData("1.129", 2, "1.12")]
    [InlineData("-1.129", 2, "-1.12")]
    public void Round_PositiveDigitsToZero_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToZero, expected);
    }

    [Theory]
    [InlineData("1.129", 2, "1.12")]
    [InlineData("-1.121", 2, "-1.13")]
    public void Round_PositiveDigitsToNegativeInfinity_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToNegativeInfinity, expected);
    }

    [Theory]
    [InlineData("1.121", 2, "1.13")]
    [InlineData("-1.129", 2, "-1.12")]
    public void Round_PositiveDigitsToPositiveInfinity_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToPositiveInfinity, expected);
    }

    [Fact]
    public void Round_MaxDigits_PreservesValue()
    {
        var value = (ChiFixed)3.141592653589m;
        var result = ChiFixed.Round(value, 12, MidpointRounding.ToEven);
        var difference = Math.Abs(ToDecimal(value) - ToDecimal(result));
        (difference < Tolerance).Should().BeTrue($"Difference {difference} was not less than tolerance {Tolerance}");
    }

    [Theory]
    [InlineData("1.25", 1, "1.2")]
    [InlineData("1.75", 1, "1.8")]
    [InlineData("1.44", 1, "1.4")]
    [InlineData("1.56", 1, "1.6")]
    [InlineData("-1.25", 1, "-1.2")]
    [InlineData("-1.75", 1, "-1.8")]
    public void Round_Digits1ToEven_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("1.234375", 3, "1.234")]
    [InlineData("1.2359375", 3, "1.236")]
    [InlineData("1.236328125", 3, "1.236")]
    [InlineData("-1.234375", 3, "-1.234")]
    [InlineData("-1.2359375", 3, "-1.236")]
    public void Round_Digits3ToEven_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("1.1234375", 5, "1.12344")]
    [InlineData("1.123466796875", 5, "1.12347")]
    [InlineData("-1.1234375", 5, "-1.12344")]
    public void Round_Digits5ToEven_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("1.123456787109375", 8, "1.12345679")]
    [InlineData("1.123456695556640625", 8, "1.1234567")]
    public void Round_Digits8ToEven_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("12345.6875", 2, "12345.69")]
    [InlineData("12345.671875", 2, "12345.67")]
    [InlineData("-12345.6875", 2, "-12345.69")]
    [InlineData("99999.99609375", 2, "100000")]
    [InlineData("-99999.99609375", 2, "-100000")]
    public void Round_LargeNumbersToEven_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("0.00122070", 4, "0.0012")]
    [InlineData("0.00138092041", 4, "0.0014")]
    [InlineData("0.00140380", 4, "0.0014")]
    [InlineData("-0.00122070", 4, "-0.0012")]
    [InlineData("-0.00138092041", 4, "-0.0014")]
    public void Round_SmallNumbersToEven_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("9.99609375", 2, "10")]
    [InlineData("9.9998779296875", 3, "10")]
    [InlineData("0.99609375", 2, "1")]
    [InlineData("-9.99609375", 2, "-10")]
    [InlineData("-0.99609375", 2, "-1")]
    public void Round_CarryToNextIntegerToEven_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("5", 0, "5")]
    [InlineData("5", 2, "5")]
    [InlineData("5", 5, "5")]
    [InlineData("-5", 0, "-5")]
    [InlineData("-5", 2, "-5")]
    public void Round_WholeNumbers_Unchanged(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("0", 0, "0")]
    [InlineData("0", 2, "0")]
    [InlineData("0", 5, "0")]
    public void Round_Zero_RemainsZero(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Theory]
    [InlineData("1.25", 1, "1.3")]
    [InlineData("1.36", 1, "1.4")]
    [InlineData("-1.25", 1, "-1.3")]
    [InlineData("-1.36", 1, "-1.4")]
    public void Round_Digits1AwayFromZero_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.AwayFromZero, expected);
    }

    [Theory]
    [InlineData("1.234375", 3, "1.234")]
    [InlineData("1.2359375", 3, "1.236")]
    [InlineData("-1.234375", 3, "-1.234")]
    [InlineData("-1.2359375", 3, "-1.236")]
    public void Round_Digits3AwayFromZero_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.AwayFromZero, expected);
    }

    [Theory]
    [InlineData("1.26", 1, "1.2")]
    [InlineData("1.36", 1, "1.3")]
    [InlineData("-1.26", 1, "-1.2")]
    [InlineData("-1.36", 1, "-1.3")]
    public void Round_Digits1ToZero_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToZero, expected);
    }

    [Theory]
    [InlineData("1.26", 1, "1.2")]
    [InlineData("1.36", 1, "1.3")]
    [InlineData("-1.21", 1, "-1.3")]
    [InlineData("-1.36", 1, "-1.4")]
    public void Round_Digits1ToNegativeInfinity_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToNegativeInfinity, expected);
    }

    [Theory]
    [InlineData("1.21", 1, "1.3")]
    [InlineData("1.36", 1, "1.4")]
    [InlineData("-1.26", 1, "-1.2")]
    [InlineData("-1.36", 1, "-1.3")]
    public void Round_Digits1ToPositiveInfinity_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToPositiveInfinity, expected);
    }

    [Theory]
    [InlineData("0.000122", 3, "0")]
    [InlineData("0.000488", 3, "0")]
    [InlineData("0.000976", 3, "0.001")]
    [InlineData("-0.000122", 3, "0")]
    [InlineData("-0.000976", 3, "-0.001")]
    public void Round_VerySmallFractionsToEven_ReturnsExpectedResult(string input, int digits, string expected)
    {
        PerformRoundingTest(input, digits, MidpointRounding.ToEven, expected);
    }

    [Fact]
    public void Round_PiAcrossDigitCounts_ReturnsExpectedResult()
    {
        var pi = ChiFixed.Pi;
        PerformRoundingTestFromChiFixed(pi, 0, MidpointRounding.ToEven, "3");
        PerformRoundingTestFromChiFixed(pi, 1, MidpointRounding.ToEven, "3.1");
        PerformRoundingTestFromChiFixed(pi, 2, MidpointRounding.ToEven, "3.14");
        PerformRoundingTestFromChiFixed(pi, 3, MidpointRounding.ToEven, "3.142");
        PerformRoundingTestFromChiFixed(pi, 4, MidpointRounding.ToEven, "3.1416");
    }

    private static void PerformRoundingTestFromChiFixed(ChiFixed input, int digits, MidpointRounding mode,
        string expected)
    {
        var expectedDecimal = decimal.Parse(expected, CultureInfo.InvariantCulture);
        var result = ChiFixed.Round(input, digits, mode);
        var resultDecimal = ToDecimal(result);
        var difference = Math.Abs(expectedDecimal - resultDecimal);
        (difference < Tolerance).Should().BeTrue(
            $"Input: {ToDecimal(input)}, Mode: {mode} | Expected: {expectedDecimal}, Got: {resultDecimal}, Diff: {difference}");
    }

    private static decimal ToDecimal(ChiFixed value)
    {
        if (ChiFixed.IsNaN(value)) return decimal.Zero;
        if (ChiFixed.IsPositiveInfinity(value)) return decimal.MaxValue;
        if (ChiFixed.IsNegativeInfinity(value)) return decimal.MinValue;
        return (decimal)value;
    }

    private static void PerformRoundingTest(string input, int digits, MidpointRounding mode, string expected)
    {
        var inputDecimal = decimal.Parse(input, CultureInfo.InvariantCulture);
        var expectedDecimal = decimal.Parse(expected, CultureInfo.InvariantCulture);
        var result = ChiFixed.Round((ChiFixed)inputDecimal, digits, mode);
        var resultDecimal = ToDecimal(result);
        var difference = Math.Abs(expectedDecimal - resultDecimal);
        (difference < Tolerance).Should().BeTrue(
            $"Input: {input}, Mode: {mode} | Expected: {expectedDecimal}, Got: {resultDecimal}, Diff: {difference}");
    }

    #region Round (no parameters) Tests

    [Theory]
    [InlineData("2.5", "3")]
    [InlineData("3.5", "4")]
    [InlineData("2.4", "2")]
    [InlineData("2.6", "3")]
    [InlineData("-2.5", "-3")]
    [InlineData("-3.5", "-4")]
    [InlineData("-2.4", "-2")]
    [InlineData("-2.6", "-3")]
    [InlineData("0", "0")]
    [InlineData("1", "1")]
    [InlineData("-1", "-1")]
    public void Round_NoParameters_RoundsHalfAwayFromZero(string input, string expected)
    {
        var inputChiFixed = (ChiFixed)decimal.Parse(input, CultureInfo.InvariantCulture);
        var expectedChiFixed = (ChiFixed)decimal.Parse(expected, CultureInfo.InvariantCulture);
        var result = ChiFixed.Round(inputChiFixed);
        result.Should().Be(expectedChiFixed);
    }

    #endregion

    #region Floor Tests

    [Theory]
    [InlineData("2.9", "2")]
    [InlineData("2.1", "2")]
    [InlineData("2.0", "2")]
    [InlineData("-2.1", "-3")]
    [InlineData("-2.9", "-3")]
    [InlineData("-2.0", "-2")]
    [InlineData("0", "0")]
    [InlineData("0.5", "0")]
    [InlineData("-0.5", "-1")]
    public void Floor_VariousInputs_ReturnsExpectedResult(string input, string expected)
    {
        var inputChiFixed = (ChiFixed)decimal.Parse(input, CultureInfo.InvariantCulture);
        var expectedChiFixed = (ChiFixed)decimal.Parse(expected, CultureInfo.InvariantCulture);
        var result = ChiFixed.Floor(inputChiFixed);
        result.Should().Be(expectedChiFixed);
    }

    #endregion

    #region Ceiling Tests

    [Theory]
    [InlineData("2.9", "3")]
    [InlineData("2.1", "3")]
    [InlineData("2.0", "2")]
    [InlineData("-2.1", "-2")]
    [InlineData("-2.9", "-2")]
    [InlineData("-2.0", "-2")]
    [InlineData("0", "0")]
    [InlineData("0.5", "1")]
    [InlineData("-0.5", "0")]
    public void Ceiling_VariousInputs_ReturnsExpectedResult(string input, string expected)
    {
        var inputChiFixed = (ChiFixed)decimal.Parse(input, CultureInfo.InvariantCulture);
        var expectedChiFixed = (ChiFixed)decimal.Parse(expected, CultureInfo.InvariantCulture);
        var result = ChiFixed.Ceiling(inputChiFixed);
        result.Should().Be(expectedChiFixed);
    }

    #endregion

    #region Truncate Tests

    [Theory]
    [InlineData("2.9", "2")]
    [InlineData("2.1", "2")]
    [InlineData("2.0", "2")]
    [InlineData("-2.1", "-2")]
    [InlineData("-2.9", "-2")]
    [InlineData("-2.0", "-2")]
    [InlineData("0", "0")]
    [InlineData("0.5", "0")]
    [InlineData("-0.5", "0")]
    public void Truncate_VariousInputs_ReturnsExpectedResult(string input, string expected)
    {
        var inputChiFixed = (ChiFixed)decimal.Parse(input, CultureInfo.InvariantCulture);
        var expectedChiFixed = (ChiFixed)decimal.Parse(expected, CultureInfo.InvariantCulture);
        var result = ChiFixed.Truncate(inputChiFixed);
        result.Should().Be(expectedChiFixed);
    }

    #endregion

    #region Lerp Tests

    [Fact]
    public void Lerp_AmountZero_ReturnsFirstValue()
    {
        var result = ChiFixed.Lerp((ChiFixed)10m, (ChiFixed)20m, ChiFixed.Zero);
        result.Should().Be((ChiFixed)10m);
    }

    [Fact]
    public void Lerp_AmountOne_ReturnsSecondValue()
    {
        var result = ChiFixed.Lerp((ChiFixed)10m, (ChiFixed)20m, ChiFixed.One);
        result.Should().Be((ChiFixed)20m);
    }

    [Fact]
    public void Lerp_AmountHalf_ReturnsMidpoint()
    {
        var result = ChiFixed.Lerp((ChiFixed)0m, (ChiFixed)100m, (ChiFixed)0.5m);
        result.Should().Be((ChiFixed)50m);
    }

    [Theory]
    [InlineData("0", "100", "0.25", "25")]
    [InlineData("0", "100", "0.75", "75")]
    [InlineData("-50", "50", "0.5", "0")]
    [InlineData("10", "20", "0.3", "13")]
    public void Lerp_VariousAmounts_ReturnsExpectedResult(string v1, string v2, string amount, string expected)
    {
        var value1 = (ChiFixed)decimal.Parse(v1, CultureInfo.InvariantCulture);
        var value2 = (ChiFixed)decimal.Parse(v2, CultureInfo.InvariantCulture);
        var amountChiFixed = (ChiFixed)decimal.Parse(amount, CultureInfo.InvariantCulture);
        var expectedChiFixed = (ChiFixed)decimal.Parse(expected, CultureInfo.InvariantCulture);
        var result = ChiFixed.Lerp(value1, value2, amountChiFixed);
        var difference = Math.Abs(ToDecimal(result) - ToDecimal(expectedChiFixed));
        (difference < Tolerance).Should().BeTrue($"Expected: {expected}, Got: {ToDecimal(result)}");
    }

    [Fact]
    public void Lerp_NegativeAmount_Extrapolates()
    {
        var result = ChiFixed.Lerp((ChiFixed)10m, (ChiFixed)20m, (ChiFixed)(-0.5m));
        result.Should().Be((ChiFixed)5m);
    }

    [Fact]
    public void Lerp_AmountGreaterThanOne_Extrapolates()
    {
        var result = ChiFixed.Lerp((ChiFixed)10m, (ChiFixed)20m, (ChiFixed)1.5m);
        result.Should().Be((ChiFixed)25m);
    }

    #endregion

    #region FusedMultiplyAdd Tests

    [Fact]
    public void FusedMultiplyAdd_SimpleValues_ReturnsExpectedResult()
    {
        var result = ChiFixed.FusedMultiplyAdd((ChiFixed)2m, (ChiFixed)3m, (ChiFixed)4m);
        result.Should().Be((ChiFixed)10m);
    }

    [Fact]
    public void FusedMultiplyAdd_WithZeroAddend_ReturnsProduct()
    {
        var result = ChiFixed.FusedMultiplyAdd((ChiFixed)5m, (ChiFixed)7m, ChiFixed.Zero);
        result.Should().Be((ChiFixed)35m);
    }

    [Fact]
    public void FusedMultiplyAdd_WithZeroMultiplier_ReturnsAddend()
    {
        var result = ChiFixed.FusedMultiplyAdd(ChiFixed.Zero, (ChiFixed)100m, (ChiFixed)42m);
        result.Should().Be((ChiFixed)42m);
    }

    [Fact]
    public void FusedMultiplyAdd_NegativeValues_ReturnsExpectedResult()
    {
        var result = ChiFixed.FusedMultiplyAdd((ChiFixed)(-2m), (ChiFixed)3m, (ChiFixed)10m);
        result.Should().Be((ChiFixed)4m);
    }

    [Theory]
    [InlineData("1.5", "2.5", "0.25", "4")]
    [InlineData("0.1", "0.2", "0.3", "0.32")]
    public void FusedMultiplyAdd_FractionalValues_ReturnsExpectedResult(string left, string right, string addend,
        string expected)
    {
        var leftChiFixed = (ChiFixed)decimal.Parse(left, CultureInfo.InvariantCulture);
        var rightChiFixed = (ChiFixed)decimal.Parse(right, CultureInfo.InvariantCulture);
        var addendChiFixed = (ChiFixed)decimal.Parse(addend, CultureInfo.InvariantCulture);
        var expectedChiFixed = (ChiFixed)decimal.Parse(expected, CultureInfo.InvariantCulture);
        var result = ChiFixed.FusedMultiplyAdd(leftChiFixed, rightChiFixed, addendChiFixed);
        var difference = Math.Abs(ToDecimal(result) - ToDecimal(expectedChiFixed));
        (difference < Tolerance).Should().BeTrue($"Expected: {expected}, Got: {ToDecimal(result)}");
    }

    #endregion

    #region Ieee754Remainder Tests

    [Fact]
    public void Ieee754Remainder_ExactDivision_ReturnsZero()
    {
        var result = ChiFixed.Ieee754Remainder((ChiFixed)10m, (ChiFixed)5m);
        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Ieee754Remainder_DivisorZero_ThrowsDivideByZeroException()
    {
        var act = () => ChiFixed.Ieee754Remainder((ChiFixed)10m, ChiFixed.Zero);
        act.Should().Throw<DivideByZeroException>();
    }

    [Theory]
    [InlineData("7", "3", "1")]
    [InlineData("8", "3", "-1")]
    [InlineData("10", "4", "2")]
    [InlineData("11", "4", "-1")]
    public void Ieee754Remainder_VariousValues_ReturnsExpectedResult(string left, string right, string expected)
    {
        var leftChiFixed = (ChiFixed)decimal.Parse(left, CultureInfo.InvariantCulture);
        var rightChiFixed = (ChiFixed)decimal.Parse(right, CultureInfo.InvariantCulture);
        var expectedChiFixed = (ChiFixed)decimal.Parse(expected, CultureInfo.InvariantCulture);
        var result = ChiFixed.Ieee754Remainder(leftChiFixed, rightChiFixed);
        var difference = Math.Abs(ToDecimal(result) - ToDecimal(expectedChiFixed));
        (difference < Tolerance).Should().BeTrue($"Expected: {expected}, Got: {ToDecimal(result)}");
    }

    [Fact]
    public void Ieee754Remainder_NegativeLeft_ReturnsExpectedResult()
    {
        var result = ChiFixed.Ieee754Remainder((ChiFixed)(-7m), (ChiFixed)3m);
        var difference = Math.Abs(ToDecimal(result) - -1m);
        (difference < Tolerance).Should().BeTrue($"Expected: -1, Got: {ToDecimal(result)}");
    }

    [Fact]
    public void Ieee754Remainder_MidpointCase_RoundsToEven()
    {
        var result = ChiFixed.Ieee754Remainder((ChiFixed)4.5m, (ChiFixed)3m);
        var difference = Math.Abs(ToDecimal(result) - -1.5m);
        (difference < Tolerance).Should().BeTrue($"Expected: -1.5, Got: {ToDecimal(result)}");
    }

    #endregion
}