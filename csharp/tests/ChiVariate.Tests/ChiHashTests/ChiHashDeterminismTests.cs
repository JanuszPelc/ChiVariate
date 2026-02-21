using System.Numerics;
using AwesomeAssertions;
using Xunit;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiHashTests;

/// <summary>
///     Tests that validate exact hash values for specific inputs to ensure
///     cross-platform determinism and prevent regressions.
/// </summary>
public class ChiHashDeterminismTests
{
    [Fact]
    public void ChiHash_SpecificInputs_ProducesExpectedHashValues()
    {
        new ChiHash().Add((byte)42).Hash.Should().Be(965500275);
        new ChiHash().Add((sbyte)-42).Hash.Should().Be(-115445778);
        new ChiHash().Add((short)1234).Hash.Should().Be(-833837988);
        new ChiHash().Add((ushort)5678).Hash.Should().Be(-13209409);
        new ChiHash().Add('A').Hash.Should().Be(1296678464);
        new ChiHash().Add(42).Hash.Should().Be(965500275);
        new ChiHash().Add(42U).Hash.Should().Be(965500275);
        new ChiHash().Add(1234567890L).Hash.Should().Be(1496366746);
        new ChiHash().Add(9876543210UL).Hash.Should().Be(511386668);
        new ChiHash().Add(3.14159f).Hash.Should().Be(1116486396);
        new ChiHash().Add(2.71828).Hash.Should().Be(1541193133);
        new ChiHash().Add((Half)1.5f).Hash.Should().Be(1880826208);
        new ChiHash().Add(123.456m).Hash.Should().Be(1433790916);
        new ChiHash().Add(Int128.MaxValue).Hash.Should().Be(1353729377);
        new ChiHash().Add(UInt128.MaxValue).Hash.Should().Be(614782079);

        new ChiHash().Add(null!).Hash.Should().Be(0);
        new ChiHash().Add("").Hash.Should().Be(0);
        new ChiHash().Add("hello").Hash.Should().Be(-525174955);
        new ChiHash().Add("Hello").Hash.Should().Be(692606700);
        new ChiHash().Add("hello world").Hash.Should().Be(-1668659344);
        new ChiHash().Add("🌍").Hash.Should().Be(1699588458);
        new ChiHash().Add("café").Hash.Should().Be(1007101452);
        new ChiHash().Add("こんにちは").Hash.Should().Be(723002084);
        new ChiHash().Add("你好世界").Hash.Should().Be(-1085445209);
        new ChiHash().Add("👍").Hash.Should().Be(145577430);

        new ChiHash().Add(true).Hash.Should().Be(-747144466);
        new ChiHash().Add(false).Hash.Should().Be(150996269);

        var guid1 = Guid.Parse("12345678-1234-5678-9abc-123456789abc");
        var guid2 = Guid.Parse("00000000-0000-0000-0000-000000000000");
        new ChiHash().Add(guid1).Hash.Should().Be(-2083929397);
        new ChiHash().Add(guid2).Hash.Should().Be(44345189);

        var date1 = new DateTime(2023, 12, 25, 15, 30, 45, DateTimeKind.Utc);
        var date2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        new ChiHash().Add(date1).Hash.Should().Be(-219477393);
        new ChiHash().Add(date2).Hash.Should().Be(2120322267);

        var dto1 = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.FromHours(5));
        var dto2 = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.Zero);
        new ChiHash().Add(dto1).Hash.Should().Be(1239868323);
        new ChiHash().Add(dto2).Hash.Should().Be(-1804676602);

        var ts1 = TimeSpan.FromMinutes(123);
        var ts2 = TimeSpan.Zero;
        new ChiHash().Add(ts1).Hash.Should().Be(-1717822425);
        new ChiHash().Add(ts2).Hash.Should().Be(-154722900);

        var complex1 = new Complex(1.5, 2.5);
        var complex2 = Complex.Zero;
        new ChiHash().Add(complex1).Hash.Should().Be(-286954189);
        new ChiHash().Add(complex2).Hash.Should().Be(44345189);

        new ChiHash().Add(new Complex(double.NaN, 1.0)).Hash
            .Should().Be(new ChiHash().Add(new Complex(Math.Sqrt(-1.0), 1.0)).Hash);
        new ChiHash().Add(new Complex(-0.0, -0.0)).Hash
            .Should().Be(new ChiHash().Add(Complex.Zero).Hash);
        new ChiHash().Add(new Complex(double.PositiveInfinity, double.NegativeInfinity)).Hash
            .Should().Be(new ChiHash().Add(new Complex(double.PositiveInfinity, double.NegativeInfinity)).Hash);

        new ChiHash().Add(TestEnum.Value1).Hash.Should().Be(1915432565);
        new ChiHash().Add(TestEnum.Value2).Hash.Should().Be(-1080233084);
        new ChiHash().Add(TestByteEnum.A).Hash.Should().Be(326026363);
        new ChiHash().Add(TestByteEnum.B).Hash.Should().Be(-385997327);
        new ChiHash().Add(TestLongEnum.Large).Hash.Should().Be(-947741260);
        new ChiHash().Add(TestLongEnum.Larger).Hash.Should().Be(-140159068);

        var intArray = new[] { 1, 2, 3, 4, 5 };
        var guidArray = new[] { guid1, guid2 };
        new ChiHash().Add(intArray.AsSpan()).Hash.Should().Be(-2084177982);
        new ChiHash().Add(guidArray.AsSpan()).Hash.Should().Be(-251003134);

        var mixedHash = new ChiHash()
            .Add(42)
            .Add("hello")
            .Add(true)
            .Add(guid1)
            .Add(date1)
            .Add(TestEnum.Value1)
            .Hash;
        mixedHash.Should().Be(-574704062);

        new ChiHash().Hash.Should().Be(0);

        new ChiHash().Add(float.NaN).Hash.Should().Be(-1606370751);
        new ChiHash().Add(float.PositiveInfinity).Hash.Should().Be(-319676561);
        new ChiHash().Add(float.NegativeInfinity).Hash.Should().Be(668732010);
        new ChiHash().Add(float.Epsilon).Hash.Should().Be(-747144466);
        new ChiHash().Add(-0.0f).Hash.Should().Be(150996269);
        new ChiHash().Add(+0.0f).Hash.Should().Be(150996269);

        new ChiHash().Add(double.NaN).Hash.Should().Be(829280496);
        new ChiHash().Add(double.PositiveInfinity).Hash.Should().Be(1542370574);
        new ChiHash().Add(double.NegativeInfinity).Hash.Should().Be(1694129150);
        new ChiHash().Add(double.Epsilon).Hash.Should().Be(1336914937);
        new ChiHash().Add(-0.0).Hash.Should().Be(-154722900);
        new ChiHash().Add(+0.0).Hash.Should().Be(-154722900);

        new ChiHash().Add((Half)float.PositiveInfinity).Hash.Should().Be(1613653932);
        new ChiHash().Add((Half)float.NegativeInfinity).Hash.Should().Be(-1866992690);
        new ChiHash().Add((Half)float.NaN).Hash.Should().Be(-1083467430);
        new ChiHash().Add(-(Half)0.0).Hash.Should().Be(150996269);
        new ChiHash().Add(+(Half)0.0).Hash.Should().Be(150996269);

        new ChiHash().Add(byte.MinValue).Hash.Should().Be(150996269);
        new ChiHash().Add(byte.MaxValue).Hash.Should().Be(-42146061);
        new ChiHash().Add(sbyte.MinValue).Hash.Should().Be(-965512797);
        new ChiHash().Add(sbyte.MaxValue).Hash.Should().Be(835380994);
        new ChiHash().Add(short.MinValue).Hash.Should().Be(1307910309);
        new ChiHash().Add(short.MaxValue).Hash.Should().Be(-2035531905);
        new ChiHash().Add(ushort.MinValue).Hash.Should().Be(150996269);
        new ChiHash().Add(ushort.MaxValue).Hash.Should().Be(1897710364);
        new ChiHash().Add(int.MinValue).Hash.Should().Be(-1538840509);
        new ChiHash().Add(int.MaxValue).Hash.Should().Be(-1846084325);
        new ChiHash().Add(uint.MinValue).Hash.Should().Be(150996269);
        new ChiHash().Add(uint.MaxValue).Hash.Should().Be(1504591717);
        new ChiHash().Add(long.MinValue).Hash.Should().Be(-1426660308);
        new ChiHash().Add(long.MaxValue).Hash.Should().Be(1649257790);
        new ChiHash().Add(ulong.MinValue).Hash.Should().Be(-154722900);
        new ChiHash().Add(ulong.MaxValue).Hash.Should().Be(861868037);

        new ChiHash().Add(decimal.MinValue).Hash.Should().Be(-1541255506);
        new ChiHash().Add(decimal.MaxValue).Hash.Should().Be(-1284415288);
        new ChiHash().Add(decimal.Zero).Hash.Should().Be(44345189);
        new ChiHash().Add(decimal.One).Hash.Should().Be(-303449875);
        new ChiHash().Add(decimal.MinusOne).Hash.Should().Be(1567788007);
    }

    private enum TestEnum
    {
        Value1 = 10,
        Value2 = 20
    }

    private enum TestByteEnum : byte
    {
        A = 100,
        B = 200
    }

    private enum TestLongEnum : long
    {
        Large = 1_000_000_000L,
        Larger = 2_000_000_000L
    }
}