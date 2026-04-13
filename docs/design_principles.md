# Overview of design principles

From general-purpose tasks to statistical modeling, ChiVariate follows clear design principles to deliver high-quality randomness. These principles center on three core areas: expressive everyday randomness, statistical integrity, and predictable performance.

## Expressive randomness with `Chance`

The method `rng.Chance()` returns a struct that exposes a familiar interface for general-purpose randomness. It's a data-oriented alternative to `System.Random`.

Familiar methods include:

- `Next()`, `Next(max)`, and `Next(min, max)` for 32-bit integers in a max-exclusive range
- `Next<T>()`, `Next<T>(max)`, and `Next<T>(min, max)` for any integer type up to 128-bit
- `NextDouble()` and `NextSingle()` for floating-point values between 0 and 1
- `Shuffle(span)` and `GetItems(...)` for working with collections

It also provides expressive methods for intention-revealing code:

- `PickEnum<T>()` for selecting a random enum value with zero allocations
- `PickBetween<T>(min, max)` to pick any integer type in an inclusive range
- `PickItem(items, weights)` for weighted selection from a collection
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
    int damage = chance.RollDie(8) + chance.RollDie(8) + 5; // 2d8 + 5
    bool criticalHit = chance.OneIn(20); //  5% chance

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

## Statistical integrity and reproducibility

ChiVariate emphasizes correctness, reproducibility, and statistical reliability in every aspect of variate generation.

- **Statistical robustness**: the default `ChiRng` source is powered by CHI32, a stateless, counter-based algorithm that has passed industry-standard test suites like TestU01's BigCrush and PractRand over 256 TB of data.
- **Bit-for-bit reproducibility**: CHI32-backed results are stable across architectures and .NET runtimes, ensuring cross-platform consistency.
- **Integrity and precision**: all integer and floating-point distributions follow best practices:
  - Rejection sampling avoids modulo bias in bounded integer generation.
  - High-order bits are prioritized to eliminate low-bit artifacts common in simpler PRNGs.
  - Floating-point distributions use precision-aware mantissa bit-filling for `float`, `double`, `Half`, `decimal`, and `ChiFixed` values.

These examples illustrate how ChiVariate, supported by helper types like ChiMatrix for linear algebra and ChiMath for generic math functions, supports deterministic, precision-critical simulations in domains like actuarial modeling and financial forecasting.

**Monte Carlo asset valuation with decimal precision:**

```csharp
decimal EstimateTerminalValue(
    ref ChiRng rng, int numPaths, decimal initialPrice, 
    decimal drift, decimal volatility, decimal timeToMaturity)
{
    // Models asset price evolution using geometric Brownian motion
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
// Supports float, double, and decimal types via generic T
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

## Low-overhead and predictable performance

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