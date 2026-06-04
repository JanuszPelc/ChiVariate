using System.Globalization;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Functions;

public class TrigonometricTests
{
    private const decimal RelativeTolerance = 0.001m;
    private const decimal MinAbsoluteTolerance = 0.001m;

    #region AcosPi Tests

    [Theory]
    [InlineData("0", "0.5")]
    [InlineData("1", "0")]
    [InlineData("-1", "1")]
    public void AcosPi_ValidInput_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.AcosPi(input);
        AssertEqual(expected, result);
    }

    #endregion

    private static void AssertEqual(ChiFixed expected, ChiFixed actual)
    {
        var expectedDecimal = (decimal)expected;
        var actualDecimal = (decimal)actual;
        var difference = Math.Abs(expectedDecimal - actualDecimal);
        var tolerance = Math.Max(MinAbsoluteTolerance, Math.Abs(expectedDecimal) * RelativeTolerance);
        (difference < tolerance).Should().BeTrue(
            $"Expected: {expectedDecimal}, Got: {actualDecimal}, Diff: {difference}, RelErr: {(expectedDecimal != 0 ? difference / Math.Abs(expectedDecimal) : difference):P4}");
    }

    #region Sin Tests

    [Fact]
    public void Sin_Zero_ReturnsZero()
    {
        var result = ChiFixed.Sin(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1.570796327", "1")]
    [InlineData("0.523598776", "0.5")]
    [InlineData("0.785398163", "0.707106781")]
    [InlineData("1.047197551", "0.866025404")]
    public void Sin_SpecialAngles_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sin(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-1.570796327", "-1")]
    [InlineData("-0.523598776", "-0.5")]
    public void Sin_NegativeAngles_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sin(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("3.141592654", "0")]
    [InlineData("6.283185307", "0")]
    public void Sin_Periodicity_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Sin(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Cos Tests

    [Fact]
    public void Cos_Zero_ReturnsOne()
    {
        var result = ChiFixed.Cos(ChiFixed.Zero);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("1.570796327", "0")]
    [InlineData("1.047197551", "0.5")]
    [InlineData("0.785398163", "0.707106781")]
    [InlineData("0.523598776", "0.866025404")]
    public void Cos_SpecialAngles_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cos(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("3.141592654", "-1")]
    [InlineData("6.283185307", "1")]
    public void Cos_Periodicity_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cos(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("-1.047197551", "0.5")]
    [InlineData("-0.785398163", "0.707106781")]
    public void Cos_NegativeAngles_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Cos(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Tan Tests

    [Fact]
    public void Tan_Zero_ReturnsZero()
    {
        var result = ChiFixed.Tan(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("0.785398163", "1")]
    [InlineData("0.463647609", "0.5")]
    [InlineData("-0.785398163", "-1")]
    public void Tan_SpecialAngles_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Tan(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("3.141592654", "0")]
    public void Tan_Periodicity_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Tan(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region Asin Tests

    [Fact]
    public void Asin_Zero_ReturnsZero()
    {
        var result = ChiFixed.Asin(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1", "1.570796327")]
    [InlineData("-1", "-1.570796327")]
    [InlineData("0.5", "0.523598776")]
    [InlineData("-0.5", "-0.523598776")]
    [InlineData("0.707106781", "0.785398163")]
    public void Asin_ValidInput_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Asin(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("1.1")]
    [InlineData("-1.1")]
    [InlineData("2")]
    public void Asin_OutOfRange_ThrowsArgumentException(string inputStr)
    {
        var value = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var act = () => ChiFixed.Asin(value);
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Acos Tests

    [Theory]
    [InlineData("0", "1.570796327")]
    [InlineData("1", "0")]
    [InlineData("-1", "3.141592654")]
    [InlineData("0.5", "1.047197551")]
    [InlineData("-0.5", "2.094395102")]
    public void Acos_ValidInput_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Acos(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("1.1")]
    [InlineData("-1.1")]
    [InlineData("2")]
    public void Acos_OutOfRange_ThrowsArgumentException(string inputStr)
    {
        var value = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var act = () => ChiFixed.Acos(value);
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Atan Tests

    [Fact]
    public void Atan_Zero_ReturnsZero()
    {
        var result = ChiFixed.Atan(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1", "0.785398163")]
    [InlineData("-1", "-0.785398163")]
    [InlineData("0.5", "0.463647609")]
    [InlineData("-0.5", "-0.463647609")]
    public void Atan_StandardInput_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Atan(input);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("10", "1.471127674")]
    [InlineData("-10", "-1.471127674")]
    [InlineData("100", "1.560796660")]
    public void Atan_LargeInput_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Atan(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region SinPi Tests

    [Fact]
    public void SinPi_Zero_ReturnsZero()
    {
        var result = ChiFixed.SinPi(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("0.5", "1")]
    [InlineData("1", "0")]
    [InlineData("0.25", "0.707106781")]
    [InlineData("-0.5", "-1")]
    public void SinPi_SpecialAngles_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.SinPi(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region CosPi Tests

    [Fact]
    public void CosPi_Zero_ReturnsOne()
    {
        var result = ChiFixed.CosPi(ChiFixed.Zero);
        AssertEqual(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("0.5", "0")]
    [InlineData("1", "-1")]
    [InlineData("2", "1")]
    [InlineData("0.25", "0.707106781")]
    public void CosPi_SpecialAngles_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.CosPi(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region TanPi Tests

    [Fact]
    public void TanPi_Zero_ReturnsZero()
    {
        var result = ChiFixed.TanPi(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("0.25", "1")]
    [InlineData("-0.25", "-1")]
    public void TanPi_SpecialAngles_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.TanPi(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region AsinPi Tests

    [Fact]
    public void AsinPi_Zero_ReturnsZero()
    {
        var result = ChiFixed.AsinPi(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1", "0.5")]
    [InlineData("-1", "-0.5")]
    [InlineData("0.5", "0.166666667")]
    public void AsinPi_ValidInput_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.AsinPi(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region AtanPi Tests

    [Fact]
    public void AtanPi_Zero_ReturnsZero()
    {
        var result = ChiFixed.AtanPi(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1", "0.25")]
    [InlineData("-1", "-0.25")]
    public void AtanPi_ValidInput_ReturnsExpectedResult(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.AtanPi(input);
        AssertEqual(expected, result);
    }

    #endregion

    #region SinCos Tests

    [Fact]
    public void SinCos_Zero_ReturnsZeroAndOne()
    {
        var (sin, cos) = ChiFixed.SinCos(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, sin);
        AssertEqual(ChiFixed.One, cos);
    }

    [Theory]
    [InlineData("0.785398163", "0.707106781", "0.707106781")]
    [InlineData("1.570796327", "1", "0")]
    public void SinCos_SpecialAngles_ReturnsExpectedResults(string inputStr, string expectedSinStr,
        string expectedCosStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expectedSin = (ChiFixed)decimal.Parse(expectedSinStr, CultureInfo.InvariantCulture);
        var expectedCos = (ChiFixed)decimal.Parse(expectedCosStr, CultureInfo.InvariantCulture);
        var (sin, cos) = ChiFixed.SinCos(input);
        AssertEqual(expectedSin, sin);
        AssertEqual(expectedCos, cos);
    }

    [Fact]
    public void SinCos_Default_ExposesNamedTupleElements()
    {
        var result = ChiFixed.SinCos(ChiFixed.One);
        _ = result.Sin;
        _ = result.Cos;
    }

    #endregion

    #region Large Angle Tests

    [Fact]
    public void Sin_LargeMultipleOfPi_ReturnsNearZero()
    {
        var largeAngle = ChiFixed.Pi * (ChiFixed)1000m;
        var result = ChiFixed.Sin(largeAngle);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Fact]
    public void Cos_LargeEvenMultipleOfPi_ReturnsNearOne()
    {
        var largeAngle = ChiFixed.Pi * (ChiFixed)1000m;
        var result = ChiFixed.Cos(largeAngle);
        AssertEqual(ChiFixed.One, result);
    }

    [Fact]
    public void Cos_LargeOddMultipleOfPi_ReturnsNearNegativeOne()
    {
        var largeAngle = ChiFixed.Pi * (ChiFixed)1001m;
        var result = ChiFixed.Cos(largeAngle);
        AssertEqual(ChiFixed.NegativeOne, result);
    }

    [Fact]
    public void Sin_LargeAngle_MatchesSmallAngle()
    {
        var smallAngle = (ChiFixed)1.23m;
        var largeAngle = smallAngle + ChiFixed.Tau * (ChiFixed)100m;
        var smallResult = ChiFixed.Sin(smallAngle);
        var largeResult = ChiFixed.Sin(largeAngle);
        AssertEqual(smallResult, largeResult);
    }

    [Fact]
    public void Cos_LargeAngle_MatchesSmallAngle()
    {
        var smallAngle = (ChiFixed)1.23m;
        var largeAngle = smallAngle + ChiFixed.Tau * (ChiFixed)100m;
        var smallResult = ChiFixed.Cos(smallAngle);
        var largeResult = ChiFixed.Cos(largeAngle);
        AssertEqual(smallResult, largeResult);
    }

    [Fact]
    public void Sin_VeryLargeArbitraryAngle_DoesNotThrow()
    {
        var veryLarge = (ChiFixed)10000m;
        var act = () => ChiFixed.Sin(veryLarge);
        act.Should().NotThrow();
    }

    [Fact]
    public void Sin_ExtremelyLargeAngle_DoesNotThrow()
    {
        var extremelyLarge = (ChiFixed)1000000m;
        var act = () => ChiFixed.Sin(extremelyLarge);
        act.Should().NotThrow();
    }

    [Fact]
    public void Cos_ExtremelyLargeAngle_DoesNotThrow()
    {
        var extremelyLarge = (ChiFixed)1000000m;
        var act = () => ChiFixed.Cos(extremelyLarge);
        act.Should().NotThrow();
    }

    #endregion

    #region Atan2 Tests

    [Fact]
    public void Atan2_BothZero_ReturnsZero()
    {
        var result = ChiFixed.Atan2(ChiFixed.Zero, ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1", "1", "0.785398163")]
    [InlineData("1", "0", "1.570796327")]
    [InlineData("0", "1", "0")]
    [InlineData("-1", "1", "-0.785398163")]
    [InlineData("1", "-1", "2.356194490")]
    [InlineData("-1", "-1", "-2.356194490")]
    [InlineData("-1", "0", "-1.570796327")]
    [InlineData("0", "-1", "3.141592654")]
    public void Atan2_AllQuadrants_ReturnsExpectedAngle(string yStr, string xStr, string expectedStr)
    {
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Atan2(y, x);
        AssertEqual(expected, result);
    }

    [Theory]
    [InlineData("3", "4", "0.643501109")]
    [InlineData("4", "3", "0.927295218")]
    [InlineData("5", "12", "0.394791120")]
    public void Atan2_PythagoreanTriples_ReturnsExpectedAngle(string yStr, string xStr, string expectedStr)
    {
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Atan2(y, x);
        AssertEqual(expected, result);
    }

    #endregion

    #region Atan2Pi Tests

    [Fact]
    public void Atan2Pi_BothZero_ReturnsZero()
    {
        var result = ChiFixed.Atan2Pi(ChiFixed.Zero, ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("1", "1", "0.25")]
    [InlineData("1", "0", "0.5")]
    [InlineData("0", "1", "0")]
    [InlineData("-1", "0", "-0.5")]
    [InlineData("0", "-1", "1")]
    public void Atan2Pi_CardinalDirections_ReturnsExpectedAngle(string yStr, string xStr, string expectedStr)
    {
        var y = (ChiFixed)decimal.Parse(yStr, CultureInfo.InvariantCulture);
        var x = (ChiFixed)decimal.Parse(xStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);
        var result = ChiFixed.Atan2Pi(y, x);
        AssertEqual(expected, result);
    }

    #endregion

    #region SinCosPi Tests

    [Fact]
    public void SinCosPi_Zero_ReturnsZeroAndOne()
    {
        var (sinPi, cosPi) = ChiFixed.SinCosPi(ChiFixed.Zero);
        AssertEqual(ChiFixed.Zero, sinPi);
        AssertEqual(ChiFixed.One, cosPi);
    }

    [Theory]
    [InlineData("0.25", "0.707106781", "0.707106781")]
    [InlineData("0.5", "1", "0")]
    [InlineData("1", "0", "-1")]
    public void SinCosPi_SpecialAngles_ReturnsExpectedResults(string inputStr, string expectedSinStr,
        string expectedCosStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expectedSin = (ChiFixed)decimal.Parse(expectedSinStr, CultureInfo.InvariantCulture);
        var expectedCos = (ChiFixed)decimal.Parse(expectedCosStr, CultureInfo.InvariantCulture);
        var (sinPi, cosPi) = ChiFixed.SinCosPi(input);
        AssertEqual(expectedSin, sinPi);
        AssertEqual(expectedCos, cosPi);
    }

    [Fact]
    public void SinCosPi_Default_ExposesNamedTupleElements()
    {
        var result = ChiFixed.SinCosPi(ChiFixed.One);
        _ = result.SinPi;
        _ = result.CosPi;
    }

    #endregion
}