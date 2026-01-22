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
