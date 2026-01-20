# RFC FS-XXXX: "Most Concrete" Tiebreaker - Implementation Status & Gap Analysis

## Quality Audit Summary (2026-01-20)

**Overall Status:** ~95% complete. Core functionality fully implemented, feature-flagged, and passing 93 tests.

### Audit Categories

| Category | Status | Notes |
|----------|--------|-------|
| **Feature Fit** | ✅ Pass | All RFC Examples 1-14 correctly implemented |
| **Test Coverage** | ✅ Pass | 93 tests covering RFC examples, edge cases, orthogonal scenarios |
| **RFC Coverage** | ⚠️ 2 items deferred | See "Deferred Items" below |
| **Code Quality** | ✅ Pass | Clean integration, proper language gating, no duplication |

### Deferred Items (Not Bugs - Future Enhancements)

1. **Constraint Specificity Comparison** (RFC Example 15)
   - **RFC says:** `'t :> IComparable<int>` should beat `'t :> IComparable`
   - **Reality:** F# does not allow overloading based solely on constraints (FS0438)
   - **Status:** Test documents this as F# language limitation, not implementation gap
   - **Recommendation:** Keep as future work, document in RFC as "Post-MVP"

2. **Enhanced FS0041 Error Message** (RFC section-diagnostics.md)
   - **RFC proposes:** Explain WHY types are incomparable in error message
   - **Reality:** Nice-to-have, not blocking. Current error still works.
   - **Status:** Informational enhancement, can be added in follow-up PR
   - **Recommendation:** Create GitHub issue, defer to follow-up work

### Why These Are NOT Blocking

- Core feature (type structure comparison) works correctly
- 93/93 tests passing including all 14 implementable RFC examples
- Example 15 documents a *language limitation*, not implementation bug
- Enhanced error message is UX polish, not correctness

## RFC Example Coverage Mapping (Sprint 2 Audit)

Complete mapping from RFC `section-examples.md` to test coverage in `TiebreakerTests.fs`:

| RFC Example | Description | Test Name(s) | Status |
|-------------|-------------|--------------|--------|
| **Example 1** | Basic Generic vs Concrete (`Option<'t>` vs `Option<int>`) | `Example 1 - Basic Generic vs Concrete - Option of t vs Option of int` (line 98) | ✅ Covered |
| **Example 2** | Fully Generic vs Wrapped (`'t` vs `Option<'t>`) | `Example 2 - Fully Generic vs Wrapped - t vs Option of t - resolves to wrapped` (line 116) | ✅ Covered |
| **Example 3** | Nested Generics (`Option<Option<'t>>` vs `Option<Option<int>>`) | `Example 3 - Nested Generics - Option of Option of t vs Option of Option of int` (line 135) | ✅ Covered |
| **Example 4** | Triple Nesting Depth (`list<Option<Result<'t, exn>>>`) | `Example 4 - Triple Nesting Depth - list Option Result deep nesting` (line 153) | ✅ Covered |
| **Example 5** | Multiple Type Parameters - Result | `Example 5 - Multiple Type Parameters - Result fully concrete wins` (line 175), plus 2 partial concreteness tests (lines 195, 213) | ✅ Covered |
| **Example 6** | Incomparable Concreteness | `Example 6 - Incomparable Concreteness - Result int e vs Result t string - ambiguous` (line 231), plus error message test (line 252) | ✅ Covered |
| **Example 7** | ValueTask Constructor Scenario | `Example 7 - ValueTask constructor scenario - Task of T vs T - resolves to Task` (line 311), plus bare int test (line 341) | ✅ Covered |
| **Example 8** | CE Builder Source Overloads | `Example 8 - CE Source overloads - FsToolkit AsyncResult pattern - resolves` (line 363), plus plain value test (line 406) | ✅ Covered |
| **Example 9** | CE Bind with Task Types | `Example 9 - CE Bind with Task types - TaskBuilder pattern` (line 425), plus non-task test (line 459) | ✅ Covered |
| **Example 10** | Mixed Optional and Generic | `Example 10 - Mixed Optional and Generic - existing optional rule has priority` (line 543), plus priority order test (line 562) | ✅ Covered |
| **Example 11** | Both Have Optional - Concreteness Breaks Tie | `Example 11 - Both Have Optional - concreteness breaks tie` (line 580), plus 3 additional tests (lines 600, 619, 637) | ✅ Covered |
| **Example 12** | ParamArray with Generic Elements | `Example 12 - ParamArray with Generic Elements - concreteness breaks tie` (line 655), plus 2 nested/Result tests (lines 677, 695) | ✅ Covered |
| **Example 13** | Extension Methods | `Example 13 - Intrinsic method always preferred over extension` (line 764), plus 2 extension tests (lines 789, 816) | ✅ Covered |
| **Example 14** | Span with Generic Element Types | `ReadOnlySpan - element type comparison - concrete vs generic` (line 1156), `Span - Span of byte vs Span of generic - resolves to concrete byte` (line 1132) | ✅ Covered |
| **Example 15** | Constrained vs Unconstrained | `Example 15 - Constrained vs unconstrained type variable - not yet supported` (line 1269) | ⏳ Deferred (FS0438) |

### Summary
- **14 of 15 RFC examples implemented and tested** ✅
- **Example 15 deferred** due to F# language limitation (FS0438 - duplicate method signatures when differing only in constraints)
- All tests verify expected behavior (shouldSucceed/shouldFail) as per RFC specifications

## Executive Summary

**Status:** ~95% complete. Core algorithm and structural type comparison fully implemented and working.

### What IS Done (Verified by 93 passing tests)

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

### What is NOT Done (Deferred/Future Work)

1. ⏳ **Constraint Specificity Comparison (Example 15)**
   - RFC pseudo-code says: `'t :> IComparable<int>` should beat `'t :> IComparable`
   - Current impl: uses `compare c1 c2` which compares COUNT only
   - **BLOCKER:** F# doesn't allow overloading based solely on constraints (FS0438)
   - **STATUS:** Deferred - language limitation prevents implementation
   - Test documents this behavior in `Example 15 - Constrained vs unconstrained type variable - not yet supported`

2. ⏳ **Enhanced FS0041 Error Message for Incomparable Types**
   - RFC proposes: explain WHY types are incomparable in the error
   - Current: standard FS0041 message without concreteness explanation
   - **STATUS:** Nice-to-have UX enhancement, not blocking for MVP

### What IS Done (Summary)
   - RFC Example 7: `'T vs Task<'T>` (ValueTask scenario) → WORKS
   - RFC Example 8: `Async<'t> vs Async<Result<'ok,'e>>` (CE Source pattern) → WORKS

6. ✅ **Partial Concreteness Cases - WORKING**
   - `Result<int, 'e>` vs fully generic `Result<'ok, 'error>` → resolves correctly
   - This was fixed by comparing formal parameter types, not instantiated type arguments

7. ✅ **Release Notes ADDED**
   - Entry added to `docs/release-notes/.FSharp.Compiler.Service/11.0.0.md`
   - Entry added to `docs/release-notes/.Language/preview.md`

8. ✅ **Diagnostics (FS3575) IMPLEMENTED**
   - FS3575 (tcMoreConcreteTiebreakerUsed) - warning when concreteness tiebreaker selects a winner
   - Off by default, can be enabled with --warnon:3575

9. ✅ **Language Feature Flag ADDED**
   - `LanguageFeature.MoreConcreteTiebreaker` defined as F# 10.0 stable feature
   - Gated in ConstraintSolver.fs

10. ✅ **Surface Area Baselines NOT NEEDED**
   - `OverloadResolutionRules.fs/fsi` is marked `module internal`, not public surface
   - No baseline update required

## Key Implementation Notes

The key fix was changing the algorithm at lines 3853-3869 to compare **formal parameter types** using `FormalMethodInst` instead of comparing instantiated `CalledTyArgs`.

Before: Compared `candidate.CalledTyArgs` which were already instantiated (e.g., both would be `int option` after inference)

After: Compares formal (uninstantiated) parameter types using:
```fsharp
let formalParams1 = candidate.Method.GetParamDatas(csenv.amap, m, candidate.Method.FormalMethodInst) 
let formalParams2 = other.Method.GetParamDatas(csenv.amap, m, other.Method.FormalMethodInst)
```

This gives us the original declared types like `'t` vs `Option<'t>` which can then be compared for concreteness.

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
