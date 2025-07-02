using System.Numerics;

namespace ChiVariate;

/// <summary>
///     Defines the contract for a struct-based, pseudo-random number generator (PRNG) source
///     that can be used by the ChiVariate library.
/// </summary>
/// <typeparam name="TRng">The type of the PRNG struct that implements this interface.</typeparam>
/// <remarks>
///     This interface uses static abstract members to allow for compile-time dispatch,
///     ensuring zero-cost abstraction when plugging in custom RNGs.
/// </remarks>
public interface IChiRngSource<TRng>
    where TRng : IChiRngSource<TRng>
{
    /// <summary>
    ///     Returns the next 32-bit unsigned integer from the random number sequence.
    /// </summary>
    /// <param name="this">A reference to the PRNG state, which will be updated by this call.</param>
    /// <returns>A 32-bit unsigned integer with a uniform distribution of bits.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Warning:</b> This method provides raw, uniformly-distributed bits and should not be used directly
    ///         for generating bounded random numbers, as this can lead to statistical bias (e.g., via a modulo operation).
    ///     </para>
    ///     <para>
    ///         This method is intended for internal use by ChiVariate's samplers, which employ bias-free techniques.
    ///         When implementing a custom generator, focus on providing a high-quality, uniform bitstream.
    ///     </para>
    /// </remarks>
    public static abstract uint NextUInt32(ref TRng @this);

    /// <summary>
    ///     Returns the next 64-bit unsigned integer from the random number sequence.
    /// </summary>
    /// <param name="this">A reference to the PRNG state, which will be updated by this call.</param>
    /// <returns>A 64-bit unsigned integer with a uniform distribution of bits.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Warning:</b> This method provides raw, uniformly-distributed bits and should not be used directly
    ///         for generating bounded random numbers, as this can lead to statistical bias (e.g., via a modulo operation).
    ///     </para>
    ///     <para>
    ///         This method is intended for internal use by ChiVariate's samplers, which employ bias-free techniques.
    ///         When implementing a custom generator, focus on providing a high-quality, uniform bitstream.
    ///     </para>
    /// </remarks>
    public static abstract ulong NextUInt64(ref TRng @this);
}

/// <summary>
///     Specifies options for controlling the boundaries of a continuous random number interval.
/// </summary>
[Flags]
public enum ChiIntervalOptions
{
    /// <summary>
    ///     Represents the default left-closed, right-open interval [min, max).
    /// </summary>
    None = 0,

    /// <summary>
    ///     Excludes the minimum value from the possible results, forming an open-left interval (min, ...).
    /// </summary>
    ExcludeMin = 1 << 0,

    /// <summary>
    ///     Includes the maximum value in the possible results, forming a closed-right interval ..., max].
    /// </summary>
    IncludeMax = 1 << 1
}

/// <summary>
///     Specifies the mode for spatial sampling.
/// </summary>
internal enum ChiPointSamplingMode
{
    /// <summary>Generate a position vector uniformly within the area of a 2D shape.</summary>
    InArea,

    /// <summary>Generate a position vector uniformly on the perimeter of a 2D shape.</summary>
    OnPerimeter,

    /// <summary>Generate a position vector uniformly within the volume of a 3D shape.</summary>
    InVolume,

    /// <summary>Generate a position vector uniformly on the surface of a 3D shape.</summary>
    OnSurface
}

/// <summary>
///     Represents a two-dimensional numeric vector used for data transfer.
/// </summary>
/// <typeparam name="T">The numeric type of the components.</typeparam>
/// <param name="X">The X-component of the vector.</param>
/// <param name="Y">The Y-component of the vector.</param>
public readonly record struct ChiNum2<T>(T X, T Y) where T : INumber<T>;

/// <summary>
///     Represents a three-dimensional numeric vector used for data transfer.
/// </summary>
/// <typeparam name="T">The numeric type of the components.</typeparam>
/// <param name="X">The X-component of the vector.</param>
/// <param name="Y">The Y-component of the vector.</param>
/// <param name="Z">The Z-component of the vector.</param>
public readonly record struct ChiNum3<T>(T X, T Y, T Z) where T : INumber<T>;

/// <summary>
///     Provides extension methods for converting ChiVariate numeric vectors to standard `System.Numerics` types.
/// </summary>
public static class ChiNumExtensions
{
    /// <summary>
    ///     Converts a <see cref="ChiNum2{T}" /> of type <see cref="float" /> to a <see cref="System.Numerics.Vector2" />.
    /// </summary>
    /// <param name="this">The <see cref="ChiNum2{T}" /> to convert.</param>
    /// <returns>A new <see cref="Vector2" /> with the same component values.</returns>
    public static Vector2 AsVector2(this ChiNum2<float> @this)
    {
        return new Vector2(@this.X, @this.Y);
    }

    /// <summary>
    ///     Converts a <see cref="ChiNum3{T}" /> of type <see cref="float" /> to a <see cref="System.Numerics.Vector3" />.
    /// </summary>
    /// <param name="this">The <see cref="ChiNum3{T}" /> to convert.</param>
    /// <returns>A new <see cref="Vector3" /> with the same component values.</returns>
    public static Vector3 AsVector3(this ChiNum3<float> @this)
    {
        return new Vector3(@this.X, @this.Y, @this.Z);
    }
}

/// <summary>
///     Specifies the generation mode for a quasi-random sequence.
/// </summary>
public enum ChiSequenceMode
{
    /// <summary>
    ///     Generates the canonical, deterministic sequence. This is useful for
    ///     testing, debugging, and cross-library validation where a fixed, predictable
    ///     set of points is required.
    /// </summary>
    Canonical,

    /// <summary>
    ///     Generates a sequence with a random starting offset or scramble seed.
    ///     This is the recommended default as it improves statistical properties for
    ///     Monte Carlo methods by breaking up potential correlations.
    /// </summary>
    Randomized
}