# PERFORMANCE_ASSISTANT Skill

This skill provides tools and patterns for performance analysis of the F# compiler.

## Quick Reference

### Install Profiling Tools

```powershell
# Install dotnet-trace for CPU profiling
dotnet tool install -g dotnet-trace

# Install dotnet-dump for memory analysis  
dotnet tool install -g dotnet-dump

# Install dotnet-counters for live monitoring
dotnet tool install -g dotnet-counters

# Verify installations
dotnet-trace --version
dotnet-dump --version
dotnet-counters --version
```

### Collect CPU Trace During Compilation

```powershell
# Method 1: Trace a dotnet build command
dotnet-trace collect --providers "Microsoft-Windows-DotNETRuntime" -- dotnet build MyProject.fsproj

# Method 2: Attach to running process
$pid = (Get-Process -Name "dotnet" | Where-Object { $_.CommandLine -match "fsc" }).Id
dotnet-trace collect -p $pid --duration 00:00:30

# Method 3: With specific providers for F# compiler
dotnet-trace collect `
  --providers "Microsoft-Windows-DotNETRuntime:0x1F000080018:5" `
  -- dotnet build MyProject.fsproj -c Release
```

### Analyze Trace Files

```powershell
# Convert to speedscope format for web viewer
dotnet-trace convert trace.nettrace --format Speedscope

# View in browser
# Open https://www.speedscope.app and load the .speedscope.json file

# Get text report (basic)
dotnet-trace report trace.nettrace --output text

# Get top methods by CPU time
dotnet-trace report trace.nettrace --output top-methods --limit 50
```

### Memory Analysis

```powershell
# Collect heap dump
dotnet-dump collect -p <PID>

# Analyze dump
dotnet-dump analyze dump.dmp

# Common commands in analyzer:
# > dumpheap -stat           # Object statistics
# > dumpheap -type String    # Find specific types
# > gcroot <address>         # Find root of object
# > dumpobj <address>        # Dump object details
```

### Live Monitoring

```powershell
# Monitor GC and allocations
dotnet-counters monitor -p <PID> --counters System.Runtime

# Monitor with refresh rate
dotnet-counters collect -p <PID> --format json --output counters.json
```

---

## Using the perf-repro Test Suite

The F# repo includes a performance reproduction suite in `tools/perf-repro/`:

### Generate Test Projects

```powershell
cd tools/perf-repro

# Generate untyped version (slow - triggers issue)
dotnet fsi GenerateXUnitPerfTest.fsx --total 1500 --methods 10 --output ./generated --untyped

# Generate typed version (fast - baseline)
dotnet fsi GenerateXUnitPerfTest.fsx --total 1500 --methods 10 --output ./generated --typed
```

### Run Full Profiling Workflow

```powershell
# PowerShell (Windows)
.\RunPerfAnalysis.ps1 -Total 1500 -Methods 10

# Bash (Linux/Mac)
./RunPerfAnalysis.sh --total 1500 --methods 10
```

### Analyze Results

```powershell
# Generate performance report
dotnet fsi AnalyzeTrace.fsx --results ./results

# Output: results/PERF_REPORT.md
```

---

## Key Compiler Hot Paths

When profiling F# compiler performance for overload resolution, focus on these methods:

| Method | File | Description |
|--------|------|-------------|
| `ResolveOverloading` | `ConstraintSolver.fs:3438` | Main overload resolution entry |
| `FilterEachThenUndo` | `ConstraintSolver.fs:497` | Tries each candidate with trace |
| `CanMemberSigsMatchUpToCheck` | `ConstraintSolver.fs` | Full signature checking |
| `CalledMeth` constructor | `MethodCalls.fs:534` | Builds candidate representation |
| `MakeCalledArgs` | `MethodCalls.fs:498` | Creates argument descriptors |
| `TypesEquiv` | `ConstraintSolver.fs` | Type equivalence checking |
| `TypesMustSubsume` | `ConstraintSolver.fs` | Subtype checking |

---

## Benchmarking Script Template

Use this F# script template for quick benchmarking:

```fsharp
#!/usr/bin/env dotnet fsi

open System
open System.Diagnostics
open System.IO

let time name f =
    let sw = Stopwatch.StartNew()
    let result = f()
    sw.Stop()
    printfn "%s: %.2f ms" name sw.Elapsed.TotalMilliseconds
    result

let runBuild projectPath =
    let psi = ProcessStartInfo("dotnet", $"build \"{projectPath}\" -c Release --no-restore")
    psi.RedirectStandardOutput <- true
    psi.RedirectStandardError <- true
    psi.UseShellExecute <- false
    use p = Process.Start(psi)
    p.WaitForExit()
    p.ExitCode

// Example usage:
// time "Untyped build" (fun () -> runBuild "./generated/XUnitPerfTest.Untyped/XUnitPerfTest.Untyped.fsproj")
// time "Typed build" (fun () -> runBuild "./generated/XUnitPerfTest.Typed/XUnitPerfTest.Typed.fsproj")
```

---

## Environment Variables for Debugging

```powershell
# Enable F# compiler timing output
$env:FCS_TIMING = "1"

# Enable detailed type checking diagnostics
$env:FSharpAllowUnionTypeAnnotations = "1"

# Force rebuild
$env:MSBuildCacheEnabled = "0"
```

---

## Trace Analysis with Python

If you need more complex trace analysis, use this Python script:

```python
#!/usr/bin/env python3
"""Analyze F# compiler performance traces."""

import json
import sys
from collections import defaultdict

def analyze_speedscope(path):
    """Parse speedscope JSON and find hot methods."""
    with open(path) as f:
        data = json.load(f)
    
    # Count time per frame
    times = defaultdict(float)
    for profile in data.get('profiles', []):
        if profile['type'] == 'sampled':
            samples = profile.get('samples', [])
            weights = profile.get('weights', [1] * len(samples))
            for sample, weight in zip(samples, weights):
                for frame_idx in sample:
                    frame = data['shared']['frames'][frame_idx]
                    times[frame['name']] += weight
    
    # Sort and print top methods
    sorted_times = sorted(times.items(), key=lambda x: -x[1])
    print("Top 20 methods by time:")
    for name, time in sorted_times[:20]:
        print(f"  {time:8.0f}  {name}")

if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("Usage: analyze_trace.py <speedscope.json>")
        sys.exit(1)
    analyze_speedscope(sys.argv[1])
```

---

## Common Performance Issues

### 1. Excessive Overload Candidates
**Symptom**: Slow compilation of calls to methods with many overloads
**Diagnostic**: Look for high call counts to `FilterEachThenUndo`
**Location**: `ConstraintSolver.fs`

### 2. Type Inference Loops
**Symptom**: Exponential slowdown with complex nested types
**Diagnostic**: Deep call stacks in `SolveTypeAsEquiv`
**Location**: `ConstraintSolver.fs`

### 3. Allocation Pressure
**Symptom**: High GC time in traces
**Diagnostic**: Use `dotnet-dump` to find large object graphs
**Location**: Often in `Trace` or `CalledMeth` creation

### 4. String Allocations
**Symptom**: Many String objects in heap dump
**Diagnostic**: Check error message formatting in hot paths
**Location**: Various places using `sprintf` or string concatenation

---

## Tracking Performance Experiments

Use `METHOD_RESOLUTION_PERF_IDEAS.md` in the repo root to track:
- Ideas and hypotheses
- Experiment results
- Rejected approaches
- Implementation status

Format:
```markdown
### Idea Name
**Status**: 🔬 | 🧪 | ✅ | ❌ | ⏸️
**Location**: File:Line
**Hypothesis**: What you expect to improve
**Expected Impact**: High/Medium/Low
**Results**: Actual measurements (if completed)
```
