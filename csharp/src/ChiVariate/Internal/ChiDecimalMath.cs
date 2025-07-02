using System.Runtime.CompilerServices;

namespace ChiVariate.Internal;

/// <summary>
///     Provides high-precision mathematical functions for the <see cref="decimal" /> type.
/// </summary>
/// <remarks>
///     This class implements common transcendental functions for <c>decimal</c> that are not
///     available in the standard <see cref="System.Math" /> library.
/// </remarks>
internal static class ChiDecimalMath
{
    private const decimal E = 2.71828182845904523536028747135266249775724709369995957496697m;
    private const decimal Pi = 3.14159265358979323846264338327950288419716939937510582097494m;

    /// <summary>
    ///     Returns a specified decimal number raised to the specified power.
    /// </summary>
    /// <remarks>
    ///     Uses exponentiation by squaring for integer exponents and the identity x^y = exp(y * ln(x)) for fractional ones.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Pow(decimal baseVal, decimal exponent)
    {
        // Handle zero base
        if (baseVal == 0m)
            return exponent switch
            {
                < 0m => throw new DivideByZeroException("Cannot raise zero to a negative power."),
                0m => 1m,
                _ => 0m
            };

        // Handle special exponents
        switch (exponent)
        {
            case 0m:
                return 1m;
            case 1m:
                return baseVal;
        }

        // Handle unit base
        if (baseVal == 1m)
            return 1m;

        var isNegativeBase = baseVal < 0m;
        if (isNegativeBase)
            baseVal = -baseVal;

        decimal result;
        var isIntegerExponent = exponent == Math.Floor(exponent);

        if (isIntegerExponent)
        {
            result = PowInteger(baseVal, (long)exponent);
        }
        else
        {
            if (isNegativeBase)
                throw new ArgumentException("Cannot raise negative number to fractional power.");

            var ln = Ln(baseVal);
            var expLn = exponent * ln;
            result = Exp(expLn);
        }

        // Handle negative result for integer exponents
        if (isNegativeBase && isIntegerExponent && (long)exponent % 2 != 0)
            result = -result;

        return result;
    }

    /// <summary>
    ///     Implements exponentiation by squaring for integer powers.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static decimal PowInteger(decimal baseVal, long exponent)
    {
        if (exponent < 0)
        {
            baseVal = 1m / baseVal;
            exponent = -exponent;
        }

        var result = 1m;
        var currentPower = baseVal;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
                result *= currentPower;

            currentPower *= currentPower;
            exponent >>= 1;
        }

        return result;
    }

    /// <summary>
    ///     Returns the natural (base e) logarithm of a specified decimal number.
    /// </summary>
    /// <remarks>
    ///     Uses argument reduction to bring the value into a small range around 1,
    ///     then computes the logarithm using a Taylor series expansion.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Ln(decimal x)
    {
        switch (x)
        {
            case <= 0m:
                throw new ArgumentException("Logarithm undefined for non-positive values.");
            case 1m:
                return 0m;
        }

        var scale = 0;

        while (x > 1.5m)
        {
            x /= E;
            scale++;
        }

        while (x < 0.5m)
        {
            x *= E;
            scale--;
        }

        var u = x - 1m;
        var term = u;
        var sum = u;

        for (var n = 2; n <= 100 && Math.Abs(term) > ChiMath.Const<decimal>.Epsilon; n++)
        {
            term *= -u;
            sum += term / n;
        }

        return sum + scale * 1m; // ln(e) = 1, so scale * 1m == scale
    }

    /// <summary>
    ///     Returns e raised to the specified decimal power.
    /// </summary>
    /// <remarks>
    ///     Uses a Taylor series expansion for the exponential function.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Exp(decimal x)
    {
        var epsilon = ChiMath.Const<decimal>.Epsilon;

        switch (x)
        {
            case 0m:
                return 1m;
            case > 28m:
                throw new OverflowException("Exponential result too large for decimal.");
            case < -28m:
                return epsilon;
        }

        var result = 1m;
        var term = 1m;

        for (var n = 1; n <= 100 && Math.Abs(term) > epsilon; n++)
        {
            term *= x / n;
            result += term;
        }

        return result <= 0m ? epsilon : result;
    }

    /// <summary>
    ///     Returns the sine of the specified angle.
    /// </summary>
    /// <remarks>
    ///     Uses a Taylor series expansion for the sine function after normalizing the angle to the range [-π, π].
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Sin(decimal x)
    {
        x = NormalizeAngle(x);

        var xSquared = x * x;
        var result = x;
        var term = x;

        for (var n = 1; n <= 50 && Math.Abs(term) > ChiMath.Const<decimal>.Epsilon; n++)
        {
            term *= -xSquared / (2 * n * (2 * n + 1));
            result += term;
        }

        return result;
    }

    /// <summary>
    ///     Returns the cosine of the specified angle.
    /// </summary>
    /// <remarks>
    ///     Uses a Taylor series expansion for the cosine function after normalizing the angle to the range [-π, π].
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Cos(decimal x)
    {
        x = NormalizeAngle(x);

        var result = 1m;
        var term = 1m;
        var xSquared = x * x;

        for (var n = 1; n <= 50 && Math.Abs(term) > ChiMath.Const<decimal>.Epsilon; n++)
        {
            term *= -xSquared / ((2 * n - 1) * 2 * n);
            result += term;
        }

        return result;
    }

    /// <summary>
    ///     Returns the tangent of the specified angle.
    /// </summary>
    /// <remarks>
    ///     Calculated as Sin(x) / Cos(x).
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Tan(decimal x)
    {
        var cos = Cos(x);

        if (Math.Abs(cos) < ChiMath.Const<decimal>.Epsilon)
            throw new ArgumentException("Tangent is undefined at this value (cos = 0).");

        return Sin(x) / cos;
    }

    /// <summary>
    ///     Normalizes an angle to the range [-π, π].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static decimal NormalizeAngle(decimal angle)
    {
        const decimal twoPi = 2 * Pi;

        while (angle > Pi)
            angle -= twoPi;

        while (angle < -Pi)
            angle += twoPi;

        return angle;
    }
}