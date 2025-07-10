// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Chi-Squared distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerChiSquaredExtensions.ChiSquared{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerChiSquared<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private static readonly T Two = T.CreateChecked(2.0);
    private static readonly T Half = T.One / Two;
    private static readonly T Epsilon = T.CreateChecked(0.000001);

    private ChiSamplerGamma<TRng, T> _gammaSampler;

    /// <summary>
    ///     Gets the degrees of freedom (k) for this Chi-Squared distribution.
    /// </summary>
    public T DegreesOfFreedom { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerChiSquared(ref TRng rng, T degreesOfFreedom)
    {
        var isNonInteger = !T.IsFinite(degreesOfFreedom) ||
                           T.Abs(degreesOfFreedom - T.Round(degreesOfFreedom)) > Epsilon;
        if (isNonInteger || degreesOfFreedom <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(degreesOfFreedom),
                "Degrees of freedom must be a positive integer value.");

        var shape = degreesOfFreedom * Half;
        var scale = Two;

        _gammaSampler = new ChiSamplerGamma<TRng, T>(ref rng, shape, scale);
        DegreesOfFreedom = degreesOfFreedom;
    }

    /// <summary>
    ///     Samples a single random value from the configured Chi-Squared distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        return _gammaSampler.Sample();
    }

    /// <summary>
    ///     Generates a sequence of random values from the Chi-Squared distribution.
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
///     Provides extension methods for sampling from the Chi-Squared distribution.
/// </summary>
public static class ChiSamplerChiSquaredExtensions
{
    /// <summary>
    ///     Returns a sampler for the Chi-Squared distribution, which models the sum of the squares of k independent standard
    ///     normal variables.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="degreesOfFreedom">The degrees of freedom (k), which must be a positive integer.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> A cornerstone of statistical hypothesis testing. Commonly used in machine learning for
    ///         feature selection, in data analysis for goodness-of-fit tests, and for modeling the variance of a sample.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample from a Chi-Squared distribution with 5 degrees of freedom
    /// var value = rng.ChiSquared(5.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerChiSquared<TRng, T> ChiSquared<TRng, T>(this ref TRng rng, T degreesOfFreedom)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerChiSquared<TRng, T>(ref rng, degreesOfFreedom);
    }
}