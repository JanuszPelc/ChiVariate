// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Triangular distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerTriangularExtensions.Triangular{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerTriangular<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _min;
    private readonly T _range;
    private readonly T _splitPoint;
    private readonly T _modeRange;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerTriangular(ref TRng rng, T min, T max, T mode)
    {
        if (min >= max)
            throw new ArgumentException("Min must be less than max.", nameof(min));
        if (mode < min || mode > max)
            throw new ArgumentOutOfRangeException(nameof(mode), "Mode must be between min and max.");

        _rng = ref rng;
        _min = min;
        _range = max - min;
        _splitPoint = (mode - min) / _range;
        _modeRange = mode - min;
    }

    /// <summary>
    ///     Samples a single random value from the configured Triangular distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng);

        return u < _splitPoint
            ? _min + ChiMath.Sqrt(u * _range * _modeRange)
            : _min + _range - ChiMath.Sqrt((T.One - u) * _range * (_range - _modeRange));
    }

    /// <summary>
    ///     Generates a sequence of random values from the Triangular distribution.
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
///     Provides extension methods for sampling from the Triangular distribution.
/// </summary>
public static class ChiSamplerTriangularExtensions
{
    /// <summary>
    ///     Returns a sampler for the Triangular distribution, which is defined by a minimum, maximum, and most likely (mode)
    ///     value.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="min">The inclusive lower bound of the distribution.</param>
    /// <param name="max">The inclusive upper bound of the distribution.</param>
    /// <param name="mode">The most likely value (peak of the triangle), which must be between min and max.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> An excellent tool for simple modeling when you only have expert estimates for the bounds and
    ///         the most likely outcome, such as in project management (PERT) or risk analysis.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Estimate a task duration: best case 5 days, worst case 20, most likely 8.
    /// var estimatedDays = rng.Triangular(5.0, 20.0, 8.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerTriangular<TRng, T> Triangular<TRng, T>(this ref TRng rng, T min, T max, T mode)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerTriangular<TRng, T>(ref rng, min, max, mode);
    }
}