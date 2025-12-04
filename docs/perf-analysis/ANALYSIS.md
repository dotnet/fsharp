# Performance Analysis - Issue #19132

## Problem Statement

Building a synthetic F# project with 5,000+ modules takes excessive time and memory.

Related Issue: https://github.com/dotnet/fsharp/issues/19132

## Test Environment

- **F# Compiler**: Built from main branch
- **Compiler Path**: `/home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll`
- **FSharp.Core**: `/home/runner/work/fsharp/fsharp/artifacts/bin/FSharp.Core/Release/netstandard2.0/FSharp.Core.dll`
- **.NET SDK**: 10.0.100-rc.2
- **Platform**: Linux (Ubuntu)

## Test Project Structure

Each module (`FooN.fs`) contains:
```fsharp
namespace TestProject

[<RequireQualifiedAccess>]
type FooN = Foo of int | Bar

[<RequireQualifiedAccess>]
module FooN =
    let foo: FooN = FooN.Bar
```

## 5000 Module Build - Detailed Analysis

### Build Result
- **Total Time**: 14 minutes 11 seconds (851.19s)
- **Configuration**: Release, ParallelCompilation=true
- **Result**: Build succeeded

### Memory Growth Over Time (Measured Every Minute)

| Elapsed Time | CPU % | Memory % | RSS (MB) |
|--------------|-------|----------|----------|
| 1 min | 380% | 6.0% | 969 |
| 2 min | 387% | 6.5% | 1,050 |
| 3 min | 336% | 14.2% | 2,287 |
| 4 min | 286% | 23.7% | 3,805 |
| 5 min | 255% | 32.1% | 5,144 |
| 6 min | 234% | 39.5% | 6,331 |
| 7 min | 218% | 46.9% | 7,513 |
| 8 min | 207% | 53.5% | 8,561 |
| 9 min | 197% | 60.4% | 9,664 |
| 10 min | 189% | 67.1% | 10,746 |
| 11 min | 183% | 79.6% | 12,748 |
| 12 min | 175% | 90.4% | 14,473 |
| 13 min | 165% | 90.6% | 14,498 |
| 14 min | - | - | Build completed |

### Key Observations From Measured Data

1. **Memory growth is linear**: ~1.1 GB per minute for first 12 minutes
2. **Peak memory**: 14.5 GB (90.6% of system RAM)
3. **CPU utilization decreases over time**: 380% → 165%
4. **Memory plateaus at ~90%**: Possible GC pressure at 12-13 min mark

### Build Log Evidence
```
Determining projects to restore...
  Restored /tmp/perf-testing/fsharp-5000/src/TestProject.fsproj (in 78 ms).
  TestProject -> /tmp/perf-testing/fsharp-5000/src/bin/Release/net8.0/TestProject.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:14:11.19
```

### Compiler Process Evidence
```
runner 35321 ... /usr/share/dotnet/dotnet /home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll @/tmp/MSBuildTempQZbd6p/tmp24fcc0624ca6474f8fc7ddd8ab0874ef.rsp
```

## Trace Collection

A 2-minute trace was collected during the build using:
```bash
dotnet-trace collect --process-id <FSC_PID> --format speedscope --output fsc-trace --duration 00:02:00
```

Trace file: `fsc-trace` (44,537 bytes)
Converted to speedscope format: `fsc-trace.speedscope.speedscope.json` (92,797 bytes)

Note: Trace shows high proportion of unmanaged code time, indicating native code execution or JIT compilation overhead.

## Experiment Reproduction Steps

### Using Compiler Timing Output

The F# compiler supports detailed timing output via project properties:

1. **Add to project file** (or pass via command line):
```xml
<PropertyGroup>
  <OtherFlags>--times --times:compilationTiming.csv</OtherFlags>
</PropertyGroup>
```

Or via command line:
```bash
dotnet build -c Release -p:OtherFlags="--times --times:compilationTiming.csv"
```

2. **Collect traces and dumps**:
```bash
# Start build with tracing
dotnet-trace collect --output build-trace.nettrace -- dotnet build -c Release -p:OtherFlags="--times --times:compilationTiming.csv"

# Or attach to running fsc.dll process
FSC_PID=$(pgrep -f fsc.dll)
dotnet-trace collect -p $FSC_PID --output fsc-trace.nettrace --duration 00:05:00 &
dotnet-gcdump collect -p $FSC_PID --output heap.gcdump

# For memory dump at specific time (e.g., 15 min mark)
sleep 900 && dotnet-dump collect -p $FSC_PID --output memory.dmp
```

3. **Analyze results**:
```bash
# Convert trace to speedscope format
dotnet-trace convert fsc-trace.nettrace --format speedscope

# Report on GC dump
dotnet-gcdump report heap.gcdump

# Analyze timing CSV
python3 -c "
import csv
with open('compilationTiming.csv') as f:
    reader = csv.DictReader(f)
    for row in sorted(reader, key=lambda x: float(x.get('Duration', 0)), reverse=True)[:20]:
        print(row)
"
```

### Validation After Memory Leak Fix

After CI builds the fixed compiler, repeat the 5000-module experiment with:

1. Same build configuration (Release, ParallelCompilation=true)
2. Same measurement methodology (memory/CPU every minute)
3. Enable `--times` output for detailed phase breakdown
4. Collect GC dumps at 1, 5, and 10 minute marks
5. Compare `ImportILTypeDef@*` closure counts with previous run
