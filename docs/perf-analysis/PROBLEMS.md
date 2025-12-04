# Identified Problems - Issue #19132

## Summary

This document catalogs problems identified during performance analysis of large F# project builds.

Related Issue: https://github.com/dotnet/fsharp/issues/19132

## Problem 1: Super-linear Build Time Scaling

### Measured Data

| Modules | Build Time | Command |
|---------|-----------|---------|
| 100 | 6.2s | `dotnet build -c Release` |
| 500 | 13.0s | `dotnet build -c Release` |
| 1000 | 27.0s | `dotnet build -c Release` |
| 2000 | 88.0s | `dotnet build -c Release` |
| 5000 | 796.0s (13m 16s) | `dotnet build -c Release` |

### Evidence
- Compiler used: `/home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll`
- Build output confirmed successful completion for all module counts up to 5000

---

## Problem 2: High Memory Consumption

### Measured Data (5000 modules)
- Process: `fsc.dll`
- Memory: 88.8% of system RAM (~14.5 GB)
- CPU: 153%

### Evidence
Process stats captured during build:
```
runner 39804 153 88.8 275370660 14552872 pts/46 Sl+ ... /usr/share/dotnet/dotnet /home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll @/tmp/MSBuildTempIAGVdP/tmp41a211215a374f7ab85347f3eaaaa88b.rsp
```

---

## Problem 3: No Build Progress Indication

### Observed Behavior
- Build output only shows: "Determining projects to restore..." then "Restored ... (in 76 ms)"
- No further output until build completes or times out
- Users cannot distinguish between slow build and hung build

### Evidence
Build log excerpt:
```
Determining projects to restore...
  Restored /tmp/perf-testing/fsharp-10k/ConsoleApp1/ConsoleApp1.fsproj (in 76 ms).
```
(No additional output during 13+ minute compilation phase)

---

## Next Steps

1. Collect dotnet-trace profile for 5000 module build
2. Collect memory dump at 15 minute mark if build does not complete
3. Analyze trace and dump for concrete insights

## References

- Issue: https://github.com/dotnet/fsharp/issues/19132
- Test Project: https://github.com/ners/fsharp-10k
