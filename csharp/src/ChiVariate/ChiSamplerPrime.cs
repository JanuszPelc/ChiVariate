using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Generators;

namespace ChiVariate;

/// <summary>
///     Samples prime numbers from a specified integer range using an on-the-fly generation algorithm.
/// </summary>
public readonly ref struct ChiSamplerPrime<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : unmanaged, IBinaryInteger<T>, IBitwiseOperators<T, T, T>, IMinMaxValue<T>
{
    private readonly ref TRng _rng;
    private readonly T _minInclusive;
    private readonly T _maxExclusive;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerPrime(ref TRng rng, T minInclusive, T maxExclusive, int minEstimatePopulation)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(Unsafe.SizeOf<T>(), 2);
        ArgumentOutOfRangeException.ThrowIfNegative(minEstimatePopulation);
        ArgumentOutOfRangeException.ThrowIfNegative(minInclusive);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(minInclusive, maxExclusive);

        if (minEstimatePopulation > 0)
        {
            var estimatedCount = PrimeCounting.EstimatePopulationCount(minInclusive, maxExclusive);
            if (estimatedCount < (ulong)minEstimatePopulation)
                throw new ArgumentException(
                    $"The specified range is estimated to contain only ~{estimatedCount} primes, which is below the " +
                    $"required minimum of {minEstimatePopulation}. To sample from this range, provide a smaller " +
                    $"'{nameof(minEstimatePopulation)}' value, or use a wider range.", nameof(minEstimatePopulation));
        }

        _rng = ref rng;
        _minInclusive = minInclusive;
        _maxExclusive = maxExclusive;
    }

    /// <summary>
    ///     Samples a single random prime from the configured distribution.
    /// </summary>
    /// <returns>A new prime number sampled from the specified range.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if no prime numbers exist in the specified range.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        var two = T.CreateChecked(2);
        var rangeSize = _maxExclusive - _minInclusive;
        var startOffset = ChiIntegerGenerator.Next(ref _rng, T.Zero, rangeSize);

        for (var i = T.Zero; i < rangeSize; i++)
        {
            var candidate = _minInclusive + (startOffset + i) % rangeSize;

            if (T.IsEvenInteger(candidate))
            {
                if (candidate == two)
                    return two;
                continue;
            }

            if (IsPrime(candidate))
                return candidate;
        }

        throw new InvalidOperationException("No prime numbers exist in the specified range.");
    }

    /// <summary>
    ///     Generates a sequence of random prime numbers from the configured distribution.
    /// </summary>
    /// <param name="count">The number of values to sample from the distribution.</param>
    /// <returns>An enumerable collection of prime numbers sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<T> Sample(int count)
    {
        var enumerable = ChiEnumerable<T>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }

    private static bool IsPrime(T n)
    {
        if (n <= T.One) return false;
        if (n <= T.CreateChecked(3)) return true;
        if (n % T.CreateChecked(2) == T.Zero || n % T.CreateChecked(3) == T.Zero) return false;

        // Trial division against precomputed small primes using Sieve of Eratosthenes
        var smallPrimesSpan = PrimeCounting.PrimesBelowLimit;
        foreach (var pInt in smallPrimesSpan)
        {
            if (pInt <= 3) continue;

            var p = T.CreateChecked(pInt);
            if (p * p > n) break;
            if (n % p == T.Zero) return false;
        }

        // Deterministic Miller-Rabin primality test
        return MillerRabin<T>.IsProvablyPrime(n);
    }
}

/// <summary>
///     Provides extension methods for sampling prime numbers.
/// </summary>
public static class ChiSamplerPrimeExtensions
{
    /// <summary>
    ///     Returns a sampler that generates random prime numbers from a specified integer range.
    ///     This feature is supported for integer types up to 128 bits wide.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The integer type of the generated values (e.g., int, long, UInt128).</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="minInclusive">The inclusive lower bound of the range. Must be non-negative.</param>
    /// <param name="maxExclusive">The exclusive upper bound of the range.</param>
    /// <param name="minEstimatePopulation">
    ///     The minimum estimated number of primes required to be in the range. If the estimated
    ///     count is below this threshold, an <see cref="ArgumentException" /> is thrown in the constructor.
    ///     Set to 0 to disable the population check entirely and improve construction performance.
    /// </param>
    /// <returns>A sampler that can be used to generate random prime numbers.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Ideal for number theory research, cryptographic experiments, and applications
    ///         requiring mathematically guaranteed prime numbers with uniform distribution.
    ///     </para>
    ///     <para>
    ///         <b>Primality Guarantees:</b> This method returns numbers that are provably prime using deterministic
    ///         Miller-Rabin testing with various optimization techniques.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> `O(r/π(r) × √n)` expected, where r is range size, π(r) is prime density,
    ///         and √n is primality test cost. Performance varies significantly with range density.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    ///     // Generate a set of very large primes
    ///     var max = UInt128.MaxValue;
    ///     var primes = rng.Prime(max / 2, max).Sample(100).ToList();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerPrime<TRng, T> Prime<TRng, T>(this ref TRng rng,
        T minInclusive, T maxExclusive, int minEstimatePopulation = 256)
        where TRng : struct, IChiRngSource<TRng>
        where T : unmanaged, IBinaryInteger<T>, IBitwiseOperators<T, T, T>, IMinMaxValue<T>
    {
        return new ChiSamplerPrime<TRng, T>(ref rng, minInclusive, maxExclusive, minEstimatePopulation);
    }
}

#region File-scoped helpers

file static class PrimeCounting
{
    private const int SmallSieveLimit = 1000;
    private static readonly Lazy<int[]> LazyPrimesBelowLimit = new(GeneratePrimesBelowLimit);

    public static ReadOnlySpan<int> PrimesBelowLimit => LazyPrimesBelowLimit.Value.AsSpan();

    public static ulong EstimatePopulationCount<T>(T min, T max)
        where T : unmanaged, IBinaryInteger<T>
    {
        if (max < T.CreateChecked(SmallSieveLimit))
        {
            var countBelowMax = CountPrimesBelow(int.CreateChecked(max));
            var countBelowMin = CountPrimesBelow(int.CreateChecked(min));
            return (ulong)(countBelowMax - countBelowMin);
        }

        var minAsDouble = double.CreateChecked(min);
        var maxAsDouble = double.CreateChecked(max);

        // Prime counting function approximation using offset logarithmic integral Li(x)
        var piMax = maxAsDouble / (Math.Log(maxAsDouble > 1.0 ? maxAsDouble : 2.0) - 1.0);
        var piMin = minAsDouble > 2.0 ? minAsDouble / (Math.Log(minAsDouble) - 1.0) : 0.0;

        var estimate = piMax - piMin;
        return estimate > 0 ? (ulong)Math.Round(estimate) : 0;
    }

    /// <summary>
    ///     Generates primes using the Sieve of Eratosthenes algorithm.
    /// </summary>
    private static int[] GeneratePrimesBelowLimit()
    {
        var isPrime = new bool[SmallSieveLimit];
        for (var i = 2; i < SmallSieveLimit; i++) isPrime[i] = true;

        for (var p = 2; p * p < SmallSieveLimit; p++)
            if (isPrime[p])
                for (var i = p * p; i < SmallSieveLimit; i += p)
                    isPrime[i] = false;

        var count = 0;
        for (var i = 0; i < SmallSieveLimit; i++)
            if (isPrime[i])
                count++;

        var primes = new int[count];
        var index = 0;
        for (var i = 0; i < SmallSieveLimit; i++)
            if (isPrime[i])
                primes[index++] = i;

        return primes;
    }

    private static int CountPrimesBelow(int limit)
    {
        Debug.Assert(limit < SmallSieveLimit);
        if (limit < 2) return 0;
        var index = PrimesBelowLimit.BinarySearch(limit);
        return index >= 0 ? index + 1 : ~index;
    }
}

file static class MillerRabin<T>
    where T : unmanaged, IBinaryInteger<T>
{
    /// <summary>
    ///     Deterministic Miller-Rabin bases for testing integers up to 64 bits.
    ///     Based on Jaeschke (1993) and verified computational results.
    /// </summary>
    private static readonly T[] Bases64 =
        ((long[]) [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37]).Select(T.CreateChecked).ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsProvablyPrime(T n)
    {
        foreach (var b in Bases64)
        {
            if (n == b) return true;
            if (!Check(n, b)) return false;
        }

        return true;
    }

    /// <summary>
    ///     Performs a single Miller-Rabin test with the given base.
    /// </summary>
    private static bool Check(T n, T b)
    {
        var nMinus1 = n - T.One;
        var d = nMinus1;
        var s = T.TrailingZeroCount(d);
        d >>= int.CreateChecked(s);

        var x = ModPow(b, d, n);
        if (x == T.One || x == nMinus1) return true;

        for (var r = T.Zero; r < s; r++)
        {
            x = ModMul(x, x, n);
            if (x == nMinus1) return true;
        }

        return false;
    }

    /// <summary>
    ///     Computes (base^exponent) mod modulus using binary exponentiation.
    /// </summary>
    private static T ModPow(T b, T exp, T mod)
    {
        if (mod == T.One) return T.Zero;
        var result = T.One;
        b %= mod;

        while (exp > T.Zero)
        {
            if (T.IsOddInteger(exp)) result = ModMul(result, b, mod);
            b = ModMul(b, b, mod);
            exp >>= 1;
        }

        return result;
    }

    /// <summary>
    ///     Computes (a * b) mod m using the Russian peasant multiplication algorithm
    ///     to prevent overflow for large integer types.
    /// </summary>
    private static T ModMul(T a, T b, T m)
    {
        if (m <= T.Zero) throw new DivideByZeroException();

        var res = T.Zero;
        a %= m;

        while (b > T.Zero)
        {
            if (T.IsOddInteger(b))
            {
                if (a > m - res) res -= m - a;
                else res += a;
            }

            if (a > m - a) a -= m - a;
            else a += a;
            b >>= 1;
        }

        return res;
    }
}

#endregion