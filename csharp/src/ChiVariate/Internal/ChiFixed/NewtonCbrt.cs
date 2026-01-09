using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

internal static class NewtonCbrt
{
    private const int TableBits = 10;
    private const int TableSize = 1 << TableBits;
    private static readonly long[] CbrtTable = CreateCbrtTable();

    private static long[] CreateCbrtTable()
    {
        var table = new long[TableSize * 3];
        for (var i = 0; i < TableSize; i++)
        {
            var input0 = 1.0 + (double)i / TableSize;
            table[i] = (long)(Math.Cbrt(input0) * ChiVariate.ChiFixed.ScaleFactor);

            var input1 = 2.0 + 2.0 * i / TableSize;
            table[TableSize + i] = (long)(Math.Cbrt(input1) * ChiVariate.ChiFixed.ScaleFactor);

            var input2 = 4.0 + 4.0 * i / TableSize;
            table[2 * TableSize + i] = (long)(Math.Cbrt(input2) * ChiVariate.ChiFixed.ScaleFactor);
        }

        return table;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Cbrt(long valueRaw)
    {
        if (valueRaw == 0)
            return 0;

        var negative = valueRaw < 0;
        if (negative)
            valueRaw = -valueRaw;

        var highBit = 63 - (int)ulong.LeadingZeroCount((ulong)valueRaw);

        var expOffset = highBit - ChiVariate.ChiFixed.FractionalBits;
        var remainder = (expOffset % 3 + 3) % 3;
        var tableOffset = remainder * TableSize;

        var shiftForIndex = highBit - TableBits;
        var index = shiftForIndex >= 0
            ? (int)((valueRaw >> shiftForIndex) & (TableSize - 1))
            : (int)((valueRaw << -shiftForIndex) & (TableSize - 1));

        var cbrtNormalized = CbrtTable[tableOffset + index];

        var resultShift = (expOffset - remainder) / 3;
        var result = resultShift >= 0
            ? cbrtNormalized << resultShift
            : cbrtNormalized >> -resultShift;

        return negative ? -result : result;
    }
}