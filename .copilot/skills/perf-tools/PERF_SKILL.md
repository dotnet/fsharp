# Performance Profiling Skill

Tools for investigating F# compiler performance issues, especially method resolution.

## Quick Start

```powershell
cd .copilot/skills/perf-tools

# Generate test project (100 untyped Assert.Equal calls)
dotnet fsi PerfTestGenerator.fsx --total 100 --untyped

# Profile compilation timing
dotnet fsi PerfProfiler.fsx --total 1500
```

## Scripts

| Script | Purpose |
|--------|---------|
| `PerfTestGenerator.fsx` | Generate typed/untyped xUnit test projects |
| `PerfProfiler.fsx` | Profile compilation with timing comparison |
| `RunPerfAnalysis.ps1` | Full workflow orchestration (Windows) |
| `RunPerfAnalysis.sh` | Full workflow orchestration (Unix) |

## PerfTestGenerator.fsx

Generates F# xUnit projects with many `Assert.Equal` calls for profiling.

```bash
dotnet fsi PerfTestGenerator.fsx --total 100 --untyped
dotnet fsi PerfTestGenerator.fsx --total 1500 --typed
```

Options:
- `--total <n>` - Number of Assert.Equal calls (default: 1500)
- `--methods <n>` - Number of test methods (default: 10)
- `--typed` - Use `Assert.Equal<T>()` (fast path)
- `--untyped` - Use `Assert.Equal()` (slow path, default)
- `--output <dir>` - Output directory (default: ./generated)

## PerfProfiler.fsx

Profiles compilation of typed vs untyped test projects and compares timing.

```bash
dotnet fsi PerfProfiler.fsx --total 1500
```

Options:
- `--total <n>` - Assert count (default: 1500)
- `--output <dir>` - Results directory (default: ./results)

## Key Metrics

1. **Untyped/Typed ratio** - Should be ~1.0 after optimizations
2. **Time per Assert.Equal** - Target: < 1ms per call
3. **Cache hit rate** - Higher is better for repetitive patterns

## Patterns Reference

See `PERFORMANCE_ASSISTANT.md` in the repo root for detailed patterns:
- Early Candidate Filtering (Arity Pre-Filter)
- Quick Type Compatibility Check
- Lazy Expensive Computations
- Overload Resolution Caching

## Related

- Issue: https://github.com/dotnet/fsharp/issues/18807
- Docs: `METHOD_RESOLUTION_PERF_IDEAS.md`
