# Performance Analysis TODO - Issue #19132

## Overview
This document tracks the performance profiling work for building large F# projects.

Related Issue: https://github.com/dotnet/fsharp/issues/19132

## Focus: 5000 Module Build Analysis

### Tasks
- [x] Prepare test project with local compiler
- [x] Measure build time (completed: 13m 16s)
- [x] Measure memory usage (completed: ~14.5 GB)
- [ ] Collect dotnet-trace profile
- [ ] Collect memory dump at 15 minute mark (if needed)
- [ ] Analyze trace file
- [ ] Analyze dump file
- [ ] Document findings from trace/dump analysis

## Completed Measurements

| Modules | Build Time | Memory Usage |
|---------|-----------|--------------|
| 100 | 6.2s | Low |
| 500 | 13.0s | Low |
| 1000 | 27.0s | Low |
| 2000 | 88.0s | Medium |
| 5000 | 796.0s (13m 16s) | ~14.5 GB |

## Environment
- F# Compiler: `/home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll`
- .NET SDK: 10.0.100-rc.2
- Test Project: Synthetic project with N modules
