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
        if (x == T.Zero)
            return exponent < T.Zero ? throw new DivideByZeroException("Cannot raise zero to a negative power.")
                : exponent == T.Zero ? T.One
                : T.Zero;

        var exponent1 = exponent;
        if (typeof(T) == typeof(double))
        {
            var pow = Math.Pow(Unsafe.As<T, double>(ref x), Unsafe.As<T, double>(ref exponent1));
            return Unsafe.As<double, T>(ref pow);
        }

        if (typeof(T) == typeof(float))
        {
            var pow = MathF.Pow(Unsafe.As<T, float>(ref x), Unsafe.As<T, float>(ref exponent1));
            return Unsafe.As<float, T>(ref pow);
        }

        if (typeof(T) == typeof(decimal))
        {
            var baseVal = Unsafe.As<T, decimal>(ref x);
            var exp = Unsafe.As<T, decimal>(ref exponent1);
            var result = ChiDecimalMath.Pow(baseVal, exp);
            return Unsafe.As<decimal, T>(ref result);
        }

        var fallbackDouble = Math.Pow(Unsafe.As<T, double>(ref x), Unsafe.As<T, double>(ref exponent1));
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

        if (typeof(T) == typeof(double))
        {
            var sqrt = Math.Sqrt(Unsafe.As<T, double>(ref x));
            return Unsafe.As<double, T>(ref sqrt);
        }

        if (typeof(T) == typeof(float))
        {
            var sqrt = MathF.Sqrt(Unsafe.As<T, float>(ref x));
            return Unsafe.As<float, T>(ref sqrt);
        }

        if (typeof(T) == typeof(decimal))
        {
            var sqrt = ChiDecimalMath.Sqrt(Unsafe.As<T, decimal>(ref x));
            return Unsafe.As<decimal, T>(ref sqrt);
        }

        var fallbackDouble = Math.Sqrt(double.CreateChecked(x));
        return T.CreateChecked(fallbackDouble);
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
        if (x <= T.Zero)
            throw new ArgumentException("Logarithm undefined for non-positive values.");

        if (typeof(T) == typeof(double))
        {
            var log = Math.Log(Unsafe.As<T, double>(ref x));
            return Unsafe.As<double, T>(ref log);
        }

        if (typeof(T) == typeof(float))
        {
            var log = MathF.Log(Unsafe.As<T, float>(ref x));
            return Unsafe.As<float, T>(ref log);
        }

        if (typeof(T) == typeof(decimal))
        {
            var log = ChiDecimalMath.Ln(Unsafe.As<T, decimal>(ref x));
            return Unsafe.As<decimal, T>(ref log);
        }

        var fallbackDouble = Math.Log(double.CreateChecked(x));
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
        if (typeof(T) == typeof(double))
        {
            var exp = Math.Exp(Unsafe.As<T, double>(ref x));
            return Unsafe.As<double, T>(ref exp);
        }

        if (typeof(T) == typeof(float))
        {
            var exp = MathF.Exp(Unsafe.As<T, float>(ref x));
            return Unsafe.As<float, T>(ref exp);
        }

        if (typeof(T) == typeof(decimal))
        {
            var exp = ChiDecimalMath.Exp(Unsafe.As<T, decimal>(ref x));
            return Unsafe.As<decimal, T>(ref exp);
        }

        var fallbackDouble = Math.Exp(double.CreateChecked(x));
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
        if (typeof(T) == typeof(double))
        {
            var cbrt = Math.Cbrt(Unsafe.As<T, double>(ref x));
            return Unsafe.As<double, T>(ref cbrt);
        }

        if (typeof(T) == typeof(float))
        {
            var cbrt = MathF.Cbrt(Unsafe.As<T, float>(ref x));
            return Unsafe.As<float, T>(ref cbrt);
        }

        if (typeof(T) == typeof(decimal))
        {
            var cbrt = ChiDecimalMath.Cbrt(Unsafe.As<T, decimal>(ref x));
            return Unsafe.As<decimal, T>(ref cbrt);
        }

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
        if (typeof(T) == typeof(double))
        {
            var tan = Math.Tan(Unsafe.As<T, double>(ref x));
            return Unsafe.As<double, T>(ref tan);
        }

        if (typeof(T) == typeof(float))
        {
            var tan = MathF.Tan(Unsafe.As<T, float>(ref x));
            return Unsafe.As<float, T>(ref tan);
        }

        if (typeof(T) == typeof(decimal))
        {
            var tan = ChiDecimalMath.Tan(Unsafe.As<T, decimal>(ref x));
            return Unsafe.As<decimal, T>(ref tan);
        }

        var fallbackDouble = Math.Tan(double.CreateChecked(x));
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
        if (typeof(T) == typeof(double))
        {
            var sin = Math.Sin(Unsafe.As<T, double>(ref x));
            return Unsafe.As<double, T>(ref sin);
        }

        if (typeof(T) == typeof(float))
        {
            var sin = MathF.Sin(Unsafe.As<T, float>(ref x));
            return Unsafe.As<float, T>(ref sin);
        }

        if (typeof(T) == typeof(decimal))
        {
            var sin = ChiDecimalMath.Sin(Unsafe.As<T, decimal>(ref x));
            return Unsafe.As<decimal, T>(ref sin);
        }

        var fallbackDouble = Math.Sin(double.CreateChecked(x));
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
        if (typeof(T) == typeof(double))
        {
            var cos = Math.Cos(Unsafe.As<T, double>(ref x));
            return Unsafe.As<double, T>(ref cos);
        }

        if (typeof(T) == typeof(float))
        {
            var cos = MathF.Cos(Unsafe.As<T, float>(ref x));
            return Unsafe.As<float, T>(ref cos);
        }

        if (typeof(T) == typeof(decimal))
        {
            var cos = ChiDecimalMath.Cos(Unsafe.As<T, decimal>(ref x));
            return Unsafe.As<decimal, T>(ref cos);
        }

        var fallbackDouble = Math.Cos(double.CreateChecked(x));
        return T.CreateChecked(fallbackDouble);
    }

    /// <summary>
    ///     Provides commonly used mathematical constants for generic floating-point types.
    /// </summary>
    public static class Const<T>
        where T : IFloatingPoint<T>
    {
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
        ///     The value ten for the floating-point type.
        /// </summary>
        public static readonly T Ten = T.CreateChecked(10);

        /// <summary>
        ///     The fractional value one-half (0.5) for the floating-point type.
        /// </summary>
        public static readonly T OneHalf = One / Two;

        /// <summary>
        ///     The fractional value one-third (approximately 0.333) for the floating-point type.
        /// </summary>
        public static readonly T OneThird = One / Three;

        /// <summary>
        ///     The natural logarithm of 2, approximately 0.693147.
        /// </summary>
        public static readonly T Ln2 = GetLn2();

        /// <summary>
        ///     The natural logarithm of 10, approximately 2.302585.
        /// </summary>
        public static readonly T Ln10 = GetLn10();

        /// <summary>
        ///     Half of π (π/2), approximately 1.570796.
        /// </summary>
        public static readonly T PiHalf = Pi / Two;

        /// <summary>
        ///     One third of π (π/3), approximately 1.047198.
        /// </summary>
        public static readonly T PiThird = Pi / Three;

        /// <summary>
        ///     One fourth of π (π/4), approximately 0.785398.
        /// </summary>
        public static readonly T PiFourth = Pi / (Two * Two);

        /// <summary>
        ///     One sixth of π (π/6), approximately 0.523599.
        /// </summary>
        public static readonly T PiSixth = Pi / (Three * Two);

        /// <summary>
        ///     √2/2, approximately 0.707107.
        /// </summary>
        public static readonly T SqrtTwoHalf = GetSqrtTwoHalf();

        /// <summary>
        ///     √3/3, approximately 0.577350.
        /// </summary>
        public static readonly T SqrtThreeThird = GetSqrtThreeThird();

        /// <summary>
        ///     √3/2, approximately 0.866025.
        /// </summary>
        public static readonly T SqrtThreeHalf = GetSqrtThreeHalf();

        /// <summary>
        ///     A smallest positive value, used for convergence checks.
        /// </summary>
        public static T Epsilon { get; } = GetEpsilon();

        private static T GetEpsilon()
        {
            if (typeof(T) == typeof(double))
                return T.CreateChecked(1e-14);

            if (typeof(T) == typeof(float))
                return T.CreateChecked(1e-6);

            if (typeof(T) == typeof(decimal))
                return T.CreateChecked(1e-27);

            return T.CreateChecked(1e-10);
        }

        private static T GetLn2()
        {
            if (typeof(T) == typeof(decimal))
                return T.CreateChecked(0.693147180559945309417232121458176568075500134360255254120680m);
            return T.CreateChecked(Math.Log(2.0));
        }

        private static T GetLn10()
        {
            if (typeof(T) == typeof(decimal))
                return T.CreateChecked(2.302585092994045684017991454684364207601101488628772976033m);
            return T.CreateChecked(Math.Log(10.0));
        }

        private static T GetSqrtTwoHalf()
        {
            if (typeof(T) == typeof(decimal))
                return T.CreateChecked(0.707106781186547524400844362104849039284835937688474036588m);
            return T.CreateChecked(Math.Sqrt(2.0) / 2.0);
        }

        private static T GetSqrtThreeThird()
        {
            if (typeof(T) == typeof(decimal))
                return T.CreateChecked(0.577350269189625764509148780501957455647601751270126876018m);
            return T.CreateChecked(Math.Sqrt(3.0) / 3.0);
        }

        private static T GetSqrtThreeHalf()
        {
            if (typeof(T) == typeof(decimal))
                return T.CreateChecked(0.866025403784438646763723170752936183471402626905190314027m);
            return T.CreateChecked(Math.Sqrt(3.0) / 2.0);
        }
    }
}