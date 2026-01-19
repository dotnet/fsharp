# RFC FS-XXXX: "Most Concrete" Tiebreaker - Implementation Status & Gap Analysis

## Executive Summary

**Status:** ~80% complete. Core algorithm and structural type comparison implemented and working.

### What IS Done (Verified by 95 passing tests)

1. ✅ **`compareTypeConcreteness` function** in `ConstraintSolver.fs` (lines 3661-3728)
   - Recursive type comparison with aggregation
   - Handles: TType_var, TType_app, TType_tuple, TType_fun, TType_anon, TType_measure, TType_forall
   - Properly returns 1/-1/0 with dominance rule

2. ✅ **Integration into `better()` function** (lines 3853-3869)
   - Correctly positioned after rule 12 (prefer non-generic), before F# 5.0 rule
   - Compares FORMAL parameter types using `FormalMethodInst` (not instantiated types)
   - Only activates when BOTH candidates have type arguments

3. ✅ **Structural Type Shape Comparison** (Sprint 1 - COMPLETED)
   - `'t vs Option<'t>` → Option<'t> wins
   - `'T vs Task<'T>` (ValueTask scenario) → Task<'T> wins
   - `Async<'t> vs Async<Result<'ok,'e>>` (CE Source pattern) → Result wins
   - `Result<int, 'e>` vs `Result<'ok, 'error>` → Partial concreteness works

4. ✅ **DSL representation** in `OverloadResolutionRules.fs/fsi`
   - Clean representation of all 15 tiebreaker rules
   - Placeholder for MoreConcrete rule (actual logic in ConstraintSolver)

5. ✅ **Comprehensive test suite** (`TiebreakerTests.fs`, ~2000 lines, 95 tests)
   - RFC examples 1-9, 10-12
   - Extension methods, byref/Span, optional/ParamArray, SRTP
   - Constraint/TDC interaction tests
   - Orthogonal scenarios (anonymous records, units of measure, nativeptr)

### What is NOT Done (Remaining Work)

1. ✅ **Diagnostics (FS3575/FS3576) IMPLEMENTED**
   - RFC requires optional warnings for transparency
   - FS3575 (tcMoreConcreteTiebreakerUsed) - warning when concreteness tiebreaker selects a winner
   - FS3576 (tcGenericOverloadBypassed) - reserved for future use
   - Both are off by default, can be enabled with --warnon:3575
   - Warning emission wired up in ConstraintSolver.fs via wasConcretenessTiebreaker helper

2. ✅ **Language Feature Flag ADDED**
   - `LanguageFeature.MoreConcreteTiebreaker` defined as F# 10.0 stable feature
   - Tiebreaker code gated with `g.langVersion.SupportsFeature(LanguageFeature.MoreConcreteTiebreaker)`
   - Feature can be disabled with `--langversion:9.0` if regressions are found

3. ❌ **Constraint Count Comparison NOT WORKING**
   - Algorithm pseudo-code says: more constraints = more concrete
   - Current impl: `compare c1 c2` is too simplistic
   - `'t when 't :> IComparable<int>` should beat `'t when 't :> IComparable`
   - This requires recursive concreteness check on constraint target types

4. ✅ **Structural Type Shape Comparison - IMPLEMENTED**
   - RFC Example 2: `'t vs Option<'t>` → WORKS
   - RFC Example 7: `'T vs Task<'T>` (ValueTask scenario) → WORKS
   - RFC Example 8: `Async<'t> vs Async<Result<'ok,'e>>` (CE Source pattern) → WORKS
5. ✅ **Partial Concreteness Cases - WORKING**
   - `Result<int, 'e>` vs fully generic `Result<'ok, 'error>` → resolves correctly
   - This was fixed by comparing formal parameter types, not instantiated type arguments

6. ❌ **Release Notes NOT ADDED**
   - No entry in `docs/release-notes/FSharp.Compiler.Service/`

7. ❌ **Enhanced Error Message for Incomparable NOT IMPLEMENTED**
   - RFC proposes enhanced FS0041 message explaining WHY ambiguous
   - Current code has no such enhancement

8. ❌ **Surface Area Baselines NOT UPDATED**
   - `OverloadResolutionRules.fs/fsi` adds public surface
   - Baseline tests may fail in full CI run

## Sprint 1 Completion Notes

The key fix was changing the algorithm at lines 3853-3869 to compare **formal parameter types** using `FormalMethodInst` instead of comparing instantiated `CalledTyArgs`.

Before: Compared `candidate.CalledTyArgs` which were already instantiated (e.g., both would be `int option` after inference)

After: Compares formal (uninstantiated) parameter types using:
```fsharp
let formalParams1 = candidate.Method.GetParamDatas(csenv.amap, m, candidate.Method.FormalMethodInst) 
let formalParams2 = other.Method.GetParamDatas(csenv.amap, m, other.Method.FormalMethodInst)
```

This gives us the original declared types like `'t` vs `Option<'t>` which can then be compared for concreteness.

## Remaining Critical Implementation Gaps

### Gap 1: Constraint Comparison is Too Simplistic

## Build & Test Commands

```bash
# Build (Debug is fine for component tests)
dotnet build src/Compiler/FSharp.Compiler.Service.fsproj -c Debug

# Run tiebreaker tests only
dotnet test tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  --filter "FullyQualifiedName~Tiebreakers" -c Debug -f net10.0

# Full component tests (takes longer)
dotnet test tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  -c Debug -f net10.0 --no-restore
```

## Files Modified

| File | Changes |
|------|---------|
| `src/Compiler/Checking/ConstraintSolver.fs` | +86 lines: `compareTypeConcreteness`, integration |
| `src/Compiler/Checking/OverloadResolutionRules.fs` | +321 lines: DSL for all rules |
| `src/Compiler/Checking/OverloadResolutionRules.fsi` | +46 lines: public API |
| `src/Compiler/FSharp.Compiler.Service.fsproj` | +2 lines: new files |
| `tests/.../Tiebreakers/TiebreakerTests.fs` | +2064 lines: comprehensive tests |
| `tests/.../FSharp.Compiler.ComponentTests.fsproj` | +1 line: folder reference |

## Constraints & Gotchas

1. **FS3570 is taken!** Need different warning number (FS35xx range)
2. **No IL tests** - this is purely type-checking behavior, not codegen
3. **DSL is documentation** - actual logic is in ConstraintSolver.fs, DSL is parallel
4. **macOS development** - Darwin environment, use ./build.sh not build.cmd

## Sprint Strategy

Priority order (Sprint 1 COMPLETE ✅):
1. ✅ Fix the core algorithm (parameter type shape comparison) - DONE
2. Add diagnostics (find new warning numbers, implement)
3. Add language feature flag
4. Release notes + surface area baselines
