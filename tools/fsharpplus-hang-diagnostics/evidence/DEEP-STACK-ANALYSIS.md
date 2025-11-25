# Deep Stack Analysis - F# Compiler Hang in FSharpPlus

**Generated:** 2025-11-25T14:20:00Z  
**Issue:** https://github.com/dotnet/fsharp/issues/19116

---

## Executive Summary

Analysis of the F# compiler during FSharpPlus compilation reveals a **deep recursive call stack** in the type checking phase. The compiler is stuck in `CheckDeclarations.fs` processing nested modules/namespaces with complex type constraints.

---

## Stack Depth Analysis

### Recursion Layers Counted

From the captured stack trace, the recursive pattern repeats multiple times:

```
TcModuleOrNamespaceElementsNonMutRec @ CheckDeclarations.fs:5516
  → TcModuleOrNamespaceElements @ CheckDeclarations.fs:5549-5562
    → TcModuleOrNamespaceElementNonMutRec @ CheckDeclarations.fs:5267-5462
      → TcModuleOrNamespaceElementsNonMutRec @ CheckDeclarations.fs:5516
        → (repeating pattern)
```

**Observed recursion depth:** At least **6-8 layers** of the same pattern visible in single stack capture.

### Very Bottom of Stack (Entry Point)

```
FSharp.Compiler.CommandLineMain.main(String[])                    @ fscmain.fs:79
  └── FSharp.Compiler.Driver.CompileFromCommandLineArguments      @ fsc.fs:1231
      └── FSharp.Compiler.Driver.main1                            @ fsc.fs:678
          └── FSharp.Compiler.Driver.TypeCheck                    @ fsc.fs:155
              └── FSharp.Compiler.ParseAndCheckInputs.CheckClosedInputSet @ ParseAndCheckInputs.fs:1884
```

### Very Top of Stack (Where Execution Is)

```
FSharp.Compiler.CheckDeclarations.TcModuleOrNamespaceElementNonMutRec
  @ CheckDeclarations.fs:5267-5462
  
  └── Called by: TcModuleOrNamespaceElementsNonMutRec @ line 5513
      └── Which processes: synModuleDecls recursively
```

---

## Cache-Related Findings

### Cache References in F# Compiler Checking Phase

Found **multiple caches** in the type checking pipeline:

#### 1. InfoReader.fs Caches (Lines 806-817)

| Cache Name | Purpose | Location |
|------------|---------|----------|
| `methodInfoCache` | Method sets of types | InfoReader.fs:806 |
| `propertyInfoCache` | Property sets of types | InfoReader.fs:807 |
| `recdOrClassFieldInfoCache` | Record/class field info | InfoReader.fs:808 |
| `ilFieldInfoCache` | IL field info | InfoReader.fs:809 |
| `eventInfoCache` | Event info | InfoReader.fs:810 |
| `namedItemsCache` | Named items lookup | InfoReader.fs:811 |
| `mostSpecificOverrideMethodInfoCache` | Override method info | InfoReader.fs:812 |
| `entireTypeHierarchyCache` | Full type hierarchy | InfoReader.fs:814 |
| `primaryTypeHierarchyCache` | Primary type hierarchy | InfoReader.fs:815 |
| `implicitConversionCache` | Implicit conversions | InfoReader.fs:816 |
| `isInterfaceWithStaticAbstractMethodCache` | Interface SAM check | InfoReader.fs:817 |

#### 2. CheckBasics.fs Caches (Lines 161-179)

| Cache Name | Purpose | Location |
|------------|---------|----------|
| `cachedFreeLocalTycons` | Free type constructors | CheckBasics.fs:162 |
| `cachedFreeTraitSolutions` | Free trait solutions | CheckBasics.fs:165 |
| `eCachedImplicitYieldExpressions` | Implicit yield exprs | CheckBasics.fs:242 |
| `argInfoCache` | Argument info | CheckBasics.fs:314 |

#### 3. CheckDeclarations.fs Cache (Line 5613)

```fsharp
eCachedImplicitYieldExpressions = HashMultiMap(HashIdentity.Structural, useConcurrentDictionary = true)
```

### Cache Behavior Analysis

The caches in `InfoReader.fs` use `MakeInfoCache` (line 730) which:
1. **Only caches monomorphic types** (line 733)
2. Does NOT cache generic type instantiations
3. Uses structural equality for type comparison

**Key insight from InfoReader.fs:738:**
```fsharp
// It would matter for different generic instantiations of the same type, 
// but we don't cache that here - TType_app is always matched for `[]` typars.
```

This means FSharpPlus's heavy use of **generic types with complex constraints** may cause:
- Cache misses (types are polymorphic, not monomorphic)
- Repeated computation for each generic instantiation
- Exponential slowdown with deeply nested generic types

---

## Thread Information

### Thread Count During Hang

| Thread State | Count | Description |
|--------------|-------|-------------|
| Main Thread | 1 | Deep in CheckDeclarations type checking |
| Worker Threads | 4-5 | Idle (waiting for work) |
| GateThread | 1 | System thread |
| **Total** | ~7-17 | Varies by dump timing |

### Main Thread Activity

The main thread (Thread 0) is consistently in:
```
FSharp.Compiler.CheckDeclarations.TcModuleOrNamespaceElementsNonMutRec
```

Worker threads are **idle**, indicating the compilation is **single-threaded** at this point.

---

## Timing Information

### Build Timing Comparison

| SDK Version | Duration | Status |
|-------------|----------|--------|
| 10.0.100 | >180s (killed) | ❌ HUNG |
| 10.0.100-rc.2 | ~265s | ✅ Completed |

### Trace Event Timeline (SDK 10.0.100)

| Time | Event Density | Status |
|------|---------------|--------|
| 0-10s | 32,314 events/sec | Active |
| 10-20s | Decreasing | Slowing |
| 20-80s | 0-28 events/sec | Minimal |
| 80-120s | 0 events/sec | **HUNG** |

### Time Gaps Detected

| Gap # | Duration | Significance |
|-------|----------|--------------|
| 1 | 36.13 seconds | **Largest - likely hang point** |
| 2-15 | 1-10 seconds | Multiple pauses during type checking |

---

## Root Cause Hypothesis

Based on the evidence:

1. **Deep Recursion in CheckDeclarations.fs**
   - `TcModuleOrNamespaceElementsNonMutRec` recurses for each nested module
   - FSharpPlus has deeply nested Control/Data modules

2. **Cache Inefficiency for Generic Types**
   - InfoReader caches only work for monomorphic types
   - FSharpPlus uses extensive generic types with complex constraints
   - Each unique generic instantiation triggers full recomputation

3. **Possible Regression in SDK 10.0.100**
   - Something changed between rc.2 and release
   - May affect constraint solving or type inference
   - Could be in `ConstraintSolver.fs` or `CheckDeclarations.fs`

---

## Files to Investigate

| File | Priority | Reason |
|------|----------|--------|
| `CheckDeclarations.fs` | **HIGH** | Contains hang point (line 5516) |
| `InfoReader.fs` | **HIGH** | Cache implementation for type info |
| `ConstraintSolver.fs` | **MEDIUM** | Constraint solving may be slow |
| `CheckBasics.fs` | **MEDIUM** | Additional caches |
| `TypeChecker.fs` | **MEDIUM** | Core type checking |

---

## Recommended Actions

1. **Profile `TcModuleOrNamespaceElementsNonMutRec`**
   - Add timing instrumentation
   - Count recursive calls per file

2. **Investigate Cache Hit Rates**
   - Add counters to InfoReader caches
   - Check hit/miss ratio for FSharpPlus build

3. **Bisect Compiler Changes**
   - Compare rc.2 vs release changes in Checking folder
   - Focus on constraint solving and type inference

4. **Test with Simpler FSharpPlus Subset**
   - Identify which specific files cause slowdown
   - Narrow down to specific type patterns

---

*Analysis completed by FSharpPlus hang diagnostic pipeline*
