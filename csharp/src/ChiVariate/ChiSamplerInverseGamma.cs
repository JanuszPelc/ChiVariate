using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from an Inverse-Gamma distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerInverseGammaExtensions.InverseGamma{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerInverseGamma<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private ChiSamplerGamma<TRng, T> _gammaSampler;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerInverseGamma(ref TRng rng, T shape, T scale)
    {
        if (!T.IsFinite(shape) || shape <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(shape), "Shape (alpha) must be positive.");
        if (!T.IsFinite(scale) || scale <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale (beta) must be positive.");

        var gammaScale = T.One / scale;
        _gammaSampler = new ChiSamplerGamma<TRng, T>(ref rng, shape, gammaScale);
    }

    /// <summary>
    ///     Samples a single random value from the configured Inverse-Gamma distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var gammaSample = _gammaSampler.Sample();
        return gammaSample > T.Zero ? T.One / gammaSample : T.Zero;
    }

    /// <summary>
    ///     Generates a sequence of random values from the Inverse-Gamma distribution.
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
///     Provides extension methods for sampling from the Inverse-Gamma distribution.
/// </summary>
public static class ChiSamplerInverseGammaExtensions
{
    /// <summary>
    ///     Returns a sampler for the Inverse-Gamma distribution, which generates the reciprocal of a Gamma-distributed
    ///     variable.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="shape">The shape parameter (α), which must be positive.</param>
    /// <param name="scale">The scale parameter (β), which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> A cornerstone of Bayesian statistics, where it serves as the standard conjugate prior for the
    ///         unknown variance of a Normal distribution. Indispensable for modeling uncertainty in variance parameters.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample a plausible variance from a prior belief
    /// var varianceSample = rng.InverseGamma(3.0, 5.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerInverseGamma<TRng, T> InverseGamma<TRng, T>(this ref TRng rng, T shape, T scale)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerInverseGamma<TRng, T>(ref rng, shape, scale);
    }
}