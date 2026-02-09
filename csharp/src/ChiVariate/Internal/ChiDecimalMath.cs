// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;

namespace ChiVariate.Internal;

/// <summary>
///     Provides high-precision mathematical functions for the <see cref="decimal" /> type.
/// </summary>
/// <remarks>
///     These functions are used for deterministic table generation (Ziggurat, fixed-point lookup tables).
///     Decimal provides 28-29 significant digits, ensuring cross-platform reproducibility.
/// </remarks>
internal static class ChiDecimalMath
{
    // Euler's number e ≈ 2.718... (28 decimal places)
    private const decimal E = 2.71828182845904523536028747135266249775724709369995957496697m;

    // Pi ≈ 3.141... (28 decimal places)
    private const decimal Pi = 3.14159265358979323846264338327950288419716939937510582097494m;

    /// <summary>
    ///     Returns a specified decimal number raised to the specified power.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Pow(decimal baseVal, decimal exponent)
    {
        switch (exponent)
        {
            case 0m:
                return 1m;
            case 1m:
                return baseVal;
        }

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

        if (isNegativeBase && isIntegerExponent && (long)exponent % 2 != 0)
            result = -result;

        return result;
    }

    /// <summary>
    ///     Implements exponentiation by squaring for integer powers.
    /// </summary>
    /// <remarks>
    ///     Binary exponentiation: computes b^n in O(log n) multiplications.
    ///     Example: b^13 = b^(1101₂) = b^8 * b^4 * b^1
    ///     Each bit of exponent corresponds to a power of b that we multiply into result.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static decimal PowInteger(decimal baseVal, long exponent)
    {
        if (exponent < 0)
        {
            baseVal = 1m / baseVal;
            exponent = -exponent;
        }

        var result = 1m;
        var currentPower = baseVal; // Tracks b^(2^i) as we iterate

        while (exponent > 0)
        {
            // If bit i is set, multiply b^(2^i) into result
            if ((exponent & 1) == 1)
                result *= currentPower;

            currentPower *= currentPower; // b^(2^i) → b^(2^(i+1))
            exponent >>= 1;
        }

        return result;
    }

    /// <summary>
    ///     Returns the natural (base e) logarithm of a specified decimal number.
    /// </summary>
    /// <remarks>
    ///     Uses argument reduction + Taylor series for ln(1+u).
    ///     1. Reduce x to range [0.5, 1.5] by dividing/multiplying by e, tracking scale
    ///     2. Apply Taylor series: ln(1+u) = u - u²/2 + u³/3 - u⁴/4 + ...
    ///     3. Result = series + scale (since ln(x) = ln(x/e^n) + n)
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Ln(decimal x)
    {
        var scale = 0;

        // Argument reduction: bring x into [0.5, 1.5] where Taylor series converges well
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

        // Taylor series for ln(1+u) where u = x-1, so |u| ≤ 0.5
        // ln(1+u) = u - u²/2 + u³/3 - u⁴/4 + ... = Σ((-1)^(n+1) * u^n / n)
        var u = x - 1m;
        var term = u;
        var sum = u;

        for (var n = 2; n <= 100 && Math.Abs(term) > ChiMath.Const<decimal>.Epsilon; n++)
        {
            term *= -u; // Alternating sign, increasing power
            sum += term / n;
        }

        // ln(original_x) = ln(reduced_x) + scale * ln(e) = sum + scale
        return sum + scale * 1m;
    }

    /// <summary>
    ///     Returns e raised to the specified decimal power.
    /// </summary>
    /// <remarks>
    ///     Uses Taylor series: e^x = 1 + x + x²/2! + x³/3! + x⁴/4! + ...
    ///     Each term is computed iteratively: term_n = term_(n-1) * x / n
    ///     This avoids computing factorials directly and is numerically stable.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Exp(decimal x)
    {
        var epsilon = ChiMath.Const<decimal>.Epsilon;

        // Overflow protection: e^28 ≈ 1.4e12, e^29 would overflow decimal
        switch (x)
        {
            case > 28m:
                throw new OverflowException("Exponential result too large for decimal.");
            case < -28m:
                return epsilon; // Underflow to near-zero
        }

        // Taylor series: e^x = Σ(x^n / n!) = 1 + x + x²/2! + x³/3! + ...
        var result = 1m;
        var term = 1m;

        for (var n = 1; n <= 100 && Math.Abs(term) > epsilon; n++)
        {
            term *= x / n; // term_n = x^n / n! computed as term_(n-1) * x / n
            result += term;
        }

        return result <= 0m ? epsilon : result;
    }

    /// <summary>
    ///     Returns the sine of the specified angle.
    /// </summary>
    /// <remarks>
    ///     Uses Taylor series: sin(x) = x - x³/3! + x⁵/5! - x⁷/7! + ...
    ///     Series converges fastest when |x| is small, so angle is normalized to [-π, π].
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Sin(decimal x)
    {
        x = NormalizeAngle(x);

        // Taylor series: sin(x) = Σ((-1)^n * x^(2n+1) / (2n+1)!)
        // term_n = term_(n-1) * (-x²) / ((2n)(2n+1))
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
    ///     Uses Taylor series: cos(x) = 1 - x²/2! + x⁴/4! - x⁶/6! + ...
    ///     Series converges fastest when |x| is small, so angle is normalized to [-π, π].
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Cos(decimal x)
    {
        x = NormalizeAngle(x);

        // Taylor series: cos(x) = Σ((-1)^n * x^(2n) / (2n)!)
        // term_n = term_(n-1) * (-x²) / ((2n-1)(2n))
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Tan(decimal x)
    {
        var cos = Cos(x);

        if (Math.Abs(cos) < ChiMath.Const<decimal>.Epsilon)
            throw new ArgumentException("Tangent is undefined at this value (cos = 0).");

        return Sin(x) / cos;
    }

    /// <summary>
    ///     Returns the arc tangent of the specified decimal number.
    /// </summary>
    /// <remarks>
    ///     Uses Taylor series: arctan(x) = x - x³/3 + x⁵/5 - x⁷/7 + ... for |x| ≤ 1.
    ///     For |x| > 1: arctan(x) = π/2 - arctan(1/x).
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Atan(decimal x)
    {
        switch (x)
        {
            case 0m:
                return 0m;
            case 1m:
                return Pi / 4m;
            case -1m:
                return -Pi / 4m;
            case > 1m:
                return Pi / 2m - Atan(1m / x);
            case < -1m:
                return -Pi / 2m - Atan(1m / x);
        }

        var result = x;
        var term = x;
        var xSquared = x * x;

        for (var n = 1; n <= 100 && Math.Abs(term) > ChiMath.Const<decimal>.Epsilon; n++)
        {
            term *= -xSquared;
            result += term / (2m * n + 1m);
        }

        return result;
    }

    /// <summary>
    ///     Returns the cube root of a specified decimal number.
    /// </summary>
    /// <remarks>
    ///     Uses Newton-Raphson iteration for f(y) = y³ - x = 0.
    ///     Derivative: f'(y) = 3y²
    ///     Update rule: y_next = y - f(y)/f'(y) = y - (y³-x)/(3y²) = (2y + x/y²) / 3
    ///     Converges quadratically (digits of precision double each iteration).
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Cbrt(decimal x)
    {
        if (x == 0m)
            return 0m;

        if (x == 1m)
            return 1m;

        if (x == -1m)
            return -1m;

        var isNegative = x < 0m;
        if (isNegative)
            x = -x;

        var current = x / 3m; // Initial guess
        const decimal tolerance = 1e-28m;

        decimal previous;
        var iterations = 0;
        const int maxIterations = 100;

        do
        {
            previous = current;
            var currentSquared = current * current;
            // Newton-Raphson: y = (2y + x/y²) / 3
            current = (2m * current + x / currentSquared) / 3m;
            iterations++;
        } while (Math.Abs(previous - current) > tolerance && iterations < maxIterations);

        return isNegative ? -current : current;
    }

    /// <summary>
    ///     Returns the square root of a specified decimal number.
    /// </summary>
    /// <remarks>
    ///     Uses Newton-Raphson iteration (Babylonian method) for f(y) = y² - x = 0.
    ///     Derivative: f'(y) = 2y
    ///     Update rule: y_next = y - f(y)/f'(y) = y - (y²-x)/(2y) = (y + x/y) / 2
    ///     Converges quadratically (digits of precision double each iteration).
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Sqrt(decimal x)
    {
        if (x == 0m)
            return 0m;

        if (x == 1m)
            return 1m;

        var current = x / 2m; // Initial guess
        const decimal tolerance = 1e-28m;

        decimal previous;
        var iterations = 0;
        const int maxIterations = 100;

        do
        {
            previous = current;
            // Newton-Raphson (Babylonian): y = (y + x/y) / 2
            current = (previous + x / previous) / 2m;
            iterations++;
        } while (Math.Abs(previous - current) > tolerance && iterations < maxIterations);

        return current;
    }

    /// <summary>
    ///     Normalizes an angle to the range [-π, π].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static decimal NormalizeAngle(decimal angle)
    {
        const decimal twoPi = 2 * Pi;
        return angle - twoPi * Math.Floor(angle / twoPi + 0.5m);
    }
}