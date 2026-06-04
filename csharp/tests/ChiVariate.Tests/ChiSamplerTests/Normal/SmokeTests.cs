using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerTests.Normal;

public class SmokeTests
{
    private const int Seed = 1337;

    [Fact]
    public void Sample_StandardNormalDouble_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Normal(0.0, 1.0).Sample();

        result.Should().BeApproximately(0.25341, 0.00001);
    }

    [Fact]
    public void Sample_Float_ReturnsExpectedValue()
    {
        var rng = new ChiRng(Seed);

        var result = rng.Normal(10.0f, 5.0f).Sample();

        result.Should().BeApproximately(11.267f, 0.001f);
    }

    [Fact]
    public void Normal_WithZeroStdDev_ThrowsArgumentOutOfRangeException()
    {
        var rng = new ChiRng(Seed);

        try
        {
            _ = rng.Normal(0.0, 0.0);
            throw new Exception("Expected ArgumentOutOfRangeException was not thrown");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ex.ParamName.Should().Be("standardDeviation");
        }
    }

    [Fact]
    public void Normal_WithNegativeStdDev_ThrowsArgumentOutOfRangeException()
    {
        var rng = new ChiRng(Seed);

        try
        {
            _ = rng.Normal(0.0, -1.0);
            throw new Exception("Expected ArgumentOutOfRangeException was not thrown");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ex.ParamName.Should().Be("standardDeviation");
        }
    }
}