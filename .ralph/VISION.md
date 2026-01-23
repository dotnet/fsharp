# Branch Cleanup Vision: copilot/create-performance-profiling-automation

## High-Level Goal
Clean up the overload resolution performance branch for code compactness, deduplication, test quality, and removal of unnecessary abstractions.

## Branch Context
This branch adds overload resolution caching to improve F# compiler performance for heavily overloaded methods (e.g., xUnit Assert.Equal). The changes span:

1. **Core optimization** (ConstraintSolver.fs): Cache for overload resolution results
2. **Method call improvements** (MethodCalls.fs): Lazy property lookups for efficiency
3. **New public API** (service.fs/fsi): `FSharpChecker.CreateOverloadCacheMetricsListener()`
4. **New type** (Caches.fs/fsi): `CacheMetricsNameListener` for listening by cache name
5. **Tests**: OverloadCacheTests.fs, ArityFilteringTest.fs, TypeCompatibilityFilterTest.fs
6. **Struct byref fix** (CheckExpressionsOps.fs): Unrelated bugfix merged into this branch

## Key Issues Identified

### 1. CacheMetricsNameListener vs CacheMetricsListener Duplication ✅ RESOLVED
**Problem**: `CacheMetricsNameListener` (public, filters by name) duplicates 90% of the code from `CacheMetrics.CacheMetricsListener` (internal, filters by exact tags). Only the filtering predicate differs.

**Solution**: Consolidated into `CacheMetrics.CacheMetricsListener` with optional `nameOnlyFilter` parameter. The type now handles both use cases:
- Name-only filtering (public constructor): Aggregates across all cache instances with matching name
- Exact tag matching (internal constructor): Matches specific cache instance by name and cacheId

### 2. Redundant Test Cases in OverloadCacheTests.fs ✅ RESOLVED
**Problem**: 4 tests that mostly do the same thing with different call counts:
- `Overload cache hit rate exceeds 95 percent` (150 calls, hard assertion)
- `Overload cache returns correct resolution` (distinct type patterns, verifies correctness)
- `Overload cache provides measurable benefit` (200 calls, no assertion, informational only)
- `CreateOverloadCacheMetricsListener returns valid listener` (50 calls, duplicates test 1)

**Solution**: Kept tests 1 and 2 (they verify distinct behavior). Removed tests 3 and 4 (redundant variations).

### 3. Public API Necessity Questionable
**Problem**: `FSharpChecker.CreateOverloadCacheMetricsListener()` exposes `CacheMetricsNameListener` as public API, but:
- Only used in tests
- Could use existing `CacheMetrics.ListenToAll()` with filtering
- Adds surface area burden

**Solution**: Consider making this internal or removing. If external perf tooling needs it, keep it but consolidate the listener types.

### 4. ArityFilteringTest.fs and TypeCompatibilityFilterTest.fs
**Status**: These are **valid** new tests. They test edge cases for filtering optimizations that the cache relies on. They're not duplicative of existing overload tests.

**Keep as-is**: These add meaningful coverage for:
- ParamArray methods with variable args
- Optional parameters
- CallerInfo attributes
- Interface/generic type compatibility

### 5. Struct Byref Fix (CheckExpressionsOps.fs)
**Status**: Unrelated bugfix (#19068, #19070) merged into this performance branch. The code itself is fine but could be in its own PR for cleaner history.

## Design Decisions

1. **Consolidate CacheMetricsListener types** - Reduce duplication, smaller surface area
2. **Prune redundant tests** - Tests should verify distinct behaviors, not variations of the same pattern
3. **Keep correctness tests** - ArityFilteringTest and TypeCompatibilityFilterTest add real value
4. **Evaluate public API necessity** - CacheMetricsNameListener may not need to be public

## Constraints

- Must not break existing tests
- Must maintain backward compatibility for any public API that stays
- Build and test must pass after each sprint
- Surface area baselines must be updated if public API changes

## Success Criteria

1. ✅ Build passes with 0 errors
2. ✅ All tests pass
3. ✅ No code duplication between listener types
4. ✅ Tests verify meaningful distinct behaviors (not 4 variations of same test)
5. ✅ Public API is minimal and necessary
