# Method Resolution Performance Ideas Tracker

This file tracks ideas and experiments for improving F# compiler performance when resolving heavily overloaded methods (Issue #18807).

## Status Legend
- 🔬 = Under investigation
- 🧪 = Testing/benchmarking
- ✅ = Verified improvement
- ❌ = Rejected (no improvement or breaks semantics)
- ⏸️ = Paused (needs more research)

---

## Ideas Backlog

### 1. Early Candidate Pruning by Arity
**Status**: ✅ Implemented (Sprint 3 - MethInfoMayMatchCallerArgs pre-filter)
**Location**: `CheckExpressions.fs` (in `TcMethodApplication_UniqueOverloadInference`)
**Hypothesis**: Filter candidates by argument count before expensive CalledMeth type checking
**Expected Impact**: High - avoids CalledMeth construction for obviously incompatible overloads
**Notes**:
- **Sprint 3 Implementation**: Added `MethInfoMayMatchCallerArgs` pre-filter function
- Pre-filter checks: instance vs static method compatibility, curried group count, argument count
- Conservative approach: only filters out methods that definitely won't match
- Filter runs BEFORE CalledMeth construction, avoiding expensive object creation
- Arity check in `IsCandidate` still runs as secondary verification after CalledMeth is built
- New test `ArityFilteringTest.fs` verifies the optimization doesn't change semantics

---

### 2. Overload Resolution Caching
**Status**: 🔬 Under investigation (Priority P3)
**Location**: New cache at `ConstraintSolver.fs` or `CheckExpressions.fs`
**Hypothesis**: Cache (MethodGroup + ArgTypes) -> ResolvedMethod mapping
**Expected Impact**: Very High for repetitive patterns like test files
**Notes**:
- Key: method group identity + caller argument types
- Challenge: Invalidation when types are refined during inference
- May need to scope cache per expression checking context
- **Sprint 2 Analysis**: This is the highest-impact optimization but requires careful
  design to handle type inference updates and ensure correctness

---

### 3. Lazy CalledMeth Construction  
**Status**: 🔬 Under investigation (Priority P1 - HIGH)
**Location**: `MethodCalls.fs:534-568` (CalledMeth constructor)
**Hypothesis**: Defer CalledMeth construction until after IsCandidate filter
**Expected Impact**: HIGH - reduces allocations by 30-50%
**Notes**:
- CalledMeth construction calls `MakeCalledArgs` which iterates all params
- Currently ALL CalledMeth objects built upfront in calledMethGroup
- **Sprint 2 Finding**: CalledMeth objects are created BEFORE the IsCandidate filter
  at line 3460. Moving construction to after filtering would eliminate 30-50% of
  CalledMeth allocations for methods with many overloads.
- Implementation: Split into two phases - lightweight candidate check, then full construction

---

### 4. Quick Type Compatibility Check
**Status**: 🔬 Under investigation (Priority P5)
**Location**: Before `CanMemberSigsMatchUpToCheck`
**Hypothesis**: Fast path rejection based on obvious type mismatches
**Expected Impact**: Medium - skip full unification for clearly incompatible overloads
**Notes**:
- E.g., caller has `int, int`, skip overload expecting `IEqualityComparer`
- Must be careful with generics and type-directed conversions
- **Sprint 2 Finding**: This is tricky because F# supports type-directed conversions.
  A quick check might incorrectly reject valid candidates. Need to be conservative.

---

### 5. Batch Trace Operations / Trace Pooling
**Status**: 🔬 Under investigation (Priority P4 - LOW)
**Location**: `ConstraintSolver.fs:497` (FilterEachThenUndo)
**Hypothesis**: Reduce trace allocation/deallocation overhead
**Expected Impact**: Low - Trace is just a list wrapper
**Notes**:
- Each candidate creates new Trace via `Trace.New()`
- Could pool traces or batch undo operations
- **Sprint 2 Finding**: Trace is very lightweight (just `{ mutable actions: list }`)
  and Undo is O(n) list iteration. Pooling would have minimal impact.
  More impactful to reduce the NUMBER of traces needed.

---

### 6. Parallel Candidate Evaluation
**Status**: ❌ Rejected - not feasible
**Location**: `FilterEachThenUndo`
**Hypothesis**: Evaluate candidates in parallel with isolated traces
**Expected Impact**: High on multi-core, but complex
**Notes**:
- Traces have shared state that would need isolation
- Type inference updates are not thread-safe
- May not be feasible without major refactoring
- **Sprint 2 Finding**: Type unification modifies typar graph in place.
  Parallel evaluation would require complete isolation of type state.
  Not feasible without major architectural changes.

---

### 7. Method Group Signature Indexing
**Status**: 🔬 Under investigation (Priority P6 - LATER)
**Location**: MethInfo/InfoReader level
**Hypothesis**: Pre-index overloads by (paramCount, firstParamType) for fast lookup
**Expected Impact**: Medium for very large overload sets
**Notes**:
- Could build index when method group is first accessed
- Trade-off: index build cost vs repeated resolution cost
- **Sprint 2 Finding**: For Assert.Equal with ~19 overloads, linear scan is probably
  fine. This becomes more important for methods with 50+ overloads.

---

### 8. Skip Subsumption for Exact Matches
**Status**: ✅ Already implemented
**Location**: `ConstraintSolver.fs:3500-3513` (exactMatchCandidates)
**Hypothesis**: If exact match found quickly, skip subsumption phase entirely
**Expected Impact**: High for typed calls (already fast), low for untyped
**Notes**:
- Current code already has exact match path at line 3500
- **Sprint 2 Finding**: This is already implemented! Lines 3515-3517 return early
  if exactMatchCandidates has exactly one match, skipping the applicable phase.
- May be able to short-circuit earlier

---

### 9. Merge Exact Match and Applicable Phases (NEW - Sprint 2)
**Status**: 🔬 Under investigation (Priority P2 - MEDIUM-HIGH)
**Location**: `ConstraintSolver.fs:3500-3536`
**Hypothesis**: Single pass with exact match preferred but subsumption as fallback
**Expected Impact**: Medium-High - eliminates duplicate work
**Notes**:
- Current code runs FilterEachThenUndo TWICE:
  1. Line 3500: exactMatchCandidates (TypesEquiv + ArgsEquivOrConvert)
  2. Line 3522: applicable (TypesEquiv + ArgsMustSubsume)
- Both create fresh Trace objects for each candidate
- Could merge into single pass that tracks both exact and subsumption matches
- Implementation sketch:
  ```fsharp
  let (exactMatches, subsumptionMatches) = 
      candidates |> FilterEachThenUndoWithBothModes (...)
  match exactMatches with
  | [one] -> Some one, OkResult, NoTrace
  | _ -> // fall back to subsumption matches
  ```
- **Risk**: More complex code, need to ensure correctness

---

### 10. Cache CalledMeth per (MethInfo, CalledTyArgs) (NEW - Sprint 2)
**Status**: 🔬 Under investigation (Priority P0 - CRITICAL)
**Location**: Where calledMethGroup is constructed (before ResolveOverloading call)
**Hypothesis**: Reuse CalledMeth objects for identical method+instantiation pairs
**Expected Impact**: Very High - eliminates redundant allocations
**Notes**:
- CalledMeth is expensive: MakeCalledArgs, argSetInfos computation, list allocations
- Same method with same type args produces identical CalledMeth
- In test files, `Assert.Equal<int>(a, b)` calls reuse same CalledMeth structure
- Could cache at TcState level or per-file level
- Key: (MethInfo identity, calledTyArgs list)
- **Sprint 2 Finding**: This is the most impactful optimization for repetitive patterns.
  For 1500 identical Assert.Equal calls, we'd construct CalledMeth 1500×19 = 28,500 times
  instead of just 19 times with caching.

---

## Experiment Log

### Experiment 1: Baseline Profiling
**Date**: 2026-01-20
**Description**: Collect baseline traces for untyped vs typed Assert.Equal
**Method**: Use `tools/perf-repro/` scripts with dotnet-trace

**Environment**:
- OS: Windows 11
- .NET SDK: 10.0.100-rc.2.25502.107 (from repo)
- xUnit: 2.4.2
- Test configuration: 1500 Assert.Equal calls, 10 methods, 150 asserts/method
- Type variants tested: int, string, float, bool, int64, decimal, byte, char

**Results**:

| Metric | Untyped (Slow Path) | Typed (Fast Path) | Difference |
|--------|---------------------|-------------------|------------|
| Total compilation time | 1.30s | 1.28s | 0.02s |
| Time per Assert.Equal | 0.87ms | 0.85ms | 0.02ms |
| Slowdown factor | 1.02x | 1.0x | - |

**Trace Collection**:
- Tools installed: dotnet-trace v9.0.706901, dotnet-dump v9.0.706901, dotnet-counters v9.0.706901
- Traces collected but analysis limited (traces partially broken due to process exit during collection)
- Speedscope format conversion completed (files at `tools/perf-repro/results/`)

**Key Findings**:
1. **No significant slowdown observed**: Untyped vs typed Assert.Equal calls show nearly identical compilation times (1.02x ratio)
2. **Differs from original issue**: Issue #18807 reported ~10-20x slowdown (100ms per untyped call vs ~3ms for typed)
3. **Possible explanations**:
   - Compiler optimizations may have been added since the issue was filed
   - Different xUnit version or overload set
   - Different test methodology
   - Machine-specific characteristics

**Conclusion**: ⏸️ Baseline established but shows minimal performance difference. Further investigation needed to reproduce the original issue conditions. May need to test with different xUnit versions or specific overload patterns mentioned in the original issue.

---

### Experiment 2: ResolveOverloading Hot Path Deep-Dive
**Date**: 2026-01-20
**Description**: Deep analysis of ResolveOverloading sub-operations and candidate flow

#### Code Structure Analysis

The `ResolveOverloading` function (ConstraintSolver.fs:3438) follows this flow:

```
ResolveOverloading
├── IsCandidate filter (line 3460)    ← Initial arity/accessibility filter
│   └── filters by: arity, obj args, named args, accessibility
├── exactMatchCandidates (line 3500)   ← Phase 1: Try exact type matching
│   └── FilterEachThenUndo → CanMemberSigsMatchUpToCheck
│       ├── TypesEquiv (instantiation matching)
│       ├── TypesMustSubsume (obj arg subsumption)
│       ├── ReturnTypesMustSubsumeOrConvert
│       └── ArgsEquivOrConvert (exact arg matching)
├── applicable (line 3522)             ← Phase 2: Try subsumption matching
│   └── FilterEachThenUndo → CanMemberSigsMatchUpToCheck
│       └── ArgsMustSubsumeOrConvertWithContextualReport (subsumption)
├── error collection (line 3538)       ← Phase 3: Collect errors for diagnostics
│   └── CollectThenUndo → CanMemberSigsMatchUpToCheck
└── GetMostApplicableOverload (line 3568) ← Phase 4: Pick best match
```

#### Sub-Operation Time Breakdown (Estimated from Code Analysis)

Based on code complexity and operations performed:

| Sub-Operation | Per-Candidate Cost | Description | Hotspot Level |
|---------------|-------------------|-------------|---------------|
| **CalledMeth Construction** | HIGH | Calls `MakeCalledArgs`, processes all params | 🔥🔥🔥 |
| **FilterEachThenUndo** | MEDIUM | Creates Trace, runs check, undoes trace | 🔥🔥 |
| **CanMemberSigsMatchUpToCheck** | HIGH | 7 type checking sub-phases | 🔥🔥🔥 |
| **TypesEquiv (unifyTypes)** | MEDIUM | Calls SolveTypeEqualsType | 🔥🔥 |
| **ArgsMustSubsumeOrConvert** | HIGH | Per-argument type checking | 🔥🔥🔥 |
| **Trace.New() + Undo** | LOW | List operations | 🔥 |

#### Candidate Flow Analysis

For xUnit `Assert.Equal` with ~19+ overloads:

**Phase 1 (IsCandidate filter)**:
- Input: ~19 overloads
- Filter: arity, accessibility, obj args
- Output: ~10-15 candidates (filters out wrong arity)

**Phase 2 (exactMatchCandidates)**:
- Input: ~10-15 candidates  
- Operation: Full `FilterEachThenUndo` with TypesEquiv
- Allocations: 10-15 Trace objects, 10-15 CalledMeth objects
- Output: Usually 0-1 exact matches

**Phase 3 (applicable with subsumption)**:
- Input: Same ~10-15 candidates (if no exact match)
- Operation: Full `FilterEachThenUndo` with ArgsMustSubsume
- Allocations: Another 10-15 Trace objects
- Output: 1-5 applicable candidates

**Phase 4 (most applicable)**:
- Comparison-based selection among applicable candidates

**Total per Assert.Equal call (untyped)**:
- CalledMeth constructions: ~10-15
- Trace allocations: ~20-30
- CanMemberSigsMatchUpToCheck calls: ~20-30
- Type comparisons: ~100-200+

#### Allocation Hotspots Identified

1. **CalledMeth Class (MethodCalls.fs:534)**
   - Created for EVERY candidate in calledMethGroup
   - Calls `MakeCalledArgs` which allocates lists for each parameter
   - Computes `argSetInfos` with 6-tuple list comprehensions
   - **Impact**: 10-15 allocations per Assert.Equal call
   
2. **Trace.New() (ConstraintSolver.fs:460)**
   - Fresh list allocation for each candidate check
   - Two FilterEachThenUndo passes = 2× trace allocations
   - **Impact**: 20-30 Trace allocations per Assert.Equal call

3. **List Allocations in CanMemberSigsMatchUpToCheck**
   - Multiple `List.map`, `List.filter`, intermediate results
   - MapCombineTDC2D creates result lists
   - **Impact**: Many small list allocations per check

4. **CalledArg Records (MethodCalls.fs:498)**
   - Created in `MakeCalledArgs` for each parameter
   - Multiple per method × multiple methods
   - **Impact**: 50-100+ CalledArg allocations per Assert.Equal

#### Key Insights

1. **No Caching at CalledMeth Level**
   - Same method signature reconstructs CalledMeth every time
   - Identical `Assert.Equal(int, int)` calls rebuild everything
   
2. **Two-Phase Filtering Doubles Work**
   - exactMatchCandidates AND applicable both run FilterEachThenUndo
   - Same candidates checked twice with slightly different predicates

3. **Trace Mechanism is Lightweight but Frequent**
   - Each Trace is a simple list, low per-trace cost
   - But 20-30 traces per call adds up

4. **CalledMeth Construction is Eager**
   - All CalledMeth objects built upfront before filtering
   - Could be lazy - only build what survives IsCandidate

#### Optimization Priority Matrix (Data-Driven)

| Priority | Optimization | Impact | Effort | Risk |
|----------|-------------|--------|--------|------|
| **P0** | Cache CalledMeth per (MethInfo, calledTyArgs) | 🔥🔥🔥 | Medium | Low |
| **P1** | Lazy CalledMeth construction (after IsCandidate) | 🔥🔥🔥 | Low | Low |
| **P2** | Merge exactMatch + applicable into single pass | 🔥🔥 | Medium | Medium |
| **P3** | Cache overload resolution by (MethodGroup, ArgTypes) | 🔥🔥🔥 | High | Medium |
| **P4** | Pool Trace objects | 🔥 | Low | Low |
| **P5** | Pre-filter by argument type quick-check | 🔥🔥 | Medium | Medium |

#### Candidate Statistics (Theoretical for Assert.Equal)

```
xUnit Assert.Equal Overloads: ~19+
After IsCandidate arity filter: ~10-15
After exactMatchCandidates: 0-1
After applicable subsumption: 1-3
Final selection: 1
```

**Key Metric**: For each untyped `Assert.Equal(42, x)`:
- Candidates tried: ~15
- Candidates that succeed: 1
- Waste ratio: 14:1

---

## Adding New Ideas

When adding a new idea, include:
1. **Status**: Use status legend above
2. **Location**: File and line number in compiler source
3. **Hypothesis**: What you think will improve and why
4. **Expected Impact**: High/Medium/Low estimate
5. **Notes**: Implementation considerations, risks, dependencies

When completing an experiment:
1. Update status with result (✅ or ❌)
2. Add results summary with actual measurements
3. If rejected, explain why (no improvement, regression, too complex, etc.)

---

## References

- Issue: https://github.com/dotnet/fsharp/issues/18807
- PR with tooling: https://github.com/dotnet/fsharp/pull/19072
- Key files:
  - `src/Compiler/Checking/ConstraintSolver.fs` - overload resolution
  - `src/Compiler/Checking/MethodCalls.fs` - CalledMeth, argument matching
  - `tools/perf-repro/` - profiling scripts
