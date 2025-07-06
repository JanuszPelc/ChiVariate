using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ChiVariate.Internal;

namespace ChiVariate;

/// <summary>
///     Provides static utility methods for generating and manipulating long seed values
///     for pseudo-random number generation.
/// </summary>
public static class ChiSeed
{
    /// <summary>
    ///     Generates a unique seed value with strong unpredictability characteristics.
    /// </summary>
    /// <returns>
    ///     A <see cref="long" /> value representing the generated pseudo-unique seed.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Although the generated seed has a high level of resistance to attacks,
    ///         this algorithm is not cryptographically secure and should not be used for
    ///         security-sensitive purposes such as password hashing or digital signatures.
    ///     </para>
    ///     <para>
    ///         Suitable for hash table DoS protection, procedural generation, and non-cryptographic
    ///         applications requiring unpredictable seed values.
    ///     </para>
    /// </remarks>
    public static long GenerateUnique()
    {
        lock (Global.Lock)
        {
            unchecked
            {
                const ulong multiplierPrime = 0x1F844CB7FD2C1EAD;
                Global.CurrentIndex = (Global.CurrentIndex ^ Global.TimerTicks) * multiplierPrime;

                return Chi32.ApplyCascadingHashInterleave((long)Global.RuntimeSelector, (long)Global.CurrentIndex);
            }
        }
    }

    /// <summary>
    ///     Scrambles a 64-bit value into a reproducible and well-mixed 64-bit form.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="long" /> value to be scrambled.
    /// </param>
    /// <returns>
    ///     A <see cref="long" /> representing a well-mixed value derived from the input value.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble(long value)
    {
        unchecked
        {
            var selector = InitializeCombiner();
            CombineValue(ref selector, (int)(uint)value);
            CombineValue(ref selector, (int)(uint)(value >> 32));

            return Chi32.ApplyCascadingHashInterleave(selector, value);
        }
    }

    /// <summary>
    ///     Transforms a string into a reproducible and well-mixed 64-bit form.
    /// </summary>
    /// <param name="string">
    ///     The <see cref="string" /> value to be scrambled.
    /// </param>
    /// <returns>
    ///     A <see cref="long" /> representing a well-mixed value derived from the input string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="string" /> is null.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble(string @string)
    {
        ArgumentNullException.ThrowIfNull(@string);

        return Scramble(@string, @string.Length);
    }

    /// <summary>
    ///     Produces a well-mixed 64-bit value by combining a scrambled string and a numeric input.
    /// </summary>
    /// <param name="string">The <see cref="string" /> value to be incorporated into the hash calculation.</param>
    /// <param name="number">A numeric value of type <typeparamref name="TNumber" /> to contribute to the hash.</param>
    /// <typeparam name="TNumber">The unmanaged numeric type implementing <see cref="System.Numerics.INumberBase{T}" />.</typeparam>
    /// <returns>
    ///     A <see cref="long" /> representing a well-mixed value derived from the input string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="string" /> is null.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Scramble<TNumber>(string @string, TNumber number)
        where TNumber : unmanaged, INumberBase<TNumber>
    {
        ArgumentNullException.ThrowIfNull(@string);

        var selector = long.CreateTruncating(number);
        var index = Core.Hash64(@string);

        return Chi32.ApplyCascadingHashInterleave(selector, index);
    }

    /// <summary>
    ///     Provides advanced utility methods.
    /// </summary>
    public static class Core
    {
        /// <summary>
        ///     Computes a deterministic 64-bit hash value from a string using cross-platform
        ///     Unicode normalization and endianness handling.
        /// </summary>
        /// <param name="string">
        ///     The string to hash. Can be null or empty.
        /// </param>
        /// <returns>
        ///     A <see cref="long" /> hash value that is deterministic across all platforms
        ///     and .NET implementations. Returns 0 for null or empty strings.
        /// </returns>
        /// <remarks>
        ///     This method uses UTF-8 encoding and endianness normalization to ensure
        ///     identical results on little-endian and big-endian systems, making it
        ///     suitable for scenarios requiring reproducible hashes across different
        ///     architectures and operating systems.
        /// </remarks>
        public static long Hash64(string? @string)
        {
            if (string.IsNullOrEmpty(@string))
                return 0;

            const int maxStackAllocSize = 512;

            var byteCount = Encoding.UTF8.GetByteCount(@string);
            byte[]? rentedArray = null;
            var byteSpan = byteCount <= maxStackAllocSize
                ? stackalloc byte[byteCount]
                : RentArray(byteCount, out rentedArray);

            var result = InitializeCombiner();

            try
            {
                Encoding.UTF8.GetBytes(@string, byteSpan);

                var intCount = byteSpan.Length & ~3;
                var intSpan = MemoryMarshal.Cast<byte, int>(byteSpan[..intCount]);

                if (!BitConverter.IsLittleEndian)
                    for (var index = 0; index < intSpan.Length; index++)
                        intSpan[index] = BinaryPrimitives.ReverseEndianness(intSpan[index]);

                foreach (var int32 in intSpan)
                    CombineValue(ref result, int32);

                var orphanCount = byteSpan.Length & 3;
                if (orphanCount > 0)
                {
                    var tailBytes = byteSpan[^orphanCount..];
                    var last = 0;
                    for (var i = 0; i < orphanCount; i++)
                        last |= tailBytes[i] << (i * 8);

                    CombineValue(ref result, last);
                }
            }
            finally
            {
                if (byteSpan.Length > maxStackAllocSize)
                    ArrayPool<byte>.Shared.Return(rentedArray!);
            }

            return result;

            static Span<byte> RentArray(int byteCount, out byte[] rentedArray)
            {
                rentedArray = ArrayPool<byte>.Shared.Rent(byteCount);
                return rentedArray.AsSpan(0, byteCount);
            }
        }
    }

    #region Private and boierplate

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long InitializeCombiner()
    {
        const ulong initialPrime = 0x46A74A57896EA3C9;

        return (long)initialPrime;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CombineValue(ref long combinedValue, int value)
    {
        const ulong multiplierPrime = 0x8A2AB4D322468F2D;
        const ulong bitmaskPrime = 0xFC2ED86788EEAD7F;

        var combinedHash = (uint)(combinedValue ^ value) * multiplierPrime;
        var offset = (int)combinedHash & 63;

        combinedValue ^= (long)BitOperations.RotateRight(combinedHash ^ bitmaskPrime, offset);
    }

    private static class Global
    {
        public static ulong RuntimeSelector { get; } =
            (ulong)BuildRuntimeFingerprint(TimerTicks);

        public static ulong CurrentIndex { get; set; } =
            (ulong)BuildRuntimeFingerprint(Math.Sin(TimerTicks));

        public static ulong TimerTicks => (ulong)Stopwatch.GetTimestamp();
        public static object Lock { get; } = new();

        private static long BuildRuntimeFingerprint(double value)
        {
            var components = new List<string>();

            TryAddComponent(() => $"{value:C53}");
            TryAddComponent(() => AppContext.BaseDirectory);
            TryAddComponent(() => RuntimeInformation.OSDescription);
            TryAddComponent(() => Guid.NewGuid().ToString());
            TryAddComponent(() => DateTime.Now.ToLongDateString());
            TryAddComponent(() => DateTime.Now.ToLongTimeString());
            TryAddComponent(() => DateTime.Now.Ticks.ToString());
            TryAddComponent(() => CultureInfo.CurrentCulture.DisplayName);
            TryAddComponent(() => Environment.Version.ToString());

            var separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            new Random().Shuffle(CollectionsMarshal.AsSpan(components));
            var selector = Core.Hash64(string.Join(separator, components));
            var index = (long)TimerTicks;

            return Chi32.ApplyCascadingHashInterleave(selector, index);

            void TryAddComponent(Func<string> valueFactory)
            {
                try
                {
                    var componentValue = valueFactory();
                    if (!string.IsNullOrEmpty(componentValue))
                        components.Add(componentValue);
                }
                catch
                {
                    // Silently ignore failed sources
                }
            }
        }
    }

    #endregion
}