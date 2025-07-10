using AwesomeAssertions;
using Xunit;

namespace ChiVariate.Tests.ChiMathTests;

#pragma warning disable CS1591
public class ChiMathLogTests
{
    #region Float Tests

    [Theory]
    [InlineData(1.0f, 0.0f)] // Log(1) = 0 (optimized path)
    [InlineData(2.71828f, 1.0f)] // Log(e) ≈ 1 (optimized path)
    [InlineData(2.0f, 0.693147f)] // Log(2) = ln(2) (optimized path)
    [InlineData(10.0f, 2.302585f)] // Log(10) = ln(10) (optimized path)
    [InlineData(7.389056f, 2.0f)] // Log(e²) = 2
    [InlineData(0.5f, -0.693147f)] // Log(1/2) = -ln(2)
    public void Log_Float_BasicCases_ShouldReturnExpectedResults(float input, float expected)
    {
        // Act
        var result = ChiMath.Log(input);

        // Assert
        result.Should().BeApproximately(expected, 1e-5f);
    }

    #endregion

    #region Optimized Path Tests

    [Fact]
    public void Log_OptimizedPaths_ShouldBeExact()
    {
        // Arrange & Act & Assert
        ChiMath.Log(1.0).Should().Be(0.0);
        ChiMath.Log(1.0f).Should().Be(0.0f);
        ChiMath.Log(1m).Should().Be(0m);

        ChiMath.Log(Math.E).Should().Be(1.0);
        ChiMath.Log((float)Math.E).Should().BeApproximately(1.0f, 1e-6f);
        ChiMath.Log(ChiMath.Const<decimal>.E).Should().BeApproximately(1m, 1e-25m);

        var expectedLn2Double = Math.Log(2.0);
        ChiMath.Log(2.0).Should().Be(expectedLn2Double);
        ChiMath.Log(2.0f).Should().BeApproximately((float)expectedLn2Double, 1e-6f);
        ChiMath.Log(2m).Should().BeApproximately(ChiMath.Const<decimal>.Ln2, 1e-25m);

        var expectedLn10Double = Math.Log(10.0);
        ChiMath.Log(10.0).Should().Be(expectedLn10Double);
        ChiMath.Log(10.0f).Should().BeApproximately((float)expectedLn10Double, 1e-6f);
        ChiMath.Log(10m).Should().BeApproximately(ChiMath.Const<decimal>.Ln10, 1e-25m);
    }

    #endregion

    #region Consistency Tests

    [Theory]
    [InlineData(1.0)]
    [InlineData(2.0)]
    [InlineData(10.0)]
    [InlineData(0.5)]
    [InlineData(100.0)]
    public void Log_CrossTypeConsistency_ShouldBeConsistentAcrossTypes(double value)
    {
        // Arrange & Act
        var doubleResult = ChiMath.Log(value);
        var floatResult = ChiMath.Log((float)value);
        var decimalResult = ChiMath.Log((decimal)value);

        // Assert
        ((double)decimalResult).Should().BeApproximately(doubleResult, 1e-12,
            $"Decimal Log({value}) should be consistent with double version");

        floatResult.Should().BeApproximately((float)doubleResult, 1e-5f,
            $"Float Log({value}) should be consistent with double version");
    }

    #endregion

    #region Double Tests

    [Theory]
    [InlineData(1.0, 0.0)] // Log(1) = 0 (optimized path)
    [InlineData(2.718281828459045, 1.0)] // Log(e) = 1 (optimized path)
    [InlineData(2.0, 0.6931471805599453)] // Log(2) = ln(2) (optimized path)
    [InlineData(10.0, 2.302585092994046)] // Log(10) = ln(10) (optimized path)
    [InlineData(7.38905609893065, 2.0)] // Log(e²) = 2
    [InlineData(0.36787944117144233, -1.0)] // Log(1/e) = -1
    [InlineData(0.5, -0.6931471805599453)] // Log(1/2) = -ln(2)
    [InlineData(100.0, 4.605170185988092)] // Log(100) = 2*ln(10)
    public void Log_Double_BasicCases_ShouldReturnExpectedResults(double input, double expected)
    {
        // Act
        var result = ChiMath.Log(input);

        // Assert
        result.Should().BeApproximately(expected, 1e-14);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-10.0)]
    public void Log_Double_NonPositiveValues_ShouldThrowArgumentException(double input)
    {
        // Act & Assert
        var act = () => ChiMath.Log(input);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Logarithm undefined for non-positive values.");
    }

    #endregion

    #region Decimal Tests

    [Theory]
    [InlineData("1", "0")] // Log(1) = 0 (optimized path)
    [InlineData("2.71828182845904523536028747135266249775724709369995957496697", "1")] // Log(e) = 1 (optimized path)
    [InlineData("2",
        "0.693147180559945309417232121458176568075500134360255254120680")] // Log(2) = ln(2) (optimized path) - use our actual constant
    [InlineData("10",
        "2.302585092994045684017991454684364207601101488628772976033")] // Log(10) = ln(10) (optimized path) - use our actual constant
    [InlineData("0.5", "-0.693147180559945309417232121458176568075500134360255254120680")] // Log(1/2) = -ln(2)
    [InlineData("100", "4.605170185988091368035982909368728415202202977257545952066")] // Log(100) = 2*ln(10)
    [InlineData("4", "1.386294361119890618834464242916353136151000268720510508241360")] // Log(4) = 2*ln(2)
    public void Log_Decimal_BasicCases_ShouldReturnExpectedResults(string inputStr, string expectedStr)
    {
        // Arrange
        var input = decimal.Parse(inputStr);
        var expected = decimal.Parse(expectedStr);

        // Act
        var result = ChiMath.Log(input);

        // Assert
        result.Should().BeApproximately(expected, 1e-27m);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-10")]
    [InlineData("-0.5")]
    public void Log_Decimal_NonPositiveValues_ShouldThrowArgumentException(string inputStr)
    {
        // Arrange
        var input = decimal.Parse(inputStr);

        // Act & Assert
        var act = () => ChiMath.Log(input);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Logarithm undefined for non-positive values.");
    }

    [Theory]
    [InlineData("1.5")]
    [InlineData("3")]
    [InlineData("5")]
    [InlineData("7")]
    [InlineData("20")]
    [InlineData("50")]
    [InlineData("0.1")]
    [InlineData("0.25")]
    [InlineData("0.75")]
    public void Log_Decimal_ArbitraryValues_ShouldHaveCorrectProperties(string inputStr)
    {
        // Arrange
        var input = decimal.Parse(inputStr);

        // Act
        var result = ChiMath.Log(input);

        // Assert
        if (input > 1m)
            result.Should().BePositive($"Log({input}) should be positive when input > 1");
        else if (input is < 1m and > 0m)
            result.Should().BeNegative($"Log({input}) should be negative when 0 < input < 1");

        var roundTrip = ChiMath.Exp(result);
        roundTrip.Should().BeApproximately(input, 1e-25m,
            $"exp(log({input})) should equal {input}, but got {roundTrip}");
    }

    [Theory]
    [InlineData("0.0000000000000000000000001")]
    [InlineData("999999999999999999999999")]
    public void Log_Decimal_ExtremeValues_ShouldNotThrowUnexpectedly(string inputStr)
    {
        // Arrange
        var input = decimal.Parse(inputStr);

        // Act & Assert
        var act = () => ChiMath.Log(input);
        act.Should().NotThrow<OverflowException>();
    }

    #endregion

    #region Mathematical Properties Tests

    [Theory]
    [InlineData("2", "3")] // Log(2*3) = Log(2) + Log(3)
    [InlineData("5", "7")] // Log(5*7) = Log(5) + Log(7)
    [InlineData("1.5", "2.5")] // Log(1.5*2.5) = Log(1.5) + Log(2.5)
    public void Log_ProductRule_ShouldSatisfyLogProperty(string aStr, string bStr)
    {
        // Arrange
        var a = decimal.Parse(aStr);
        var b = decimal.Parse(bStr);
        var product = a * b;

        // Act
        var logProduct = ChiMath.Log(product);
        var logA = ChiMath.Log(a);
        var logB = ChiMath.Log(b);
        var sumOfLogs = logA + logB;

        // Assert
        logProduct.Should().BeApproximately(sumOfLogs, 1e-25m,
            $"Log({a}*{b}) should equal Log({a}) + Log({b})");
    }

    [Theory]
    [InlineData("8", "2")] // Log(8/2) = Log(8) - Log(2)
    [InlineData("15", "3")] // Log(15/3) = Log(15) - Log(3)
    [InlineData("7.5", "2.5")] // Log(7.5/2.5) = Log(7.5) - Log(2.5)
    public void Log_QuotientRule_ShouldSatisfyLogProperty(string aStr, string bStr)
    {
        // Arrange
        var a = decimal.Parse(aStr);
        var b = decimal.Parse(bStr);
        var quotient = a / b;

        // Act
        var logQuotient = ChiMath.Log(quotient);
        var logA = ChiMath.Log(a);
        var logB = ChiMath.Log(b);
        var differenceOfLogs = logA - logB;

        // Assert
        logQuotient.Should().BeApproximately(differenceOfLogs, 1e-25m,
            $"Log({a}/{b}) should equal Log({a}) - Log({b})");
    }

    [Theory]
    [InlineData("2", "3")] // Log(2³) = 3*Log(2)
    [InlineData("5", "2")] // Log(5²) = 2*Log(5)
    [InlineData("1.5", "4")] // Log(1.5⁴) = 4*Log(1.5)
    public void Log_PowerRule_ShouldSatisfyLogProperty(string baseStr, string exponentStr)
    {
        // Arrange
        var baseVal = decimal.Parse(baseStr);
        var exponent = decimal.Parse(exponentStr);
        var power = ChiMath.Pow(baseVal, exponent);

        // Act
        var logPower = ChiMath.Log(power);
        var logBase = ChiMath.Log(baseVal);
        var scaledLog = exponent * logBase;

        // Assert
        logPower.Should().BeApproximately(scaledLog, 1e-20m,
            $"Log({baseVal}^{exponent}) should equal {exponent}*Log({baseVal})");
    }

    #endregion

    #region Edge Cases

    [Theory]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NaN)]
    public void Log_Double_SpecialValues_ShouldHandleGracefully(double input)
    {
        // Act & Assert
        var act = () => ChiMath.Log(input);
        act.Should().NotThrow<ArgumentException>();
    }

    [Theory]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NaN)]
    public void Log_Float_SpecialValues_ShouldHandleGracefully(float input)
    {
        // Act & Assert
        var act = () => ChiMath.Log(input);
        act.Should().NotThrow<ArgumentException>();
    }

    #endregion
}