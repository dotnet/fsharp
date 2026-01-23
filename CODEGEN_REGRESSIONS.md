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
| [#18319](#issue-18319) | Literal upcast missing box instruction | Invalid IL | IlxGen.fs | Low |
| [#18263](#issue-18263) | DU .Is* properties duplicate method | Compile Error | IlxGen.fs | Medium |
| [#18140](#issue-18140) | Callvirt on value type ILVerify error | Invalid IL | IlxGen.fs | Low |
| [#18135](#issue-18135) | Static abstract with byref params error | Compile Error | ilwrite.fs | Low |
| [#18125](#issue-18125) | Wrong StructLayoutAttribute.Size for struct unions | Incorrect Metadata | IlxGen.fs | Low |
| [#17692](#issue-17692) | Mutual recursion duplicate param name | Invalid IL | IlxGen.fs | Low |
| [#17641](#issue-17641) | IsMethod/IsProperty incorrect for generated | API Issue | Symbols.fs | Low |
| [#16565](#issue-16565) | DefaultAugmentation(false) duplicate entry | Compile Error | IlxGen.fs | Low |
| [#16546](#issue-16546) | Debug build recursive reference null | Wrong Behavior | IlxGen.fs | Medium |
| [#16378](#issue-16378) | DU logging allocations | Performance | IlxGen.fs | Low |
| [#16362](#issue-16362) | Extension methods generate C# incompatible names | C# Interop | IlxGen.fs | Low |
| [#16292](#issue-16292) | Debug SRTP mutable struct incorrect codegen | Wrong Behavior | IlxGen.fs | Medium |
| [#16245](#issue-16245) | Span IL gen produces 2 get_Item calls | Performance | IlxGen.fs | Low |
| [#16037](#issue-16037) | Tuple pattern in lambda suboptimal | Performance | Optimizer.fs | Low |
| [#15627](#issue-15627) | Async before EntryPoint hangs program | Wrong Behavior | IlxGen.fs | Medium |
| [#15467](#issue-15467) | Include language version in metadata | Feature Request | ilwrite.fs | Low |
| [#15352](#issue-15352) | User code gets CompilerGeneratedAttribute | Incorrect Attribute | IlxGen.fs | Low |
| [#15326](#issue-15326) | InlineIfLambda delegates not inlined | Optimization | Optimizer.fs | Low |
| [#15092](#issue-15092) | DebuggerProxies in release builds | Feature Request | IlxGen.fs | Low |
| [#14712](#issue-14712) | Signature generation uses System.Int32 | Cosmetic | NicePrint.fs | Low |

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

## Issue #18319

**Title:** Non-null constant literal of less-specific type generates invalid IL

**Link:** https://github.com/dotnet/fsharp/issues/18319

**Category:** Invalid IL (InvalidProgramException)

### Minimal Repro

```fsharp
[<Literal>]
let badobj: System.ValueType = 1

System.Console.WriteLine(badobj)
```

### Expected Behavior
The code should either compile and print "1", or fail to compile if the upcast is not a valid constant expression.

### Actual Behavior
Compiles without warnings but generates invalid IL:
```cil
ldc.i4.1
call void [System.Console]System.Console::WriteLine(object)
```
The `box` instruction is missing, causing `System.InvalidProgramException` at runtime.

### Test Location
`CodeGenRegressions.fs` → `Issue_18319_LiteralUpcastMissingBox`

### Analysis
When a literal value is assigned to a variable of a less-specific type (e.g., `int` to `ValueType`), the compiler stores only the underlying constant value in metadata without recording the upcast. When the literal is used, the box instruction needed for the upcast is not emitted.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - literal value emission needs to check for type mismatches and emit appropriate boxing

### Risks
- Low: Fix should emit box instruction when literal type differs from declared type
- Alternative: Disallow such upcasts in literal expressions at compile time

---

## Issue #18263

**Title:** DU .Is* properties causing compile time error

**Link:** https://github.com/dotnet/fsharp/issues/18263

**Category:** Compile Error (FS2014)

### Minimal Repro

```fsharp
type Foo = 
| SZ
| STZ
| ZS
| ASZ
```

### Expected Behavior
The DU compiles successfully with `.IsSZ`, `.IsSTZ`, `.IsZS`, `.IsASZ` properties.

### Actual Behavior
```
Error FS2014: duplicate entry 'get_IsSZ' in method table
```

### Test Location
`CodeGenRegressions.fs` → `Issue_18263_DUIsPropertiesDuplicateMethod`

### Analysis
F# 9 generates `.Is*` properties for each DU case. The normalization logic for generating property names from case names creates collisions when case names share certain prefixes (SZ and STZ both normalize to produce `IsSZ` in some code path).

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - Is* property name generation for DU cases

### Risks
- Medium: Need to ensure unique property names while maintaining backward compatibility
- Workaround: Use language version 8 or rename cases

---

## Issue #18140

**Title:** Codegen causes ilverify errors - Callvirt on value type method

**Link:** https://github.com/dotnet/fsharp/issues/18140

**Category:** Invalid IL (ILVerify Error)

### Minimal Repro

```fsharp
// Pattern that triggers callvirt on value type
[<Struct>]
type MyStruct =
    { Value: int }
    override this.GetHashCode() = this.Value

type MyComparer() =
    interface System.Collections.Generic.IEqualityComparer<MyStruct> with
        member _.GetHashCode(obj) = obj.GetHashCode()
```

### Expected Behavior
IL should use `constrained.` prefix before `callvirt` or use `call` for value type method invocations.

### Actual Behavior
Generated IL uses plain `callvirt` on value type method:
```
[IL]: Error [CallVirtOnValueType]: Callvirt on a value type method.
```

### Test Location
`CodeGenRegressions.fs` → `Issue_18140_CallvirtOnValueType`

### Analysis
When calling interface-implemented methods on struct types, the compiler emits `callvirt` without the `constrained.` prefix. While this may work at runtime in some cases, it violates ECMA-335 and causes ILVerify to flag it as invalid.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - method call emission for struct types

### Risks
- Low: Using proper `constrained.` prefix or `call` instruction is more correct
- Benefits: Cleaner IL that passes ILVerify

---

## Issue #18135

**Title:** Can't compile static abstract member functions with byref parameters

**Link:** https://github.com/dotnet/fsharp/issues/18135

**Category:** Compile Error (FS2014)

### Minimal Repro

```fsharp
[<Interface>]
type I =
    static abstract Foo: int inref -> int

type T =
    interface I with
        static member Foo i = i

let f<'T when 'T :> I>() =
    let x = 123
    printfn "%d" ('T.Foo &x)

f<T>()
```

### Expected Behavior
Static abstract interface members with byref parameters compile correctly.

### Actual Behavior
```
FS2014: Error in pass3 for type T, error: Error in GetMethodRefAsMethodDefIdx 
for mref = ("Program.I.Foo", "T"), error: MethodDefNotFound
```

### Test Location
`CodeGenRegressions.fs` → `Issue_18135_StaticAbstractByrefParams`

### Analysis
The metadata writer cannot locate the method definition for static abstract members when they have byref parameters. The byref modifier causes a mismatch in method signature lookup.

### Fix Location
- `src/Compiler/AbstractIL/ilwrite.fs` or `src/Compiler/CodeGen/IlxGen.fs` - method reference resolution with byref parameters

### Risks
- Low: Fix should correctly handle byref modifiers in IWSAM signatures
- Workaround: Use `Span<T>` instead of byref

---

## Issue #18125

**Title:** Wrong StructLayoutAttribute.Size for struct unions with no data fields

**Link:** https://github.com/dotnet/fsharp/issues/18125

**Category:** Incorrect Metadata

### Minimal Repro

```fsharp
[<Struct>]
type ABC = A | B | C

sizeof<ABC>  // Returns 4
// But StructLayoutAttribute.Size = 1
```

### Expected Behavior
`StructLayoutAttribute.Size` should be >= actual size (4 bytes for the `_tag` field).

### Actual Behavior
Emits `[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Size = 1)]` but actual size is 4.

### Test Location
`CodeGenRegressions.fs` → `Issue_18125_WrongStructLayoutSize`

### Analysis
When calculating struct layout size for unions with no data fields, the compiler only considers user-defined fields (none) and sets Size=1. It doesn't account for the compiler-generated `_tag` field.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - struct union layout size calculation

### Risks
- Low: Fix should include `_tag` field size in layout calculation
- Marked as "good first issue" - straightforward fix

---

## Issue #17692

**Title:** Mutual recursion codegen issue with duplicate param names

**Link:** https://github.com/dotnet/fsharp/issues/17692

**Category:** Invalid IL (ilasm warning)

### Minimal Repro

```fsharp
// Complex mutual recursion with closures generates IL with duplicate 'self@' param names
let rec caller x = callee (x - 1)
and callee y = if y > 0 then caller y else 0
```

### Expected Behavior
Generated IL should have unique parameter names in all methods.

### Actual Behavior
ilasm reports warnings:
```
warning : Duplicate param name 'self@' in method '.ctor'
```

### Test Location
`CodeGenRegressions.fs` → `Issue_17692_MutualRecursionDuplicateParamName`

### Analysis
When generating closure classes for mutually recursive functions, the compiler reuses the `self@` parameter name in constructors, causing duplicates. This is caught by ilasm during IL round-tripping.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - closure/class generation for mutual recursion

### Risks
- Low: Parameter names should be uniquified
- Marked as regression - was working before

---

## Issue #17641

**Title:** IsMethod and IsProperty don't act as expected for generated methods/properties

**Link:** https://github.com/dotnet/fsharp/issues/17641

**Category:** API/Metadata Issue

### Minimal Repro

```fsharp
type MyUnion = | CaseA of int | CaseB of string

// When inspecting MyUnion via FSharpImplementationFileDeclaration:
// - get_IsCaseA should have IsProperty = true
// - Equals should have IsMethod = true
// But both show incorrect values when enumerated from assembly contents
```

### Expected Behavior
Generated properties (like `.IsCaseA`) have `IsProperty = true`, generated methods (like `Equals`) have `IsMethod = true`.

### Actual Behavior
When enumerating declarations via `FSharpAssemblyContents.ImplementationFiles`, generated properties/methods have incorrect `IsProperty`/`IsMethod` flags. Using `GetSymbolUseAtLocation` returns correct values.

### Test Location
`CodeGenRegressions.fs` → `Issue_17641_IsMethodIsPropertyIncorrectForGenerated`

### Analysis
The FCS API constructs FSharpMemberOrFunctionOrValue differently depending on access path. Generated members from assembly contents enumeration don't have their member kind properly set.

### Fix Location
- `src/Compiler/Symbols/Symbols.fs` - FSharpMemberOrFunctionOrValue construction for generated members

### Risks
- Low: This is a metadata/API issue affecting tooling
- Breaking change risk minimal as it's fixing incorrect behavior

---

## Issue #16565

**Title:** Codegen issue with DefaultAugmentation(false)

**Link:** https://github.com/dotnet/fsharp/issues/16565

**Category:** Compile Error (FS2014)

### Minimal Repro

```fsharp
open System

[<DefaultAugmentation(false)>]
type Option<'T> =
    | Some of Value: 'T
    | None

    member x.Value =
        match x with
        | Some x -> x
        | None -> raise (new InvalidOperationException("Option.Value"))

    static member None : Option<'T> = None

and 'T option = Option<'T>
```

### Expected Behavior
Compiles successfully - the static member `None` should shadow or coexist with the union case.

### Actual Behavior
```
Error FS2014: duplicate entry 'get_None' in method table
```

### Test Location
`CodeGenRegressions.fs` → `Issue_16565_DefaultAugmentationFalseDuplicateEntry`

### Analysis
With `DefaultAugmentation(false)`, F# shouldn't generate default `.None` property. But when user defines `static member None`, there's still a collision with some generated code.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - method table generation with DefaultAugmentation

### Risks
- Low: Should properly respect DefaultAugmentation attribute
- Note: This pattern is used in FSharp.Core for FSharpOption

---

## Issue #16546

**Title:** NullReferenceException with Debug build and recursive reference

**Link:** https://github.com/dotnet/fsharp/issues/16546

**Category:** Wrong Runtime Behavior (Debug only)

### Minimal Repro

```fsharp
type Type = | TPrim of string | TParam of string * Type

let tryParam pv x =
    match x with
    | TParam (name, typ) -> pv typ |> Result.map (fun t -> [name, t])
    | _ -> Error "unexpected"

module TypeModule =
    let parse =
        let rec paramParse = tryParam parse  // Reference to 'parse' before definition
        and parse node =
            match node with
            | TPrim name -> Ok(TPrim name)
            | _ -> match paramParse node with
                   | Ok [name, typ] -> Ok(TParam(name,typ))
                   | _ -> Error "invalid"
        parse

TypeModule.parse (TParam ("ptr", TPrim "float"))
```

### Expected Behavior
Works in both Debug and Release builds.

### Actual Behavior
- Release: Works correctly
- Debug: NullReferenceException because `parse` is null when `paramParse` is initialized

### Test Location
`CodeGenRegressions.fs` → `Issue_16546_DebugRecursiveReferenceNull`

### Analysis
In Debug mode, the initialization order of mutually recursive bindings differs from Release. When `paramParse` captures `parse`, it captures null in Debug mode.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - recursive binding initialization order in Debug mode

### Risks
- Medium: Changing initialization order could affect other mutual recursion scenarios
- Workaround: Reorder bindings so referenced binding comes first

---

## Issue #16378

**Title:** Significant allocations writing F# types using Console.Logger

**Link:** https://github.com/dotnet/fsharp/issues/16378

**Category:** Performance/Allocation Issue

### Minimal Repro

```fsharp
type StoreError =
    | NotFound of Guid
    | AlreadyExists of Guid

let error = NotFound(Guid.NewGuid())

// High allocation path (~36KB per call):
logger.LogError("Error: {Error}", error)

// Low allocation path (~1.8KB per call):
logger.LogError("Error: {Error}", error.ToString())
```

### Expected Behavior
Logging F# DU values should have comparable allocation to logging their string representation.

### Actual Behavior
Direct DU logging allocates ~20x more memory due to excessive boxing and reflection during formatting.

### Test Location
`CodeGenRegressions.fs` → `Issue_16378_DULoggingAllocations`

### Analysis
When F# DU values are boxed for logging methods that accept `obj`, the runtime uses reflection to format them, causing many intermediate allocations. The generated `ToString()` for DUs also has suboptimal allocation behavior.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - DU ToString generation
- `src/FSharp.Core/prim-types.fs` - structural formatting

### Risks
- Low: Performance improvement with no semantic change
- May require changes to how DUs implement IFormattable or similar

---

## Issue #16362

**Title:** Extension methods with CompiledName generate C# incompatible names

**Link:** https://github.com/dotnet/fsharp/issues/16362

**Category:** C# Interop Issue

### Minimal Repro

```fsharp
type Exception with
    member ex.Reraise() = raise ex
```

### Expected Behavior
Extension method generates a C#-compatible name, or emits a warning suggesting to use `[<CompiledName>]`.

### Actual Behavior
Generated extension method name is "Exception.Reraise" which contains a dot - not valid C# syntax and doesn't appear in C# autocomplete.

### Test Location
`CodeGenRegressions.fs` → `Issue_16362_ExtensionMethodCompiledName`

### Analysis
F# style extension methods generate compiled names using the pattern `TypeName.MethodName`, but the dot character is not valid in C# method identifiers, breaking interop.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - extension method naming

### Risks
- Low: Could generate different names or emit a warning
- Workaround exists: use `[<CompiledName>]` attribute

---

## Issue #16292

**Title:** Incorrect codegen for Debug build with SRTP and mutable struct

**Link:** https://github.com/dotnet/fsharp/issues/16292

**Category:** Wrong Runtime Behavior (Debug only)

### Minimal Repro

```fsharp
let inline forEach<'C, 'E, 'I
                        when 'C: (member GetEnumerator: unit -> 'I)
                        and  'I: struct
                        and  'I: (member MoveNext: unit -> bool)
                        and  'I: (member Current : 'E) >
        ([<InlineIfLambda>] f: 'E -> unit) (container: 'C) =
    let mutable iter = container.GetEnumerator()
    while iter.MoveNext() do
        f iter.Current

let showIt (buffer: ReadOnlySequence<byte>) =
    buffer |> forEach (fun segment -> ())
```

### Expected Behavior
Both Debug and Release builds iterate correctly.

### Actual Behavior
In Debug builds, the struct enumerator is copied in each loop iteration, so `MoveNext()` mutates the copy instead of the original. The loop either hangs (infinite) or produces wrong results.

### Test Location
`CodeGenRegressions.fs` → `Issue_16292_SrtpDebugMutableStructEnumerator`

### Analysis
The Debug codegen creates an additional local for the enumerator and reinitializes it each iteration from the original (unmutated) copy. This pattern is common with `ReadOnlySequence<T>` and other BCL types.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - mutable struct SRTP codegen in debug mode

### Risks
- Medium: Debug/Release behavior difference is critical
- Workaround: Use Release mode or avoid SRTP with mutable structs

---

## Issue #16245

**Title:** Span IL gen produces 2 get_Item calls

**Link:** https://github.com/dotnet/fsharp/issues/16245

**Category:** Performance (Suboptimal IL)

### Minimal Repro

```fsharp
let incrementSpan (span: Span<byte>) =
    for i = 0 to span.Length - 1 do
        span[i] <- span[i] + 1uy
```

### Expected Behavior
Single `get_Item` call, add, single `set_Item` call.

### Actual Behavior
Two `System.Span`1::get_Item(int32)` method calls are generated.

### Test Location
`CodeGenRegressions.fs` → `Issue_16245_SpanDoubleGetItem`

### Analysis
When incrementing a span element, the compiler reads the element once for the right side of the assignment and once for the address to store to. Should instead use a single ref and modify in place.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - Span indexer codegen

### Risks
- Low: Performance improvement only
- Workaround: Use `incv &span[i]` pattern

---

## Issue #16037

**Title:** Suboptimal code generated when pattern matching tuple in lambda parameter

**Link:** https://github.com/dotnet/fsharp/issues/16037

**Category:** Performance (Extra Allocations)

### Minimal Repro

```fsharp
let data = Map.ofList [ "1", (true, 1) ]

// Suboptimal - 2 FSharpFunc classes, 136 bytes allocated
let foldWithPattern () =
    (data, []) ||> Map.foldBack (fun _ (x, _) state -> x :: state)

// Optimal - 1 FSharpFunc class, 64 bytes allocated
let foldWithFst () =
    (data, []) ||> Map.foldBack (fun _ v state -> fst v :: state)
```

### Expected Behavior
Both patterns should generate equivalent code with similar allocation.

### Actual Behavior
Pattern matching in lambda parameter generates extra closure classes, ~50% slower with 2x memory allocation.

### Test Location
`CodeGenRegressions.fs` → `Issue_16037_TuplePatternLambdaSuboptimal`

### Analysis
When pattern matching in a lambda parameter, the compiler generates an intermediate wrapper function instead of direct tuple element access.

### Fix Location
- `src/Compiler/Optimize/Optimizer.fs` - lambda pattern optimization

### Risks
- Low: Pure optimization, no semantic change
- Workaround: Use `fst`/`snd` or match in function body

---

## Issue #15627

**Title:** Program stuck when using async/task before EntryPoint

**Link:** https://github.com/dotnet/fsharp/issues/15627

**Category:** Wrong Runtime Behavior (Hang)

### Minimal Repro

```fsharp
open System.IO

let deployPath = Path.GetFullPath "deploy"

printfn "1"

async {
     printfn "2 %s" deployPath
}
|> Async.RunSynchronously

[<EntryPoint>]
let main args =
    printfn "3"
    0
```

### Expected Behavior
Prints 1, 2, 3 and exits.

### Actual Behavior
Prints 1, then hangs indefinitely. The async never completes.

### Test Location
`CodeGenRegressions.fs` → `Issue_15627_AsyncBeforeEntryPointHangs`

### Analysis
When there's an `[<EntryPoint>]` function, module-level initialization including async operations may deadlock due to module initialization ordering and threading issues.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - module initialization with EntryPoint

### Risks
- Medium: Module initialization ordering is complex
- Workaround: Move async inside EntryPoint or remove EntryPoint

---

## Issue #15467

**Title:** Include info about language version into compiled metadata

**Link:** https://github.com/dotnet/fsharp/issues/15467

**Category:** Feature Request (Metadata)

### Minimal Repro

N/A - This is a feature request, not a bug.

### Expected Behavior
Compiled F# assemblies should include the F# language version in metadata. When an older compiler encounters a newer DLL, it can give a specific error like "Tooling must support F# 7 or higher".

### Actual Behavior
Generic pickle errors that don't tell the user what action to take.

### Test Location
`CodeGenRegressions.fs` → `Issue_15467_LanguageVersionInMetadata`

### Analysis
Each F# DLL should contain language version info in custom attributes or pickle format so older compilers can give actionable error messages.

### Fix Location
- `src/Compiler/AbstractIL/ilwrite.fs` - metadata emission
- `src/Compiler/TypedTree/TypedTreePickle.fs` - pickle format

### Risks
- Low: Adding metadata is backward compatible
- Improves user experience when using mixed tooling versions

---

## Issue #15352

**Title:** Some user defined symbols started to get CompilerGeneratedAttribute

**Link:** https://github.com/dotnet/fsharp/issues/15352

**Category:** Incorrect Attribute

### Minimal Repro

```fsharp
type T() =
    let f x = x + 1
```

The method `f` gets `[<CompilerGenerated>]` attribute in IL, but it's user-written code.

### Expected Behavior
User-defined methods should not have `CompilerGeneratedAttribute`.

### Actual Behavior
Private let-bound functions in classes get `CompilerGeneratedAttribute`, which is misleading for debuggers and reflection tools.

### Test Location
`CodeGenRegressions.fs` → `Issue_15352_UserCodeCompilerGeneratedAttribute`

### Analysis
The attribute is probably being added because the method is "hidden" or has internal accessibility, but the semantic is wrong - it's still user-written code.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - attribute generation for class members

### Risks
- Low: Removing incorrect attribute shouldn't break anything
- May affect debugging experience (positively - debugger will step into these)

---

## Issue #15326

**Title:** Delegates aren't getting inlined in certain cases when using InlineIfLambda

**Link:** https://github.com/dotnet/fsharp/issues/15326

**Category:** Optimization Regression

### Minimal Repro

```fsharp
type SystemAction<'a when 'a: struct and 'a :> IConvertible> = 
    delegate of byref<'a> -> unit

let inline doAction (span: Span<'a>) ([<InlineIfLambda>] action: SystemAction<'a>) =
    for i = 0 to span.Length - 1 do
        let batch = &span[i]
        action.Invoke &batch

doAction (array.AsSpan()) (SystemAction(fun batch -> batch <- batch + 1))
```

### Expected Behavior
The delegate should be inlined as it was in .NET 7 Preview 4.

### Actual Behavior
The delegate is not inlined in .NET 7 Preview 5 and later - a closure is generated.

### Test Location
`CodeGenRegressions.fs` → `Issue_15326_InlineIfLambdaDelegateRegression`

### Analysis
Regression introduced between Preview 4 and Preview 5. The `InlineIfLambda` attribute is not being respected for custom delegates with certain constraints.

### Fix Location
- `src/Compiler/Optimize/Optimizer.fs` - InlineIfLambda handling

### Risks
- Low: Restoring previous behavior
- Marked as regression - fix should be straightforward

---

## Issue #15092

**Title:** Should we generate DebuggerProxies in release code?

**Link:** https://github.com/dotnet/fsharp/issues/15092

**Category:** Feature Request (Binary Size)

### Minimal Repro

N/A - This is a design question about whether DebuggerProxy types should be elided in release builds.

### Expected Behavior
Consider not generating DebuggerProxy types in release builds to reduce binary size.

### Actual Behavior
DebuggerProxy types are always generated, even in optimized release builds where debugging is less common.

### Test Location
`CodeGenRegressions.fs` → `Issue_15092_DebuggerProxiesInRelease`

### Analysis
F# generates helper types for better debugging experience. In release builds, these add to binary size but may not be needed if the assembly won't be debugged.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - DebuggerProxy generation

### Risks
- Low: Could be opt-in via compiler flag
- Trade-off between binary size and production debugging capability

---

## Issue #14712

**Title:** Signature file generation should use F# Core alias

**Link:** https://github.com/dotnet/fsharp/issues/14712

**Category:** Cosmetic (Signature Files)

### Minimal Repro

```fsharp
type System.Int32 with
    member i.PlusPlus () = i + 1
    member i.PlusPlusPlus () : int = i + 1 + 1
```

Generates signature with `System.Int32` for `PlusPlus` but `int` for `PlusPlusPlus`.

### Expected Behavior
Generated signature files should consistently use F# type aliases (`int`, `string`, etc.) instead of BCL names (`System.Int32`, `System.String`).

### Actual Behavior
Inferred types use BCL names, explicit type annotations use F# aliases.

### Test Location
`CodeGenRegressions.fs` → `Issue_14712_SignatureFileTypeAlias`

### Analysis
The signature file generator doesn't normalize types to their F# aliases when printing inferred types.

### Fix Location
- `src/Compiler/Checking/NicePrint.fs` or signature file generation

### Risks
- Low: Cosmetic change only
- Workaround: Always add explicit type annotations

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
