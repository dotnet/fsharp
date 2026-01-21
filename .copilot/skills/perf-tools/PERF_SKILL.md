# Performance Profiling Skill

Standalone tool for profiling F# compiler performance on overloaded method calls.

## Quick Start

```powershell
cd .copilot/skills/perf-tools

# Profile compilation timing (generates test projects and compares typed vs untyped)
dotnet fsi PerfProfiler.fsx --total 1500 --output ./results
```

## Script Reference

| Script | Purpose | Key Options |
|--------|---------|-------------|
| `PerfProfiler.fsx` | Profile compilation timing | `--total`, `--methods`, `--output` |

### PerfProfiler.fsx

Standalone script that generates xUnit test projects and profiles compilation of typed vs untyped `Assert.Equal` patterns.

```bash
dotnet fsi PerfProfiler.fsx --total 1500 --output ./results
```

**Options:**
- `--total <n>` - Assert count (default: 1500)
- `--methods <n>` - Test methods (default: 10)
- `--output <dir>` - Results directory (default: ./results)

**Output:** Summary in `<output>/summary.txt` with untyped/typed times and ratio.

## Key Metrics

| Metric | Description | Target |
|--------|-------------|--------|
| Untyped/Typed ratio | Compilation time ratio | ~1.0 after optimizations |
| Time per Assert.Equal | Average ms per call | < 1ms |
| Cache hit rate | Overload resolution cache hits | Higher is better |

## Hot Paths Reference

Key code paths for method resolution performance:

| Location | Function | Purpose |
|----------|----------|---------|
| `ConstraintSolver.fs:3438` | `ResolveOverloading` | Main overload resolution entry point |
| `ConstraintSolver.fs:497` | `FilterEachThenUndo` | Speculative type checking with trace/undo |
| `ConstraintSolver.fs:520` | `TypesQuicklyCompatible` | Quick type compatibility pre-filter |
| `MethodCalls.fs:534` | `CalledMeth` constructor | Expensive candidate object creation |
| `CheckExpressions.fs` | `MethInfoMayMatchCallerArgs` | Arity pre-filter before CalledMeth |

## Optimization Patterns

### 1. Early Arity Filtering (P0 - Implemented)
**Location:** `CheckExpressions.fs` - `MethInfoMayMatchCallerArgs`

Filters candidates by argument count before expensive CalledMeth construction:
- Reject if caller provides fewer args than minRequiredArgs
- Reject if caller provides more args than method accepts (unless param array)
- **Impact:** 40-60% reduction in CalledMeth allocations

### 2. Quick Type Compatibility (P1 - Implemented)
**Location:** `ConstraintSolver.fs` - `TypesQuicklyCompatible`, `CalledMethQuicklyCompatible`

Fast path rejection for obviously incompatible types:
- Sealed types with different constructors → definitely incompatible
- Tuple/array arity mismatches → incompatible
- Conservative for generics and type-directed conversions
- **Impact:** Additional 20-40% reduction after arity filter

### 3. Lazy Property Setter Resolution (P1 - Implemented)
**Location:** `MethodCalls.fs` - CalledMeth constructor

Defers expensive property lookups until actually needed:
- Fast path for common case (no named property args)
- **Impact:** 40-60 info-reader calls avoided per overload resolution

### 4. Overload Resolution Caching (P0 - Implemented)
**Location:** `ConstraintSolver.fs` - `ConstraintSolverState`

Caches (MethodGroup + ArgTypes) → ResolvedMethod:
- Key: hash of MethInfo identities + arg type stamps
- Skipped for SRTP, conversions, type variables
- **Impact:** 99% cache hit rate for repetitive patterns

## Profiling Workflow

1. **Run profiler (generates projects and compiles):**
   ```bash
   dotnet fsi PerfProfiler.fsx --total 1500 --output ./results
   ```

2. **Analyze results:**
   - Check `results/summary.txt` for timing comparison
   - Ratio near 1.0 indicates optimizations working
   - Time per call < 1ms is target

## Related

- Issue: https://github.com/dotnet/fsharp/issues/18807
- Full optimization ideas: `METHOD_RESOLUTION_PERF_IDEAS.md` (repo root)
