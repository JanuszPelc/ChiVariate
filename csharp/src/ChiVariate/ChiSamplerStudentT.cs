// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples from a Student's t-distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerStudentTExtensions.StudentT{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerStudentT<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private ChiStatefulNormalProvider<TRng, T> _normalProvider;
    private ChiSamplerChiSquared<TRng, T> _chiSquaredSampler;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerStudentT(ref TRng rng, T degreesOfFreedom)
    {
        if (!T.IsFinite(degreesOfFreedom) || degreesOfFreedom <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(degreesOfFreedom), "Degrees of freedom (ν) must be positive.");

        _normalProvider = new ChiStatefulNormalProvider<TRng, T>(ref rng);
        _chiSquaredSampler = new ChiSamplerChiSquared<TRng, T>(ref rng, degreesOfFreedom);
    }

    /// <summary>
    ///     Samples a single random value from the configured Student's t-distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var z = _normalProvider.Next();
        var v = _chiSquaredSampler.Sample();
        var dof = _chiSquaredSampler.DegreesOfFreedom;
        return z / ChiMath.Sqrt(v / dof);
    }

    /// <summary>
    ///     Generates a sequence of random values from the Student's t-distribution.
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
///     Provides extension methods for sampling from the Student's t-distribution.
/// </summary>
public static class ChiSamplerStudentTExtensions
{
    /// <summary>
    ///     Returns a sampler for the Student's t-distribution, a bell-shaped curve similar to the Normal but with heavier
    ///     tails.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="degreesOfFreedom">The degrees of freedom (ν), which must be a positive number.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> A key tool for statistical inference when the sample size is small or the population's true
    ///         variance is unknown. Its heavier tails make it more robust for modeling data with potential outliers.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample from a t-distribution with 5 degrees of freedom
    /// var value = rng.StudentT(5.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerStudentT<TRng, T> StudentT<TRng, T>(this ref TRng rng, T degreesOfFreedom)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerStudentT<TRng, T>(ref rng, degreesOfFreedom);
    }
}