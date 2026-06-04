# Test Method Naming Convention

**`[UnitOfWork]_[TestedScenario]_[ExpectedBehavior]`** — three PascalCase segments,
separated by single underscores.

Only one thing is *enforced*: the three-segment shape (analyzer **CV1001**).
Everything below is **guidance and good taste**, not a linter. Aim for names a
newcomer understands on first read, and remember: **consistency within a file beats
rigid rule-following.**

## The golden rule

**Read the test body and name what it actually does.** Old names may be obsolete,
vague, or even *overclaiming* — e.g. `Sin_ExtremelyLargeAngle_ComputesCorrectly`
whose body only asserts "doesn't throw." Trust the body, never the existing name.

## 1. UnitOfWork — what's under test

The operation being exercised — not the type, and not the test class (the class
already supplies that context: in `Gamma/SampleTests` the unit is `Sample`, not
`Gamma` or `ChiRng`).

- Prefer the **method or member**: `Sin`, `Lerp`, `Indexer`, `Eye`, `Sample`.
- **Fluent chains** (`rng.Prime(min,max).Sample()`): name the call whose behavior
  you're asserting — usually the **producer** (`Sample_…`). Switch to the builder
  only when *construction itself* is the subject: `Prime_WithNegativeRange_Throws`.
- When a class tests many members of one type, name each by its real member
  (`Eye_…`, `Hilbert_…`, `Indexer_…`), not the type (`Matrix_…`).
- A whole-component quality test with no single method may use the component
  (`ChiHash_…`) — the rare exception, not the default.

## 2. TestedScenario — the meaningful condition

General, not granular — never enumerate `[InlineData]` values.

- There's almost always a scenario. If "I just called it" is all you have, name the
  **input space** instead: `Sample_AcrossShapeAndScale_…`,
  `NextDouble_OverUnitInterval_…`. Avoid empty filler (`CalledRepeatedly`,
  `Always`). For a genuinely nullary test, a neutral `Default` beats a fake.
- Include a **type** only when the type *is* the variation — written bare, with
  framework casing: `Next_Int_…`, `Sample_UInt128_…` (not `ForInt`, not `Uint128`).
- **Descriptive domain terms are welcome** (`SpecialAngles`, `AllQuadrants`,
  `TwinPrimes`, `Goldbach`, `LoremIpsum`). Only third-party brands/IP are out.

## 3. ExpectedBehavior — the observable result

A concrete assertion, led by a verb: `Returns…`, `Throws…`, `Produces…`, `Is…`,
`Matches…`.

- **Be specific.** Use the landmark value when there is one (`ReturnsZero`,
  `ReturnsOne`); use `ReturnsExpectedResult` for table-driven checks. Name the
  exception: `ThrowsArgumentException`, not `ThrowsException`.
- **Avoid words that assert nothing:** `Works`, `IsCorrect`, `HasCorrectValues`,
  `ComputesCorrectly`, `ShowsReasonable…`, `CanFind…`.
- **No third-party brands/IP** (`Dota2`, `SkullBasher`) — describe the mechanic.
- **Don't name a phenomenon and then restate its definition**
  (`AvalancheEffect_SmallInputChangesCauseLargeHashChanges`) — pick the concrete
  observable.

## Quick reference (real before → after from this suite)

| Before | After | Why |
|---|---|---|
| `Sample_ProducesDistributionWithCorrectStatistics` | `Sample_AcrossShapeAndScale_MatchesGammaDistribution` | name the input space (was only 2 segments) |
| `PrimeSampler_TwinPrimes_FollowExpectedDistribution` | `Sample_TwinPrimes_FollowExpectedDistribution` | unit = producer, not the sampler type |
| `Primes_WithInvalidParameters_ThrowsArgumentOutOfRangeException` | `Prime_WithInvalidParameters_ThrowsArgumentOutOfRangeException` | construction is the subject → builder unit |
| `Sin_ExtremelyLargeAngle_ComputesCorrectly` | `Sin_ExtremelyLargeAngle_DoesNotThrow` | name must match the body |
| `Asin_OutOfRange_ThrowsException` | `Asin_OutOfRange_ThrowsArgumentException` | name the exception |
| `Lerp_Double_StepZero_ReturnsOrigin` | `Lerp_StepZero_ReturnsOrigin` | drop the incidental type |
| `NegativeBinomial_IsEquivalentToGeometric_WhenNumSuccessesIsOne` | `Sample_WhenNumSuccessesIsOne_MatchesGeometric` | producer unit + scenario before behavior |

## Applying it (operational)

- Work **one unit (folder) at a time**; keep names consistent within the unit.
- **Read each body first** — derive all three segments from observed behavior.
- Rename the **method declaration only**; don't touch test logic, and leave
  commented-out tests alone.
