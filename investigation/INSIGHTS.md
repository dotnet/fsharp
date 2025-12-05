# F# Large Project Build Performance Investigation

## Issue Summary
Building a project with 10,000 F# modules is indeterminately slow due to super-linear (O(n²)) scaling behavior in the compiler.

## Key Findings

### Timing Measurements
| File Count | Build Time | Ratio vs 1000 |
|------------|-----------|---------------|
| 1000       | 20s       | 1x            |
| 2000       | 63s       | 3.15x         |
| 3000       | 154s      | 7.7x          |

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

## Hypothesis

### H1: TcEnv/NameResolutionEnv Growth (MOST LIKELY)
The `TcEnv` and `NameResolutionEnv` structures use layered maps (`LayeredMap`, `LayeredMultiMap`) that grow with each file. Each file lookup/operation on these maps becomes slower as more entries accumulate.

Key data structures:
- `eUnqualifiedItems: LayeredMap<string, Item>` - grows with each type/value
- `eTyconsByAccessNames: LayeredMultiMap<string, TyconRef>` - grows with each type
- `eModulesAndNamespaces: NameMultiMap<ModuleOrNamespaceRef>` - grows with each module

### H2: CombineCcuContentFragments Quadratic Behavior
The `CombineCcuContentFragments` function called for each file combines the signature with accumulated CCU content. This has O(n) complexity per call, leading to O(n²) total.

### H3: Memory Pressure / GC
With 10,000 modules, memory usage reaches 15GB (per T-Gro's comment). This causes GC pressure, but is likely a symptom rather than root cause.

## Reproduction
Test project: https://github.com/ners/fsharp-10k
- Each file declares a type `FooN` that depends on `Foo(N-1)`
- Creates 10,001 source files (including Program.fs)

## Next Steps
1. Profile AddLocalRootModuleOrNamespace and AddTyconRefsToNameEnv
2. Investigate LayeredMap/LayeredMultiMap implementation efficiency
3. Consider whether batching or caching strategies could help
4. Analyze memory allocation patterns in CombineCcuContentFragments
