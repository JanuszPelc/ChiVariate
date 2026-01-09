using System.Globalization;

namespace ChiVariate.Internal.ChiFixed;

internal static class FixedFormatter
{
    public static string Format(long raw, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        Span<char> buffer = stackalloc char[256];

        return TryFormat(raw, buffer, out var charsWritten, format, provider)
            ? new string(buffer[..charsWritten])
            : throw new FormatException("Number too large to format");
    }

    public static bool TryFormat(
        long raw,
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        provider ??= CultureInfo.InvariantCulture;

        var formatType = format.IsEmpty || format[0] == '\0' ? 'G' : char.ToUpper(format[0]);

        switch (formatType)
        {
            case 'G':
                return TryFormat(raw, destination, out charsWritten);
            case 'F':
            {
                var precision = ExtractPrecision(format);
                var decimalValue = FixedMath.ToDecimal(raw);

                Span<char> formatBuffer = stackalloc char[16];
                formatBuffer[0] = 'F';
                precision.TryFormat(formatBuffer[1..], out var precisionChars);

                return decimalValue.TryFormat(destination, out charsWritten, formatBuffer[..(1 + precisionChars)],
                    provider);
            }
            default:
                throw new FormatException($"Format specifier '{format.ToString()}' is not supported");
        }
    }

    private static int ExtractPrecision(ReadOnlySpan<char> format)
    {
        if (format.Length <= 1)
            return ChiVariate.ChiFixed.FormatFractionalDigits;

        return int.TryParse(format[1..], NumberStyles.Integer, CultureInfo.InvariantCulture, out var precision)
            ? precision
            : ChiVariate.ChiFixed.FormatFractionalDigits;
    }

    private static bool TryFormat(long raw, Span<char> destination, out int charsWritten)
    {
        var isNegative = raw < 0;
        ulong absRaw;

        if (isNegative)
            absRaw = (ulong)-raw;
        else
            absRaw = (ulong)raw;

        const ulong scale = 1UL << ChiVariate.ChiFixed.FractionalBits;
        var integerPart = absRaw >> ChiVariate.ChiFixed.FractionalBits;
        var fractionalRaw = absRaw & (scale - 1);

        var pos = 0;

        if (fractionalRaw == 0)
        {
            if (isNegative)
            {
                if (destination.Length < 1)
                {
                    charsWritten = 0;
                    return false;
                }

                destination[pos++] = '-';
            }

            if (!integerPart.TryFormat(destination[pos..], out var intChars))
            {
                charsWritten = 0;
                return false;
            }

            charsWritten = pos + intChars;
            return true;
        }

        Span<char> fracSpan = stackalloc char[ChiVariate.ChiFixed.FormatFractionalDigits];
        var remainder = fractionalRaw;

        for (var i = 0; i < ChiVariate.ChiFixed.FormatFractionalDigits; i++)
        {
            remainder *= 10;
            var digit = (int)(remainder / scale);
            remainder -= (ulong)digit * scale;
            fracSpan[i] = (char)('0' + digit);
        }

        pos = 0;
        if (isNegative)
        {
            if (destination.Length < 1)
            {
                charsWritten = 0;
                return false;
            }

            destination[pos++] = '-';
        }

        if (!integerPart.TryFormat(destination[pos..], out var integerChars))
        {
            charsWritten = 0;
            return false;
        }

        pos += integerChars;

        if (pos >= destination.Length)
        {
            charsWritten = 0;
            return false;
        }

        destination[pos++] = '.';

        if (pos + fracSpan.Length > destination.Length)
        {
            charsWritten = 0;
            return false;
        }

        fracSpan.CopyTo(destination[pos..]);
        var fullLength = pos + fracSpan.Length;

        var currentBestLength = fullLength;
        var startOfFractional = pos;

        for (var i = fullLength - 1; i >= startOfFractional; i--)
        {
            var ch = destination[i];
            if (ch is not ('0' or '.'))
                break;

            if (ChiVariate.ChiFixed.Parse(destination[..i], CultureInfo.InvariantCulture).Raw != raw)
                break;

            currentBestLength = i;
        }

        charsWritten = currentBestLength;
        return true;
    }
}