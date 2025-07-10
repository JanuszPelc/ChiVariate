// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a continuous Uniform distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerUniformContinuousExtensions.Uniform{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerUniformContinuous<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _minInclusive;
    private readonly T _maxExclusive;
    private readonly ChiIntervalOptions _intervalOptions;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerUniformContinuous(ref TRng rng, T minInclusive, T maxExclusive,
        ChiIntervalOptions intervalOptions)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(minInclusive, maxExclusive);

        _rng = ref rng;
        _minInclusive = minInclusive;
        _maxExclusive = maxExclusive;
        _intervalOptions = intervalOptions;
    }

    /// <summary>
    ///     Samples a single random value from the configured continuous Uniform distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng, _intervalOptions);
        var oneMinusU = T.One - u;
        return _minInclusive * oneMinusU + _maxExclusive * u;
    }

    /// <summary>
    ///     Generates a sequence of random values from the continuous Uniform distribution.
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
///     Samples from a discrete Uniform distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerUniformDiscreteExtensions.Uniform{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerUniformDiscrete<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IBinaryInteger<T>, IMinMaxValue<T>, IBitwiseOperators<T, T, T>
{
    private readonly ref TRng _rng;
    private readonly T _minInclusive;
    private readonly T _maxExclusive;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerUniformDiscrete(ref TRng rng, T minInclusive, T maxExclusive)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(minInclusive, maxExclusive);

        _rng = ref rng;
        _minInclusive = minInclusive;
        _maxExclusive = maxExclusive;
    }

    /// <summary>
    ///     Samples a single random value from the configured discrete Uniform distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        return ChiIntegerProvider.Next(ref _rng, _minInclusive, _maxExclusive);
    }

    /// <summary>
    ///     Generates a sequence of random values from the discrete Uniform distribution.
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
///     Provides extension methods for sampling from the continuous Uniform distribution.
/// </summary>
public static class ChiSamplerUniformContinuousExtensions
{
    /// <summary>
    ///     Returns a sampler for the continuous Uniform distribution, which generates a real number where every value in a
    ///     given range has an equal chance of being selected.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="minInclusive">The inclusive lower bound of the range.</param>
    /// <param name="maxExclusive">The exclusive upper bound of the range.</param>
    /// <param name="intervalOptions">Options to control the interval boundaries, such as including the maximum value.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> The most fundamental continuous distribution. Used for generating other types of random
    ///         variates and modeling any scenario where all outcomes within a specific range are equally likely.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Generate a random floating-point value between -10.0 and 10.0
    /// var value = rng.Uniform(-10.0, 10.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerUniformContinuous<TRng, T> Uniform<TRng, T>(ref this TRng rng,
        T minInclusive, T maxExclusive, ChiIntervalOptions intervalOptions = ChiIntervalOptions.None)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerUniformContinuous<TRng, T>(ref rng, minInclusive, maxExclusive, intervalOptions);
    }
}

/// <summary>
///     Provides extension methods for sampling from the discrete Uniform distribution.
/// </summary>
public static class ChiSamplerUniformDiscreteExtensions
{
    /// <summary>
    ///     Returns a sampler for the discrete Uniform distribution, which generates an integer where every value in a given
    ///     range has an equal chance of being selected.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The integer type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="minInclusive">The inclusive lower bound of the range.</param>
    /// <param name="maxExclusive">The exclusive upper bound of the range.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> The fundamental distribution for unbiased choice. Essential for dice rolls, random array
    ///         indexing, or any scenario requiring a fair selection from a set of discrete options.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Generate a random integer between 1 and 100 (inclusive)
    /// var value = rng.Uniform(1, 101).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerUniformDiscrete<TRng, T> Uniform<TRng, T>(ref this TRng rng,
        T minInclusive, T maxExclusive)
        where TRng : struct, IChiRngSource<TRng>
        where T : IBinaryInteger<T>, IMinMaxValue<T>, IBitwiseOperators<T, T, T>
    {
        return new ChiSamplerUniformDiscrete<TRng, T>(ref rng, minInclusive, maxExclusive);
    }
}