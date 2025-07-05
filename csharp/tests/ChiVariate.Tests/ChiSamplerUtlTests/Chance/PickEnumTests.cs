using System.Numerics;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Chance;

public class PickEnumTests
{
    private const int SampleCount = 100_000;

    [Fact]
    public void PickEnum_ForSimpleEnum_OnlyReturnsDefinedValues()
    {
        // Arrange
        var rng = new ChiRng(123);
        var definedValues = Enum.GetValues<TestColor>();

        // Act & Assert
        for (var i = 0; i < SampleCount; i++)
        {
            var result = rng.Chance().PickEnum<TestColor>();
            definedValues.Should().Contain(result, "because PickEnum should only return a defined member.");
        }
    }

    [Fact]
    public void PickEnum_ForSimpleEnum_ProducesUniformDistribution()
    {
        // Arrange
        var rng = new ChiRng(456);
        var definedValues = Enum.GetValues<TestColor>();
        var histogram = new Dictionary<TestColor, int>();
        foreach (var value in definedValues) histogram[value] = 0;
        const int sampleCount = 100_000;

        // Act
        for (var i = 0; i < sampleCount; i++)
        {
            var result = rng.Chance().PickEnum<TestColor>();
            histogram[result]++;
        }

        // Assert
        var expectedCount = (double)sampleCount / definedValues.Length;
        const double tolerance = 0.05; // 5% tolerance
        var minAllowed = (int)(expectedCount * (1.0 - tolerance));
        var maxAllowed = (int)(expectedCount * (1.0 + tolerance));

        foreach (var value in definedValues)
            histogram[value].Should().BeInRange(minAllowed, maxAllowed,
                $"because the distribution for {value} should be uniform.");
    }

    [Fact]
    public void PickEnum_ForFlagsEnum_OnlyReturnsSingleDefinedFlags()
    {
        // Arrange
        var rng = new ChiRng(789);
        var definedValues = Enum.GetValues<TestFlags>();

        // Act & Assert
        for (var i = 0; i < SampleCount; i++)
        {
            var result = rng.Chance().PickEnum<TestFlags>();

            definedValues.Should().Contain(result,
                "because the result must be a value explicitly defined in the enum.");

            if (result != TestFlags.None)
                BitOperations.IsPow2((int)result).Should().BeTrue(
                    $"because {result} should be a single flag, not a combination like (Read | Write).");
        }
    }

    // ReSharper disable UnusedMember.Local
    private enum TestColor
    {
        Red,
        Green,
        Blue
    }

    [Flags]
    private enum TestFlags
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    }
    // ReSharper restore UnusedMember.Local
}