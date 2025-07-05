using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Normal;

public class SmokeTests
{
    private const int Seed = 1337;

    [Fact]
    public void Next_DefaultDouble_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Normal(0.0, 1.0).Sample();

        // Assert
        result.Should().BeApproximately(-0.13411, 0.00001);
    }

    [Fact]
    public void Next_ConfiguredFloat_ReturnsExpectedValue()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act
        var result = rng.Normal(10.0f, 5.0f).Sample();

        // Assert
        result.Should().BeApproximately(7.523f, 0.001f);
    }

    [Fact]
    public void Normal_WithInvalidStdDev_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var rng = new ChiRng(Seed);

        // Act & Assert
        try
        {
            rng.Normal(0.0, 0.0);

            Assert.Fail(
                "Expected an ArgumentOutOfRangeException for zero standard deviation, but no exception was thrown.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ex.ParamName.Should().Be("standardDeviation");
        }

        // Act & Assert
        try
        {
            rng.Normal(0.0, -1.0);

            Assert.Fail(
                "Expected an ArgumentOutOfRangeException for negative standard deviation, but no exception was thrown.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ex.ParamName.Should().Be("standardDeviation");
        }
    }
}