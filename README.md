# ChiVariate

> **⚠️ Pre-release version. Installation may not work properly. Official release coming very soon.**

ChiVariate is a deterministic, data-oriented random engine seamlessly bridging expressive everyday randomness and statistically rigorous, domain-specific simulations.

## Who is ChiVariate for

Whether you're writing a game or a Monte Carlo simulator, ChiVariate gives you expressive tools focused on predictable performance and statistical integrity.

### Game developers, engine architects, and real-time systems engineers

If you're building games, real-time systems, or low-overhead libraries and care about:

- Data-oriented, non-allocating randomness for hot paths
- Expressive APIs like `FlipCoin()`, `RollDie(12)`, or `PickEnum<Element>()`
- Deterministic procedural generation (textures, meshes, levels)
- Replay, save/load, and undo/redo via full RNG state snapshot and restore

### Researchers, data scientists, and statistical computing professionals

If you're working on simulations, modeling, ML pipelines, or numerical methods and need:

- Statistical rigor and cross-platform reproducibility
- 96-bit decimal variate support for precision-critical financial modeling
- A wide range of probability distributions (from Beta to Zipf)
- Pluggable custom random generators with zero-cost, compile-time abstraction

## Getting started

ChiVariate is quick to install and easy to explore.

1. **Install the package**

   ```bash
   dotnet add package ChiVariate
   ```

2. **Create a random generator**

   ```csharp
   var rng = new ChiRng();
   ```

3. **Explore available samplers**

    Autocomplete in modern IDEs shows all available samplers on `rng`, each with inline documentation and examples.

## Overview of design principles

From general-purpose tasks to statistical modeling, ChiVariate follows clear design principles to deliver high-quality randomness. These principles center on three core areas: expressive everyday randomness, statistical integrity, and predictable performance.

### Expressive randomness with `Chance`

The method `rng.Chance()` returns a struct that exposes a familiar interface for general-purpose randomness. It's a data-oriented alternative to `System.Random`.

Familiar methods include:

- `Next()`, `Next(max)`, and `Next(min, max)` for 32-bit integers in a max-exclusive range
- `Next<T>()`, `Next<T>(max)`, and `Next<T>(min, max)` for any integer type up to 128-bit
- `NextDouble()` and `NextSingle()` for floating-point values between 0 and 1
- `Shuffle(span)` and `GetItems(...)` for working with collections

It also provides expressive methods for intention-revealing code:

- `PickEnum<T>()` for selecting a random enum value with zero allocations
- `PickBetween<T>(min, max)` to pick any integer type in an inclusive range
- `PickItem(items, weights)` for weighted selection from a list
- `RollDie(sides)` and `OneIn(chances)` for dice rolls and chance-based logic
- `FlipCoin()` and `NextBool(probability)` for coin flips and Bernoulli trials

**Think of a number between 1 and 10, and hold it in your mind:**

```csharp
var rng = new ChiRng();
var secretNumber = rng.Chance().PickBetween(1, 10); // Inclusive range
```

**Simulate an AoE attack with random damage and criticals:**

```csharp
var rng = new ChiRng();
var chance = rng.Chance();

foreach (var enemy in enemies)
{
    // Reusing a 'chance' struct in the hot path
    int damage = chance.RollDie(8) + chance.RollDie(8) + 5; // 2d8 + 5
    bool criticalHit = chance.OneIn(10);

    enemy.ApplyDamage(damage, criticalHit);
}
```

Whether you're building a collectible card game, modeling stochastic outcomes, or exploring probability-driven design, drawing from weighted possibilities is a foundational technique. ChiVariate makes this both statistically sound and expressive.

**Procedurally generate a magic card with statistically correct drop rates:**

The `[ChiEnumWeight]` attribute keeps probability logic close to the enum definition, eliminating the need for separate weight arrays.

```csharp
public enum Rarity
{
    [ChiEnumWeight(0.65)] Common,
    [ChiEnumWeight(0.25)] Rare,
    [ChiEnumWeight(0.10)] Epic
}

public MagicCard GenerateMagicCard(ref ChiRng rng)
{
    var rarity = rng.Chance().PickEnum<Rarity>(); // Non-allocating
    var (attack, defense) = RollStatsForRarity(ref rng, rarity);

    return new MagicCard(rarity, attack, defense);
}

private static (int, int) RollStatsForRarity(ref ChiRng rng, Rarity rarity)
{
    var chance = rng.Chance();
    return rarity switch
    {
        Rarity.Common => (chance.PickBetween(1, 3), chance.PickBetween(1, 4)),
        Rarity.Rare => (chance.PickBetween(2, 5), chance.PickBetween(3, 6)),
        Rarity.Epic => (chance.PickBetween(4, 8), chance.PickBetween(5, 9)),
        _ => throw new UnreachableException() // Will never happen
    };
}
```

For hot-path scenarios, create and reuse a `Categorical` sampler to avoid the O(k) cost of each `PickEnum` call. See `WeightedEnumTests.cs` for a complete performance-optimized example.

This pattern uses familiar idioms and is backed by a statistically robust engine. It also opens the door to using a broad selection of more advanced sampling methods with the same fluent interface.

### Statistical integrity and reproducibility

ChiVariate emphasizes correctness, reproducibility, and statistical reliability in every aspect of variate generation.

- **Statistical robustness**: the default `ChiRng` source is powered by CHI32, a stateless, counter-based algorithm that has passed industry-standard test suites like TestU01's BigCrush and PractRand over 256 TB of data.
- **Bit-for-bit reproducibility**: CHI32-backed results are stable across architectures and .NET runtimes, ensuring cross-platform consistency.
- **Integrity and precision**: all integer and floating-point distributions follow best practices:
  - Rejection sampling avoids modulo bias in bounded integer generation.
  - High-order bits are prioritized to eliminate low-bit artifacts common in simpler PRNGs.
  - Floating-point distributions use precision-aware mantissa bit-filling for `float`, `double`, `Half`, and high-quality `decimal` values.

These examples illustrate how ChiVariate, supported by helper types like ChiMatrix for linear algebra and ChiMath for generic math functions, supports deterministic, precision-critical simulations in domains like actuarial modeling and financial forecasting.

**Monte Carlo asset valuation with decimal precision:**

```csharp
decimal EstimateTerminalValue(
    ref ChiRng rng, int numPaths, decimal initialPrice, 
    decimal drift, decimal volatility, decimal timeToMaturity)
{
    // Model asset price evolution using geometric Brownian motion
    var variance = volatility * volatility;
    var logReturnMean = (drift - 0.5m * variance) * timeToMaturity;
    var logReturnStdDev = volatility * ChiMath.Sqrt(timeToMaturity);
    var shockSampler = rng.LogNormal(logReturnMean, logReturnStdDev);

    decimal sumOfFinalPrices = 0;
    for (var i = 0; i < numPaths; i++)
    {
        var shock = shockSampler.Sample();
        var finalPrice = initialPrice * shock;
        sumOfFinalPrices += finalPrice;
    }

    return sumOfFinalPrices / numPaths;
}
```

**Modeling sensor fusion with Bayesian sampling:**

```csharp
var rng = new ChiRng("noisy-sensor-model");
var sensorCovariance = ChiMatrix.With([1.0, 0.5], [0.5, 1.5]);
var expectedReading = ChiMatrix.With([0.0, 0.0]);
var covarianceSampler = rng.Wishart(10, sensorCovariance);

foreach (var sampledCovariance in covarianceSampler.Sample(10_000))
{
    var noisyMeasurement = rng
        .MultivariateNormal(expectedReading, sampledCovariance)
        .Sample();

    // Use the result in a larger model, such as a Kalman filter update
    UpdateFilterWithMeasurement(noisyMeasurement);
}
```

**Insurance risk modeling with Gamma distribution:**

```csharp
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
```

### Low-overhead and predictable performance

Many domains, including game development, simulations, and real-time systems, require predictable performance with minimal overhead. ChiVariate follows a pay-for-what-you-use model with emphasis on low-level efficiency.

- **Data-oriented structure**: the layout supports data-oriented patterns, making it suitable for systems where cache locality and memory layout are critical.
- **Zero heap allocations**: all core generators and distribution samplers avoid heap usage. Struct-based RNGs and samplers are passed by reference to eliminate GC pressure.
- **Zero-cost abstraction**: `static abstract` dispatch and `ref struct` constraints allow distribution samplers to be optimized away entirely by the JIT, combining clean APIs with raw performance.
- **Aggressive inlining**: performance-critical paths are marked for inlining to enable JIT flattening and eliminate function call overhead.

**Non-allocating nebula particle system using Cauchy distribution:**

```csharp
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
```

```csharp
// A calm, gentle nebula
UpdateParticleSystem(nebulaParticles, deltaTime, 0.1f);

// A chaotic magical explosion
UpdateParticleSystem(explosionParticles, deltaTime, 0.9f);
```

These performance-focused design principles extend beyond core distributions to specialized sampling tools that handle common real-world tasks.

## Supported probability distributions

The performance of each distribution is grouped into six tiers, based on its relative cost compared to the `Uniform (Continuous)` sampler for `double` precision (~30 ns per sample on Apple M1 Pro).

| Tier | Cost | Description |
|:----:|:----:|-------------|
| 1 | 1-2.5x | Simple sampling and highly efficient algorithms. |
| 2 | 2.5-6x | Common composite distributions. |
| 3 | 6-15.5x | More complex calculations or compositions. |
| 4 | 15.5-39x | Specialized algorithms with significant computational cost. |
| 5 | 39x+ | Inherently high-cost algorithms (e.g., many trials). |

The following probability distributions are supported out of the box, with statistically robust generators and predictable performance costs across `float`, `double`, and high-quality `decimal` variates.

### Discrete distributions

Distributions that produce integer-valued outcomes. Useful for modeling countable events, selections, or success/failure trials.

#### Bernoulli

> A single trial that results in 1 (success) or 0 (failure). The fundamental building block for modeling coin flips, critical hits, or any binary yes/no decision with a known probability.
>
>  *Complexity: `O(1)`. Tier 1.*

#### Binomial

> The number of successes in a fixed number of independent trials. Excels at counting scenarios like shots hitting a target, customers making purchases, or applications in quality control and A/B testing.
>
>  *Complexity: `O(n)`, where n is the number of trials. Tier 5.*

#### Categorical

> A single trial that selects an outcome from a set of discrete categories, each with a different weight. The go-to choice for weighted loot tables, entity spawning, or any selection from multiple options with different likelihoods.
>
>  *Complexity: `O(1)` per sample, with `O(k)` setup where k is the number of categories. Tier 1.*

#### Geometric

> The number of trials needed to achieve the first success. Naturally models "how many attempts until success" scenarios, such as attacks needed to land a critical hit or API calls required to get a successful response.
>
>  *Complexity: `O(1/p)`, where p is the success probability. Tier 2.*

#### Hypergeometric

> The number of successes in a sample drawn *without replacement* from a finite population. A foundational distribution that handles card games (e.g., drawing a 5-card hand), lottery systems, or quality control where items are not returned to the population.
>
>  *Complexity: `O(n)`, where n is the sample size. Tier 4.*

#### Multinomial

> The number of outcomes in each of several categories after a fixed number of independent trials. It generalizes the Binomial distribution to multiple categories and handles modeling dice rolls, sorting items into bins, or simulating survey results.
>
>  *Complexity: `O(k+n)`, where k is the number of categories and n is the number of trials. Tier 5.*

#### Negative Binomial

> The number of trials required to achieve a fixed number of successes. Designed for scenarios like customer acquisition (how many contacts to get 10 sales), reliability testing, or game mechanics with accumulating success.
>
>  *Complexity: `O(r/p)`, where r is the number of successes and p is the success probability. Tier 4.*

#### Poisson

> The number of events occurring in a fixed interval of time or space at a steady average rate. Excels at modeling counts of "bursty" or rare events, such as emails arriving per minute, customers visiting a store per hour, or random encounters in a game.
>
>  *Complexity: `O(λ)`, where λ (lambda) is the mean. Tier 3.*

#### Uniform (Discrete)

> An integer where every value in a given range has an equal chance of being selected. As the fundamental distribution for unbiased choice, it provides the foundation for dice rolls, random array indexing, or any scenario requiring fair selection from a set of options.
>
>  *Complexity: `O(1)`. Tier 1.*

#### Zipf

> A distribution where frequency is inversely proportional to rank, modeling "power-law" or "long-tail" phenomena where a few items are common and most are rare. Ideal for simulating word frequencies, city populations, or user engagement patterns.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

### Continuous distributions

Distributions that produce real-valued variates, including `float`, `double`, `Half`, and high-quality `decimal` types. Suitable for modeling measurements, durations, or naturally varying quantities.

#### Beta

> A random value between 0 and 1 used to represent an unknown probability or proportion. The natural choice for modeling uncertainty about percentages, such as estimating a player's skill from match history or modeling task completion rates.
>
>  *Complexity: Amortized `O(1)`. Tier 3.*

#### Cauchy

> A bell-shaped curve with very heavy tails, leading to frequent extreme outliers. Excels at creating dramatic visual effects in particle systems, simulating financial market volatility, or modeling physical resonance and systems prone to extreme events.
>
>  *Complexity: `O(1)`. Tier 1.*

#### Chi

> The magnitude (Euclidean distance from the origin) of `k` independent standard normal variables. Commonly applied to calculating signal strength from I/Q components, error distances in targeting systems, or the speed of a particle with random velocity components.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

#### Chi-Squared

> The sum of the squares of `k` independent standard normal variables. As a cornerstone of statistical hypothesis testing, it provides the foundation for goodness-of-fit tests, feature selection in machine learning, and modeling the variance of a sample.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

#### Exponential

> The time between events in a Poisson process. Its "memoryless" property makes it the go-to distribution for modeling waiting times, such as intervals between customer arrivals, time until component failure, or delays before enemy attacks.
>
>  *Complexity: `O(1)`. Tier 1.*

#### F

> The ratio of two scaled chi-squared variables. A cornerstone of hypothesis testing, particularly in Analysis of Variance (ANOVA), it specializes in comparing variances between two or more groups to determine if their means are significantly different.
>
>  *Complexity: Amortized `O(1)`. Tier 3.*

#### Gamma

> A flexible distribution for positive, skewed values that generalizes the Exponential and Chi-Squared distributions. Well-suited to modeling the total time to complete multiple tasks, the size of insurance claims, or rainfall amounts. Its shape can be tuned from exponential-like to nearly normal.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

#### Gumbel

> An extreme value distribution used to model the maximum (or minimum) of a set of samples. The standard choice in risk management and engineering for events like maximum floods or wind speeds, and foundational to the Gumbel-Max trick in Machine Learning.
>
>  *Complexity: `O(1)`. Tier 1.*

#### Inverse-Gamma

> The reciprocal of a Gamma-distributed variable. A cornerstone of Bayesian statistics, it serves as the standard conjugate prior for the unknown variance of a Normal distribution, making it indispensable for modeling uncertainty in variance parameters.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

#### Laplace

> A symmetric, "pointy" distribution (also known as the "double exponential") that is more robust to outliers than the Normal. Particularly effective in robust regression, signal processing, and any model where errors have occasional large deviations.
>
>  *Complexity: `O(1)`. Tier 1.*

#### Logistic

> A bell-shaped curve similar to the Normal distribution but with heavier tails. Forms the mathematical basis for logistic regression in machine learning and excels at modeling population growth or any process that follows an S-shaped (sigmoid) curve.
>
>  *Complexity: `O(1)`. Tier 1.*

#### Log-Normal

> A continuous distribution of a positive random variable whose logarithm is normally distributed. Designed for phenomena arising from multiplicative processes of many small factors, such as stock prices, income levels, internet traffic, or biological growth rates.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

#### Normal

> The classic symmetric, bell-shaped curve (also known as the Gaussian distribution). The workhorse distribution for modeling a vast range of phenomena from measurement errors to test scores, and for generating realistic noise in simulations.
>
>  *Complexity: Amortized `O(1)`. Tier 1.*

#### Pareto

> A skewed distribution that models the "80/20" rule, where a small number of events account for a large effect. The quintessential choice for "winner-take-all" phenomena like wealth distribution, city populations, or item popularity.
>
>  *Complexity: `O(1)`. Tier 1.*

#### Rayleigh

> The magnitude of a 2D vector whose components are independent, zero-mean normal variables. A special case of the Chi distribution (k=2), it naturally models wind speed, wave heights, or the effect of multi-path signal fading.
>
>  *Complexity: `O(1)`. Tier 1.*

#### Student's t

> A bell-shaped curve similar to the Normal but with heavier tails, making it more robust to outliers. The preferred tool for statistical inference, especially when sample sizes are small or the population's variance is unknown.
>
>  *Complexity: Amortized `O(1)`. Tier 2.*

#### Triangular

> A continuous distribution defined by a minimum, maximum, and most likely (mode) value. Designed for simple modeling when you only have expert estimates for the bounds and the most likely outcome, such as in project management or risk analysis.
>
>  *Complexity: `O(1)`. Tier 1.*

#### Uniform (Continuous)

> A real number where every value in a given range has an equal chance of being selected. As the most fundamental continuous distribution, it provides the building blocks for generating other random variates and modeling any scenario where all outcomes in a range are equally likely.
>
>  *Complexity: `O(1)`. Tier 1 (Baseline).*

#### Weibull

> A flexible distribution used to model time-to-failure or survival data. The backbone of reliability engineering, its shape parameter allows it to model systems where the failure rate is decreasing (infant mortality), constant (random failures), or increasing (wear-out) over time.
>
>  *Complexity: `O(1)`. Tier 1.*

### Multivariate distributions 

Distributions that produce structured outputs such as vectors or matrices of correlated variates. Used for modeling interdependent random variables and higher-dimensional phenomena.

#### Dirichlet

> A vector of probabilities that sum to 1, representing the multivariate generalization of the Beta distribution. The go-to distribution for generating sets of related proportions, such as in topic modeling for text analysis, population genetics, or resource allocation in strategy games.
>
>  *Complexity: `O(k)` per sample, where k is the number of categories. Tier 4.*

#### Multivariate Normal

> A vector of correlated random variables, where each variable is normally distributed. As the multivariate generalization of the Normal distribution, it provides the foundation for realistic physics simulations with coupled variables, financial portfolio modeling, or any scenario where multiple random factors influence each other.
>
>  *Complexity: `O(k²)` per sample, plus `O(k³)` setup, where k is the number of dimensions. Tier 3.*

#### Wishart

> A random, symmetric, positive-semidefinite matrix that represents the multivariate generalization of the Chi-Squared distribution. A cornerstone of multivariate Bayesian analysis, it specializes in generating random covariance matrices, allowing you to model uncertainty in the relationships between multiple variables.
>
>  *Complexity: `O(k³)` per sample, plus `O(k³)` setup, where k is the number of dimensions. Tier 4.*

## Beyond probability distributions

ChiVariate provides specialized tools for general-purpose randomness tasks and uniform sampling scenarios where traditional probability distributions aren't the right fit.

### Utility samplers

#### Chance

> A toolkit for general-purpose randomness, providing a zero-allocation, statistically robust replacement for `System.Random`. Features expressive methods for common tasks like dice rolls (`RollDie`), coin flips (`FlipCoin`), random selections (`PickItem`), and shuffling (`Shuffle`).
>
> *Complexity: Generally `O(1)`, except for collection methods like `Shuffle` (`O(n)`). Tier 1.*

#### Spatial

> A toolkit for uniform spatial sampling within or on the surface of geometric primitives. Delivers a simple, reliable API for tasks like picking random positions in areas (`InSquare`, `InCube`) or generating random direction vectors (`OnCircle`, `OnSphere`), handling all mathematical corrections internally to prevent common statistical biases.
>
> *Complexity: `O(1)`. Tier 2.*

ChiVariate includes robust support for uniform spatial sampling within or on the surface of common geometric primitives. The `rng.Spatial()` automatically applies the necessary mathematical corrections to avoid common pitfalls, such as central clustering in circles or polar bias on spheres.

```csharp
// Generate a uniformly distributed point inside the unit square
var pointInSquare = rng.Spatial().InSquare(0.5f).Sample();

// Sample a uniformly distributed direction vector (unit sphere)
var randomDirection = rng.Spatial().OnSphere(1.0f).Sample().AsVector3();
```

#### Prime

> Generates provably prime numbers from specified integer ranges up to 128-bit using deterministic Miller-Rabin testing and various optimization techniques. Ideal for number theory research, cryptographic experiments, and applications requiring mathematically guaranteed primes with uniform distribution.
>
> *Complexity: `O(r/π(r) × √n)` expected, where r is range size, π(r) is prime density, and √n is primality test cost. Performance varies significantly with range density. Tier 5.*

This specialized sampler uses various optimization techniques and deterministic Miller-Rabin testing to deliver mathematically rigorous primes suitable for cryptographic and security-adjacent research.

```csharp
// Generate a set of very large primes
var max = UInt128.MaxValue;
var primes = rng.Prime(max / 2, max).Sample(100).ToList();
```

### Quasi-random sequences

For applications requiring maximum uniformity rather than randomness, ChiVariate provides low-discrepancy sequences that generate points covering a sample space as evenly and uniformly as possible. These deterministic tools provide a powerful alternative to pseudo-random numbers when uniform coverage is the primary goal.

#### Halton

> A classic, fast-to-compute low-discrepancy sequence based on prime numbers. The go-to choice for lower-dimensional problems where simplicity and speed are key, providing excellent uniformity with minimal computational overhead.
> 
> *Complexity: O(d) setup cost and an amortized O(d) per sample, where d is the number of dimensions. Highly optimized for up to 10 dimensions. Tier 1.*

#### Sobol

> A more sophisticated sequence with superior distribution properties, especially in higher dimensions. The preferred choice for demanding numerical integration tasks and applications requiring the highest uniformity across many dimensions.
>
> *Complexity: O(d) setup cost and an O(d) per sample, where d is the number of dimensions. Highly optimized for up to 10 dimensions. Tier 1.*

Both canonical (pure deterministic) and randomized variants are supported, with randomization improving statistical properties while preserving uniform coverage. Common applications include quasi-Monte Carlo integration and uniform sampling in computer graphics.

```csharp
// Create a 4D Sobol sampler that produces vectors of doubles
var sobolSampler = rng.Sobol(4).OfType<double>();
var points = sobolSampler.Sample(100);
```

Beyond these built-in samplers, ChiVariate's architecture supports seamless integration of custom generators and distributions.

## Pluggable extension architecture

ChiVariate cleanly separates entropy generation from distribution logic using zero-cost, generic abstractions. This enables integration of both custom RNG sources and domain-specific samplers with no virtual calls, heap allocations, or runtime overhead.

### Custom entropy sources

Any `struct` that implements `IChiRngSource<T>` can serve as a randomness backend. This includes pseudo-random generators, cryptographically secure sources, or hardware-based entropy.

**Integrating a cryptographically strong random number generator:**

```csharp
public record struct CryptoRng : IChiRngSource<CryptoRng>
{
    public static uint NextUInt32(ref CryptoRng _)
    {
        Span<byte> buffer = stackalloc byte[4];
        System.Security.Cryptography.RandomNumberGenerator.Fill(buffer);
        return Unsafe.ReadUnaligned<uint>(ref buffer[0]);
    }

    public static ulong NextUInt64(ref CryptoRng _)
    {
        Span<byte> buffer = stackalloc byte[8];
        System.Security.Cryptography.RandomNumberGenerator.Fill(buffer);
        return Unsafe.ReadUnaligned<ulong>(ref buffer[0]);
    }
}
```

### Custom distribution samplers

Custom samplers are lightweight `ref struct` types that encapsulate reusable sampling logic. They integrate seamlessly with any RNG source using the same fluent, data-oriented patterns as built-in samplers, with no GC or abstraction overhead.

**Defining a Maxwell-Boltzmann sampler for particle speeds:**

```csharp
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
```

### Combining custom RNG and sampler

Built-in and custom RNGs and samplers compose naturally to support domain-specific simulation scenarios. These extension points let ChiVariate scale across domains, from cryptographic security to statistical modeling, while preserving performance and expressiveness.

**Using the custom `CryptoRng` with the custom Maxwell-Boltzmann sampler:**

```csharp
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
```

### Authoring advanced, custom samplers

ChiVariate supports defining custom distribution samplers, which is especially useful for modeling complex relationships or behaviors not covered by built-in distributions. These samplers can maintain internal state to optimize performance, cache intermediate values, or enable correlated sampling.

See `StatefulSamplerTests.cs` in the test suite for a complete implementation of a `CorrelatedNormals` sampler with full statistical validation.

## Trade-offs and gotchas

ChiVariate's fully composable, non-allocating design delivers predictable performance and flexibility, but achieving this required specific architectural choices. This section covers the key tradeoffs and potential gotchas to be aware of when using the library in production.

### Working with ref structs

ChiVariate's `struct`-based design eliminates allocations but requires understanding a key constraint: RNG instances must be stored as fields, not properties. This happens because samplers use `ref` parameters to avoid copying state.

Since C# properties can't be passed by reference, attempting to use an RNG stored as a property will fail to compile when calling sampler methods. For example, declaring an RNG as `public ChiRng Rng { get; set; }` property will cause compilation errors, while `public ChiRng Rng;` field works perfectly. The solution is straightforward: declare RNG instances as fields rather than properties.

This design choice prevents the classic "modifying a copy" bug that plagues mutable structs, ensuring RNG state updates correctly every time.

### Precision vs performance

ChiVariate provides full support for the `decimal` type across all major distributions, enabling simulations and models that demand exact base-10 precision. This is especially important in financial and actuarial contexts, where even small base-2 rounding errors in `double` can accumulate and undermine accuracy. `decimal` avoids such errors by offering exact decimal representation, making it suitable for calculations involving monetary values, interest rates, or regulatory compliance.

The performance cost is inherent: `decimal` is a 128-bit, software-emulated type without hardware acceleration for mathematical operations like `Log`, `Sqrt`, or `Exp`, which results in significantly slower execution compared to `double`.

ChiVariate embraces this trade-off, delivering consistent APIs across numeric types while preserving statistical integrity for precision-critical domains. Use `decimal` only when necessary for compliance or base-10 fidelity. For most general simulations, `double` remains faster and precise enough.

### Cross-platform determinism

ChiVariate guarantees bit-for-bit reproducibility across platforms and .NET runtimes, but understanding the boundaries of this guarantee is important for mission-critical applications.

**Fully deterministic across all platforms:**
- All integer distributions of any bit-size (8-bit through 128-bit)
- All `decimal` distributions - ChiVariate implements its own decimal math library with no dependencies on platform-specific math functions
- Hashing, seeding, and core RNG state generation

**Platform-dependent precision in last bits:**

These differences are rather academic, as any tiny precision variations are dwarfed by the inherent randomness being modeled. `float` and `double` distributions may have minute precision differences in the least significant bit on exotic platforms, as they rely on the underlying runtime's math library (`System.Math.Log`, `System.Math.Sqrt`, etc.).

**Recommendation:** For applications requiring absolute cross-platform determinism, use `decimal` or `integer` types for critical calculations or validate your specific platform combinations during testing. For most use cases, the potential `float`/`double` variance is insignificant compared to the inherent randomness being modeled.

### Working with ChiMatrix

ChiVariate includes `ChiMatrix<T>` for multivariate distributions with full support for any floating-point type, including `decimal`. For most users, the simplest approach is converting to familiar arrays:

```csharp
var covariances = wishart
    .Sample(SampleCount)
    .Select(matrix => matrix.ToArray())
    .ToList();
```

ChiMatrix automatically handles memory management by storing small matrices (up to 5×5) inline on the stack and using pooled heap memory for larger matrices. When working directly with linear algebra operations, use the `Peek()` method, which is the safe default choice:

```csharp
var vandermonde = ChiMatrix.Vandermonde([1.0, 2.0, 3.0]);
var halfScalar = ChiMatrix.Scalar(0.5);
var scaled = vandermonde.Peek() * halfScalar.Peek();
var transposed = scaled.Peek().Transpose();
```

For explicit cleanup of large matrices, standard `Dispose()` calls or try/finally patterns work as expected. Library authors implementing custom samplers may benefit from the more advanced `Consume()` method for heap memory optimization. See the XML documentation for detailed guidance on memory management patterns.

This gives users three clear options in order of familiarity: `ToArray()` (simplest), `Peek()` (safe default) for linear algebra operations, and `Consume()` and `Dispose()` (advanced optimization) for explicit memory control.

For scenarios requiring minimal allocations when working with matrices larger than 5×5, the same example using advanced memory optimization would look like this:

```csharp
var covariances = wishart
    .Sample(SampleCount)
    .Select(matrix => { using(matrix) return matrix.ToArray(); })
    .ToList();
```

The `using` statement disposes each intermediate matrix immediately, returning its memory to the pool and reducing GC pressure.

Note: `using` statements work exclusively with `T[,] ToArray()` and `T[] VectorToArray()`, which are instance methods, and do not work with `Peek()` or `Consume()`, which are `ref this` extension methods.

## Project status

ChiVariate is a passion project maintained in spare time. The primary focus is on stability and correctness above all else. This includes fixing bugs, improving documentation, and ensuring reliable behavior across platforms and use cases.

New features are considered carefully but take lower priority than maintaining the existing functionality at high quality.

## Contributing

See [Contributing](https://github.com/JanuszPelc/ChiVariate/blob/master/CONTRIBUTING.md) for details. Bug reports and performance optimizations are especially welcome and receive priority support.

## License

ChiVariate is distributed under the [MIT license](https://github.com/JanuszPelc/ChiVariate/blob/master/LICENSE).

## Related projects

[CHI32](https://github.com/JanuszPelc/chi32) is a stateless, deterministic random number generator that produces high-quality 32-bit values by applying a sequence of well-mixed hashing operations to a pair of 64-bit integers. It has passed industry-standard test suites like TestU01's BigCrush and PractRand over 256 TB of data.