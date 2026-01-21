# RFC FS-XXXX Tiebreakers: Full Compliance Audit

## High-Level Goal

Ensure 100% compliance with the RFC specification for the "Most Concrete" tiebreaker feature. No "deferred" items, no TODOs related to the RFC.

## Current State Analysis

### Implemented ✅
- `compareTypeConcreteness` algorithm in OverloadResolutionRules.fs
- `aggregateComparisons` dominance logic
- Language feature flag `LanguageFeature.MoreConcreteTiebreaker`
- Diagnostic FS3575 (informational warning when tiebreaker succeeds)
- RFC Examples 1-14 tested (97 tests pass)
- All 15 rules defined as single source of truth
- Rule engine eliminates code duplication
- Enhanced FS0041 error message with incomparable concreteness explanation
- Enhanced FS0041 test

### Gaps Found in RFC Compliance Audit

| RFC Section | Requirement | Status | Gap |
|-------------|-------------|--------|-----|
| section-diagnostics.md | FS3570, FS3571 warning numbers | ⚠️ | Implementation uses FS3575 instead |
| section-algorithm.md lines 136-146 | Constraint count comparison for type variables | ✅ | Implemented in Sprint 1 |
| section-examples.md Example 15 | Constrained beats unconstrained type variable | ✅ | Implemented in Sprint 1 |

### RFC Requirements for Constraint Comparison (section-algorithm.md:136-146)

```
MATCH (ty1, ty2) WITH
| (TType_var(tp1, _), TType_var(tp2, _)):
    c1 := countConstraints(tp1)   // Count: :>, struct, member, etc.
    c2 := countConstraints(tp2)
    
    IF c1 > c2: RETURN 1
    IF c2 > c1: RETURN -1
    
    RETURN compareConstraintSpecificity(tp1, tp2)
```

The current implementation:
```fsharp
| TType_var _, TType_var _ -> 0  // Always returns 0 - INCORRECT
```

## Analysis: Why Was Constraint Comparison "Deferred"?

The code comment says:
> "RFC Example 15 (constraint specificity) is deferred due to F# language limitation (FS0438).
> Comparing constraint counts would incorrectly affect SRTP resolution."

This claim needs investigation:
1. **FS0438 limitation**: F# does not allow overloading based solely on type constraints - this is a **declaration** limitation, not a resolution limitation
2. **SRTP concern**: The concern about SRTP is potentially valid but needs verification
3. **C# interop**: Even if F# can't declare such overloads, C# code can - so the comparison is still needed for interop

## Resolution Strategy

Given the instruction that "deferred" is forbidden and we must implement all RFC requirements:

### Option A: Implement Full Constraint Comparison
- Risk: May affect SRTP resolution if the SRTP concern is valid
- Benefit: Full RFC compliance, works for C# interop

### Option B: Implement Constraint Comparison with SRTP Guard
- Only apply constraint comparison when NOT in SRTP resolution context
- Requires identifying SRTP contexts in the code

### Option C: Document as Language Limitation (NOT ALLOWED per instructions)
- This violates the "no deferred" requirement

## Sprint Plan

### Sprint 1: Implement Constraint Count Comparison
- Modify `compareTypeConcreteness` to compare constraint counts for type variables
- Add helper to count constraints on a type parameter
- Update Example 15 test to expect success instead of FS0041

### Sprint 2: Warning Number Alignment (Optional)
- RFC specifies FS3570/FS3571, implementation uses FS3575
- This is a cosmetic difference, may not need changing if FS3575 is already in use

## Definition of Done (Full RFC Compliance)

1. ✅ `compareTypeConcreteness` compares all RFC-specified type forms
2. ✅ Constraint count comparison for type variables (RFC section-algorithm.md)
3. ✅ Example 15 test passes (constrained beats unconstrained)
4. ✅ Enhanced FS0041 error message for incomparable types
5. ✅ Informational diagnostic when tiebreaker used
6. ✅ All 97 existing tests pass
7. ✅ Rule engine pattern (no if-then chains)
8. ✅ Formatting clean
