# Trade-offs and gotchas

ChiVariate's fully composable, non-allocating design delivers predictable performance and flexibility, but achieving this required specific architectural choices. This section covers the key trade-offs and potential gotchas to be aware of when using the library in production.

## Working with ref structs

ChiVariate's `struct`-based design eliminates allocations but requires understanding a key constraint: RNG instances must be stored as fields, not properties. This happens because samplers use `ref` parameters to avoid copying state.

Since C# properties can't be passed by reference, attempting to use an RNG stored as a property will fail to compile when calling sampler methods. For example, declaring an RNG as a `public ChiRng Rng { get; set; }` property will cause compilation errors, while a `public ChiRng Rng;` field works perfectly. The solution is straightforward: declare RNG instances as fields rather than properties.

This design choice prevents the classic "modifying a copy" bug that plagues mutable structs, ensuring RNG state updates correctly every time.

## Precision vs performance

ChiVariate provides full support for the `decimal` type across all major distributions, enabling simulations and models that demand exact base-10 precision. This is especially important in financial and actuarial contexts, where even small base-2 rounding errors in `double` can accumulate and undermine accuracy. `decimal` avoids such errors by offering exact decimal representation, making it suitable for calculations involving monetary values, interest rates, or regulatory compliance.

The performance cost is inherent: `decimal` is a 128-bit, software-emulated type without hardware acceleration for mathematical operations like `Log`, `Sqrt`, or `Exp`, resulting in significantly slower execution compared to `double`.

ChiVariate embraces this trade-off, delivering consistent APIs across numeric types while preserving statistical integrity for precision-critical domains. Use `decimal` only when necessary for compliance or base-10 fidelity. For most general simulations, `double` remains faster and precise enough.

## Cross-platform determinism

ChiVariate guarantees bit-for-bit reproducibility across platforms and .NET runtimes, but understanding the boundaries of this guarantee is important for mission-critical applications.

**Fully deterministic across all platforms:**
- All integer distributions of any bit-size (8-bit through 128-bit)
- All `decimal` distributions - ChiVariate implements its own decimal math library with no dependencies on platform-specific math functions
- All `ChiFixed` distributions - a deterministic fixed-point type using pure integer arithmetic with lookup tables
- Hashing, seeding, and core RNG state generation

**Platform-dependent precision in last bits:**

These differences are largely academic, as any tiny precision variations are dwarfed by the inherent randomness being modeled. `float` and `double` distributions may have minute precision differences in the least significant bit on exotic platforms, as they rely on the underlying runtime's math library (`System.Math.Log`, `System.Math.Sqrt`, etc.).

**Recommendation:** For applications requiring absolute cross-platform determinism, use `decimal` or `integer` types for critical calculations or validate your specific platform combinations during testing. For most use cases, the potential `float`/`double` variance is insignificant compared to the inherent randomness being modeled.

## Working with ChiMatrix

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