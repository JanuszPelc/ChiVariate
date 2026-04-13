# Pluggable extension architecture

ChiVariate cleanly separates entropy generation from distribution logic using zero-cost, generic abstractions. This enables integration of both custom RNG sources and domain-specific samplers with no virtual calls, heap allocations, or runtime overhead.

## Custom entropy sources

Any `struct` that implements `IChiRngSource<T>` can serve as a randomness backend. This includes pseudo-random generators, cryptographically secure sources, or hardware-based entropy.

**Integrating a cryptographically strong random number generator:**

```csharp
public record struct CryptoRng : IChiRngSource<CryptoRng>
{
    public static uint NextUInt32(ref CryptoRng _)
    {
        Span<byte> buffer = stackalloc byte[4];
        System.Security.Cryptography.RandomNumberGenerator.Fill(buffer);
        return Unsafe.ReadUnaligned<uint>(ref buffer[0]);
    }

    public static ulong NextUInt64(ref CryptoRng _)
    {
        Span<byte> buffer = stackalloc byte[8];
        System.Security.Cryptography.RandomNumberGenerator.Fill(buffer);
        return Unsafe.ReadUnaligned<ulong>(ref buffer[0]);
    }
}
```

## Custom distribution samplers

Custom samplers are lightweight `ref struct` types that encapsulate reusable sampling logic. They integrate seamlessly with any RNG source using the same fluent, data-oriented patterns as built-in samplers, with no GC or abstraction overhead.

**Defining a Maxwell-Boltzmann sampler for particle speeds:**

```csharp
/// <summary>
///     Samples from the Maxwell-Boltzmann distribution, which models particle speeds in thermal equilibrium.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerMaxwellBoltzmannExtensions.MaxwellBoltzmann{TRng, T}" />
///     method.
/// </remarks>
public ref struct ChiSamplerMaxwellBoltzmann<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _a;
    private ChiSamplerChi<TRng, T> _chiSampler;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerMaxwellBoltzmann(ref TRng rng, T a)
    {
        if (!T.IsFinite(a) || a <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(a), "Scale parameter 'a' must be positive.");

        _rng = ref rng;
        _a = a;

        // The Maxwell-Boltzmann distribution is a Chi distribution
        // with 3 degrees of freedom, scaled by the parameter 'a'.
        _chiSampler = _rng.Chi(T.CreateTruncating(3.0));
    }

    /// <summary>
    ///     Samples a single random speed from the configured Maxwell-Boltzmann distribution.
    /// </summary>
    /// <returns>A new speed value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        return _a * _chiSampler.Sample();
    }

    /// <summary>
    ///     Generates a sequence of random speeds from the Maxwell-Boltzmann distribution.
    /// </summary>
    /// <param name="count">The number of values to sample from the distribution.</param>
    /// <returns>An enumerable collection of speed values sampled from the distribution.</returns>
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

public static class ChiSamplerMaxwellBoltzmannExtensions
{
    /// <summary>
    ///     Returns a sampler for the Maxwell-Boltzmann distribution, which models particle speeds
    ///     in gases at thermal equilibrium.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="a">The scale parameter, related to temperature and particle mass. Must be positive.</param>
    /// <returns>A sampler that can be used to generate random particle speeds.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Ideal for physics simulations, computational chemistry, or generating realistic velocity
    ///         magnitudes for particle effects in games.
    ///     </para>
    ///     <para><b>Performance:</b> Amortized O(1) per sample.</para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerMaxwellBoltzmann<TRng, T> MaxwellBoltzmann<TRng, T>(this ref TRng rng, T a)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerMaxwellBoltzmann<TRng, T>(ref rng, a);
    }
}
```

## Combining custom RNG and sampler

Built-in and custom RNGs and samplers compose naturally to support domain-specific simulation scenarios. These extension points let ChiVariate scale across domains, from cryptographic security to statistical modeling, while preserving performance and expressiveness.

**Using the custom `CryptoRng` with the custom Maxwell-Boltzmann sampler:**

```csharp
var cryptoRng = new CryptoRng(); // Crypto RNG has no seed
const double particleMass = 0.5; // kg
const double speedScale = 300.0; // The 'a' parameter (m/s)
const int sampleSize = 10_000; // 10 thousand samples

var totalKineticEnergy = cryptoRng
    .MaxwellBoltzmann(speedScale)
    .Sample(sampleSize)
    .Aggregate(0.0, (totalEnergy, speed) =>
        totalEnergy + 0.5 * particleMass * speed * speed
    );

Console.WriteLine(
    $"Total Kinetic Energy: {totalKineticEnergy:N0} Joules");
```

## Authoring advanced, custom samplers

ChiVariate supports defining custom distribution samplers, which is especially useful for modeling complex relationships or behaviors not covered by built-in distributions. These samplers can maintain internal state to optimize performance, cache intermediate values, or enable correlated sampling.

See `StatefulSamplerTests.cs` in the test suite for a complete implementation of a `CorrelatedNormals` sampler with full statistical validation.