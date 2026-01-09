namespace ChiVariate.Internal.ChiFixed;

internal static class FixedMagnitude
{
    public static long MaxMagnitude(long x, long y)
    {
        var absX = Abs(x);
        var absY = Abs(y);

        if (absX > absY) return x;
        if (absY > absX) return y;

        return x >= 0 ? x : y;
    }

    public static long MaxMagnitudeNumber(long x, long y)
    {
        return MaxMagnitude(x, y);
    }

    public static long MinMagnitude(long x, long y)
    {
        var absX = Abs(x);
        var absY = Abs(y);

        if (absX < absY) return x;
        if (absY < absX) return y;

        return x < 0 ? x : y;
    }

    public static long MinMagnitudeNumber(long x, long y)
    {
        return MinMagnitude(x, y);
    }

    private static long Abs(long raw)
    {
        if (raw < 0) return -raw;
        return raw;
    }
}