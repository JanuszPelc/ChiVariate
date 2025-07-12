using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace ChiVariate.Tests.DocExamplesTests;

#pragma warning disable CS1591

public class StatelessSamplerTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 100_000;

    [Theory]
    [InlineData(1.0)] // Standard scale
    [InlineData(0.5)] // Lower temperature/higher mass -> slower speeds
    [InlineData(10.0)] // Higher temperature/lower mass -> faster speeds
    public void Sample_ProducesDistributionWithCorrectMean(double a)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("MaxwellBoltzmann", a));
        var sampler = rng.MaxwellBoltzmann(a);
        var expectedMean = 2.0 * a * Math.Sqrt(2.0 / Math.PI);
        var maxBound = expectedMean * 3;
        var histogram = new Histogram(0, maxBound, 150);

        // Act
        foreach (var sample in sampler.Sample(SampleCount))
            if (sample < maxBound)
                histogram.AddSample(sample);

        // Assert
        histogram.DebugPrint(testOutputHelper, $"Maxwell-Boltzmann (a={a})");

        var actualMean = histogram.CalculateMean();
        actualMean.Should().BeApproximately(expectedMean, expectedMean * 0.05,
            "because the sample mean should match the theoretical mean 2a * sqrt(2/π).");
    }

    [Fact]
    public void Sample_Decimal_ProducesCorrectStatistics()
    {
        // Arrange
        const decimal a = 1.5m;
        var rng = new ChiRng(ChiSeed.Scramble("MaxwellBoltzmannDecimal", (double)a));
        var sampler = rng.MaxwellBoltzmann(a);

        // Theoretical properties
        var expectedMean = 2.0m * a * (decimal)Math.Sqrt(2.0 / Math.PI);
        var expectedMode = (decimal)Math.Sqrt(2.0) * a;

        double sum = 0;
        var samples = new List<double>(SampleCount);

        // Act
        foreach (var sample in sampler.Sample(SampleCount))
        {
            var s = (double)sample;
            sum += s;
            samples.Add(s);
        }

        // Assert
        var actualMean = sum / SampleCount;
        actualMean.Should().BeApproximately((double)expectedMean, (double)expectedMean * 0.05,
            "because the mean should be correct for high-precision decimal values.");

        // For mode, we can check the histogram of the collected samples
        var maxBound = (double)expectedMean * 3;
        var histogram = new Histogram(0, maxBound, 150);
        foreach (var s in samples)
            if (s < maxBound)
                histogram.AddSample(s);

        var actualMode = histogram.CalculateMode();
        actualMode.Should().BeApproximately((double)expectedMode, (double)expectedMode * 0.07,
            "because the mode should be correct for high-precision decimal values.");
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        // Arrange
        var rng = new ChiRng(1337);

        // Act
        var result = rng.MaxwellBoltzmann(10.0).Sample();

        // Assert
        result.Should().BeApproximately(14.33683, 0.00001);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(double.NaN)]
    public void MaxwellBoltzmann_WithInvalidScale_ThrowsArgumentOutOfRangeException(double invalidA)
    {
        // Arrange
        var rng = new ChiRng();
        var act = () => { _ = rng.MaxwellBoltzmann(invalidA); };

        // Act & Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("a");
    }

    [Fact]
    public void CryptoRng_MultivariateNormalSampling_ProducesExpectedMarginals()
    {
        // Arrange
        var mean = ChiMatrix.With([10.0, 20.0, 30.0]);
        var covariance = ChiMatrix.With(
            [1.0, 0.0, 0.0], // Var(X1)=1 -> StdDev=1
            [0.0, 4.0, 0.0], // Var(X2)=4 -> StdDev=2
            [0.0, 0.0, 9.0]); // Var(X3)=9 -> StdDev=3

        var rng = new CryptoRng();

        var histogramX1 = new Histogram(5, 15, 100); // Mean 10, StdDev 1
        var histogramX2 = new Histogram(10, 30, 100); // Mean 20, StdDev 2
        var histogramX3 = new Histogram(15, 45, 100); // Mean 30, StdDev 3

        // Act
        const int sampleCount = 100_000;
        for (var i = 0; i < sampleCount; i++)
        {
            using var destination = rng.MultivariateNormal(mean, covariance).Sample();

            histogramX1.AddSample(destination[0]);
            histogramX2.AddSample(destination[1]);
            histogramX3.AddSample(destination[2]);
        }

        // Assert
        const string methodName = nameof(CryptoRng_MultivariateNormalSampling_ProducesExpectedMarginals);

        histogramX1.DebugPrint(testOutputHelper, $"{methodName} - Marginal Distribution for X1");
        histogramX1.AssertIsNormal(10.0, 1.0, 0.1);

        histogramX1.DebugPrint(testOutputHelper, $"{methodName} - Marginal Distribution for X2");
        histogramX2.AssertIsNormal(20.0, 2.0, 0.1);

        histogramX1.DebugPrint(testOutputHelper, $"{methodName} - Marginal Distribution for X3");
        histogramX3.AssertIsNormal(30.0, 3.0, 0.1);
    }

    [Fact]
    public void CustomCryptoRng_WithCustomMaxwellBoltzmannSampler_ProducesStatisticallyPlausibleResult()
    {
        // Arrange
        const double expectedMeanTotalEnergy = 675_000_000.0; // E = N * 0.5 * m * (3 * a^2)
        const double threeSigma = 95_000_000.0; // Derived from the variance of v^2
        const double lowerBound = expectedMeanTotalEnergy - threeSigma;
        const double upperBound = expectedMeanTotalEnergy + threeSigma;

        // Act
        var totalKineticEnergy = SimulateParticleSpeeds();

        // Assert
        totalKineticEnergy.Should().BeInRange(lowerBound, upperBound,
            "because the result of the simulation should be statistically close to the theoretical mean.");

        return;

        // ReSharper disable once Xunit.XunitTestWithConsoleOutput
        // ReSharper disable once VariableHidesOuterVariable
        double SimulateParticleSpeeds()
        {
            // --- example code begin ---
            var cryptoRng = new CryptoRng(); // Crypto RNG has no seed
            const double particleMass = 0.5; // kg
            const double speedScale = 300.0; // The 'a' parameter (m/s)
            const int sampleSize = 10_000; // 10 thousand samples

            var totalKineticEnergy = cryptoRng
                .MaxwellBoltzmann(speedScale)
                .Sample(sampleSize)
                .Aggregate(0.0, (totalEnergy, speed) =>
                    totalEnergy + 0.5 * particleMass * speed * speed
                );

            Console.WriteLine(
                $"Total Kinetic Energy: {totalKineticEnergy:N0} Joules");
            // --- example code end ---

            return totalKineticEnergy;
        }
    }

    // --- example code begin ---
    // Integrating a cryptographically strong random number generator
    public record struct CryptoRng : IChiRngSource<CryptoRng>
    {
        public static uint NextUInt32(ref CryptoRng _)
        {
            Span<byte> buffer = stackalloc byte[4];
            RandomNumberGenerator.Fill(buffer);
            return Unsafe.ReadUnaligned<uint>(ref buffer[0]);
        }

        public static ulong NextUInt64(ref CryptoRng _)
        {
            Span<byte> buffer = stackalloc byte[8];
            RandomNumberGenerator.Fill(buffer);
            return Unsafe.ReadUnaligned<ulong>(ref buffer[0]);
        }
    }
    // --- example code end ---
}

// --- example code begin ---
/// <summary>
///     Samples from the Maxwell-Boltzmann distribution, which models particle speeds in thermal equilibrium.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerMaxwellBoltzmannExtensions.MaxwellBoltzmann{TRng, T}" />
///     method.
/// </remarks>
public ref struct ChiSamplerMaxwellBoltzmann<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly ref TRng _rng;
    private readonly T _a;
    private ChiSamplerChi<TRng, T> _chiSampler;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerMaxwellBoltzmann(ref TRng rng, T a)
    {
        if (!T.IsFinite(a) || a <= T.Zero)
            throw new ArgumentOutOfRangeException(nameof(a), "Scale parameter 'a' must be positive.");

        _rng = ref rng;
        _a = a;

        // The Maxwell-Boltzmann distribution is a Chi distribution
        // with 3 degrees of freedom, scaled by the parameter 'a'.
        _chiSampler = _rng.Chi(T.CreateTruncating(3.0));
    }

    /// <summary>
    ///     Samples a single random speed from the configured Maxwell-Boltzmann distribution.
    /// </summary>
    /// <returns>A new speed value sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Sample()
    {
        return _a * _chiSampler.Sample();
    }

    /// <summary>
    ///     Generates a sequence of random speeds from the Maxwell-Boltzmann distribution.
    /// </summary>
    /// <param name="count">The number of values to sample from the distribution.</param>
    /// <returns>An enumerable collection of speed values sampled from the distribution.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<T> Sample(int count)
    {
        var enumerable = ChiEnumerable<T>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }
}

public static class ChiSamplerMaxwellBoltzmannExtensions
{
    /// <summary>
    ///     Returns a sampler for the Maxwell-Boltzmann distribution, which models particle speeds
    ///     in gases at thermal equilibrium.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="a">The scale parameter, related to temperature and particle mass. Must be positive.</param>
    /// <returns>A sampler that can be used to generate random particle speeds.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> Ideal for physics simulations, computational chemistry, or generating realistic velocity
    ///         magnitudes for particle effects in games.
    ///     </para>
    ///     <para><b>Performance:</b> Amortized O(1) per sample.</para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerMaxwellBoltzmann<TRng, T> MaxwellBoltzmann<TRng, T>(this ref TRng rng, T a)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerMaxwellBoltzmann<TRng, T>(ref rng, a);
    }
}
// --- example code end ---