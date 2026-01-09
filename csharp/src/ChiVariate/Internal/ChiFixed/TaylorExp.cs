using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

internal static class TaylorExp
{
    private const int TableBits = 10;
    private const int TableSize = 1 << TableBits;

    private static readonly long[] EPowers;
    private static readonly long[] ExpFracTable;

    static TaylorExp()
    {
        var eRaw = FixedMath.FromDecimal(2.71828182845904523536028747135m);

        EPowers = new long[28];
        EPowers[0] = ChiVariate.ChiFixed.ScaleFactor;
        EPowers[1] = eRaw;

        for (var i = 2; i < EPowers.Length; i++)
        {
            var product = FixedMath.Mul(EPowers[i - 1], eRaw);
            if (product > long.MaxValue / 2)
                EPowers[i] = long.MaxValue;
            else
                EPowers[i] = product;
        }

        ExpFracTable = new long[TableSize];
        for (var i = 0; i < TableSize; i++)
        {
            var x = (double)i / TableSize;
            ExpFracTable[i] = (long)(Math.Exp(x) * ChiVariate.ChiFixed.ScaleFactor);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Exp(long xRaw)
    {
        if (xRaw == 0)
            return ChiVariate.ChiFixed.ScaleFactor;

        var negative = xRaw < 0;
        if (negative)
            xRaw = -xRaw;

        var intPart = (int)(xRaw >> ChiVariate.ChiFixed.FractionalBits);
        var fracPart = xRaw & ((1L << ChiVariate.ChiFixed.FractionalBits) - 1);

        if (intPart >= EPowers.Length)
            return negative ? 0 : long.MaxValue;

        var intExp = EPowers[intPart];

        if (fracPart == 0)
            return negative ? Reciprocal(intExp) : intExp;

        var fracIndex = (int)(fracPart >> (ChiVariate.ChiFixed.FractionalBits - TableBits));
        var fracExp = ExpFracTable[fracIndex];

        var result = FixedMath.Mul(intExp, fracExp);

        return negative ? Reciprocal(result) : result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Reciprocal(long xRaw)
    {
        if (xRaw == 0)
            return long.MaxValue;

        return FixedMath.Div(ChiVariate.ChiFixed.ScaleFactor, xRaw);
    }
}