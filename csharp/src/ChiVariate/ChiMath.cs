using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     Provides generic mathematical functions that support various <see cref="IFloatingPoint{TSelf}" /> types,
///     including high-precision <see cref="decimal" /> operations.
/// </summary>
public static class ChiMath
{
    /// <summary>
    ///     Returns a specified number raised to the specified power.
    /// </summary>
    /// <param name="x">The number to be raised to a power.</param>
    /// <param name="exponent">The number that specifies a power.</param>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <returns>The number <paramref name="x" /> raised to the power <paramref name="exponent" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Pow<T>(T x, T exponent)
        where T : IFloatingPoint<T>
    {
        var x1 = x;
        var exponent1 = exponent;
        if (typeof(T) == typeof(double))
        {
            var pow = Math.Pow(Unsafe.As<T, double>(ref x1), Unsafe.As<T, double>(ref exponent1));
            return Unsafe.As<double, T>(ref pow);
        }

        if (typeof(T) == typeof(float))
        {
            var pow = MathF.Pow(Unsafe.As<T, float>(ref x1), Unsafe.As<T, float>(ref exponent1));
            return Unsafe.As<float, T>(ref pow);
        }

        if (typeof(T) == typeof(decimal))
        {
            var baseVal = Unsafe.As<T, decimal>(ref x1);
            var exp = Unsafe.As<T, decimal>(ref exponent1);
            var result = ChiDecimalMath.Pow(baseVal, exp);
            return Unsafe.As<decimal, T>(ref result);
        }

        var fallbackDouble = Math.Pow(Unsafe.As<T, double>(ref x1), Unsafe.As<T, double>(ref exponent1));
        return Unsafe.As<double, T>(ref fallbackDouble);
    }

    /// <summary>
    ///     Returns the square root of a specified number.
    /// </summary>
    /// <param name="x">The number whose square root is to be found. Must be non-negative.</param>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <returns>The positive square root of <paramref name="x" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sqrt<T>(T x)
        where T : IFloatingPoint<T>
    {
        if (x < T.Zero)
            throw new OverflowException("Cannot calculate square root of a negative number.");

        if (x == T.Zero)
            return T.Zero;

        var current = T.CreateChecked(Math.Sqrt(double.CreateChecked(x)));

        T previous;
        var iterations = 0;
        const int maxIterations = 100;

        do
        {
            previous = current;
            if (previous == T.Zero)
                return T.Zero;

            current = (previous + x / previous) * Const<T>.OneHalf;
            iterations++;
        } while (T.Abs(previous - current) > Const<T>.Epsilon && iterations < maxIterations);

        return current;
    }

    /// <summary>
    ///     Returns the natural (base e) logarithm of a specified number.
    /// </summary>
    /// <param name="x">The number whose logarithm is to be found. Must be positive.</param>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <returns>The natural logarithm of <paramref name="x" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Log<T>(T x)
        where T : IFloatingPoint<T>
    {
        var x1 = x;
        if (typeof(T) == typeof(double))
        {
            var log = Math.Log(Unsafe.As<T, double>(ref x1));
            return Unsafe.As<double, T>(ref log);
        }

        if (typeof(T) == typeof(float))
        {
            var log = MathF.Log(Unsafe.As<T, float>(ref x1));
            return Unsafe.As<float, T>(ref log);
        }

        if (typeof(T) == typeof(decimal))
        {
            var log = ChiDecimalMath.Ln(Unsafe.As<T, decimal>(ref x1));
            return Unsafe.As<decimal, T>(ref log);
        }

        var fallbackDouble = Math.Log(double.CreateChecked(x1));
        return T.CreateChecked(fallbackDouble);
    }

    /// <summary>
    ///     Returns e raised to the specified power.
    /// </summary>
    /// <param name="x">A number specifying a power.</param>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <returns>The number e raised to the power <paramref name="x" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Exp<T>(T x)
        where T : IFloatingPoint<T>
    {
        var x1 = x;
        if (typeof(T) == typeof(double))
        {
            var exp = Math.Exp(Unsafe.As<T, double>(ref x1));
            return Unsafe.As<double, T>(ref exp);
        }

        if (typeof(T) == typeof(float))
        {
            var exp = MathF.Exp(Unsafe.As<T, float>(ref x1));
            return Unsafe.As<float, T>(ref exp);
        }

        if (typeof(T) == typeof(decimal))
        {
            var exp = ChiDecimalMath.Exp(Unsafe.As<T, decimal>(ref x1));
            return Unsafe.As<decimal, T>(ref exp);
        }

        var fallbackDouble = Math.Exp(double.CreateChecked(x1));
        return T.CreateChecked(fallbackDouble);
    }

    /// <summary>
    ///     Returns the cube root of a specified number.
    /// </summary>
    /// <param name="x">The number whose cube root is to be found.</param>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <returns>The cube root of <paramref name="x" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cbrt<T>(T x)
        where T : IFloatingPoint<T>
    {
        return x < T.Zero
            ? -Pow(-x, Const<T>.OneThird)
            : Pow(x, Const<T>.OneThird);
    }

    /// <summary>
    ///     Returns the tangent of the specified angle.
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <returns>The tangent of <paramref name="x" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Tan<T>(T x)
        where T : IFloatingPoint<T>
    {
        var x1 = x;
        if (typeof(T) == typeof(double))
        {
            var tan = Math.Tan(Unsafe.As<T, double>(ref x1));
            return Unsafe.As<double, T>(ref tan);
        }

        if (typeof(T) == typeof(float))
        {
            var tan = MathF.Tan(Unsafe.As<T, float>(ref x1));
            return Unsafe.As<float, T>(ref tan);
        }

        if (typeof(T) == typeof(decimal))
        {
            var tan = ChiDecimalMath.Tan(Unsafe.As<T, decimal>(ref x1));
            return Unsafe.As<decimal, T>(ref tan);
        }

        var fallbackDouble = Math.Tan(double.CreateChecked(x1));
        return T.CreateChecked(fallbackDouble);
    }

    /// <summary>
    ///     Returns the sine of the specified angle.
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <returns>The sine of <paramref name="x" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Sin<T>(T x)
        where T : IFloatingPoint<T>
    {
        var x1 = x;
        if (typeof(T) == typeof(double))
        {
            var sin = Math.Sin(Unsafe.As<T, double>(ref x1));
            return Unsafe.As<double, T>(ref sin);
        }

        if (typeof(T) == typeof(float))
        {
            var sin = MathF.Sin(Unsafe.As<T, float>(ref x1));
            return Unsafe.As<float, T>(ref sin);
        }

        if (typeof(T) == typeof(decimal))
        {
            var sin = ChiDecimalMath.Sin(Unsafe.As<T, decimal>(ref x1));
            return Unsafe.As<decimal, T>(ref sin);
        }

        var fallbackDouble = Math.Sin(double.CreateChecked(x1));
        return T.CreateChecked(fallbackDouble);
    }

    /// <summary>
    ///     Returns the cosine of the specified angle.
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <typeparam name="T">The floating-point type.</typeparam>
    /// <returns>The cosine of <paramref name="x" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cos<T>(T x)
        where T : IFloatingPoint<T>
    {
        var x1 = x;
        if (typeof(T) == typeof(double))
        {
            var cos = Math.Cos(Unsafe.As<T, double>(ref x1));
            return Unsafe.As<double, T>(ref cos);
        }

        if (typeof(T) == typeof(float))
        {
            var cos = MathF.Cos(Unsafe.As<T, float>(ref x1));
            return Unsafe.As<float, T>(ref cos);
        }

        if (typeof(T) == typeof(decimal))
        {
            var cos = ChiDecimalMath.Cos(Unsafe.As<T, decimal>(ref x1));
            return Unsafe.As<decimal, T>(ref cos);
        }

        var fallbackDouble = Math.Cos(double.CreateChecked(x1));
        return T.CreateChecked(fallbackDouble);
    }

    /// <summary>
    ///     Provides commonly used mathematical constants for generic floating-point types.
    /// </summary>
    public static class Const<T>
        where T : IFloatingPoint<T>
    {
        /// <summary>
        ///     A smallest positive value, used for convergence checks.
        /// </summary>
        public static readonly T Epsilon = GetEpsilon();

        /// <summary>
        ///     The mathematical constant π (pi), approximately 3.14159.
        /// </summary>
        public static readonly T Pi = T.Pi;

        /// <summary>
        ///     The mathematical constant τ (tau), equal to 2π, approximately 6.28318.
        /// </summary>
        public static readonly T Tau = T.Tau;

        /// <summary>
        ///     The mathematical constant e (Euler's number), approximately 2.71828.
        /// </summary>
        public static readonly T E = T.E;

        /// <summary>
        ///     The additive identity value (zero) for the floating-point type.
        /// </summary>
        public static readonly T Zero = T.Zero;

        /// <summary>
        ///     The multiplicative identity value (one) for the floating-point type.
        /// </summary>
        public static readonly T One = T.One;

        /// <summary>
        ///     The value two for the floating-point type.
        /// </summary>
        public static readonly T Two = T.One + T.One;

        /// <summary>
        ///     The value three for the floating-point type.
        /// </summary>
        public static readonly T Three = Two + One;

        /// <summary>
        ///     The fractional value one-half (0.5) for the floating-point type.
        /// </summary>
        public static readonly T OneHalf = One / Two;

        /// <summary>
        ///     The fractional value one-third (approximately 0.333) for the floating-point type.
        /// </summary>
        public static readonly T OneThird = One / Three;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T GetEpsilon()
        {
            if (typeof(T) == typeof(double))
                return T.CreateChecked(1e-15); // Machine epsilon for double is ~2.2e-16

            if (typeof(T) == typeof(float))
                return T.CreateChecked(1e-7); // Machine epsilon for float is ~1.2e-7

            if (typeof(T) == typeof(decimal))
                return T.CreateChecked(1e-15); // Conservative for decimal

            return T.CreateChecked(1e-10); // Default fallback
        }
    }
}