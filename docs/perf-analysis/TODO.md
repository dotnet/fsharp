# Performance Analysis TODO - Issue #19132

## Overview
This document tracks the performance profiling work for building large F# projects, specifically the fsharp-10k synthetic project with 10,000 modules.

Related Issue: https://github.com/dotnet/fsharp/issues/19132

## Test Matrix Status

### Configuration 1: ParallelCompilation=true, Release
- [x] Prepare fsharp-10k with local compiler
- [ ] Run traced build
- [ ] Collect/analyze trace
- [ ] Document findings

### Configuration 2: ParallelCompilation=false, Release
- [ ] Prepare fsharp-10k with local compiler
- [ ] Run traced build
- [ ] Collect/analyze trace
- [ ] Document findings

### Configuration 3: ParallelCompilation=true, Debug
- [ ] Build F# compiler in Debug mode
- [ ] Prepare fsharp-10k with local compiler
- [ ] Run traced build
- [ ] Collect/analyze trace
- [ ] Document findings

### Configuration 4: ParallelCompilation=false, Debug
- [ ] Prepare fsharp-10k with local compiler
- [ ] Run traced build
- [ ] Collect/analyze trace
- [ ] Document findings

## Preliminary Scaling Analysis (Completed)

Build time measurements using local F# compiler (Release, main branch):

| Modules | Build Time | Scaling Factor |
|---------|-----------|----------------|
| 100 | ~6s | baseline |
| 500 | ~13s | 2.2x (5x modules) |
| 1000 | ~27s | 4.5x (10x modules) |
| 2000 | ~88s | 14.7x (20x modules) |
| 5000 | ~796s (~13 min) | 132.7x (50x modules) |

**Observation**: Build time appears to scale super-linearly, possibly O(n²) or worse.
Memory usage for 5000-module build: ~14.5 GB

## Environment
- F# Compiler: Built from main branch (Release configuration)
- .NET SDK: 10.0.100-rc.2
- Test Project: Synthetic project with N modules, each containing a simple DU type and module
- Local Compiler Path: /home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll

## Next Steps
1. Attempt full 10,000 module build
2. Collect performance traces with dotnet-trace
3. Analyze hot paths and bottlenecks
4. Document findings in ANALYSIS.md, HOT_PATHS.md, and PROBLEMS.md
