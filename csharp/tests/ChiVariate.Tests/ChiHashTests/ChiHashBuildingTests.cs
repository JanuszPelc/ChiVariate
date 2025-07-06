using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiHashTests;

/// <summary>
///     Tests that validate the different ways to build ChiHash instances as shown
///     in the XML documentation examples, including the not recommended usage pattern.
/// </summary>
public class ChiHashBuildingTests
{
    [Fact]
    public void ChiHash_DifferentBuildingStyles_ProduceIdenticalResults()
    {
        var value1 = "Hello, world!".AsSpan();
        var value2 = DateTime.Now;

        // Arrange & Act
        var recommendedStyle = new ChiHash().Add(value1).Add(value2);

        var correctStyle = new ChiHash();
        correctStyle = correctStyle.Add(value1);
        correctStyle = correctStyle.Add(value2);

        var discouragedStyle = new ChiHash();
        discouragedStyle.Add(value1);
        discouragedStyle.Add(value2);

        // Assert
        Assert.Equal(recommendedStyle.Hash, correctStyle.Hash);
        Assert.Equal(correctStyle.Hash, discouragedStyle.Hash);
    }
}