using System.Globalization;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Functions;

public class RootTests
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

    #region Sqrt Tests

    [Fact]
    public void Sqrt_Zero_ReturnsZero()
    {
        var result = ChiFixed.Sqrt(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Fact]
    public void Sqrt_One_ReturnsOne()
    {
        var result = ChiFixed.Sqrt(ChiFixed.One);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("4", "2")]
    [InlineData("9", "3")]
    [InlineData("16", "4")]
    [InlineData("25", "5")]
    [InlineData("100", "10")]
    [InlineData("144", "12")]
    public void Sqrt_PerfectSquares_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sqrt(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("2", "1.414213562")]
    [InlineData("3", "1.732050808")]
    [InlineData("5", "2.236067977")]
    [InlineData("10", "3.162277660")]
    public void Sqrt_NonPerfectSquares_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sqrt(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.25", "0.5")]
    [InlineData("0.01", "0.1")]
    [InlineData("0.04", "0.2")]
    [InlineData("1.44", "1.2")]
    public void Sqrt_DecimalValues_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sqrt(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.0001", "0.01")]
    [InlineData("0.000001", "0.001")]
    public void Sqrt_VerySmallValues_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sqrt(input);
        AssertEqual(expected, result);
    }

    [Fact]
    public void Sqrt_NegativeValue_ThrowsArgumentException()
    {
        var value = (ChiFixed)(-4.0m);
        var act = () => ChiFixed.Sqrt(value);
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Cbrt Tests

    [Fact]
    public void Cbrt_Zero_ReturnsZero()
    {
        var result = ChiFixed.Cbrt(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Fact]
    public void Cbrt_One_ReturnsOne()
    {
        var result = ChiFixed.Cbrt(ChiFixed.One);
        AssertEqual(ChiFixed.One, result);
    }

    [Fact]
    public void Cbrt_NegativeOne_ReturnsNegativeOne()
    {
        var result = ChiFixed.Cbrt(ChiFixed.NegativeOne);
        AssertEqual(ChiFixed.NegativeOne, result);
    }

    [Theory]
    [InlineData("8", "2")]
    [InlineData("27", "3")]
    [InlineData("64", "4")]
    [InlineData("125", "5")]
    [InlineData("1000", "10")]
    public void Cbrt_PerfectCubes_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cbrt(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-8", "-2")]
    [InlineData("-27", "-3")]
    [InlineData("-64", "-4")]
    [InlineData("-125", "-5")]
    public void Cbrt_NegativePerfectCubes_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cbrt(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("2", "1.259921050")]
    [InlineData("10", "2.154434690")]
    [InlineData("100", "4.641588834")]
    public void Cbrt_NonPerfectCubes_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cbrt(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.125", "0.5")]
    [InlineData("0.001", "0.1")]
    [InlineData("0.008", "0.2")]
    public void Cbrt_DecimalValues_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cbrt(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Hypot Tests

    [Theory]
    [InlineData("3", "4", "5")]
    [InlineData("5", "12", "13")]
    [InlineData("8", "15", "17")]
    [InlineData("7", "24", "25")]
    public void Hypot_PythagoreanTriples_ReturnsExpectedResult(string xStr, string yStr, string expectedStr)
    {
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Hypot(x, y);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0", "5", "5")]
    [InlineData("5", "0", "5")]
    [InlineData("0", "0", "0")]
    public void Hypot_WithZero_ReturnsExpectedResult(string xStr, string yStr, string expectedStr)
    {
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Hypot(x, y);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("1", "1", "1.414213562")]
    [InlineData("3", "3", "4.242640687")]
    [InlineData("5", "5", "7.071067812")]
    public void Hypot_EqualValues_ReturnsExpectedResult(string xStr, string yStr, string expectedStr)
    {
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Hypot(x, y);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.3", "0.4", "0.5")]
    [InlineData("1.5", "2.0", "2.5")]
    public void Hypot_DecimalValues_ReturnsExpectedResult(string xStr, string yStr, string expectedStr)
    {
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Hypot(x, y);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-3", "4", "5")]
    [InlineData("3", "-4", "5")]
    [InlineData("-3", "-4", "5")]
    public void Hypot_NegativeValues_ReturnsExpectedResult(string xStr, string yStr, string expectedStr)
    {
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Hypot(x, y);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("10000", "10000", "14142.135623730950488")]
    public void Hypot_LargeValues_ReturnsExpectedResult(string xStr, string yStr, string expectedStr)
    {
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Hypot(x, y);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("100000", "100000", 3)]
    [InlineData("1000000", "1000000", 30)]
    public void Hypot_VeryLargeValues_StaysWithinUlpTolerance(string xStr, string yStr, long maxUlps)
    {
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var expected = ChiFixed.Sqrt((ChiFixed)2) * x;
        var result = ChiFixed.Hypot(x, y);

        var ulpDiff = Math.Abs(expected.Raw - result.Raw);
        (ulpDiff <= maxUlps).Should().BeTrue($"Expected <= {maxUlps} ULPs, got {ulpDiff}");
    }

    [Theory]
    [InlineData("1600000000", "1600000000")]
    [InlineData("2000000000", "2000000000")]
    public void Hypot_VeryLargeValues_SaturatesToMaxValue(string xStr, string yStr)
    {
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Hypot(x, y);

        result.Should().Be(ChiFixed.MaxValue, $"Expected saturation to MaxValue, got {(decimal)result}");
    }

    #endregion

    #region RootN Tests

    [Fact]
    public void RootN_N0_ThrowsArgumentException()
    {
        var value = (ChiFixed)16.0m;
        var act = () => ChiFixed.RootN(value, 0);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("5", "5")]
    [InlineData("123.456", "123.456")]
    [InlineData("-42", "-42")]
    public void RootN_N1_ReturnsSameValue(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.RootN(input, 1);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("4", 2, "2")]
    [InlineData("9", 2, "3")]
    [InlineData("16", 2, "4")]
    public void RootN_N2_MatchesSqrt(string inputStr, int n, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.RootN(input, n);
        var sqrtResult = ChiFixed.Sqrt(input);
        AssertEqual(expected, result);
        AssertEqual(sqrtResult, result);
    }

    [Theory]
    [InlineData("8", 3, "2")]
    [InlineData("27", 3, "3")]
    [InlineData("64", 3, "4")]
    public void RootN_N3_MatchesCbrt(string inputStr, int n, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.RootN(input, n);
        var cbrtResult = ChiFixed.Cbrt(input);
        AssertEqual(expected, result);
        AssertEqual(cbrtResult, result);
    }

    [Theory]
    [InlineData("16", 4, "2")]
    [InlineData("81", 4, "3")]
    [InlineData("32", 5, "2")]
    [InlineData("243", 5, "3")]
    public void RootN_HigherRoots_ReturnsExpectedResult(string inputStr, int n, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.RootN(input, n);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("2", 4, "1.189207115")]
    [InlineData("10", 5, "1.584893192")]
    public void RootN_NonPerfectRoots_ReturnsExpectedResult(string inputStr, int n, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.RootN(input, n);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("0.0625", 4, "0.5")]
    [InlineData("0.00032", 5, "0.2")]
    public void RootN_DecimalValues_ReturnsExpectedResult(string inputStr, int n, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.RootN(input, n);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-32", 5, "-2")]
    [InlineData("-243", 5, "-3")]
    public void RootN_NegativeValueOddRoot_ReturnsExpectedResult(string inputStr, int n, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.RootN(input, n);
        AssertEqual(expected, result);
    }

    [Fact]
    public void RootN_NegativeValueEvenRoot_ThrowsArgumentException()
    {
        var value = (ChiFixed)(-16.0m);
        var act = () => ChiFixed.RootN(value, 4);
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region ReciprocalEstimate Tests

    [Fact]
    public void ReciprocalEstimate_One_ReturnsOne()
    {
        var result = ChiFixed.ReciprocalEstimate(ChiFixed.One);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("2", "0.5")]
    [InlineData("4", "0.25")]
    [InlineData("5", "0.2")]
    [InlineData("10", "0.1")]
    [InlineData("0.5", "2")]
    [InlineData("0.25", "4")]
    public void ReciprocalEstimate_VariousValues_ReturnsReciprocal(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.ReciprocalEstimate(input);
        AssertEqual(expected, result);
    }

    [Fact]
    public void ReciprocalEstimate_NegativeValue_ReturnsNegativeReciprocal()
    {
        var result = ChiFixed.ReciprocalEstimate((ChiFixed)(-2m));
        AssertEqual((ChiFixed)(-0.5m), result);
    }

    [Fact]
    public void ReciprocalEstimate_Zero_ThrowsDivideByZeroException()
    {
        var act = () => ChiFixed.ReciprocalEstimate(ChiFixed.Zero);
        act.Should().Throw<DivideByZeroException>();
    }

    #endregion

    #region ReciprocalSqrtEstimate Tests

    [Fact]
    public void ReciprocalSqrtEstimate_One_ReturnsOne()
    {
        var result = ChiFixed.ReciprocalSqrtEstimate(ChiFixed.One);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("4", "0.5")]
    [InlineData("9", "0.333333333")]
    [InlineData("16", "0.25")]
    [InlineData("25", "0.2")]
    [InlineData("100", "0.1")]
    public void ReciprocalSqrtEstimate_PerfectSquares_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.ReciprocalSqrtEstimate(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("2", "0.707106781")]
    [InlineData("0.25", "2")]
    public void ReciprocalSqrtEstimate_NonPerfectSquares_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.ReciprocalSqrtEstimate(input);
        AssertEqual(expected, result);
    }

    #endregion
}