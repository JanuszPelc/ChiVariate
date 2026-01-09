using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiFixedTests.Arithmetic;

public class ComparisonOpsTests
{
    [Fact]
    public void Equality_SameValue_ReturnsTrue()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void Equality_DifferentValues_ReturnsFalse()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)43m;

        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void Equality_ZeroAndZero_ReturnsTrue()
    {
        Assert.True(ChiFixed.Zero == (ChiFixed)0m);
        Assert.False(ChiFixed.Zero != (ChiFixed)0m);
    }

    [Fact]
    public void Equality_Reflexive_ValueEqualsItself()
    {
        var value = (ChiFixed)3.14m;

        Assert.True(value == (ChiFixed)3.14m);
    }

    [Fact]
    public void Equality_Symmetric_OrderDoesNotMatter()
    {
        var a = (ChiFixed)2.5m;
        var b = (ChiFixed)2.5m;

        Assert.Equal(a == b, b == a);
    }

    [Fact]
    public void Equality_Transitive_ChainedEquality()
    {
        var a = (ChiFixed)5m;
        var b = (ChiFixed)5m;
        var c = (ChiFixed)5m;

        Assert.True(a == b);
        Assert.True(b == c);
        Assert.True(a == c);
    }

    [Fact]
    public void GreaterThan_LargerValue_ReturnsTrue()
    {
        var larger = (ChiFixed)10m;
        var smaller = (ChiFixed)5m;

        Assert.True(larger > smaller);
        Assert.False(smaller > larger);
    }

    [Fact]
    public void GreaterThan_EqualValues_ReturnsFalse()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        Assert.False(a > b);
    }

    [Theory]
    [InlineData(10, 5)]
    [InlineData(0, -1)]
    [InlineData(0.5, 0.25)]
    [InlineData(100, 99.99)]
    public void GreaterThan_VariousValues_ReturnsTrue(decimal larger, decimal smaller)
    {
        Assert.True((ChiFixed)larger > (ChiFixed)smaller);
    }

    [Fact]
    public void LessThan_SmallerValue_ReturnsTrue()
    {
        var smaller = (ChiFixed)5m;
        var larger = (ChiFixed)10m;

        Assert.True(smaller < larger);
        Assert.False(larger < smaller);
    }

    [Fact]
    public void LessThan_EqualValues_ReturnsFalse()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        Assert.False(a < b);
    }

    [Theory]
    [InlineData(5, 10)]
    [InlineData(-1, 0)]
    [InlineData(0.25, 0.5)]
    [InlineData(99.99, 100)]
    public void LessThan_VariousValues_ReturnsTrue(decimal smaller, decimal larger)
    {
        Assert.True((ChiFixed)smaller < (ChiFixed)larger);
    }

    [Fact]
    public void GreaterThanOrEqual_LargerValue_ReturnsTrue()
    {
        var larger = (ChiFixed)10m;
        var smaller = (ChiFixed)5m;

        Assert.True(larger >= smaller);
    }

    [Fact]
    public void GreaterThanOrEqual_EqualValues_ReturnsTrue()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        Assert.True(a >= b);
    }

    [Fact]
    public void GreaterThanOrEqual_SmallerValue_ReturnsFalse()
    {
        var smaller = (ChiFixed)5m;
        var larger = (ChiFixed)10m;

        Assert.False(smaller >= larger);
    }

    [Fact]
    public void LessThanOrEqual_SmallerValue_ReturnsTrue()
    {
        var smaller = (ChiFixed)5m;
        var larger = (ChiFixed)10m;

        Assert.True(smaller <= larger);
    }

    [Fact]
    public void LessThanOrEqual_EqualValues_ReturnsTrue()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        Assert.True(a <= b);
    }

    [Fact]
    public void LessThanOrEqual_LargerValue_ReturnsFalse()
    {
        var larger = (ChiFixed)10m;
        var smaller = (ChiFixed)5m;

        Assert.False(larger <= smaller);
    }

    [Fact]
    public void CompareTo_SmallerValue_ReturnsNegative()
    {
        var smaller = (ChiFixed)5m;
        var larger = (ChiFixed)10m;

        Assert.True(smaller.CompareTo(larger) < 0);
    }

    [Fact]
    public void CompareTo_LargerValue_ReturnsPositive()
    {
        var larger = (ChiFixed)10m;
        var smaller = (ChiFixed)5m;

        Assert.True(larger.CompareTo(smaller) > 0);
    }

    [Fact]
    public void CompareTo_EqualValue_ReturnsZero()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        Assert.Equal(0, a.CompareTo(b));
    }

    [Fact]
    public void Comparison_NegativeAndPositive_CorrectOrdering()
    {
        var negative = (ChiFixed)(-5m);
        var positive = (ChiFixed)5m;

        Assert.True(negative < positive);
        Assert.True(positive > negative);
        Assert.False(negative >= positive);
        Assert.False(positive <= negative);
    }

    [Fact]
    public void Comparison_NegativeValues_CorrectOrdering()
    {
        var moreNegative = (ChiFixed)(-10m);
        var lessNegative = (ChiFixed)(-5m);

        Assert.True(moreNegative < lessNegative);
        Assert.True(lessNegative > moreNegative);
    }

    [Fact]
    public void Comparison_ZeroWithPositiveAndNegative_CorrectOrdering()
    {
        var negative = (ChiFixed)(-1m);
        var positive = (ChiFixed)1m;

        Assert.True(negative < ChiFixed.Zero);
        Assert.True(ChiFixed.Zero > negative);
        Assert.True(positive > ChiFixed.Zero);
        Assert.True(ChiFixed.Zero < positive);
    }

    [Fact]
    public void Transitivity_LessThanChain_Holds()
    {
        var a = (ChiFixed)1m;
        var b = (ChiFixed)2m;
        var c = (ChiFixed)3m;

        Assert.True(a < b);
        Assert.True(b < c);
        Assert.True(a < c);
    }

    [Fact]
    public void Antisymmetry_EqualValues_HoldsTrue()
    {
        var a = (ChiFixed)5m;
        var b = (ChiFixed)5m;
        var c = (ChiFixed)10m;

        Assert.True(a <= b && b <= a);
        Assert.True(a == b);
        Assert.False(a <= c && c <= a);
    }
}