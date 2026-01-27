# VISION: Overload Resolution Cache Quality Cleanup

## High-Level Goal

Refactor the Overload Resolution Cache implementation in the F# compiler to address quality issues identified in `QUALITY_AUDIT.md`. These are cleanup tasks that improve code quality (DRY, performance, maintainability) without changing functionality.

## Key Design Decisions

### 1. Helper Function for Cache Store Pattern ✅ DONE (Sprint 1)
Extract the duplicated cache-store pattern into a single `storeCacheResult` helper function. This is called from 4 locations in ConstraintSolver.fs and represents a clear DRY violation.
- **Completed:** Lines 296-315 define `storeCacheResult`, called at lines 3694, 3718, 3745, 4107.

### 2. CachedFailed Handling - INTENTIONAL
The audit shows `CachedFailed` is stored but effectively ignored on lookup (falls through to re-resolution). After reviewing the code at line 3833-3837, `CachedFailed` IS handled - it triggers re-resolution intentionally (per comment: "to generate proper error messages"). The audit is partially incorrect.
- **Decision:** No code change needed. The comment already explains the intent.

### 3. Cache Lookup Optimization ✅ DONE (Sprint 2)
Capture `getOverloadResolutionCache g` once at the start of `ResolveOverloading` and pass cache reference through to `storeCacheResult` and related functions. Avoids 4 redundant WeakMap lookups.
- **Completed:** `storeCacheResult`, `ResolveOverloadingCore`, and `GetMostApplicableOverload` now accept the cache directly instead of `TcGlobals`. All 4 call sites pass the cache captured at line 3820.

### 4. List Allocation Pattern - REMAINING  
Replace `mutable list + prepend + List.rev` pattern with `ResizeArray` in `tryComputeOverloadCacheKey` (lines 476-504). Build structures forward instead of prepend + reverse.

### 5. ProvidedMeth Hash Improvement - REMAINING
Include declaring type in hash for type provider methods (lines 447-448). Currently only hashes method name.

### 6. Magic Number 256 - REMAINING
Define `[<Literal>] let private MaxTokenCount = 256` in TypeHashing.fs to replace 10+ magic number occurrences (lines 413, 443, 451, 458, 464, 478, 485, 492, 549, 570).

### 7. Code Style: System.Object.ReferenceEquals ✅ DONE (Sprint 1)
Use `obj.ReferenceEquals` for consistency. Done in `storeCacheResult` helper.

### 8. Test Boilerplate - REMAINING
Extract `checkSourceHasNoErrors` helper in OverloadCacheTests.fs to reduce repetition across 8+ tests (lines 66, 119, 161, 194, 223, 258, 300, 340).

## Constraints and Gotchas

1. **Safety is paramount**: The cache must NEVER return a wrong overload. Any refactoring must preserve exact semantics.
2. **No functional changes**: These are pure quality/refactoring changes.
3. **Bootstrap awareness**: Changes to ConstraintSolver.fs affect compiler bootstrap.
4. **Formatting**: Must pass `dotnet fantomas . --check` after changes.
5. **Tests**: All existing tests must pass, especially OverloadCacheTests.

## Lessons Learned

- Sprint 1 successfully extracted `storeCacheResult` helper. Tests pass.
- Sprint 2 successfully optimized cache lookups - `storeCacheResult` now accepts the cache directly, eliminating 4 redundant `getOverloadResolutionCache` calls.
- The `CachedFailed` case is properly documented - no action needed.

## Sprint Strategy

Completed:
- Sprint 1: storeCacheResult helper extraction ✅
- Sprint 2: Cache lookup optimization (pass cache to storeCacheResult) ✅

Remaining (ordered by priority):
1. Sprint 3: ResizeArray pattern in tryComputeOverloadCacheKey  
2. Sprint 4: ProvidedMeth hash to include declaring type
3. Sprint 5: MaxTokenCount constant in TypeHashing.fs
4. Sprint 6: Test boilerplate extraction

Each sprint delivers a tested, working increment. Tests are included in each sprint.
