using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

internal static class NewtonSqrt
{
    private const int TableBits = 12;
    private const int TableSize = 1 << TableBits;
    private static readonly long[] SqrtTable = CreateSqrtTable();

    private static long[] CreateSqrtTable()
    {
        var table = new long[TableSize * 2];
        for (var i = 0; i < TableSize; i++)
        {
            var evenInput = 1.0 + (double)i / TableSize;
            table[i] = (long)(Math.Sqrt(evenInput) * ChiVariate.ChiFixed.ScaleFactor);

            var oddInput = 2.0 + 2.0 * i / TableSize;
            table[TableSize + i] = (long)(Math.Sqrt(oddInput) * ChiVariate.ChiFixed.ScaleFactor);
        }

        return table;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Sqrt(long valueRaw)
    {
        if (valueRaw <= 0)
        {
            if (valueRaw < 0)
                throw new ArgumentException("Cannot compute square root of negative number.");
            return 0;
        }

        var highBit = 63 - (int)ulong.LeadingZeroCount((ulong)valueRaw);

        var isOddExponent = ((highBit ^ ChiVariate.ChiFixed.FractionalBits) & 1) != 0;
        var tableOffset = isOddExponent ? TableSize : 0;

        var shiftForIndex = highBit - TableBits;
        var index = shiftForIndex >= 0
            ? (int)((valueRaw >> shiftForIndex) & (TableSize - 1))
            : (int)((valueRaw << -shiftForIndex) & (TableSize - 1));

        var sqrtNormalized = SqrtTable[tableOffset + index];

        var resultShift = (highBit - ChiVariate.ChiFixed.FractionalBits) >> 1;
        return resultShift >= 0
            ? sqrtNormalized << resultShift
            : sqrtNormalized >> -resultShift;
    }
}