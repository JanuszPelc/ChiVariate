using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Log-Normal distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerLogNormalExtensions.LogNormal{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerLogNormal<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly T _logMean;
    private readonly T _logStandardDeviation;

    private ChiStatefulNormalProvider<TRng, T> _normalProvider;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerLogNormal(ref TRng rng, T logMean, T logStandardDeviation)
    {
        if (!T.IsFinite(logStandardDeviation) || logStandardDeviation <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(logStandardDeviation),
                "Sigma (std dev of the underlying normal) must be positive.");

        _logMean = logMean;
        _logStandardDeviation = logStandardDeviation;
        _normalProvider = new ChiStatefulNormalProvider<TRng, T>(ref rng);
    }

    /// <summary>
    ///     Samples a single random value from the configured Log-Normal distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var normalSample = _logMean + _normalProvider.Next() * _logStandardDeviation;
        return ChiMath.Exp(normalSample);
    }

    /// <summary>
    ///     Generates a sequence of random values from the Log-Normal distribution.
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
///     Provides extension methods for sampling from the Log-Normal distribution.
/// </summary>
public static class ChiSamplerLogNormalExtensions
{
    /// <summary>
    ///     Returns a sampler for the Log-Normal distribution, which models a random variable whose logarithm is normally
    ///     distributed.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="logMean">The mean (μ) of the underlying Normal distribution.</param>
    /// <param name="logStandardDeviation">The standard deviation (σ) of the underlying Normal distribution. Must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models any positive random variable that arises from a multiplicative process of many small
    ///         factors. Perfect for simulating stock prices, income levels, internet traffic, or biological growth rates.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample a value modeling a stock price
    /// var stockPrice = rng.LogNormal(0.0, 0.1).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerLogNormal<TRng, T> LogNormal<TRng, T>(this ref TRng rng, T logMean, T logStandardDeviation)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerLogNormal<TRng, T>(ref rng, logMean, logStandardDeviation);
    }
}