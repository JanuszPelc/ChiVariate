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
    public void Sqrt_Float_PerfectSquares_ShouldReturnExactResults(float input, float expected)
    {
        // Act
        var result = ChiMath.Sqrt(input);

        // Assert
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
    public void Sqrt_Double_PerfectSquares_ShouldReturnExactResults(double input, double expected)
    {
        // Act
        var result = ChiMath.Sqrt(input);

        // Assert
        result.Should().BeApproximately(expected, 1e-10);
    }

    [Theory]
    [InlineData(2.0, 1.414213562373095)]
    [InlineData(3.0, 1.732050807568877)]
    [InlineData(5.0, 2.236067977499790)]
    [InlineData(10.0, 3.162277660168380)]
    public void Sqrt_Double_IrrationalResults_ShouldReturnAccurateApproximations(double input, double expected)
    {
        // Act
        var result = ChiMath.Sqrt(input);

        // Assert
        result.Should().BeApproximately(expected, 1e-10);
    }

    [Fact]
    public void Sqrt_Double_Zero_ShouldReturnZero()
    {
        // Act
        var result = ChiMath.Sqrt(0.0);

        // Assert
        result.Should().Be(0.0);
    }

    [Fact]
    public void Sqrt_Double_NegativeNumber_ShouldThrowOverflowException()
    {
        // Act & Assert
        var act = () => ChiMath.Sqrt(-1.0);
        act.Should().Throw<OverflowException>()
            .WithMessage("Cannot calculate square root of a negative number.");
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
    public void Sqrt_Decimal_PerfectSquares_ShouldReturnExactResults(string inputStr, string expectedStr)
    {
        // Arrange
        var input = decimal.Parse(inputStr);
        var expected = decimal.Parse(expectedStr);

        // Act
        var result = ChiMath.Sqrt(input);

        // Assert
        result.Should().BeApproximately(expected, 1e-10m);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("0.5")]
    public void Sqrt_Decimal_IrrationalResults_ShouldConverge(string inputStr)
    {
        // Arrange
        var input = decimal.Parse(inputStr);

        // Act
        var result = ChiMath.Sqrt(input);

        // Assert
        // Verify it's actually a square root by squaring the result
        var verification = result * result;
        verification.Should().BeApproximately(input, 1e-8m);
    }

    [Fact]
    public void Sqrt_Decimal_Zero_ShouldReturnZero()
    {
        // Act
        var result = ChiMath.Sqrt(0m);

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void Sqrt_Decimal_NegativeNumber_ShouldThrowOverflowException()
    {
        // Act & Assert
        var act = () => ChiMath.Sqrt(-1m);
        act.Should().Throw<OverflowException>()
            .WithMessage("Cannot calculate square root of a negative number.");
    }

    [Theory]
    [InlineData("0.000000000000001")]
    [InlineData("999999999999999999999999")]
    public void Sqrt_Decimal_ExtremeValues_ShouldNotThrowUnexpectedly(string inputStr)
    {
        // Arrange
        var input = decimal.Parse(inputStr);

        // Act & Assert
        var act = () => ChiMath.Sqrt(input);
        act.Should().NotThrow();
    }

    [Fact]
    public void Sqrt_Decimal_VeryLargeNumber_ShouldNotExceedIterationLimit()
    {
        // Arrange
        var input = decimal.MaxValue / 1000; // Large but manageable

        // Act & Assert
        var act = () => ChiMath.Sqrt(input);
        act.Should().NotThrow<TimeoutException>(); // Ensures iteration limit works
    }

    #endregion

    #region Convergence Tests

    [Theory]
    [InlineData(1000000.0)]
    [InlineData(0.000001)]
    [InlineData(1.0)]
    public void Sqrt_Double_ShouldConvergeWithinIterationLimit(double input)
    {
        // Act
        var result = ChiMath.Sqrt(input);

        // Assert
        var expected = Math.Sqrt(input);
        result.Should().BeApproximately(expected, ChiMath.Const<double>.Epsilon * 10);
    }

    [Fact]
    public void Sqrt_AllTypes_WithDefaultEpsilon_ShouldUseTypeSpecificPrecision()
    {
        // Arrange & Act
        var doubleResult = ChiMath.Sqrt(2.0);
        var floatResult = ChiMath.Sqrt(2.0f);
        var decimalResult = ChiMath.Sqrt(2m);

        // Assert
        doubleResult.Should().BeApproximately(Math.Sqrt(2.0), 1e-14);
        floatResult.Should().BeApproximately((float)Math.Sqrt(2.0), 1e-6f);

        // For decimal, verify by squaring
        var decimalVerification = decimalResult * decimalResult;
        decimalVerification.Should().BeApproximately(2m, 1e-10m);
    }

    #endregion
}