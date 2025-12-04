# Identified Problems - Issue #19132

## Summary

This document catalogs problems identified during performance analysis of large F# project builds.

Related Issue: https://github.com/dotnet/fsharp/issues/19132

## Problem 1: Linear Memory Growth During Compilation

### Measured Data (5000 modules, Release, ParallelCompilation=true)

| Elapsed Time | Memory (MB) | Memory Growth Rate |
|--------------|-------------|-------------------|
| 1 min | 969 | baseline |
| 2 min | 1,050 | +81 MB/min |
| 3 min | 2,287 | +1,237 MB/min |
| 4 min | 3,805 | +1,518 MB/min |
| 5 min | 5,144 | +1,339 MB/min |
| 6 min | 6,331 | +1,187 MB/min |
| 7 min | 7,513 | +1,182 MB/min |
| 8 min | 8,561 | +1,048 MB/min |
| 9 min | 9,664 | +1,103 MB/min |
| 10 min | 10,746 | +1,082 MB/min |
| 11 min | 12,748 | +2,002 MB/min |
| 12 min | 14,473 | +1,725 MB/min |
| 13 min | 14,498 | +25 MB/min (plateau) |

### Evidence
- Average memory growth: ~1.1 GB per minute
- Peak memory: 14.5 GB (90.6% of 16 GB system)
- Memory plateaus at ~90% system RAM
- Process command: `/usr/share/dotnet/dotnet /home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll`

---

## Problem 2: Decreasing CPU Utilization Over Time

### Measured Data

| Elapsed Time | CPU % |
|--------------|-------|
| 1 min | 380% |
| 2 min | 387% |
| 3 min | 336% |
| 4 min | 286% |
| 5 min | 255% |
| 6 min | 234% |
| 7 min | 218% |
| 8 min | 207% |
| 9 min | 197% |
| 10 min | 189% |
| 11 min | 183% |
| 12 min | 175% |
| 13 min | 165% |

### Evidence
- CPU utilization drops from 380% to 165% over 13 minutes
- This suggests reduced parallelism as compilation progresses
- Possible single-threaded bottleneck in later compilation phases

---

## Problem 3: No Build Progress Indication

### Observed Behavior
Build output during 14-minute compilation:
```
Determining projects to restore...
  Restored /tmp/perf-testing/fsharp-5000/src/TestProject.fsproj (in 78 ms).
```
(No additional output until build completes)

### Evidence
- No per-file progress reporting
- No phase transition messages
- Users cannot determine if build is progressing or stuck

---

## Trace Analysis

### Trace Collection
- Tool: `dotnet-trace collect --process-id <PID> --duration 00:02:00`
- Trace file size: 44,537 bytes
- Converted speedscope file: 92,797 bytes

### Trace Content
The trace shows:
- 28 active threads during capture
- High proportion of "UNMANAGED_CODE_TIME" 
- Stack frames show "?!?" (unresolved symbols)

Note: The trace symbols were not fully resolved, limiting detailed function-level analysis.

## References

- Issue: https://github.com/dotnet/fsharp/issues/19132
- Test Project: Synthetic 5000-module F# project
