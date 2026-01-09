using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

internal static class CordicAtan
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Atan(long valueRaw)
    {
        if (valueRaw == 0)
            return 0;

        const long x = ChiVariate.ChiFixed.ScaleFactor;
        return AtanCore(x, valueRaw);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long AtanCore(long x, long y)
    {
        var z = 0L;

        for (var i = 0; i < CordicTables.Iterations; i++)
        {
            var dx = y >> i;
            var dy = x >> i;
            var dz = CordicTables.AtanTable[i];

            if (y >= 0)
            {
                x += dx;
                y -= dy;
                z += dz;
            }
            else
            {
                x -= dx;
                y += dy;
                z -= dz;
            }
        }

        return z;
    }
}