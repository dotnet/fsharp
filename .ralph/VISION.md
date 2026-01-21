# RFC FS-XXXX Tiebreakers: Final Phase Vision

## High-Level Goal

Complete the "Most Concrete" tiebreaker feature. The core algorithm and rule engine are complete. The only remaining work is implementing the enhanced FS0041 error message that explains WHY types are incomparable when overload resolution fails.

## Current State (Rule Engine Complete)

- ✅ `compareTypeConcreteness` algorithm implemented in OverloadResolutionRules.fs
- ✅ `aggregateComparisons` dominance logic implemented
- ✅ Language feature flag `LanguageFeature.MoreConcreteTiebreaker`
- ✅ Basic diagnostic FS3575 (informational warning when tiebreaker succeeds)
- ✅ RFC Examples 1-14 tested (93 tests pass)
- ✅ All 15 rules defined in OverloadResolutionRules.fs as single source of truth
- ✅ `better()` and `wasConcretenessTiebreaker()` use the rule engine (no code duplication)
- ⚠️ Example 15 (constraint specificity) deferred due to F# language limitation FS0438

## Remaining Work: Enhanced FS0041 Error Message

### Problem

When overload resolution fails because neither candidate is strictly more concrete (incomparable types), the error message doesn't explain WHY the types are incomparable.

**Current message:**
```
error FS0041: A unique overload for method 'Invoke' could not be determined 
based on type information prior to this program point.
```

**Required enhancement (from RFC section-diagnostics.md):**
```
error FS0041: A unique overload for method 'Invoke' could not be determined 
based on type information prior to this program point.
Neither candidate is strictly more concrete than the other:
  - Invoke(x: Result<int, 'e>) - first type argument is more concrete
  - Invoke(x: Result<'t, string>) - second type argument is more concrete
```

## Implementation Strategy

### Step 1: Detect Incomparable Concreteness Cases

When overload resolution fails with multiple candidates (in `FailOverloading`), we need to:
1. Check if any pair of remaining candidates has incomparable concreteness
2. If so, compute a per-position breakdown showing which type args favor which candidate
3. Include this in the error information

### Step 2: Modify Error Formatting

In `CompilerDiagnostics.fs`, enhance the `PossibleCandidates` case to include concreteness comparison details when available.

### Implementation Files

| File | Change |
|------|--------|
| `src/Compiler/Checking/OverloadResolutionRules.fs/fsi` | Add `explainIncomparableConcreteness` function |
| `src/Compiler/Checking/ConstraintSolver.fs` | Detect incomparable pairs, pass details to error |
| `src/Compiler/Checking/MethodCalls.fs` | Extend `UnresolvedOverloading` to carry concreteness details |
| `src/Compiler/Driver/CompilerDiagnostics.fs` | Format the enhanced error message |
| `src/Compiler/FSComp.txt` | Add new string resource for enhanced message |

## Constraints

1. **Example 15 out of scope**: Constraint specificity comparison cannot be implemented due to F# limitation FS0438 (methods cannot differ only in generic constraints)
2. **Performance**: Enhanced error formatting only happens on failure path - no perf impact on success
3. **Backwards compatibility**: The enhanced message adds information, doesn't remove any

## Definition of Done (Final)

1. ✅ No code duplication between `better()` and `wasConcretenessTiebreaker()` - DONE
2. ✅ All rules defined in `OverloadResolutionRules.fs` as single source of truth - DONE
3. ✅ Enhanced FS0041 error message explains why types are incomparable - DONE
4. ✅ Test for enhanced error message - DONE
5. ✅ All existing tests pass - VERIFIED (93 TiebreakerTests)
6. ✅ Code passes quality audit for if-then chain avoidance - DONE
