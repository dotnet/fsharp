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

The `compareTypeConcreteness` function in `ConstraintSolver.fs` recursively compares two types and returns:
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

The tiebreaker is integrated into the `better()` function in `ConstraintSolver.fs`, positioned:
- **After** Rule 12 (prefer non-generic methods)
- **Before** F# 5.0 optional/ParamArray tiebreaker

The comparison uses **formal (uninstantiated) parameter types** via `FormalMethodInst`, not the instantiated types from type inference.

## Implementation Files

| File | Purpose |
|------|---------|
| `src/Compiler/Checking/ConstraintSolver.fs` | Core algorithm: `compareTypeConcreteness`, integration into `better()` |
| `src/Compiler/Checking/OverloadResolutionRules.fs/fsi` | DSL representation of all 15 overload resolution rules |
| `src/Compiler/Facilities/LanguageFeatures.fs/fsi` | `LanguageFeature.MoreConcreteTiebreaker` (F# 10.0) |
| `src/Compiler/FSComp.txt` | Diagnostic FS3575 (tcMoreConcreteTiebreakerUsed) |

## Language Feature Flag

The feature is gated behind `LanguageFeature.MoreConcreteTiebreaker`:
- Enabled in F# 10.0 (stable)
- Can be enabled in earlier language versions with `--langversion:preview`

## Diagnostics

**FS3575** (informational warning, off by default):
- Reports when the concreteness tiebreaker resolves an ambiguous overload
- Enable with `--warnon:3575` for debugging/auditing
- Message: "The concreteness tiebreaker selected the overload with more specific type structure"

## Test Coverage

The test suite (`tests/FSharp.Compiler.ComponentTests/Conformance/Tiebreakers/TiebreakerTests.fs`) covers:
- RFC Examples 1-14 (Example 15 deferred due to F# language limitation FS0438)
- Edge cases: nested generics, partial concreteness, incomparable types
- Orthogonal scenarios: byref/Span, extension methods, optional/ParamArray, SRTP
- Interaction with TDCs (type-directed conversions)

## Known Limitations

### Example 15: Constraint Specificity (Deferred)

The RFC proposes that `'t :> IComparable<int>` should beat `'t :> IComparable`. However, F# does not allow overloading methods that differ only in generic constraints (FS0438). This is a language limitation, not an implementation gap.

### Enhanced FS0041 Error Message (Future Work)

The RFC proposes enhanced error messages that explain WHY types are incomparable. This is a UX enhancement for future work.

## Release Notes

- Language: `docs/release-notes/.Language/preview.md`
- Compiler Service: `docs/release-notes/.FSharp.Compiler.Service/11.0.0.md`

## References

- RFC: FS-XXXX (Most Concrete Tiebreaker)
- Related issue: [Link to fslang-suggestions issue]
- PR: [Link to PR]
