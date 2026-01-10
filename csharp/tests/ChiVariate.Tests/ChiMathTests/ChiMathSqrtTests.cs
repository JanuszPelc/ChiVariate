using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiMathTests;

#pragma warning disable CS1591
public class ChiMathSqrtTests
{
    #region Float Tests

    [Theory]
    [InlineData(4.0f, 2.0f)]
    [InlineData(9.0f, 3.0f)]
    [InlineData(16.0f, 4.0f)]
    public void Sqrt_FloatPerfectSquares_ReturnsExactResults(float input, float expected)
    {
        var result = ChiMath.Sqrt(input);

        result.Should().BeApproximately(expected, 1e-5f);
    }

    #endregion

    #region Double Tests

    [Theory]
    [InlineData(4.0, 2.0)]
    [InlineData(9.0, 3.0)]
    [InlineData(16.0, 4.0)]
    [InlineData(25.0, 5.0)]
    [InlineData(100.0, 10.0)]
    [InlineData(0.25, 0.5)]
    [InlineData(0.04, 0.2)]
    public void Sqrt_DoublePerfectSquares_ReturnsExactResults(double input, double expected)
    {
        var result = ChiMath.Sqrt(input);

        result.Should().BeApproximately(expected, 1e-10);
    }

    [Theory]
    [InlineData(2.0, 1.414213562373095)]
    [InlineData(3.0, 1.732050807568877)]
    [InlineData(5.0, 2.236067977499790)]
    [InlineData(10.0, 3.162277660168380)]
    public void Sqrt_DoubleIrrational_ReturnsAccurateApproximations(double input, double expected)
    {
        var result = ChiMath.Sqrt(input);

        result.Should().BeApproximately(expected, 1e-10);
    }

    [Fact]
    public void Sqrt_DoubleZero_ReturnsZero()
    {
        var result = ChiMath.Sqrt(0.0);

        result.Should().Be(0.0);
    }

    [Fact]
    public void Sqrt_DoubleNegative_ThrowsOverflowException()
    {
        var act = () => ChiMath.Sqrt(-1.0);

        act.Should().Throw<OverflowException>();
    }

    #endregion

    #region Decimal Tests

    [Theory]
    [InlineData("4", "2")]
    [InlineData("9", "3")]
    [InlineData("16", "4")]
    [InlineData("25", "5")]
    [InlineData("100", "10")]
    [InlineData("0.25", "0.5")]
    public void Sqrt_DecimalPerfectSquares_ReturnsExactResults(string inputStr, string expectedStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Sqrt(input);

        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("0.5")]
    public void Sqrt_DecimalIrrational_SquaredEqualsInput(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Sqrt(input);

        var verification = result * result;
        verification.Should().BeApproximately(input, 1e-8m);
    }

    [Fact]
    public void Sqrt_DecimalZero_ReturnsZero()
    {
        var result = ChiMath.Sqrt(0m);

        result.Should().Be(0m);
    }

    [Fact]
    public void Sqrt_DecimalNegative_ThrowsOverflowException()
    {
        var act = () => ChiMath.Sqrt(-1m);

        act.Should().Throw<OverflowException>();
    }

    [Theory]
    [InlineData("0.000000000000001")]
    [InlineData("999999999999999999999999")]
    public void Sqrt_DecimalExtremeValues_DoesNotThrow(string inputStr)
    {
        var input = decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var act = () => ChiMath.Sqrt(input);

        act.Should().NotThrow();
    }

    [Fact]
    public void Sqrt_DecimalVeryLarge_Converges()
    {
        var input = decimal.MaxValue / 1000;

        var act = () => ChiMath.Sqrt(input);

        act.Should().NotThrow<TimeoutException>();
    }

    #endregion

    #region ChiFixed Tests

    [Theory]
    [InlineData("4", "2")]
    [InlineData("9", "3")]
    [InlineData("16", "4")]
    [InlineData("25", "5")]
    [InlineData("100", "10")]
    [InlineData("0.25", "0.5")]
    public void Sqrt_ChiFixedPerfectSquares_ReturnsExactResults(string inputStr, string expectedStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);
        var expected = (ChiFixed)decimal.Parse(expectedStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Sqrt(input);

        ((decimal)result).Should().BeApproximately((decimal)expected, 1e-10m);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("0.5")]
    public void Sqrt_ChiFixedIrrational_SquaredEqualsInput(string inputStr)
    {
        var input = (ChiFixed)decimal.Parse(inputStr, CultureInfo.InvariantCulture);

        var result = ChiMath.Sqrt(input);

        var verification = result * result;
        ((decimal)verification).Should().BeApproximately((decimal)input, 1e-8m);
    }

    [Fact]
    public void Sqrt_ChiFixedZero_ReturnsZero()
    {
        var result = ChiMath.Sqrt(ChiFixed.Zero);

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Sqrt_ChiFixedNegative_ThrowsOverflowException()
    {
        var negativeValue = (ChiFixed)(-1m);

        var act = () => ChiMath.Sqrt(negativeValue);

        act.Should().Throw<OverflowException>();
    }

    #endregion

    #region Convergence Tests

    [Theory]
    [InlineData(1000000.0)]
    [InlineData(0.000001)]
    [InlineData(1.0)]
    public void Sqrt_DoubleAnyValue_Converges(double input)
    {
        var result = ChiMath.Sqrt(input);

        var expected = Math.Sqrt(input);
        result.Should().BeApproximately(expected, ChiMath.Const<double>.Epsilon * 10);
    }

    [Fact]
    public void Sqrt_AllTypes_UsesTypeSpecificPrecision()
    {
        var doubleResult = ChiMath.Sqrt(2.0);
        var floatResult = ChiMath.Sqrt(2.0f);
        var decimalResult = ChiMath.Sqrt(2m);
        var fixedResult = ChiMath.Sqrt((ChiFixed)2m);

        doubleResult.Should().BeApproximately(Math.Sqrt(2.0), 1e-14);
        floatResult.Should().BeApproximately((float)Math.Sqrt(2.0), 1e-6f);

        var decimalVerification = decimalResult * decimalResult;
        decimalVerification.Should().BeApproximately(2m, 1e-10m);

        var fixedVerification = fixedResult * fixedResult;
        ((decimal)fixedVerification).Should().BeApproximately(2m, 1e-8m);
    }

    #endregion
}