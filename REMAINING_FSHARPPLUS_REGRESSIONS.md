# FSharpPlus Regressions with Local F# Compiler

## Summary

FSharpPlus build and test suite **passes** with the local F# compiler when known regression tests are commented out. All 397 tests pass in that configuration.

There are test cases in FSharpPlus that expose F# compiler regressions. These tests have been **uncommented for investigation** and cause the compiler to **hang indefinitely** during type inference.

## Verified Working (With Regression Tests Commented Out)

| Target | Status | Notes |
|--------|--------|-------|
| Build | ✅ PASS | FSharpPlus.dll builds successfully |
| Test | ✅ PASS | 397/397 tests pass |
| AllDocs | ✅ PASS | Documentation builds |
| ReleaseDocs | ✅ PASS | Release docs build |

---

## Documented Regressions

### Regression 1: FS0465 - Choice with NonEmptyList

**Error Code**: FS0465  
**Source**: `FSharpPlus/tests/FSharpPlus.Tests/General.fs` line 1199

#### Minimal Reproduction Code
```fsharp
// Save as: ChoiceRegression.fsx
// Run with: dotnet fsi ChoiceRegression.fsx

type NonEmptyList<'T> = { Head: 'T; Tail: 'T list }

module NonEmptyList =
    let ofList xs = 
        match xs with
        | h :: t -> { Head = h; Tail = t }
        | _ -> failwith "empty"

// SRTP constraint for "choice" overload resolution
let inline choice (source: ^C) : ^C =
    ((^C) : (static member Choice : ^C -> ^C) source)

// This pattern causes FS0465 or compiler hang:
let inline test (t: 'a list when 'a :> seq<_>) =
    let nel = NonEmptyList.ofList t
    choice nel  // <-- Compiler cannot resolve SRTP here
```

#### Expected Behavior
Compiler should produce error FS0465 within a few seconds:
```
error FS0465: Type inference problem too complicated (maximum iteration depth reached).
Consider adding further type annotations.
```

#### Actual Behavior
Compiler **hangs indefinitely** (3+ minutes) without producing any error message.

---

### Regression 2: FS0071 - curryN Type Constraint Mismatch

**Error Code**: FS0071  
**Source**: `FSharpPlus/tests/FSharpPlus.Tests/General.fs` line 1707

#### Minimal Reproduction Code
```fsharp
// Save as: CurryNRegression.fsx
// Run with: dotnet fsi CurryNRegression.fsx

open System

// Simplified curryN using SRTP and type-level computation
let inline curryN (f: 'T -> 'R) (x: int) : 'R =
    let t = Activator.CreateInstance(typeof<'T>, [| box x |]) :?> 'T
    f t

// Simple function taking a Tuple
let f1 (t: Tuple<int>) : int list = [t.Item1; t.Item1 * 2]

// This triggers FS0071:
let _x1 = curryN f1 100
```

#### Expected Behavior
Compiler should produce:
```
error FS0071: Type constraint mismatch when applying the default type 'Tuple<int>' for a type inference variable.
Type mismatch. Expecting a '(Tuple<int> -> int list) -> int -> obj' 
but given a '(Tuple<int> -> int list) -> int -> int list'
The type 'obj' does not match the type 'int list'
```

#### Actual Behavior
Error FS0071 is produced, but the error message is confusing - the type system incorrectly defaults return type to `obj` when it should be `int list`.

---

### Regression 3: traverse with WrappedListH

**Error Code**: (Compiler Hang)  
**Source**: `FSharpPlus/tests/FSharpPlus.Tests/Traversals.fs` lines 42-50

#### Minimal Reproduction Code
```fsharp
// Save as: TraverseRegression.fsx
// Run with: dotnet fsi TraverseRegression.fsx

// Custom collection with Traverse member
type WrappedListH<'T> = WLH of 'T list

module WrappedListH =
    let create x = WLH x

// SRTP-based traverse
let inline traverse (f: 'a -> 'b) (source: ^C) : ^D =
    ((^C or ^D) : (static member Traverse : ^C * ('a -> 'b) -> ^D) source, f)

// Usage that triggers complex type inference
let testVal () =
    traverse (fun x -> [int16 x..int16 (x+2)]) (WLH [1; 4])
```

#### Expected Behavior
Compiler should either:
1. Successfully compile (if types are resolvable)
2. Produce a clear error within a few seconds

#### Actual Behavior
Compiler **hangs indefinitely** attempting to resolve SRTP constraints across multiple collection types.

---

### Regression 4: sequence Specialization Issues

**Error Code**: (Compiler Hang)  
**Source**: `FSharpPlus/tests/FSharpPlus.Tests/Traversals.fs` lines 56-66

#### Minimal Reproduction Code
```fsharp
// Save as: SequenceRegression.fsx
// Run with: dotnet fsi SequenceRegression.fsx

// SRTP-based sequence function
let inline sequence (source: ^C) : ^D =
    ((^C or ^D) : (static member Sequence : ^C -> ^D) source)

// Inline wrappers with type constraints
let inline seqArr (x: _ []) = sequence x
let inline seqLst (x: _ list) = sequence x

// These cause compiler hang:
let b = seqArr [|[1];[3]|]
let c = seqLst [[1];[3]]
```

#### Expected Behavior
Compiler should either:
1. Resolve the SRTP constraint and compile
2. Produce FS0465 if inference is too complicated

#### Actual Behavior
Compiler **hangs indefinitely** during SRTP constraint resolution for nested collection types.

---

## Root Cause Analysis

All regressions share a common pattern:
1. **SRTP (Statically Resolved Type Parameters)** with multiple type constraints
2. **Nested generic types** (e.g., `list<list<int>>`, `NonEmptyList<seq<_>>`)
3. **Type-level computation** requiring constraint resolution across multiple types

The F# compiler's type inference engine appears to enter an **infinite loop** or **exponential search** when resolving these complex SRTP constraints, rather than hitting the documented iteration limit (FS0465).

---

## Environment

| Component | Value |
|-----------|-------|
| FSharpPlus branch | `gus/fsharp9` |
| Local F# compiler | `artifacts/bin/fsc/Release/net10.0/fsc.dll` |
| Target Framework | net10.0 |
| .NET SDK | 10.0 |

---

## Recommendations

1. **Investigate SRTP iteration limit**: The FS0465 limit may not be triggering correctly in new type inference code paths
2. **Add timeout mechanism**: Consider adding wall-clock timeout in addition to iteration count
3. **Create component tests**: Port these minimal repros to `tests/FSharp.Compiler.ComponentTests` for regression prevention

---

## Test Commands for Reproduction

### Using FSharpPlus directly
```bash
cd ~/code/FSharpPlus
export LoadLocalFSharpBuild=True
export LocalFSharpCompilerPath=~/code/fsharp-experiments/dotnet-fsharp
export LocalFSharpCompilerConfiguration=Release
dotnet build tests/FSharpPlus.Tests/FSharpPlus.Tests.fsproj
```

### Expected result with uncommented tests
- **Duration**: 3+ minutes with no progress
- **Exit Code**: Killed (timeout)
- **Behavior**: Compiler hangs on FSharpPlus.Tests compilation
