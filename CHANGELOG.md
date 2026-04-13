# Changelog

## v2.0.0 (Draft)

- Add ChiFixed: fast, deterministic Q31.32 fixed-point type implementing the full IFloatingPointIeee754 interface
- Add Pity sampler: PRD with escalating probability for fair-feeling randomness
- Improve decimal math performance 10-14x for Normal/Exponential samplers via the Ziggurat algorithm

## v1.0.0 (Jul 14, 2025)

- Deterministic, cross-platform reproducible randomness
- Expressive APIs for dice rolls and weighted enum choices
- Wide range of probability distributions (from Beta to Zipf)
- 96-bit decimal variate support for precision-critical modeling
- Pluggable entropy sources and custom distribution samplers
- Data-oriented, zero-cost, compile-time abstractions