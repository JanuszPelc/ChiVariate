// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from an Exponential distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerExponentialExtensions.Exponential{TRng,T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerExponential<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _rateLambda;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerExponential(ref TRng rng, T rateLambda)
    {
        if (!T.IsFinite(rateLambda) || rateLambda <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(rateLambda), "Rate (lambda) must be positive.");

        _rng = ref rng;
        _rateLambda = rateLambda;
    }

    /// <summary>
    ///     Samples a single random value from the configured Exponential distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var u = ChiRealProvider.Next<TRng, T>(ref _rng, ChiIntervalOptions.ExcludeMin);
        return -ChiMath.Log(u) / _rateLambda;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Exponential distribution.
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
///     Provides extension methods for sampling from the Exponential distribution.
/// </summary>
public static class ChiSamplerExponentialExtensions
{
    /// <summary>
    ///     Returns a sampler for the Exponential distribution, which models the time between events in a Poisson process.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="rateLambda">The rate parameter (λ), which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models waiting times for an event to occur, such as the interval between customer arrivals,
    ///         the time until a component fails, or the delay before an enemy attacks. Its "memoryless" property makes it
    ///         ideal for event-driven simulations.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample a waiting time for an event that occurs, on average, 5 times per second.
    /// var waitingTime = rng.Exponential(5.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerExponential<TRng, T> Exponential<TRng, T>(this ref TRng rng, T rateLambda)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerExponential<TRng, T>(ref rng, rateLambda);
    }
}