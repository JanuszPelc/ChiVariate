using System.Runtime.CompilerServices;

namespace ChiVariate.Internal.ChiFixed;

internal static class Pow10Round
{
    private static readonly long[] Pow10Table =
    [
        1L,
        10L,
        100L,
        1_000L,
        10_000L,
        100_000L,
        1_000_000L,
        10_000_000L,
        100_000_000L,
        1_000_000_000L,
        10_000_000_000L,
        100_000_000_000L,
        1_000_000_000_000L,
        10_000_000_000_000L,
        100_000_000_000_000L,
        1_000_000_000_000_000L,
        10_000_000_000_000_000L,
        100_000_000_000_000_000L,
        1_000_000_000_000_000_000L
    ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Round(long raw, int digits, MidpointRounding mode)
    {
        if (raw == 0) return 0;

        if (digits == 0)
            return RoundToInteger(raw, mode);

        if (digits < 0 || digits >= Pow10Table.Length)
            throw new ArgumentOutOfRangeException(nameof(digits));

        var negative = raw < 0;
        var absRaw = negative ? -raw : raw;
        if (absRaw < 0) return raw;

        var pow10 = Pow10Table[digits];

        var scaled = (Int128)absRaw * pow10;
        var quotient = (long)(scaled / ChiVariate.ChiFixed.ScaleFactor);
        var remainder = scaled % ChiVariate.ChiFixed.ScaleFactor;

        var rounded = ApplyRounding(quotient, remainder, mode, negative);

        var result = (Int128)rounded * ChiVariate.ChiFixed.ScaleFactor / pow10;
        if (result > long.MaxValue)
            return negative ? ChiVariate.ChiFixed.NegativeInfinity.Raw : ChiVariate.ChiFixed.PositiveInfinity.Raw;

        var resultRaw = (long)result;
        return negative ? -resultRaw : resultRaw;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long RoundToInteger(long raw, MidpointRounding mode)
    {
        if (mode == MidpointRounding.AwayFromZero)
            return RoundToIntegerAwayFromZero(raw);

        const long fracMask = ChiVariate.ChiFixed.ScaleFactor - 1;
        const long half = ChiVariate.ChiFixed.ScaleFactor / 2;
        const long maxInteger = long.MaxValue & ~fracMask;

        var negative = raw < 0;
        var absRaw = negative ? -raw : raw;
        if (absRaw < 0) return raw;

        var frac = absRaw & fracMask;
        var integer = absRaw & ~fracMask;

        if (frac != 0)
        {
            var roundUp = mode switch
            {
                MidpointRounding.ToEven => frac > half ||
                                           (frac == half && ((integer >> ChiVariate.ChiFixed.FractionalBits) & 1) != 0),
                MidpointRounding.ToZero => false,
                MidpointRounding.ToNegativeInfinity => negative,
                MidpointRounding.ToPositiveInfinity => !negative,
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };

            if (roundUp)
            {
                if (integer >= maxInteger)
                    return raw;
                integer += ChiVariate.ChiFixed.ScaleFactor;
            }
        }

        return negative ? -integer : integer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long RoundToIntegerAwayFromZero(long raw)
    {
        const long fracMask = ChiVariate.ChiFixed.ScaleFactor - 1;
        const long half = ChiVariate.ChiFixed.ScaleFactor / 2;

        var sign = raw >> 63;
        var absRaw = (raw ^ sign) - sign;

        if (absRaw < 0) return raw;

        var rounded = (absRaw + half) & ~fracMask;
        return (rounded ^ sign) - sign;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ApplyRounding(long quotient, Int128 remainder, MidpointRounding mode, bool negative)
    {
        if (remainder == 0) return quotient;

        const long half = ChiVariate.ChiFixed.ScaleFactor / 2;

        return mode switch
        {
            MidpointRounding.ToEven => remainder > half ? quotient + 1 :
                remainder < half ? quotient :
                (quotient & 1) == 0 ? quotient : quotient + 1,
            MidpointRounding.AwayFromZero => remainder >= half ? quotient + 1 : quotient,
            MidpointRounding.ToZero => quotient,
            MidpointRounding.ToNegativeInfinity => negative ? quotient + 1 : quotient,
            MidpointRounding.ToPositiveInfinity => negative ? quotient : quotient + 1,
            _ => throw new ArgumentOutOfRangeException(nameof(mode))
        };
    }
}