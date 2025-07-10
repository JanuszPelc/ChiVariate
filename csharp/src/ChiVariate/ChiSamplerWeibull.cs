using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Weibull distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerWeibullExtensions.Weibull{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerWeibull<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _scale;
    private readonly T _invShape;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerWeibull(ref TRng rng, T shape, T scale)
    {
        if (!T.IsFinite(shape) || shape <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(shape), "Shape (k) must be positive.");
        if (!T.IsFinite(scale) || scale <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale (lambda) must be positive.");

        _rng = ref rng;
        _scale = scale;
        _invShape = T.One / shape;
    }

    /// <summary>
    ///     Samples a single random value from the configured Weibull distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng, ChiIntervalOptions.ExcludeMin);
        return _scale * ChiMath.Pow(-ChiMath.Log(u), _invShape);
    }

    /// <summary>
    ///     Generates a sequence of random values from the Weibull distribution.
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
///     Provides extension methods for sampling from the Weibull distribution.
/// </summary>
public static class ChiSamplerWeibullExtensions
{
    /// <summary>
    ///     Returns a sampler for the Weibull distribution, a flexible distribution used to model time-to-failure or survival
    ///     data.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="shape">The shape parameter (k), which must be positive.</param>
    /// <param name="scale">The scale parameter (λ), which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> A cornerstone of reliability engineering. Its shape parameter allows it to model systems
    ///         where the failure rate is decreasing (infant mortality), constant (random failures), or increasing (wear-out)
    ///         over time.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Models a system with an increasing failure rate (e.g., wear-out)
    /// var timeToFailure = rng.Weibull(2.0, 5000.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerWeibull<TRng, T> Weibull<TRng, T>(this ref TRng rng, T shape, T scale)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerWeibull<TRng, T>(ref rng, shape, scale);
    }
}