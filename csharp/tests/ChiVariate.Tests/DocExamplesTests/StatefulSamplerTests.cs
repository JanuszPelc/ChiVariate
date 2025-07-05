using System.Numerics;
using System.Runtime.CompilerServices;
using AwesomeAssertions;
using ChiVariate.Generators;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace ChiVariate.Tests.DocExamplesTests;

#pragma warning disable CS1591

public class StatefulSamplerTests(ITestOutputHelper testOutputHelper)
{
    private const int SampleCount = 50_000;

    [Theory]
    [InlineData(0.8)] // Strong positive correlation
    [InlineData(-0.5)] // Moderate negative correlation
    [InlineData(0.0)] // No correlation (should be independent)
    [InlineData(1.0)] // Perfect positive correlation
    [InlineData(-1.0)] // Perfect negative correlation
    public void Sample_ProducesCorrectlyCorrelatedStandardNormalMarginals(double correlation)
    {
        // Arrange
        var rng = new ChiRng(ChiSeed.Scramble("CorrelatedNormals", correlation));
        var sampler = rng.CorrelatedNormals(correlation);

        var z1Samples = new List<double>(SampleCount);
        var z2Samples = new List<double>(SampleCount);

        var histogram1 = new Histogram(-4, 4, 100);
        var histogram2 = new Histogram(-4, 4, 100);

        // Act
        foreach (var (z1, z2) in sampler.Sample(SampleCount))
        {
            z1Samples.Add(z1);
            z2Samples.Add(z2);
            histogram1.AddSample(z1);
            histogram2.AddSample(z2);
        }

        // Assert
        histogram1.DebugPrint(testOutputHelper, $"Z1 Marginal (ρ={correlation})");
        histogram1.AssertIsNormal(0.0, 1.0, 0.05);

        histogram2.DebugPrint(testOutputHelper, $"Z2 Marginal (ρ={correlation})");
        histogram2.AssertIsNormal(0.0, 1.0, 0.05);

        var sampleCorrelation = CalculateSampleCorrelation(z1Samples, z2Samples);
        sampleCorrelation.Should().BeApproximately(correlation, 0.05,
            "because the generated pairs should have the specified correlation.");
    }

    [Fact]
    public void Sample_Decimal_ProducesCorrectCorrelation()
    {
        // Arrange
        const decimal correlation = 0.6m;
        var rng = new ChiRng(ChiSeed.Scramble("CorrelatedNormalsDecimal", (double)correlation));
        var sampler = rng.CorrelatedNormals(correlation);

        var z1Samples = new List<decimal>(SampleCount);
        var z2Samples = new List<decimal>(SampleCount);

        // Act
        foreach (var (z1, z2) in sampler.Sample(SampleCount))
        {
            z1Samples.Add(z1);
            z2Samples.Add(z2);
        }

        // Assert
        var sampleCorrelation = CalculateSampleCorrelation(z1Samples, z2Samples);
        ((double)sampleCorrelation).Should().BeApproximately((double)correlation, 0.05);
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        // Arrange
        var rng = new ChiRng(1337);

        // Act
        var (z1, z2) = rng.CorrelatedNormals(0.75).Sample();

        // Assert
        z1.Should().BeApproximately(-0.134114, 0.00001);
        z2.Should().BeApproximately(-0.900725, 0.00001);
    }

    [Theory]
    [InlineData(-1.1)]
    [InlineData(1.1)]
    [InlineData(double.NaN)]
    public void CorrelatedNormals_WithInvalidCorrelation_ThrowsArgumentOutOfRangeException(double invalidCorrelation)
    {
        // Arrange
        var rng = new ChiRng();
        var act = () => { _ = rng.CorrelatedNormals(invalidCorrelation); };

        // Act & Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("correlation");
    }

    // ====================================================================
    // Helper Methods
    // ====================================================================

    private static T CalculateSampleCorrelation<T>(IReadOnlyList<T> x, IReadOnlyList<T> y) where T : IFloatingPoint<T>
    {
        var n = T.CreateChecked(x.Count);
        if (n < T.CreateChecked(2)) return T.Zero;

        var meanX = x.Aggregate(T.Zero, (acc, val) => acc + val) / n;
        var meanY = y.Aggregate(T.Zero, (acc, val) => acc + val) / n;

        var sumCov = T.Zero;
        var sumSqX = T.Zero;
        var sumSqY = T.Zero;

        for (var i = 0; i < x.Count; i++)
        {
            var devX = x[i] - meanX;
            var devY = y[i] - meanY;
            sumCov += devX * devY;
            sumSqX += devX * devX;
            sumSqY += devY * devY;
        }

        var denominator = ChiMath.Sqrt(sumSqX * sumSqY);
        return denominator == T.Zero ? T.Zero : sumCov / denominator;
    }
}

// --- example code begin ---
/// <summary>
///     A sampler for correlated standard normal variable pairs.
/// </summary>
/// <remarks>
///     This struct is constructed by the <see cref="ChiSamplerCorrelatedNormalsExtensions.CorrelatedNormals{TRng, T}" />
///     method.
/// </remarks>
public ref struct ChiSamplerCorrelatedNormals<TRng, T>
    where TRng : struct, IChiRngSource<TRng>
    where T : IFloatingPoint<T>
{
    private readonly T _correlation;
    private readonly T _sqrtTerm;

    private ChiStatefulNormalGenerator<TRng, T> _normalGenerator;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ChiSamplerCorrelatedNormals(ref TRng rng, T correlation)
    {
        if (!T.IsFinite(correlation) || correlation < -T.One || correlation > T.One)
            throw new ArgumentOutOfRangeException(nameof(correlation), "Correlation must be between -1.0 and 1.0.");

        _correlation = correlation;
        _sqrtTerm = ChiMath.Sqrt(T.One - correlation * correlation);
        _normalGenerator = new ChiStatefulNormalGenerator<TRng, T>(ref rng);
    }

    /// <summary>
    ///     Samples a single pair of correlated standard normal variables.
    /// </summary>
    /// <returns>A tuple `(z1, z2)` containing the pair of correlated standard normal variables.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (T z1, T z2) Sample()
    {
        var independentZ1 = _normalGenerator.Next();
        var independentZ2 = _normalGenerator.Next();
        var correlatedZ2 = _correlation * independentZ1 + _sqrtTerm * independentZ2;

        return (independentZ1, correlatedZ2);
    }

    /// <summary>
    ///     Generates a sequence of correlated standard normal variable pairs.
    /// </summary>
    /// <param name="count">The number of pairs to sample.</param>
    /// <returns>An enumerable collection of tuples, each containing a pair of correlated variables.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ChiEnumerable<(T z1, T z2)> Sample(int count)
    {
        var enumerable = ChiEnumerable<(T z1, T z2)>.Rent(count);
        var list = enumerable.List;
        for (var i = 0; i < list.Count; i++)
            list[i] = Sample();
        return enumerable;
    }
}

/// <summary>
///     Provides extension methods for sampling pairs of correlated standard normal variables.
/// </summary>
public static class ChiSamplerCorrelatedNormalsExtensions
{
    /// <summary>
    ///     Returns a sampler that generates pairs of standard normal variables with a specified correlation.
    /// </summary>
    /// <typeparam name="TRng">The type of the random number generator.</typeparam>
    /// <typeparam name="T">The floating-point type of the generated values.</typeparam>
    /// <param name="rng">The random number generator to use for sampling.</param>
    /// <param name="correlation">The correlation coefficient (ρ), which must be in the interval [-1, 1].</param>
    /// <returns>A sampler that can be used to generate pairs of correlated standard normal variables.</returns>
    /// <remarks>
    ///     <para>
    ///         <b>Use Cases:</b> A convenience sampler for modeling two related factors, such as a character's
    ///         height and weight, or the returns of two related stocks.
    ///         It simplifies a common 2D `MultivariateNormal` case into a single, intuitive call.
    ///     </para>
    ///     <para>
    ///         <b>Performance:</b> O(1) per sample.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code><![CDATA[
    /// var rng = new ChiRng();
    /// // Generate height and weight, which are positively correlated.
    /// var (zHeight, zWeight) = rng.CorrelatedNormals(0.6).Sample();
    /// var height = 175.0 + zHeight * 10.0;  // Mean 175cm, StdDev 10cm
    /// var weight = 70.0 + zWeight * 15.0;   // Mean 70kg, StdDev 15kg
    /// ]]></code>
    /// </example>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ChiSamplerCorrelatedNormals<TRng, T> CorrelatedNormals<TRng, T>(
        this ref TRng rng, T correlation)
        where TRng : struct, IChiRngSource<TRng>
        where T : IFloatingPoint<T>
    {
        return new ChiSamplerCorrelatedNormals<TRng, T>(ref rng, correlation);
    }
}
// --- example code end ---