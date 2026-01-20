# F# Compiler Performance Patterns Guide

This document captures performance patterns and optimization insights discovered during method resolution performance investigations. Use these patterns when profiling or optimizing the F# compiler.

## Quick Reference

### Method Resolution Hot Paths

| Location | Function | Impact | Notes |
|----------|----------|--------|-------|
| `ConstraintSolver.fs:3438` | `ResolveOverloading` | 🔥🔥🔥 | Main entry for overload resolution |
| `ConstraintSolver.fs:497` | `FilterEachThenUndo` | 🔥🔥 | Creates traces per candidate |
| `MethodCalls.fs:534` | `CalledMeth` constructor | 🔥🔥🔥 | Expensive per-candidate object |
| `CheckExpressions.fs:10096` | Pre-filter integration | ✅ | Arity filtering before CalledMeth |

---

## Pattern 1: Early Candidate Filtering (Arity Pre-Filter)

**Location**: `CheckExpressions.fs` - `MethInfoMayMatchCallerArgs`

**Pattern**: Filter method candidates by argument count *before* expensive CalledMeth construction.

**Key Insight**: CalledMeth construction is expensive (allocates MakeCalledArgs, argSetInfos, etc.). Reject obviously incompatible candidates based on arity before paying this cost.

**Implementation Notes**:
- Use `GetParamAttribs` to analyze parameters (required vs optional vs param array)
- Calculate minimum required args (excluding optional, CallerInfo, param array params)
- Allow param array methods to accept unlimited args
- Check instance vs static compatibility

**Impact**: 40-60% reduction in CalledMeth constructions for typical patterns.

---

## Pattern 2: Quick Type Compatibility Check

**Location**: `ConstraintSolver.fs` - `TypesQuicklyCompatible`, `TypesQuicklyCompatibleStructural`, `CalledMethQuicklyCompatible`

**Pattern**: Reject candidates with definitely incompatible types *before* full unification.

**Key Insight**: Full type unification (via `FilterEachThenUndo`) is expensive. If caller arg is `int` and callee expects `string`, both are sealed types with different type constructors - definitely incompatible.

**Implementation Notes**:
- Use `isSealedTy` to identify sealed types
- Use `tyconRefEq` to compare type constructors
- Be conservative: return "compatible" for generics, interfaces, abstract types
- Handle type-directed conversions (func→delegate, numeric widening, nullable)

**Impact**: Additional 20-40% reduction for overloads with same arity but different sealed param types.

---

## Pattern 3: Lazy Expensive Computations

**Location**: `MethodCalls.fs` - `CalledMeth` constructor, `computeAssignedNamedProps`

**Pattern**: Defer expensive operations until they're actually needed.

**Key Insight**: Property setter lookups (`GetIntrinsicPropInfoSetsOfType`, `ExtensionPropInfosOfTypeInScope`, etc.) are expensive but only needed when:
1. Named arguments are used
2. Those named args don't match method parameters
3. They might be property setters on the return type

**Implementation Notes**:
- Use F# `lazy` to defer computation
- Add fast-path for common case (no named property args)
- Only force lazy when `AssignedItemSetters` is actually accessed

**Impact**: 40-60 avoided info-reader calls per Assert.Equal (for typical pattern with no named args).

---

## Pattern 4: Overload Resolution Caching

**Location**: `ConstraintSolver.fs` - `ConstraintSolverState`, `tryComputeOverloadCacheKey`, `storeOverloadResolutionResult`

**Pattern**: Cache (MethodGroup + ArgTypes) → ResolvedMethod for repeated patterns.

**Key Insight**: In test files, the same method call pattern (e.g., `Assert.Equal(int, int)`) appears hundreds of times. After resolving once, cache the result for identical future calls.

**Implementation Notes**:
- Cache key: Hash of method group + list of arg type stamps
- Only cache when all arg types are fully resolved (no type variables)
- Only cache for simple cases (no SRTP, no named args)
- Be conservative: Skip caching for trait constraints, conversions

**Impact**: 99%+ cache hit rate for repetitive patterns like test files.

---

## Pattern 5: Allocation Hot Spots

### Hot Spot: CalledMeth Construction
- **Problem**: Each candidate creates CalledMeth, MakeCalledArgs, argSetInfos
- **Solution**: Pre-filter candidates (Pattern 1), lazy property lookup (Pattern 3)

### Hot Spot: Trace Allocations
- **Problem**: `FilterEachThenUndo` creates Trace per candidate, runs twice (exact + subsumption)
- **Solution**: Reduce number of candidates reaching this point (Patterns 1, 2)

### Hot Spot: List Operations in CanMemberSigsMatchUpToCheck
- **Problem**: Multiple `List.map`, `List.filter`, intermediate results
- **Solution**: Early filtering reduces invocations of this function

### Hot Spot: CalledArg Records
- **Problem**: Created in MakeCalledArgs for each parameter × each method
- **Solution**: Reduce methods entering CalledMeth construction (Pattern 1)

---

## Profiling Tools

The `tools/perf-repro/` directory contains scripts for profiling method resolution:

| Script | Purpose |
|--------|---------|
| `GenerateXUnitPerfTest.fsx` | Generate test projects with N Assert.Equal calls |
| `ProfileCompilation.fsx` | Profile compilation with dotnet-trace |
| `AnalyzeTrace.fsx` | Analyze trace files for hot paths |
| `RunPerfAnalysis.ps1` | Orchestrate profiling workflow (Windows) |
| `RunPerfAnalysis.sh` | Orchestrate profiling workflow (Unix) |

### Running a Profile

```powershell
cd tools/perf-repro
./RunPerfAnalysis.ps1 -AssertCount 1500 -Typed $false
```

### Key Metrics to Track

1. **Untyped/Typed ratio**: Should be ~1.0 (no overhead for untyped)
2. **CalledMeth constructions per call**: Should match final candidate count
3. **FilterEachThenUndo invocations**: Lower is better
4. **Cache hit rate**: Higher is better for repetitive patterns

---

## Decision Tree: When to Optimize

```
Is method heavily overloaded (>10 overloads)?
├── Yes → Check candidate filtering (Pattern 1, 2)
├── No → Check if hot in trace
    └── If hot → Check caching (Pattern 4)

Is CalledMeth construction showing in trace?
├── Yes → Check arity pre-filter (Pattern 1)
├── No → Check type checking time

Are property lookups showing in trace?
├── Yes → Check lazy initialization (Pattern 3)
├── No → Check other hot spots

Is same call pattern repeated?
├── Yes → Check caching (Pattern 4)
├── No → Focus on per-call optimizations
```

---

## References

- Issue: https://github.com/dotnet/fsharp/issues/18807
- Method resolution investigation: `METHOD_RESOLUTION_PERF_IDEAS.md`
- Key source files:
  - `src/Compiler/Checking/ConstraintSolver.fs`
  - `src/Compiler/Checking/MethodCalls.fs`
  - `src/Compiler/Checking/Expressions/CheckExpressions.fs`
