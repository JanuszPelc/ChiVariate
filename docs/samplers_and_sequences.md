# Beyond probability distributions

ChiVariate provides specialized tools for general-purpose randomness tasks and uniform sampling scenarios where traditional probability distributions aren't the right fit.

## Utility samplers

### Chance

> A toolkit for common, expressive randomization tasks, offering a non-allocating, statistically robust replacement for `System.Random`. Features expressive methods for common tasks like dice rolls (`RollDie`), coin flips (`FlipCoin`), random selections (`PickItem`), and shuffling (`Shuffle`).
>
> *Complexity: Generally `O(1)`, except for collection methods like `Shuffle` (`O(n)`). Tier 1.*

### Pity

> A Pseudo-Random Distribution (PRD) sampler with escalating probability, implementing the "pity system" found in games like Dota 2 and Genshin Impact. Unlike independent Bernoulli trials, probability escalates after each failure, bounding worst-case streaks while maintaining randomness. Ideal for critical hits, gacha pulls, and loot drops.
>
> *Complexity: `O(1)`. Tier 1.*

The stateful sampler tracks consecutive failures and supports soft pity (escalation threshold) and hard pity (guaranteed success cap).

```csharp
// Gacha system: 1% base, +2% after 5 failures, guaranteed at 20
var gacha = rng.Pity(0.01, 0.02, softPityThreshold: 5, hardPityCap: 20);
while (gacha.Sample() == 0) { }
```

### Spatial

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

### Prime

> Generates provably prime numbers from specified integer ranges up to 128-bit using deterministic Miller-Rabin testing and various optimization techniques. Ideal for number theory research, cryptographic experiments, and applications requiring mathematically guaranteed primes with uniform distribution.
>
> *Complexity: `O(r/π(r) × √n)` expected, where r is range size, π(r) is prime density, and √n is primality test cost. Performance varies significantly with range density. Tier 5.*

This specialized sampler uses various optimization techniques and deterministic Miller-Rabin testing to deliver mathematically rigorous primes suitable for cryptographic and security-adjacent research.

```csharp
// Generate a set of very large primes
var max = UInt128.MaxValue;
var primes = rng.Prime(max / 2, max).Sample(100).ToList();
```

## Quasi-random sequences

For applications requiring maximum uniformity rather than randomness, ChiVariate provides low-discrepancy sequences that generate points covering a sample space as evenly and uniformly as possible. These deterministic tools provide a powerful alternative to pseudo-random numbers when uniform coverage is the primary goal.

### Halton

> A classic, fast-to-compute low-discrepancy sequence based on prime numbers. The go-to choice for lower-dimensional problems where simplicity and speed are key, providing excellent uniformity with minimal computational overhead.
> 
> *Complexity: O(d) setup cost and an amortized O(d) per sample, where d is the number of dimensions. Highly optimized for up to 10 dimensions. Tier 1.*

### Sobol

> A more sophisticated sequence with superior distribution properties, especially in higher dimensions. The preferred choice for demanding numerical integration tasks and applications requiring the highest uniformity across many dimensions.
>
> *Complexity: O(d) setup cost and an O(d) per sample, where d is the number of dimensions. Highly optimized for up to 10 dimensions. Tier 1.*

Both canonical (pure deterministic) and randomized variants are supported, with randomization improving statistical properties while preserving uniform coverage. Common applications include quasi-Monte Carlo integration and uniform sampling in computer graphics.

```csharp
// Create a 4D Sobol sampler that produces vectors of doubles
var sobolSampler = rng.Sobol(4).OfType<double>();
var points = sobolSampler.Sample(100);
```

## Deterministic fixed-point arithmetic

### ChiFixed

> A Q31.32 fixed-point numeric type providing deterministic cross-platform results with ~9.6 decimal digit precision. Useful for simulations requiring bit-exact reproducibility across different machines, runtimes, and architectures.
>
> Range: approximately ±2 billion (any int32 value can be represented exactly). Precision: ~9.6 decimal digits.

ChiFixed implements `IFloatingPointIeee754<ChiFixed>`, so it works with .NET's generic math and all ChiVariate distribution samplers. Unlike `float` and `double`, which rely on platform-specific `System.Math` implementations, ChiFixed uses pure integer arithmetic with precomputed lookup tables for all operations.

Built around a 64-bit integer representation, ChiFixed is particularly well-suited for high-performance scenarios like game development, real-time simulations, and networked applications where determinism matters more than extended range.

When to use ChiFixed:
- Cross-platform determinism is required (save/load, replays, networked simulations)
- Transcendental functions (sin, cos, exp, log) need to be reproducible
- Precision of ~9.6 decimal digits is sufficient
- Values stay within the ±2 billion range (any int32 can be stored exactly)

Performance characteristics:
- Addition/subtraction: ~1.7x faster than `double`
- Multiplication: ~1.3x slower than `double`
- Division: ~6x slower than `double`
- Trigonometric functions: ~2x faster than `double`
- Square root: ~2.7x slower than `double`
- Memory: 8 bytes (same as `double`)

```csharp
// Generate deterministic random values in [0, 1)
var rng = new ChiRng(42);
var sample = rng.Uniform<ChiFixed>().Sample();

// Use with any distribution
var normalSampler = rng.Normal(ChiFixed.Zero, ChiFixed.One);
var values = normalSampler.Sample(1000);

// Full math support
var angle = ChiFixed.Pi / 4;
var (sin, cos) = ChiFixed.SinCos(angle);
```

Limitations:
- Arithmetic overflow saturates to min/max values
- Transcendental functions (sin, cos, exp, log) have ~0.1% error compared to double
- NaN and Infinity are sentinel bit patterns, not IEEE 754 encodings

For applications where `decimal`'s 96-bit precision is needed or values exceed ±2 billion, use `decimal` instead. For maximum floating-point performance without cross-platform determinism requirements, use `double`.