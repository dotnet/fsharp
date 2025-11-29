# F# Compiler Performance Profiling Suite

This directory contains automated tools for profiling and analyzing F# compiler performance, specifically focused on the xUnit `Assert.Equal` compilation issue ([#18807](https://github.com/dotnet/fsharp/issues/18807)).

## Overview

The F# compiler exhibits slow compilation times when processing untyped `Assert.Equal` calls from xUnit, with each call adding ~100ms to compilation time. This is due to expensive overload resolution in the type checker.

This profiling suite helps:
- Generate reproducible test cases with 1500+ `Assert.Equal` calls
- Profile F# compilation (not test execution) to measure the impact
- Compare untyped (slow) vs typed (fast) versions
- Analyze traces to identify hot paths in the compiler
- Generate comprehensive performance reports

## Quick Start

### Linux / macOS

```bash
cd tools/perf-repro
chmod +x RunPerfAnalysis.sh
./RunPerfAnalysis.sh
```

### Windows

```powershell
cd tools\perf-repro
.\RunPerfAnalysis.ps1
```

This will:
1. Generate test projects (untyped and typed versions)
2. Profile compilation of both versions
3. Analyze results
4. Generate a comprehensive report at `./results/PERF_REPORT.md`

## Components

### 1. `GenerateXUnitPerfTest.fsx`

Generates F# test projects with configurable numbers of `Assert.Equal` calls.

**Usage:**
```bash
# Generate untyped version (slow path) with 1500 asserts
dotnet fsi GenerateXUnitPerfTest.fsx --total 1500 --untyped

# Generate typed version (fast path) with 1500 asserts
dotnet fsi GenerateXUnitPerfTest.fsx --total 1500 --typed
```

**Options:**
- `--total <n>`: Total number of Assert.Equal calls (default: 1500)
- `--methods <n>`: Number of test methods (default: 10)
- `--output <path>`: Output directory (default: ./generated)
- `--typed`: Generate typed Assert.Equal calls (fast path)
- `--untyped`: Generate untyped Assert.Equal calls (slow path, default)

### 2. `ProfileCompilation.fsx`

Automates the profiling workflow: generates projects, restores dependencies, and profiles compilation.

**Usage:**
```bash
dotnet fsi ProfileCompilation.fsx --total 1500 --methods 10
```

**Options:**
- `--total <n>`: Total number of Assert.Equal calls (default: 1500)
- `--methods <n>`: Number of test methods (default: 10)
- `--generated <path>`: Directory for generated projects (default: ./generated)
- `--output <path>`: Output directory for results (default: ./results)

**Features:**
- Automatically installs `dotnet-trace` if available
- Generates both untyped and typed versions
- Restores dependencies upfront
- Profiles compilation (not test execution)
- Captures timing data and optional traces
- Calculates slowdown factor

### 3. `AnalyzeTrace.fsx`

Analyzes profiling results and generates a comprehensive markdown report.

**Usage:**
```bash
dotnet fsi AnalyzeTrace.fsx --results ./results
```

**Options:**
- `--results <path>`: Results directory (default: ./results)
- `--output <path>`: Output path for report (default: ./results/PERF_REPORT.md)

**Features:**
- Parses timing data
- Analyzes trace files (if available)
- Identifies performance bottlenecks
- Suggests optimization opportunities
- Generates actionable recommendations

### 4. Orchestration Scripts

#### `RunPerfAnalysis.sh` (Linux/macOS)

```bash
./RunPerfAnalysis.sh [options]
```

#### `RunPerfAnalysis.ps1` (Windows)

```powershell
.\RunPerfAnalysis.ps1 [options]
```

**Common Options:**
- `--total <n>` / `-Total <n>`: Total Assert.Equal calls
- `--methods <n>` / `-Methods <n>`: Number of test methods
- `--generated <path>` / `-Generated <path>`: Generated projects directory
- `--results <path>` / `-Results <path>`: Results directory

## Prerequisites

### Required
- .NET SDK 8.0 or later
- F# Interactive (included with .NET SDK)

### Optional (for detailed profiling)
- `dotnet-trace` for trace collection:
  ```bash
  dotnet tool install -g dotnet-trace
  ```

**Note:** The suite will work without `dotnet-trace` by falling back to timing-only mode. When trace collection is enabled, expect significant slowdown (3-10x) during profiling - this is normal and necessary to capture detailed execution data.

## Understanding the Results

### Performance Report

The generated `PERF_REPORT.md` includes:

1. **Test Configuration**: Number of asserts, methods, and type variants
2. **Compilation Times**: Detailed timing for untyped and typed versions
3. **Performance Difference**: Slowdown factor and time difference
4. **Hot Path Analysis**: Trace analysis identifying compiler bottlenecks
5. **Key Findings**: Summary of performance issues
6. **Optimization Opportunities**: Specific recommendations with impact estimates
7. **Recommendations**: Actionable advice for users and compiler developers

### Expected Results

Typical results for 1500 `Assert.Equal` calls:

| Version | Compilation Time | Time per Assert | Notes |
|---------|-----------------|-----------------|-------|
| Untyped (slow) | ~150s | ~100ms | Each call triggers expensive overload resolution |
| Typed (fast) | ~5s | ~3ms | Type annotations bypass overload resolution |
| **Slowdown** | **~30x** | - | Dramatic performance difference |

## Customizing the Analysis

### Different Test Sizes

```bash
# Small test (500 asserts)
./RunPerfAnalysis.sh --total 500 --methods 5

# Medium test (1500 asserts, default)
./RunPerfAnalysis.sh --total 1500 --methods 10

# Large test (3000 asserts)
./RunPerfAnalysis.sh --total 3000 --methods 20
```

### Manual Steps

You can run each component individually:

```bash
# 1. Generate projects
dotnet fsi GenerateXUnitPerfTest.fsx --total 1500 --untyped
dotnet fsi GenerateXUnitPerfTest.fsx --total 1500 --typed

# 2. Profile compilation
dotnet fsi ProfileCompilation.fsx --total 1500

# 3. Generate report
dotnet fsi AnalyzeTrace.fsx --results ./results
```

## Troubleshooting

### "dotnet-trace not found"

The suite will work in timing-only mode. For detailed trace analysis, install:
```bash
dotnet tool install -g dotnet-trace
```

**Note**: Trace collection adds significant overhead (3-10x slowdown) to build times. This is normal and expected when profiling.

### Permission Errors on Linux/macOS

Make scripts executable:
```bash
chmod +x RunPerfAnalysis.sh
```

### Build Failures

Ensure you have:
- .NET SDK 8.0 or later installed
- Internet connection for NuGet package restore
- Write permissions in the output directories

## Output Files

After running the analysis, you'll find:

```
results/
├── PERF_REPORT.md                    # Comprehensive analysis report
├── summary.txt                        # Quick summary
├── XUnitPerfTest.Untyped.timing.txt   # Untyped version timing
├── XUnitPerfTest.Typed.timing.txt     # Typed version timing
├── XUnitPerfTest.Untyped.nettrace     # Untyped trace (if dotnet-trace available)
└── XUnitPerfTest.Typed.nettrace       # Typed trace (if dotnet-trace available)

generated/
├── XUnitPerfTest.Untyped/    # Generated untyped test project
│   ├── Tests.fs
│   ├── XUnitPerfTest.Untyped.fsproj
│   └── README.md
└── XUnitPerfTest.Typed/      # Generated typed test project
    ├── Tests.fs
    ├── XUnitPerfTest.Typed.fsproj
    └── README.md
```

## Contributing to Issue #18807

Use this suite to:
1. **Reproduce the issue** with consistent test cases
2. **Benchmark optimizations** before and after code changes
3. **Profile specific scenarios** by customizing the generator
4. **Share results** in standardized format

### Benchmarking Compiler Changes

```bash
# Before optimization
./RunPerfAnalysis.sh
mv results results-before

# After optimization (rebuild compiler, then)
./RunPerfAnalysis.sh
mv results results-after

# Compare reports
diff results-before/PERF_REPORT.md results-after/PERF_REPORT.md
```

## Technical Details

### What Gets Profiled

The suite profiles **F# compilation only**, not test execution:
- Type checking
- Overload resolution
- Constraint solving
- IL generation

### Why This Matters

The issue affects real-world codebases with many xUnit tests. A project with 500 untyped `Assert.Equal` calls can see:
- ~50 seconds of extra compilation time
- Slower IDE responsiveness
- Reduced developer productivity

### Known Compiler Hotspots

Based on issue analysis, the likely bottlenecks are in:
- `src/Compiler/Checking/ConstraintSolver.fs` (line ~3486)
- `src/Compiler/Checking/MethodCalls.fs`
- `src/Compiler/Checking/TypeChecker.fs`

## License

This profiling suite is part of the F# compiler repository and follows the same license.

## References

- Issue: [#18807 - F# compiler slow with xUnit Assert.Equal](https://github.com/dotnet/fsharp/issues/18807)
- F# Compiler Docs: [/docs](../../docs)
- Performance Discussions: [/docs/perf-discussions-archive.md](../../docs/perf-discussions-archive.md)
