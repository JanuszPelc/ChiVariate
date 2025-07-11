// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            var selector = ChiMix64.MixValue(ChiMix64.InitialValue, (int)(uint)value);
            selector = ChiMix64.MixValue(selector, (int)(uint)(value >> 32));

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
        var index = ChiMix64.MixString(ChiMix64.InitialValue, @string);

        return Chi32.ApplyCascadingHashInterleave(selector, index);
    }

    #region Private and boierplate

    private static class Global
    {
        public static ulong RuntimeSelector { get; } = (ulong)GatherRandomness(() => Math.Cbrt(TimerTicks));
        public static ulong CurrentIndex { get; set; } = (ulong)GatherRandomness(() => Math.Sin(TimerTicks));

        public static ulong TimerTicks => (ulong)Stopwatch.GetTimestamp();
        public static object Lock { get; } = new();

        private static long GatherRandomness(Func<double> valueFactory)
        {
            var selectorComponents = new List<string>();
            AddComponent(selectorComponents, () => $"{valueFactory():C53}");
            AddComponent(selectorComponents, () => RuntimeInformation.OSDescription);
            AddComponent(selectorComponents, () => AppContext.BaseDirectory);
            AddComponent(selectorComponents, () => Environment.Version.ToString());
            AddComponent(selectorComponents, () => Environment.ProcessId.ToString());
            var selector = FinalizeComponents(selectorComponents);

            var indexComponents = new List<string>();
            AddComponent(indexComponents, () => $"{valueFactory()}");
            AddComponent(indexComponents, () => CultureInfo.CurrentCulture.DisplayName);
            AddComponent(indexComponents, () => DateTime.Now.ToLongDateString());
            AddComponent(indexComponents, () => DateTime.Now.ToLongTimeString());
            AddComponent(indexComponents, () => DateTime.Now.Ticks.ToString());
            var index = FinalizeComponents(indexComponents);

            return Chi32.ApplyCascadingHashInterleave(selector, index);

            static void AddComponent(List<string> components, Func<string> valueFactory)
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

            static long FinalizeComponents(List<string> components)
            {
                AddComponent(components, () => Guid.NewGuid().ToString());
                AddComponent(components, () => TimerTicks.ToString());

                new Random().Shuffle(CollectionsMarshal.AsSpan(components));

                return ChiMix64.MixString(
                    ChiMix64.InitialValue,
                    string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, components));
            }
        }
    }

    #endregion
}