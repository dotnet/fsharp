# Performance Profiling Skill

Tools for investigating F# compiler performance issues, especially method resolution.

## Quick Start

```powershell
cd .copilot/skills/perf-tools

# Generate test project (100 untyped Assert.Equal calls)
dotnet fsi PerfTestGenerator.fsx --total 100 --untyped

# Profile compilation timing
dotnet fsi PerfProfiler.fsx --total 1500

# Full orchestrated workflow (Windows)
.\RunPerfAnalysis.ps1 -Total 1500

# Full orchestrated workflow (Unix)
./RunPerfAnalysis.sh --total 1500
```

## Script Reference

| Script | Purpose | Key Options |
|--------|---------|-------------|
| `PerfTestGenerator.fsx` | Generate xUnit test projects | `--total`, `--typed`/`--untyped`, `--methods`, `--output` |
| `PerfProfiler.fsx` | Profile compilation timing | `--total`, `--methods`, `--output` |
| `RunPerfAnalysis.ps1` | Orchestration (Windows) | `-Total`, `-Methods`, `-Output` |
| `RunPerfAnalysis.sh` | Orchestration (Unix) | `--total`, `--methods`, `--output` |

### PerfTestGenerator.fsx

Generates F# xUnit projects with `Assert.Equal` calls for profiling overload resolution.

```bash
dotnet fsi PerfTestGenerator.fsx --total 100 --untyped   # Slow path (overload resolution)
dotnet fsi PerfTestGenerator.fsx --total 1500 --typed    # Fast path (explicit type)
```

**Options:**
- `--total <n>` - Number of Assert.Equal calls (default: 1500)
- `--methods <n>` - Number of test methods (default: 10)
- `--typed` - Use `Assert.Equal<T>()` (fast path)
- `--untyped` - Use `Assert.Equal()` (slow path, default)
- `--output <dir>` - Output directory (default: ./generated)

### PerfProfiler.fsx

Profiles compilation of typed vs untyped projects and compares timing.

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

1. **Generate test projects:**
   ```bash
   dotnet fsi PerfTestGenerator.fsx --total 1500 --untyped
   dotnet fsi PerfTestGenerator.fsx --total 1500 --typed
   ```

2. **Profile compilation:**
   ```bash
   dotnet fsi PerfProfiler.fsx --total 1500
   ```

3. **Analyze results:**
   - Check `results/summary.txt` for timing comparison
   - Ratio near 1.0 indicates optimizations working
   - Time per call < 1ms is target

## Related

- Issue: https://github.com/dotnet/fsharp/issues/18807
- Full optimization ideas: `METHOD_RESOLUTION_PERF_IDEAS.md` (repo root)
