// SPDX-License-Identifier: MIT
// See LICENSE file for full terms

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using ChiVariate.Providers;

namespace ChiVariate;

/// <summary>
///     Samples points within or on a circle.
/// </summary>
public readonly ref struct ChiSamplerSpatialCircle<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _radius;
    private readonly ChiPointSamplingMode _mode;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerSpatialCircle(ref TRng rng, T radius, ChiPointSamplingMode mode)
    {
        if (!T.IsFinite(radius) || radius < T.Zero)
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius cannot be negative.");

        _rng = ref rng;
        _radius = radius;
        _mode = mode;
    }

    /// <summary>
    ///     Samples a single random 2D point from the configured circle sampler.
    /// </summary>
    /// <returns>A new <see cref="ChiNum2{T}" /> point sampled from the circle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiNum2<T> Sample()
    {
        return _mode switch
        {
            ChiPointSamplingMode.InArea => SampleInArea(),
            ChiPointSamplingMode.OnPerimeter => SampleOnPerimeter(),
            _ => throw new UnreachableException()
        };
    }

    private ChiNum2<T> SampleInArea()
    {
        var uForRadius = ChiRealProvider.Next<TRng, T>(ref _rng);
        var r = _radius * ChiMath.Sqrt(uForRadius);

        var uForAngle = ChiRealProvider.Next<TRng, T>(ref _rng);
        var angle = uForAngle * T.Pi * (T.One + T.One);
        var sin = ChiMath.Sin(angle);
        var cos = ChiMath.Cos(angle);

        return new ChiNum2<T>(r * cos, r * sin);
    }

    private ChiNum2<T> SampleOnPerimeter()
    {
        var uForAngle = ChiRealProvider.Next<TRng, T>(ref _rng);
        var angle = uForAngle * T.Pi * (T.One + T.One);
        var sin = ChiMath.Sin(angle);
        var cos = ChiMath.Cos(angle);

        return new ChiNum2<T>(_radius * cos, _radius * sin);
    }

    /// <summary>
    ///     Generates a sequence of random points from the circle sampler.
    /// </summary>
    /// <param name="count">The number of points to sample.</param>
    /// <returns>An enumerable collection of points sampled from the circle.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<ChiNum2<T>> Sample(int count)
    {
        var enumerable = ChiEnumerable<ChiNum2<T>>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }
}

/// <summary>
///     Samples points within or on a cube.
/// </summary>
public readonly ref struct ChiSamplerSpatialCube<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _extents;
    private readonly ChiPointSamplingMode _mode;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerSpatialCube(ref TRng rng, T extents, ChiPointSamplingMode mode)
    {
        if (!T.IsFinite(extents) || extents < T.Zero)
            throw new ArgumentOutOfRangeException(nameof(extents), "Extents cannot be negative.");

        _rng = ref rng;
        _extents = extents;
        _mode = mode;
    }

    /// <summary>
    ///     Samples a single random 3D point from the configured cube sampler.
    /// </summary>
    /// <returns>A new <see cref="ChiNum3{T}" /> point sampled from the cube.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiNum3<T> Sample()
    {
        return _mode switch
        {
            ChiPointSamplingMode.InVolume => SampleInVolume(),
            ChiPointSamplingMode.OnSurface => SampleOnSurface(),
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    ///     Generates a sequence of random points from the cube sampler.
    /// </summary>
    /// <param name="count">The number of points to sample.</param>
    /// <returns>An enumerable collection of points sampled from the cube.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<ChiNum3<T>> Sample(int count)
    {
        var enumerable = ChiEnumerable<ChiNum3<T>>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }

    private ChiNum3<T> SampleInVolume()
    {
        var x = _rng.Uniform(-_extents, _extents).Sample();
        var y = _rng.Uniform(-_extents, _extents).Sample();
        var z = _rng.Uniform(-_extents, _extents).Sample();
        return new ChiNum3<T>(x, y, z);
    }

    private ChiNum3<T> SampleOnSurface()
    {
        var faceIndex = _rng.Chance().Next(6);
        var u = _rng.Uniform(-_extents, _extents).Sample();
        var v = _rng.Uniform(-_extents, _extents).Sample();

        return faceIndex switch
        {
            0 => new ChiNum3<T>(u, v, _extents),
            1 => new ChiNum3<T>(u, v, -_extents),
            2 => new ChiNum3<T>(u, _extents, v),
            3 => new ChiNum3<T>(u, -_extents, v),
            4 => new ChiNum3<T>(_extents, u, v),
            5 => new ChiNum3<T>(-_extents, u, v),
            _ => default // Unreachable
        };
    }
}

/// <summary>
///     Samples points within or on a sphere.
/// </summary>
public readonly ref struct ChiSamplerSpatialSphere<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _radius;
    private readonly ChiPointSamplingMode _mode;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerSpatialSphere(ref TRng rng, T radius, ChiPointSamplingMode mode)
    {
        if (!T.IsFinite(radius) || radius < T.Zero)
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius cannot be negative.");

        _rng = ref rng;
        _radius = radius;
        _mode = mode;
    }

    /// <summary>
    ///     Samples a single random 3D point from the configured sphere sampler.
    /// </summary>
    /// <returns>A new <see cref="ChiNum3{T}" /> point sampled from the sphere.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiNum3<T> Sample()
    {
        return _mode switch
        {
            ChiPointSamplingMode.InVolume => SampleInVolume(),
            ChiPointSamplingMode.OnSurface => SampleOnSurface(),
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    ///     Generates a sequence of random points from the sphere sampler.
    /// </summary>
    /// <param name="count">The number of points to sample.</param>
    /// <returns>An enumerable collection of points sampled from the sphere.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<ChiNum3<T>> Sample(int count)
    {
        var enumerable = ChiEnumerable<ChiNum3<T>>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }

    private ChiNum3<T> SampleInVolume()
    {
        var unitDirection = SampleOnUnitSphere();
        var u = ChiRealProvider.Next<TRng, T>(ref _rng);
        var scaledMagnitude = _radius * ChiMath.Cbrt(u);

        return new ChiNum3<T>(
            unitDirection.X * scaledMagnitude,
            unitDirection.Y * scaledMagnitude,
            unitDirection.Z * scaledMagnitude
        );
    }

    private ChiNum3<T> SampleOnUnitSphere()
    {
        T x, y, s;
        do
        {
            x = _rng.Uniform(-T.One, T.One).Sample();
            y = _rng.Uniform(-T.One, T.One).Sample();
            s = x * x + y * y;
        } while (s >= T.One);

        var two = T.One + T.One;
        var common = two * ChiMath.Sqrt(T.One - s);

        var z = T.One - two * s;
        var finalX = x * common;
        var finalY = y * common;

        return new ChiNum3<T>(finalX, finalY, z);
    }

    private ChiNum3<T> SampleOnSurface()
    {
        var unitDirection = SampleOnUnitSphere();
        return new ChiNum3<T>(
            unitDirection.X * _radius,
            unitDirection.Y * _radius,
            unitDirection.Z * _radius
        );
    }
}

/// <summary>
///     Samples points within or on a square.
/// </summary>
public readonly ref struct ChiSamplerSpatialSquare<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private static readonly T Two = T.One + T.One;
    private static readonly T Eight = Two * Two * Two;

    private readonly ref TRng _rng;
    private readonly T _extents;
    private readonly ChiPointSamplingMode _mode;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerSpatialSquare(ref TRng rng, T extents, ChiPointSamplingMode mode)
    {
        if (!T.IsFinite(extents) || extents < T.Zero)
            throw new ArgumentOutOfRangeException(nameof(extents), "Extents cannot be negative.");

        _rng = ref rng;
        _extents = extents;
        _mode = mode;
    }

    /// <summary>
    ///     Samples a single random 2D point from the configured square sampler.
    /// </summary>
    /// <returns>A new <see cref="ChiNum2{T}" /> point sampled from the square.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiNum2<T> Sample()
    {
        return _mode switch
        {
            ChiPointSamplingMode.InArea => SampleInArea(),
            ChiPointSamplingMode.OnPerimeter => SampleOnPerimeter(),
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    ///     Generates a sequence of random points from the square sampler.
    /// </summary>
    /// <param name="count">The number of points to sample.</param>
    /// <returns>An enumerable collection of points sampled from the square.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<ChiNum2<T>> Sample(int count)
    {
        var enumerable = ChiEnumerable<ChiNum2<T>>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }

    private ChiNum2<T> SampleInArea()
    {
        var x = _rng.Uniform(-_extents, _extents).Sample();
        var y = _rng.Uniform(-_extents, _extents).Sample();
        return new ChiNum2<T>(x, y);
    }

    private ChiNum2<T> SampleOnPerimeter()
    {
        var perimeter = Eight * _extents;
        var p = _rng.Uniform(T.Zero, perimeter).Sample();
        var sideLength = Two * _extents;

        if (p < sideLength)
            return new ChiNum2<T>(p - _extents, _extents);

        p -= sideLength;
        if (p < sideLength)
            return new ChiNum2<T>(_extents, _extents - p);

        p -= sideLength;
        if (p < sideLength)
            return new ChiNum2<T>(_extents - p, -_extents);

        p -= sideLength;
        return new ChiNum2<T>(-_extents, p - _extents);
    }
}

/// <summary>
///     An intermediate type that provides access to a toolkit for uniform spatial sampling.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerSpatialExtensions.Spatial{TRng}" /> method.
/// </remarks>
public readonly ref struct ChiSamplerSpatial<TRng>
    where TRng : struct, IChiRngSource<TRng>
{
    private readonly ref TRng _rng;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerSpatial(ref TRng rng)
    {
        _rng = ref rng;
    }

    /// <summary>
    ///     Returns a sampler for generating random points uniformly distributed within a circle.
    /// </summary>
    /// <param name="radius">The radius of the circle. Must be non-negative.</param>
    /// <example>
    ///     <code><![CDATA[
    /// // Simulate a circular area-of-effect (AoE) attack
    /// var aoeCenter = new Vector2(100, 50);
    /// var pointInAoe = rng.Spatial().InCircle(10.0f).Sample().AsVector2();
    /// var impactPosition = aoeCenter + pointInAoe;
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSamplerSpatialCircle<TRng, T> InCircle<T>(T radius)
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerSpatialCircle<TRng, T>(ref _rng, radius, ChiPointSamplingMode.InArea);
    }

    /// <summary>
    ///     Returns a sampler for generating random points uniformly distributed on the perimeter of a circle.
    /// </summary>
    /// <param name="radius">The radius of the circle. Must be non-negative.</param>
    /// <example>
    ///     <code><![CDATA[
    /// // Generate a random 2D direction vector
    /// var direction = rng.Spatial().OnCircle(1.0f).Sample().AsVector2();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSamplerSpatialCircle<TRng, T> OnCircle<T>(T radius)
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerSpatialCircle<TRng, T>(ref _rng, radius, ChiPointSamplingMode.OnPerimeter);
    }

    /// <summary>
    ///     Returns a sampler for generating random points uniformly distributed within a square.
    /// </summary>
    /// <param name="extents">The half-width of the square. Must be non-negative.</param>
    /// <example>
    ///     <code><![CDATA[
    /// // Place an object randomly within a rectangular zone
    /// var zoneCenter = new Vector2(0, 0);
    /// var zoneSize = new Vector2(200, 100);
    /// var pointInUnitSquare = rng.Spatial().InSquare(1.0f).Sample().AsVector2();
    /// var objectPosition = zoneCenter + pointInUnitSquare * (zoneSize / 2.0f);
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSamplerSpatialSquare<TRng, T> InSquare<T>(T extents)
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerSpatialSquare<TRng, T>(ref _rng, extents, ChiPointSamplingMode.InArea);
    }

    /// <summary>
    ///     Returns a sampler for generating random points uniformly distributed on the perimeter of a square.
    /// </summary>
    /// <param name="extents">The half-width of the square. Must be non-negative.</param>
    /// <example>
    ///     <code><![CDATA[
    /// // Spawn an enemy at a random point along the wall of a square room
    /// var roomSize = 50.0f;
    /// var spawnPoint = rng.Spatial().OnSquare(roomSize / 2f).Sample().AsVector2();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSamplerSpatialSquare<TRng, T> OnSquare<T>(T extents)
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerSpatialSquare<TRng, T>(ref _rng, extents, ChiPointSamplingMode.OnPerimeter);
    }

    /// <summary>
    ///     Returns a sampler for generating random points uniformly distributed within a sphere.
    /// </summary>
    /// <param name="radius">The radius of the sphere. Must be non-negative.</param>
    /// <example>
    ///     <code><![CDATA[
    /// // Spawn a particle inside a spherical explosion volume
    /// var explosionCenter = new Vector3(0, 50, 0);
    /// var pointInVolume = rng.Spatial().InSphere(25.0f).Sample().AsVector3();
    /// var particlePosition = explosionCenter + pointInVolume;
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSamplerSpatialSphere<TRng, T> InSphere<T>(T radius)
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerSpatialSphere<TRng, T>(ref _rng, radius, ChiPointSamplingMode.InVolume);
    }

    /// <summary>
    ///     Returns a sampler for generating random points uniformly distributed on the surface of a sphere.
    /// </summary>
    /// <param name="radius">The radius of the sphere. Must be non-negative.</param>
    /// <example>
    ///     <code><![CDATA[
    /// // Generate a random 3D direction vector for a projectile
    /// var projectileDirection = rng.Spatial().OnSphere(1.0f).Sample().AsVector3();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSamplerSpatialSphere<TRng, T> OnSphere<T>(T radius)
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerSpatialSphere<TRng, T>(ref _rng, radius, ChiPointSamplingMode.OnSurface);
    }

    /// <summary>
    ///     Returns a sampler for generating random points uniformly distributed within a cube.
    /// </summary>
    /// <param name="extents">The half-width of the cube. Must be non-negative.</param>
    /// <example>
    ///     <code><![CDATA[
    /// // Place a collectible randomly inside a cubic room
    /// var roomCenter = Vector3.Zero;
    /// var roomExtents = new Vector3(10, 5, 10);
    /// var pointInUnitCube = rng.Spatial().InCube(1.0f).Sample().AsVector3();
    /// var collectiblePosition = roomCenter + pointInUnitCube * roomExtents;
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSamplerSpatialCube<TRng, T> InCube<T>(T extents)
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerSpatialCube<TRng, T>(ref _rng, extents, ChiPointSamplingMode.InVolume);
    }

    /// <summary>
    ///     Returns a sampler for generating random points uniformly distributed on the surface of a cube.
    /// </summary>
    /// <param name="extents">The half-width of the cube. Must be non-negative.</param>
    /// <example>
    ///     <code><![CDATA[
    /// // Spawn a decal on a random face of a box
    /// var boxPosition = new Vector3(20, 0, 0);
    /// var boxExtents = 5.0f;
    /// var pointOnSurface = rng.Spatial().OnCube(boxExtents).Sample().AsVector3();
    /// var decalPosition = boxPosition + pointOnSurface;
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiSamplerSpatialCube<TRng, T> OnCube<T>(T extents)
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerSpatialCube<TRng, T>(ref _rng, extents, ChiPointSamplingMode.OnSurface);
    }
}

/// <summary>
///     Provides extension methods for spatial sampling.
/// </summary>
public static class ChiSamplerSpatialExtensions
{
    /// <summary>
    ///     Returns a toolkit for uniform spatial sampling within or on the surface of geometric primitives.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <returns>A sampler that provides access to various spatial sampling methods.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Provides a simple, reliable API for common sampling tasks like picking a random position in
    ///         an area (`InSquare`, `InCube`) or generating a random direction vector (`OnCircle`, `OnSphere`). It handles all
    ///         mathematical corrections internally to prevent common statistical biases.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Get a random direction vector
    /// var direction = rng.Spatial().OnSphere(1.0f).Sample().AsVector3();
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerSpatial<TRng> Spatial<TRng>(this ref TRng rng)
        where TRng : struct, IChiRngSource<TRng>
    {
        return new ChiSamplerSpatial<TRng>(ref rng);
    }
}