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
///         For hash table security and DoS protection, use the ChiHash.Seed value
///         or a custom entropy source to introduce per-application randomization.
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
///     .Add(ChiHash.Seed)
///     .Add(data)
///     .Hash;
/// </code>
/// </example>
public ref struct ChiHash
{
    /// <summary>
    ///     A pseudo-randomly generated seed value that remains constant for the lifetime
    ///     of the current application instance. Each application restart generates a new value.
    /// </summary>
    /// <remarks>
    ///     This seed enables non-deterministic hashing for security purposes
    ///     (e.g., DoS protection in hash tables) while maintaining consistency within
    ///     the current application session.
    /// </remarks>
    public static int Seed { get; } = new ChiHash().Add(ChiSeed.GenerateUnique()).Hash;

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
}