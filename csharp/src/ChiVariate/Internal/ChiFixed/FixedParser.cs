using System.Globalization;

namespace ChiVariate.Internal.ChiFixed;

internal static class FixedParser
{
    public static bool TryParse(string? s, IFormatProvider? provider, out long result)
    {
        if (s != null)
            return TryParse(s.AsSpan(), provider, out result);

        result = 0;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out long result)
    {
        const NumberStyles defaultStyles =
            NumberStyles.AllowLeadingSign |
            NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowLeadingWhite |
            NumberStyles.AllowTrailingWhite;

        return TryParse(s, defaultStyles, provider, out result);
    }

    public static bool TryParse(string? s, NumberStyles styles, IFormatProvider? provider, out long result)
    {
        if (s != null)
            return TryParse(s.AsSpan(), styles, provider, out result);

        result = 0;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles styles, IFormatProvider? provider, out long result)
    {
        provider ??= CultureInfo.InvariantCulture;

        if (decimal.TryParse(s, styles, provider, out var decimalValue))
        {
            result = FixedMath.FromDecimal(decimalValue);
            return true;
        }

        result = 0;
        return false;
    }

    public static long Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"Invalid ChiFixed format: '{s.ToString()}'");
    }

    public static long Parse(ReadOnlySpan<char> s, NumberStyles styles, IFormatProvider? provider)
    {
        return TryParse(s, styles, provider, out var result)
            ? result
            : throw new FormatException($"Invalid ChiFixed format: '{s.ToString()}'");
    }
}