using System.Numerics;
using AwesomeAssertions;

namespace ChiVariate.Tests.TestInfrastructure;

#pragma warning disable CS1591

public static class WishartTestExtensions
{
    public static void AssertIsWishart<T>(
        this List<T[,]> samples,
        int degreesOfFreedom,
        T[,] scaleMatrix,
        double relativeTolerance) where T : IFloatingPoint<T>
    {
        samples.Should().NotBeEmpty();

        var dimension = scaleMatrix.GetLength(0);
        var sampleCount = samples.Count;

        var expectedMean = new T[dimension, dimension];
        var v = T.CreateChecked(degreesOfFreedom);
        for (var i = 0; i < dimension; i++)
        for (var j = 0; j < dimension; j++)
            expectedMean[i, j] = v * scaleMatrix[i, j];

        var sampleMean = new T[dimension, dimension];
        foreach (var sample in samples)
            for (var i = 0; i < dimension; i++)
            for (var j = 0; j < dimension; j++)
                sampleMean[i, j] += sample[i, j];

        var n = T.CreateChecked(sampleCount);
        for (var i = 0; i < dimension; i++)
        for (var j = 0; j < dimension; j++)
            sampleMean[i, j] /= n;

        for (var i = 0; i < dimension; i++)
        for (var j = 0; j < dimension; j++)
        {
            var expected = double.CreateChecked(expectedMean[i, j]);
            var actual = double.CreateChecked(sampleMean[i, j]);
            var tolerance = Math.Max(0.1, Math.Abs(expected * relativeTolerance));
            actual.Should().BeApproximately(expected, tolerance,
                $"because the sample mean for element [{i},{j}] should match the expected value.");
        }
    }
}