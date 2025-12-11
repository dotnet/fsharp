# DList Migration TODO

## Status: IN PROGRESS

## Completed Tasks
- [x] Create comprehensive QueueList benchmarks
- [x] Identify V5 (DList with cached iteration) as best performer (4.1x faster, 1.6x memory)
- [x] Document all benchmark results
- [x] Find all QueueList usage sites (49 files)
- [x] Create DList.fsi and DList.fs implementation
- [x] Add DList to build system (FSharp.Compiler.Service.fsproj)
- [x] Verify DList compiles successfully

## QueueList Usage Sites (Priority Hot Paths)
1. **TypedTree.fs** - Core type definition (ModuleOrNamespaceType)
2. **TypedTreeOps.fs** - CombineModuleOrNamespaceTypes (MAIN HOT PATH)
3. **TypedTreePickle.fs** - Serialization
4. **Symbols.fs** - Symbol operations
5. **Optimizer.fs** - Dead code elimination
6. **fsi.fs** - Interactive

## Current Tasks

### 1. Create DList Implementation ✅ NEXT
- [ ] Create `src/Compiler/Utilities/DList.fsi` (interface file)
- [ ] Create `src/Compiler/Utilities/DList.fs` (implementation)
  - Core DList type: `type DList<'T> = DList of ('T list -> 'T list)`
  - Wrapper type `CachedDList<'T>` with lazy materialized list
  - Functions: empty, singleton, cons, append, appendMany, toList
  - QueueList-compatible API: AppendOne, ofList, map, filter, foldBack, etc.
  - Fast O(1) "DList Append DList" operation
  
### 2. Add DList to Build System
- [ ] Add DList.fsi and DList.fs to FSharp.Compiler.Service.fsproj
- [ ] Ensure proper ordering in compilation

### 3. Migrate Hot Paths
- [ ] TypedTree.fs: Change ModuleOrNamespaceType to use CachedDList
- [ ] TypedTree.fsi: Update interface
- [ ] TypedTreeOps.fs: Update CombineModuleOrNamespaceTypes (KEY OPTIMIZATION)
- [ ] TypedTreePickle.fs: Add p_dlist/u_dlist functions
- [ ] Other usage sites as needed for compilation

### 4. Build and Test
- [ ] Ensure all code builds successfully (`./build.sh -c Release`)
- [ ] Run targeted tests for TypedTree/TypedTreeOps
- [ ] Run full test suite

### 5. Performance Validation
- [ ] Rebuild compiler with DList changes
- [ ] Generate 5000 files/5000 modules test project
- [ ] Run compilation with --times flag
- [ ] Capture memory usage with /usr/bin/time -v
- [ ] Compare with baseline:
  - Baseline: 8:43 total, 11.69 GB, 171s typecheck
  - Target: ~2-3 min total (4x improvement in typecheck)
- [ ] Document results in investigation/dlist_results/

## Expected Outcome
Based on benchmarks showing V5 (DList Cached) at 4.1x faster:
- Typecheck phase: 171s → ~40-50s (4x improvement)
- Total time: 523s → ~200-250s
- Memory: Should remain similar or improve (1.6x overhead in micro-benchmark)

## Implementation Notes
- Keep all benchmark code and results (per instructions)
- DList provides O(1) append for two DLists (key optimization)
- Lazy cache ensures iteration/foldBack performance
- Wrapper type provides QueueList-compatible API surface
- Focus on hot path first: CombineModuleOrNamespaceTypes

## Rollback Plan
If DList migration causes issues:
1. Revert to QueueList (all changes localized to utilities + TypedTree*)
2. Keep benchmark results for future reference
3. Document lessons learned
