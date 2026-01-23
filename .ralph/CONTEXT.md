# Product Increments

This file is updated after each sprint completes. Use it to understand what was delivered.

---

## Sprint 1: Anonymous record field ordering

**Summary:** Completed in 5 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Array indexer get_Item

**Summary:** Fixed array indexing in LINQ expressions to generate proper array index expressions instead of GetArray method calls. This enables LINQ providers like Azure Cosmos DB to translate array access correctly.

**Issue:** #16918

**Root cause:** The `ArrayLookupQ` pattern in Linq.fs expected 3 type parameters (`GenericArgs [|_; _; _|]`) but `GetArray` only has 1 type parameter. This caused the pattern to never match, so array access fell through to the default method call handling.

**Fix:** Changed `GenericArgs [|_; _; _|]` to `GenericArgs [|_|]` in Linq.fs line 626.

**Files touched:**
- src/FSharp.Core/Linq.fs (1 line change)
- tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Quotations/FSharpQuotations.fs (3 new tests)
- docs/release-notes/.FSharp.Core/10.0.300.md (1 new entry)

**Tests added:**
- `Array indexing produces ArrayIndex expression not GetArray - issue 16918`
- `Nested array member access produces clean LINQ expression - issue 16918`  
- `Array indexing with variable index produces clean expression`

---

## Sprint 3: Tuple join and groupBy

**Summary:** Fixed tuple handling in join conditions and groupBy operations. Inline tuple joins like `join b on ((a.Id1, a.Id2) = (b.Id1, b.Id2))` now work correctly.

**Issues:** #7885, #47

**Root cause:** The `AnonymousObject<T1, T2, ...>` types used to represent tuples in LINQ query translation did not implement `Equals` and `GetHashCode`. This caused join operations to use reference equality instead of structural equality, resulting in no matches for tuple join keys.

**Fix:** Added `Equals` and `GetHashCode` implementations to all `AnonymousObject` types in MutableTuple.fs. The implementations use `EqualityComparer<T>.Default` for proper generic equality comparison and a consistent hash code algorithm.

**Files touched:**
- src/FSharp.Core/MutableTuple.fs (complete rewrite with Equals/GetHashCode)
- tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Linq/QueryTests.fs (new test file)
- tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Linq/NullableOperators.fs (fixed namespace)
- tests/FSharp.Core.UnitTests/FSharp.Core.UnitTests.fsproj (added test files)
- tests/FSharp.Core.UnitTests/FSharp.Core.SurfaceArea.netstandard21.release.bsl (updated baseline)
- docs/release-notes/.FSharp.Core/10.0.300.md (1 new entry)

**Tests added:**
- `Inline tuple join returns correct matches - issue 7885`
- `Inline tuple join matches function-based tuple join - issue 7885`
- `GroupBy with tuple key works - issue 47`
- `Accessing tuple elements after groupBy works - issue 47`
- `GroupBy with tuple key allows iteration over group elements`
- `GroupJoin with inline tuple key works`
- `AnonymousObject with same values are equal`
- `AnonymousObject with different values are not equal`
- `AnonymousObject hash codes are consistent with equality`
- `CastingUint` (existing test, fixed namespace)

---

## Sprint 3: Tuple join and groupBy

**Summary:** Completed in 3 iterations

**Files touched:** Check git log for details.

---

## Sprint 4: Tuple select IQueryable

**Summary:** Fixed tuple/multi-value projections in queries to preserve IQueryable type, enabling query composition and async operations like ToListAsync() in Entity Framework Core.

**Issues:** #3782, #15133

**Root cause:** When a query had a tuple projection like `select (p.Id, p.Name)`, the F# query system was:
1. First using `Queryable.Select` to project to `AnonymousObject` types (mutable tuples)
2. Then using `Enumerable.Select` + `AsQueryable()` to convert back to F# tuples

The `Enumerable.Select` step broke the IQueryable chain, producing `EnumerableQuery` instead of preserving the original provider's queryable type. This broke Entity Framework Core's ability to translate the query or use async operations.

**Fix:** Changed `TransInnerWithFinalConsume` in Query.fs to use `Queryable.Select` (via `MakeSelect` with `isIQ=true`) when the source is IQueryable, instead of using `Enumerable.Select` + `AsQueryable()`.

**Files touched:**
- src/FSharp.Core/Query.fs (2 locations fixed)
- tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Linq/QueryTests.fs (7 new tests added)
- docs/release-notes/.FSharp.Core/10.0.300.md (1 new entry)

**Tests added:**
- `Tuple select preserves IQueryable type - issue 3782`
- `System.Tuple select preserves IQueryable type`
- `F# tuple and System.Tuple produce equivalent query behavior`
- `Tuple select query can be composed with Where - issue 15133`
- `Tuple select query can be composed with OrderBy - issue 15133`
- `Record projection query is composable`
- `Multi-element tuple select preserves all elements for composition`

---

## Sprint 4: Tuple select IQueryable

**Summary:** Completed in 8 iterations

**Files touched:** Check git log for details.

---

## Sprint 5: EvaluateQuotation and edge cases

**Summary:** Fixed edge cases in quotation evaluation and query conditionals.

**Issues:** #19099, #3445 (full fix); #3845 (documented as known limitation - requires compiler warning)

**Root causes fixed:**
- #19099: ConvExprToLinqInContext was missing handlers for Sequential, VarSet, FieldSet, PropertySet patterns. EvaluateQuotation was using Func<unit, ty> but when ty is unit, LINQ's System.Void can't be a return type.
- #3445: TransInner's IfThenElse handler was passing `t.Type` (IQueryable<T>) to MakeEmpty when it should pass the element type T.
- #3845: headOrDefault with tuple returns null for empty sequences. Accessing tuple fields on null causes NRE. This requires a compiler warning for proper fix (per VISION.md Option A) - documented as known limitation.

**Files touched:**
- src/FSharp.Core/Linq.fs (added Sequential, VarSet, FieldSet, PropertySet handlers; fixed EvaluateQuotation for unit return)
- src/FSharp.Core/Query.fs (fixed IfThenElse to extract element type)
- tests/FSharp.Core.UnitTests/FSharp.Core/Microsoft.FSharp.Linq/QueryTests.fs (9 new tests)
- docs/release-notes/.FSharp.Core/10.0.300.md (2 new entries)

**Tests added:**
- `EvaluateQuotation handles Sequential expressions - issue 19099`
- `EvaluateQuotation handles void method calls - issue 19099`
- `EvaluateQuotation handles unit return - issue 19099`
- `Query with if-then no else compiles and runs - issue 3445`
- `Query with if-then no else with false condition returns empty - issue 3445`
- `Query with complex if-then condition works - issue 3445`
- `headOrDefault with empty sequence returns default`
- `headOrDefault with matching element returns first match`
- `headOrDefault with tuple and no match returns null - issue 3845 known limitation`

---

## Sprint 6: FS1182 false positive (Issue #422)

**Summary:** Fixed! The issue was resolved by marking synthetic lambda parameters in query translation as compiler-generated.

**Issue:** #422

**Root cause:** Query expression translation creates synthetic lambdas for projection parameters. The lambda parameters are new Vals that may not be directly referenced, triggering false FS1182 "unused variable" warnings.

**Solution:** Mark synthetic lambda parameters as compiler-generated using `mkSynCompGenSimplePatVar`. The FS1182 check in `PostInferenceChecks.fs` skips warnings for compiler-generated Vals.

**Files touched:**
- src/Compiler/Checking/Expressions/CheckComputationExpressions.fs (added helper functions, updated mkSimplePatForVarSpace and join patterns)
- tests/FSharp.Compiler.ComponentTests/CompilerOptions/fsc/warnon/warnon.fs (added 5 new tests for query variable usage)
- tests/FSharp.Compiler.Service.Tests/ProjectAnalysisTests.fs (updated Project12 baseline for compgen symbols)
- .ralph/VISION.md (updated documentation)

**Tests added:**
- `Query variable used in where does not trigger FS1182 - issue 422`
- `Query variable used in let binding does not trigger FS1182 - issue 422`
- `Join variable used in select does not trigger FS1182 - issue 422`
- `Multiple query variables in nested for do not trigger FS1182 - issue 422`
- Plus 2 existing tests updated

**Side effect:** Query variable symbols now report `IsCompilerGenerated = true` via FSharp.Compiler.Service APIs. This is intentional and accurate.

---

## Sprint 5: EvaluateQuotation and edge cases

**Summary:** Completed in 4 iterations

**Files touched:** Check git log for details.

---
