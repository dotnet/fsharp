# QueueList Benchmark Results Summary

## Overview

Created comprehensive BenchmarkDotNet benchmarks for QueueList to simulate the 5000-element append scenario as used in CheckDeclarations. Tested 5 implementations:

- **Original**: Current baseline implementation
- **V1**: AppendOptimized (current commit's optimization)
- **V2**: Optimized for single-element appends  
- **V3**: Array-backed with preallocation
- **V4**: ResizeArray-backed

## Key Findings

### AppendOne Performance (5000 sequential appends)

| Implementation | Mean (ms) | Ratio | Allocated | Alloc Ratio |
|----------------|-----------|-------|-----------|-------------|
| V3 (Array)     | 3.765     | 0.21  | 47.97 MB  | 38.37       |
| V4 (ResizeArray) | 12.746  | 0.73  | 143.53 MB | 114.80      |
| V2 (Optimized) | 17.473    | 0.99  | 1.25 MB   | 1.00        |
| V1 (Current)   | 17.541    | 1.00  | 1.25 MB   | 1.00        |
| Original       | 17.576    | 1.00  | 1.25 MB   | 1.00        |

**Key Insight**: V1/V2 (list-based) have identical performance to Original for AppendOne operations, as expected. V3 (array) is **4.7x faster** but allocates 38x more memory. V4 (ResizeArray) is slower due to frequent internal copying.

### Combined Scenario (append + iteration + foldBack every 100 items)

This is closest to real CheckDeclarations usage:

| Implementation | Mean (ms) | Ratio | Allocated    |
|----------------|-----------|-------|--------------|
| V3 (Array)     | 4.718     | 0.24  | 50.81 MB     |
| V4 (ResizeArray) | 13.911  | 0.70  | 150.50 MB    |
| V1 (Current)   | 19.560    | 0.98  | 1.84 MB      |
| V2 (Optimized) | 19.708    | 0.99  | 1.84 MB      |
| Original       | 19.891    | 1.00  | N/A          |

**Key Insight**: V1/V2 perform nearly identically (~1% difference, within margin of error). Array-based V3 is **4.2x faster** but allocates **27x more memory**.

## Analysis

### Why V1 (AppendOptimized) Didn't Help

1. **AppendOne dominates**: The real workload uses `AppendOne` for single elements, not `Append` for QueueLists
2. **AppendOptimized overhead**: Creating intermediate merged lists has cost without benefit for single-element case
3. **No structural sharing**: Each operation creates new objects, so optimization can't amortize

### Why V3 (Array) is Fastest

1. **Contiguous memory**: Better cache locality
2. **Direct indexing**: No list traversal overhead
3. **Simple iteration**: Array enumeration is highly optimized
4. **Trade-off**: 27-38x more memory allocation

### Recommendations

1. **For this PR**: The AppendOptimized/caching changes don't help and should be reverted
2. **Future work**: Consider array-backed implementation if willing to accept higher memory usage
3. **Real solution**: Architectural change to avoid O(n²) iterations in CombineCcuContentFragments

## Benchmark Categories

The benchmark includes 5 categories:
1. **AppendOne**: Just 5000 sequential appends
2. **AppendWithIteration**: Append + full iteration each time
3. **AppendWithFoldBack**: Append + foldBack each time
4. **Combined**: Realistic scenario with periodic operations
5. **AppendQueueList**: Appending QueueList objects (not single elements)

All results confirm: **Current optimizations (V1/V2) provide no measurable benefit** over the baseline for the actual usage pattern.
