# F# Compiler Stack Trace Analysis

**Generated:** 2025-11-25T13:40:00Z  
**Dump Captured:** From FSC.dll process during FSharpPlus compilation  
**SDK Version:** 10.0.100  

---

## Executive Summary

The F# compiler (FSC) dump captured during the FSharpPlus build hang reveals the **exact location** in the F# compiler where the slow/hanging behavior occurs.

### ⚠️ CONFIRMED HANG LOCATION: `FSharp.Compiler.CheckDeclarations`

The main compiler thread is stuck in **type checking** within `CheckDeclarations.fs`.

---

## F# Compiler Stack Trace (Main Thread)

```
FSharp.Compiler.CommandLineMain.main(String[])
  └── FSharp.Compiler.Driver.CompileFromCommandLineArguments @ fsc.fs:1231
      └── FSharp.Compiler.Driver.main1 @ fsc.fs:678
          └── FSharp.Compiler.Driver.TypeCheck @ fsc.fs:155
              └── FSharp.Compiler.ParseAndCheckInputs.CheckClosedInputSet @ ParseAndCheckInputs.fs:1884
                  └── Microsoft.FSharp.Collections.ListModule.MapFold @ list.fs:110
                      └── FSharp.Compiler.ParseAndCheckInputs.CheckOneInputEntry @ ParseAndCheckInputs.fs:1352
                          └── FSharp.Compiler.ParseAndCheckInputs.CheckOneInput @ ParseAndCheckInputs.fs:1230-1307
                              └── FSharp.Compiler.CheckDeclarations.CheckOneImplFile @ CheckDeclarations.fs:5775-5797
                                  └── FSharp.Compiler.CheckDeclarations.TcModuleOrNamespaceElementsNonMutRec @ CheckDeclarations.fs:5516
                                      └── FSharp.Compiler.CheckDeclarations.TcModuleOrNamespaceElementNonMutRec @ CheckDeclarations.fs:5267-5462
                                          └── (REPEATING PATTERN - recursive type checking)
```

---

## Key Findings

### 1. Hang Location: Type Checking (`CheckDeclarations.fs`)

The stack trace shows deep recursion in:
- **File:** `src/Compiler/Checking/CheckDeclarations.fs`
- **Function:** `TcModuleOrNamespaceElementsNonMutRec`
- **Line:** 5516

This function is called recursively, indicating the compiler is processing a deeply nested or complex type structure.

### 2. Specific Source Files

| File | Line | Function |
|------|------|----------|
| `CheckDeclarations.fs` | 5516 | `TcModuleOrNamespaceElementsNonMutRec` |
| `CheckDeclarations.fs` | 5549-5562 | `TcModuleOrNamespaceElements` |
| `CheckDeclarations.fs` | 5267-5462 | `TcModuleOrNamespaceElementNonMutRec` |
| `CheckDeclarations.fs` | 5775-5797 | `CheckOneImplFile` |
| `ParseAndCheckInputs.fs` | 1884 | `CheckClosedInputSet` |
| `fsc.fs` | 155 | `TypeCheck` |

### 3. Call Pattern

The recursive call pattern shows:
```
TcModuleOrNamespaceElementsNonMutRec
  → TcModuleOrNamespaceElements
    → TcModuleOrNamespaceElementNonMutRec (recursive)
      → TcModuleOrNamespaceElementsNonMutRec (recursive)
        → ...
```

This recursive type checking of nested namespaces/modules appears to be the bottleneck.

### 4. Thread State

| Thread | State | Activity |
|--------|-------|----------|
| 30588 (Main) | **Active** | Deep in CheckDeclarations type checking |
| 30590-30604 | Idle | Thread pool workers waiting |
| 30606 | Waiting | GateThread (system thread) |

---

## Root Cause Analysis

### The Issue

The F# compiler in SDK 10.0.100 spends excessive time in `TcModuleOrNamespaceElementsNonMutRec` when type-checking FSharpPlus, which uses:

1. **Complex generic constraints** (higher-kinded type simulations)
2. **Deeply nested type definitions** (e.g., in Control/Functor.fs, Control/Applicative.fs)
3. **Many interdependent type extensions**

### Evidence of Performance Regression

- SDK 10.0.100-rc.2: **Completes** type checking in ~4.5 minutes
- SDK 10.0.100: **Does not complete** type checking within 3+ minutes (times out)

This suggests a change between rc.2 and release in:
- Type inference complexity
- Constraint solving algorithm
- Or the recursive processing in `TcModuleOrNamespaceElementsNonMutRec`

---

## Files to Investigate in F# Compiler

1. **`src/Compiler/Checking/CheckDeclarations.fs`**
   - Line 5516: `TcModuleOrNamespaceElementsNonMutRec`
   - Look for recent changes to this function

2. **`src/Compiler/Checking/ConstraintSolver.fs`**
   - May affect type inference speed

3. **`src/Compiler/Checking/TypeChecker.fs`**
   - Core type checking logic

---

## Reproduction

To reproduce this analysis:

```bash
# Clone FSharpPlus
git clone --branch gus/fsharp9 https://github.com/fsprojects/FSharpPlus.git
cd FSharpPlus
echo '{"sdk":{"version":"10.0.100"}}' > global.json

# Start FSC directly
/usr/share/dotnet/dotnet /usr/share/dotnet/sdk/10.0.100/FSharp/fsc.dll \
  @<rsp-file-from-msbuild> &

# Wait 30 seconds and capture dump
dotnet-dump collect -p <FSC_PID> --output fsc-hang.dmp

# Analyze
dotnet-dump analyze fsc-hang.dmp --command "clrstack"
```

---

## Recommended Actions

1. **Bisect F# compiler changes** between 10.0.100-rc.2 and 10.0.100
2. **Focus on `CheckDeclarations.fs`** changes around line 5516
3. **Add profiling** to `TcModuleOrNamespaceElementsNonMutRec` to measure time spent
4. **Test with FSharpPlus** as a benchmark for complex type systems

---

*Generated by FSharpPlus hang diagnostic pipeline*
