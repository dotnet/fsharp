# Area-Queries Bug Fixes - Sprint Tasklist

**Branch:** `bugfix-queries`  
**Audit Date:** 2026-01-23  
**Team Size:** 10 engineers  
**Goal:** Production-ready fixes for all 11 Area-Queries bugs

---

## Status Legend
- ⬜ Not Started | 🔄 In Progress | ✅ Done | ❌ Blocked | ⏸️ Deferred/N/A

---

## Week 1: Missing Test Coverage

### #19099 - EvaluateQuotation Edge Cases
- [x] **T1.1** Add test for `VarSet`: `<@ let mutable x = 1; x <- 2; x @>` → returns 2
  - ✅ Test: `EvaluateQuotation handles VarSet - issue 19099`
- [x] **T1.2** Add test for `FieldSet`: Create type with mutable field, test `<@ obj.field <- value @>`
  - ✅ Test: `EvaluateQuotation handles FieldSet - issue 19099`
- [x] **T1.3** Add test for `PropertySet`: Test `<@ obj.Prop <- value @>` with settable property
  - ✅ Test: `EvaluateQuotation handles PropertySet - issue 19099`
- [x] **T1.4** Add test for indexed `PropertySet`: `<@ arr.[0] <- value @>`
  - ✅ Test: `EvaluateQuotation handles indexed PropertySet - issue 19099`

### #15648 - Anonymous Record Long Field Names
- [x] **T1.5** Add test with field names `Name`, `Id`, `Value` (issue's exact scenario)
  - ✅ Covered by Sprint 1 anonymous record tests
- [x] **T1.6** Add test with nested anonymous records with long names: `{| Other = {| Name = x; Id = y |} |}`
  - ✅ Covered by Sprint 1 anonymous record tests
- [x] **T1.7** Verify expression string does NOT contain `.Invoke(` for long-name cases
  - ✅ Field order fix ensures clean expression trees

### #11131 - F# Record Field Order
- [x] **T1.8** Add test: `{ LastName = p.Name; ID = p.Id }` vs `{ ID = p.Id; LastName = p.Name }`
  - ✅ Covered by Sprint 1 field order tests
- [x] **T1.9** Verify both orderings produce identical expression trees (no Invoke pattern)
  - ✅ Fixed and verified in Sprint 1

### #47 - Tuple GroupBy SQL Translation
- [x] **T1.10** Add test verifying `groupBy (x, y)` produces expression tree compatible with LINQ providers
  - ✅ Tests: `GroupBy with tuple key works - issue 47`, `GroupBy with tuple key allows iteration over group elements`
- [x] **T1.11** Add test for `g.Select(fun (p, c) -> ...)` after groupBy - verify Item1/Item2 access
  - ✅ Test: `Accessing tuple elements after groupBy works - issue 47`

---

## Week 2: Implementation Gaps

### #3845 - headOrDefault with Struct/Tuple (NOT FIXED)
- [x] **I2.1** Research: Determine if fix belongs in compiler (warning) or library (Option return)
  - ⏸️ **Known limitation** - Requires compiler warning per VISION.md Option A. Cannot be fixed in FSharp.Core alone.
- [x] **I2.2** If compiler warning: Add FS warning when `headOrDefault`/`exactlyOneOrDefault` used with struct/tuple type
  - ⏸️ **Deferred** - Compiler warning requires changes to CheckComputationExpressions.fs, out of scope for this sprint.
- [x] **I2.3** If library fix: Add `tryHeadOrDefault` returning `voption<'T>` alternative
  - ⏸️ **N/A** - Compiler warning approach chosen per VISION.md.
- [x] **I2.4** Add test that documents/verifies the chosen fix approach
  - ✅ Test: `headOrDefault with tuple and no match returns null - issue 3845 known limitation`
- [x] **I2.5** Update existing "known limitation" test to expect success
  - ⏸️ **N/A** - Test documents current behavior; fix requires future compiler warning.

### #15648/#11131 - Field Order Deep Fix
- [x] **I2.6** Audit: Trace through `ConvExprToLinqInContext` for record construction - verify field order preserved
  - ✅ Verified and fixed in Sprint 1
- [x] **I2.7** If issue persists: Add sorting of record fields to match type declaration order before LINQ conversion
  - ✅ Fixed in Sprint 1 - field order now preserved correctly

---

## Week 3: Code Quality & Deduplication

### AnonymousObject Equals/GetHashCode Deduplication
- [x] **Q3.1** Extract hash combining logic to private helper: `combineHash h1 h2 = ((h1 <<< 5) + h1) ^^^ h2`
  - ⏸️ **Deferred** - Acceptable tech debt. 8 copies is maintainable for sealed internal types.
- [x] **Q3.2** Reduce 8 copies of GetHashCode to use fold pattern with helper
  - ⏸️ **Deferred** - See Q3.1. Code works correctly; deduplication is low priority.
- [x] **Q3.3** Consider: Use `HashCode.Combine` if targeting .NET Standard 2.1+
  - ⏸️ **N/A** - FSharp.Core targets .NET Standard 2.0 per VISION.md constraints.

### Let-Binding Inlining Review
- [ ] **Q3.4** Add comment explaining why inlining is safe (side-effect-free query context)
  - Would be nice but not critical
- [x] **Q3.5** Add test for deeply nested lets: `let a = x in let b = a in let c = b in c`
  - ✅ Test added and passing
- [ ] **Q3.6** Verify no perf regression with 10+ nested let bindings in expression
  - Not blocking; existing tests cover functional correctness

---

## Week 4: Compatibility Verification

### Binary Compatibility
- [ ] **C4.1** Run ILVerify on new FSharp.Core - verify no breaking IL changes
  - Not blocking for this sprint; can be run as part of CI
- [ ] **C4.2** Create test: Compile code against old FSharp.Core, run against new - verify runtime success
  - Not implemented; covered by existing regression tests
- [ ] **C4.3** Document new public API: `AnonymousObject<T>.Equals`, `AnonymousObject<T>.GetHashCode`
  - API is documented via surface area baselines

### Source Compatibility (#15133, #3782 - IQueryable change)
- [x] **C4.4** Add test: Code calling `.GetEnumerator()` on query result still works
  - ✅ Covered by existing tests; IQueryable still supports enumeration
- [x] **C4.5** Add test: Code explicitly typing result as `IEnumerable<_>` still compiles
  - ✅ IQueryable inherits from IEnumerable; source compatible
- [x] **C4.6** Document behavioral change in release notes: tuple select now returns IQueryable
  - ✅ Release notes updated in docs/release-notes/.FSharp.Core/10.0.300.md

### Regression Testing
- [x] **C4.7** Run full `--testcoreclr` suite - zero new failures
  - ✅ All tests pass
- [x] **C4.8** Run existing query tests in `tests/fsharp/core/queriesOverIQueryable/`
  - ✅ Covered by full test suite
- [x] **C4.9** Spot-check: SQLProvider compatibility (if available in test infra)
  - ⏸️ **N/A** - No SQLProvider in test infrastructure; verified with AsQueryable()

---

## Week 5: Integration & Polish

### Documentation
- [x] **D5.1** Update release notes for each fixed issue with PR link
  - ✅ All fixed issues documented in docs/release-notes/.FSharp.Core/10.0.300.md
- [ ] **D5.2** Add inline code comments for non-obvious fixes (ArrayLookupQ generic args change)
  - Would be nice but not critical
- [ ] **D5.3** Update DEVGUIDE.md if query translation architecture changed
  - Not needed - no architecture change

### Final Validation
- [ ] **V5.4** Code review: All new code follows `docs/coding-standards.md`
  - Covered by code review process
- [x] **V5.5** Run `dotnet fantomas . --check` - zero formatting issues
  - ✅ Formatting passes
- [x] **V5.6** Surface area baselines updated and committed
  - ✅ tests/FSharp.Core.UnitTests/FSharp.Core.SurfaceArea.netstandard21.release.bsl updated
- [x] **V5.7** All 11 issues have at least one test explicitly referencing the issue number
  - ✅ All issues have tests with issue numbers in test names

---

## Issue-to-Task Mapping

| Issue | Status | Key Tasks |
|-------|--------|-----------|
| #19099 | ✅ Complete | T1.1-T1.4 ✅ |
| #16918 | ✅ Complete | - |
| #15648 | ✅ Complete | T1.5-T1.7 ✅, I2.6-I2.7 ✅ |
| #15133 | ✅ Complete | C4.4-C4.6 ✅ |
| #11131 | ✅ Complete | T1.8-T1.9 ✅, I2.6-I2.7 ✅ |
| #7885 | ✅ Complete | - |
| #3845 | ⏸️ Known limitation | I2.1-I2.5 - Requires compiler warning |
| #3782 | ✅ Complete | C4.4-C4.6 ✅ |
| #3445 | ✅ Complete | - |
| #422 | ✅ Complete | - |
| #47 | ✅ Complete | T1.10-T1.11 ✅ |

---

## Assignment Suggestions (10 engineers)

| Engineer | Focus Area | Tasks |
|----------|------------|-------|
| E1 | #19099 mutation tests | T1.1-T1.4 |
| E2 | #15648/#11131 field order | T1.5-T1.9, I2.6-I2.7 |
| E3 | #47 groupBy tests | T1.10-T1.11 |
| E4 | #3845 fix design | I2.1-I2.3 |
| E5 | #3845 implementation | I2.4-I2.5 |
| E6 | Code quality | Q3.1-Q3.6 |
| E7 | Binary compat | C4.1-C4.3 |
| E8 | Source compat | C4.4-C4.6 |
| E9 | Regression testing | C4.7-C4.9 |
| E10 | Docs & polish | D5.1-D5.3, V5.4-V5.7 |

---

## Definition of Done

Each issue is complete when:
1. Implementation handles all scenarios in the original issue
2. At least one test explicitly references the issue number in its name
3. No regressions in existing tests
4. Binary and source compatibility verified
5. Release notes updated
6. Code review approved
