# Product Increments

This file is updated after each sprint completes. Use it to understand what was delivered.

---

## Sprint 1: C# Test Library + Failing Tests

**Summary:** Completed in 6 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Pre-filter Implementation

**Summary:** Completed - All DoD criteria verified

**Implementation:**
- `filterByOverloadResolutionPriority` function added to ConstraintSolver.fs (line 3687)
- Function called in `GetMostApplicableOverload` at line 3721 before `bestMethods` computation
- Feature gated on `LanguageFeature.OverloadResolutionPriority` (line 3688)
- Groups methods by declaring type using `typeEquiv` for comparison
- Keeps only highest-priority methods within each group
- Passes filtered list to existing `better()` comparison

**DoD Verification:**
- ✅ filterByOverloadResolutionPriority function added to ConstraintSolver.fs
- ✅ Function called in GetMostApplicableOverload before bestMethods computation
- ✅ Feature gated on LanguageFeature.OverloadResolutionPriority
- ✅ All Sprint 1 tests pass (6 ORP tests)
- ✅ Existing MoreConcrete tiebreaker tests still pass (107 total TiebreakerTests pass)
- ✅ Build succeeds with 0 errors

---

## Sprint 2: Pre-filter Implementation

**Summary:** Completed in 4 iterations

**Files touched:** Check git log for details.

---

## Sprint 3: Extension Method & Edge Case Tests

**Summary:** Completed - 12 edge case tests added

**Tests Added (ORP Edge tests):**
1. `ORP Edge - Priority scoped per-declaring-type - different modules have independent priorities`
2. `ORP Edge - Same priority uses normal tiebreaker - int more concrete than generic`
3. `ORP Edge - Same priority uses normal tiebreaker - string more concrete`
4. `ORP Edge - Same priority array overloads - concreteness on element type`
5. `ORP Edge - Inheritance - derived new method with highest priority wins`
6. `ORP Edge - Inheritance - base priority respected in derived`
7. `ORP Edge - Instance method priority within same type`
8. `ORP Edge - Extension adds new overload type`
9. `ORP Edge - Explicit zero vs implicit zero are equal priority`
10. `ORP Edge - Complex generics - highest priority fully generic wins`
11. `ORP Edge - Complex generics - partial match when only some overloads applicable`
12. `ORP Edge - SRTP inline function - priority should be ignored for SRTP`

**C# Test Library Expanded:**
- `csharpExtensionPriorityLib` with namespace `ExtensionPriorityTests`
- Classes: ExtensionModuleA, ExtensionModuleB, SamePriorityTiebreaker, SamePriorityOptionTypes, BaseClass, DerivedClass, DerivedClassWithNewMethods, TargetClass, TargetClassExtensions, InstanceOnlyClass, ExplicitVsImplicitZero, ComplexGenerics

**DoD Verification:**
- ✅ At least 8 additional edge case tests added (12 tests)
- ✅ Extension method grouping behavior verified
- ✅ SRTP interaction tested (priority ignored for SRTP)
- ✅ Same-priority fallback to normal rules verified
- ✅ All tests pass (12/12 Sprint 3 tests, 119/119 total TiebreakerTests)
- ✅ No regressions in existing tests

---

## Sprint 3: Extension
   Method & Edge Case Tests

**Summary:** Completed in 3 iterations

**Files touched:** Check git log for details.

---
