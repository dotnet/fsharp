# F# Compiler Performance Analysis - xUnit Assert.Equal Issue #18807

*This report contains **ACTUAL RESULTS** from running the profiling automation suite on .NET 10.0.100-rc.2*

*Generated: 2025-11-11 14:17:05*

## Test Configuration
- **Total Assert.Equal calls**: 1500
- **Test methods**: 15
- **Type variants**: int, string, float, bool, int64, decimal, byte, char
- **F# Compiler**: 14.0.100.0 for F# 10.0
- **.NET SDK**: 10.0.100-rc.2.25502.107
- **Test Environment**: Linux (Ubuntu) on GitHub Actions runner

## Compilation Times

### Untyped Version (Slow Path)
- **Total compilation time**: 5.96 seconds
- **Time per Assert.Equal**: 3.97 ms

### Typed Version (Fast Path)
- **Total compilation time**: 5.29 seconds
- **Time per Assert.Equal**: 3.52 ms

### Performance Difference
- **Slowdown factor**: 1.13x
- **Time difference**: 0.67 seconds

## Hot Path Analysis

*Note: Detailed trace analysis was not performed in this run due to the overhead of trace collection.*
*The profiling focused on accurate timing measurements of compilation performance.*

### Key Observation

The performance difference observed (13% slowdown) is **significantly less** than the issue #18807 originally reported (~100ms per Assert.Equal, or 30x+ slowdown for larger test suites). This suggests:

1. **Compiler improvements**: Recent F# compiler versions may have optimized overload resolution
2. **Test scale**: The overhead may become more pronounced with even larger test files (3000+ asserts)
3. **Environment differences**: The issue reporter may have been using different hardware/environment
4. **Pattern sensitivity**: Certain patterns of Assert.Equal usage may trigger worse performance

### Actual Impact Measured

For the 1500 Assert.Equal test:
- Extra time with untyped: **0.67 seconds** total (**0.45ms per call**)
- This is **much better** than the reported 100ms per call
- However, it still represents wasted compilation time that could be eliminated

## Key Findings

### Performance Impact of Untyped Assert.Equal

While the impact is smaller than initially reported, there is still measurable overhead:
- Each untyped Assert.Equal adds approximately **0.45ms** more compilation time than typed
- For large test suites, this accumulates (1500 calls = 0.67s extra)
- The overhead exists even with modern compiler optimizations

### Likely Root Causes (Based on Issue Analysis)

Based on the issue discussion and F# compiler architecture:

1. **Overload Resolution Complexity**
   - xUnit's `Assert.Equal` has many overloads
   - F# compiler tries each overload during type inference
   - Each attempt typechecks the full overload signature
   - Location: `src/Compiler/Checking/ConstraintSolver.fs` around line 3486

2. **Type Inference Without Explicit Types**
   - Untyped calls force the compiler to infer types from usage
   - This requires constraint solving for each Assert.Equal call
   - Typed calls bypass most of this overhead

3. **Lack of Caching**
   - Overload resolution results may not be cached
   - Each Assert.Equal call repeats the same expensive analysis

## Optimization Opportunities

### 1. Overload Resolution Caching (High Impact)
- **Location**: `src/Compiler/Checking/ConstraintSolver.fs`
- **Opportunity**: Cache overload resolution results for identical call patterns
- **Expected Impact**: Could reduce compilation time by 50-80% for repetitive patterns
- **Rationale**: Many Assert.Equal calls have identical type patterns

### 2. Early Overload Pruning (Medium Impact)
- **Location**: `src/Compiler/Checking/MethodCalls.fs`
- **Opportunity**: Filter incompatible overloads before full type checking
- **Expected Impact**: Could reduce time by 30-50%
- **Rationale**: Many overloads can be ruled out based on argument count/types

### 3. Incremental Type Inference (Medium Impact)
- **Location**: `src/Compiler/Checking/TypeChecker.fs`
- **Opportunity**: Reuse partial type information across similar calls
- **Expected Impact**: Could reduce time by 20-40%

## Recommendations

### For Users (Immediate Workarounds)

1. **Add Type Annotations**
   ```fsharp
   Assert.Equal<int>(expected, actual)  // Explicit type
   ```

2. **Use Wrapper Functions**
   ```fsharp
   let assertEqual (x: 'T) (y: 'T) = Assert.Equal<'T>(x, y)
   assertEqual expected actual  // Type inferred once
   ```

### For Compiler Developers

1. **Further Investigation Needed**: The reduced impact compared to the issue report suggests the problem may be:
   - Already partially improved in recent compiler versions
   - More pronounced with specific usage patterns
   - Dependent on test file structure or size

2. **Recommend Deeper Profiling**: Use dotnet-trace with actual trace collection to identify exact bottlenecks in ConstraintSolver.fs

3. **Scale Testing**: Test with 3000-5000 Assert.Equal calls to see if overhead scales linearly or exponentially

4. **Pattern Analysis**: Investigate if certain combinations of types or test structures trigger worse performance

## Test Artifacts

### Generated Test Structure
- **Untyped test file**: 1500 calls without type annotations (e.g., `Assert.Equal(42, value)`)
- **Typed test file**: 1500 calls with explicit types (e.g., `Assert.Equal<int>(42, value)`)
- **Type distribution**: Each test method cycles through 8 primitive types
- **Method structure**: 15 test methods with 100 Assert.Equal calls each

### Build Configuration
- Release mode compilation
- No debug symbols (`/p:DebugType=None /p:DebugSymbols=false`)
- Dependencies restored before timing to isolate compilation performance

## Raw Data

| Metric | Untyped (Slow) | Typed (Fast) | Difference |
|--------|----------------|--------------|------------|
| Total Time | 5.96s | 5.29s | 0.67s |
| Time/Assert | 3.97ms | 3.52ms | 0.45ms |
| Slowdown | 1.13x | 1.0x | - |

## Reproducibility

To reproduce these results:

```bash
cd tools/perf-repro
./RunPerfAnalysis.sh --total 1500 --methods 15
```

The actual test projects and build logs are available in the generated directories for verification.

---

*This report was automatically generated by the F# compiler performance profiling suite.*
*For more information, see issue [#18807](https://github.com/dotnet/fsharp/issues/18807).*

## Next Steps

1. **Investigate the discrepancy** between this result (1.13x slowdown) and the issue report (30x+ slowdown)
2. **Run with larger scale** (3000-5000 asserts) to see if overhead compounds
3. **Collect actual traces** with dotnet-trace to identify exact hot paths
4. **Test on different environments** to see if results vary by platform/hardware
5. **Analyze the generated IL** to understand what the compiler is doing differently
