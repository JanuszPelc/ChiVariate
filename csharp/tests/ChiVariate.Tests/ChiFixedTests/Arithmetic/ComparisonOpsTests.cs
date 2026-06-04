using AwesomeAssertions;
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

        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
    }

    [Fact]
    public void Equality_DifferentValues_ReturnsFalse()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)43m;

        (a == b).Should().BeFalse();
        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Equality_ZeroAndZero_ReturnsTrue()
    {
        (ChiFixed.Zero == (ChiFixed)0m).Should().BeTrue();
        (ChiFixed.Zero != (ChiFixed)0m).Should().BeFalse();
    }

    [Fact]
    public void Equality_Reflexive_ReturnsTrue()
    {
        var value = (ChiFixed)3.14m;

        (value == (ChiFixed)3.14m).Should().BeTrue();
    }

    [Fact]
    public void Equality_Symmetric_ReturnsSameResultRegardlessOfOrder()
    {
        var a = (ChiFixed)2.5m;
        var b = (ChiFixed)2.5m;

        (b == a).Should().Be(a == b);
    }

    [Fact]
    public void Equality_Transitive_ReturnsTrue()
    {
        var a = (ChiFixed)5m;
        var b = (ChiFixed)5m;
        var c = (ChiFixed)5m;

        (a == b).Should().BeTrue();
        (b == c).Should().BeTrue();
        (a == c).Should().BeTrue();
    }

    [Fact]
    public void GreaterThan_LargerValue_ReturnsTrue()
    {
        var larger = (ChiFixed)10m;
        var smaller = (ChiFixed)5m;

        (larger > smaller).Should().BeTrue();
        (smaller > larger).Should().BeFalse();
    }

    [Fact]
    public void GreaterThan_EqualValues_ReturnsFalse()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        (a > b).Should().BeFalse();
    }

    [Theory]
    [InlineData(10, 5)]
    [InlineData(0, -1)]
    [InlineData(0.5, 0.25)]
    [InlineData(100, 99.99)]
    public void GreaterThan_VariousValues_ReturnsTrue(decimal larger, decimal smaller)
    {
        ((ChiFixed)larger > (ChiFixed)smaller).Should().BeTrue();
    }

    [Fact]
    public void LessThan_SmallerValue_ReturnsTrue()
    {
        var smaller = (ChiFixed)5m;
        var larger = (ChiFixed)10m;

        (smaller < larger).Should().BeTrue();
        (larger < smaller).Should().BeFalse();
    }

    [Fact]
    public void LessThan_EqualValues_ReturnsFalse()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        (a < b).Should().BeFalse();
    }

    [Theory]
    [InlineData(5, 10)]
    [InlineData(-1, 0)]
    [InlineData(0.25, 0.5)]
    [InlineData(99.99, 100)]
    public void LessThan_VariousValues_ReturnsTrue(decimal smaller, decimal larger)
    {
        ((ChiFixed)smaller < (ChiFixed)larger).Should().BeTrue();
    }

    [Fact]
    public void GreaterThanOrEqual_LargerValue_ReturnsTrue()
    {
        var larger = (ChiFixed)10m;
        var smaller = (ChiFixed)5m;

        (larger >= smaller).Should().BeTrue();
    }

    [Fact]
    public void GreaterThanOrEqual_EqualValues_ReturnsTrue()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        (a >= b).Should().BeTrue();
    }

    [Fact]
    public void GreaterThanOrEqual_SmallerValue_ReturnsFalse()
    {
        var smaller = (ChiFixed)5m;
        var larger = (ChiFixed)10m;

        (smaller >= larger).Should().BeFalse();
    }

    [Fact]
    public void LessThanOrEqual_SmallerValue_ReturnsTrue()
    {
        var smaller = (ChiFixed)5m;
        var larger = (ChiFixed)10m;

        (smaller <= larger).Should().BeTrue();
    }

    [Fact]
    public void LessThanOrEqual_EqualValues_ReturnsTrue()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        (a <= b).Should().BeTrue();
    }

    [Fact]
    public void LessThanOrEqual_LargerValue_ReturnsFalse()
    {
        var larger = (ChiFixed)10m;
        var smaller = (ChiFixed)5m;

        (larger <= smaller).Should().BeFalse();
    }

    [Fact]
    public void CompareTo_SmallerValue_ReturnsNegative()
    {
        var smaller = (ChiFixed)5m;
        var larger = (ChiFixed)10m;

        (smaller.CompareTo(larger) < 0).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_LargerValue_ReturnsPositive()
    {
        var larger = (ChiFixed)10m;
        var smaller = (ChiFixed)5m;

        (larger.CompareTo(smaller) > 0).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_EqualValue_ReturnsZero()
    {
        var a = (ChiFixed)42m;
        var b = (ChiFixed)42m;

        a.CompareTo(b).Should().Be(0);
    }

    [Fact]
    public void Comparison_NegativeAndPositive_OrdersByValue()
    {
        var negative = (ChiFixed)(-5m);
        var positive = (ChiFixed)5m;

        (negative < positive).Should().BeTrue();
        (positive > negative).Should().BeTrue();
        (negative >= positive).Should().BeFalse();
        (positive <= negative).Should().BeFalse();
    }

    [Fact]
    public void Comparison_NegativeValues_OrdersByValue()
    {
        var moreNegative = (ChiFixed)(-10m);
        var lessNegative = (ChiFixed)(-5m);

        (moreNegative < lessNegative).Should().BeTrue();
        (lessNegative > moreNegative).Should().BeTrue();
    }

    [Fact]
    public void Comparison_ZeroWithPositiveAndNegative_OrdersByValue()
    {
        var negative = (ChiFixed)(-1m);
        var positive = (ChiFixed)1m;

        (negative < ChiFixed.Zero).Should().BeTrue();
        (ChiFixed.Zero > negative).Should().BeTrue();
        (positive > ChiFixed.Zero).Should().BeTrue();
        (ChiFixed.Zero < positive).Should().BeTrue();
    }

    [Fact]
    public void LessThan_Transitive_ReturnsTrue()
    {
        var a = (ChiFixed)1m;
        var b = (ChiFixed)2m;
        var c = (ChiFixed)3m;

        (a < b).Should().BeTrue();
        (b < c).Should().BeTrue();
        (a < c).Should().BeTrue();
    }

    [Fact]
    public void LessThanOrEqual_Antisymmetric_ReturnsTrueBothDirections()
    {
        var a = (ChiFixed)5m;
        var b = (ChiFixed)5m;
        var c = (ChiFixed)10m;

        (a <= b && b <= a).Should().BeTrue();
        (a == b).Should().BeTrue();
        (a <= c && c <= a).Should().BeFalse();
    }
}