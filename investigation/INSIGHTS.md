# F# Large Project Build Performance Investigation

## Issue Summary
Building a project with 10,000 F# modules is indeterminately slow due to super-linear (O(n²)) scaling behavior in the compiler.

## Key Findings

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
