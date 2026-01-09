using System.Numerics;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Arithmetic;

public class UnaryOperatorsTests
{
    private static T Increment<T>(T value) where T : IIncrementOperators<T>
    {
        return ++value;
    }

    private static T Decrement<T>(T value) where T : IDecrementOperators<T>
    {
        return --value;
    }

    [Fact]
    public void Negation_Zero_ReturnsZero()
    {
        var result = -ChiFixed.Zero;

        Assert.Equal(ChiFixed.Zero, result);
    }

    [Fact]
    public void Negation_PositiveValue_ReturnsNegative()
    {
        var positive = (ChiFixed)42m;

        var result = -positive;

        Assert.Equal((ChiFixed)(-42m), result);
    }

    [Fact]
    public void Negation_NegativeValue_ReturnsPositive()
    {
        var negative = (ChiFixed)(-42m);

        var result = -negative;

        Assert.Equal((ChiFixed)42m, result);
    }

    [Fact]
    public void Negation_DoubleNegation_ReturnsOriginalValue()
    {
        var value = (ChiFixed)3.14m;

        var result = - -value;

        Assert.Equal(value, result);
    }

    [Fact]
    public void UnaryPlus_Value_PreservesValue()
    {
        var value = (ChiFixed)42.5m;

        var result = +value;

        Assert.Equal(value, result);
    }

    [Fact]
    public void Increment_FromZero_ReturnsOne()
    {
        var result = Increment(ChiFixed.Zero);
        Assert.Equal(ChiFixed.One, result);
    }

    [Theory]
    [InlineData("10.5", "11.5")]
    [InlineData("-5.5", "-4.5")]
    [InlineData("0.9", "1.9")]
    public void Increment_FromVariousValues_ReturnsCorrectValue(string input, string expected)
    {
        var value = ChiFixed.Parse(input);
        var expectedValue = ChiFixed.Parse(expected);
        var result = Increment(value);
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Decrement_FromOne_ReturnsZero()
    {
        var result = Decrement(ChiFixed.One);
        Assert.Equal(ChiFixed.Zero, result);
    }

    [Theory]
    [InlineData("10.5", "9.5")]
    [InlineData("-5.5", "-6.5")]
    [InlineData("0.1", "-0.9")]
    public void Decrement_FromVariousValues_ReturnsCorrectValue(string input, string expected)
    {
        var value = ChiFixed.Parse(input);
        var expectedValue = ChiFixed.Parse(expected);
        var result = Decrement(value);
        Assert.Equal(expectedValue, result);
    }
}