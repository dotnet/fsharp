# Performance Analysis TODO - Issue #19132

## Overview
This document tracks the performance profiling work for building large F# projects.

Related Issue: https://github.com/dotnet/fsharp/issues/19132

## Completed: 5000 Module Build Analysis

### Tasks
- [x] Prepare test project with local compiler
- [x] Run build and measure time: **14m 11s**
- [x] Monitor memory usage every minute: **Peak 14.5 GB**
- [x] Monitor CPU usage every minute: **380% → 165%**
- [x] Collect dotnet-trace profile (2 min sample)
- [x] Convert trace to speedscope format
- [x] Document findings

### Build Configuration
- Modules: 5000
- Configuration: Release
- ParallelCompilation: true
- Compiler: `/home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll`

### Results Summary

| Metric | Value |
|--------|-------|
| Build Time | 14m 11s (851.19s) |
| Peak Memory | 14.5 GB (90.6%) |
| Peak CPU | 387% |
| Final CPU | 165% |
| Memory Growth | ~1.1 GB/min |

### Memory Profile

| Time | Memory |
|------|--------|
| 1m | 969 MB |
| 5m | 5,144 MB |
| 10m | 10,746 MB |
| 13m | 14,498 MB |

### Trace Analysis
- Trace file collected: 44 KB
- 28 threads active
- High unmanaged code time
- Symbols not fully resolved

## Files Generated
- Build log: `/tmp/perf-testing/build.log`
- Trace: `/tmp/perf-testing/traces/fsc-trace`
- Speedscope: `/tmp/perf-testing/traces/fsc-trace.speedscope.speedscope.json`
