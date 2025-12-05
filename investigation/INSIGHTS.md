# F# Large Project Build Performance Investigation

## Issue Summary
Building a project with 10,000 F# modules is indeterminately slow due to super-linear (O(n²)) scaling behavior in the compiler.

## Key Findings

### Timing Measurements
| File Count | Build Time | Ratio vs 1000 |
|------------|-----------|---------------|
| 1000       | 22s       | 1x            |
| 2000       | 68s       | 3.1x          |
| 3000       | 161s      | 7.3x          |

This clearly demonstrates O(n²) behavior (if linear, ratios would be 2x and 3x).

### Per-File Type Check Duration (from timing.csv with 1000 files)
| File # | Type Check Time |
|--------|-----------------|
| 50     | 0.0083s         |
| 100    | 0.0067s         |
| 500    | 0.0087s         |
| 1000   | 0.0181s         |

Later files take ~2-3x longer to type-check than earlier files, demonstrating O(n) per-file work.

### Phase Breakdown (1000 files, 18.9s total)
- **Typecheck: 12.81s (68%)** - Main bottleneck
- TAST -> IL: 1.88s
- Write .NET Binary: 1.71s
- Optimizations: 1.35s
- Parse inputs: 0.32s

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

### Attempted Optimization
Added a fast path in `CombineModuleOrNamespaceTypes`:
- When no entity name conflicts exist, use `QueueList.append` instead of rebuilding
- This helps for deeper nesting but not for the top-level namespace conflict

### Required Fix (Future Work)
A proper fix would require:
1. Restructuring the CCU accumulator to support O(1) entity appends
2. Using incremental updates instead of full merges
3. Potentially caching the `AllEntitiesByLogicalMangledName` map across merges
4. Or using a different data structure that supports efficient union operations

## Reproduction
Test project: https://github.com/ners/fsharp-10k
- Each file declares a type `FooN` that depends on `Foo(N-1)`
- Creates 10,001 source files (including Program.fs)
- All in same namespace `ConsoleApp1`
