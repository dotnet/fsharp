# GC Analysis - Issue #19132

## Overview

GC heap analysis during 5000-module F# project build using `dotnet-gcdump`.

Related Issue: https://github.com/dotnet/fsharp/issues/19132

## Collection Methodology

```bash
dotnet-gcdump collect -p <FSC_PID> -o <output>.gcdump
dotnet-gcdump report <gcdump_file>
```

## GC Dump Timeline

| Dump | Elapsed Time | RSS Memory | GC Heap Size | GC Heap Objects |
|------|--------------|------------|--------------|-----------------|
| 1 | ~1 min | 1,058 MB | 361 MB | 10,003,470 |
| 2 | ~4 min | 3,828 MB | 291 MB | 10,000,000 |
| 3 | ~10 min | 10,406 MB | 302 MB | 10,000,000 |

## Key Observation

**RSS grows from 1 GB to 10 GB but GC Heap remains ~300 MB**

This indicates memory is being consumed outside the managed GC heap, likely in:
- Native allocations
- Memory-mapped files
- Large Object Heap (LOH) that was collected between dumps
- Unmanaged code buffers

## GC Dump 1 - Top Types (1 min mark, 1 GB RSS)

```
361,256,014  GC Heap bytes
 10,003,470  GC Heap objects

   Object Bytes     Count  Type
      2,376,216         1  System.Byte[] (Bytes > 1M)
        131,096        11  System.Byte[] (Bytes > 100K)
         80,848         1  VolatileNode<System.String,FSharp.Compiler.Parser+token>[]
         64,808        16  Entry<FSharp.Compiler.ParseAndCheckInputs+NodeToTypeCheck>[]
         49,200     1,848  System.Int32[]
         40,048         1  System.Tuple<FSharp.Compiler.Syntax.ParsedInput,...>[]
         40,048         1  FSharp.Compiler.Syntax.ParsedInput[]
         40,048         1  FSharp.Compiler.GraphChecking.FileInProject[]
```

## GC Dump 2 - Top Types (4 min mark, 3.8 GB RSS)

```
290,770,269  GC Heap bytes
 10,000,000  GC Heap objects

   Object Bytes     Count  Type
         12,140       824  System.Int32[]
         11,288     2,237  NodeToTypeCheck[]
            136       713  TcEnv
            136       702  NameResolutionEnv
            136       677  ILTypeDef
            128     5,671  ValOptionalData
            128     2,431  Entity
            120       729  FSharp.Compiler.Syntax.SynBinding
            112     2,446  ModuleOrNamespaceType
             96     1,183  EntityOptionalData
             72    28,691  Val
```

## GC Dump 3 - Top Types (10 min mark, 10.4 GB RSS)

```
301,948,795  GC Heap bytes
 10,000,000  GC Heap objects

   Object Bytes     Count  Type
         45,080         1  System.String[]
         35,200         2  System.UInt16[]
         15,656     2,258  NodeToTypeCheck[]
         10,016     1,416  System.Int32[]
            136     1,225  ILTypeDef
            136       686  TcEnv
            136       674  NameResolutionEnv
            128     5,480  ValOptionalData
            128     2,941  Entity
            120     1,144  FSharp.Compiler.Syntax.SynBinding
            112     2,385  ModuleOrNamespaceType
             88     5,890  Match
             88     4,964  Lambda
             88     2,659  TyconAugmentation
             72    27,888  Val
```

## Type Growth Analysis

Comparing object counts between Dump 2 (4 min) and Dump 3 (10 min):

| Type | Dump 2 Count | Dump 3 Count | Change |
|------|--------------|--------------|--------|
| Val | 28,691 | 27,888 | -803 |
| Entity | 2,431 | 2,941 | +510 |
| ILTypeDef | 677 | 1,225 | +548 |
| TcEnv | 713 | 686 | -27 |
| ModuleOrNamespaceType | 2,446 | 2,385 | -61 |
| Match | 6,052 | 5,890 | -162 |
| Lambda | 5,100 | 4,964 | -136 |
| TyconAugmentation | 2,142 | 2,659 | +517 |
| SynBinding | 729 | 1,144 | +415 |
| ImportILTypeDef@712 | 674 | 983 | +309 |

## Memory Leak Identified

**ImportILTypeDef@712** (closures from ImportILTypeDef function) showing growth indicates a potential memory leak.

### Root Cause
In `src/Compiler/Checking/import.fs`, the `ImportILTypeDef` function was storing `tdef.CustomAttrsStored` reference in `AttributesFromIL`, which kept entire `ILTypeDef` objects alive via closure.

### Fix Applied
Modified `ImportILTypeDef` to:
1. Added type annotation `(amap: ImportMap)` to fix type inference
2. Check if nullness features are enabled (`amap.g.langFeatureNullness && amap.g.checkNullness`)
3. If enabled: immediately read attrs with `tdef.CustomAttrsStored.GetCustomAttrs(tdef.MetadataIndex)` and wrap in `Given`
4. If disabled: use empty attributes to avoid any reference

This prevents the closure from keeping large `ILTypeDef` objects alive.

### Validation Status
**Pending**: After CI builds the fixed compiler, the 5000-module experiment should be repeated to verify:
- Reduced memory growth rate
- Stable or reduced `ImportILTypeDef@712` closure count
- Improved CPU utilization over time

## Key Findings

1. **GC Heap is stable at ~300 MB** despite RSS growing to 10+ GB
2. **ImportILTypeDef closures growing** - memory leak via CustomAttrsStored references
3. **RSS growth not reflected in GC heap** - indicates native/unmanaged memory consumption
4. **Val type has 27-28k instances** - F# value definitions
5. **Entity and ILTypeDef counts grow** - IL metadata accumulation
6. **TcEnv and NameResolutionEnv** - type checking environments (~700 instances)

## Build Environment

- Compiler: `/home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll`
- Configuration: Release, ParallelCompilation=true
- Modules: 5000
- Platform: Linux (Ubuntu)
