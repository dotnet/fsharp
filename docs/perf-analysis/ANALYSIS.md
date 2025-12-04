# Performance Analysis - Issue #19132

## Executive Summary

This document contains analysis of performance issues when building large F# projects with many modules (10,000+), as reported in [Issue #19132](https://github.com/dotnet/fsharp/issues/19132).

## Problem Statement

Building a synthetic F# project with 10,000 modules (`fsharp-10k`) takes an indeterminate/excessive amount of time and consumes significant memory. Users report the build never completing.

## Test Environment

- **F# Compiler**: Built from main branch
- **Compiler Path**: `/home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll`
- **FSharp.Core**: `/home/runner/work/fsharp/fsharp/artifacts/bin/FSharp.Core/Release/netstandard2.0/FSharp.Core.dll`
- **.NET SDK**: 10.0.100-rc.2
- **Platform**: Linux (Ubuntu)

## Test Project Structure

Each module (`FooN.fs`) contains:
```fsharp
namespace ConsoleApp1

[<RequireQualifiedAccess>]
type FooN = Foo of int | Bar

[<RequireQualifiedAccess>]
module FooN =
    let foo: FooN = FooN.Bar
```

## Scaling Analysis Results

### Build Time vs Module Count (Release Configuration)

| Modules | Build Time | Time/Module | Scaling Factor | Memory Usage |
|---------|-----------|-------------|----------------|--------------|
| 100 | 6.2s | 62ms | 1.0x (baseline) | Low |
| 500 | 13.0s | 26ms | 2.1x | Low |
| 1000 | 27.0s | 27ms | 4.4x | Low |
| 2000 | 88.0s | 44ms | 14.2x | Medium |
| 5000 | 796.0s | 159ms | 128.4x | ~14.5 GB |

### Observations

1. **Super-linear Scaling**: Build time does not scale linearly with module count
   - Expected (linear): 10x modules → 10x time
   - Actual: 50x modules → 128x time

2. **Memory Consumption**: Memory usage grows significantly with module count
   - 5000 modules consumed ~14.5 GB RAM
   - 10000 modules likely requires 30+ GB

3. **Build Phase Breakdown**:
   - Restore: Fast (~75-90ms regardless of module count)
   - Compile: Majority of time spent here
   - The compiler appears to be the bottleneck

## Key Findings

### 1. Non-linear Time Complexity
The compilation time suggests an algorithmic complexity worse than O(n):
- O(n log n) would give ~5.6x at 50x modules
- O(n²) would give ~2500x at 50x modules
- Observed ~128x suggests O(n^1.5) to O(n^1.7) complexity

### 2. Memory Pressure
High memory consumption suggests:
- Large intermediate data structures
- Possible lack of streaming/incremental processing
- All type information may be kept in memory

### 3. Single-threaded Bottleneck
Even with ParallelCompilation=true, certain phases may be single-threaded:
- Type checking
- Symbol resolution across modules
- Global optimization passes

## Recommendations for Investigation

1. **Profile Type Checking Phase**: Likely candidate for O(n²) behavior in symbol lookup
2. **Analyze Memory Allocation**: Use memory profiler to identify large allocations
3. **Check Graph Algorithms**: Module dependency resolution may have inefficient implementations
4. **Review Symbol Table**: Hash table sizing and collision handling

## Evidence

### Compiler Invocation (5000 modules)
```
/usr/share/dotnet/dotnet /home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll @/tmp/MSBuildTempIAGVdP/tmp41a211215a374f7ab85347f3eaaaa88b.rsp
```

### Process Stats During Build
- CPU: 153% (using multiple cores where possible)
- Memory: 88.8% of system RAM (~14.5 GB)
- Time: 13 minutes 16 seconds for 5000 modules

## Next Steps

1. Run full 10,000 module test with timeout
2. Collect dotnet-trace profile for detailed analysis
3. Collect memory dump if build hangs
4. Analyze trace for hot paths
