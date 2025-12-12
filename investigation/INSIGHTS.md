# F# Large Project Build Performance Investigation

## Issue Summary
Building a project with 10,000 F# modules is indeterminately slow due to super-linear (O(n²)) scaling behavior in the compiler.

## Key Findings

### File Count vs Module Count Experiment

To isolate whether the issue is with file count or module count, we tested the same 3000 modules organized differently:

| Experiment | Files | Modules/File | Typecheck Time | Total Time | Memory (MB) |
|------------|-------|--------------|----------------|------------|-------------|
| Exp1       | 3000  | 1            | 142.07s        | 163.15s    | 5202 MB     |
| Exp2       | 1000  | 3            | 30.59s         | 46.36s     | 2037 MB     |
| Exp3       | 3     | 1000         | 10.41s         | 28.00s     | 1421 MB     |
| Exp4       | 1     | 3000         | 18.08s         | 36.57s     | 1441 MB     |

**Key observations:**
- Same 3000 modules: 3000 files takes 142s, 1 file takes 18s = **7.9x slower with more files**
- Memory: 5202 MB vs 1441 MB = **3.6x more memory with more files**
- **The issue is clearly correlated with NUMBER OF FILES, not number of modules**
- Typecheck phase dominates in all cases

### CombineModuleOrNamespaceTypes Instrumentation

Added instrumentation to track the growth of entities processed in `CombineModuleOrNamespaceTypes`:

| Iteration | Path | mty1.entities | mty2.entities | Total Entities Processed | Elapsed (ms) |
|-----------|------|---------------|---------------|-------------------------|--------------|
| 1         | root | 0             | 1             | 1                       | 35,000       |
| 500       | root | 0             | 1             | 28,221                  | 36,400       |
| 1000      | ConsoleApp1 | 2       | 664           | 112,221                 | 37,600       |
| 2000      | root | 0             | 1             | 446,221                 | 41,200       |
| 3000      | root | 1             | 1             | 1,004,000               | 47,300       |
| 5000      | root | 0             | 1             | 2,782,221               | 69,900       |
| 7000      | ConsoleApp1 | 2       | 4,664         | 5,452,221               | 109,500      |
| 9000      | root | 1             | 1             | 8,008,000               | 155,000      |
| 12000     | ConsoleApp1 | 2       | 3,000         | 11,263,500              | 175,500      |
| 14500     | ConsoleApp1 | 2       | 5,500         | 16,582,250              | 180,500      |

**Key observations from instrumentation:**
- 14,500+ total iterations of `CombineModuleOrNamespaceTypes` for 3000 files
- Total entities processed grows quadratically: ~16.6 million entity operations for 3000 files
- The `ConsoleApp1` namespace merge handles increasingly large entity counts (up to 5,500 entities per merge)
- Each file adds 2 new entities (type + module), but the accumulated namespace grows linearly

### Timing Comparison (Stock vs Optimized Compiler)

| File Count | Stock Compiler | Optimized Compiler | Difference |
|------------|---------------|-------------------|------------|
| 1000       | 24.0s         | 26.9s             | +12%       |
| 2000       | 65.0s         | 79.5s             | +22%       |
| 3000       | 159.8s        | 187.6s            | +17%       |

**Scaling Analysis:**
| Files | Stock Ratio | Optimized Ratio | Expected (linear) |
|-------|------------|-----------------|-------------------|
| 1000  | 1x         | 1x              | 1x                |
| 2000  | 2.7x       | 2.96x           | 2x                |
| 3000  | 6.7x       | 6.98x           | 3x                |

Both compilers exhibit O(n²) scaling. The optimization adds overhead without fixing the fundamental issue.

### Phase Breakdown from --times (1000/2000/3000 files)

| Phase              | 1000 files | 2000 files | 3000 files | Growth Rate |
|--------------------|------------|------------|------------|-------------|
| **Typecheck**      | 16.75s     | 67.69s     | 171.45s    | O(n²)       |
| Optimizations      | 2.80s      | 4.96s      | 6.14s      | ~O(n)       |
| TAST -> IL         | 1.50s      | 2.25s      | 3.16s      | ~O(n)       |
| Write .NET Binary  | 0.87s      | 1.50s      | 2.35s      | ~O(n)       |
| Parse inputs       | 0.51s      | 0.61s      | 0.91s      | ~O(n)       |

**The Typecheck phase dominates and exhibits clear O(n²) growth.**

### dotnet-trace Analysis
Trace file captured at `/tmp/trace1000.nettrace` (25.8MB) and converted to speedscope format.
Key hot paths in the trace are in type checking and CCU signature combination.

## Root Cause Analysis

### Primary Bottleneck: CombineCcuContentFragments
The function `CombineCcuContentFragments` in `TypedTreeOps.fs` is called for each file to merge the file's signature into the accumulated CCU signature. 

The algorithm in `CombineModuleOrNamespaceTypes`:
1. Builds a lookup table from ALL accumulated entities - O(n)
2. Iterates ALL accumulated entities to check for conflicts - O(n)
3. Creates a new list of combined entities - O(n)

This is O(n) per file, giving O(n²) total for n files.

### Why This Affects fsharp-10k
All 10,000 files use `namespace ConsoleApp1`, so:
- At the TOP level, there's always a conflict (the `ConsoleApp1` namespace entity)
- The `CombineEntities` function recursively combines the namespace contents
- INSIDE the namespace, each file adds unique types (Foo1, Foo2, etc.) - no conflicts
- But the full iteration still happens to check for conflicts

### Attempted Optimization (Reverted)
Attempted a fast path in `CombineModuleOrNamespaceTypes`:
- When no entity name conflicts exist, use `QueueList.append` instead of rebuilding
- **Result: Made performance WORSE** (+12-22% overhead)
- The overhead from conflict detection exceeded savings from fast path
- Reverted this change as it was not beneficial

### Required Fix (Future Work)
A proper fix would require architectural changes:
1. Restructuring the CCU accumulator to support O(1) entity appends
2. Using incremental updates instead of full merges  
3. Potentially caching the `AllEntitiesByLogicalMangledName` map across merges
4. Or using a different data structure that supports efficient union operations
5. Consider lazy evaluation of entity lookups

## Reproduction
Test project: https://github.com/ners/fsharp-10k
- Each file declares a type `FooN` that depends on `Foo(N-1)`
- Creates 10,001 source files (including Program.fs)
- All in same namespace `ConsoleApp1`
