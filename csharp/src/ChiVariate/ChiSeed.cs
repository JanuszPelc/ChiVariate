// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     Provides static utility methods for generating and manipulating long seed values
///     for pseudo-random number generation.
/// </summary>
public static class ChiSeed
{
    /// <summary>
    ///     Generates an unpredictable seed value.
    /// </summary>
    /// <returns>
    ///     A <see cref="long" /> value representing the generated seed.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         The value is read from the operating system's cryptographically secure
    ///         random source and is unpredictable on every supported platform.
    ///     </para>
    /// </remarks>
    public static long GetEntropy()
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        RandomNumberGenerator.Fill(buffer);
        return BinaryPrimitives.ReadInt64LittleEndian(buffer);
    }

    /// <inheritdoc cref="ScrambleSharedDoc" />
    /// <summary>
    ///     Produces a well-mixed 64-bit value by combining any number of values of the same type.
    /// </summary>
    /// <typeparam name="T">The type of values to combine.</typeparam>
    /// <param name="args">The values to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static long Scramble<T>(params ReadOnlySpan<T> args)
    {
        var state = ChiMix64.InitialValue;
        foreach (var arg in args)
            state = ChiMix64.MixValue(state, arg);

        return FinalizeScramble(state, args.Length);
    }

    /// <inheritdoc cref="ScrambleSharedDoc" />
    /// <summary>
    ///     Produces a well-mixed 64-bit value by combining two values of independent types.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="v1">The first value to be incorporated into the calculation.</param>
    /// <param name="v2">The second value to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble<T1, T2>(T1 v1, T2 v2)
    {
        var state = ChiMix64.InitialValue;
        state = ChiMix64.MixValue(state, v1);
        state = ChiMix64.MixValue(state, v2);

        return FinalizeScramble(state, 2);
    }

    /// <inheritdoc cref="ScrambleSharedDoc" />
    /// <summary>
    ///     Produces a well-mixed 64-bit value by combining three values of independent types.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="v1">The first value to be incorporated into the calculation.</param>
    /// <param name="v2">The second value to be incorporated into the calculation.</param>
    /// <param name="v3">The third value to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
    {
        var state = ChiMix64.InitialValue;
        state = ChiMix64.MixValue(state, v1);
        state = ChiMix64.MixValue(state, v2);
        state = ChiMix64.MixValue(state, v3);

        return FinalizeScramble(state, 3);
    }

    /// <inheritdoc cref="ScrambleSharedDoc" />
    /// <summary>
    ///     Produces a well-mixed 64-bit value by combining four values of independent types.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <param name="v1">The first value to be incorporated into the calculation.</param>
    /// <param name="v2">The second value to be incorporated into the calculation.</param>
    /// <param name="v3">The third value to be incorporated into the calculation.</param>
    /// <param name="v4">The fourth value to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
    {
        var state = ChiMix64.InitialValue;
        state = ChiMix64.MixValue(state, v1);
        state = ChiMix64.MixValue(state, v2);
        state = ChiMix64.MixValue(state, v3);
        state = ChiMix64.MixValue(state, v4);

        return FinalizeScramble(state, 4);
    }

    /// <inheritdoc cref="ScrambleSharedDoc" />
    /// <summary>
    ///     Produces a well-mixed 64-bit value by combining five values of independent types.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <param name="v1">The first value to be incorporated into the calculation.</param>
    /// <param name="v2">The second value to be incorporated into the calculation.</param>
    /// <param name="v3">The third value to be incorporated into the calculation.</param>
    /// <param name="v4">The fourth value to be incorporated into the calculation.</param>
    /// <param name="v5">The fifth value to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
    {
        var state = ChiMix64.InitialValue;
        state = ChiMix64.MixValue(state, v1);
        state = ChiMix64.MixValue(state, v2);
        state = ChiMix64.MixValue(state, v3);
        state = ChiMix64.MixValue(state, v4);
        state = ChiMix64.MixValue(state, v5);

        return FinalizeScramble(state, 5);
    }

    /// <inheritdoc cref="ScrambleSharedDoc" />
    /// <summary>
    ///     Produces a well-mixed 64-bit value by combining six values of independent types.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <param name="v1">The first value to be incorporated into the calculation.</param>
    /// <param name="v2">The second value to be incorporated into the calculation.</param>
    /// <param name="v3">The third value to be incorporated into the calculation.</param>
    /// <param name="v4">The fourth value to be incorporated into the calculation.</param>
    /// <param name="v5">The fifth value to be incorporated into the calculation.</param>
    /// <param name="v6">The sixth value to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
    {
        var state = ChiMix64.InitialValue;
        state = ChiMix64.MixValue(state, v1);
        state = ChiMix64.MixValue(state, v2);
        state = ChiMix64.MixValue(state, v3);
        state = ChiMix64.MixValue(state, v4);
        state = ChiMix64.MixValue(state, v5);
        state = ChiMix64.MixValue(state, v6);

        return FinalizeScramble(state, 6);
    }

    /// <inheritdoc cref="ScrambleSharedDoc" />
    /// <summary>
    ///     Produces a well-mixed 64-bit value by combining seven values of independent types.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <typeparam name="T7">The type of the seventh value.</typeparam>
    /// <param name="v1">The first value to be incorporated into the calculation.</param>
    /// <param name="v2">The second value to be incorporated into the calculation.</param>
    /// <param name="v3">The third value to be incorporated into the calculation.</param>
    /// <param name="v4">The fourth value to be incorporated into the calculation.</param>
    /// <param name="v5">The fifth value to be incorporated into the calculation.</param>
    /// <param name="v6">The sixth value to be incorporated into the calculation.</param>
    /// <param name="v7">The seventh value to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble<T1, T2, T3, T4, T5, T6, T7>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
    {
        var state = ChiMix64.InitialValue;
        state = ChiMix64.MixValue(state, v1);
        state = ChiMix64.MixValue(state, v2);
        state = ChiMix64.MixValue(state, v3);
        state = ChiMix64.MixValue(state, v4);
        state = ChiMix64.MixValue(state, v5);
        state = ChiMix64.MixValue(state, v6);
        state = ChiMix64.MixValue(state, v7);

        return FinalizeScramble(state, 7);
    }

    /// <inheritdoc cref="ScrambleSharedDoc" />
    /// <summary>
    ///     Produces a well-mixed 64-bit value by combining eight values of independent types.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <typeparam name="T7">The type of the seventh value.</typeparam>
    /// <typeparam name="T8">The type of the eighth value.</typeparam>
    /// <param name="v1">The first value to be incorporated into the calculation.</param>
    /// <param name="v2">The second value to be incorporated into the calculation.</param>
    /// <param name="v3">The third value to be incorporated into the calculation.</param>
    /// <param name="v4">The fourth value to be incorporated into the calculation.</param>
    /// <param name="v5">The fifth value to be incorporated into the calculation.</param>
    /// <param name="v6">The sixth value to be incorporated into the calculation.</param>
    /// <param name="v7">The seventh value to be incorporated into the calculation.</param>
    /// <param name="v8">The eighth value to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble<T1, T2, T3, T4, T5, T6, T7, T8>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7, T8 v8)
    {
        var state = ChiMix64.InitialValue;
        state = ChiMix64.MixValue(state, v1);
        state = ChiMix64.MixValue(state, v2);
        state = ChiMix64.MixValue(state, v3);
        state = ChiMix64.MixValue(state, v4);
        state = ChiMix64.MixValue(state, v5);
        state = ChiMix64.MixValue(state, v6);
        state = ChiMix64.MixValue(state, v7);
        state = ChiMix64.MixValue(state, v8);

        return FinalizeScramble(state, 8);
    }

    #region Private and boierplate

    /// <returns>A <see cref="long" /> representing a well-mixed value derived from the input values. </returns>
    /// <exception cref="NotSupportedException">
    ///     Thrown when any value is of an unsupported type.
    /// </exception>
    /// <remarks>
    ///     Supported types include all numeric types implementing
    ///     <see cref="System.Numerics.INumberBase{T}" />, <see cref="string" />, <see cref="bool" />, enums,
    ///     <see cref="System.Guid" />, <see cref="System.Numerics.Complex" />, <see cref="System.DateTime" />,
    ///     <see cref="System.DateTimeOffset" />, and <see cref="System.TimeSpan" />.
    ///     A null <see cref="string" /> is treated as empty.
    /// </remarks>
    // ReSharper disable once UnusedMember.Local
    private static long ScrambleSharedDoc()
    {
        return 0;
    }

    /// <summary>
    ///     Derives the final scrambled seed from the accumulated mix state and the number of combined values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long FinalizeScramble(long state, int count)
    {
        unchecked
        {
            const ulong mixingPrime = 0xDabbaDeeFecaFeed;
            var mixedLength = (long)BinaryPrimitives.ReverseEndianness(~(ulong)count * mixingPrime);

            return Chi32.ApplyCascadingHashInterleave(mixedLength, state);
        }
    }

    #endregion
}