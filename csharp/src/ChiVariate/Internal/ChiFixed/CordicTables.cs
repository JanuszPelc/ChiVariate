using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

internal static class CordicTables
{
    internal const int Iterations = 48;

    private const decimal DecimalPi = 3.1415926535897932384626433833m;
    private const decimal DecimalEpsilon = 1e-28m;

    internal static readonly long[] AtanTable;
    internal static readonly long K;
    internal static readonly long Pi;
    internal static readonly long HalfPi;
    internal static readonly long TwoPi;

    static CordicTables()
    {
        AtanTable = new long[Iterations];

        var k = 1m;
        for (var i = 0; i < Iterations; i++)
        {
            var powerOf2 = 1m / (1L << i);
            var atan = DecimalAtan(powerOf2);
            AtanTable[i] = FixedMath.FromDecimal(atan);
            k *= DecimalCos(atan);
        }

        K = FixedMath.FromDecimal(k);
        Pi = FixedMath.FromDecimal(DecimalPi);
        HalfPi = FixedMath.FromDecimal(DecimalPi / 2m);
        TwoPi = FixedMath.FromDecimal(DecimalPi * 2m);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static decimal DecimalAtan(decimal x)
    {
        switch (x)
        {
            case 0m:
                return 0m;
            case 1m:
                return DecimalPi / 4m;
            case -1m:
                return -DecimalPi / 4m;
            case > 1m:
                return DecimalPi / 2m - DecimalAtan(1m / x);
            case < -1m:
                return -DecimalPi / 2m - DecimalAtan(1m / x);
        }

        var result = x;
        var term = x;
        var xSquared = x * x;

        for (var n = 1; n <= 100 && Math.Abs(term) > DecimalEpsilon; n++)
        {
            term *= -xSquared;
            result += term / (2m * n + 1m);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static decimal DecimalCos(decimal x)
    {
        var result = 1m;
        var term = 1m;
        var xSquared = x * x;

        for (var n = 1; n <= 50 && Math.Abs(term) > DecimalEpsilon; n++)
        {
            term *= -xSquared / ((2 * n - 1) * 2 * n);
            result += term;
        }

        return result;
    }
}