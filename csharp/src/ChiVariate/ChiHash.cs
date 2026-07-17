// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     A deterministic hash builder that provides cross-platform reproducible hash codes
///     for non-cryptographic purposes.
/// </summary>
/// <remarks>
///     <para>
///         ChiHash produces deterministic hash values that remain consistent across
///         different platforms, .NET versions, and application runs. This makes it
///         suitable for scenarios requiring reproducible hashing: save files,
///         networking protocols, procedural generation, and distributed systems.
///     </para>
///     <para>
///         This hashing algorithm is not cryptographically secure and should not be used for
///         security-sensitive purposes such as password hashing or digital signatures.
///     </para>
/// </remarks>
/// <example>
///     <code>
/// // Deterministic hashing
/// var hash = new ChiHash()
///     .Add(playerId)
///     .Add(playerName)
///     .Add(level)
///     .Hash;
///  
/// // Security-conscious hashing
/// var secureHash = new ChiHash()
///     .Add(ChiHash.Salt)
///     .Add(data)
///     .Hash;
/// </code>
/// </example>
public ref struct ChiHash
{
    /// <summary>
    ///     An unpredictable salt value that remains constant for the lifetime
    ///     of the current application instance. Each application restart generates a new value.
    /// </summary>
    /// <remarks>
    ///     This salt enables non-deterministic hashing for security purposes
    ///     (e.g., DoS protection in hash tables) while maintaining consistency within
    ///     the current application instance.
    /// </remarks>
    public static int Salt { get; } = new ChiHash().Add(ChiSeed.GetEntropy()).Hash;

    /// <inheritdoc cref="CombineSharedDoc" />
    /// <summary>
    ///     Produces a 32-bit hash code by combining any number of values of the same type.
    /// </summary>
    /// <typeparam name="T">The type of values to combine.</typeparam>
    /// <param name="args">The values to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(1)]
    public static int Combine<T>(params ReadOnlySpan<T> args)
    {
        var hash = 0;
        foreach (var arg in args)
            hash = ChiHash32.HashValue(arg, hash);

        return hash;
    }

    /// <inheritdoc cref="CombineSharedDoc" />
    /// <summary>
    ///     Produces a 32-bit hash code by combining two values of independent types.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <param name="v1">The first value to be incorporated into the calculation.</param>
    /// <param name="v2">The second value to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Combine<T1, T2>(T1 v1, T2 v2)
    {
        var hash = 0;
        hash = ChiHash32.HashValue(v1, hash);
        hash = ChiHash32.HashValue(v2, hash);

        return hash;
    }

    /// <inheritdoc cref="CombineSharedDoc" />
    /// <summary>
    ///     Produces a 32-bit hash code by combining three values of independent types.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <param name="v1">The first value to be incorporated into the calculation.</param>
    /// <param name="v2">The second value to be incorporated into the calculation.</param>
    /// <param name="v3">The third value to be incorporated into the calculation.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Combine<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
    {
        var hash = 0;
        hash = ChiHash32.HashValue(v1, hash);
        hash = ChiHash32.HashValue(v2, hash);
        hash = ChiHash32.HashValue(v3, hash);

        return hash;
    }

    /// <inheritdoc cref="CombineSharedDoc" />
    /// <summary>
    ///     Produces a 32-bit hash code by combining four values of independent types.
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
    public static int Combine<T1, T2, T3, T4>(T1 v1, T2 v2, T3 v3, T4 v4)
    {
        var hash = 0;
        hash = ChiHash32.HashValue(v1, hash);
        hash = ChiHash32.HashValue(v2, hash);
        hash = ChiHash32.HashValue(v3, hash);
        hash = ChiHash32.HashValue(v4, hash);

        return hash;
    }

    /// <inheritdoc cref="CombineSharedDoc" />
    /// <summary>
    ///     Produces a 32-bit hash code by combining five values of independent types.
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
    public static int Combine<T1, T2, T3, T4, T5>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5)
    {
        var hash = 0;
        hash = ChiHash32.HashValue(v1, hash);
        hash = ChiHash32.HashValue(v2, hash);
        hash = ChiHash32.HashValue(v3, hash);
        hash = ChiHash32.HashValue(v4, hash);
        hash = ChiHash32.HashValue(v5, hash);

        return hash;
    }

    /// <inheritdoc cref="CombineSharedDoc" />
    /// <summary>
    ///     Produces a 32-bit hash code by combining six values of independent types.
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
    public static int Combine<T1, T2, T3, T4, T5, T6>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6)
    {
        var hash = 0;
        hash = ChiHash32.HashValue(v1, hash);
        hash = ChiHash32.HashValue(v2, hash);
        hash = ChiHash32.HashValue(v3, hash);
        hash = ChiHash32.HashValue(v4, hash);
        hash = ChiHash32.HashValue(v5, hash);
        hash = ChiHash32.HashValue(v6, hash);

        return hash;
    }

    /// <inheritdoc cref="CombineSharedDoc" />
    /// <summary>
    ///     Produces a 32-bit hash code by combining seven values of independent types.
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
    public static int Combine<T1, T2, T3, T4, T5, T6, T7>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7)
    {
        var hash = 0;
        hash = ChiHash32.HashValue(v1, hash);
        hash = ChiHash32.HashValue(v2, hash);
        hash = ChiHash32.HashValue(v3, hash);
        hash = ChiHash32.HashValue(v4, hash);
        hash = ChiHash32.HashValue(v5, hash);
        hash = ChiHash32.HashValue(v6, hash);
        hash = ChiHash32.HashValue(v7, hash);

        return hash;
    }

    /// <inheritdoc cref="CombineSharedDoc" />
    /// <summary>
    ///     Produces a 32-bit hash code by combining eight values of independent types.
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
    public static int Combine<T1, T2, T3, T4, T5, T6, T7, T8>(T1 v1, T2 v2, T3 v3, T4 v4, T5 v5, T6 v6, T7 v7,
        T8 v8)
    {
        var hash = 0;
        hash = ChiHash32.HashValue(v1, hash);
        hash = ChiHash32.HashValue(v2, hash);
        hash = ChiHash32.HashValue(v3, hash);
        hash = ChiHash32.HashValue(v4, hash);
        hash = ChiHash32.HashValue(v5, hash);
        hash = ChiHash32.HashValue(v6, hash);
        hash = ChiHash32.HashValue(v7, hash);
        hash = ChiHash32.HashValue(v8, hash);

        return hash;
    }

    /// <summary>
    ///     Gets the current 32-bit hash code based on all values added so far.
    /// </summary>
    /// <value>A 32-bit signed integer hash code.</value>
    public int Hash { get; private set; }

    /// <summary>
    ///     Adds a string to the hash.
    /// </summary>
    /// <param name="value">The value to add. Null is treated as an empty string.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(string? value)
    {
        Hash = ChiHash32.HashString(value ?? "", Hash);
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 8-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(byte value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a signed 8-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(sbyte value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a signed 16-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(short value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 16-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(ushort value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a Unicode character to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(char value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a signed 32-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(int value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 32-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(uint value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a signed 64-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(long value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 64-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(ulong value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a signed 128-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(Int128 value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds an unsigned 128-bit integer to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(UInt128 value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a single-precision floating-point value to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(float value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a double-precision floating-point value to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(double value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a half-precision floating-point value to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(Half value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a decimal value to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(decimal value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a boolean value to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(bool value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a GUID to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(Guid value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a complex number to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(Complex value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a date and time value to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(DateTime value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a date, time, and offset value to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(DateTimeOffset value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a time interval to the hash.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(TimeSpan value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds an enum value to the hash.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The value to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add<T>(T value) where T : struct, Enum
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a span of values to the hash.
    /// </summary>
    /// <typeparam name="T">The type of values in the span.</typeparam>
    /// <param name="values">The values to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add<T>(scoped ReadOnlySpan<T> values)
    {
        foreach (var value in values)
            Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a span of values to the hash.
    /// </summary>
    /// <typeparam name="T">The type of values in the span.</typeparam>
    /// <param name="values">The values to add.</param>
    /// <returns>The current instance for chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add<T>(scoped Span<T> values)
    {
        foreach (var value in values)
            Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <returns>A 32-bit signed integer hash code derived from the combined values.</returns>
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
    ///     to a new <see cref="ChiHash" /> instance.
    /// </remarks>
    // ReSharper disable once UnusedMember.Local
    private static int CombineSharedDoc()
    {
        return 0;
    }
}