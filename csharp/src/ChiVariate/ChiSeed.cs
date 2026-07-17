// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     A deterministic seed builder that provides cross-platform reproducible 64-bit seed values
///     for pseudo-random number generation.
/// </summary>
/// <remarks>
///     <para>
///         ChiSeed produces deterministic seed values that remain consistent across
///         different platforms, .NET versions, and application runs. This makes it
///         suitable for scenarios requiring reproducible seeding: save files,
///         procedural generation, networking protocols, and distributed simulations.
///     </para>
///     <para>
///         For a non-deterministic seed, use <see cref="GetEntropy" />.
///     </para>
/// </remarks>
/// <example>
///     <code>
/// // One-shot seed derivation
/// var rng = new ChiRng(ChiSeed.Scramble("Overworld", chunkX, chunkY));
///  
/// // Incremental seed building
/// var seed = new ChiSeed()
///     .Add("Overworld")
///     .Add(chunkX)
///     .Add(chunkY)
///     .Seed;
///     </code>
/// </example>
public ref struct ChiSeed
{
    private long _mix;
    private int _count;

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

    /// <summary>
    ///     Gets the current 64-bit seed value based on all values added so far.
    /// </summary>
    /// <value>A 64-bit signed integer seed value.</value>
    public readonly long Seed => FinalizeScramble(_count == 0 ? ChiHash64.InitialValue : _mix, _count);

    /// <summary>
    ///     Adds a string to the seed.
    /// </summary>
    /// <param name="value">The value to add. Null is treated as an empty string.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(string? value)
    {
        Mix(value ?? "");
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 8-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(byte value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a signed 8-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(sbyte value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a signed 16-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(short value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 16-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(ushort value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a Unicode character to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(char value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a signed 32-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(int value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 32-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(uint value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a signed 64-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(long value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 64-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(ulong value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a signed 128-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(Int128 value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 128-bit integer to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(UInt128 value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a single-precision floating-point value to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(float value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a double-precision floating-point value to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(double value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a half-precision floating-point value to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(Half value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a decimal value to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(decimal value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a boolean value to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(bool value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a GUID to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(Guid value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a complex number to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(Complex value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a date and time value to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(DateTime value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a date, time, and offset value to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(DateTimeOffset value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a time interval to the seed.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add(TimeSpan value)
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds an enum value to the seed.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add<T>(T value) where T : struct, Enum
    {
        Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a span of values to the seed.
    /// </summary>
    /// <typeparam name="T">The type of values in the span.</typeparam>
    /// <param name="values">The values to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add<T>(scoped ReadOnlySpan<T> values)
    {
        foreach (var value in values)
            Mix(value);
        return this;
    }

    /// <summary>
    ///     Adds a span of values to the seed.
    /// </summary>
    /// <typeparam name="T">The type of values in the span.</typeparam>
    /// <param name="values">The values to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSeed Add<T>(scoped Span<T> values)
    {
        foreach (var value in values)
            Mix(value);
        return this;
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
        var state = ChiHash64.InitialValue;
        foreach (var arg in args)
            state = ChiHash64.HashValue(arg, state);

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
        var state = ChiHash64.InitialValue;
        state = ChiHash64.HashValue(v1, state);
        state = ChiHash64.HashValue(v2, state);

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
        var state = ChiHash64.InitialValue;
        state = ChiHash64.HashValue(v1, state);
        state = ChiHash64.HashValue(v2, state);
        state = ChiHash64.HashValue(v3, state);

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
        var state = ChiHash64.InitialValue;
        state = ChiHash64.HashValue(v1, state);
        state = ChiHash64.HashValue(v2, state);
        state = ChiHash64.HashValue(v3, state);
        state = ChiHash64.HashValue(v4, state);

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
        var state = ChiHash64.InitialValue;
        state = ChiHash64.HashValue(v1, state);
        state = ChiHash64.HashValue(v2, state);
        state = ChiHash64.HashValue(v3, state);
        state = ChiHash64.HashValue(v4, state);
        state = ChiHash64.HashValue(v5, state);

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
        var state = ChiHash64.InitialValue;
        state = ChiHash64.HashValue(v1, state);
        state = ChiHash64.HashValue(v2, state);
        state = ChiHash64.HashValue(v3, state);
        state = ChiHash64.HashValue(v4, state);
        state = ChiHash64.HashValue(v5, state);
        state = ChiHash64.HashValue(v6, state);

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
        var state = ChiHash64.InitialValue;
        state = ChiHash64.HashValue(v1, state);
        state = ChiHash64.HashValue(v2, state);
        state = ChiHash64.HashValue(v3, state);
        state = ChiHash64.HashValue(v4, state);
        state = ChiHash64.HashValue(v5, state);
        state = ChiHash64.HashValue(v6, state);
        state = ChiHash64.HashValue(v7, state);

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
        var state = ChiHash64.InitialValue;
        state = ChiHash64.HashValue(v1, state);
        state = ChiHash64.HashValue(v2, state);
        state = ChiHash64.HashValue(v3, state);
        state = ChiHash64.HashValue(v4, state);
        state = ChiHash64.HashValue(v5, state);
        state = ChiHash64.HashValue(v6, state);
        state = ChiHash64.HashValue(v7, state);
        state = ChiHash64.HashValue(v8, state);

        return FinalizeScramble(state, 8);
    }

    #region Private and boilerplate

    /// <summary>
    ///     Mixes a single value into the accumulated builder state.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Mix<T>(T value)
    {
        _mix = ChiHash64.HashValue(value, _count == 0 ? ChiHash64.InitialValue : _mix);
        _count++;
    }

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
    ///     Produces the same value as adding the same values, in the same order,
    ///     to a new <see cref="ChiSeed" /> instance.
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