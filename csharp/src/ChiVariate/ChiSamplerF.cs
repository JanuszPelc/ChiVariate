using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from an F-distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerFExtensions.F{TRng,T}" /> method.
/// </remarks>
public ref struct ChiSamplerF<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private ChiSamplerChiSquared<TRng, T> _chiSqSampler1;
    private ChiSamplerChiSquared<TRng, T> _chiSqSampler2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerF(ref TRng rng, T numeratorDegreesOfFreedom, T denominatorDegreesOfFreedom)
    {
        if (!T.IsFinite(numeratorDegreesOfFreedom) || numeratorDegreesOfFreedom <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(numeratorDegreesOfFreedom),
                "Numerator degrees of freedom must be positive.");
        if (!T.IsFinite(denominatorDegreesOfFreedom) || denominatorDegreesOfFreedom <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(denominatorDegreesOfFreedom),
                "Denominator degrees of freedom must be positive.");

        _chiSqSampler1 = new ChiSamplerChiSquared<TRng, T>(ref rng, numeratorDegreesOfFreedom);
        _chiSqSampler2 = new ChiSamplerChiSquared<TRng, T>(ref rng, denominatorDegreesOfFreedom);
    }

    /// <summary>
    ///     Samples a single random value from the configured F-distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var chiSq1 = _chiSqSampler1.Sample();
        var chiSq2 = _chiSqSampler2.Sample();

        var d1 = _chiSqSampler1.DegreesOfFreedom;
        var d2 = _chiSqSampler2.DegreesOfFreedom;

        var fStatistic = chiSq1 / d1 / (chiSq2 / d2);

        return fStatistic;
    }

    /// <summary>
    ///     Generates a sequence of random values from the F-distribution.
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
///     Provides extension methods for sampling from the F-distribution.
/// </summary>
public static class ChiSamplerFExtensions
{
    /// <summary>
    ///     Returns a sampler for the F-distribution, which models the ratio of two scaled chi-squared variables.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="numeratorDegreesOfFreedom">The numerator degrees of freedom (d₁), which must be positive.</param>
    /// <param name="denominatorDegreesOfFreedom">The denominator degrees of freedom (d₂), which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> A cornerstone of hypothesis testing, particularly in Analysis of Variance (ANOVA). It's used
    ///         to compare the variances between two or more groups to determine if their means are significantly different.
    ///         Essential for A/B testing, experimental design, and regression model analysis.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample an F-statistic for comparing two groups with 5 and 10 samples
    /// var fStat = rng.F(5.0, 10.0).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerF<TRng, T> F<TRng, T>(
        this ref TRng rng, T numeratorDegreesOfFreedom, T denominatorDegreesOfFreedom)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerF<TRng, T>(ref rng, numeratorDegreesOfFreedom, denominatorDegreesOfFreedom);
    }
}