// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Runtime.CompilerServices;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     Represents a deterministic pseudo-random number generator (PRNG).
/// </summary>
/// <param name="Seed">The initial 64-bit value that determines the unique sequence of random numbers.</param>
/// <param name="Phase">The current position, or counter, within the random sequence.</param>
/// <remarks>
///     <para>
///         The default PRNG for ChiVariate. It is powered by a stateless, counter-based CHI32 algorithm
///         designed for statistical integrity and cross-platform reproducibility.
///     </para>
/// </remarks>
public record struct ChiRng(long Seed, long Phase = 0) : IChiRngSource<ChiRng>
{
    /// <summary>
    ///     Initializes the <see cref="ChiRng" /> struct with randomly generated seed.
    /// </summary>
    public ChiRng() : this(ChiSeed.GenerateUnique())
    {
    }

    /// <summary>
    ///     Initializes the <see cref="ChiRng" /> struct using a string-based seed.
    /// </summary>
    /// <param name="seed">The string to use as a seed. It will be hashed into a 64-bit integer.</param>
    /// <param name="phase">The starting position, or counter, within the random sequence. Defaults to 0.</param>
    public ChiRng(string seed, long phase = 0) : this(ChiSeed.Scramble(seed), phase)
    {
    }

    /// <summary>
    ///     Initializes the <see cref="ChiRng" /> struct from a state captured with <see cref="Snapshot" />.
    /// </summary>
    /// <param name="state">The state object containing the seed and phase to restore.</param>
    public ChiRng(ChiRngState state) : this(state.Seed, state.Phase)
    {
    }

    /// <summary>
    ///     Gets the initial 64-bit seed value.
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
    ///     Creates a snapshot of the current state.
    /// </summary>
    /// <returns>A <see cref="ChiRngState" /> object containing the current seed and phase.</returns>
    /// <remarks>
    ///     The captured state can be used to create a new <see cref="ChiRng" /> instance that will produce
    ///     an identical sequence of random numbers, enabling features like save/load and simulation replay.
    /// </remarks>
    public ChiRngState Snapshot()
    {
        return new ChiRngState(Seed, Phase);
    }
}

/// <summary>
///     Represents the state of a <see cref="ChiRng" /> instance at a specific moment.
/// </summary>
/// <param name="Seed">The initial 64-bit value that determines the unique sequence of random numbers.</param>
/// <param name="Phase">The 64-bit position, or counter, within the random sequence at the time of the snapshot.</param>
public readonly record struct ChiRngState(long Seed, long Phase);