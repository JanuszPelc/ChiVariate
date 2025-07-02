using System.Runtime.CompilerServices;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     Represents a deterministic, counter-based pseudo-random number generator (PRNG) powered by the CHI32 algorithm.
/// </summary>
/// <param name="Seed">The initial 64-bit value that determines the unique sequence of random numbers.</param>
/// <param name="Phase">The current position, or counter, within the random sequence.</param>
/// <remarks>
///     <para>
///         The default PRNG for ChiVariate. It is a stateless, high-performance generator designed for statistical
///         integrity and cross-platform reproducibility.
///     </para>
///     <para>
///         Instances of <see cref="ChiRng" /> are lightweight structs and should be created per-thread as needed.
///     </para>
/// </remarks>
public record struct ChiRng(long Seed, long Phase = 0) : IChiRngSource<ChiRng>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ChiRng" /> struct with a reasonably unique, time-dependent seed.
    /// </summary>
    /// <remarks>
    ///     Use this constructor when you need a different random sequence for each run of the application.
    /// </remarks>
    public ChiRng() : this(ChiSeed.GenerateUnique())
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChiRng" /> struct using a string-based seed.
    /// </summary>
    /// <param name="seed">The string to use as a seed. It will be hashed into a 64-bit integer.</param>
    /// <param name="phase">The starting position, or counter, within the random sequence. Defaults to 0.</param>
    /// <remarks>
    ///     Use this constructor to create a deterministic and reproducible sequence based on a memorable name.
    /// </remarks>
    public ChiRng(string seed, long phase = 0) : this(ChiSeed.Scramble(seed), phase)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChiRng" /> struct from a previously captured state.
    /// </summary>
    /// <param name="state">The state object containing the seed and phase to restore.</param>
    /// <remarks>
    ///     This is ideal for scenarios like loading a saved game or replaying a simulation, as it restores the
    ///     generator to the exact state it was in when the snapshot was taken.
    /// </remarks>
    public ChiRng(ChiRngState state) : this(state.Seed, state.Phase)
    {
    }

    /// <summary>
    ///     Gets the initial 64-bit seed value for this random number generator.
    /// </summary>
    public long Seed { get; } = Seed;

    /// <summary>
    ///     Gets the current 64-bit phase value.
    /// </summary>
    public long Phase { get; private set; } = Phase;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static uint IChiRngSource<ChiRng>.NextUInt32(ref ChiRng @this)
    {
        var result = Chi32.DeriveValueAt(@this.Seed, @this.Phase);
        @this.Phase++;

        return (uint)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ulong IChiRngSource<ChiRng>.NextUInt64(ref ChiRng @this)
    {
        var highPart = (uint)Chi32.DeriveValueAt(@this.Seed, @this.Phase);
        @this.Phase++;

        var lowPart = (uint)Chi32.DeriveValueAt(@this.Seed, @this.Phase);
        @this.Phase++;

        return ((ulong)highPart << 32) | lowPart;
    }

    /// <summary>
    ///     Creates a snapshot of the current state of the random number generator.
    /// </summary>
    /// <returns>A <see cref="ChiRngState" /> object containing the current seed and phase.</returns>
    /// <remarks>
    ///     The returned state can be used to create a new <see cref="ChiRng" /> instance that will produce
    ///     an identical sequence of random numbers, enabling features like save/load and simulation replay.
    /// </remarks>
    public ChiRngState Snapshot()
    {
        return new ChiRngState(Seed, Phase);
    }
}

/// <summary>
///     Represents the complete state of a <see cref="ChiRng" /> instance at a specific moment.
/// </summary>
/// <param name="Seed">The initial 64-bit value that determines the unique sequence of random numbers.</param>
/// <param name="Phase">The position, or counter, within the random sequence at the time of the snapshot.</param>
public readonly record struct ChiRngState(long Seed, long Phase);