using AwesomeAssertions;
using ChiVariate.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS1591

namespace ChiVariate.Tests.ChiSamplerUtlTests.Primes;

public class SampleTests(ITestOutputHelper testOutputHelper)
{
    private const int LargeSampleCount = 50_000;
    private const int SmallSampleCount = 10_000;

    [Fact]
    public void PrimeSampler_TwinPrimes_FollowExpectedDistribution()
    {
        var rng = new ChiRng("TwinPrimes");
        var sampler = rng.Primes(1000L, 100_000L, 1);

        var twinPrimeCount = 0;
        var totalSamples = SmallSampleCount;

        for (var i = 0; i < totalSamples; i++)
        {
            var prime = sampler.Sample();
            if (IsPrime(prime + 2))
            {
                twinPrimeCount++;
                if (twinPrimeCount <= 10)
                    testOutputHelper.WriteLine($"Twin prime found: ({prime}, {prime + 2})");
            }
        }

        var twinPrimeRatio = (double)twinPrimeCount / totalSamples;
        testOutputHelper.WriteLine($"Twin prime ratio: {twinPrimeRatio:P2} ({twinPrimeCount}/{totalSamples})");

        twinPrimeRatio.Should().BeGreaterThan(0.10).And.BeLessThan(0.25);
    }

    [Fact]
    public void PrimeSampler_SophieGermainPrimes_AreDetected()
    {
        var rng = new ChiRng("SophieGermain");
        var sampler = rng.Primes(100L, 10_000L, 1);

        var sophieGermainPrimes = new List<long>();

        for (var i = 0; i < SmallSampleCount && sophieGermainPrimes.Count < 20; i++)
        {
            var prime = sampler.Sample();
            var safePrime = 2 * prime + 1;

            if (safePrime < long.MaxValue && IsPrime(safePrime))
            {
                sophieGermainPrimes.Add(prime);
                testOutputHelper.WriteLine($"Sophie Germain prime: {prime} (safe prime: {safePrime})");
            }
        }

        testOutputHelper.WriteLine($"Found {sophieGermainPrimes.Count} Sophie Germain primes");
        sophieGermainPrimes.Count.Should().BeGreaterThan(5);
    }

    [Fact]
    public void PrimeSampler_DigitalRoot_ShowsCorrectDistribution()
    {
        var rng = new ChiRng("DigitalRoot");
        var sampler = rng.Primes(100L, 1_000_000L, 1);

        var digitalRootCounts = new int[10];

        for (var i = 0; i < LargeSampleCount; i++)
        {
            var prime = sampler.Sample();
            var digitalRoot = CalculateDigitalRoot(prime);
            digitalRootCounts[digitalRoot]++;
        }

        testOutputHelper.WriteLine("Digital Root Distribution:");
        for (var i = 1; i <= 9; i++)
        {
            var percentage = (double)digitalRootCounts[i] / LargeSampleCount * 100;
            testOutputHelper.WriteLine($"  Root {i}: {digitalRootCounts[i]} samples ({percentage:F2}%)");
        }

        // Only digital roots 3, 6, 9 are impossible for primes (divisible by 3)
        var impossibleRoots = new[] { 3, 6, 9 };
        foreach (var root in impossibleRoots)
            digitalRootCounts[root].Should().Be(0, $"digital root {root} should be impossible for primes");

        // All other digital roots should occur for primes
        var possibleRoots = new[] { 1, 2, 4, 5, 7, 8 };
        foreach (var root in possibleRoots)
        {
            var percentage = (double)digitalRootCounts[root] / LargeSampleCount;
            percentage.Should().BeGreaterThan(0.05, $"digital root {root} should occur for primes");
        }
    }

    [Fact]
    public void PrimeSampler_LastDigit_RespectsModularConstraints()
    {
        var rng = new ChiRng("LastDigit");
        var sampler = rng.Primes(100L, 1_000_000L, 1);

        var lastDigitCounts = new int[10];

        for (var i = 0; i < LargeSampleCount; i++)
        {
            var prime = sampler.Sample();
            var lastDigit = (int)(prime % 10);
            lastDigitCounts[lastDigit]++;
        }

        testOutputHelper.WriteLine("Last Digit Distribution:");
        for (var digit = 0; digit <= 9; digit++)
        {
            var percentage = (double)lastDigitCounts[digit] / LargeSampleCount * 100;
            testOutputHelper.WriteLine($"  Digit {digit}: {lastDigitCounts[digit]} samples ({percentage:F2}%)");
        }

        // Even digits and 5 should not appear as last digits for primes > 5
        var impossibleDigits = new[] { 0, 2, 4, 5, 6, 8 };
        foreach (var digit in impossibleDigits)
            lastDigitCounts[digit].Should().Be(0);

        // Only 1, 3, 7, 9 can be last digits for primes > 5
        var possibleDigits = new[] { 1, 3, 7, 9 };
        foreach (var digit in possibleDigits)
        {
            var percentage = (double)lastDigitCounts[digit] / LargeSampleCount;
            percentage.Should().BeGreaterThan(0.20).And.BeLessThan(0.30);
        }
    }

    [Fact]
    public void PrimeSampler_SampledPrimes_ShowReasonableDistribution()
    {
        var rng = new ChiRng("PrimeDistribution");
        var sampler = rng.Primes(1_000L, 100_000L, 1);

        var uniquePrimes = new SortedSet<long>();
        for (var i = 0; i < 5000; i++)
            uniquePrimes.Add(sampler.Sample());

        var primesList = uniquePrimes.ToList();
        testOutputHelper.WriteLine($"Unique primes collected: {primesList.Count}");

        if (primesList.Count > 100)
        {
            var averagePrime = primesList.Average();
            var rangeSpan = primesList.Max() - primesList.Min();

            testOutputHelper.WriteLine($"Average prime: {averagePrime:F0}");
            testOutputHelper.WriteLine($"Range span: {rangeSpan:F0}");
            testOutputHelper.WriteLine($"Coverage: {100.0 * rangeSpan / (100_000 - 1_000):F1}%");

            var coverage = rangeSpan / (100_000.0 - 1_000.0);
            coverage.Should().BeGreaterThan(0.5, "should cover a significant portion of the range");

            averagePrime.Should().BeInRange(20_000, 80_000, "average should be reasonably centered");
        }
    }

    [Fact]
    public void PrimeSampler_Goldbach_CanFindPairs()
    {
        var rng = new ChiRng("Goldbach");
        var sampler = rng.Primes(2L, 10_000L, 1);

        var primeSet = new HashSet<long>();
        for (var i = 0; i < 5000; i++)
            primeSet.Add(sampler.Sample());

        var goldbachSuccesses = 0;
        var testNumbers = new[] { 100, 200, 500, 1000, 1500, 2000, 3000, 4000, 5000 };

        foreach (var evenNumber in testNumbers)
        {
            var foundPair = false;

            foreach (var prime in primeSet.Where(p => p < evenNumber))
            {
                var complement = evenNumber - prime;
                if (primeSet.Contains(complement))
                {
                    testOutputHelper.WriteLine($"Goldbach pair for {evenNumber}: {prime} + {complement}");
                    foundPair = true;
                    break;
                }
            }

            if (foundPair) goldbachSuccesses++;
        }

        testOutputHelper.WriteLine(
            $"Goldbach conjecture verified for {goldbachSuccesses}/{testNumbers.Length} test numbers");
        goldbachSuccesses.Should().BeGreaterThan((int)(testNumbers.Length * 0.8));
    }

    [Fact]
    public void PrimeSampler_LargePrimes_MaintainPrimality()
    {
        var rng = new ChiRng("LargePrimes");
        var sampler = rng.Primes(uint.MaxValue - 1000, uint.MaxValue, 1);

        testOutputHelper.WriteLine("Testing large primes near uint.MaxValue:");

        for (var i = 0; i < 100; i++)
        {
            var largePrime = sampler.Sample();

            largePrime.Should().BeLessThan(uint.MaxValue);
            IsPrime(largePrime).Should().BeTrue();

            if (i < 10)
                testOutputHelper.WriteLine($"  Large prime: {largePrime}");
        }
    }

    [Theory]
    [InlineData(2, 100, 2)]
    [InlineData(97, 100, 97)]
    [InlineData(2, 3, 2)]
    public void PrimeSampler_SpecificRanges_ReturnsExpectedPrimes(long min, long max, long expectedPrime)
    {
        var rng = new ChiRng($"Specific_{min}_{max}");
        var sampler = rng.Primes(min, max, 0);

        var foundExpected = false;
        for (var i = 0; i < 1000; i++)
        {
            var prime = sampler.Sample();
            if (prime == expectedPrime)
            {
                foundExpected = true;
                break;
            }
        }

        foundExpected.Should().BeTrue();
    }

    [Fact]
    public void Sample_AcrossLargeRange_IsSpatiallyUniform()
    {
        var rng = new ChiRng(ChiSeed.Scramble("PrimesSpatialUniformity"));
        const uint min = 0;
        const uint max = uint.MaxValue;

        var sampler = rng.Primes(min, max, 1);

        const int segmentCount = 50;
        var histogram = new Histogram(min, max, segmentCount);

        for (var i = 0; i < LargeSampleCount; i++)
        {
            var prime = sampler.Sample();
            histogram.AddSample(prime);
        }

        histogram.DebugPrint(testOutputHelper, "Distribution of Primes " + "Across Segments");
        histogram.AssertIsUniform(0.15);
    }

    [Theory]
    [InlineData(1_000L, 5_000L)]
    [InlineData(1_000_000L, 1_010_000L)]
    [InlineData(0L, 100L)]
    public void Sample_Always_ReturnsAPrimeWithinTheSpecifiedRange(long min, long max)
    {
        var rng = new ChiRng(ChiSeed.Scramble("PrimesContractTest", min + max));
        var sampler = rng.Primes(min, max, 1);

        for (var i = 0; i < 1000; i++)
        {
            var primeCandidate = sampler.Sample();

            primeCandidate.Should().BeGreaterThanOrEqualTo(min).And.BeLessThan(max);
            IsPrime(primeCandidate).Should().BeTrue(
                $"because the number {primeCandidate} returned by the sampler must be prime.");
        }
    }

    [Fact]
    public void Sample_WithFixedSeed_IsDeterministic()
    {
        var rng = new ChiRng(1337);

        var result = rng.Primes(1000, 2000, 10).Sample();

        result.Should().Be(1597);
    }

    [Fact]
    public void Sample_ForUInt64_IsDeterministic()
    {
        var rng = new ChiRng(42);
        var min = uint.MaxValue - 100;
        var max = uint.MaxValue;

        var result = rng.Primes(min, max, 1).Sample();

        result.Should().Be(4294967279u);
    }

    [Fact]
    public void Sample_RangeWithNoPrimes_ThrowsInvalidOperationException()
    {
        var rng = new ChiRng();
        var act = () => rng.Primes(24, 28, 0).Sample();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("No prime numbers exist in the specified range.");
    }

    [Theory]
    [InlineData(100, 10)] // min > max
    [InlineData(-1, 100)] // min is negative
    [InlineData(100, 200, -1)] // minEstimatePopulation is negative
    public void Primes_WithInvalidParameters_ThrowsArgumentOutOfRangeException(int min, int max, int minPop = 256)
    {
        var rng = new ChiRng();
        var act = () => { rng.Primes(min, max, minPop); };

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Primes_WithInsufficientEstimatedPopulation_ThrowsArgumentException()
    {
        var rng = new ChiRng();
        var act = () => { rng.Primes(140, 180); };

        act.Should().Throw<ArgumentException>()
            .WithMessage("*The specified range is estimated to contain only ~7 primes*");
    }

    private static bool IsPrime(long n)
    {
        if (n <= 1) return false;
        if (n <= 3) return true;
        if (n % 2 == 0 || n % 3 == 0) return false;

        for (long i = 5; i * i <= n; i += 6)
            if (n % i == 0 || n % (i + 2) == 0)
                return false;
        return true;
    }

    private static int CalculateDigitalRoot(long number)
    {
        while (number >= 10)
        {
            long sum = 0;
            while (number > 0)
            {
                sum += number % 10;
                number /= 10;
            }

            number = sum;
        }

        return (int)number;
    }
}