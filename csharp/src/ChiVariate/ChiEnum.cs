using System.Reflection;
using System.Runtime.CompilerServices;

namespace ChiVariate;

/// <summary>
///     Provides cached access to enum values and their associated weights for efficient random selection.
/// </summary>
/// <typeparam name="TEnum">The enum type to provide cached data for.</typeparam>
public static class ChiEnum<TEnum> where TEnum : unmanaged, Enum
{
    /// <summary>
    ///     Gets all values of the enum type <typeparamref name="TEnum" /> as a read-only span.
    ///     The values are cached for performance and returned in the same order as weights.
    /// </summary>
    /// <value>
    ///     A read-only span containing all enum values in declaration order.
    /// </value>
    public static ReadOnlySpan<TEnum> Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LocalCache.Enum<TEnum>.Values;
    }

    /// <summary>
    ///     Gets the weights for each enum value as a read-only span of doubles.
    ///     Weights correspond to enum values in the same order as <see cref="Values" />.
    ///     Fields decorated with <see cref="ChiEnumWeightAttribute" /> use their specified weight,
    ///     while undecorated fields default to 1.0.
    /// </summary>
    /// <value>
    ///     A read-only span of weight values, or an empty span if all enum fields have default weights.
    /// </value>
    /// <remarks>
    ///     If all enum fields have equal probability (no custom weights), this span will be empty
    ///     to optimize memory usage and indicate uniform distribution.
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// public enum Priority
    /// {
    ///     [ChiEnumWeight(10.0)] High,    // 62.5% probability (10/16)
    ///     [ChiEnumWeight(5.0)]  Medium,  // 31.25% probability (5/16)
    ///     Low                            // 6.25% probability (1/16)
    /// }
    /// // Total weight: 10 + 5 + 1 = 16
    ///  
    /// var weights = ChiEnum<Priority>.Weights; // [10.0, 5.0, 1.0]
    /// var values = ChiEnum<Priority>.Values;   // [High, Medium, Low]
    /// ]]></code>
    /// </example>
    public static ReadOnlySpan<double> Weights
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => LocalCache.Enum<TEnum>.Weights;
    }
}

/// <summary>
///     Specifies the weight for an enum field when used in weighted random selection.
///     Higher weights increase the probability of selection in categorical distributions.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ChiEnumWeightAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the ChiRngWeightAttribute class.
    /// </summary>
    /// <param name="weight">The weight value. Must be non-negative. Higher values increase selection probability.</param>
    public ChiEnumWeightAttribute(double weight)
    {
        Weight = weight;
    }

    /// <summary>
    ///     Gets the weight value for this enum field.
    /// </summary>
    public double Weight { get; }
}

file static class LocalCache
{
    public static class Enum<TEnum>
        where TEnum : unmanaged, Enum
    {
        private static readonly Lazy<(TEnum[] values, double[] weights)> LazyValuesAndWeights =
            new(ExtractValuesAndWeights<TEnum>);

        public static ReadOnlySpan<TEnum> Values
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => LazyValuesAndWeights.Value.values.AsSpan();
        }

        public static ReadOnlySpan<double> Weights
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => LazyValuesAndWeights.Value.weights.AsSpan();
        }

        private static (T[] values, double[] weights) ExtractValuesAndWeights<T>() where T : unmanaged, Enum
        {
            var enumType = typeof(T);
            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            var results = fields
                .Select(field => new
                {
                    Value = (T)(field.GetValue(null) ?? throw new ArgumentNullException()),
                    Attribute = field.GetCustomAttribute<ChiEnumWeightAttribute>()
                })
                .ToArray();

            var hasWeights = results.Any(r => r.Attribute != null);

            return (
                values: results.Select(r => r.Value).ToArray(),
                weights: hasWeights
                    ? results.Select(r => r.Attribute?.Weight ?? 1.0).ToArray()
                    : []
            );
        }
    }
}