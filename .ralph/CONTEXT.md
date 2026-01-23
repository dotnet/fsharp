# Product Increments

This file is updated after each sprint completes. Use it to understand what was delivered.

---

## Sprint 1: Remove redundant OverloadCacheTests

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: Consolidate CacheMetricsListener types

**Summary:** Consolidated `CacheMetricsNameListener` into `CacheMetricsListener` by adding optional name-only filter mode.

**Changes:**
- Modified `CacheMetrics.CacheMetricsListener` to accept optional `nameOnlyFilter` parameter
- When constructed with just a cache name string, aggregates metrics across all cache instances with that name
- When constructed with exact cache tags (internal), matches both name and cacheId exactly  
- Removed redundant `CacheMetricsNameListener` class
- Updated `FSharpChecker.CreateOverloadCacheMetricsListener()` to return `CacheMetrics.CacheMetricsListener`
- Updated surface area baseline

**Files touched:**
- src/Compiler/Utilities/Caches.fs
- src/Compiler/Utilities/Caches.fsi
- src/Compiler/Service/service.fs
- src/Compiler/Service/service.fsi
- tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.SurfaceArea.netstandard20.bsl

---

## Sprint 3: Add Hits/Misses convenience members

**Summary:** NO-OP - Sprint 2 already added `Hits` and `Misses` properties to `CacheMetricsListener`.

**Verified:**
- `member _.Hits` exists in Caches.fs (line 170)
- `member _.Misses` exists in Caches.fs (line 173)
- Corresponding signatures in Caches.fsi (lines 29-30)
- OverloadCacheTests use these properties and pass (4/4 tests)

**Files touched:** None (already complete from Sprint 2)

---
