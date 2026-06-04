using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Arithmetic;

public class ArithmeticOpsTests
{
    [Fact]
    public void Addition_ZeroPlusZero_ReturnsZero()
    {
        var result = ChiFixed.Zero + ChiFixed.Zero;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Addition_OnePlusOne_ReturnsTwo()
    {
        var result = ChiFixed.One + ChiFixed.One;

        result.Should().Be((ChiFixed)2m);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(10, 5, 15)]
    [InlineData(-5, 3, -2)]
    [InlineData(-10, -5, -15)]
    [InlineData(0.5, 0.25, 0.75)]
    public void Addition_VariousValues_ReturnsCorrectSum(decimal a, decimal b, decimal expected)
    {
        var result = (ChiFixed)a + (ChiFixed)b;

        result.Should().Be((ChiFixed)expected);
    }

    [Fact]
    public void Addition_Commutative_ProducesSameResult()
    {
        var a = (ChiFixed)3.14m;
        var b = (ChiFixed)2.71m;

        (b + a).Should().Be(a + b);
    }

    [Fact]
    public void Subtraction_ZeroMinusZero_ReturnsZero()
    {
        var result = ChiFixed.Zero - ChiFixed.Zero;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Subtraction_OneMinusOne_ReturnsZero()
    {
        var result = ChiFixed.One - ChiFixed.One;

        result.Should().Be(ChiFixed.Zero);
    }

    [Theory]
    [InlineData(5, 3, 2)]
    [InlineData(10, 15, -5)]
    [InlineData(-5, -3, -2)]
    [InlineData(0.75, 0.25, 0.5)]
    public void Subtraction_VariousValues_ReturnsCorrectDifference(decimal a, decimal b, decimal expected)
    {
        var result = (ChiFixed)a - (ChiFixed)b;

        result.Should().Be((ChiFixed)expected);
    }

    [Fact]
    public void Multiplication_ZeroTimesAnything_ReturnsZero()
    {
        var value = (ChiFixed)42m;

        var result = ChiFixed.Zero * value;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Multiplication_OneTimesValue_ReturnsValue()
    {
        var value = (ChiFixed)42.5m;

        var result = ChiFixed.One * value;

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(2, 3, 6)]
    [InlineData(5, 4, 20)]
    [InlineData(-3, 4, -12)]
    [InlineData(-2, -5, 10)]
    [InlineData(0.5, 2, 1)]
    [InlineData(0.25, 4, 1)]
    public void Multiplication_VariousValues_ReturnsCorrectProduct(decimal a, decimal b, decimal expected)
    {
        var result = (ChiFixed)a * (ChiFixed)b;

        result.Should().Be((ChiFixed)expected);
    }

    [Fact]
    public void Multiplication_Commutative_ProducesSameResult()
    {
        var a = (ChiFixed)3.5m;
        var b = (ChiFixed)2.5m;

        (b * a).Should().Be(a * b);
    }

    [Fact]
    public void Division_ValueByOne_ReturnsValue()
    {
        var value = (ChiFixed)42.5m;

        var result = value / ChiFixed.One;

        result.Should().Be(value);
    }

    [Theory]
    [InlineData(6, 2, 3)]
    [InlineData(10, 5, 2)]
    [InlineData(-12, 4, -3)]
    [InlineData(-20, -5, 4)]
    [InlineData(1, 2, 0.5)]
    [InlineData(1, 4, 0.25)]
    public void Division_VariousValues_ReturnsCorrectQuotient(decimal a, decimal b, decimal expected)
    {
        var result = (ChiFixed)a / (ChiFixed)b;

        result.Should().Be((ChiFixed)expected);
    }

    [Fact]
    public void Division_ByZero_ThrowsDivideByZeroException()
    {
        var act = () => ChiFixed.One / ChiFixed.Zero;

        act.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void Operators_ChainedExpression_ReturnsExpectedResult()
    {
        var a = (ChiFixed)10m;
        var b = (ChiFixed)5m;
        var c = (ChiFixed)2m;

        var result = (a + b) * c - (ChiFixed)5m;

        result.Should().Be((ChiFixed)25m);
    }

    [Fact]
    public void Multiplication_DistributesOverAddition_ProducesEqualResults()
    {
        var a = (ChiFixed)3m;
        var b = (ChiFixed)4m;
        var c = (ChiFixed)5m;

        var left = a * (b + c);
        var right = a * b + a * c;

        right.Should().Be(left);
    }

    [Fact]
    public void Modulo_ZeroModuloAnything_ReturnsZero()
    {
        var divisor = (ChiFixed)42m;

        var result = ChiFixed.Zero % divisor;

        result.Should().Be(ChiFixed.Zero);
    }

    [Fact]
    public void Modulo_ByZero_ThrowsDivideByZeroException()
    {
        var act = () => ChiFixed.One % ChiFixed.Zero;

        act.Should().Throw<DivideByZeroException>();
    }

    [Theory]
    [InlineData(5, 3, 2)]
    [InlineData(10, 3, 1)]
    [InlineData(7, 4, 3)]
    [InlineData(100, 7, 2)]
    public void Modulo_PositiveIntegers_ReturnsCorrectRemainder(decimal a, decimal b, decimal expected)
    {
        var result = (ChiFixed)a % (ChiFixed)b;

        result.Should().Be((ChiFixed)expected);
    }

    [Theory]
    [InlineData(5.5, 2, 1.5)]
    [InlineData(7.25, 2, 1.25)]
    [InlineData(10.5, 3, 1.5)]
    [InlineData(3.14, 1, 0.14)]
    public void Modulo_FractionalValues_ReturnsCorrectRemainder(decimal a, decimal b, decimal expected)
    {
        var result = (ChiFixed)a % (ChiFixed)b;

        result.Should().Be((ChiFixed)expected);
    }

    [Theory]
    [InlineData(-5, 3, -2)]
    [InlineData(5, -3, 2)]
    [InlineData(-5, -3, -2)]
    [InlineData(-5.5, 2, -1.5)]
    public void Modulo_NegativeValues_FollowsCSharpSemantics(decimal a, decimal b, decimal expected)
    {
        var result = (ChiFixed)a % (ChiFixed)b;

        result.Should().Be((ChiFixed)expected);
    }

    [Fact]
    public void Modulo_PositiveValues_RemainderSmallerThanDivisor()
    {
        var a = (ChiFixed)17.3m;
        var b = (ChiFixed)5m;

        var result = a % b;

        (result >= ChiFixed.Zero).Should().BeTrue();
        (result < b).Should().BeTrue();
    }

    [Fact]
    public void Modulo_OnCommonCases_MatchesDecimalRemainder()
    {
        var testCases = new[]
        {
            (a: 10.0m, b: 3.0m),
            (a: 7.5m, b: 2.5m),
            (a: -8.5m, b: 3.0m),
            (a: 5.25m, b: 1.5m)
        };

        foreach (var (a, b) in testCases)
        {
            var fixedResult = (ChiFixed)a % (ChiFixed)b;
            var expectedResult = a % b;

            fixedResult.Should().Be((ChiFixed)expectedResult);
        }
    }

    [Fact]
    public void Modulo_WithTruncatedQuotient_ReconstructsDividend()
    {
        var a = (ChiFixed)17.5m;
        var b = (ChiFixed)5m;

        var remainder = a % b;
        var quotient = a / b;
        var quotientTruncated = new ChiFixed(quotient.Raw / ChiFixed.ScaleFactor * ChiFixed.ScaleFactor);

        var reconstructed = quotientTruncated * b + remainder;

        reconstructed.Should().Be(a);
    }
}