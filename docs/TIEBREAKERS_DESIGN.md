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

**Note on ordering terminology:** The implementation uses internal priority numbers (Rule 13 = "MoreConcrete"), while the F# Language Spec §14.4 uses a different step numbering. The RFC refers to this as "Step 9" which corresponds to its logical position in the specification prose. Both refer to the same rule - this is a documentation vs implementation naming difference, not a bug.

The comparison uses **formal (uninstantiated) parameter types** via `FormalMethodInst`, not the instantiated types from type inference.

## Implementation Files

| File | Purpose |
|------|---------|
| `src/Compiler/Checking/ConstraintSolver.fs` | Core algorithm: `compareTypeConcreteness`, integration into `better()` |
| `src/Compiler/Checking/OverloadResolutionRules.fs/fsi` | DSL representation of all 15 overload resolution rules |
| `src/Compiler/Facilities/LanguageFeatures.fs/fsi` | `LanguageFeature.MoreConcreteTiebreaker` (F# 10.0) |
| `src/Compiler/FSComp.txt` | Diagnostic FS3575 (tcMoreConcreteTiebreakerUsed), FS3576 (tcGenericOverloadBypassed) |
| `src/Compiler/Driver/CompilerDiagnostics.fs` | Off-by-default configuration for FS3575 and FS3576 |

## Language Feature Flag

The feature is gated behind `LanguageFeature.MoreConcreteTiebreaker`:
- Enabled in F# 10.0 (stable)
- Can be enabled in earlier language versions with `--langversion:preview`

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

### Example 15: Constraint Count Comparison

The RFC specifies that type variables with more constraints should be considered more concrete than type variables with fewer constraints. This is implemented in `OverloadResolutionRules.fs` via the `countTypeParamConstraints` helper function, which counts the following 10 constraint types:
- `CoercesTo` (`:>` subtype constraint)
- `IsNonNullableStruct` (struct constraint)
- `IsReferenceType` (class constraint)
- `MayResolveMember` (member constraint)
- `RequiresDefaultConstructor` (new() constraint)
- `IsEnum` (enum constraint)
- `IsDelegate` (delegate constraint)
- `IsUnmanaged` (unmanaged constraint)
- `SupportsComparison` (comparison constraint)
- `SupportsEquality` (equality constraint)

Constraints NOT counted: `DefaultsTo` (inference-only), `SupportsNull`/`NotSupportsNull` (nullability), `SimpleChoice` (printf-specific), `AllowsRefStruct` (anti-constraint).

Note: While F# does not allow overloading methods that differ only in generic constraints (FS0438), this comparison is still needed for C# interop where such overloads may exist.

### SRTP (Statically Resolved Type Parameters) Exclusion

SRTP type parameters (denoted `^T`) are explicitly excluded from the "more concrete" comparison. This is because:
1. SRTP uses a fundamentally different constraint resolution mechanism than regular generics
2. SRTP constraints are resolved at inline expansion time, not at overload resolution time
3. Comparing SRTP constraints against regular constraints would produce confusing results

The exclusion is implemented in `OverloadResolutionRules.fs` at three levels:
- **Type variable comparison** (lines 120-121): Skip if either type var is SRTP
- **Concrete vs type var** (lines 132-133): Skip if the type var is SRTP
- **Method-level** (lines 580-606): Skip entire comparison if method has SRTP type params or SRTP in formal params

## Release Notes

- Language: `docs/release-notes/.Language/preview.md`
- Compiler Service: `docs/release-notes/.FSharp.Compiler.Service/11.0.0.md`

## References

- RFC: FS-XXXX (Most Concrete Tiebreaker)
- Related issue: [Link to fslang-suggestions issue]
- PR: [Link to PR]
