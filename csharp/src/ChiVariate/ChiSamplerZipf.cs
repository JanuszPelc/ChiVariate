using System.Numerics;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Samples from a Zipf distribution.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerZipfExtensions.Zipf{TRng, T}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerZipf<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IBinaryInteger<T>
{
    private readonly ref TRng _rng;
    private readonly T _numberOfElements;
    private readonly double _exponent;
    private readonly double _hxm;
    private readonly double _hHalf;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerZipf(ref TRng rng, T numberOfElements, double exponent)
    {
        if (!T.IsFinite(numberOfElements) || numberOfElements <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(numberOfElements), "Number of elements must be positive.");
        if (!double.IsFinite(exponent) || exponent <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(exponent), "Exponent must be positive.");

        _rng = ref rng;
        _numberOfElements = numberOfElements;
        _exponent = exponent;

        _hHalf = H(0.5, exponent);
        _hxm = H(double.CreateChecked(numberOfElements) + 0.5, exponent);
    }

    /// <summary>
    ///     Samples a single random value from the configured Zipf distribution.
    /// </summary>
    /// <returns>A new value sampled from the distribution, between 1 and N (inclusive).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var uniform = _rng.Chance();

        while (true)
        {
            var v = uniform.NextDouble();
            var u = uniform.NextDouble();

            var hInv = _hxm + v * (_hHalf - _hxm);
            var x = H(hInv, 1.0 / _exponent);

            var k = T.CreateTruncating(Math.Round(x));

            if (k < T.One)
                k = T.One;
            else if (k > _numberOfElements)
                k = _numberOfElements;

            var kDouble = double.CreateChecked(k);
            var hKMinusHalf = H(kDouble - 0.5, _exponent);
            var hKPlusHalf = H(kDouble + 0.5, _exponent);

            if (u * hInv * (hKMinusHalf - hKPlusHalf) <= hKMinusHalf * H(kDouble, _exponent))
                return k;
        }
    }

    /// <summary>
    ///     Generates a sequence of random values from the Zipf distribution.
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double H(double x, double s)
    {
        return Math.Exp(-s * Math.Log(x));
    }
}

/// <summary>
///     Provides extension methods for sampling from the Zipf distribution.
/// </summary>
public static class ChiSamplerZipfExtensions
{
    /// <summary>
    ///     Returns a sampler for the Zipf distribution, which models a "power-law" where frequency is inversely proportional
    ///     to rank.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The integer type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="numberOfElements">The number of elements (N) in the distribution, which must be positive.</param>
    /// <param name="exponent">The exponent (s) of the distribution, which must be positive.</param>
    /// <returns>A sampler that can be used to generate random values from the specified distribution.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Models "power-law" or "long-tail" phenomena where a few items are extremely common and most
    ///         are rare. Perfect for simulating word frequencies in a language, city populations, or user engagement patterns.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> Amortized O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Sample a word rank from a vocabulary of 10,000 words.
    /// var wordRank = rng.Zipf(10_000, 1.0).Sample();
    /// // Sample a user ID from a very large user base (using long)
    /// var userId = rng.Zipf<long>(5_000_000_000L, 1.2).Sample();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerZipf<TRng, T> Zipf<TRng, T>(this ref TRng rng, T numberOfElements, double exponent)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IBinaryInteger<T>
    {
        return new ChiSamplerZipf<TRng, T>(ref rng, numberOfElements, exponent);
    }
}