using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiMathTests;

public class ChiMathLerpTests
{
    #region Float Tests

    [Theory]
    [InlineData(0.0f, 100.0f, 0, 10, 0.0f)]
    [InlineData(0.0f, 100.0f, 5, 10, 50.0f)]
    [InlineData(0.0f, 100.0f, 10, 10, 100.0f)]
    public void Lerp_Float_ReturnsCorrectValue(
        float origin, float target, int step, int totalSteps, float expected)
    {
        var result = ChiMath.Lerp(origin, target, step, totalSteps);

        result.Should().BeApproximately(expected, 1e-5f);
    }

    #endregion

    #region Basic Interpolation Tests

    [Fact]
    public void Lerp_Double_StepZero_ReturnsOrigin()
    {
        var result = ChiMath.Lerp(0.0, 100.0, 0, 10);

        result.Should().Be(0.0);
    }

    [Fact]
    public void Lerp_Double_StepEqualsTotalSteps_ReturnsTarget()
    {
        var result = ChiMath.Lerp(0.0, 100.0, 10, 10);

        result.Should().Be(100.0);
    }

    [Fact]
    public void Lerp_Double_MidPoint_ReturnsHalfway()
    {
        var result = ChiMath.Lerp(0.0, 100.0, 5, 10);

        result.Should().BeApproximately(50.0, 1e-10);
    }

    [Theory]
    [InlineData(0.0, 100.0, 1, 10, 10.0)]
    [InlineData(0.0, 100.0, 2, 10, 20.0)]
    [InlineData(0.0, 100.0, 7, 10, 70.0)]
    [InlineData(0.0, 100.0, 9, 10, 90.0)]
    public void Lerp_Double_VariousSteps_ReturnsCorrectValue(
        double origin, double target, int step, int totalSteps, double expected)
    {
        var result = ChiMath.Lerp(origin, target, step, totalSteps);

        result.Should().BeApproximately(expected, 1e-10);
    }

    #endregion

    #region Bidirectional Tests

    [Fact]
    public void Lerp_Double_TargetLessThanOrigin_WorksCorrectly()
    {
        var result = ChiMath.Lerp(100.0, 0.0, 5, 10);

        result.Should().BeApproximately(50.0, 1e-10);
    }

    [Theory]
    [InlineData(100.0, 0.0, 0, 10, 100.0)]
    [InlineData(100.0, 0.0, 2, 10, 80.0)]
    [InlineData(100.0, 0.0, 5, 10, 50.0)]
    [InlineData(100.0, 0.0, 10, 10, 0.0)]
    public void Lerp_Double_Descending_ReturnsCorrectValue(
        double origin, double target, int step, int totalSteps, double expected)
    {
        var result = ChiMath.Lerp(origin, target, step, totalSteps);

        result.Should().BeApproximately(expected, 1e-10);
    }

    [Fact]
    public void Lerp_NegativeValues_WorksCorrectly()
    {
        var result = ChiMath.Lerp(-100.0, 100.0, 5, 10);

        result.Should().BeApproximately(0.0, 1e-10);
    }

    [Fact]
    public void Lerp_CrossingZero_WorksCorrectly()
    {
        var result = ChiMath.Lerp(-50.0, 50.0, 5, 10);

        result.Should().BeApproximately(0.0, 1e-10);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Lerp_StepNegative_ReturnsOrigin()
    {
        var result = ChiMath.Lerp(0.0, 100.0, -5, 10);

        result.Should().Be(0.0);
    }

    [Fact]
    public void Lerp_StepExceedsTotalSteps_ReturnsTarget()
    {
        var result = ChiMath.Lerp(0.0, 100.0, 15, 10);

        result.Should().Be(100.0);
    }

    [Fact]
    public void Lerp_TotalStepsZero_ThrowsArgumentOutOfRangeException()
    {
        var act = () => ChiMath.Lerp(0.0, 100.0, 5, 0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Lerp_TotalStepsNegative_ThrowsArgumentOutOfRangeException()
    {
        var act = () => ChiMath.Lerp(0.0, 100.0, 5, -10);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Lerp_SameOriginAndTarget_ReturnsValue()
    {
        var result = ChiMath.Lerp(42.0, 42.0, 5, 10);

        result.Should().Be(42.0);
    }

    [Fact]
    public void Lerp_SingleStep_ReturnsTarget()
    {
        var result = ChiMath.Lerp(0.0, 100.0, 1, 1);

        result.Should().Be(100.0);
    }

    #endregion

    #region Decimal Tests

    [Theory]
    [InlineData(0, 100, 0, 10, 0)]
    [InlineData(0, 100, 5, 10, 50)]
    [InlineData(0, 100, 10, 10, 100)]
    public void Lerp_Decimal_ReturnsCorrectValue(
        int originInt, int targetInt, int step, int totalSteps, int expectedInt)
    {
        var origin = (decimal)originInt;
        var target = (decimal)targetInt;
        var expected = (decimal)expectedInt;

        var result = ChiMath.Lerp(origin, target, step, totalSteps);

        result.Should().Be(expected);
    }

    [Fact]
    public void Lerp_Decimal_FractionalResult_ReturnsCorrectValue()
    {
        var result = ChiMath.Lerp(0m, 100m, 1, 3);

        result.Should().BeApproximately(33.333333333m, 0.000001m);
    }

    #endregion

    #region Integer Tests

    [Theory]
    [InlineData(0, 100, 0, 10, 0)]
    [InlineData(0, 100, 5, 10, 50)]
    [InlineData(0, 100, 10, 10, 100)]
    public void Lerp_Int_ReturnsCorrectValue(
        int origin, int target, int step, int totalSteps, int expected)
    {
        var result = ChiMath.Lerp(origin, target, step, totalSteps);

        result.Should().Be(expected);
    }

    [Fact]
    public void Lerp_Int_TruncatesTowardZero()
    {
        // 100 * 1/3 = 33.333... truncated to 33
        var result = ChiMath.Lerp(0, 100, 1, 3);

        result.Should().Be(33);
    }

    [Fact]
    public void Lerp_Long_LargeValues_ReturnsCorrectValue()
    {
        var result = ChiMath.Lerp(0L, 1_000_000_000L, 500, 1000);

        result.Should().Be(500_000_000L);
    }

    #endregion

    #region ChiFixed Tests

    [Fact]
    public void Lerp_ChiFixed_ReturnsCorrectValue()
    {
        var origin = (ChiFixed)0m;
        var target = (ChiFixed)100m;

        var result = ChiMath.Lerp(origin, target, 5, 10);

        ((decimal)result).Should().BeApproximately(50m, 0.0001m);
    }

    [Fact]
    public void Lerp_ChiFixed_FractionalResult_ReturnsCorrectValue()
    {
        var origin = (ChiFixed)0m;
        var target = (ChiFixed)100m;

        var result = ChiMath.Lerp(origin, target, 1, 3);

        ((decimal)result).Should().BeApproximately(33.333333m, 0.0001m);
    }

    [Fact]
    public void Lerp_ChiFixed_Bidirectional_WorksCorrectly()
    {
        var origin = (ChiFixed)100m;
        var target = (ChiFixed)0m;

        var result = ChiMath.Lerp(origin, target, 5, 10);

        ((decimal)result).Should().BeApproximately(50m, 0.0001m);
    }

    #endregion

    #region Practical Use Cases

    [Fact]
    public void Lerp_Animation_SmoothTransition()
    {
        const double start = 0.0;
        const double end = 100.0;
        const int frames = 60;

        var values = new double[frames + 1];
        for (var i = 0; i <= frames; i++) values[i] = ChiMath.Lerp(start, end, i, frames);

        values[0].Should().Be(start);
        values[frames].Should().Be(end);
        values[30].Should().BeApproximately(50.0, 1e-10);

        // Values should be monotonically increasing
        for (var i = 1; i <= frames; i++) (values[i] >= values[i - 1]).Should().BeTrue();
    }

    [Fact]
    public void Lerp_ColorBlending_RedToBlue()
    {
        // Simulate RGB color blending (R channel)
        const int redStart = 255;
        const int redEnd = 0;
        const int steps = 10;

        var midRed = ChiMath.Lerp(redStart, redEnd, 5, steps);

        // 255 + (-255 * 5 / 10) = 255 + (-127) = 128
        midRed.Should().Be(128);
    }

    #endregion
}