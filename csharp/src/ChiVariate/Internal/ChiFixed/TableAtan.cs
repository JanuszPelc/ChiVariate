using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

internal static class TableAtan
{
    private const int TableBits = 12;
    private const int TableSize = 1 << TableBits;

    private static readonly long[] AtanTable;
    private static readonly long HalfPi;
    private static readonly long Pi;

    static TableAtan()
    {
        HalfPi = CordicTables.HalfPi;
        Pi = CordicTables.Pi;

        AtanTable = new long[TableSize + 1];
        for (var i = 0; i <= TableSize; i++)
        {
            var x = (double)i / TableSize;
            AtanTable[i] = (long)(Math.Atan(x) * ChiVariate.ChiFixed.ScaleFactor);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Atan(long valueRaw)
    {
        if (valueRaw == 0)
            return 0;

        var negative = valueRaw < 0;
        if (negative)
            valueRaw = -valueRaw;

        long result;
        if (valueRaw <= ChiVariate.ChiFixed.ScaleFactor)
        {
            var index = (int)(valueRaw * TableSize / ChiVariate.ChiFixed.ScaleFactor);
            if (index > TableSize) index = TableSize;
            result = AtanTable[index];
        }
        else
        {
            var reciprocal = FixedMath.Div(ChiVariate.ChiFixed.ScaleFactor, valueRaw);
            var index = (int)(reciprocal * TableSize / ChiVariate.ChiFixed.ScaleFactor);
            if (index > TableSize) index = TableSize;
            result = HalfPi - AtanTable[index];
        }

        return negative ? -result : result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Atan2(long yRaw, long xRaw)
    {
        if (xRaw == 0)
        {
            if (yRaw == 0)
                return 0;
            return yRaw > 0 ? HalfPi : -HalfPi;
        }

        if (yRaw == 0)
            return xRaw > 0 ? 0 : Pi;

        if (xRaw > 0) return AtanRatio(yRaw, xRaw);

        var baseAngle = AtanRatio(yRaw, -xRaw);
        return yRaw >= 0 ? Pi - baseAngle : -Pi - baseAngle;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long AtanRatio(long yRaw, long xRaw)
    {
        var negative = yRaw < 0;
        if (negative)
            yRaw = -yRaw;

        long result;
        if (yRaw <= xRaw)
        {
            var ratio = FixedMath.Div(yRaw, xRaw);
            var index = (int)(ratio * TableSize / ChiVariate.ChiFixed.ScaleFactor);
            if (index > TableSize) index = TableSize;
            result = AtanTable[index];
        }
        else
        {
            var ratio = FixedMath.Div(xRaw, yRaw);
            var index = (int)(ratio * TableSize / ChiVariate.ChiFixed.ScaleFactor);
            if (index > TableSize) index = TableSize;
            result = HalfPi - AtanTable[index];
        }

        return negative ? -result : result;
    }
}