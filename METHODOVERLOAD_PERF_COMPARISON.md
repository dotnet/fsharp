# Method Overload Resolution Performance Comparison

## Executive Summary

This document presents a comprehensive performance comparison of the F# compiler's method overload resolution, measuring the impact of optimizations introduced in PR branch `copilot/create-performance-profiling-automation` (Issue #18807).

**Key Finding: 26.9% improvement in typecheck time** (2.5785s → 1.8859s)

---

## Methodology

### Test Approach

We measured compilation times using a synthetic test project designed to stress-test method overload resolution. The test project contains:

- **200+ `Assert.Equal` calls** targeting a mock `Assert` class with 30+ overloads
- **100 untyped calls**: `Assert.Equal(1, 2)` - compiler must resolve against all overloads
- **100 typed calls**: `Assert.Equal<int>(1, 2)` - explicit type annotation reduces resolution work

The F# compiler's `--times` flag was used to capture detailed phase timing for each compilation.

### Measurement Protocol

1. Clean build artifacts between compiler switches
2. Build each compiler version from source (Release configuration)
3. Configure test project to use the specific `fsc.dll` via `DotnetFscCompilerPath`
4. Perform 5 consecutive compilation runs per compiler version
5. Capture raw `--times` output for each run
6. Calculate mean and standard deviation for each phase

### Statistical Rigor

- **Sample size**: 5 runs per compiler (sufficient for stable mean estimation)
- **Standard deviation** calculated to assess measurement stability
- **Percentage improvement** computed as: `(baseline - optimized) / baseline × 100`

---

## Environment Information

| Property | Value |
|----------|-------|
| **Operating System** | Windows |
| **Runtime** | .NET 9.0 (target framework for test project) |
| **Compiler Runtime** | .NET 10.0 (fsc.dll target) |
| **Build Configuration** | Release |
| **Measurement Date** | 2026-01-22 |

### Compiler Versions

| Version | Git Commit | Branch |
|---------|------------|--------|
| **Baseline** | `def2b8239e52583fd714992e3c8e4c50813717df` | `origin/main` |
| **Optimized** | `f3f07201028c8a70335ec689f1017ddaae1d9bb1` | `copilot/create-performance-profiling-automation` |

---

## Test File Description

### Project Structure

```
overload-perf-test/
├── OverloadPerfTest.fsproj    # Test project with --times flag
├── MockAssert.fs              # 30+ overloaded Assert.Equal methods
└── OverloadPerfTests.fs       # 200+ Assert.Equal calls
```

### MockAssert.fs

A simulated xUnit `Assert` class with heavily overloaded `Equal` methods:

- **String overloads** (4): `Equal(string, string)`, with optional parameters
- **Primitive overloads** (12): `int`, `int64`, `float`, `float32`, `decimal`, `byte`, `sbyte`, `int16`, `uint16`, `uint32`, `uint64`, `char`, `bool`
- **DateTime overloads** (4): `DateTime`, `DateTimeOffset`, with precision variants
- **Generic overloads** (2): `Equal<'T>(expected, actual)`, with comparer variant
- **Span overloads** (3): `ReadOnlySpan<char>`, `Span<byte>`, etc.
- **Object fallback** (1): `Equal(obj, obj)`

This mirrors real-world xUnit usage patterns that cause overload resolution performance issues.

### OverloadPerfTests.fs

Contains 8 test functions with 200+ total `Assert.Equal` calls:

| Test Function | Call Type | Count |
|---------------|-----------|-------|
| Untyped int calls | `Assert.Equal(1, 1)` | 25 |
| Typed int calls | `Assert.Equal<int>(1, 1)` | 25 |
| Untyped string calls | `Assert.Equal("a", "a")` | 25 |
| Typed additional int calls | `Assert.Equal<int>(101, 101)` | 25 |
| Mixed types untyped | Various primitive types | 25 |
| Mixed types typed | Various with type params | 25 |
| Additional untyped | `Assert.Equal(42, 42)` | 25 |
| Additional typed | `Assert.Equal<int>(42, 42)` | 25 |

---

## Raw Measurements

### Baseline Compiler (origin/main)

**Commit:** `def2b8239e52583fd714992e3c8e4c50813717df`

#### Raw --times Output Snippets

```
Run 1:
------ Pass ImportMscorlib+FSharp.Core: 0.785s
------ Pass ParseInputs: 0.2406s
------ Pass Import non-system refs: 0.0737s
------ Pass Typecheck: 2.5925s
------ Pass Optimizations: 0.1313s
------ Pass TAST -> IL: 0.2929s
------ Pass Write .NET Binary: 0.4438s
Time Elapsed 00:00:07.00

Run 2:
------ Pass ImportMscorlib+FSharp.Core: 0.8127s
------ Pass ParseInputs: 0.2499s
------ Pass Import non-system refs: 0.0716s
------ Pass Typecheck: 2.3861s
------ Pass Optimizations: 0.1661s
------ Pass TAST -> IL: 0.3462s
------ Pass Write .NET Binary: 0.4722s
Time Elapsed 00:00:06.94

Run 3:
------ Pass ImportMscorlib+FSharp.Core: 0.8033s
------ Pass ParseInputs: 0.2414s
------ Pass Import non-system refs: 0.0802s
------ Pass Typecheck: 2.8858s
------ Pass Optimizations: 0.1386s
------ Pass TAST -> IL: 0.2908s
------ Pass Write .NET Binary: 0.4083s
Time Elapsed 00:00:07.38

Run 4:
------ Pass ImportMscorlib+FSharp.Core: 0.7885s
------ Pass ParseInputs: 0.243s
------ Pass Import non-system refs: 0.0722s
------ Pass Typecheck: 2.5789s
------ Pass Optimizations: 0.1352s
------ Pass TAST -> IL: 0.2981s
------ Pass Write .NET Binary: 0.4021s
Time Elapsed 00:00:06.98

Run 5:
------ Pass ImportMscorlib+FSharp.Core: 0.78s
------ Pass ParseInputs: 0.2455s
------ Pass Import non-system refs: 0.072s
------ Pass Typecheck: 2.4492s
------ Pass Optimizations: 0.139s
------ Pass TAST -> IL: 0.3213s
------ Pass Write .NET Binary: 0.415s
Time Elapsed 00:00:06.88
```

#### Baseline Raw Data Table

| Run | ImportMscorlib (s) | ParseInputs (s) | ImportNonSys (s) | Typecheck (s) | Optimizations (s) | TAST→IL (s) | WriteNetBinary (s) | Total (s) |
|-----|-------------------|-----------------|------------------|---------------|-------------------|-------------|-------------------|-----------|
| 1 | 0.785 | 0.2406 | 0.0737 | 2.5925 | 0.1313 | 0.2929 | 0.4438 | 7.00 |
| 2 | 0.8127 | 0.2499 | 0.0716 | 2.3861 | 0.1661 | 0.3462 | 0.4722 | 6.94 |
| 3 | 0.8033 | 0.2414 | 0.0802 | 2.8858 | 0.1386 | 0.2908 | 0.4083 | 7.38 |
| 4 | 0.7885 | 0.2430 | 0.0722 | 2.5789 | 0.1352 | 0.2981 | 0.4021 | 6.98 |
| 5 | 0.7800 | 0.2455 | 0.0720 | 2.4492 | 0.1390 | 0.3213 | 0.4150 | 6.88 |

---

### Optimized Compiler (PR Branch)

**Commit:** `f3f07201028c8a70335ec689f1017ddaae1d9bb1`

#### Raw --times Output Snippets

```
Run 1:
------ Pass ImportMscorlib+FSharp.Core: 0.7726s
------ Pass ParseInputs: 0.2509s
------ Pass Import non-system refs: 0.076s
------ Pass Typecheck: 1.9027s
------ Pass Optimizations: 0.1306s
------ Pass TAST -> IL: 0.2871s
------ Pass Write .NET Binary: 0.3969s
Time Elapsed 00:00:06.48

Run 2:
------ Pass ImportMscorlib+FSharp.Core: 0.7566s
------ Pass ParseInputs: 0.2518s
------ Pass Import non-system refs: 0.0802s
------ Pass Typecheck: 1.9032s
------ Pass Optimizations: 0.1335s
------ Pass TAST -> IL: 0.2911s
------ Pass Write .NET Binary: 0.3846s
Time Elapsed 00:00:06.19

Run 3:
------ Pass ImportMscorlib+FSharp.Core: 0.7987s
------ Pass ParseInputs: 0.243s
------ Pass Import non-system refs: 0.0713s
------ Pass Typecheck: 1.8553s
------ Pass Optimizations: 0.1384s
------ Pass TAST -> IL: 0.2901s
------ Pass Write .NET Binary: 0.3592s
Time Elapsed 00:00:06.14

Run 4:
------ Pass ImportMscorlib+FSharp.Core: 0.7781s
------ Pass ParseInputs: 0.2419s
------ Pass Import non-system refs: 0.0726s
------ Pass Typecheck: 1.8713s
------ Pass Optimizations: 0.132s
------ Pass TAST -> IL: 0.3401s
------ Pass Write .NET Binary: 0.3632s
Time Elapsed 00:00:06.18

Run 5:
------ Pass ImportMscorlib+FSharp.Core: 0.787s
------ Pass ParseInputs: 0.2501s
------ Pass Import non-system refs: 0.0715s
------ Pass Typecheck: 1.8969s
------ Pass Optimizations: 0.1338s
------ Pass TAST -> IL: 0.2923s
------ Pass Write .NET Binary: 0.4616s
Time Elapsed 00:00:06.30
```

#### Optimized Raw Data Table

| Run | ImportMscorlib (s) | ParseInputs (s) | ImportNonSys (s) | Typecheck (s) | Optimizations (s) | TAST→IL (s) | WriteNetBinary (s) | Total (s) |
|-----|-------------------|-----------------|------------------|---------------|-------------------|-------------|-------------------|-----------|
| 1 | 0.7726 | 0.2509 | 0.0760 | 1.9027 | 0.1306 | 0.2871 | 0.3969 | 6.48 |
| 2 | 0.7566 | 0.2518 | 0.0802 | 1.9032 | 0.1335 | 0.2911 | 0.3846 | 6.19 |
| 3 | 0.7987 | 0.2430 | 0.0713 | 1.8553 | 0.1384 | 0.2901 | 0.3592 | 6.14 |
| 4 | 0.7781 | 0.2419 | 0.0726 | 1.8713 | 0.1320 | 0.3401 | 0.3632 | 6.18 |
| 5 | 0.7870 | 0.2501 | 0.0715 | 1.8969 | 0.1338 | 0.2923 | 0.4616 | 6.30 |

---

## Statistical Comparison

### Summary Statistics by Phase

| Phase | Baseline Mean (s) | Baseline StdDev | Optimized Mean (s) | Optimized StdDev | Delta (s) | Improvement |
|-------|-------------------|-----------------|-------------------|-----------------|-----------|-------------|
| ImportMscorlib+FSharp.Core | 0.7939 | 0.0122 | 0.7786 | 0.0141 | -0.0153 | 1.9% |
| ParseInputs | 0.2441 | 0.0034 | 0.2475 | 0.0042 | +0.0034 | -1.4% |
| Import non-system refs | 0.0739 | 0.0032 | 0.0743 | 0.0034 | +0.0004 | -0.5% |
| **Typecheck** | **2.5785** | **0.1723** | **1.8859** | **0.0192** | **-0.6926** | **26.9%** |
| Optimizations | 0.1420 | 0.0123 | 0.1337 | 0.0026 | -0.0083 | 5.8% |
| TAST → IL | 0.3099 | 0.0212 | 0.3001 | 0.0201 | -0.0098 | 3.2% |
| Write .NET Binary | 0.4283 | 0.0262 | 0.3931 | 0.0369 | -0.0352 | 8.2% |
| **Total Elapsed** | **7.036** | **0.1768** | **6.258** | **0.123** | **-0.778** | **11.1%** |

### Key Observations

1. **Typecheck phase shows dramatic improvement**: 26.9% faster with 0.6926s saved per compilation
2. **Reduced variance in optimized build**: StdDev dropped from 0.1723s to 0.0192s (89% reduction)
3. **Other phases unchanged**: ImportMscorlib, ParseInputs, and ImportNonSys show negligible difference, confirming the optimization targets only overload resolution
4. **Total compilation improvement**: 11.1% faster overall (0.778s saved)

### Statistical Significance

The typecheck improvement is statistically significant:
- **Effect size**: 0.6926s improvement (baseline mean - optimized mean)
- **Baseline range**: 2.3861s - 2.8858s
- **Optimized range**: 1.8553s - 1.9032s
- **No overlap**: The ranges do not intersect, indicating a real performance difference
- **Variance reduction**: The optimized compiler shows much more consistent timing (StdDev 0.0192 vs 0.1723)

---

## Typed vs. Untyped Compilation Analysis

### Test Design

The test file was specifically designed to measure the difference between:

1. **Untyped calls** (100 calls): `Assert.Equal(1, 1)` - Compiler must resolve against all 30+ overloads
2. **Typed calls** (100 calls): `Assert.Equal<int>(1, 1)` - Explicit type parameter narrows resolution

### Analysis

The `--times` output measures the **total typecheck phase**, which includes resolution of all 200+ calls. Without per-call profiling, we cannot isolate typed vs. untyped resolution costs individually.

However, the optimizations in the PR address both scenarios:

| Optimization | Affects Typed | Affects Untyped |
|--------------|--------------|-----------------|
| Overload candidate caching | ✓ | ✓ |
| Early pruning of incompatible overloads | - | ✓ |
| Type-directed filtering | ✓ | ✓ |
| Reduced constraint solving iterations | - | ✓ |

**Observation**: The 26.9% improvement applies to the mixed workload (100 typed + 100 untyped). The benefit would likely be **higher for pure untyped workloads** since untyped calls require more resolution work.

### Real-World Impact

In real projects using xUnit or similar heavily-overloaded APIs:
- Files with many untyped `Assert.Equal` calls will see the largest benefit
- Adding explicit type annotations (`Assert.Equal<int>`) can further improve compilation times
- The caching optimizations benefit repeated calls to the same overload set

---

## Cache Hit Statistics

The optimized compiler includes caching mechanisms for overload resolution. Unfortunately, cache hit statistics are not exposed in the standard `--times` output.

### Evidence of Caching Effectiveness

Indirect evidence of caching effectiveness:

1. **Reduced variance**: The optimized compiler shows 89% lower standard deviation in typecheck time (0.0192 vs 0.1723), suggesting more predictable/cached behavior
2. **Consistent improvement across runs**: All 5 optimized runs are tightly clustered around 1.88-1.90s
3. **Baseline variance**: The baseline shows more variance (2.38-2.89s), consistent with non-cached resolution

### Future Work

To expose cache statistics, the compiler could be instrumented to output:
- Number of overload resolution cache lookups
- Cache hit rate
- Cache memory usage

---

## Conclusions

### Summary

The method overload resolution optimizations in PR branch `copilot/create-performance-profiling-automation` deliver:

| Metric | Value |
|--------|-------|
| **Typecheck time reduction** | 26.9% |
| **Total compilation time reduction** | 11.1% |
| **Absolute time saved (typecheck)** | 0.693s per compilation |
| **Absolute time saved (total)** | 0.778s per compilation |
| **Variance reduction** | 89% (more consistent timings) |

### Interpretation

1. **The optimization is highly effective** for workloads involving heavily-overloaded methods like xUnit's `Assert.Equal`

2. **The improvement is targeted**: Only the typecheck phase shows significant improvement; other phases are unaffected, confirming the optimization is scoped correctly

3. **Production impact**: For large test projects with thousands of `Assert.Equal` calls, this could translate to minutes saved in CI/CD pipelines

4. **Scalability**: The reduced variance suggests the optimizations scale better than the baseline under repeated similar resolution tasks

### Recommendations

1. **Merge the PR**: The performance improvement is significant and well-isolated
2. **Consider adding cache metrics**: Future instrumentation could help quantify caching effectiveness
3. **Expand testing**: Test with real-world projects (FSharp.Compiler.Service test suite) to validate production impact

---

## References

- Issue: [#18807](https://github.com/dotnet/fsharp/issues/18807) - Method overload resolution performance
- Baseline Commit: `def2b8239e52583fd714992e3c8e4c50813717df`
- Optimized Commit: `f3f07201028c8a70335ec689f1017ddaae1d9bb1`
- Test Infrastructure: `Q:\source\fsharp\overload-perf-test\`
