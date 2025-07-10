using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Rayleigh distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerRayleighExtensions.Rayleigh{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerRayleigh<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private static readonly T Two = T.CreateChecked(2.0);

    private readonly ref TRng _rng;
    private readonly T _sigma;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerRayleigh(ref TRng rng, T sigma)
    {
        if (!T.IsFinite(sigma) || sigma <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(sigma), "Sigma (scale parameter) must be positive.");

        _rng = ref rng;
        _sigma = sigma;
    }

    /// <summary>
    ///     Samples a single random value from the configured Rayleigh distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng,
            ChiIntervalOptions.ExcludeMin);
        var twoSigmaSq = Two * _sigma * _sigma;

        return ChiMath.Sqrt(-twoSigmaSq * ChiMath.Log(u));
    }

    /// <summary>
    ///     Generates a sequence of random values from the Rayleigh distribution.
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
///     Provides extension methods for sampling from the Rayleigh distribution.
/// </summary>
public static class ChiSamplerRayleighExtensions
{
    /// <summary>
    ///     Returns a sampler for the Rayleigh distribution, which models the magnitude of a 2D vector with independent,
    ///     zero-mean normal components.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="sigma">The scale parameter (σ), which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> A special case of the Chi distribution for 2 degrees of freedom. Commonly used to model wind
    ///         speed, the effect of multi-path signal fading, or wave heights.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample a wind speed from a Rayleigh distribution with scale 10.0
    /// var windSpeed = rng.Rayleigh(10.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerRayleigh<TRng, T> Rayleigh<TRng, T>(this ref TRng rng, T sigma)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerRayleigh<TRng, T>(ref rng, sigma);
    }
}