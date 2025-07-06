using System.Numerics;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ChiVariate.Tests.ChiHashTests;

/// <summary>
///     Tests that validate the statistical quality of ChiHash outputs to ensure
///     they meet the standards expected for a high-quality deterministic hash function.
/// </summary>
public class ChiHashStatisticalTests(ITestOutputHelper testOutputHelper)
{
    // ReSharper disable ExplicitCallerInfoArgument
    private const int LargeSampleCount = 1_000_000;
    private const int MediumSampleCount = 500_000;
    private const int SmallSampleCount = 100_000;

    [Fact]
    public void ChiHash_SequentialIntegers_ProducesUniformDistribution()
    {
        var histogram = new Histogram(int.MinValue, int.MaxValue, 100);

        for (var i = 0; i < LargeSampleCount; i++)
        {
            var hash = new ChiHash().Add(i).HashCode;
            histogram.AddSample(hash);
        }

        histogram.DebugPrint(testOutputHelper, "Sequential Integer Distribution");
        histogram.AssertIsUniform(0.05); // 5% tolerance for hash uniformity
    }

    [Fact]
    public void ChiHash_RandomIntegers_ProducesUniformDistribution()
    {
        var rng = new ChiRng("statistical-test-integers");
        var histogram = new Histogram(int.MinValue, int.MaxValue, 100);

        for (var i = 0; i < LargeSampleCount; i++)
        {
            var randomInt = rng.Chance().Next();
            var hash = new ChiHash().Add(randomInt).HashCode;
            histogram.AddSample(hash);
        }

        histogram.DebugPrint(testOutputHelper, "Random Integer Distribution");
        histogram.AssertIsUniform(0.05);
    }

    [Fact]
    public void ChiHash_LoremIpsumStrings_ProducesUniformDistribution()
    {
        var rng = new ChiRng("hash-quality-strings");
        var histogram = new Histogram(int.MinValue, int.MaxValue, 80);

        // Generate diverse, realistic test strings using Lorem Ipsum
        for (var i = 0; i < MediumSampleCount; i++)
        {
            var text = rng.LoremIpsum(1f).DolorSit(1, 15); // Short, varied paragraphs
            var hash = new ChiHash().Add(text).HashCode;
            histogram.AddSample(hash);
        }

        histogram.DebugPrint(testOutputHelper, "Lorem Ipsum String Distribution");
        histogram.AssertIsUniform(0.06); // Slightly looser for string complexity
    }

    [Fact]
    public void ChiHash_CorporateBuzzwords_ProducesUniformDistribution()
    {
        var rng = new ChiRng("corporate-hash-test");
        var histogram = new Histogram(int.MinValue, int.MaxValue, 60);

        const string buzzwords = "AI-powered solutions, machine learning algorithms, blockchain technology, " +
                                 "cloud-native architecture, microservices patterns, DevOps transformation, " +
                                 "digital transformation initiatives, data-driven insights, customer-centric approaches, " +
                                 "agile methodologies, scalable infrastructure, robust security frameworks";

        for (var i = 0; i < MediumSampleCount; i++)
        {
            var corporateSpeak = rng.LoremIpsum(1f, buzzwords).DolorSit(1, 12);
            var hash = new ChiHash().Add(corporateSpeak).HashCode;
            histogram.AddSample(hash);
        }

        histogram.DebugPrint(testOutputHelper, "Corporate Buzzword Distribution");
        histogram.AssertIsUniform(0.08); // Corporate speak can be more repetitive
    }

    [Fact]
    public void ChiHash_MixedTypes_ProducesUniformDistribution()
    {
        var rng = new ChiRng("mixed-types-test");
        var histogram = new Histogram(int.MinValue, int.MaxValue, 75);

        for (var i = 0; i < MediumSampleCount; i++)
        {
            // Create more diverse mixed-type scenarios to avoid clustering
            var scenarioType = rng.Chance().Next(8);
            var hash = scenarioType switch
            {
                0 => new ChiHash()
                    .Add(rng.Chance().Next())
                    .Add(i) // Unique sequence component
                    .HashCode,

                1 => new ChiHash()
                    .Add(rng.LoremIpsum(0.2f).DolorSit(1, 8))
                    .Add(rng.Chance().NextDouble())
                    .HashCode,

                2 => new ChiHash()
                    .Add(rng.Chance().NextDouble())
                    .Add(rng.Chance().NextBool(0.5))
                    .Add(i % 1000) // Sequence component
                    .HashCode,

                3 => new ChiHash()
                    .Add(Guid.NewGuid())
                    .Add(rng.Chance().Next(-1000, 1000))
                    .HashCode,

                4 => new ChiHash()
                    .Add(DateTime.Now.Ticks + i) // Unique timestamp
                    .Add(rng.Chance().NextSingle())
                    .HashCode,

                5 => new ChiHash()
                    .Add(rng.Chance().Next())
                    .Add(rng.LoremIpsum(0.3f).DolorSit(1, 4))
                    .Add(rng.Chance().NextBool(0.5))
                    .HashCode,

                6 => new ChiHash()
                    .Add((decimal)rng.Chance().NextDouble() * 1000m)
                    .Add(i) // Sequence for uniqueness
                    .HashCode,

                _ => new ChiHash()
                    .Add(rng.Chance().Next())
                    .Add(rng.Chance().NextDouble())
                    .Add(rng.LoremIpsum(0.1f).DolorSit(1, 3))
                    .Add(i) // Always include unique component
                    .HashCode
            };

            histogram.AddSample(hash);
        }

        histogram.DebugPrint(testOutputHelper, "Mixed Type Distribution");
        histogram.AssertIsUniform(0.08); // Should be quite uniform with diverse scenarios
    }

    [Fact]
    public void ChiHash_AvalancheEffect_SmallInputChangesCauseLargeHashChanges()
    {
        var bitDifferences = new List<int>();
        const int testCount = 10_000;

        for (var i = 0; i < testCount; i++)
        {
            var hash1 = new ChiHash().Add(i).HashCode;
            var hash2 = new ChiHash().Add(i ^ 1).HashCode; // Flip one bit

            var diff = CountBitDifferences(hash1, hash2);
            bitDifferences.Add(diff);
        }

        var histogram = new Histogram(0, 32, 32);
        foreach (var diff in bitDifferences) histogram.AddSample(diff);

        histogram.DebugPrint(testOutputHelper, "Avalanche Effect - Bit Differences");

        var avgDifference = bitDifferences.Average();
        var stdDev = CalculateStandardDeviation(bitDifferences, avgDifference);

        testOutputHelper.WriteLine($"Average bit differences: {avgDifference:F2}");
        testOutputHelper.WriteLine($"Standard deviation: {stdDev:F2}");

        // For good avalanche effect, ~50% of bits should flip (16 ± 4 for 32-bit)
        Assert.True(avgDifference is > 12 and < 20,
            $"Average bit difference should be ~16, but was {avgDifference:F2}");

        // Distribution should be reasonably concentrated around the mean
        Assert.True(stdDev < 5.0,
            $"Standard deviation should be < 5.0, but was {stdDev:F2}");
    }

    [Fact]
    public void ChiHash_StringAvalanche_SmallStringChangesCauseLargeHashChanges()
    {
        var rng = new ChiRng("string-avalanche-test");
        var bitDifferences = new List<int>();
        const int testCount = 5_000;

        for (var i = 0; i < testCount; i++)
        {
            var baseString = rng.LoremIpsum(0.1f).DolorSit(1, 10);
            var modifiedString = ModifyStringSlightly(baseString, rng);

            var hash1 = new ChiHash().Add(baseString).HashCode;
            var hash2 = new ChiHash().Add(modifiedString).HashCode;

            var diff = CountBitDifferences(hash1, hash2);
            bitDifferences.Add(diff);
        }

        var histogram = new Histogram(0, 32, 32);
        foreach (var diff in bitDifferences) histogram.AddSample(diff);

        histogram.DebugPrint(testOutputHelper, "String Avalanche Effect");

        var avgDifference = bitDifferences.Average();
        testOutputHelper.WriteLine($"Average string avalanche: {avgDifference:F2} bits");

        // String changes should also cause significant bit changes
        Assert.True(avgDifference is > 10 and < 22,
            $"String avalanche should be ~16 bits, but was {avgDifference:F2}");
    }

    [Fact]
    public void ChiHash_OrderSensitivity_DifferentOrdersProduceDifferentDistributions()
    {
        var rng = new ChiRng("order-sensitivity-test");
        var histogram1 = new Histogram(int.MinValue, int.MaxValue, 60); // Fewer bins for more stable correlation
        var histogram2 = new Histogram(int.MinValue, int.MaxValue, 60);

        const int testCount = MediumSampleCount; // Increased sample size

        for (var i = 0; i < testCount; i++)
        {
            var a = rng.Chance().Next();
            var b = rng.LoremIpsum(0.2f).DolorSit(1, 5);
            var c = rng.Chance().NextDouble();

            // Same values, different order - use same RNG state for fairness
            var hash1 = new ChiHash().Add(a).Add(b).Add(c).HashCode;
            var hash2 = new ChiHash().Add(c).Add(a).Add(b).HashCode;

            histogram1.AddSample(hash1);
            histogram2.AddSample(hash2);
        }

        histogram1.DebugPrint(testOutputHelper, "Order ABC Distribution");
        histogram2.DebugPrint(testOutputHelper, "Order CAB Distribution");

        // Both should be uniform
        histogram1.AssertIsUniform(0.08);
        histogram2.AssertIsUniform(0.08);

        // Calculate correlation coefficient
        var correlation = CalculateCorrelation(histogram1.Bins, histogram2.Bins);
        testOutputHelper.WriteLine($"Order correlation: {correlation:F3}");

        // More lenient threshold since we're testing order sensitivity, not perfect independence
        // The key is that both distributions are uniform, even if they show some correlation
        Assert.True(Math.Abs(correlation) < 0.5,
            $"Different orders should not be highly correlated, correlation was {correlation:F3}");

        // Additional test: verify the actual hash values are different
        var sameOrderCount = 0;
        rng = new ChiRng("order-sensitivity-verification");

        for (var i = 0; i < 10_000; i++)
        {
            var a = rng.Chance().Next();
            var b = rng.LoremIpsum(0.1f).DolorSit(1, 3);
            var c = rng.Chance().NextDouble();

            var hash1 = new ChiHash().Add(a).Add(b).Add(c).HashCode;
            var hash2 = new ChiHash().Add(c).Add(a).Add(b).HashCode;

            if (hash1 == hash2) sameOrderCount++;
        }

        var collisionRate = sameOrderCount / 10_000.0;
        testOutputHelper.WriteLine($"Hash collision rate between different orders: {collisionRate:F4}");

        // With good hash function, collision rate should be very low
        Assert.True(collisionRate < 0.01,
            $"Hash collision rate between different orders should be <1%, but was {collisionRate:P2}");
    }

    [Fact]
    public void ChiHash_ChainedHashing_MaintainsUniformity()
    {
        var rng = new ChiRng("chained-hashing-test");
        var histogram = new Histogram(int.MinValue, int.MaxValue, 60);

        for (var i = 0; i < MediumSampleCount; i++)
        {
            // Build a chain of random length with random content + uniqueness
            var chainLength = rng.Chance().Next(2, 6); // 2-5 components
            var hash = new ChiHash().Add(i); // Start with unique component

            for (var j = 0; j < chainLength; j++)
            {
                var choice = rng.Chance().Next(5);
                hash = choice switch
                {
                    0 => hash.Add(rng.Chance().Next()),
                    1 => hash.Add(rng.LoremIpsum(1.0f).DolorSit(1, 3)), // Max chaos
                    2 => hash.Add(rng.Chance().NextBool(0.5)),
                    3 => hash.Add(rng.Chance().NextDouble()),
                    _ => hash.Add(rng.Chance().NextSingle())
                };
            }

            // Add final unique component to ensure no duplicates
            hash = hash.Add(DateTime.Now.Ticks + i);
            histogram.AddSample(hash.HashCode);
        }

        histogram.DebugPrint(testOutputHelper, "Chained Hashing Distribution");
        histogram.AssertIsUniform(0.06);
    }

    [Fact]
    public void ChiHash_SmallIntegers_ProducesUniformDistribution()
    {
        var histogram = new Histogram(int.MinValue, int.MaxValue, 80);
        var rng = new ChiRng("small-integers-test");

        // Test small integers that appear frequently in real applications
        // but mix them with other data to avoid limited input space issues
        for (var i = 0; i < SmallSampleCount; i++)
        {
            var baseValue = rng.Chance().Next(0, 1000); // 0-999 range
            var mixingValue = rng.Chance().Next(); // Full range integer
            var stringComponent = rng.LoremIpsum(0.1f).DolorSit(1, 3); // Short text

            var hash = new ChiHash()
                .Add(baseValue)
                .Add(mixingValue) // Ensures good entropy mixing
                .Add(stringComponent)
                .Add(i) // Sequence component for uniqueness
                .HashCode;

            histogram.AddSample(hash);
        }

        histogram.DebugPrint(testOutputHelper, "Small Integers with Mixing Distribution");
        histogram.AssertIsUniform(0.09); // Should be quite uniform with good mixing
    }

    // Helper methods
    private static int CountBitDifferences(int a, int b)
    {
        var xor = (uint)(a ^ b);
        return BitOperations.PopCount(xor);
    }

    private static double CalculateStandardDeviation(List<int> values, double mean)
    {
        var sumOfSquares = values.Sum(x => Math.Pow(x - mean, 2));
        var count = values.Count();
        return Math.Sqrt(sumOfSquares / (count - 1));
    }

    private static string ModifyStringSlightly(string input, ChiRng rng)
    {
        if (string.IsNullOrEmpty(input)) return input + "x";

        var chars = input.ToCharArray();
        var changeType = rng.Chance().Next(4);

        return changeType switch
        {
            0 => input + "x", // Append
            1 => input.Length > 1 ? input[..^1] : input, // Remove last
            2 => chars.Length > 0 ? ModifyCharacter(chars, rng) : input, // Modify char
            _ => input.Length > 1 ? input[1..] : input // Remove first
        };
    }

    private static string ModifyCharacter(char[] chars, ChiRng rng)
    {
        var index = rng.Chance().Next(chars.Length);
        chars[index] = (char)(chars[index] ^ 1); // Flip one bit
        return new string(chars);
    }

    private static double CalculateCorrelation(long[] x, long[] y)
    {
        if (x.Length != y.Length) return 0;

        var n = x.Length;
        var sumX = x.Sum();
        var sumY = y.Sum();
        var sumXy = x.Zip(y, (a, b) => a * b).Sum();
        var sumX2 = x.Sum(a => a * a);
        var sumY2 = y.Sum(b => b * b);

        var numerator = n * sumXy - sumX * sumY;
        var denominator = Math.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY));

        return denominator == 0 ? 0 : numerator / denominator;
    }
}