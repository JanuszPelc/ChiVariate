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
/// <remarks>
///     This class uses a more complex and statistically robust mixing algorithm than the general-purpose
///     <see cref="ChiHash" /> utility, making it ideal for creating the high-entropy seeds required
///     by <see cref="ChiRng" />.
/// </remarks>
public static class ChiSeed
{
    /// <summary>
    ///     Generates a reasonably unique seed value.
    /// </summary>
    /// <returns>
    ///     A <see cref="long" /> value representing the generated pseudo-unique seed.
    /// </returns>
    public static long GenerateUnique()
    {
        lock (Global.Lock)
        {
            unchecked
            {
                const ulong deltaPrime = 0x1F844CB7FD2C1EAD;
                Global.CurrentIndex += Global.TimerTicks + deltaPrime;

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
        var index = HashString(@string);

        return Chi32.ApplyCascadingHashInterleave(selector, index);
    }

    #region Internal & Boierplate

    private static long HashString(string? @string)
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

    private static string BuildRuntimeFingerprint()
    {
        var components = new List<string>();

        TryAddComponent(() => AppContext.BaseDirectory);
        TryAddComponent(() => RuntimeInformation.OSDescription);
        TryAddComponent(() => Guid.NewGuid().ToString());
        TryAddComponent(() => Stopwatch.GetTimestamp().ToString());
        TryAddComponent(() => DateTime.Now.ToLongDateString());
        TryAddComponent(() => DateTime.Now.ToLongTimeString());
        TryAddComponent(() => DateTime.Now.Ticks.ToString());
        TryAddComponent(() => CultureInfo.CurrentCulture.DisplayName);
        TryAddComponent(() => Environment.Version.ToString());

        var separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        new Random().Shuffle(CollectionsMarshal.AsSpan(components));
        return string.Join(separator, components);

        void TryAddComponent(Func<string> valueFactory)
        {
            try
            {
                var value = valueFactory();
                if (!string.IsNullOrEmpty(value))
                    components.Add(value);
            }
            catch
            {
                // Silently ignore failed sources
            }
        }
    }

    private static class Global
    {
        public static ulong RuntimeSelector { get; } =
            (ulong)Chi32.ApplyCascadingHashInterleave(
                HashString(BuildRuntimeFingerprint()), HashString(TimerTicks.ToString()));

        public static ulong CurrentIndex { get; set; } =
            (ulong)HashString(
                new string(TimerTicks.ToString().Reverse().ToArray()));

        public static ulong TimerTicks => (ulong)Stopwatch.GetTimestamp();
        public static object Lock { get; } = new();
    }

    #endregion
}