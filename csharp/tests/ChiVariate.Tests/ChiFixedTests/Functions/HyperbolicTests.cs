using System.Globalization;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Functions;

public class HyperbolicTests
{
    private const decimal RelativeTolerance = 0.005m;
    private const decimal MinAbsoluteTolerance = 0.0001m;

    private static void AssertEqual(ChiFixed expected, ChiFixed actual)
    {
        var expectedDecimal = (decimal)expected;
        var actualDecimal = (decimal)actual;
        var difference = Math.Abs(expectedDecimal - actualDecimal);
        var tolerance = Math.Max(MinAbsoluteTolerance, Math.Abs(expectedDecimal) * RelativeTolerance);
        Assert.True(difference < tolerance,
            $"Expected: {expectedDecimal}, Got: {actualDecimal}, Diff: {difference}, RelErr: {(expectedDecimal != 0 ? difference / Math.Abs(expectedDecimal) : difference):P4}");
    }

    #region Sinh Tests

    [Fact]
    public void Sinh_Zero_ReturnsZero()
    {
        var result = ChiFixed.Sinh(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1", "1.175201194")]
    [InlineData("2", "3.626860408")]
    [InlineData("0.5", "0.521095305")]
    [InlineData("0.1", "0.100166750")]
    public void Sinh_PositiveValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sinh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-1", "-1.175201194")]
    [InlineData("-2", "-3.626860408")]
    [InlineData("-0.5", "-0.521095305")]
    public void Sinh_NegativeValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sinh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("3", "10.017874927")]
    [InlineData("4", "27.289917197")]
    public void Sinh_LargerValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sinh(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Cosh Tests

    [Fact]
    public void Cosh_Zero_ReturnsOne()
    {
        var result = ChiFixed.Cosh(ChiFixed.Zero);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("1", "1.543080635")]
    [InlineData("2", "3.762195691")]
    [InlineData("0.5", "1.127625965")]
    [InlineData("0.1", "1.005004168")]
    public void Cosh_PositiveValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cosh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-1", "1.543080635")]
    [InlineData("-2", "3.762195691")]
    [InlineData("-0.5", "1.127625965")]
    public void Cosh_NegativeValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cosh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("3", "10.067661996")]
    [InlineData("4", "27.308232836")]
    public void Cosh_LargerValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cosh(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Tanh Tests

    [Fact]
    public void Tanh_Zero_ReturnsZero()
    {
        var result = ChiFixed.Tanh(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1", "0.761594156")]
    [InlineData("2", "0.964027580")]
    [InlineData("0.5", "0.462117157")]
    [InlineData("0.1", "0.099667995")]
    public void Tanh_PositiveValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Tanh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-1", "-0.761594156")]
    [InlineData("-2", "-0.964027580")]
    [InlineData("-0.5", "-0.462117157")]
    public void Tanh_NegativeValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Tanh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("3", "0.995054754")]
    [InlineData("5", "0.999909204")]
    public void Tanh_LargerValues_ApproachesOne(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Tanh(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Asinh Tests

    [Fact]
    public void Asinh_Zero_ReturnsZero()
    {
        var result = ChiFixed.Asinh(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1", "0.881373587")]
    [InlineData("2", "1.443635475")]
    [InlineData("0.5", "0.481211825")]
    [InlineData("0.1", "0.099834078")]
    public void Asinh_PositiveValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Asinh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-1", "-0.881373587")]
    [InlineData("-2", "-1.443635475")]
    [InlineData("-0.5", "-0.481211825")]
    public void Asinh_NegativeValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Asinh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("10", "2.998222950")]
    [InlineData("100", "5.298342366")]
    public void Asinh_LargeValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Asinh(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Acosh Tests

    [Fact]
    public void Acosh_One_ReturnsZero()
    {
        var result = ChiFixed.Acosh(ChiFixed.One);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("2", "1.316957897")]
    [InlineData("3", "1.762747174")]
    [InlineData("1.5", "0.962423650")]
    public void Acosh_ValidValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Acosh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("10", "2.993222846")]
    [InlineData("100", "5.298292366")]
    public void Acosh_LargeValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Acosh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("0.5")]
    [InlineData("-1")]
    public void Acosh_ValueLessThanOne_ThrowsException(string inputStr)
    {
        var value = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        Assert.Throws<ArgumentException>(() => ChiFixed.Acosh(value));
    }

    #endregion

    #region Atanh Tests

    [Fact]
    public void Atanh_Zero_ReturnsZero()
    {
        var result = ChiFixed.Atanh(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("0.5", "0.549306144")]
    [InlineData("0.9", "1.472219490")]
    [InlineData("0.1", "0.100335348")]
    [InlineData("0.25", "0.255412812")]
    public void Atanh_ValidPositiveValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Atanh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-0.5", "-0.549306144")]
    [InlineData("-0.9", "-1.472219490")]
    [InlineData("-0.1", "-0.100335348")]
    public void Atanh_ValidNegativeValues_ReturnsCorrectValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Atanh(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1.5")]
    [InlineData("-1.5")]
    [InlineData("2")]
    public void Atanh_OutOfRange_ThrowsException(string inputStr)
    {
        var value = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        Assert.Throws<ArgumentException>(() => ChiFixed.Atanh(value));
    }

    #endregion

    #region Inverse Relationships

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("0.5")]
    [InlineData("-1")]
    [InlineData("-0.5")]
    public void Sinh_InverseOfAsinh_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var asinhResult = ChiFixed.Asinh(input);
        var sinhResult = ChiFixed.Sinh(asinhResult);
        AssertEqual(input, sinhResult);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("5")]
    public void Cosh_InverseOfAcosh_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var acoshResult = ChiFixed.Acosh(input);
        var coshResult = ChiFixed.Cosh(acoshResult);
        AssertEqual(input, coshResult);
    }

    [Theory]
    [InlineData("0.5")]
    [InlineData("0.9")]
    [InlineData("-0.5")]
    [InlineData("-0.9")]
    [InlineData("0.1")]
    public void Tanh_InverseOfAtanh_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var atanhResult = ChiFixed.Atanh(input);
        var tanhResult = ChiFixed.Tanh(atanhResult);
        AssertEqual(input, tanhResult);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("0.5")]
    [InlineData("-1")]
    public void Asinh_InverseOfSinh_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var sinhResult = ChiFixed.Sinh(input);
        var asinhResult = ChiFixed.Asinh(sinhResult);
        AssertEqual(input, asinhResult);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    public void Acosh_InverseOfCosh_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var coshResult = ChiFixed.Cosh(input);
        var acoshResult = ChiFixed.Acosh(coshResult);
        AssertEqual(input, acoshResult);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("-1")]
    public void Atanh_InverseOfTanh_ReturnsOriginalValue(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var tanhResult = ChiFixed.Tanh(input);
        var atanhResult = ChiFixed.Atanh(tanhResult);
        AssertEqual(input, atanhResult);
    }

    #endregion

    #region Mathematical Identities

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("0.5")]
    [InlineData("-1")]
    public void HyperbolicIdentity_CoshSquaredMinusSinhSquared_EqualsOne(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var sinh = ChiFixed.Sinh(input);
        var cosh = ChiFixed.Cosh(input);
        var identity = cosh * cosh - sinh * sinh;
        AssertEqual(ChiFixed.One, identity);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("0.5")]
    [InlineData("-1")]
    public void Tanh_VariousInputs_EqualsSinhOverCosh(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var tanh = ChiFixed.Tanh(input);
        var sinh = ChiFixed.Sinh(input);
        var cosh = ChiFixed.Cosh(input);
        var ratio = sinh / cosh;
        AssertEqual(tanh, ratio);
    }

    #endregion
}