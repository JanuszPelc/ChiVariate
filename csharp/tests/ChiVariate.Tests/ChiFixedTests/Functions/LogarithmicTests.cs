using System.Globalization;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Functions;

public class LogarithmicTests
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

    #region Log (Natural Logarithm) Tests

    [Fact]
    public void Log_One_ReturnsZero()
    {
        var result = ChiFixed.Log(ChiFixed.One);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Fact]
    public void Log_E_ReturnsOne()
    {
        var result = ChiFixed.Log(ChiFixed.E);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("2", "0.693147181")]
    [InlineData("3", "1.098612289")]
    [InlineData("10", "2.302585093")]
    [InlineData("0.5", "-0.693147181")]
    public void Log_PositiveValues_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("7.389056099", "2")]
    [InlineData("20.085536923", "3")]
    [InlineData("0.367879441", "-1")]
    public void Log_PowersOfE_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.1", "-2.302585093")]
    [InlineData("0.01", "-4.605170186")]
    public void Log_SmallValues_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("100", "4.605170186")]
    [InlineData("1000", "6.907755279")]
    public void Log_LargeValues_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-10")]
    public void Log_NonPositiveValue_ThrowsArgumentException(string inputStr)
    {
        var value = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var act = () => ChiFixed.Log(value);
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Log2 Tests

    [Fact]
    public void Log2_One_ReturnsZero()
    {
        var result = ChiFixed.Log2(ChiFixed.One);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("2", "1")]
    [InlineData("4", "2")]
    [InlineData("8", "3")]
    [InlineData("16", "4")]
    [InlineData("32", "5")]
    [InlineData("1024", "10")]
    public void Log2_PowersOfTwo_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log2(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.5", "-1")]
    [InlineData("0.25", "-2")]
    [InlineData("0.125", "-3")]
    [InlineData("0.0625", "-4")]
    public void Log2_NegativePowersOfTwo_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log2(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("3", "1.584962501")]
    [InlineData("5", "2.321928095")]
    [InlineData("10", "3.321928095")]
    public void Log2_NonPowersOfTwo_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log2(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("1.414213562", "0.5")]
    [InlineData("2.828427125", "1.5")]
    public void Log2_FractionalResults_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log2(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-10")]
    public void Log2_NonPositiveValue_ThrowsArgumentException(string inputStr)
    {
        var value = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var act = () => ChiFixed.Log2(value);
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Log10 Tests

    [Fact]
    public void Log10_One_ReturnsZero()
    {
        var result = ChiFixed.Log10(ChiFixed.One);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("10", "1")]
    [InlineData("100", "2")]
    [InlineData("1000", "3")]
    [InlineData("10000", "4")]
    public void Log10_PowersOfTen_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log10(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.1", "-1")]
    [InlineData("0.01", "-2")]
    [InlineData("0.001", "-3")]
    [InlineData("0.0001", "-4")]
    public void Log10_NegativePowersOfTen_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log10(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("2", "0.301029996")]
    [InlineData("5", "0.698970004")]
    [InlineData("50", "1.698970004")]
    public void Log10_NonPowersOfTen_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log10(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("3.162277660", "0.5")]
    [InlineData("31.622776602", "1.5")]
    public void Log10_FractionalResults_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log10(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-10")]
    public void Log10_NonPositiveValue_ThrowsArgumentException(string inputStr)
    {
        var value = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var act = () => ChiFixed.Log10(value);
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Log(x, base) Tests

    [Theory]
    [InlineData("9", "3", "2")]
    [InlineData("27", "3", "3")]
    [InlineData("25", "5", "2")]
    [InlineData("125", "5", "3")]
    public void Log_CustomBase_ReturnsExpectedResult(string inputStr, string baseStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var baseValue = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log(input, baseValue);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("8", "2", "3")]
    [InlineData("100", "10", "2")]
    public void Log_CustomBase_MatchesSpecializedMethods(string inputStr, string baseStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var baseValue = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log(input, baseValue);
        AssertEqual(expected, result);

        if (baseStr == "2")
        {
            var log2Result = ChiFixed.Log2(input);
            AssertEqual(log2Result, result);
        }
        else if (baseStr == "10")
        {
            var log10Result = ChiFixed.Log10(input);
            AssertEqual(log10Result, result);
        }
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    public void Log_NonPositiveBase_ThrowsArgumentException(string baseStr)
    {
        var input = (ChiFixed)10m;
        var baseValue = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var act = () => ChiFixed.Log(input, baseValue);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Log_BaseOne_ThrowsArgumentException()
    {
        var input = (ChiFixed)10m;
        var baseValue = ChiFixed.One;
        var act = () => ChiFixed.Log(input, baseValue);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("0", "2")]
    [InlineData("-1", "2")]
    [InlineData("-10", "10")]
    public void Log_NonPositiveInput_ThrowsArgumentException(string inputStr, string baseStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var baseValue = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var act = () => ChiFixed.Log(input, baseValue);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("16", "4", "2")]
    [InlineData("81", "3", "4")]
    public void Log_PerfectPowerBase_ReturnsExpectedResult(string inputStr, string baseStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var baseValue = (ChiFixed)decimal.Parse(baseStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Log(input, baseValue);
        AssertEqual(expected, result);
    }

    #endregion

    #region Inverse Relationships

    [Theory]
    [InlineData("2")]
    [InlineData("5")]
    [InlineData("10")]
    public void Log_InverseOfExp_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expResult = ChiFixed.Exp(input);
        var logResult = ChiFixed.Log(expResult);
        AssertEqual(input, logResult);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("5")]
    [InlineData("10")]
    public void Log2_InverseOfExp2_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var exp2Result = ChiFixed.Exp2(input);
        var log2Result = ChiFixed.Log2(exp2Result);
        AssertEqual(input, log2Result);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("3")]
    public void Log10_InverseOfExp10_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var exp10Result = ChiFixed.Exp10(input);
        var log10Result = ChiFixed.Log10(exp10Result);
        AssertEqual(input, log10Result);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("5")]
    [InlineData("10")]
    public void Exp_InverseOfLog_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var logResult = ChiFixed.Log(input);
        var expResult = ChiFixed.Exp(logResult);
        AssertEqual(input, expResult);
    }

    [Fact]
    public void Log_ChangeOfBase_MatchesCustomBase()
    {
        var x = (ChiFixed)27m;
        var baseValue = (ChiFixed)3m;

        var logWithBase = ChiFixed.Log(x, baseValue);
        var logChangeOfBase = ChiFixed.Log(x) / ChiFixed.Log(baseValue);

        AssertEqual(logWithBase, logChangeOfBase);
    }

    #endregion

    #region ILogB Tests

    [Fact]
    public void ILogB_Zero_ReturnsIntMinValue()
    {
        var result = ChiFixed.ILogB(ChiFixed.Zero);
        result.Should().Be(int.MinValue);
    }

    [Fact]
    public void ILogB_One_ReturnsZero()
    {
        var result = ChiFixed.ILogB(ChiFixed.One);
        result.Should().Be(0);
    }

    [Theory]
    [InlineData("2", 1)]
    [InlineData("4", 2)]
    [InlineData("8", 3)]
    [InlineData("16", 4)]
    [InlineData("1024", 10)]
    public void ILogB_PowersOfTwo_ReturnsExponent(string inputStr, int expected)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.ILogB(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("0.5", -1)]
    [InlineData("0.25", -2)]
    [InlineData("0.125", -3)]
    public void ILogB_FractionalPowersOfTwo_ReturnsNegativeExponent(string inputStr, int expected)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.ILogB(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("3", 1)]
    [InlineData("5", 2)]
    [InlineData("7", 2)]
    [InlineData("15", 3)]
    public void ILogB_NonPowersOfTwo_ReturnsFlooredExponent(string inputStr, int expected)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.ILogB(input);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("-1", 0)]
    [InlineData("-2", 1)]
    [InlineData("-4", 2)]
    [InlineData("-0.5", -1)]
    public void ILogB_NegativeValues_ReturnsExponentOfAbsoluteValue(string inputStr, int expected)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.ILogB(input);
        result.Should().Be(expected);
    }

    #endregion
}