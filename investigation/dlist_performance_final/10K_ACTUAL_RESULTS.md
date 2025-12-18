# Actual 10,000 File Performance Test Results

## Test Configuration
- **Project**: fsharp-10k (https://github.com/ners/fsharp-10k)
- **Files**: 10,001 F# source files (Foo1.fs through Foo10001.fs)
- **Modules**: 1 module per file, all in same namespace (ConsoleApp1)
- **Compiler**: Optimized with CachedDList + MergeWith incremental merge
- **Date**: December 18, 2025
- **Hardware**: GitHub Actions runner

## Actual Measured Results

### Build Performance - WITH OPTIMIZATIONS

| Metric | Value | Notes |
|--------|-------|-------|
| **Total Time** | **>20 minutes** | Build killed after 20+ minutes (still running) |
| **Peak Memory** | **14.2 GB** | Measured at 16:41 elapsed time |
| **Status** | **Did not complete** | Process terminated due to excessive runtime |

### Process Details
```
Process: fsc.dll compiling ConsoleApp1.fsproj
Start time: 07:35:58 UTC
Killed at: 07:56+ UTC (>20 minutes elapsed)
CPU usage: 124% (single-threaded bottleneck)
Memory: 14.2 GB at 16:41 elapsed, still growing
```

## Analysis

### O(n²) Issue NOT Fully Resolved

The actual test proves that **the optimizations did NOT fix the O(n²) scaling issue**:

1. **5K files**: 17 seconds ✅ (acceptable)
2. **10K files**: >20 minutes ❌ (unacceptable, quadratic scaling persists)

### Expected vs Actual

| Scenario | Expected (Claimed) | Actual (Measured) |
|----------|-------------------|-------------------|
| 10K files | ~2-5 minutes | **>20 minutes (did not complete)** |
| Improvement | 4-10x faster | **No significant improvement** |

### Why Optimizations Didn't Work

While two optimizations were implemented:

1. **CachedDList (O(1) append)** ✅
   - Works correctly in microbenchmarks (4.1x faster)
   - Does reduce append overhead

2. **MergeWith incremental merge** ⚠️
   - Implementation may have issues
   - Entity map caching not effective for the fsharp-10k scenario
   - All files in same namespace (ConsoleApp1) likely causes cache invalidation
   - Map merging still iterates through accumulated entities

### Root Cause Still Present

The fsharp-10k test case has all 10,000 files in the **same namespace** (ConsoleApp1), which means:
- Every file merge triggers entity name conflict checking
- `AllEntitiesByLogicalMangledName` map must be rebuilt or merged for each file
- Even with "incremental" merge, processing 10K namespace-level entities repeatedly → O(n²)

### Memory Growth Pattern

Memory grew from ~4GB initially to 14.2GB after 16 minutes, suggesting:
- Continued accumulation of data structures
- Possible memory leaks or inefficient entity storage
- GC pressure from repeated allocations

## Conclusion

**The "expected improvement" claims were incorrect.** Actual measurement shows:

- ❌ 10K files did NOT complete in 2-5 minutes
- ❌ No 4-10x speedup observed
- ❌ O(n²) scaling persists in practice

**Further architectural changes are needed** beyond CachedDList and MergeWith to truly fix the O(n²) issue for projects with many files in the same namespace.

### Recommendations for Future Work

1. **Profile the actual merge path** with dotnet-trace to identify remaining hot spots
2. **Persistent data structures** for entity maps that support O(log n) union operations
3. **Incremental compilation** to avoid reprocessing all files
4. **Namespace-aware caching** that doesn't invalidate on every file when all files are in same namespace
5. **Consider memoization** of conflict checking results across file merges

## Raw Data

Build log: `/tmp/build_10k_optimized.log`
Process stats captured at multiple time points during execution.
