using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace ChiVariate.Internal.ChiFixed;

internal static class FixedConversion
{
    public static bool TryConvertFromChecked<TOther>(TOther value, out long result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(byte))
        {
            result = FixedMath.FromDecimal((byte)(object)value);
            return true;
        }

        if (typeof(TOther) == typeof(sbyte))
        {
            result = FixedMath.FromDecimal((sbyte)(object)value);
            return true;
        }

        if (typeof(TOther) == typeof(short))
        {
            result = FixedMath.FromDecimal((short)(object)value);
            return true;
        }

        if (typeof(TOther) == typeof(ushort))
        {
            result = FixedMath.FromDecimal((ushort)(object)value);
            return true;
        }

        if (typeof(TOther) == typeof(int))
        {
            result = FixedMath.FromDecimal((int)(object)value);
            return true;
        }

        if (typeof(TOther) == typeof(uint))
        {
            result = FixedMath.FromDecimal((uint)(object)value);
            return true;
        }

        if (typeof(TOther) == typeof(long))
        {
            var longValue = (long)(object)value;
            if (longValue is < int.MinValue or > int.MaxValue)
            {
                result = 0;
                return false;
            }

            result = FixedMath.FromDecimal(longValue);
            return true;
        }

        if (typeof(TOther) == typeof(ulong))
        {
            var ulongValue = (ulong)(object)value;
            if (ulongValue > int.MaxValue)
            {
                result = 0;
                return false;
            }

            result = FixedMath.FromDecimal(ulongValue);
            return true;
        }

        if (typeof(TOther) == typeof(float))
        {
            var floatValue = (float)(object)value;
            if (float.IsNaN(floatValue) || float.IsInfinity(floatValue))
            {
                result = 0;
                return false;
            }

            result = FixedMath.FromDecimal((decimal)floatValue);
            return true;
        }

        if (typeof(TOther) == typeof(double))
        {
            var doubleValue = (double)(object)value;
            if (double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
            {
                result = 0;
                return false;
            }

            result = FixedMath.FromDecimal((decimal)doubleValue);
            return true;
        }

        if (typeof(TOther) == typeof(decimal))
        {
            result = FixedMath.FromDecimal((decimal)(object)value);
            return true;
        }

        if (typeof(TOther) == typeof(Half))
        {
            var halfValue = (Half)(object)value;
            if (Half.IsNaN(halfValue) || Half.IsInfinity(halfValue))
            {
                result = 0;
                return false;
            }

            result = FixedMath.FromDecimal((decimal)halfValue);
            return true;
        }

        if (typeof(TOther) == typeof(ChiVariate.ChiFixed))
        {
            result = ((ChiVariate.ChiFixed)(object)value).Raw;
            return true;
        }

        result = 0;
        return false;
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, out long result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(long))
        {
            var longValue = (long)(object)value;
            switch (longValue)
            {
                case < int.MinValue:
                    result = ChiVariate.ChiFixed.MinValue.Raw;
                    return true;
                case > int.MaxValue:
                    result = ChiVariate.ChiFixed.MaxValue.Raw;
                    return true;
                default:
                    result = FixedMath.FromDecimal(longValue);
                    return true;
            }
        }

        if (typeof(TOther) == typeof(ulong))
        {
            var ulongValue = (ulong)(object)value;
            if (ulongValue > int.MaxValue)
            {
                result = ChiVariate.ChiFixed.MaxValue.Raw;
                return true;
            }

            result = FixedMath.FromDecimal(ulongValue);
            return true;
        }

        return TryConvertFromChecked(value, out result);
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, out long result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(long))
        {
            var longValue = (long)(object)value;
            result = longValue << ChiVariate.ChiFixed.FractionalBits;
            return true;
        }

        if (typeof(TOther) == typeof(ulong))
        {
            var ulongValue = (ulong)(object)value;
            result = (long)(ulongValue << ChiVariate.ChiFixed.FractionalBits);
            return true;
        }

        return TryConvertFromChecked(value, out result);
    }

    public static bool TryConvertToChecked<TOther>(long raw, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(byte))
        {
            const long byteMaxRaw = (long)byte.MaxValue << ChiVariate.ChiFixed.FractionalBits;
            if (raw is < 0 or > byteMaxRaw)
            {
                result = default;
                return false;
            }

            result = (TOther)(object)(byte)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        if (typeof(TOther) == typeof(sbyte))
        {
            const long sbyteMinRaw = (long)sbyte.MinValue << ChiVariate.ChiFixed.FractionalBits;
            const long sbyteMaxRaw = (long)sbyte.MaxValue << ChiVariate.ChiFixed.FractionalBits;
            if (raw is < sbyteMinRaw or > sbyteMaxRaw)
            {
                result = default;
                return false;
            }

            result = (TOther)(object)(sbyte)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        if (typeof(TOther) == typeof(short))
        {
            const long shortMinRaw = (long)short.MinValue << ChiVariate.ChiFixed.FractionalBits;
            const long shortMaxRaw = (long)short.MaxValue << ChiVariate.ChiFixed.FractionalBits;
            if (raw is < shortMinRaw or > shortMaxRaw)
            {
                result = default;
                return false;
            }

            result = (TOther)(object)(short)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        if (typeof(TOther) == typeof(ushort))
        {
            const long ushortMaxRaw = (long)ushort.MaxValue << ChiVariate.ChiFixed.FractionalBits;
            if (raw is < 0 or > ushortMaxRaw)
            {
                result = default;
                return false;
            }

            result = (TOther)(object)(ushort)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        if (typeof(TOther) == typeof(int))
        {
            var value = Math.Round(FixedMath.ToDecimal(raw));
            if (value is < int.MinValue or > int.MaxValue)
            {
                result = default;
                return false;
            }

            result = (TOther)(object)(int)value;
            return true;
        }

        if (typeof(TOther) == typeof(uint))
        {
            if (raw < 0)
            {
                result = default;
                return false;
            }

            var value = Math.Round(FixedMath.ToDecimal(raw));
            if (value > uint.MaxValue)
            {
                result = default;
                return false;
            }

            result = (TOther)(object)(uint)value;
            return true;
        }

        if (typeof(TOther) == typeof(long))
        {
            result = (TOther)(object)(long)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        if (typeof(TOther) == typeof(ulong))
        {
            if (raw < 0)
            {
                result = default;
                return false;
            }

            result = (TOther)(object)(ulong)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        if (typeof(TOther) == typeof(float))
        {
            result = (TOther)(object)(float)FixedMath.ToDecimal(raw);
            return true;
        }

        if (typeof(TOther) == typeof(double))
        {
            result = (TOther)(object)(double)FixedMath.ToDecimal(raw);
            return true;
        }

        if (typeof(TOther) == typeof(decimal))
        {
            result = (TOther)(object)FixedMath.ToDecimal(raw);
            return true;
        }

        if (typeof(TOther) == typeof(Half))
        {
            result = (TOther)(object)(Half)FixedMath.ToDecimal(raw);
            return true;
        }

        if (typeof(TOther) == typeof(ChiVariate.ChiFixed))
        {
            result = (TOther)(object)new ChiVariate.ChiFixed(raw);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryConvertToSaturating<TOther>(long raw, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(byte))
        {
            if (raw < 0)
            {
                result = (TOther)(object)(byte)0;
                return true;
            }

            const long byteMaxRaw = (long)byte.MaxValue << ChiVariate.ChiFixed.FractionalBits;
            if (raw > byteMaxRaw)
            {
                result = (TOther)(object)byte.MaxValue;
                return true;
            }

            result = (TOther)(object)(byte)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        if (typeof(TOther) == typeof(sbyte))
        {
            const long sbyteMinRaw = (long)sbyte.MinValue << ChiVariate.ChiFixed.FractionalBits;
            const long sbyteMaxRaw = (long)sbyte.MaxValue << ChiVariate.ChiFixed.FractionalBits;

            switch (raw)
            {
                case < sbyteMinRaw:
                    result = (TOther)(object)sbyte.MinValue;
                    return true;
                case > sbyteMaxRaw:
                    result = (TOther)(object)sbyte.MaxValue;
                    return true;
                default:
                    result = (TOther)(object)(sbyte)Math.Round(FixedMath.ToDecimal(raw));
                    return true;
            }
        }

        if (typeof(TOther) == typeof(short))
        {
            const long shortMinRaw = (long)short.MinValue << ChiVariate.ChiFixed.FractionalBits;
            const long shortMaxRaw = (long)short.MaxValue << ChiVariate.ChiFixed.FractionalBits;

            switch (raw)
            {
                case < shortMinRaw:
                    result = (TOther)(object)short.MinValue;
                    return true;
                case > shortMaxRaw:
                    result = (TOther)(object)short.MaxValue;
                    return true;
                default:
                    result = (TOther)(object)(short)Math.Round(FixedMath.ToDecimal(raw));
                    return true;
            }
        }

        if (typeof(TOther) == typeof(ushort))
        {
            if (raw < 0)
            {
                result = (TOther)(object)(ushort)0;
                return true;
            }

            const long ushortMaxRaw = (long)ushort.MaxValue << ChiVariate.ChiFixed.FractionalBits;
            if (raw > ushortMaxRaw)
            {
                result = (TOther)(object)ushort.MaxValue;
                return true;
            }

            result = (TOther)(object)(ushort)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        if (typeof(TOther) == typeof(int))
        {
            var value = Math.Round(FixedMath.ToDecimal(raw));
            switch (value)
            {
                case < int.MinValue:
                    result = (TOther)(object)int.MinValue;
                    return true;
                case > int.MaxValue:
                    result = (TOther)(object)int.MaxValue;
                    return true;
                default:
                    result = (TOther)(object)(int)value;
                    return true;
            }
        }

        if (typeof(TOther) == typeof(uint))
        {
            if (raw < 0)
            {
                result = (TOther)(object)0u;
                return true;
            }

            var value = Math.Round(FixedMath.ToDecimal(raw));
            if (value > uint.MaxValue)
            {
                result = (TOther)(object)uint.MaxValue;
                return true;
            }

            result = (TOther)(object)(uint)value;
            return true;
        }

        if (typeof(TOther) == typeof(long))
        {
            result = (TOther)(object)(long)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        if (typeof(TOther) == typeof(ulong))
        {
            if (raw < 0)
            {
                result = (TOther)(object)0UL;
                return true;
            }

            result = (TOther)(object)(ulong)Math.Round(FixedMath.ToDecimal(raw));
            return true;
        }

        return TryConvertToChecked(raw, out result);
    }

    public static bool TryConvertToTruncating<TOther>(long raw, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        if (typeof(TOther) == typeof(byte))
        {
            var rounded = (long)Math.Round(FixedMath.ToDecimal(raw));
            unchecked
            {
                result = (TOther)(object)(byte)rounded;
            }

            return true;
        }

        if (typeof(TOther) == typeof(sbyte))
        {
            var rounded = (long)Math.Round(FixedMath.ToDecimal(raw));
            unchecked
            {
                result = (TOther)(object)(sbyte)rounded;
            }

            return true;
        }

        if (typeof(TOther) == typeof(short))
        {
            result = (TOther)(object)(short)Math.Round(FixedMath.ToDecimal(raw));

            return true;
        }

        if (typeof(TOther) == typeof(ushort))
        {
            result = (TOther)(object)(ushort)Math.Round(FixedMath.ToDecimal(raw));

            return true;
        }

        if (typeof(TOther) == typeof(int))
        {
            result = (TOther)(object)(int)Math.Round(FixedMath.ToDecimal(raw));

            return true;
        }

        if (typeof(TOther) == typeof(uint))
        {
            result = (TOther)(object)(uint)Math.Round(FixedMath.ToDecimal(raw));

            return true;
        }

        if (typeof(TOther) == typeof(long))
        {
            result = (TOther)(object)(long)Math.Round(FixedMath.ToDecimal(raw));

            return true;
        }

        if (typeof(TOther) == typeof(ulong))
        {
            result = (TOther)(object)(ulong)Math.Round(FixedMath.ToDecimal(raw));

            return true;
        }

        return TryConvertToChecked(raw, out result);
    }
}