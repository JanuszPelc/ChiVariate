using System.Numerics;
using AwesomeAssertions;

namespace ChiVariate.Tests.TestInfrastructure;

#pragma warning disable CS1591

public static class DirichletTestExtensions
{
    public static void AssertIsDirichlet<T>(
        this List<T[]> samples,
        T[] alpha,
        double relativeTolerance) where T : IFloatingPoint<T>
    {
        samples.Should().NotBeEmpty();

        var dimension = alpha.Length;
        var sampleCount = samples.Count;
        var alphaSum = alpha.Aggregate(T.Zero, (current, a) => current + a);

        var expectedMean = new T[dimension];
        for (var i = 0; i < dimension; i++)
            expectedMean[i] = alpha[i] / alphaSum;

        var sampleMean = new T[dimension];
        foreach (var sample in samples)
            for (var i = 0; i < dimension; i++)
                sampleMean[i] += sample[i];

        for (var i = 0; i < dimension; i++)
            sampleMean[i] /= T.CreateChecked(sampleCount);

        for (var i = 0; i < dimension; i++)
        {
            var expected = double.CreateChecked(expectedMean[i]);
            var actual = double.CreateChecked(sampleMean[i]);
            var tolerance = Math.Max(0.01, Math.Abs(expected * relativeTolerance));
            actual.Should().BeApproximately(expected, tolerance,
                $"because the sample mean for component {i} should match the expected value E[x_i] = alpha_i / alpha_0.");
        }
    }
}