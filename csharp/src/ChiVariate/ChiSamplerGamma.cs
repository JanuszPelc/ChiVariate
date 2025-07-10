// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Gamma distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerGammaExtensions.Gamma{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerGamma<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _shape;
    private readonly T _scale;

    private ChiStatefulNormalProvider<TRng, T> _normalProvider;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerGamma(ref TRng rng, T shape, T scale)
    {
        _shape = T.IsFinite(shape) && shape > T.Zero
            ? shape
            : throw new ArgumentOutOfRangeException(nameof(shape), "Shape (alpha) must be positive.");
        _scale = T.IsFinite(scale) && scale > T.Zero
            ? scale
            : throw new ArgumentOutOfRangeException(nameof(scale), "Scale (theta) must be positive.");
        _rng = ref rng;

        _normalProvider = new ChiStatefulNormalProvider<TRng, T>(ref rng);
    }

    /// <summary>
    ///     Samples a Gamma variate using the Ahrens-Dieter acceptance-rejection method, optimized for shape &lt; 1.
    /// </summary>
    private T SampleShapeLessThanOne()
    {
        var b = T.One + _shape / T.E;

        while (true)
        {
            var p = b * ChiRealProvider.Next<TRng, T>(ref _rng);
            var u = ChiRealProvider.Next<TRng, T>(ref _rng);

            if (p <= T.One)
            {
                var x = ChiMath.Pow(p, T.One / _shape);
                if (u <= ChiMath.Exp(-x)) return x * _scale;
            }
            else
            {
                var x = -ChiMath.Log((b - p) / _shape);
                if (u <= ChiMath.Pow(x, _shape - T.One)) return x * _scale;
            }
        }
    }

    /// <summary>
    ///     Samples a Gamma variate using the Marsaglia-Tsang acceptance-rejection method, optimized for shape >= 1.
    /// </summary>
    private T SampleShapeGreaterOrEqualOne()
    {
        var d = _shape - T.One / T.CreateChecked(3.0);
        var c = T.One / ChiMath.Sqrt(T.CreateChecked(9.0) * d);

        while (true)
        {
            var z = _normalProvider.Next();
            var xCubed = T.One + c * z;

            if (xCubed <= T.Zero) continue;

            var x = xCubed * xCubed * xCubed;
            var v = ChiRealProvider.Next<TRng, T>(ref _rng, ChiIntervalOptions.ExcludeMin);

            if (v < T.One - T.CreateChecked(0.0331) * z * z * z * z ||
                ChiMath.Log(v) < T.CreateChecked(0.5) * z * z + d * (T.One - x + ChiMath.Log(x)))
                return d * x * _scale;
        }
    }

    /// <summary>
    ///     Samples a single random value from the configured Gamma distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        return _shape < T.One
            ? SampleShapeLessThanOne()
            : SampleShapeGreaterOrEqualOne();
    }

    /// <summary>
    ///     Generates a sequence of random values from the Gamma distribution.
    /// </summary>
    /// <param name="count">The number of values to sample from the distribution.</param>
    /// <returns>An enumerable collection of values sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<T> Sample(int count)
    {
        var enumerable = ChiEnumerable<T>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }
}

/// <summary>
///     Provides extension methods for sampling from the Gamma distribution.
/// </summary>
public static class ChiSamplerGammaExtensions
{
    /// <summary>
    ///     Returns a sampler for the Gamma distribution, a flexible distribution for positive, skewed values.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="shape">The shape parameter (α), which must be positive.</param>
    /// <param name="scale">The scale parameter (θ), which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Generalizes the Exponential and Chi-Squared distributions. Perfect for modeling the total
    ///         time to complete multiple tasks, the size of insurance claims, or rainfall amounts. Its shape parameter allows
    ///         fine-tuning from exponential-like to nearly normal.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample the size of an insurance claim
    /// var claimSize = rng.Gamma(2.0, 1500.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerGamma<TRng, T> Gamma<TRng, T>(this ref TRng rng, T shape, T scale)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerGamma<TRng, T>(ref rng, shape, scale);
    }
}