# RFC FS-XXXX: "Most Concrete" Tiebreaker for Overload Resolution

## Overview

This document describes the implementation of the "Most Concrete" tiebreaker rule for F# overload resolution. When multiple method overloads match the same call site, this feature allows the compiler to prefer the overload with more concrete (less generic) type parameters.

## Motivation

The F# overload resolution algorithm sometimes results in ambiguous calls where multiple candidates are equally viable. Common scenarios include:

- **ValueTask constructor pattern**: `ValueTask<'T>.op_Implicit(Task<'T>)` vs `ValueTask<'T>.op_Implicit('T)`
- **CE Source pattern**: `Async<'t>` vs `Async<Result<'ok, 'e>>`
- **Wrapped vs bare types**: `'t` vs `Option<'t>`

Without this tiebreaker, F# produces FS0041 (ambiguous overload) errors that force users to add explicit type annotations.

## Algorithm

### Type Concreteness Comparison

The `compareTypeConcreteness` function in `OverloadResolutionRules.fs` recursively compares two types and returns:
- `1` if the first type is more concrete
- `-1` if the second type is more concrete
- `0` if they are equally concrete or incomparable

#### Rules

1. **Type variables are less concrete than any concrete type**
   - `'t` vs `int` → `int` wins (-1)
   - `'t` vs `Option<'t>` → `Option<'t>` wins (-1)

2. **For type applications, compare element types**
   - `Option<int>` vs `Option<'t>` → `Option<int>` wins (1)
   - Recursive comparison of type arguments

3. **Dominance rule**: All comparisons must agree in direction
   - If any comparison returns 0 (incomparable), the overall result is 0
   - Mixed results (some 1, some -1) produce 0 (incomparable)

### Integration Point

The tiebreaker is integrated via `evaluateTiebreakRules` (called from `better()` in `ConstraintSolver.fs`), positioned:
- **After** Rule 12 (prefer non-generic methods)
- **Before** F# 5.0 optional/ParamArray tiebreaker

**Note on ordering terminology:** The implementation uses internal priority numbers (Rule 13 = "MoreConcrete"), while the F# Language Spec §14.4 uses a different step numbering. The RFC refers to this as "Step 9" which corresponds to its logical position in the specification prose. Both refer to the same rule - this is a documentation vs implementation naming difference, not a bug.

The comparison uses **formal (uninstantiated) parameter types** via `FormalMethodInst`, not the instantiated types from type inference.

## Implementation Files

| File | Purpose |
|------|---------|
| `src/Compiler/Checking/ConstraintSolver.fs` | Integration point: `better()` calls `evaluateTiebreakRules`, emits FS3575/FS3576 warnings |
| `src/Compiler/Checking/OverloadResolutionRules.fs/fsi` | Core algorithm: `compareTypeConcreteness`, DSL for all 15 tiebreaker rules |
| `src/Compiler/Facilities/LanguageFeatures.fs/fsi` | `LanguageFeature.MoreConcreteTiebreaker` (preview) |
| `src/Compiler/FSComp.txt` | Diagnostic FS3575 (tcMoreConcreteTiebreakerUsed), FS3576 (tcGenericOverloadBypassed) |
| `src/Compiler/Driver/CompilerDiagnostics.fs` | Off-by-default configuration for FS3575 and FS3576 |

## Language Feature Flag

The feature is gated behind `LanguageFeature.MoreConcreteTiebreaker`:
- Currently available only in preview (`--langversion:preview`)
- Not yet enabled in any stable F# language version

## Diagnostics

**FS3575** (informational warning, off by default):
- Reports when the concreteness tiebreaker resolves an ambiguous overload
- Enable with `--warnon:3575` for debugging/auditing
- Message: "Overload resolution selected '%s' based on type concreteness. The more concrete type '%s' was preferred over '%s'. This is an informational message and can be enabled with --warnon:3575."

**FS3576** (informational warning, off by default):
- Reports each generic overload that was bypassed during tiebreaker resolution
- Enable with `--warnon:3576` for detailed visibility of bypassed candidates
- Message: "A more generic overload was bypassed: '%s'. The selected overload '%s' was chosen because it has more concrete type parameters."
- Complements FS3575 by showing all candidates that lost the tiebreaker

Both diagnostics are implemented in `ConstraintSolver.fs` in the `ResolveOverloading` function and provide visibility into the tiebreaker's decision-making process.

## Test Coverage

The test suite (`tests/FSharp.Compiler.ComponentTests/Conformance/Tiebreakers/TiebreakerTests.fs`) covers:
- RFC Examples 1-15 (all examples implemented)
- Edge cases: nested generics, partial concreteness, incomparable types
- Orthogonal scenarios: byref/Span, extension methods, optional/ParamArray, SRTP
- Interaction with TDCs (type-directed conversions)

## Implementation Notes

### SRTP Integration

SRTP (statically resolved type parameters) works naturally with the tiebreaker:

- **At definition time**: SRTP type variables (`^T`) are not compared for concreteness since they represent constraints to be resolved later
- **At instantiation time**: When the inline function is called with concrete types, those types participate fully in concreteness comparison
- **Resolution path**: SRTP member constraints use the same `ResolveOverloading` function and all 15 tiebreaker rules apply

The implementation skips comparing `^T` itself (in `compareTypeConcreteness`) but does NOT exclude SRTP methods from the tiebreaker—only the SRTP type variable placeholders are skipped.

### Type Variable Comparison

When both types being compared are type variables (`'a` vs `'b`), they are treated as equally concrete (comparison returns 0).

## Release Notes

- Language: `docs/release-notes/.Language/preview.md`

## References

- RFC: FS-XXXX (Most Concrete Tiebreaker)
- Related issue: [Link to fslang-suggestions issue]
- PR: [Link to PR]
