using System.Numerics;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using ChiVariate.Internal;
using Xunit;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSeedTests;

public class ChiSeedApiTests
{
    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(-1L)]
    [InlineData(123456789012345L)]
    [InlineData(-987654321098765L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void Scramble_LongSameInputTwice_ReturnsSameOutput(long value)
    {
        var scrambled1 = ChiSeed.Scramble(value);
        var scrambled2 = ChiSeed.Scramble(value);

        scrambled2.Should().Be(scrambled1);
    }

    [Theory]
    [InlineData(0L, 1L)]
    [InlineData(1L, -1L)]
    [InlineData(123L, 124L)]
    [InlineData(long.MaxValue, long.MaxValue - 1)]
    [InlineData(long.MinValue, long.MinValue + 1)]
    [InlineData(0L, -1L)]
    public void Scramble_LongDistinctInputs_ReturnsDifferentOutputs(long value1, long value2)
    {
        value1.Should().NotBe(value2);

        var scrambled1 = ChiSeed.Scramble(value1);
        var scrambled2 = ChiSeed.Scramble(value2);

        scrambled2.Should().NotBe(scrambled1);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(-1L)]
    [InlineData(123456789012345L)]
    [InlineData(-987654321098765L)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void Scramble_LongGivenSameInput_ReturnsSameOutput(long value)
    {
        var scrambled1 = ChiSeed.Scramble(value);
        var scrambled2 = ChiSeed.Scramble(value);

        scrambled2.Should().Be(scrambled1);
    }

    [Theory]
    [InlineData(0L, 1L)]
    [InlineData(1L, -1L)]
    [InlineData(123L, 124L)]
    [InlineData(long.MaxValue, long.MaxValue - 1)]
    [InlineData(long.MinValue, long.MinValue + 1)]
    [InlineData(0L, -1L)]
    public void Scramble_LongGivenDifferentInputs_ReturnsDifferentOutputs(long value1, long value2)
    {
        value1.Should().NotBe(value2);

        var scrambled1 = ChiSeed.Scramble(value1);
        var scrambled2 = ChiSeed.Scramble(value2);

        scrambled2.Should().NotBe(scrambled1);
    }

    [Theory]
    [InlineData("")] // Empty string
    [InlineData("test")]
    [InlineData("Hello, World!")]
    [InlineData("A moderately long string seed for testing determinism.")]
    [InlineData("\t\n\r ")] // Whitespace variations
    [InlineData("你好世界")] // Non-ASCII BMP
    [InlineData("👍")] // Supplementary character
    public void Scramble_StringGivenSameInput_ReturnsSameOutput(string input)
    {
        var scrambled1 = ChiSeed.Scramble(input);
        var scrambled2 = ChiSeed.Scramble(input);

        scrambled1.Should().Be(scrambled2);
    }

    [Theory]
    [InlineData("abc", "abd")]
    [InlineData("Test", "test")] // Case sensitivity
    [InlineData("Hello", " Hello")] // Whitespace sensitivity
    [InlineData("Short", "Longer")]
    [InlineData("", " ")] // Empty vs Space
    [InlineData("👍", "👎")] // Different supplementary chars
    [InlineData("A", "a")]
    public void
        Scramble_StringGivenDifferentInputs_ReturnsDifferentOutputs(string input1, string input2)
    {
        input1.Should().NotBe(input2);

        var scrambled1 = ChiSeed.Scramble(input1);
        var scrambled2 = ChiSeed.Scramble(input2);

        scrambled2.Should().NotBe(scrambled1);
    }

    [Fact]
    public void Scramble_StringGivenEmptyInput_ReturnsSameNonZeroOutput()
    {
        const string emptyString = "";

        var scrambled1 = ChiSeed.Scramble(emptyString);
        var scrambled2 = ChiSeed.Scramble(emptyString);

        scrambled1.Should().Be(scrambled2);
        scrambled1.Should().NotBe(0L);
    }

    [Theory]
    [InlineData("", 5091119643593974729L)] // Empty string
    [InlineData("--null--", 5091119643593974729L)] // Null string
    [InlineData("test", -6718970030811709247L)]
    [InlineData("Hello, World!", -5048483389635163709L)]
    [InlineData("A moderately long string seed for testing determinism.", -8572949766761681549L)]
    [InlineData("\t\n\r ", 6552475925946505334L)] // Whitespace variations
    [InlineData("你好世界", 3950144026336539582L)] // Non-ASCII BMP
    [InlineData("👍", -7582485440326701622L)] // Supplementary character
    public void MixValue_GivenStringInput_ReturnsExpectedResult(string input, long expected)
    {
        if (input == "--null--") input = null!;
        var scrambled = ChiMix64.MixValue(ChiMix64.InitialValue, input);

        scrambled.Should().Be(expected);
    }

    [Fact]
    public void Scramble_AcrossSupportedTypes_ReturnsExpectedSeeds()
    {
        const string? nullString = null;

        using (new AssertionScope())
        {
            // One value per supported type
            ChiSeed.Scramble((sbyte)-5).Should().Be(-4451518155967586974L);
            ChiSeed.Scramble((byte)5).Should().Be(8218792169260817022L);
            ChiSeed.Scramble((short)-1234).Should().Be(9181238287741035579L);
            ChiSeed.Scramble((ushort)1234).Should().Be(-6624350909098874771L);
            ChiSeed.Scramble('Z').Should().Be(-1428490732051570465L);
            ChiSeed.Scramble(-123456).Should().Be(6196111863653467819L);
            ChiSeed.Scramble(123456u).Should().Be(1487188348098688835L);
            ChiSeed.Scramble(-9876543210L).Should().Be(736267325729548934L);
            ChiSeed.Scramble(9876543210uL).Should().Be(-8329670455288885313L);
            ChiSeed.Scramble((Int128)(-123456789012345)).Should().Be(6892683288884869389L);
            ChiSeed.Scramble((UInt128)123456789012345).Should().Be(8842383100629725959L);
            ChiSeed.Scramble((Half)1.5f).Should().Be(8974007776989071390L);
            ChiSeed.Scramble(3.14159f).Should().Be(7331938837981904290L);
            ChiSeed.Scramble(2.718281828).Should().Be(-5333829559198302767L);
            ChiSeed.Scramble(123.456m).Should().Be(-9006105036561612866L);
            ChiSeed.Scramble(true).Should().Be(6233762563770651597L);
            ChiSeed.Scramble(DayOfWeek.Wednesday).Should().Be(-190962954999580667L);
            ChiSeed.Scramble("ChiVariate").Should().Be(1027640967480614865L);
            ChiSeed.Scramble(new Complex(1.5, -2.5)).Should().Be(-746094621248433121L);
            ChiSeed.Scramble(TimeSpan.FromMinutes(90)).Should().Be(-3458993407761408223L);
            ChiSeed.Scramble(Guid.Parse("3f2504e0-4f89-41d3-9a0c-0305e82c3301"))
                .Should().Be(-7343348111894657616L);
            ChiSeed.Scramble(new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc))
                .Should().Be(-5911855917834253741L);
            ChiSeed.Scramble(new DateTimeOffset(2026, 6, 4, 12, 30, 0, TimeSpan.FromHours(2)))
                .Should().Be(694771102433449214L);

            // Multiple values: homogeneous span + heterogeneous arity overloads
            ChiSeed.Scramble(1, 2, 3).Should().Be(-7986552161234627112L);
            ChiSeed.Scramble("seed", 42).Should().Be(-2359032897814657578L);
            ChiSeed.Scramble("seed", 42, true).Should().Be(-2786163074096978042L);
            ChiSeed.Scramble("a", 1, 2L, true, 'c', 3.0, 4.5f, (byte)5).Should().Be(3737481551047750148L);

            // Edges: null string folds as empty, empty string, empty span
            ChiSeed.Scramble(nullString!).Should().Be(2575574750121395476L);
            ChiSeed.Scramble("").Should().Be(2575574750121395476L);
            ChiSeed.Scramble<int>().Should().Be(-3846579739405230477L);
        }
    }
}