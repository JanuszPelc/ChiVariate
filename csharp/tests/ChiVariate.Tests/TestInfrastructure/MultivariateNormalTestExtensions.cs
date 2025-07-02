using System.Numerics;
using FluentAssertions;

namespace ChiVariate.Tests.TestInfrastructure;

#pragma warning disable CS1591

public static class MultivariateNormalTestExtensions
{
    public static void AssertIsMultivariateNormal<T>(
        this List<T[]> samples,
        T[] expectedMean,
        T[,] expectedCovariance,
        double relativeTolerance) where T : IFloatingPoint<T>
    {
        samples.Should().NotBeEmpty();

        var dimension = expectedMean.Length;
        var sampleCount = samples.Count;

        var sampleMean = new T[dimension];
        foreach (var sample in samples)
            for (var i = 0; i < dimension; i++)
                sampleMean[i] += sample[i];

        for (var i = 0; i < dimension; i++) sampleMean[i] /= T.CreateChecked(sampleCount);

        for (var i = 0; i < dimension; i++)
        {
            var expected = double.CreateChecked(expectedMean[i]);
            var actual = double.CreateChecked(sampleMean[i]);
            var tolerance = Math.Max(0.01, Math.Abs(expected * relativeTolerance));
            actual.Should().BeApproximately(expected, tolerance,
                $"because the sample mean for dimension {i} should match the expected mean.");
        }

        var sampleCovariance = new T[dimension, dimension];
        foreach (var sample in samples)
            for (var i = 0; i < dimension; i++)
            for (var j = 0; j < dimension; j++)
                sampleCovariance[i, j] += (sample[i] - sampleMean[i]) * (sample[j] - sampleMean[j]);

        var nMinus1 = T.CreateChecked(sampleCount - 1);
        for (var i = 0; i < dimension; i++)
        for (var j = 0; j < dimension; j++)
            sampleCovariance[i, j] /= nMinus1;

        for (var i = 0; i < dimension; i++)
        for (var j = 0; j < dimension; j++)
        {
            var expected = double.CreateChecked(expectedCovariance[i, j]);
            var actual = double.CreateChecked(sampleCovariance[i, j]);
            var tolerance = Math.Max(0.05, Math.Abs(expected * relativeTolerance * 2.5));
            actual.Should().BeApproximately(expected, tolerance,
                $"because the sample covariance for element [{i},{j}] should match the expected covariance.");
        }
    }
}