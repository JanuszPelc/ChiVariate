using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Pareto distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerParetoExtensions.Pareto{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerPareto<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _scale;
    private readonly T _invShape;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerPareto(ref TRng rng, T scale, T shape)
    {
        if (!T.IsFinite(scale) || scale <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale (x_m) must be positive.");
        if (!T.IsFinite(shape) || shape <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(shape), "Shape (alpha) must be positive.");

        _rng = ref rng;
        _scale = scale;
        _invShape = T.One / shape;
    }

    /// <summary>
    ///     Samples a single random value from the configured Pareto distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng, ChiIntervalOptions.ExcludeMin);

        return _scale / ChiMath.Pow(u, _invShape);
    }

    /// <summary>
    ///     Generates a sequence of random values from the Pareto distribution.
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
///     Provides extension methods for sampling from the Pareto distribution.
/// </summary>
public static class ChiSamplerParetoExtensions
{
    /// <summary>
    ///     Returns a sampler for the Pareto distribution, which models the "80/20" rule where a small number of events account
    ///     for a large effect.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="scale">The scale parameter (xₘ), representing the minimum possible value. Must be positive.</param>
    /// <param name="shape">The shape parameter (α), which controls the "tail heaviness". Must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Captures "winner-take-all" phenomena. Ideal for modeling wealth distribution, city
    ///         populations, or the popularity of items where a few top entries dominate the rest.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample from a Pareto distribution that models the 80/20 rule.
    /// var wealth = rng.Pareto(1000.0, 1.16).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerPareto<TRng, T> Pareto<TRng, T>(this ref TRng rng, T scale, T shape)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerPareto<TRng, T>(ref rng, scale, shape);
    }
}