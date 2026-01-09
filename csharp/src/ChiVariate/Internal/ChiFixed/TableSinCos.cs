using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

internal static class TableSinCos
{
    private const int TableBits = 12;
    private const int TableSize = 1 << TableBits;

    private static readonly long[] SinTable;
    private static readonly long HalfPi;
    private static readonly long Pi;
    private static readonly long ThreeHalfPi;
    private static readonly long TwoPi;

    static TableSinCos()
    {
        HalfPi = CordicTables.HalfPi;
        Pi = CordicTables.Pi;
        ThreeHalfPi = HalfPi + Pi;
        TwoPi = CordicTables.TwoPi;

        SinTable = new long[TableSize + 1];
        for (var i = 0; i <= TableSize; i++)
        {
            var angle = (double)i / TableSize * Math.PI / 2.0;
            SinTable[i] = (long)(Math.Sin(angle) * ChiVariate.ChiFixed.ScaleFactor);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (long sin, long cos) SinCos(long angleRaw)
    {
        angleRaw %= TwoPi;
        if (angleRaw < 0)
            angleRaw += TwoPi;

        long sin, cos;
        int index;

        if (angleRaw < HalfPi)
        {
            index = (int)(angleRaw * TableSize / HalfPi);
            if (index > TableSize) index = TableSize;
            sin = SinTable[index];
            cos = SinTable[TableSize - index];
        }
        else if (angleRaw < Pi)
        {
            index = (int)((Pi - angleRaw) * TableSize / HalfPi);
            if (index > TableSize) index = TableSize;
            sin = SinTable[index];
            cos = -SinTable[TableSize - index];
        }
        else if (angleRaw < ThreeHalfPi)
        {
            index = (int)((angleRaw - Pi) * TableSize / HalfPi);
            if (index > TableSize) index = TableSize;
            sin = -SinTable[index];
            cos = -SinTable[TableSize - index];
        }
        else
        {
            index = (int)((TwoPi - angleRaw) * TableSize / HalfPi);
            if (index > TableSize) index = TableSize;
            sin = -SinTable[index];
            cos = SinTable[TableSize - index];
        }

        return (sin, cos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Sin(long angleRaw)
    {
        var (sin, _) = SinCos(angleRaw);
        return sin;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Cos(long angleRaw)
    {
        var (_, cos) = SinCos(angleRaw);
        return cos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Tan(long angleRaw)
    {
        var (sin, cos) = SinCos(angleRaw);

        if (cos == 0)
            throw new DivideByZeroException("Tangent is undefined at this value (cos = 0).");

        return FixedMath.Div(sin, cos);
    }
}