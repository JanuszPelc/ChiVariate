using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiStepperTests;

public class ChiStepperTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        var stepper = new ChiStepper(24, 60);

        stepper.Numerator.Should().Be(24);
        stepper.Denominator.Should().Be(60);
        stepper.Accumulated.Should().Be(0);
    }

    [Fact]
    public void Constructor_IntOverload_CreatesInstance()
    {
        var stepper = new ChiStepper(1, 3);

        stepper.Numerator.Should().Be(1);
        stepper.Denominator.Should().Be(3);
    }

    [Fact]
    public void Constructor_ZeroDenominator_ThrowsArgumentOutOfRangeException()
    {
        var act = () => new ChiStepper(1, 0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_NegativeNumerator_AllowsNegativeRate()
    {
        var stepper = new ChiStepper(-1, 3);

        stepper.Numerator.Should().Be(-1);
    }

    [Fact]
    public void Constructor_NegativeDenominator_Allowed()
    {
        var stepper = new ChiStepper(1, -3);

        stepper.Denominator.Should().Be(-3);
    }

    #endregion

    #region Step Tests

    [Fact]
    public void Step_SingleStep_AccumulatesNumerator()
    {
        var stepper = new ChiStepper(24, 60);

        var stepped = stepper.Step();

        stepped.Accumulated.Should().Be(24);
    }

    [Fact]
    public void Step_MultipleSteps_AccumulatesCorrectly()
    {
        var stepper = new ChiStepper(24, 60);

        var stepped = stepper.Step().Step().Step();

        stepped.Accumulated.Should().Be(72);
    }

    [Fact]
    public void Step_WithCount_AccumulatesCorrectly()
    {
        var stepper = new ChiStepper(24, 60);

        var stepped = stepper.Step(5);

        stepped.Accumulated.Should().Be(120);
    }

    [Fact]
    public void Step_WithLongCount_AccumulatesCorrectly()
    {
        var stepper = new ChiStepper(1, 1000);

        var stepped = stepper.Step(1_000_000L);

        stepped.Accumulated.Should().Be(1_000_000L);
    }

    [Fact]
    public void Step_IsImmutable_OriginalUnchanged()
    {
        var original = new ChiStepper(24, 60);

        var stepped = original.Step();

        original.Accumulated.Should().Be(0);
        stepped.Accumulated.Should().Be(24);
    }

    #endregion

    #region WholeUnits and Remainder Tests

    [Theory]
    [InlineData(24, 60, 1, 0)]
    [InlineData(24, 60, 2, 0)]
    [InlineData(24, 60, 3, 1)]
    [InlineData(24, 60, 5, 2)]
    [InlineData(24, 60, 10, 4)]
    public void WholeUnits_AfterSteps_ReturnsCorrectValue(int numerator, int denominator, int steps,
        int expectedWholeUnits)
    {
        var stepper = new ChiStepper(numerator, denominator).Step(steps);

        stepper.WholeUnits.Should().Be(expectedWholeUnits);
    }

    [Theory]
    [InlineData(24, 60, 1, 24)]
    [InlineData(24, 60, 2, 48)]
    [InlineData(24, 60, 3, 12)]
    [InlineData(24, 60, 5, 0)]
    public void Remainder_AfterSteps_ReturnsCorrectValue(int numerator, int denominator, int steps,
        int expectedRemainder)
    {
        var stepper = new ChiStepper(numerator, denominator).Step(steps);

        stepper.Remainder.Should().Be(expectedRemainder);
    }

    [Fact]
    public void WholeUnitsAndRemainder_FrameRateConversion_24fpsTo60fps()
    {
        // 24fps content on 60fps display: every 2.5 frames, advance content
        var stepper = new ChiStepper(24, 60);

        // After 5 display frames, should have shown 2 content frames
        var afterFive = stepper.Step(5);
        afterFive.WholeUnits.Should().Be(2);
        afterFive.Remainder.Should().Be(0);

        // After 10 display frames, should have shown 4 content frames
        var afterTen = stepper.Step(10);
        afterTen.WholeUnits.Should().Be(4);
    }

    [Fact]
    public void WholeUnitsAndRemainder_AudioResampling_44100To48000()
    {
        // 44100 Hz to 48000 Hz: ratio = 44100/48000 = 147/160
        var stepper = new ChiStepper(147, 160);

        // After 160 output samples, should have consumed 147 input samples
        var after160 = stepper.Step(160);
        after160.WholeUnits.Should().Be(147);
        after160.Remainder.Should().Be(0);
    }

    #endregion

    #region Reset Tests

    [Fact]
    public void Reset_AfterSteps_ResetsAccumulator()
    {
        var stepper = new ChiStepper(24, 60).Step(10);

        var reset = stepper.Reset();

        reset.Accumulated.Should().Be(0);
        reset.WholeUnits.Should().Be(0);
        reset.Remainder.Should().Be(0);
    }

    [Fact]
    public void Reset_PreservesRate()
    {
        var stepper = new ChiStepper(24, 60).Step(10);

        var reset = stepper.Reset();

        reset.Numerator.Should().Be(24);
        reset.Denominator.Should().Be(60);
    }

    #endregion

    #region AsValue Tests

    [Fact]
    public void AsValue_Double_ReturnsCorrectValue()
    {
        var stepper = new ChiStepper(1, 3).Step(1);

        var result = stepper.AsValue<double>();

        result.Should().BeApproximately(1.0 / 3.0, 1e-10);
    }

    [Fact]
    public void AsValue_Decimal_ReturnsCorrectValue()
    {
        var stepper = new ChiStepper(1, 4).Step(1);

        var result = stepper.AsValue<decimal>();

        result.Should().Be(0.25m);
    }

    [Fact]
    public void AsValue_Int_TruncatesTowardZero()
    {
        var stepper = new ChiStepper(24, 60).Step(3); // 72/60 = 1.2

        var result = stepper.AsValue<int>();

        result.Should().Be(1);
    }

    [Fact]
    public void AsValue_ChiFixed_ReturnsCorrectValue()
    {
        var stepper = new ChiStepper(1, 2).Step(1);

        var result = stepper.AsValue<ChiFixed>();

        ((decimal)result).Should().Be(0.5m);
    }

    [Fact]
    public void AsValue_AfterMultipleSteps_AccumulatesCorrectly()
    {
        var stepper = new ChiStepper(1, 10).Step(25);

        var result = stepper.AsValue<double>();

        result.Should().BeApproximately(2.5, 1e-10);
    }

    #endregion

    #region Practical Use Cases

    [Fact]
    public void Stepper_TileMovement_FractionalSpeed()
    {
        // Move at 0.4 tiles per frame
        var stepper = new ChiStepper(2, 5);
        var totalTilesMoved = 0L;

        for (var frame = 0; frame < 100; frame++)
        {
            stepper = stepper.Step();
            var newTiles = stepper.WholeUnits;
            if (newTiles > totalTilesMoved) totalTilesMoved = newTiles;
        }

        // After 100 frames at 0.4 tiles/frame = 40 tiles
        totalTilesMoved.Should().Be(40);
    }

    [Fact]
    public void Stepper_NoDrift_PerfectAccumulation()
    {
        // Verify no floating-point drift over many iterations
        var stepper = new ChiStepper(1, 7);

        stepper = stepper.Step(7_000_000);

        // Should be exactly 1,000,000 with no drift
        stepper.WholeUnits.Should().Be(1_000_000);
        stepper.Remainder.Should().Be(0);
    }

    [Fact]
    public void Stepper_NegativeRate_WorksCorrectly()
    {
        var stepper = new ChiStepper(-1, 3);

        var stepped = stepper.Step(6);

        stepped.WholeUnits.Should().Be(-2);
        stepped.Remainder.Should().Be(0);
    }

    #endregion
}