using System.Numerics;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSeedTests;

public class ChiSeedBuilderTests
{
    [Fact]
    public void Add_SingleValueOfEachSupportedType_MatchesScrambleOutput()
    {
        using var scope = new AssertionScope();

        new ChiSeed().Add("Apollo 440").Seed.Should().Be(ChiSeed.Scramble("Apollo 440"));
        new ChiSeed().Add((byte)0x2A).Seed.Should().Be(ChiSeed.Scramble((byte)0x2A));
        new ChiSeed().Add((sbyte)-42).Seed.Should().Be(ChiSeed.Scramble((sbyte)-42));
        new ChiSeed().Add((short)-12345).Seed.Should().Be(ChiSeed.Scramble((short)-12345));
        new ChiSeed().Add((ushort)54321).Seed.Should().Be(ChiSeed.Scramble((ushort)54321));
        new ChiSeed().Add('Ω').Seed.Should().Be(ChiSeed.Scramble('Ω'));
        new ChiSeed().Add(-123456789).Seed.Should().Be(ChiSeed.Scramble(-123456789));
        new ChiSeed().Add(0xDEADBEEFu).Seed.Should().Be(ChiSeed.Scramble(0xDEADBEEFu));
        new ChiSeed().Add(long.MinValue).Seed.Should().Be(ChiSeed.Scramble(long.MinValue));
        new ChiSeed().Add(ulong.MaxValue).Seed.Should().Be(ChiSeed.Scramble(ulong.MaxValue));
        new ChiSeed().Add(Int128.MaxValue).Seed.Should().Be(ChiSeed.Scramble(Int128.MaxValue));
        new ChiSeed().Add(UInt128.MaxValue).Seed.Should().Be(ChiSeed.Scramble(UInt128.MaxValue));
        new ChiSeed().Add(3.14159f).Seed.Should().Be(ChiSeed.Scramble(3.14159f));
        new ChiSeed().Add(Math.E).Seed.Should().Be(ChiSeed.Scramble(Math.E));
        new ChiSeed().Add((Half)1.5f).Seed.Should().Be(ChiSeed.Scramble((Half)1.5f));
        new ChiSeed().Add(123.456m).Seed.Should().Be(ChiSeed.Scramble(123.456m));
        new ChiSeed().Add(true).Seed.Should().Be(ChiSeed.Scramble(true));

        var guid = new Guid("a1b2c3d4-e5f6-4789-abcd-ef0123456789");
        new ChiSeed().Add(guid).Seed.Should().Be(ChiSeed.Scramble(guid));

        var complex = new Complex(1.5, -2.5);
        new ChiSeed().Add(complex).Seed.Should().Be(ChiSeed.Scramble(complex));

        var dateTime = new DateTime(2026, 7, 17, 12, 34, 56, DateTimeKind.Utc);
        new ChiSeed().Add(dateTime).Seed.Should().Be(ChiSeed.Scramble(dateTime));

        var dateTimeOffset = new DateTimeOffset(2026, 7, 17, 12, 34, 56, TimeSpan.FromHours(2));
        new ChiSeed().Add(dateTimeOffset).Seed.Should().Be(ChiSeed.Scramble(dateTimeOffset));

        var timeSpan = TimeSpan.FromMinutes(90);
        new ChiSeed().Add(timeSpan).Seed.Should().Be(ChiSeed.Scramble(timeSpan));

        new ChiSeed().Add(TestEnum.First).Seed.Should().Be(ChiSeed.Scramble(TestEnum.First));
        new ChiSeed().Add(TestEnum.Second).Seed.Should().Be(ChiSeed.Scramble(TestEnum.Second));
    }

    [Fact]
    public void Add_ChainedMixedValues_MatchesScrambleAtEveryArity()
    {
        using var scope = new AssertionScope();

        new ChiSeed().Add("a").Add(1).Seed
            .Should().Be(ChiSeed.Scramble("a", 1));
        new ChiSeed().Add("a").Add(1).Add(2.5).Seed
            .Should().Be(ChiSeed.Scramble("a", 1, 2.5));
        new ChiSeed().Add("a").Add(1).Add(2.5).Add(true).Seed
            .Should().Be(ChiSeed.Scramble("a", 1, 2.5, true));
        new ChiSeed().Add("a").Add(1).Add(2.5).Add(true).Add('x').Seed
            .Should().Be(ChiSeed.Scramble("a", 1, 2.5, true, 'x'));
        new ChiSeed().Add("a").Add(1).Add(2.5).Add(true).Add('x').Add(42L).Seed
            .Should().Be(ChiSeed.Scramble("a", 1, 2.5, true, 'x', 42L));
        new ChiSeed().Add("a").Add(1).Add(2.5).Add(true).Add('x').Add(42L).Add((byte)7).Seed
            .Should().Be(ChiSeed.Scramble("a", 1, 2.5, true, 'x', 42L, (byte)7));
        new ChiSeed().Add("a").Add(1).Add(2.5).Add(true).Add('x').Add(42L).Add((byte)7).Add("z").Seed
            .Should().Be(ChiSeed.Scramble("a", 1, 2.5, true, 'x', 42L, (byte)7, "z"));
    }

    [Fact]
    public void Add_SpanOfValues_MatchesParamsSpanScramble()
    {
        ReadOnlySpan<int> numbers = [3, 1, 4, 1, 5];
        new ChiSeed().Add(numbers).Seed.Should().Be(ChiSeed.Scramble(numbers));

        ReadOnlySpan<string> words = ["alpha", "beta", "gamma"];
        new ChiSeed().Add(words).Seed.Should().Be(ChiSeed.Scramble(words));
    }

    [Fact]
    public void Add_ScalarsAndSpansInterleaved_MatchesFlattenedScramble()
    {
        ReadOnlySpan<int> middle = [2, 3];
        var seed = new ChiSeed().Add(1).Add(middle).Add(4).Seed;

        seed.Should().Be(ChiSeed.Scramble(1, 2, 3, 4));
    }

    [Fact]
    public void Add_NullString_MatchesEmptyStringScramble()
    {
        new ChiSeed().Add(null).Seed.Should().Be(ChiSeed.Scramble(""));
    }

    [Fact]
    public void Seed_WithNoValuesAdded_MatchesEmptyScramble()
    {
        new ChiSeed().Seed.Should().Be(ChiSeed.Scramble(ReadOnlySpan<int>.Empty));
        default(ChiSeed).Seed.Should().Be(new ChiSeed().Seed);
    }

    [Fact]
    public void Seed_ReadMidChain_DoesNotAffectFinalValue()
    {
        var builder = new ChiSeed().Add("checkpoint");
        _ = builder.Seed;

        builder.Add(42).Seed.Should().Be(ChiSeed.Scramble("checkpoint", 42));
    }

    private enum TestEnum
    {
        First = 1,
        Second = 2
    }
}