using System.Numerics;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiHashTests;

public class ChiHashCombineTests
{
    [Fact]
    public void Combine_SingleValueOfEachSupportedType_MatchesAddChain()
    {
        using var scope = new AssertionScope();

        ChiHash.Combine("Apollo 440").Should().Be(new ChiHash().Add("Apollo 440").Hash);
        ChiHash.Combine((byte)0x2A).Should().Be(new ChiHash().Add((byte)0x2A).Hash);
        ChiHash.Combine((sbyte)-42).Should().Be(new ChiHash().Add((sbyte)-42).Hash);
        ChiHash.Combine((short)-12345).Should().Be(new ChiHash().Add((short)-12345).Hash);
        ChiHash.Combine((ushort)54321).Should().Be(new ChiHash().Add((ushort)54321).Hash);
        ChiHash.Combine('Ω').Should().Be(new ChiHash().Add('Ω').Hash);
        ChiHash.Combine(-123456789).Should().Be(new ChiHash().Add(-123456789).Hash);
        ChiHash.Combine(0xDEADBEEFu).Should().Be(new ChiHash().Add(0xDEADBEEFu).Hash);
        ChiHash.Combine(long.MinValue).Should().Be(new ChiHash().Add(long.MinValue).Hash);
        ChiHash.Combine(ulong.MaxValue).Should().Be(new ChiHash().Add(ulong.MaxValue).Hash);
        ChiHash.Combine(Int128.MaxValue).Should().Be(new ChiHash().Add(Int128.MaxValue).Hash);
        ChiHash.Combine(UInt128.MaxValue).Should().Be(new ChiHash().Add(UInt128.MaxValue).Hash);
        ChiHash.Combine(3.14159f).Should().Be(new ChiHash().Add(3.14159f).Hash);
        ChiHash.Combine(Math.E).Should().Be(new ChiHash().Add(Math.E).Hash);
        ChiHash.Combine((Half)1.5f).Should().Be(new ChiHash().Add((Half)1.5f).Hash);
        ChiHash.Combine(123.456m).Should().Be(new ChiHash().Add(123.456m).Hash);
        ChiHash.Combine(true).Should().Be(new ChiHash().Add(true).Hash);

        var guid = new Guid("a1b2c3d4-e5f6-4789-abcd-ef0123456789");
        ChiHash.Combine(guid).Should().Be(new ChiHash().Add(guid).Hash);

        var complex = new Complex(1.5, -2.5);
        ChiHash.Combine(complex).Should().Be(new ChiHash().Add(complex).Hash);

        var dateTime = new DateTime(2026, 7, 17, 12, 34, 56, DateTimeKind.Utc);
        ChiHash.Combine(dateTime).Should().Be(new ChiHash().Add(dateTime).Hash);

        var dateTimeOffset = new DateTimeOffset(2026, 7, 17, 12, 34, 56, TimeSpan.FromHours(2));
        ChiHash.Combine(dateTimeOffset).Should().Be(new ChiHash().Add(dateTimeOffset).Hash);

        var timeSpan = TimeSpan.FromMinutes(90);
        ChiHash.Combine(timeSpan).Should().Be(new ChiHash().Add(timeSpan).Hash);

        ChiHash.Combine(TestEnum.First).Should().Be(new ChiHash().Add(TestEnum.First).Hash);
        ChiHash.Combine(TestEnum.Second).Should().Be(new ChiHash().Add(TestEnum.Second).Hash);
    }

    [Fact]
    public void Combine_MixedValuesAtEveryArity_MatchesAddChain()
    {
        using var scope = new AssertionScope();

        ChiHash.Combine("a", 1)
            .Should().Be(new ChiHash().Add("a").Add(1).Hash);
        ChiHash.Combine("a", 1, 2.5)
            .Should().Be(new ChiHash().Add("a").Add(1).Add(2.5).Hash);
        ChiHash.Combine("a", 1, 2.5, true)
            .Should().Be(new ChiHash().Add("a").Add(1).Add(2.5).Add(true).Hash);
        ChiHash.Combine("a", 1, 2.5, true, 'x')
            .Should().Be(new ChiHash().Add("a").Add(1).Add(2.5).Add(true).Add('x').Hash);
        ChiHash.Combine("a", 1, 2.5, true, 'x', 42L)
            .Should().Be(new ChiHash().Add("a").Add(1).Add(2.5).Add(true).Add('x').Add(42L).Hash);
        ChiHash.Combine("a", 1, 2.5, true, 'x', 42L, (byte)7)
            .Should().Be(new ChiHash().Add("a").Add(1).Add(2.5).Add(true).Add('x').Add(42L).Add((byte)7).Hash);
        ChiHash.Combine("a", 1, 2.5, true, 'x', 42L, (byte)7, "z")
            .Should().Be(new ChiHash().Add("a").Add(1).Add(2.5).Add(true).Add('x').Add(42L).Add((byte)7).Add("z")
                .Hash);
    }

    [Fact]
    public void Combine_SpanOfValues_MatchesAddSpan()
    {
        ReadOnlySpan<int> numbers = [3, 1, 4, 1, 5];
        ChiHash.Combine(numbers).Should().Be(new ChiHash().Add(numbers).Hash);
    }

    [Fact]
    public void Combine_NullString_MatchesAddNullString()
    {
        ChiHash.Combine((string?)null).Should().Be(new ChiHash().Add(null).Hash);
    }

    [Fact]
    public void Add_SpanOfStrings_MatchesChainedStringAdds()
    {
        ReadOnlySpan<string> words = ["alpha", "beta", "gamma"];

        new ChiHash().Add(words).Hash
            .Should().Be(new ChiHash().Add("alpha").Add("beta").Add("gamma").Hash);
    }

    private enum TestEnum
    {
        First = 1,
        Second = 2
    }
}