# TODO: Performance Investigation for Large F# Projects

## Phase 1: Baseline Capture (5000 files, 1 module each)
- [x] Create test project with 5000 files, 1 module each
- [x] Capture baseline with stock SDK compiler
- [x] Capture timing data with --times flag
- [x] Record memory usage profile
- [x] Store all baseline data in investigation/baseline/ directory

## Phase 2: Implement Optimizations

### QueueList Improvements (src/Compiler/Utilities/QueueList.fs)
- [x] Expose `Length` property
- [x] Expose `LastElementsRev` internal property
- [x] Optimize `GetEnumerator()` to avoid full ToList()
- [x] Optimize `foldBack` using List.fold on reversed tail
- [x] Add `AppendOptimized` method for efficient QueueList concatenation
- [x] Update `QueueList.append` to use `AppendOptimized`

### TypedTree Caching (src/Compiler/TypedTree/TypedTree.fs)
- [x] Add `allEntitiesByLogicalMangledNameCache` mutable field
- [x] Update `AllEntitiesByLogicalMangledName` to use caching via `cacheOptByref`
- [x] Verify cache invalidation on mutations

## Phase 3: Rebuild and Validate
- [x] Build compiler with changes
- [x] Run test to verify compilation works

## Phase 4: After-Changes Capture (5000 files, 1 module each)
- [x] Capture timing data with --times flag
- [x] Record memory usage profile
- [x] Store all data in investigation/after_changes/ directory

## Phase 5: Comparison and Summary
- [x] Create comparison tables for baseline vs after_changes

## RESULTS: Changes made performance WORSE

| Metric | Baseline (Stock SDK) | After Changes | Delta |
|--------|---------------------|---------------|-------|
| Total Time | 8:43 (523s) | 11:28 (688s) | +31% SLOWER |
| Memory | 11.69 GB | 15.01 GB | +28% MORE |

### Why Changes Made Things Worse

1. **QueueList.AppendOptimized**: Creates intermediate lists increasing allocations
2. **Caching doesn't help**: Each `CombineModuleOrNamespaceTypes` creates NEW objects, so cache is never reused
3. The fundamental O(n²) issue requires architectural changes, not caching

