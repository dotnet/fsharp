# DList Migration Decisions

## Migration Strategy

### 1. Naming Convention
- Using `CachedDList` instead of `DList` in public APIs for clarity
- Module functions follow same naming as `QueueList` for easy replacement

### 2. API Compatibility Decisions

#### QueueList.appendOne vs CachedDList.appendOne
- **QueueList**: `QueueList<'T> -> 'T -> QueueList<'T>` (curried)
- **CachedDList**: Both member (`x.AppendOne(y)`) and module function (`appendOne x y`)
- **Decision**: Use module function `CachedDList.appendOne` for compatibility
- **Perf Impact**: None - O(1) for both

#### QueueList.append vs CachedDList.append  
- **QueueList**: `QueueList<'T> -> QueueList<'T> -> QueueList<'T>` - O(n) operation
- **CachedDList**: `CachedDList<'T> -> CachedDList<'T> -> CachedDList<'T>` - **O(1) operation**
- **Decision**: Direct replacement - this is the KEY OPTIMIZATION
- **Perf Impact**: **Massive improvement** - O(1) vs O(n) for main hot path

#### QueueList.foldBack
- **QueueList**: Custom implementation with reversed tail handling
- **CachedDList**: Delegates to `List.foldBack` on materialized (cached) list
- **Decision**: Direct replacement via cached list
- **Perf Impact**: Neutral to positive (caching amortizes cost across multiple foldBack calls)

#### QueueList.ofList
- **QueueList**: Creates front/back split
- **CachedDList**: Stores list directly, creates DList wrapper
- **Decision**: Direct replacement
- **Perf Impact**: Slightly better (less splitting)

### 3. Migration Order

1. **Phase 1: Core Types** (TypedTree.fs/fsi)
   - Change `ModuleOrNamespaceType` constructor to use `CachedDList`
   - Update cache invalidation in mutation methods
   - Update all property implementations using foldBack

2. **Phase 2: Serialization** (TypedTreePickle.fs)
   - Add `p_cached_dlist` and `u_cached_dlist` functions
   - Replace `p_qlist`/`u_qlist` usage for `ModuleOrNamespaceType`

3. **Phase 3: Hot Paths** (TypedTreeOps.fs)
   - **CombineModuleOrNamespaceTypes** - CRITICAL: O(1) append instead of O(n)
   - Update all `QueueList.foldBack` calls to `CachedDList.foldBack`

4. **Phase 4: Remaining Usage Sites**
   - Symbols.fs, Optimizer.fs, fsi.fs, etc.
   - Replace as needed for compilation

### 4. Backward Compatibility

#### Pickle Format
- **Decision**: Keep pickle format compatible by converting CachedDList to/from list
- **Implementation**: `p_cached_dlist = p_wrap CachedDList.toList (p_list pv)`
- **Rationale**: Avoids breaking binary compatibility

#### FirstElements/LastElements Properties
- **QueueList**: Has separate front and reversed back lists
- **CachedDList**: Single materialized list
- **Decision**: `FirstElements` returns full materialized list, `LastElements` returns empty list
- **Rationale**: These are rarely used except in debugging; compatibility maintained
- **Perf Impact**: None for actual usage

### 5. Performance Expectations

Based on benchmarks (V5 - DList with cached iteration):

| Metric | QueueList | CachedDList | Improvement |
|--------|-----------|-------------|-------------|
| Append (2 DLists) | O(n) | **O(1)** | **Massive** |
| AppendOne | O(1) | O(1) | Same |
| foldBack (first call) | O(n) | O(n) | Same |
| foldBack (subsequent) | O(n) | O(1) (cached) | Better |
| Memory overhead | 1x | 1.6x | Acceptable |
| Combined scenario (5000 appends) | 19.7ms | 4.8ms | **4.1x faster** |

Expected impact on compilation (5000 files, same namespace):
- **Typecheck phase**: 171s → ~40-50s (4x improvement)
- **Total time**: 8:43 → ~2-3 min
- **Memory**: 11.69 GB → ~12-14 GB (small increase acceptable)

### 6. Known Limitations

1. **LastElements always empty**: CachedDList doesn't maintain separate front/back
   - **Impact**: Minimal - only used in debug views
   - **Alternative**: Could track but adds complexity with no benefit

2. **Lazy materialization**: First iteration/foldBack forces full materialization
   - **Impact**: Positive - amortizes cost across multiple operations
   - **Benchmark confirmed**: Still 4.1x faster overall

3. **Memory overhead 1.6x**: Stores both DList function and cached list
   - **Impact**: Acceptable trade-off for 4x speedup
   - **Mitigation**: Lazy evaluation means cache only created when needed

### 7. Rollback Plan

If issues arise:
1. All changes localized to TypedTree* files and utilities
2. Can revert by changing imports back to QueueList
3. DList code can remain for future use
4. Benchmark results preserved for reference

### 8. Testing Strategy

1. **Unit Tests**: Existing TypedTree tests should pass unchanged
2. **Integration**: Full compiler test suite
3. **Performance**: 5000 file scenario with --times flag
4. **Validation**: Compare against baseline results in investigation/

## Status

- [x] DList implementation complete (DList.fs/fsi)
- [x] Benchmarks confirm 4.1x improvement
- [ ] TypedTree migration
- [ ] Build validation
- [ ] Test suite validation
- [ ] Performance measurements
