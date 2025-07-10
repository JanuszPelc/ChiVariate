using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Normal (Gaussian) distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerNormalExtensions.Normal{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerNormal<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly T _mean;
    private readonly T _standardDeviation;

    private ChiStatefulNormalProvider<TRng, T> _normalProvider;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerNormal(ref TRng rng, T mean, T standardDeviation)
    {
        if (!T.IsFinite(mean))
            throw new ArgumentOutOfRangeException(nameof(mean), "Mean must be a finite number.");
        if (!T.IsFinite(standardDeviation) || standardDeviation <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(standardDeviation), "Standard deviation must be positive.");

        _mean = mean;
        _standardDeviation = standardDeviation;
        _normalProvider = new ChiStatefulNormalProvider<TRng, T>(ref rng);
    }

    /// <summary>
    ///     Samples a single random value from the configured Normal distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var z = _normalProvider.Next();
        return _mean + z * _standardDeviation;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Normal distribution.
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
///     Provides extension methods for sampling from the Normal (Gaussian) distribution.
/// </summary>
public static class ChiSamplerNormalExtensions
{
    /// <summary>
    ///     Returns a sampler for the Normal (or Gaussian) distribution, the classic symmetric, bell-shaped curve defined by a
    ///     mean and standard deviation.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="mean">The mean (μ) of the distribution, which is the center of the bell curve.</param>
    /// <param name="standardDeviation">
    ///     The standard deviation (σ), which controls the spread of the distribution. Must be
    ///     positive.
    /// </param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models a vast range of phenomena, from measurement errors to test scores. Essential for
    ///         generating realistic noise, statistical analysis, and any process involving the sum of many small, independent
    ///         random effects.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample a value from a standard normal distribution (mean=0, std dev=1)
    /// var z = rng.Normal(0.0, 1.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerNormal<TRng, T> Normal<TRng, T>(this ref TRng rng, T mean, T standardDeviation)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerNormal<TRng, T>(ref rng, mean, standardDeviation);
    }
}