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
**Status**: 🔬 Under investigation
**Location**: `ConstraintSolver.fs:3460` (before `FilterEachThenUndo`)
**Hypothesis**: Filter candidates by argument count before expensive type checking
**Expected Impact**: High - could eliminate 50%+ candidates early
**Notes**:
- Current code: `candidates |> List.filter (fun cmeth -> cmeth.IsCandidate(m, ad))`
- `IsCandidate` checks: accessibility, arity, obj args, named args assignment
- Could add stricter arity check earlier in the pipeline

---

### 2. Overload Resolution Caching
**Status**: 🔬 Under investigation  
**Location**: New cache at `ConstraintSolver.fs` or `CheckExpressions.fs`
**Hypothesis**: Cache (MethodGroup + ArgTypes) -> ResolvedMethod mapping
**Expected Impact**: Very High for repetitive patterns like test files
**Notes**:
- Key: method group identity + caller argument types
- Challenge: Invalidation when types are refined during inference
- May need to scope cache per expression checking context

---

### 3. Lazy CalledMeth Construction
**Status**: 🔬 Under investigation
**Location**: `MethodCalls.fs:534-568` (CalledMeth constructor)
**Hypothesis**: Defer MakeCalledArgs and other expensive work
**Expected Impact**: Medium - reduces allocations
**Notes**:
- CalledMeth construction calls `MakeCalledArgs` which iterates all params
- Could lazily compute `fullCurriedCalledArgs` only when needed

---

### 4. Quick Type Compatibility Check
**Status**: 🔬 Under investigation
**Location**: Before `CanMemberSigsMatchUpToCheck`
**Hypothesis**: Fast path rejection based on obvious type mismatches
**Expected Impact**: Medium - skip full unification for clearly incompatible overloads
**Notes**:
- E.g., caller has `int, int`, skip overload expecting `IEqualityComparer`
- Must be careful with generics and type-directed conversions

---

### 5. Batch Trace Operations
**Status**: 🔬 Under investigation
**Location**: `ConstraintSolver.fs:497` (FilterEachThenUndo)
**Hypothesis**: Reduce trace allocation/deallocation overhead
**Expected Impact**: Low-Medium - reduces GC pressure
**Notes**:
- Each candidate creates new Trace via `Trace.New()`
- Could pool traces or batch undo operations

---

### 6. Parallel Candidate Evaluation
**Status**: ⏸️ Paused - needs research
**Location**: `FilterEachThenUndo`
**Hypothesis**: Evaluate candidates in parallel with isolated traces
**Expected Impact**: High on multi-core, but complex
**Notes**:
- Traces have shared state that would need isolation
- Type inference updates are not thread-safe
- May not be feasible without major refactoring

---

### 7. Method Group Signature Indexing
**Status**: 🔬 Under investigation
**Location**: MethInfo/InfoReader level
**Hypothesis**: Pre-index overloads by (paramCount, firstParamType) for fast lookup
**Expected Impact**: Medium for very large overload sets
**Notes**:
- Could build index when method group is first accessed
- Trade-off: index build cost vs repeated resolution cost

---

### 8. Skip Subsumption for Exact Matches
**Status**: 🔬 Under investigation
**Location**: `ConstraintSolver.fs:3500-3513` (exactMatchCandidates)
**Hypothesis**: If exact match found quickly, skip subsumption phase entirely
**Expected Impact**: High for typed calls (already fast), low for untyped
**Notes**:
- Current code already has exact match path at line 3500
- May be able to short-circuit earlier

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
