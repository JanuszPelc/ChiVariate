using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Internal.ChiFixed;

namespace ChiVariate;

/// <summary>
///     A Q21.42 fixed-point numeric type providing deterministic cross-platform arithmetic.
/// </summary>
/// <param name="raw">The raw 64-bit integer representation of the fixed-point value.</param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct ChiFixed(long raw) : IFloatingPointIeee754<ChiFixed>, IMinMaxValue<ChiFixed>
{
    #region Raw Storage

    /// <summary>
    ///     The raw 64-bit integer representation. The actual value equals Raw / 2^42.
    /// </summary>
    public readonly long Raw = raw;

    #endregion

    #region Constants & Configuration

    /// <summary>
    ///     The number of bits used for the fractional part (42 bits).
    /// </summary>
    public const int FractionalBits = 42;

    /// <summary>
    ///     The scaling factor (2^42) used to convert between raw and actual values.
    /// </summary>
    public const long ScaleFactor = 1L << FractionalBits;

    /// <summary>
    ///     The maximum number of decimal digits needed to represent the fractional part.
    /// </summary>
    public static readonly int FormatFractionalDigits = (int)Math.Ceiling((FractionalBits + 1) * Math.Log10(2.0));

    #endregion

    #region Special Values

    /// <inheritdoc />
    public static ChiFixed Zero { get; } = new(0);

    /// <inheritdoc />
    public static ChiFixed One { get; } = new(ScaleFactor);

    /// <inheritdoc />
    public static ChiFixed NegativeOne { get; } = new(-ScaleFactor);

    /// <inheritdoc />
    public static ChiFixed Epsilon { get; } = new(1);

    /// <inheritdoc />
    public static ChiFixed MinValue { get; } = new(long.MinValue);

    /// <inheritdoc />
    public static ChiFixed MaxValue { get; } = new(long.MaxValue);

    /// <inheritdoc />
    public static ChiFixed NaN { get; } = Zero;

    /// <inheritdoc />
    public static ChiFixed NegativeZero { get; } = Zero;

    /// <inheritdoc />
    public static ChiFixed PositiveInfinity { get; } = MaxValue;

    /// <inheritdoc />
    public static ChiFixed NegativeInfinity { get; } = MinValue;

    /// <inheritdoc />
    public static ChiFixed E { get; } = (ChiFixed)2.7182818284590452353602874713m;

    /// <inheritdoc />
    public static ChiFixed Pi { get; } = (ChiFixed)3.1415926535897932384626433832m;

    /// <inheritdoc />
    public static ChiFixed Tau { get; } = (ChiFixed)6.2831853071795864769252867665m;

    /// <inheritdoc />
    public static ChiFixed Ln2 { get; } = (ChiFixed)0.69314718055994530941723212146m;

    /// <inheritdoc />
    public static ChiFixed Ln10 { get; } = (ChiFixed)2.30258509299404568401799145468m;

    /// <summary>
    ///     The value 2.
    /// </summary>
    public static ChiFixed Two { get; } = new(ScaleFactor << 1);

    /// <inheritdoc />
    public static ChiFixed AdditiveIdentity { get; } = Zero;

    /// <inheritdoc />
    public static ChiFixed MultiplicativeIdentity { get; } = One;

    /// <inheritdoc />
    public static int Radix { get; } = 2;

    #endregion

    #region Factory Methods

    /// <summary>
    ///     Converts a decimal value to ChiFixed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ChiFixed(decimal v)
    {
        return new ChiFixed(FixedMath.FromDecimal(v));
    }

    /// <summary>
    ///     Converts a ChiFixed value to decimal.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator decimal(ChiFixed v)
    {
        return FixedMath.ToDecimal(v.Raw);
    }

    /// <summary>
    ///     Converts a fraction (numerator, denominator) to ChiFixed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ChiFixed((int numerator, int denominator) v)
    {
        return (ChiFixed)v.numerator / (ChiFixed)v.denominator;
    }

    #endregion

    #region Arithmetic Operators

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed operator +(ChiFixed a, ChiFixed b)
    {
        return new ChiFixed(a.Raw + b.Raw);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed operator -(ChiFixed a, ChiFixed b)
    {
        return new ChiFixed(a.Raw - b.Raw);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed operator *(ChiFixed a, ChiFixed b)
    {
        return new ChiFixed(FixedMath.Mul(a.Raw, b.Raw));
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed operator /(ChiFixed a, ChiFixed b)
    {
        if (b.Raw == 0)
            throw new DivideByZeroException();

        return new ChiFixed(FixedMath.Div(a.Raw, b.Raw));
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed operator %(ChiFixed left, ChiFixed right)
    {
        if (right.Raw == 0)
            throw new DivideByZeroException();

        return new ChiFixed(FixedMath.Mod(left.Raw, right.Raw));
    }

    #endregion

    #region Comparison Operators

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ChiFixed a, ChiFixed b)
    {
        return a.Raw == b.Raw;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ChiFixed a, ChiFixed b)
    {
        return a.Raw != b.Raw;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(ChiFixed a, ChiFixed b)
    {
        return a.Raw > b.Raw;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(ChiFixed a, ChiFixed b)
    {
        return a.Raw < b.Raw;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(ChiFixed a, ChiFixed b)
    {
        return a.Raw >= b.Raw;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(ChiFixed a, ChiFixed b)
    {
        return a.Raw <= b.Raw;
    }

    #endregion

    #region System.Object Overrides

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is ChiFixed other && Raw == other.Raw;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Raw.GetHashCode();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return FixedFormatter.Format(Raw, "G", null);
    }

    #endregion

    #region IEquatable & IComparable

    /// <inheritdoc />
    public bool Equals(ChiFixed other)
    {
        return Raw == other.Raw;
    }

    /// <inheritdoc />
    public int CompareTo(ChiFixed other)
    {
        return Raw.CompareTo(other.Raw);
    }

    /// <inheritdoc />
    public int CompareTo(object? obj)
    {
        return obj is ChiFixed other ? Raw.CompareTo(other.Raw) : 0;
    }

    #endregion

    #region Formatting

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        return FixedFormatter.Format(Raw, format ?? "G", formatProvider);
    }

    /// <inheritdoc />
    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        return FixedFormatter.TryFormat(Raw, destination, out charsWritten, format, provider);
    }

    #endregion

    #region Parsing

    /// <inheritdoc />
    public static ChiFixed Parse(string s, IFormatProvider? provider = null)
    {
        return new ChiFixed(FixedParser.Parse(s, provider));
    }

    /// <inheritdoc />
    public static ChiFixed Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return new ChiFixed(FixedParser.Parse(s, provider));
    }

    /// <inheritdoc />
    public static ChiFixed Parse(string s, NumberStyles style, IFormatProvider? provider = null)
    {
        return new ChiFixed(FixedParser.Parse(s, style, provider));
    }

    /// <inheritdoc />
    public static ChiFixed Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        return new ChiFixed(FixedParser.Parse(s, style, provider));
    }

    /// <inheritdoc />
    public static bool TryParse(string? s, IFormatProvider? provider, out ChiFixed result)
    {
        var success = FixedParser.TryParse(s, provider, out var raw);
        result = success ? new ChiFixed(raw) : default;
        return success;
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ChiFixed result)
    {
        var success = FixedParser.TryParse(s, provider, out var raw);
        result = success ? new ChiFixed(raw) : default;
        return success;
    }

    /// <inheritdoc />
    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out ChiFixed result)
    {
        var success = FixedParser.TryParse(s, style, provider, out var raw);
        result = success ? new ChiFixed(raw) : default;
        return success;
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
        out ChiFixed result)
    {
        var success = FixedParser.TryParse(s, style, provider, out var raw);
        result = success ? new ChiFixed(raw) : default;
        return success;
    }

    #endregion

    #region Unary Operators

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed operator +(ChiFixed a)
    {
        return a;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed operator -(ChiFixed a)
    {
        return new ChiFixed(-a.Raw);
    }

    /// <inheritdoc />
    public static ChiFixed operator --(ChiFixed value)
    {
        return value - One;
    }

    /// <inheritdoc />
    public static ChiFixed operator ++(ChiFixed value)
    {
        return value + One;
    }

    #endregion

    #region INumberBase Classification

    /// <inheritdoc />
    public static bool IsCanonical(ChiFixed value)
    {
        return true;
    }

    /// <inheritdoc />
    public static bool IsComplexNumber(ChiFixed value)
    {
        return false;
    }

    /// <inheritdoc />
    public static bool IsEvenInteger(ChiFixed value)
    {
        return IsInteger(value) && ((value.Raw >> FractionalBits) & 1) == 0;
    }

    /// <inheritdoc />
    public static bool IsFinite(ChiFixed value)
    {
        return true;
    }

    /// <inheritdoc />
    public static bool IsImaginaryNumber(ChiFixed value)
    {
        return false;
    }

    /// <inheritdoc />
    public static bool IsInfinity(ChiFixed value)
    {
        return false;
    }

    /// <inheritdoc />
    public static bool IsInteger(ChiFixed value)
    {
        return (value.Raw & (ScaleFactor - 1)) == 0;
    }

    /// <inheritdoc />
    public static bool IsNaN(ChiFixed value)
    {
        return false;
    }

    /// <inheritdoc />
    public static bool IsNegative(ChiFixed value)
    {
        return value.Raw < 0;
    }

    /// <inheritdoc />
    public static bool IsNegativeInfinity(ChiFixed value)
    {
        return false;
    }

    /// <inheritdoc />
    public static bool IsNormal(ChiFixed value)
    {
        return value.Raw != 0;
    }

    /// <inheritdoc />
    public static bool IsOddInteger(ChiFixed value)
    {
        return IsInteger(value) && ((value.Raw >> FractionalBits) & 1) != 0;
    }

    /// <inheritdoc />
    public static bool IsPositive(ChiFixed value)
    {
        return value.Raw > 0;
    }

    /// <inheritdoc />
    public static bool IsPositiveInfinity(ChiFixed value)
    {
        return false;
    }

    /// <inheritdoc />
    public static bool IsRealNumber(ChiFixed value)
    {
        return true;
    }

    /// <inheritdoc />
    public static bool IsSubnormal(ChiFixed value)
    {
        return false;
    }

    /// <inheritdoc />
    public static bool IsZero(ChiFixed value)
    {
        return value.Raw == 0;
    }

    #endregion

    #region Magnitude

    /// <inheritdoc />
    public static ChiFixed MaxMagnitude(ChiFixed x, ChiFixed y)
    {
        return new ChiFixed(FixedMagnitude.MaxMagnitude(x.Raw, y.Raw));
    }

    /// <inheritdoc />
    public static ChiFixed MaxMagnitudeNumber(ChiFixed x, ChiFixed y)
    {
        return new ChiFixed(FixedMagnitude.MaxMagnitudeNumber(x.Raw, y.Raw));
    }

    /// <inheritdoc />
    public static ChiFixed MinMagnitude(ChiFixed x, ChiFixed y)
    {
        return new ChiFixed(FixedMagnitude.MinMagnitude(x.Raw, y.Raw));
    }

    /// <inheritdoc />
    public static ChiFixed MinMagnitudeNumber(ChiFixed x, ChiFixed y)
    {
        return new ChiFixed(FixedMagnitude.MinMagnitudeNumber(x.Raw, y.Raw));
    }

    #endregion

    #region Conversion

    /// <inheritdoc />
    public static bool TryConvertFromChecked<TOther>(TOther value, out ChiFixed result)
        where TOther : INumberBase<TOther>
    {
        var success = FixedConversion.TryConvertFromChecked(value, out var raw);
        result = success ? new ChiFixed(raw) : default;
        return success;
    }

    /// <inheritdoc />
    public static bool TryConvertFromSaturating<TOther>(TOther value, out ChiFixed result)
        where TOther : INumberBase<TOther>
    {
        var success = FixedConversion.TryConvertFromSaturating(value, out var raw);
        result = success ? new ChiFixed(raw) : default;
        return success;
    }

    /// <inheritdoc />
    public static bool TryConvertFromTruncating<TOther>(TOther value, out ChiFixed result)
        where TOther : INumberBase<TOther>
    {
        var success = FixedConversion.TryConvertFromTruncating(value, out var raw);
        result = success ? new ChiFixed(raw) : default;
        return success;
    }

    /// <inheritdoc />
    public static bool TryConvertToChecked<TOther>(ChiFixed value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        return FixedConversion.TryConvertToChecked(value.Raw, out result);
    }

    /// <inheritdoc />
    public static bool TryConvertToSaturating<TOther>(ChiFixed value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        return FixedConversion.TryConvertToSaturating(value.Raw, out result);
    }

    /// <inheritdoc />
    public static bool TryConvertToTruncating<TOther>(ChiFixed value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        return FixedConversion.TryConvertToTruncating(value.Raw, out result);
    }

    #endregion

    #region Numeric Functions

    /// <inheritdoc />
    public static ChiFixed Round(ChiFixed x, int digits, MidpointRounding mode)
    {
        return FixedMath.Round(x, digits, mode);
    }

    /// <inheritdoc />
    public static ChiFixed Round(ChiFixed x)
    {
        return Round(x, 0, MidpointRounding.AwayFromZero);
    }

    /// <inheritdoc />
    public static ChiFixed Floor(ChiFixed x)
    {
        return FixedMath.Floor(x);
    }

    /// <inheritdoc />
    public static ChiFixed Ceiling(ChiFixed x)
    {
        return FixedMath.Ceiling(x);
    }

    /// <inheritdoc />
    public static ChiFixed Truncate(ChiFixed x)
    {
        return FixedMath.Truncate(x);
    }

    /// <inheritdoc />
    public static ChiFixed Abs(ChiFixed value)
    {
        return value.Raw < 0 ? new ChiFixed(-value.Raw) : value;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed Lerp(ChiFixed value1, ChiFixed value2, ChiFixed amount)
    {
        return value1 + (value2 - value1) * amount;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed FusedMultiplyAdd(ChiFixed left, ChiFixed right, ChiFixed addend)
    {
        var product = (Int128)left.Raw * right.Raw;
        var addendScaled = (Int128)addend.Raw << FractionalBits;
        var result = (product + addendScaled) >> FractionalBits;
        return new ChiFixed((long)result);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed Ieee754Remainder(ChiFixed left, ChiFixed right)
    {
        if (right.Raw == 0)
            throw new DivideByZeroException();

        var quotient = left / right;
        var n = Round(quotient, 0, MidpointRounding.ToEven);
        return left - n * right;
    }

    #endregion

    #region Binary Representation

    /// <inheritdoc />
    public int GetExponentByteCount()
    {
        return 0;
    }

    /// <inheritdoc />
    public int GetExponentShortestBitLength()
    {
        return 0;
    }

    /// <inheritdoc />
    public int GetSignificandBitLength()
    {
        return 63;
    }

    /// <inheritdoc />
    public int GetSignificandByteCount()
    {
        return 8;
    }

    /// <inheritdoc />
    public bool TryWriteExponentBigEndian(Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;
        return false;
    }

    /// <inheritdoc />
    public bool TryWriteExponentLittleEndian(Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = 0;
        return false;
    }

    /// <inheritdoc />
    public bool TryWriteSignificandBigEndian(Span<byte> destination, out int bytesWritten)
    {
        if (destination.Length >= 8)
        {
            BinaryPrimitives.WriteInt64BigEndian(destination, Raw);
            bytesWritten = 8;
            return true;
        }

        bytesWritten = 0;
        return false;
    }

    /// <inheritdoc />
    public bool TryWriteSignificandLittleEndian(Span<byte> destination, out int bytesWritten)
    {
        if (destination.Length >= 8)
        {
            BinaryPrimitives.WriteInt64LittleEndian(destination, Raw);
            bytesWritten = 8;
            return true;
        }

        bytesWritten = 0;
        return false;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed BitDecrement(ChiFixed x)
    {
        return x.Raw == long.MinValue ? NegativeInfinity : new ChiFixed(x.Raw - 1);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed BitIncrement(ChiFixed x)
    {
        return x.Raw == long.MaxValue ? PositiveInfinity : new ChiFixed(x.Raw + 1);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed ScaleB(ChiFixed x, int n)
    {
        if (n >= 0)
        {
            if (n > 63)
                return x.Raw >= 0 ? PositiveInfinity : NegativeInfinity;

            var result = x.Raw << n;
            if (result >> n != x.Raw)
                return x.Raw >= 0 ? PositiveInfinity : NegativeInfinity;

            return new ChiFixed(result);
        }

        var shift = -n;
        if (shift > 63)
            return Zero;

        return new ChiFixed(x.Raw >> shift);
    }

    #endregion

    #region Power Functions

    /// <inheritdoc />
    public static ChiFixed Pow(ChiFixed x, ChiFixed y)
    {
        switch (y.Raw)
        {
            case 0:
                return One;
            case ScaleFactor:
                return x;
        }

        if (x.Raw == ScaleFactor)
            return One;

        var isNegativeBase = x.Raw < 0;
        const long fracMask = (1L << FractionalBits) - 1;
        var isIntegerExponent = (y.Raw & fracMask) == 0;

        if (!isIntegerExponent)
            return !isNegativeBase
                ? Exp(y * Log(x))
                : throw new ArgumentException("Cannot raise negative number to fractional power.");

        var exp = (int)(y.Raw >> FractionalBits);
        return PowInteger(x, exp);
    }

    private static ChiFixed PowInteger(ChiFixed baseVal, int exponent)
    {
        if (exponent < 0)
        {
            baseVal = One / baseVal;
            exponent = -exponent;
        }

        var result = One;
        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
                result *= baseVal;
            baseVal *= baseVal;
            exponent >>= 1;
        }

        return result;
    }

    #endregion

    #region Root Functions

    /// <inheritdoc />
    public static ChiFixed Sqrt(ChiFixed x)
    {
        return FixedMath.Sqrt(x);
    }

    /// <inheritdoc />
    public static ChiFixed Cbrt(ChiFixed x)
    {
        return FixedMath.Cbrt(x);
    }

    /// <inheritdoc />
    public static ChiFixed Hypot(ChiFixed x, ChiFixed y)
    {
        var ax = Abs(x);
        var ay = Abs(y);

        if (ax < ay) (ax, ay) = (ay, ax);
        if (ax.Raw == 0) return Zero;

        var ratio = ay / ax;
        return ax * Sqrt(One + ratio * ratio);
    }

    /// <inheritdoc />
    public static ChiFixed RootN(ChiFixed x, int n)
    {
        if (n == 0)
            throw new ArgumentException("Root degree cannot be zero.", nameof(n));

        if (x.Raw == 0)
            return Zero;

        if (n == 1)
            return x;

        var isNegative = x.Raw < 0;
        if (isNegative)
        {
            if ((n & 1) == 0)
                throw new ArgumentException("Cannot compute even root of negative number.");
            x = -x;
        }

        var result = Exp(Log(x) / (ChiFixed)n);
        return isNegative ? -result : result;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed ReciprocalEstimate(ChiFixed x)
    {
        return One / x;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed ReciprocalSqrtEstimate(ChiFixed x)
    {
        return One / Sqrt(x);
    }

    #endregion

    #region Trigonometric Functions

    /// <inheritdoc />
    public static ChiFixed Sin(ChiFixed x)
    {
        return FixedMath.Sin(x);
    }

    /// <inheritdoc />
    public static ChiFixed Cos(ChiFixed x)
    {
        return FixedMath.Cos(x);
    }

    /// <inheritdoc />
    public static ChiFixed Tan(ChiFixed x)
    {
        return FixedMath.Tan(x);
    }

    /// <inheritdoc />
    public static ChiFixed Asin(ChiFixed x)
    {
        return FixedMath.Asin(x);
    }

    /// <inheritdoc />
    public static ChiFixed Acos(ChiFixed x)
    {
        return FixedMath.Acos(x);
    }

    /// <inheritdoc />
    public static ChiFixed Atan(ChiFixed x)
    {
        return FixedMath.Atan(x);
    }

    /// <inheritdoc />
    public static ChiFixed SinPi(ChiFixed x)
    {
        return Sin(x * Pi);
    }

    /// <inheritdoc />
    public static ChiFixed CosPi(ChiFixed x)
    {
        return Cos(x * Pi);
    }

    /// <inheritdoc />
    public static ChiFixed TanPi(ChiFixed x)
    {
        return Tan(x * Pi);
    }

    /// <inheritdoc />
    public static ChiFixed AsinPi(ChiFixed x)
    {
        return Asin(x) / Pi;
    }

    /// <inheritdoc />
    public static ChiFixed AcosPi(ChiFixed x)
    {
        return Acos(x) / Pi;
    }

    /// <inheritdoc />
    public static ChiFixed AtanPi(ChiFixed x)
    {
        return Atan(x) / Pi;
    }

    /// <inheritdoc />
    public static (ChiFixed Sin, ChiFixed Cos) SinCos(ChiFixed x)
    {
        return FixedMath.SinCos(x);
    }

    /// <inheritdoc />
    public static (ChiFixed SinPi, ChiFixed CosPi) SinCosPi(ChiFixed x)
    {
        return SinCos(x * Pi);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed Atan2(ChiFixed y, ChiFixed x)
    {
        return FixedMath.Atan2(y, x);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiFixed Atan2Pi(ChiFixed y, ChiFixed x)
    {
        return Atan2(y, x) / Pi;
    }

    #endregion

    #region Exponential Functions

    /// <inheritdoc />
    public static ChiFixed Exp(ChiFixed x)
    {
        return FixedMath.Exp(x);
    }

    /// <inheritdoc />
    public static ChiFixed Exp2(ChiFixed x)
    {
        return Exp(x * Ln2);
    }

    /// <inheritdoc />
    public static ChiFixed Exp10(ChiFixed x)
    {
        return Exp(x * Ln10);
    }

    #endregion

    #region Logarithmic Functions

    /// <inheritdoc />
    public static ChiFixed Log(ChiFixed x)
    {
        return FixedMath.Ln(x);
    }

    /// <inheritdoc />
    public static ChiFixed Log(ChiFixed x, ChiFixed newBase)
    {
        if (newBase.Raw is ScaleFactor or <= 0)
            throw new ArgumentException("Logarithm base must be positive and not equal to 1.");
        return Log(x) / Log(newBase);
    }

    /// <inheritdoc />
    public static ChiFixed Log2(ChiFixed x)
    {
        return Log(x) / Ln2;
    }

    /// <inheritdoc />
    public static ChiFixed Log10(ChiFixed x)
    {
        return Log(x) / Ln10;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ILogB(ChiFixed x)
    {
        if (x.Raw == 0)
            return int.MinValue;

        var absRaw = x.Raw < 0 ? -x.Raw : x.Raw;
        var leadingZeros = long.LeadingZeroCount(absRaw);
        return 63 - (int)leadingZeros - FractionalBits;
    }

    #endregion

    #region Hyperbolic Functions

    /// <inheritdoc />
    public static ChiFixed Sinh(ChiFixed x)
    {
        var expX = Exp(x);
        var expNegX = One / expX;
        return (expX - expNegX) / Two;
    }

    /// <inheritdoc />
    public static ChiFixed Cosh(ChiFixed x)
    {
        var expX = Exp(x);
        var expNegX = One / expX;
        return (expX + expNegX) / Two;
    }

    /// <inheritdoc />
    public static ChiFixed Tanh(ChiFixed x)
    {
        var exp2X = Exp(Two * x);
        return (exp2X - One) / (exp2X + One);
    }

    /// <inheritdoc />
    public static ChiFixed Asinh(ChiFixed x)
    {
        return Log(x + Sqrt(x * x + One));
    }

    /// <inheritdoc />
    public static ChiFixed Acosh(ChiFixed x)
    {
        if (x.Raw < ScaleFactor)
            throw new ArgumentException("Acosh is only defined for values >= 1.");
        return Log(x + Sqrt(x * x - One));
    }

    /// <inheritdoc />
    public static ChiFixed Atanh(ChiFixed x)
    {
        if (x.Raw is <= -ScaleFactor or >= ScaleFactor)
            throw new ArgumentException("Atanh is only defined for values in (-1, 1).");
        return (Log(One + x) - Log(One - x)) / Two;
    }

    #endregion
}