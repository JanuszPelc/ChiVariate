using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Beta distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerBetaExtensions.Beta{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerBeta<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private ChiSamplerGamma<TRng, T> _gammaSampler1;
    private ChiSamplerGamma<TRng, T> _gammaSampler2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerBeta(ref TRng rng, T alpha, T beta)
    {
        if (!T.IsFinite(alpha) || alpha <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha must be positive and finite.");
        if (!T.IsFinite(beta) || beta <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(beta), "Beta must be positive and finite.");

        _gammaSampler1 = new ChiSamplerGamma<TRng, T>(ref rng, alpha, T.One);
        _gammaSampler2 = new ChiSamplerGamma<TRng, T>(ref rng, beta, T.One);
    }

    /// <summary>
    ///     Samples a single random value from the configured Beta distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution, between 0 and 1.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var gamma1 = _gammaSampler1.Sample();
        var gamma2 = _gammaSampler2.Sample();

        var sum = gamma1 + gamma2;
        return sum > T.Zero ? gamma1 / sum : T.Zero;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Beta distribution.
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
///     Provides extension methods for sampling from the Beta distribution.
/// </summary>
public static class ChiSamplerBetaExtensions
{
    /// <summary>
    ///     Returns a sampler for the Beta distribution, which generates random values between 0 and 1, often used to represent
    ///     an unknown probability.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="alpha">The shape parameter α (alpha), which must be positive.</param>
    /// <param name="beta">The shape parameter β (beta), which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models uncertainty about a probability. Ideal for estimating a player's skill from match
    ///         history, modeling task completion rates, or any scenario where the outcome is a percentage bounded between 0
    ///         and 1.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample a plausible click-through rate from a Beta prior
    /// var clickRate = rng.Beta(5.0, 95.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerBeta<TRng, T> Beta<TRng, T>(this ref TRng rng, T alpha, T beta)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerBeta<TRng, T>(ref rng, alpha, beta);
    }
}