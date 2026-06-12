# ChiVariate

ChiVariate is a deterministic, data-oriented random engine for both everyday randomness and statistically rigorous simulations. It also includes deterministic fixed-point arithmetic and decimal math utilities that are useful on their own.

## Who is ChiVariate for

Whether you're writing a game or a Monte Carlo simulator, ChiVariate gives you expressive tools focused on predictable performance and statistical integrity.

### Game developers, engine architects, and real-time systems engineers

If you're building games, real-time systems, or low-overhead libraries and care about:

- Data-oriented, non-allocating randomness for hot paths
- Expressive APIs like `FlipCoin()`, `RollDie(20)`, or `PickEnum<Rarity>()`
- Deterministic procedural generation (textures, meshes, levels)
- Replay, save/load, and undo/redo via full RNG state snapshot and restore
- Comprehensive fixed-point type for bit-for-bit cross-platform math

### Researchers, data scientists, and statistical computing professionals

If you're working on simulations, modeling, ML pipelines, or numerical methods and need:

- Statistical rigor and cross-platform reproducibility
- 96-bit decimal variate support for precision-critical financial modeling
- Wide range of probability distributions (from Beta to Zipf)
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

## Documentation

- [Design principles](docs/design_principles.md) — Chance API, statistical integrity, and performance
- [Probability distributions](docs/probability_distributions.md) — discrete, continuous, and multivariate distributions
- [Samplers and sequences](docs/samplers_and_sequences.md) — utility samplers, quasi-random sequences, and ChiFixed
- [Trade-offs and gotchas](docs/trade_offs.md) — ref structs, decimal precision, determinism, and ChiMatrix
- [Extensibility](docs/extensibility.md) — custom RNG sources and distribution samplers

## Project status

ChiVariate is actively developed. I build and improve this library because I use it across my own hobby projects. It grows from real needs, not deadlines.

The design follows four goals, in priority order: Determinism, Convenience, Clarity, and Completeness.

The `master` branch reflects ongoing work and may include breaking changes. Use released versions for stability. See the [changelog](CHANGELOG.md) for what's new and what's coming.

## Contributing

See [Contributing](https://github.com/JanuszPelc/ChiVariate/blob/master/CONTRIBUTING.md) for details. Bug reports and performance optimizations are especially welcome.

## License

ChiVariate is distributed under the [MIT license](https://github.com/JanuszPelc/ChiVariate/blob/master/LICENSE).

## Related projects

[CHI32](https://github.com/JanuszPelc/chi32) is a stateless, deterministic random number generator that produces high-quality 32-bit values by applying a sequence of well-mixed hashing operations to a pair of 64-bit integers. It has passed industry-standard test suites like TestU01's BigCrush and PractRand over 256 TB of data.