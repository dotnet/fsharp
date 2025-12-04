# Performance Analysis - Issue #19132

## Problem Statement

Building a synthetic F# project with 10,000 modules (`fsharp-10k`) takes an excessive amount of time. 

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
namespace ConsoleApp1

[<RequireQualifiedAccess>]
type FooN = Foo of int | Bar

[<RequireQualifiedAccess>]
module FooN =
    let foo: FooN = FooN.Bar
```

## Measured Build Times

| Modules | Build Time | Configuration |
|---------|-----------|---------------|
| 100 | 6.2s | Release |
| 500 | 13.0s | Release |
| 1000 | 27.0s | Release |
| 2000 | 88.0s | Release |
| 5000 | 796.0s (13m 16s) | Release |

## Evidence

### Compiler Invocation (5000 modules)
```
/usr/share/dotnet/dotnet /home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll @/tmp/MSBuildTempIAGVdP/tmp41a211215a374f7ab85347f3eaaaa88b.rsp
```

### Process Stats During 5000 Module Build
- CPU: 153%
- Memory: 88.8% of system RAM (~14.5 GB)
- Time: 13 minutes 16 seconds

## Next Steps

1. Run 5000 module build with dotnet-trace to collect performance trace
2. Collect memory dump at 15 minute mark if build exceeds that time
3. Analyze trace and dump for concrete bottleneck identification
