using System.Numerics;
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
        // Arrange & Act & Assert
        Assert.Equal(965500275, new ChiHash().Add((byte)42).Hash);
        Assert.Equal(-115445778, new ChiHash().Add((sbyte)-42).Hash);
        Assert.Equal(-833837988, new ChiHash().Add((short)1234).Hash);
        Assert.Equal(-13209409, new ChiHash().Add((ushort)5678).Hash);
        Assert.Equal(1296678464, new ChiHash().Add('A').Hash);
        Assert.Equal(965500275, new ChiHash().Add(42).Hash);
        Assert.Equal(965500275, new ChiHash().Add(42U).Hash);
        Assert.Equal(1496366746, new ChiHash().Add(1234567890L).Hash);
        Assert.Equal(511386668, new ChiHash().Add(9876543210UL).Hash);
        Assert.Equal(1116486396, new ChiHash().Add(3.14159f).Hash);
        Assert.Equal(1541193133, new ChiHash().Add(2.71828).Hash);
        Assert.Equal(1880826208, new ChiHash().Add((Half)1.5f).Hash);
        Assert.Equal(1433790916, new ChiHash().Add(123.456m).Hash);
        Assert.Equal(1353729377, new ChiHash().Add(Int128.MaxValue).Hash);
        Assert.Equal(614782079, new ChiHash().Add(UInt128.MaxValue).Hash);

        Assert.Equal(0, new ChiHash().Add(null!).Hash);
        Assert.Equal(0, new ChiHash().Add("").Hash);
        Assert.Equal(-525174955, new ChiHash().Add("hello").Hash);
        Assert.Equal(692606700, new ChiHash().Add("Hello").Hash);
        Assert.Equal(-1668659344, new ChiHash().Add("hello world").Hash);
        Assert.Equal(1699588458, new ChiHash().Add("🌍").Hash);
        Assert.Equal(1007101452, new ChiHash().Add("café").Hash);
        Assert.Equal(723002084, new ChiHash().Add("こんにちは").Hash);
        Assert.Equal(-1085445209, new ChiHash().Add("你好世界").Hash);
        Assert.Equal(145577430, new ChiHash().Add("👍").Hash);

        Assert.Equal(-747144466, new ChiHash().Add(true).Hash);
        Assert.Equal(150996269, new ChiHash().Add(false).Hash);

        var guid1 = Guid.Parse("12345678-1234-5678-9abc-123456789abc");
        var guid2 = Guid.Parse("00000000-0000-0000-0000-000000000000");
        Assert.Equal(-2083929397, new ChiHash().Add(guid1).Hash);
        Assert.Equal(44345189, new ChiHash().Add(guid2).Hash);

        var date1 = new DateTime(2023, 12, 25, 15, 30, 45, DateTimeKind.Utc);
        var date2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(-219477393, new ChiHash().Add(date1).Hash);
        Assert.Equal(2120322267, new ChiHash().Add(date2).Hash);

        var dto1 = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.FromHours(5));
        var dto2 = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.Zero);
        Assert.Equal(1239868323, new ChiHash().Add(dto1).Hash);
        Assert.Equal(-1804676602, new ChiHash().Add(dto2).Hash);

        var ts1 = TimeSpan.FromMinutes(123);
        var ts2 = TimeSpan.Zero;
        Assert.Equal(-1717822425, new ChiHash().Add(ts1).Hash);
        Assert.Equal(-154722900, new ChiHash().Add(ts2).Hash);

        var complex1 = new Complex(1.5, 2.5);
        var complex2 = Complex.Zero;
        Assert.Equal(-286954189, new ChiHash().Add(complex1).Hash);
        Assert.Equal(44345189, new ChiHash().Add(complex2).Hash);

        Assert.Equal(1915432565, new ChiHash().Add(TestEnum.Value1).Hash);
        Assert.Equal(-1080233084, new ChiHash().Add(TestEnum.Value2).Hash);
        Assert.Equal(326026363, new ChiHash().Add(TestByteEnum.A).Hash);
        Assert.Equal(-385997327, new ChiHash().Add(TestByteEnum.B).Hash);
        Assert.Equal(-947741260, new ChiHash().Add(TestLongEnum.Large).Hash);
        Assert.Equal(-140159068, new ChiHash().Add(TestLongEnum.Larger).Hash);

        var intArray = new[] { 1, 2, 3, 4, 5 };
        var guidArray = new[] { guid1, guid2 };
        Assert.Equal(-2084177982, new ChiHash().Add(intArray.AsSpan()).Hash);
        Assert.Equal(-251003134, new ChiHash().Add(guidArray.AsSpan()).Hash);

        var mixedHash = new ChiHash()
            .Add(42)
            .Add("hello")
            .Add(true)
            .Add(guid1)
            .Add(date1)
            .Add(TestEnum.Value1)
            .Hash;
        Assert.Equal(-574704062, mixedHash);

        Assert.Equal(0, new ChiHash().Hash);

        Assert.Equal(-1822049585, new ChiHash().Add(float.NaN).Hash);
        Assert.Equal(-319676561, new ChiHash().Add(float.PositiveInfinity).Hash);
        Assert.Equal(668732010, new ChiHash().Add(float.NegativeInfinity).Hash);
        Assert.Equal(-747144466, new ChiHash().Add(float.Epsilon).Hash);
        Assert.Equal(-1538840509, new ChiHash().Add(-0.0f).Hash);
        Assert.Equal(150996269, new ChiHash().Add(+0.0f).Hash);

        Assert.Equal(1914784418, new ChiHash().Add(double.NaN).Hash);
        Assert.Equal(1542370574, new ChiHash().Add(double.PositiveInfinity).Hash);
        Assert.Equal(1694129150, new ChiHash().Add(double.NegativeInfinity).Hash);
        Assert.Equal(1336914937, new ChiHash().Add(double.Epsilon).Hash);
        Assert.Equal(-1426660308, new ChiHash().Add(-0.0).Hash);
        Assert.Equal(-154722900, new ChiHash().Add(+0.0).Hash);

        Assert.Equal(1613653932, new ChiHash().Add((Half)float.PositiveInfinity).Hash);
        Assert.Equal(-1866992690, new ChiHash().Add((Half)float.NegativeInfinity).Hash);
        Assert.Equal(-1737678051, new ChiHash().Add((Half)float.NaN).Hash);

        Assert.Equal(150996269, new ChiHash().Add(byte.MinValue).Hash);
        Assert.Equal(-42146061, new ChiHash().Add(byte.MaxValue).Hash);
        Assert.Equal(-965512797, new ChiHash().Add(sbyte.MinValue).Hash);
        Assert.Equal(835380994, new ChiHash().Add(sbyte.MaxValue).Hash);
        Assert.Equal(1307910309, new ChiHash().Add(short.MinValue).Hash);
        Assert.Equal(-2035531905, new ChiHash().Add(short.MaxValue).Hash);
        Assert.Equal(150996269, new ChiHash().Add(ushort.MinValue).Hash);
        Assert.Equal(1897710364, new ChiHash().Add(ushort.MaxValue).Hash);
        Assert.Equal(-1538840509, new ChiHash().Add(int.MinValue).Hash);
        Assert.Equal(-1846084325, new ChiHash().Add(int.MaxValue).Hash);
        Assert.Equal(150996269, new ChiHash().Add(uint.MinValue).Hash);
        Assert.Equal(1504591717, new ChiHash().Add(uint.MaxValue).Hash);
        Assert.Equal(-1426660308, new ChiHash().Add(long.MinValue).Hash);
        Assert.Equal(1649257790, new ChiHash().Add(long.MaxValue).Hash);
        Assert.Equal(-154722900, new ChiHash().Add(ulong.MinValue).Hash);
        Assert.Equal(861868037, new ChiHash().Add(ulong.MaxValue).Hash);

        Assert.Equal(-1541255506, new ChiHash().Add(decimal.MinValue).Hash);
        Assert.Equal(-1284415288, new ChiHash().Add(decimal.MaxValue).Hash);
        Assert.Equal(44345189, new ChiHash().Add(decimal.Zero).Hash);
        Assert.Equal(-303449875, new ChiHash().Add(decimal.One).Hash);
        Assert.Equal(1567788007, new ChiHash().Add(decimal.MinusOne).Hash);
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