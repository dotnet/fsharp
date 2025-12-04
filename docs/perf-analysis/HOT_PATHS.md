# Hot Paths Analysis - Issue #19132

## Overview

This document contains hot path analysis from performance traces collected during large F# project builds.

Related Issue: https://github.com/dotnet/fsharp/issues/19132

## Trace Collection

### Method
```bash
dotnet-trace collect --process-id <FSC_PID> --format speedscope --output fsc-trace --duration 00:02:00
```

### Collected Trace
- **Process**: fsc.dll (PID 35798)
- **Duration**: 2 minutes (captured during 5000-module build)
- **Trace file**: `fsc-trace` (44,537 bytes)
- **Speedscope file**: `fsc-trace.speedscope.speedscope.json` (92,797 bytes)

## Trace Analysis Results

### Thread Activity
The trace captured 28 active threads:
- Thread 35798 (main)
- Threads 35810-35838 (worker threads)

### Time Distribution
From speedscope conversion:
- Main thread (35798): 17.88ms - 267.12ms captured
- High proportion of `UNMANAGED_CODE_TIME` frames
- Many `?!?` (unresolved) stack frames

### Observations
1. **Unresolved symbols**: Many stack frames show as "?!?" indicating native code or missing debug symbols
2. **Multi-threaded**: 28 threads active confirms ParallelCompilation is engaged
3. **Unmanaged code**: Significant time in unmanaged code (possibly JIT, GC, or native runtime)

### Limitations
- Trace symbols not fully resolved
- 2-minute sample may not capture all phases
- Native/unmanaged code not fully visible

## Raw Trace Data

### Speedscope Frame Names (from trace)
```
"Process64 Process(35798) (35798) Args: "
"(Non-Activities)"
"Threads"
"Thread (35798)"
"?!?"
"UNMANAGED_CODE_TIME"
"Thread (35810)" ... "Thread (35838)"
"CPU_TIME"
```

## Next Steps for Deeper Analysis
1. Build with debug symbols for better stack resolution
2. Use longer trace duration to capture full build
3. Consider using PerfView or dotnet-gcdump for memory analysis
