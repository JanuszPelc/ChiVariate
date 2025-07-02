using System.Numerics;
using ChiVariate.Tests.TestInfrastructure;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ChiVariate.Tests.DocExamplesTests;

#region boilerplate

#if !EXAMPLE_RELATED_BOILERPLATE // Necessary for example code integrity

// To prevent code cleanup from removing fully qualified
// System.Security.Cryptography.RandomNumberGenerator
using RandomNumberGenerator = DummyRandomNumberGenerator;

file record DummyRandomNumberGenerator;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedLocalFunctionReturnValue
// ReSharper disable UnusedVariable
// ReSharper disable UnusedType.Local
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable NotAccessedPositionalProperty.Local
// ReSharper disable NotAccessedVariable
// ReSharper disable UnusedLocalFunctionReturnValue
#endif

#endregion

#pragma warning disable CS1591

public class MainReadmeTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    private void CompilationTest()
    {
        {
            // --- example code begin --- 
            var rng = new ChiRng();
            var secretNumber = rng.Chance().PickBetween(1, 10); // Inclusive range
            // --- example code end ---
        }

        {
            var rng = new ChiRng();

            // --- example code begin ---
            // Sample a uniformly distributed point on a sphere of radius 10
            var pointOnSphere = rng.Spatial().OnSphere(10.0f).Sample();

            // Convert to System.Numerics.Vector3 using built-in method
            var randomDirection = pointOnSphere.AsVector3();
            // --- example code end ---
        }

        {
            var rng = new ChiRng();

            // --- example code begin ---
            // Generate a uniformly distributed point inside the unit square
            var pointInUnitSquare = rng.Spatial().InSquare(1.0f).Sample();

            // Define the top-left position and size of a rectangular region
            var position = new Vector2(10.0f, 20.0f);
            var size = new Vector2(50.0f, 25.0f);

            // Map to System.Numerics.Vector2 and scale to position within the region
            var randomPosition = position + pointInUnitSquare.AsVector2() * size;
            // --- example code end ---
        }

        {
            var rng = new ChiRng();

            // --- example code begin ---
            // Spawn an enemy at a random point along the wall of a square room
            var roomSize = 50.0f;
            var spawnPoint = rng.Spatial().OnSquare(roomSize / 2f).Sample().AsVector2();
            // --- example code end ---
        }

        {
            var vandermonde = ChiMatrix.Vandermonde([1.0, 2.0, 3.0]);
            var halfScalar = ChiMatrix.Scalar(0.5);
            var scaled = vandermonde.Peek() * halfScalar.Peek();
            var transposed = scaled.Peek().Transpose();
        }
    }

    [Fact]
    public void ChiRng_DirichletSampling_OutputsNormalizedVector()
    {
        // --- example code begin ---
        // Generate a 3-outcome probability vector using the Dirichlet distribution
        var rng = new ChiRng();

        var probabilities = rng
            .Dirichlet([0.1, 1.0, 10.0])
            .Sample();
        // --- example code end ---

        probabilities.Length.Should().Be(3);
        probabilities.ToArray().Sum().Should().BeApproximately(1.0, 1e-12);
    }

    [Fact]
    public void GammaSampler_WithFixedSeed_GeneratesStableAggregate()
    {
        EstimateTotalLiability(2.5f, 10_000).Should()
            .BeGreaterThan(24_000_000).And
            .BeLessThan(28_000_000);
        EstimateTotalLiability(2.5, 10_000).Should()
            .BeGreaterThan(24_000_000).And
            .BeLessThan(28_000_000);
        EstimateTotalLiability(2.5m, 10_000).Should()
            .BeGreaterThan(24_000_000).And
            .BeLessThan(28_000_000);
    }

    // --- example code begin ---
    // Supports float, double, and decimal types via a generic T
    public static T EstimateTotalLiability<T>(T shape, int payoutCount)
        where T : IFloatingPoint<T>
    {
        // Using a fixed seed makes the simulation fully deterministic
        var rng = new ChiRng("actuarial-seed");
        var scale = T.CreateChecked(1000); // Long-tailed distribution

        return rng
            .Gamma(shape, scale)
            .Sample(payoutCount)
            .Aggregate(T.Zero, (total, payout) => total + payout);
    }
    // --- example code end ---

    [Fact]
    public void WishartSampling_InHierarchicalModel_ReflectsPriorCovariance()
    {
        const int sampleCount = 10_000;
        const int degreesOfFreedom = 10;
        var measurements = new List<double[]>();

        // --- example code begin ---
        var rng = new ChiRng("noisy-sensor-model");

        // Prior belief about the covariance of two correlated sensors
        var priorCov = ChiMatrix.With([1.0, 0.5], [0.5, 1.5]);

        // One-time setup: create a sampler for the Wishart distribution
        var wishart = rng.Wishart(10, priorCov); // 10 degrees of freedom

        // Expected "true" reading of the sensors (e.g., zero drift)
        var meanSensorReading = ChiMatrix.With([0.0, 0.0]);

        // Sample plausible covariance matrices from the Wishart prior
        foreach (var sampledCov in wishart.Sample(10_000))
        {
            // Simulate one noisy reading from the correlated sensors
            var noisyMeasurement = rng
                .MultivariateNormal(meanSensorReading, sampledCov)
                .Sample();

            // Use the result in a larger model, such as a Kalman filter update
            UpdateFilterWithMeasurement(noisyMeasurement);
        }
        // --- example code end ---

        // Assertions
        // We can assert on the statistical properties of the collected measurements.
        var sampleMean = new[] { 0.0, 0.0 };
        foreach (var m in measurements)
        {
            sampleMean[0] += m[0];
            sampleMean[1] += m[1];
        }

        sampleMean[0] /= sampleCount;
        sampleMean[1] /= sampleCount;

        // 1. The overall mean of the measurements should be close to zero.
        sampleMean[0].Should().BeApproximately(0.0, 0.15, "because the mean of the normal distribution was [0,0]");
        sampleMean[1].Should().BeApproximately(0.0, 0.15, "because the mean of the normal distribution was [0,0]");

        // 2. The key assertion: The overall covariance of the measurements should be
        //    proportional to the mean of the Wishart distribution, which is (nu * Sigma).
        var sampleCov = new[,] { { 0.0, 0.0 }, { 0.0, 0.0 } };
        foreach (var m in measurements)
        {
            sampleCov[0, 0] += (m[0] - sampleMean[0]) * (m[0] - sampleMean[0]);
            sampleCov[0, 1] += (m[0] - sampleMean[0]) * (m[1] - sampleMean[1]);
            sampleCov[1, 1] += (m[1] - sampleMean[1]) * (m[1] - sampleMean[1]);
        }

        sampleCov[1, 0] = sampleCov[0, 1]; // Covariance matrix is symmetric

        sampleCov[0, 0] /= sampleCount - 1;
        sampleCov[0, 1] /= sampleCount - 1;
        sampleCov[1, 0] /= sampleCount - 1;
        sampleCov[1, 1] /= sampleCount - 1;

        var expectedCovX = degreesOfFreedom * priorCov[0, 0];
        var expectedCovY = degreesOfFreedom * priorCov[1, 1];
        var expectedCovXy = degreesOfFreedom * priorCov[0, 1];

        sampleCov[0, 0].Should().BeApproximately(expectedCovX, expectedCovX * 0.15,
            "variance of X should reflect the Wishart mean");
        sampleCov[1, 1].Should().BeApproximately(expectedCovY, expectedCovY * 0.15,
            "variance of Y should reflect the Wishart mean");
        sampleCov[0, 1].Should().BeApproximately(expectedCovXy, expectedCovXy * 0.25,
            "covariance of XY should reflect the Wishart mean");

        return;

        void UpdateFilterWithMeasurement(ChiMatrix<double> measurement)
        {
            measurements.Add(measurement.VectorToArray());
        }
    }

    [Fact]
    public void FinancialSimulation_WithDecimal_ProducesLogNormalDistribution()
    {
        var prices = new List<decimal>();
        var testRng = new ChiRng(nameof(FinancialSimulation_WithDecimal_ProducesLogNormalDistribution));

        const decimal testVolatility = 0.80m;
        const decimal testDrift = 0.05m;
        const int testNumPaths = 50_000;

        SimulateAndSumTerminalPrices(ref testRng, testNumPaths, testDrift, testVolatility);

        prices.Should().HaveCount(testNumPaths);

        var logPrices = prices.Select(ChiMath.Log).ToList();

        var histogram = new Histogram(4.5, 5.6, 100);
        foreach (var logPrice in logPrices)
            histogram.AddSample((double)logPrice);

        const decimal dailyProbability = 1.0m / 252.0m;
        var expectedLogMean = (double)ChiMath.Log(150.0m) +
                              (double)((testDrift - 0.5m * testVolatility * testVolatility) * dailyProbability);
        var expectedLogStdDev = (double)(testVolatility * ChiMath.Sqrt(dailyProbability));

        histogram.DebugPrint(testOutputHelper);
        histogram.AssertIsNormal(expectedLogMean, expectedLogStdDev, 0.06);

        return;

        // --- example code begin ---
        decimal SimulateAndSumTerminalPrices(
            ref ChiRng rng, int numPaths, decimal drift, decimal volatility)
        {
            const decimal timeDelta = 1.0m / 252.0m;
            var mean = (drift - 0.5m * volatility * volatility) * timeDelta;
            var sqrtTime = ChiMath.Sqrt(timeDelta); // Generic decimal square root
            var stdDev = volatility * sqrtTime;
            var shock = rng.Normal(mean, stdDev);

            decimal sumOfFinalPrices = 0;
            for (var i = 0; i < numPaths; i++)
            {
                var randomShock = shock.Sample();
                var finalPrice = 150.0m * ChiMath.Exp(randomShock);

                sumOfFinalPrices += finalPrice;
                CollectFinalPrice(finalPrice); // ############## To be removed from example code! ##############
            }

            return sumOfFinalPrices;
        }
        // --- example code end ---

        decimal CollectFinalPrice(decimal price)
        {
            prices.Add(price);
            return price;
        }
    }

    [Fact]
    public void ParticleSystem_WithDifferentChaos_ProducesStatisticallyDifferentResults()
    {
        // Arrange
        const int particleCount = 20_000;
        const float deltaTime = 0.1f;

        // Create two identical sets of particles, all starting at the origin.
        var nebulaParticles = new ParticlesExample.Particle[particleCount];
        var explosionParticles = new ParticlesExample.Particle[particleCount];
        Array.Copy(nebulaParticles, explosionParticles, particleCount);

        var exampleSystem = new ParticlesExample();

        // Act
        // Run both simulations. This modifies the particles in-place.
        exampleSystem.UpdateAllSystems(nebulaParticles, explosionParticles, deltaTime);

        // Assert
        // We will measure the final displacement (distance from origin) for each particle.
        var nebulaDisplacements = new List<float>(particleCount);
        var explosionDisplacements = new List<float>(particleCount);

        for (var i = 0; i < particleCount; i++)
        {
            nebulaDisplacements.Add(nebulaParticles[i].Position.Length());
            explosionDisplacements.Add(explosionParticles[i].Position.Length());
        }

        // Create histograms for the displacements.
        var nebulaHistogram = new Histogram(0, 1.0, 50);
        var explosionHistogram = new Histogram(0, 10.0, 50); // Needs a much larger range!

        foreach (var d in nebulaDisplacements) nebulaHistogram.AddSample(d);
        foreach (var d in explosionDisplacements) explosionHistogram.AddSample(d);

        // Print the histograms for visual confirmation.
        nebulaHistogram.DebugPrint(testOutputHelper, $"Displacement for Calm Nebula (chaos={0.1})");
        explosionHistogram.DebugPrint(testOutputHelper, $"Displacement for Chaotic Explosion (chaos={0.9})");

        // Get the mean displacement for each system.
        var meanNebulaDisplacement = nebulaHistogram.CalculateMean();
        var meanExplosionDisplacement = explosionHistogram.CalculateMean();

        // The core assertion: The chaotic system must have a significantly larger
        // average displacement and standard deviation than the calm one.
        meanExplosionDisplacement.Should().BeGreaterThan(meanNebulaDisplacement * 5,
            "because a higher chaos parameter should result in particles being flung much further.");

        // Calculate the standard deviation *after* calculating the mean.
        var stdDevNebula = nebulaHistogram.CalculateStdDev(meanNebulaDisplacement);
        var stdDevExplosion = explosionHistogram.CalculateStdDev(meanExplosionDisplacement);

        stdDevExplosion.Should().BeGreaterThan(stdDevNebula * 5,
            "because the spread of outcomes should be much wider for the chaotic system.");
    }

    private class ParticlesExample
    {
        public void UpdateAllSystems(
            Span<Particle> nebulaParticles, Span<Particle> explosionParticles, float deltaTime)
        {
            // --- example code begin ---
            // A calm, gentle nebula
            UpdateParticleSystem(nebulaParticles, deltaTime, 0.1f);

            // A chaotic magical explosion
            UpdateParticleSystem(explosionParticles, deltaTime, 0.9f);
            // --- example code end ---
        }

#if !EXAMPLE_RELATED_BOILERPLATE // Necessary for example code integrity
        // --- example code begin ---
        // The data for a single particle.
        public struct Particle
        {
            public Vector3 Position;
            public Vector3 Velocity;
            public float Lifetime;
        }

        // Data-oriented particle update system with zero GC pressure
        public void UpdateParticleSystem(Span<Particle> particles, float deltaTime, float chaos)
        {
            var rng = new ChiRng(); // Lives on the stack

            // Using a heavy-tailed Cauchy distribution adds visual drama.
            // Most particles drift gently, but some get sharp velocity kicks,
            // creating beautiful streaks and bursts. The 'chaos' parameter
            // controls the likelihood of these high-energy events.
            var turbulence = rng.Cauchy(0.0f, chaos);

            for (var i = 0; i < particles.Length; i++)
            {
                ref var particle = ref particles[i];

                var randomForce = new Vector3(
                    turbulence.Sample(),
                    turbulence.Sample(),
                    turbulence.Sample()
                );

                particle.Velocity += randomForce * deltaTime;
                particle.Position += particle.Velocity * deltaTime;
                particle.Lifetime -= deltaTime;
            }
        }
        // --- example code end ---

        // ReSharper restore NotAccessedField.Local
        // ReSharper restore MemberCanBePrivate.Local
#endif
    }

    #region boilerplate

#if !EXAMPLE_RELATED_BOILERPLATE // Necessary for example code integrity
    // ReSharper disable once UnusedType.Local
    private static class Boilerplate
    {
        private static MainReadmeTests? _unused;

        // ReSharper disable once UnusedMember.Local
        private static MainReadmeTests? Unused()
        {
            _ = new RandomNumberGenerator();
            return _unused ??= null;
        }
    }
#endif

    #endregion
}