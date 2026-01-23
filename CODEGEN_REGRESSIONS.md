# CodeGen Regressions Documentation

This document tracks known code generation bugs in the F# compiler that have documented test cases but are not yet fixed. Each issue has a corresponding test in `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs` with its `[<Fact>]` attribute commented out.

## Summary Table

| Issue | Title | Category | Fix Location | Risk |
|-------|-------|----------|--------------|------|
| [#19075](#issue-19075) | CLR crash with constrained calls | Runtime Crash | IlxGen.fs | Medium |
| [#19068](#issue-19068) | Struct object expression generates byref field | Invalid IL | IlxGen.fs | Low |
| [#19020](#issue-19020) | [<return:>] not respected on class members | Missing Attribute | IlxGen.fs | Low |
| [#18956](#issue-18956) | Decimal [<Literal>] InvalidProgramException in Debug | Invalid IL | IlxGen.fs | Low |
| [#18953](#issue-18953) | Action/Func conversion captures extra expressions | Wrong Behavior | TypeRelations.fs | Medium |
| [#18868](#issue-18868) | CallerFilePath in delegates error | Compile Error | CheckDeclarations.fs | Low |
| [#18815](#issue-18815) | Duplicate extension method names | Compile Error | IlxGen.fs | Low |
| [#18753](#issue-18753) | CE inlining prevented by DU constructor | Optimization | Optimizer.fs | Low |
| [#18672](#issue-18672) | Resumable code top-level value null in Release | Wrong Behavior | IlxGen.fs/StateMachine | Medium |
| [#18374](#issue-18374) | RuntimeWrappedException cannot be caught | Wrong Behavior | IlxGen.fs | Low |

---

## Issue #19075

**Title:** CLR Crashes when running program using constrained calls

**Link:** https://github.com/dotnet/fsharp/issues/19075

**Category:** Runtime Crash (Segfault)

### Minimal Repro

```fsharp
module Dispose

open System
open System.IO

[<RequireQualifiedAccess>]
module Dispose =
    let inline action<'a when 'a: (member Dispose: unit -> unit) and 'a :> IDisposable>(a: 'a) = a.Dispose()

[<EntryPoint>]
let main argv =
    let ms = new MemoryStream()
    ms |> Dispose.action
    0
```

### Expected Behavior
Program runs and disposes the MemoryStream without error.

### Actual Behavior
Fatal CLR error (0x80131506) - segfault when running the program.

### Test Location
`CodeGenRegressions.fs` → `Issue_19075_ConstrainedCallsCrash`

### Analysis
The IL generates a constrained call that violates CLR constraints. The combination of SRTP member constraint with IDisposable interface constraint produces invalid IL:
```
constrained. !!a
callvirt instance void [System.Runtime]System.IDisposable::Dispose()
```

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - constrained call generation for SRTP with interface constraints

### Risks
- Medium: Changes to constrained call generation could affect other SRTP scenarios
- Need careful testing of all SRTP with interface constraint combinations

---

## Issue #19068

**Title:** Object expression in struct generates byref field in a class

**Link:** https://github.com/dotnet/fsharp/issues/19068

**Category:** Invalid IL (TypeLoadException)

### Minimal Repro

```fsharp
type Class(test : obj) = class end

[<Struct>]
type Struct(test : obj) =
    member _.Test() = {
        new Class(test) with
        member _.ToString() = ""
    }
```

### Expected Behavior
Struct compiles to valid IL that creates an object expression.

### Actual Behavior
Generated anonymous class has a byref field, causing TypeLoadException at runtime.

### Test Location
`CodeGenRegressions.fs` → `Issue_19068_StructObjectExprByrefField`

### Analysis
When an object expression inside a struct depends on the containing type's values, the compiler generates a closure-like class with a byref field to the struct. However, byref fields are not valid in classes.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - object expression codegen for struct context

### Risks
- Low: Fix would change closure capture strategy for object expressions in structs
- Workaround exists: bind constructor args to variables first

---

## Issue #19020

**Title:** [<return: ...>] not respected on class members

**Link:** https://github.com/dotnet/fsharp/issues/19020

**Category:** Missing Attribute

### Minimal Repro

```fsharp
type SomeAttribute() =
    inherit System.Attribute()

module Module =
    [<return: SomeAttribute>]
    let func a = a + 1  // Works - attribute is emitted

type Class() =
    [<return: SomeAttribute>]
    static member func a = a + 1  // Broken - attribute is dropped
```

### Expected Behavior
`[<return: SomeAttribute>]` should emit the attribute on the return type for both module functions and class members.

### Actual Behavior
Attribute is correctly emitted for module functions but dropped for class members.

### Test Location
`CodeGenRegressions.fs` → `Issue_19020_ReturnAttributeNotRespected`

### Analysis
The IL generation path for class members doesn't process the `return:` attribute target correctly.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - attribute emission for class member return types

### Risks
- Low: Straightforward fix to handle return attribute target in class member path

---

## Issue #18956

**Title:** Decimal constant causes InvalidProgramException for debug builds

**Link:** https://github.com/dotnet/fsharp/issues/18956

**Category:** Invalid IL (InvalidProgramException)

### Minimal Repro

```fsharp
module A =
    [<Literal>]
    let B = 42m

[<EntryPoint>]
let main args = 0
```

Compile with: `dotnet run` (Debug configuration)

### Expected Behavior
Program compiles and runs successfully.

### Actual Behavior
`System.InvalidProgramException: Common Language Runtime detected an invalid program.`

Works in Release configuration.

### Test Location
`CodeGenRegressions.fs` → `Issue_18956_DecimalConstantInvalidProgram`

### Analysis
Debug builds emit `.locals init` with different maxstack compared to Release. The decimal constant initialization generates IL that's invalid in Debug mode due to incorrect local variable handling.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - decimal literal codegen in debug mode

### Risks
- Low: Fix should align Debug and Release codegen for decimal literals

---

## Issue #18953

**Title:** Implicit Action/Func conversion captures extra expressions

**Link:** https://github.com/dotnet/fsharp/issues/18953

**Category:** Wrong Runtime Behavior

### Minimal Repro

```fsharp
let x (f: System.Action<int>) =
    f.Invoke 99
    f.Invoke 98

let y () =
    printfn "one time"
    fun num -> printfn "%d" num

x (y ())  // Prints "one time" TWICE instead of once
```

### Expected Behavior
`y()` is called once, the resulting function is converted to Action, and that Action is invoked twice.

### Actual Behavior
`y()` is called twice because `x (y ())` expands to `x (Action (fun num -> y () num))`.

### Test Location
`CodeGenRegressions.fs` → `Issue_18953_ActionFuncCapturesExtraExpressions`

### Analysis
The implicit conversion from F# function to delegate re-captures the entire expression instead of capturing the result.

### Fix Location
- `src/Compiler/Checking/TypeRelations.fs` or `src/Compiler/Checking/CheckExpressions.fs` - implicit delegate conversion

### Risks
- Medium: Changing conversion semantics could affect existing code relying on current (buggy) behavior

---

## Issue #18868

**Title:** Error using [<CallerFilePath>] with caller info in delegates

**Link:** https://github.com/dotnet/fsharp/issues/18868

**Category:** Compile Error

### Minimal Repro

```fsharp
type A = delegate of [<System.Runtime.CompilerServices.CallerFilePath>] a: string -> unit
```

### Expected Behavior
Delegate type compiles successfully with caller info attribute.

### Actual Behavior
```
error FS1246: 'CallerFilePath' must be applied to an argument of type 'string', but has been applied to an argument of type 'string'
```

### Test Location
`CodeGenRegressions.fs` → `Issue_18868_CallerInfoInDelegates`

### Analysis
The error message is self-contradictory. The caller info handling in delegate definitions is broken.

### Fix Location
- `src/Compiler/Checking/CheckDeclarations.fs` - caller info attribute handling for delegates

### Risks
- Low: Fix should properly handle caller info attributes in delegate definitions
- Workaround exists: use `?a: string` instead

---

## Issue #18815

**Title:** Can't define extensions for two same named types in a single module

**Link:** https://github.com/dotnet/fsharp/issues/18815

**Category:** Compile Error (FS2014)

### Minimal Repro

```fsharp
module Compiled

type Task = { F: int }

module CompiledExtensions =
    type System.Threading.Tasks.Task with
        static member CompiledStaticExtension() = ()

    type Task with
        static member CompiledStaticExtension() = ()
```

### Expected Behavior
Both extensions compile - they extend different types.

### Actual Behavior
```
Error FS2014: duplicate entry 'Task.CompiledStaticExtension.Static' in method table
```

### Test Location
`CodeGenRegressions.fs` → `Issue_18815_DuplicateExtensionMethodNames`

### Analysis
The extension method naming scheme uses the simple type name without qualification, causing collision.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - extension method naming/mangling

### Risks
- Low: Fix should use fully qualified type name in extension method table key

---

## Issue #18753

**Title:** Inlining in CEs is prevented by DU constructor in the CE block

**Link:** https://github.com/dotnet/fsharp/issues/18753

**Category:** Optimization Issue

### Minimal Repro

```fsharp
type IntOrString = I of int | S of string

// With InlineIfLambda CE builder...

let test1 () = builder { 1; "two"; 3 }      // Fully inlined
let test2 () = builder { I 1; "two"; 3 }    // Lambdas generated - not inlined
```

### Expected Behavior
Both `test1` and `test2` produce equivalent, fully inlined code.

### Actual Behavior
`test2` generates closure classes and lambda invocations instead of inlined code.

### Test Location
`CodeGenRegressions.fs` → `Issue_18753_CEInliningPreventedByDU`

### Analysis
The presence of a DU constructor in the CE block prevents the optimizer from inlining subsequent yields.

### Fix Location
- `src/Compiler/Optimize/Optimizer.fs` - InlineIfLambda handling with DU constructors

### Risks
- Low: Fix should improve inlining heuristics
- Workaround exists: construct DU values outside the CE block

---

## Issue #18672

**Title:** Resumable code: CE created as top level value does not work

**Link:** https://github.com/dotnet/fsharp/issues/18672

**Category:** Wrong Runtime Behavior

### Minimal Repro

```fsharp
// Using custom resumable code CE builder...

let testFailing = sync {
    return "result"
}

testFailing.Run() // Returns null in Release, works in Debug
```

### Expected Behavior
Top-level CE values work the same in Debug and Release.

### Actual Behavior
Top-level CE values return null in Release mode.

### Test Location
`CodeGenRegressions.fs` → `Issue_18672_ResumableCodeTopLevelValue`

### Analysis
The state machine initialization for top-level values differs between Debug and Release, with Release failing to properly initialize the state machine data.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` or state machine compilation code

### Risks
- Medium: Resumable code/state machine compilation is complex
- Workaround exists: wrap in a class member

---

## Issue #18374

**Title:** RuntimeWrappedException cannot be caught

**Link:** https://github.com/dotnet/fsharp/issues/18374

**Category:** Wrong Runtime Behavior

### Minimal Repro

```fsharp
// When non-Exception object is thrown (via CIL or other languages):
try
    throwNonException "test"  // Throws a string
with
| e -> printf "%O" e  // InvalidCastException instead of catching
```

### Expected Behavior
Non-Exception objects should be caught, either directly or wrapped in RuntimeWrappedException.

### Actual Behavior
InvalidCastException because generated IL does:
```
catch [netstandard]System.Object { castclass [System.Runtime]System.Exception ... }
```

### Test Location
`CodeGenRegressions.fs` → `Issue_18374_RuntimeWrappedExceptionCannotBeCaught`

### Analysis
F# catches `System.Object` but then unconditionally casts to `Exception`, which fails for non-Exception objects.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - exception handler codegen

### Risks
- Low: Fix should check type before casting or use RuntimeWrappedException
- Workaround exists: `[<assembly: RuntimeCompatibility(WrapNonExceptionThrows=true)>]`

---

## Contributing

To fix one of these issues:

1. Uncomment the `[<Fact>]` attribute on the corresponding test
2. Run the test to confirm it fails as documented
3. Implement the fix in the identified location
4. Run the test to confirm it passes
5. Run the full test suite to check for regressions
6. Update this document to mark the issue as fixed

When adding new regression tests:

1. Add the test to `CodeGenRegressions.fs` with commented `// [<Fact>]`
2. Add documentation to this file following the template above
3. Update the summary table
