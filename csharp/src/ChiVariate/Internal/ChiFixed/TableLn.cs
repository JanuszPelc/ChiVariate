using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

internal static class TableLn
{
    private const int TableBits = 14;
    private const int TableSize = 1 << TableBits;
    private const long One = ChiVariate.ChiFixed.ScaleFactor;
    private const long Two = One << 1;

    private static readonly long Ln2Raw;
    private static readonly long[] LnTable;

    static TableLn()
    {
        Ln2Raw = FixedMath.FromDecimal(0.69314718055994530941723212146m);

        LnTable = new long[TableSize + 1];
        for (var i = 0; i <= TableSize; i++)
        {
            var x = 1m + (decimal)i / TableSize;
            var ln = DecimalLn(x);
            LnTable[i] = FixedMath.FromDecimal(ln);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Ln(long xRaw)
    {
        switch (xRaw)
        {
            case <= 0:
                throw new ArgumentException("Logarithm is only defined for positive values.");
            case One:
                return 0;
        }

        var scale = 0;

        while (xRaw >= Two)
        {
            xRaw >>= 1;
            scale++;
        }

        while (xRaw < One)
        {
            xRaw <<= 1;
            scale--;
        }

        var fractional = xRaw - One;
        var index = (int)(fractional >> (ChiVariate.ChiFixed.FractionalBits - TableBits));
        var remainder = fractional & ((1L << (ChiVariate.ChiFixed.FractionalBits - TableBits)) - 1);

        var lnLow = LnTable[index];
        var lnHigh = LnTable[index + 1];
        var lnInterpolated =
            lnLow + (long)(((Int128)(lnHigh - lnLow) * remainder) >> (ChiVariate.ChiFixed.FractionalBits - TableBits));

        return lnInterpolated + scale * Ln2Raw;
    }

    private static decimal DecimalLn(decimal x)
    {
        if (x <= 0m)
            throw new ArgumentException("Logarithm is only defined for positive values.");

        const decimal e = 2.71828182845904523536028747135m;
        var scale = 0;

        while (x > 1.5m)
        {
            x /= e;
            scale++;
        }

        while (x < 0.5m)
        {
            x *= e;
            scale--;
        }

        var u = x - 1m;
        var term = u;
        var sum = u;

        for (var n = 2; n <= 100; n++)
        {
            term *= -u;
            var delta = term / n;
            if (Math.Abs(delta) < 1e-28m)
                break;
            sum += delta;
        }

        return sum + scale;
    }
}