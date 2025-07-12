// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

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
///         For hash table security and DoS protection, use ChiHash.Seed value
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
    ///     Adds a string value to the hash calculation.
    /// </summary>
    /// <param name="value">The string to add to the hash. Null values are treated as empty strings.</param>
    /// <returns>A new ChiHash instance with the string incorporated into the hash calculation.</returns>
    /// <remarks>
    ///     <para>
    ///         This method returns a new ChiHash instance. It is recommended to always use the returned value:
    ///     </para>
    ///     <code>
    /// // Recommended - fluent style
    /// var hash = new ChiHash().Add(value1).Add(value2).Hash;
    ///  
    /// // Correct - with reassignment  
    /// var builder = new ChiHash();
    /// builder = builder.Add(value);
    ///  
    /// // Discouraged - class-like usage
    /// var builder = new ChiHash();
    /// builder.Add(value);
    /// </code>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add(string? value)
    {
        Hash = ChiHash32.HashString(value ?? "", Hash);
        return this;
    }

    /// <summary>
    ///     Adds a value of any supported type to the hash calculation.
    /// </summary>
    /// <typeparam name="T">The type of value to add.</typeparam>
    /// <param name="value">
    ///     The value to add to the hash. Supports standard numeric types, bool, enums,
    ///     Guid, Complex, DateTime, DateTimeOffset, and TimeSpan.
    /// </param>
    /// <returns>A new ChiHash instance with the value incorporated into the hash calculation.</returns>
    /// <inheritdoc cref="Add(string)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add<T>(T value)
    {
        Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a span of values supported by <see cref="Add{T}(T)" /> to the hash calculation.
    /// </summary>
    /// <typeparam name="T">The type of values to add.</typeparam>
    /// <param name="values">The span of values to add to the hash.</param>
    /// <returns>A new ChiHash instance with all values incorporated into the hash calculation.</returns>
    /// <inheritdoc cref="Add(string)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add<T>(scoped ReadOnlySpan<T> values)
    {
        foreach (var value in values)
            Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }

    /// <summary>
    ///     Adds a span of values supported by <see cref="Add{T}(T)" /> to the hash calculation.
    /// </summary>
    /// <typeparam name="T">The type of values to add.</typeparam>
    /// <param name="values">The span of values to add to the hash.</param>
    /// <returns>A new ChiHash instance with all values incorporated into the hash calculation.</returns>
    /// <inheritdoc cref="Add(string)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiHash Add<T>(scoped Span<T> values)
    {
        foreach (var value in values)
            Hash = ChiHash32.HashValue(value, Hash);
        return this;
    }
}