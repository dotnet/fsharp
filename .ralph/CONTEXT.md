# Product Increments

This file is updated after each sprint completes. Use it to understand what was delivered.

---

## Sprint 1: Fix core algorithm

**Summary:** Completed in 3 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Flip test expectations

**Summary:** Verified - no changes needed. Sprint 1 already correctly configured all test expectations:
- Example 2 (t vs Option<t>): already `shouldSucceed`
- Example 5 (partial concreteness): already `shouldSucceed`  
- Example 6 (incomparable): correctly `shouldFail`
- Example 7 (ValueTask): already `shouldSucceed`
- Example 8 (CE Source): already `shouldSucceed`

**Verification:** Build succeeded, 91/91 tiebreaker tests passing.

---

## Sprint 2: Flip test expectations

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 3: Add diagnostics FS35xx

**Summary:** Completed in 3 iterations

**Files touched:** Check git log for details.

---

## Sprint 4: Add language feature flag

**Summary:** Added `LanguageFeature.MoreConcreteTiebreaker` as F# 10.0 stable feature.

**Changes:**
- Added `MoreConcreteTiebreaker` to LanguageFeature enum in LanguageFeatures.fs/fsi
- Added feature to version 10.0 in features dictionary
- Added feature string `featureMoreConcreteTiebreaker` in FSComp.txt
- Gated `better()` concreteness comparison with `g.langVersion.SupportsFeature(LanguageFeature.MoreConcreteTiebreaker)`
- Gated `wasConcretenessTiebreaker` helper similarly

**Verification:** Build succeeded, 93/93 tiebreaker tests passing.

---

## Sprint 4: Add language feature flag

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 5: Release notes and baselines

**Summary:** Added release notes entries and fixed formatting.

**Changes:**
- Added release notes entry in `docs/release-notes/.FSharp.Compiler.Service/11.0.0.md` under "Added" section
- Added release notes entry in `docs/release-notes/.Language/preview.md` for language feature
- Applied formatting via `dotnet fantomas` to `OverloadResolutionRules.fs/fsi`
- Surface area baselines not needed (module is internal)

**Verification:** Build succeeded, 93/93 tiebreaker tests passing, formatting check passed.

---

## Sprint 5: Release notes and baselines

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 1: Verification: Run full test suite

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Audit: RFC example coverage verification

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---
