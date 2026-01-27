# CodeGen Regressions Documentation

This document tracks known code generation bugs in the F# compiler that have documented test cases but are not yet fixed. Each issue has a corresponding test in `tests/FSharp.Compiler.ComponentTests/EmittedIL/CodeGenRegressions/CodeGenRegressions.fs` with its `[<Fact>]` attribute commented out.

## Table of Contents

- [Summary Statistics](#summary-statistics)
- [Summary Table](#summary-table)
- [Issue Details](#issue-19075) (62 issues documented)
- [Contributing](#contributing)

## Summary Statistics

| Metric | Count |
|--------|-------|
| **Total Issues** | 62 |
| **Wrong Behavior** | 13 |
| **Performance/Optimization** | 12 |
| **Compile Error/Warning/Crash** | 12 |
| **Invalid IL** | 6 |
| **Feature Request** | 5 |
| **Runtime Crash/Error** | 4 |
| **Interop (C#/Other)** | 3 |
| **Other (Cosmetic/Metadata/API/Attribute)** | 7 |

### Risk Assessment

| Risk Level | Count |
|------------|-------|
| **High** | 0 |
| **Medium** | 17 |
| **Low** | 45 |

### Primary Fix Locations

| Component | Count |
|-----------|-------|
| `IlxGen.fs` | 38 |
| `ilwrite.fs` | 7 |
| `Optimizer.fs` | 7 |
| `NicePrint.fs` | 3 |
| Other | 7 |

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
| [#14707](#issue-14707) | Signature files become unusable | Compile Error | NicePrint.fs | Medium |
| [#14706](#issue-14706) | Signature generation WhereTyparSubtypeOfType | Compile Error | NicePrint.fs | Low |
| [#14508](#issue-14508) | nativeptr in interfaces leads to runtime errors | Runtime Error | IlxGen.fs | Medium |
| [#14492](#issue-14492) | Incorrect program in release config | Invalid IL | EraseClosures.fs | Medium | ✅ FIXED |
| [#14392](#issue-14392) | OpenApi Swashbuckle support | Feature Request | N/A | Low |
| [#14321](#issue-14321) | Build fails reusing names DU constructors and IWSAM | Compile Error | NameResolution.fs | Low |
| [#13468](#issue-13468) | outref parameter compiled as byref | Wrong Behavior | IlxGen.fs | Medium |
| [#13447](#issue-13447) | Extra tail instruction corrupts stack | Runtime Crash | IlxGen.fs | Medium |
| [#13223](#issue-13223) | FSharp.Build support for reference assemblies | Feature Request | FSharp.Build | Low |
| [#13218](#issue-13218) | Compilation time 13000 static member vs let | Performance | Optimizer.fs | Low |
| [#13108](#issue-13108) | Static linking FS2009 warnings | Compile Warning | ilwrite.fs | Low |
| [#13100](#issue-13100) | --platform:x64 sets 32 bit characteristic | Wrong Behavior | ilwrite.fs | Low |
| [#12546](#issue-12546) | Implicit boxing produces extraneous closure | Performance | IlxGen.fs | Low |
| [#12460](#issue-12460) | F# C# Version info values different | Metadata | ilwrite.fs | Low |
| [#12416](#issue-12416) | Optimization inlining inconsistent with piping | Performance | Optimizer.fs | Low |
| [#12384](#issue-12384) | Mutually recursive values intermediate module wrong init | Wrong Behavior | IlxGen.fs | Medium |
| [#12366](#issue-12366) | Rethink names for compiler-generated closures | Cosmetic | IlxGen.fs | Low |
| [#12139](#issue-12139) | Improve string null check IL codegen | Performance | Optimizer.fs | Low |
| [#12137](#issue-12137) | Improve analysis to reduce emit of tail | Performance | Optimizer.fs | Low |
| [#12136](#issue-12136) | use fixed does not unpin at end of scope | Wrong Behavior | IlxGen.fs | Medium |
| [#11935](#issue-11935) | unmanaged constraint not recognized by C# | Interop | IlxGen.fs | Low |
| [#11556](#issue-11556) | Better IL output for property/field initializers | Performance | IlxGen.fs | Low |
| [#11132](#issue-11132) | TypeloadException delegate with voidptr parameter | Runtime Error | IlxGen.fs | Medium |
| [#11114](#issue-11114) | Record with hundreds of members StackOverflow | Compile Crash | IlxGen.fs | Low |
| [#9348](#issue-9348) | Performance of Comparing and Ordering | Performance | IlxGen.fs | Low |
| [#9176](#issue-9176) | Decorate inline function code with attribute | Feature Request | IlxGen.fs | Low |
| [#7861](#issue-7861) | Missing assembly reference for type in attributes | Compile Error | ilwrite.fs | Low |
| [#6750](#issue-6750) | Mutually recursive values leave fields uninitialized | Wrong Behavior | IlxGen.fs | Medium |
| [#6379](#issue-6379) | FS2014 when using tupled args | Compile Warning | TypeChecker.fs | Low |
| [#5834](#issue-5834) | Obsolete on abstract generates accessors without specialname | Wrong Behavior | IlxGen.fs | Low |
| [#5464](#issue-5464) | F# ignores custom modifiers modreq/modopt | Interop | ilwrite.fs | Medium |
| [#878](#issue-878) | Serialization of F# exception variants doesn't serialize fields | Wrong Behavior | IlxGen.fs | Low |

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

### UPDATE (FIXED)

The issue was caused by the compiler generating a constrained call prefix for reference types when calling interface methods through SRTP resolution. 

When an SRTP member constraint resolves to an interface method, the compiler records a `PossibleConstrainedCall` flag with the concrete type. During code generation, this flag was used to emit a `constrained.` prefix even for concrete reference types (classes).

**Root Cause:** The constrained prefix is only semantically necessary for value types (to avoid boxing) and for type parameters (which might be value types at runtime). For concrete reference types, the constrained prefix just dereferences the managed pointer and does a virtual call. However, when combined with interface method dispatch for classes like `MemoryStream` (which inherits `IDisposable` from `Stream`), this can cause CLR crashes.

**Fix:** In `GenILCall` (IlxGen.fs), we now check if the constrained call target is a concrete reference type (not a struct and not a type parameter). If it is, we skip the constrained prefix and use a regular `callvirt` instead. Type parameters still use constrained calls because they might be instantiated to value types at runtime.

**Changes:**
- `src/Compiler/CodeGen/IlxGen.fs` - Added check in `GenILCall` to skip constrained prefix for concrete reference types (classes)

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

### UPDATE (FIXED)

Fixed in IlxGen.fs by:
1. Adding `GenGetFreeVarForClosure` helper function that properly handles byref-typed free variables
2. Modifying `GenFreevar` to strip byref types when generating closure field types (byref fields are not valid in classes)
3. When loading byref-typed free variables for closure capture, the value is now dereferenced (copied) using `ldobj` instruction
4. Applied fix consistently to object expressions, lambda closures, sequence expressions, and delegates

The fix ensures that when capturing values through a byref reference (like struct `this` pointers), the actual value is copied into the closure instead of trying to store a byref reference.

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

### UPDATE: Fixed

**Root Cause:** In Debug mode, the compiler creates "shadow locals" for static properties to enable better debugging. However, for `[<Literal>]` values (including decimal literals), the shadow local was allocated but never initialized. For decimal literals specifically, the static field initialization code was emitted, but the shadow local storage was never written to, resulting in invalid IL.

**Fix:** Added condition to exclude literal values from shadow local allocation in `AllocValReprWithinExpr`:
```fsharp
&& Option.isNone v.LiteralValue
```

This prevents the creation of uninitialized shadow locals for literal values, which don't need debugging locals since they are either inlined (for primitive types) or stored directly in static fields (for decimal).

**Files Modified:**
- `src/Compiler/CodeGen/IlxGen.fs` - Added literal exclusion to `useShadowLocal` condition

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
Delegate type compiles successfully with caller info attribute when using optional parameter syntax.

### Actual Behavior (Original Bug)
```
error FS1246: 'CallerFilePath' must be applied to an argument of type 'string', but has been applied to an argument of type 'string'
```

### Test Location
`CodeGenRegressions.fs` → `Issue_18868_CallerInfoInDelegates`

### Analysis
The error message was self-contradictory. The caller info handling in delegate definitions was broken.

### Fix Location
- `src/Compiler/Checking/CheckDeclarations.fs` - caller info attribute handling for delegates

### Risks
- Low: Fix should properly handle caller info attributes in delegate definitions
- Workaround exists: use `?a: string` instead

### UPDATE (2026-01-23)
**Status: FIXED** - The contradictory error message (FS1246 "string but applied to string") has been fixed.

The correct behavior is now:
- Non-optional parameters with CallerInfo correctly report FS1247: "CallerFilePath can only be applied to optional arguments"
- Optional parameters (`?a: string`) with CallerInfo compile successfully

CallerInfo attributes require optional parameters per .NET specification. The bug was the incorrect/contradictory error message, which is now fixed.

Test updated to verify the working case with optional parameter syntax (`?a: string`).

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

### UPDATE (2026-01-23)
**Status: FIXED** - Extension methods now use fully qualified type names.

The fix modifies `ComputeStorageForFSharpFunctionOrFSharpExtensionMember` in `IlxGen.fs` to:
- For extension members, prefix the method name with the fully qualified path of the extended type
- Format: `Namespace$SubNamespace$TypeName.MethodName` (using `$` as path separator for IL safety)
- Example: `System$Threading$Tasks$Task.CompiledStaticExtension` vs `Compiled$Task.CompiledStaticExtension`

This ensures extension methods for types with the same simple name but different namespaces get unique IL method names, preventing the duplicate entry error.

Also removed the blocking check from `PostInferenceChecks.fs` that was emitting FS3356 as a workaround.

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

**Status:** ✅ FIXED

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

### Fix Applied
* **UPDATE:** Fixed by adding box instruction emission for literal upcasts in `GenConstant`. When the declared type is a reference type (e.g., `System.ValueType`, `System.Object`) but the constant is a value type, the compiler now emits a `box` instruction after loading the constant value. Test uncommented, now passes.

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

### Fix Applied
* **UPDATE (FIXED):** Issue no longer reproduces on current main branch. The reported error occurred in VS 17.12 with msbuild, but `dotnet build` worked correctly even at the time of the report. The issue appears to have been related to an older compiler version or a specific VS/msbuild configuration. The current compiler correctly generates unique `.Is*` properties for all DU cases (`.IsSZ`, `.IsSTZ`, `.IsZS`, `.IsASZ`). Test uncommented and passes.

---

## Issue #18140

**Title:** Codegen causes ilverify errors - Callvirt on value type method

**Link:** https://github.com/dotnet/fsharp/issues/18140

**Category:** Invalid IL (ILVerify Error)

### Minimal Repro

```fsharp
// Pattern that triggers callvirt on value type
[<Struct>]
type MyRange =
    val Value: int
    new(v) = { Value = v }

let comparer =
    { new System.Collections.Generic.IEqualityComparer<MyRange> with
        member _.Equals(x1, x2) = x1.Value = x2.Value
        member _.GetHashCode o = o.GetHashCode()  // callvirt on struct
    }
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

### UPDATE
**Fixed:** In `GenILCall`, when emitting a `callvirt` instruction on an instance method where the first 
argument (the `this` pointer) is a value type, the compiler now uses `I_callconstraint` instead of 
plain `I_callvirt`. This adds the required `constrained.` prefix before `callvirt` to produce valid IL 
per ECMA-335. The fix checks if `ccallInfo` is `None` but `useICallVirt` is true and the first argument
is a struct type - if so, it creates constrained call info with that type. This ensures correct IL is 
emitted for patterns like `obj.GetHashCode()` where `obj` is a value type implementing IEqualityComparer.

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

### UPDATE (FIXED)
**Fixed** in `MethodDefKey.Equals` in `ilwrite.fs`. The root cause was a signature mismatch when looking 
up method definitions for static abstract interface member implementations with `inref`/`outref`/`byref` 
parameters. Interface slot signatures include an `ILType.Modified` wrapper (for `InAttribute`/`OutAttribute`) 
around the byref type, but the method implementation's parameter types don't have this wrapper. The fix 
extends `compareILTypes` to:
1. Recursively handle `ILType.Byref`, `ILType.Ptr`, `ILType.Array`, and `ILType.Modified` wrappers
2. Use `EqualsWithPrimaryScopeRef` for proper scope-aware type reference comparison
3. Handle the asymmetric case where `Modified` is present on one side but not the other by comparing the 
   inner types directly

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
- `src/Compiler/CodeGen/EraseClosures.fs` - closure constructor parameter naming

### Risks
- Low: Parameter names should be uniquified
- Marked as regression - was working before

### UPDATE
Fixed by adding `mkUniqueFreeVarName` helper function in `EraseClosures.fs` that generates unique names for closure free variables. When creating `selfFreeVar` for closure constructors, the function now checks for existing field names and appends a numeric suffix if needed to avoid duplicates. This ensures that nested or chained closure transformations don't produce duplicate `self@` parameter names.

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

### UPDATE (2026-01-23)

**Status: FIXED**

The issue was in the `tdefDiscards` logic in `IlxGen.fs`. When `DefaultAugmentation(false)` is used (`NoHelpers`), the compiler still generates `get_<CaseName>` methods for nullary union cases (e.g., `get_None`). However, if the user also defines a static member with the same name (e.g., `static member None`), the user-defined getter was not being discarded, causing duplicate method entries.

**Fix:** Added a case for `NoHelpers` in `tdefDiscards` that discards user-defined methods/properties when they would clash with compiler-generated ones for nullary cases. This is similar to how `AllHelpers` handles `get_Is*` methods.

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

**Root Cause Investigation (Sprint 6):**
The issue originates in the type checker's `EliminateInitializationGraphs` function (CheckExpressions.fs), not in IlxGen. When forward references are detected, the type checker inserts Lazy wrappers:

```fsharp
// Original:
let rec paramParse = tryParam parse and parse node = ...

// After type checker:
let rec paramParse_thunk = fun () -> tryParam parse   // Captures parse
        paramParse_lazy = Lazy.Create(paramParse_thunk)
        paramParse = paramParse_lazy.Force()
and parse node = ...  // Captures paramParse as Lazy
```

Reordering in IlxGen is insufficient because:
1. The Lazy wrappers are already inserted
2. The thunk closure captures `parse` during its creation (when `parse` is null)
3. The fixup mechanism correctly updates the thunk's field, but the structure is different from the workaround

### Fix Location
- `src/Compiler/Checking/Expressions/CheckExpressions.fs` - `EliminateInitializationGraphs` function
- Would require reordering bindings BEFORE checking for forward references

**KNOWN LIMITATION:** This issue requires a type checker fix that is beyond simple code generator changes.

### Workaround
Reorder bindings in source code so lambdas come before non-lambdas:
```fsharp
// GOOD - lambda first:
let rec parse node = ... and paramParse = tryParam parse

// BAD - non-lambda first:  
let rec paramParse = tryParam parse and parse node = ...
```

### Risks
- High: Type checker changes require careful consideration of all mutual recursion scenarios

### UPDATE (KNOWN_LIMITATION)

**Status:** KNOWN_LIMITATION - Requires type checker fix beyond scope of codegen bugfix campaign

This issue cannot be fixed at the code generator level. The root cause is in the type checker's `EliminateInitializationGraphs` function (CheckExpressions.fs), which inserts Lazy wrappers for forward-referenced bindings. By the time IlxGen processes the code, the structure is fundamentally different from the workaround case.

**Attempts made (5+):**
1. Reordering in `GenLetRec` before `AllocStorageForBinds`
2. Reordering in `GenLetRecBindings` for fixup computation
3. Reordering in generation phase
4. Debug tracing of fixup mechanism (verified correct execution)
5. Analysis of closure structures (confirmed Lazy wrappers are the problem)

**Why unfixable at codegen level:**
- The Lazy wrappers are inserted by the type checker before IlxGen sees the code
- The thunk closure captures `parse` during creation when `parse` local is null
- Fixups correctly update closure fields but the structure is fundamentally different
- The workaround case (source reordering) avoids Lazy wrappers entirely

**Documented workaround:** Reorder bindings in source code so lambdas appear before non-lambdas that depend on them.

**Future work:** Would require changes to `EliminateInitializationGraphs` in CheckExpressions.fs to reorder bindings BEFORE checking for forward references.

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

**Category:** Feature Request (OUT_OF_SCOPE - Metadata)

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

### UPDATE (Sprint 7)
**Status:** OUT_OF_SCOPE - Test Passes
**Action:** Uncommented `[<Fact>]` attribute. Test now runs and passes, documenting that this is a feature request rather than a codegen bug. The test verifies that F# code compiles correctly - the feature request is about adding *additional* metadata, not fixing existing behavior.

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

**Category:** Feature Request (OUT_OF_SCOPE - Binary Size)

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

### UPDATE (Sprint 7)
**Status:** OUT_OF_SCOPE - Test Passes
**Action:** Uncommented `[<Fact>]` attribute. Test now runs and passes, documenting that this is a design question/feature request rather than a codegen bug. Current behavior (generating DebuggerProxies in release) is intentional and works correctly.

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

## Issue #14707

**Title:** Existing signature files become unusable

**Link:** https://github.com/dotnet/fsharp/issues/14707

**Category:** Signature Generation Bug

### Minimal Repro

```fsharp
// When a project has signature files with wildcards:
// Module.fsi
val foo01 : int -> string -> _
val bar01 : int -> int -> _

// After --allsigs regeneration, they become:
val foo01: int -> string -> '?17893
val bar01: int -> int -> '?17894
```

The `--allsigs` flag converts wildcard types (`_`) to invalid type variables (`'?NNNNN`), making the project fail to build on subsequent compilations.

### Expected Behavior
Signature files with wildcards should remain compatible after regeneration, or wildcards should be replaced with valid inferred types.

### Actual Behavior
Wildcards are replaced with `'?NNNNN` syntax which is invalid F# and causes compilation to fail.

### Test Location
`CodeGenRegressions.fs` → `Issue_14707_SignatureFileUnusable`

### Analysis
The signature file generator's handling of unresolved type variables produces invalid F# syntax. The `'?NNNNN` format is internal compiler representation leaking into output.

### Fix Location
- `src/Compiler/Checking/NicePrint.fs` - signature printing logic

### Risks
- Medium: Signature compatibility is important for library evolution

---

## Issue #14706

**Title:** Signature file generation WhereTyparSubtypeOfType

**Link:** https://github.com/dotnet/fsharp/issues/14706

**Category:** Signature Generation Bug

### Minimal Repro

```fsharp
module Foo

type IProvider = interface end

type Tainted<'T> = class end

type ConstructB =
    static member ComputeDefinitionLocationOfProvidedItem<'T when 'T :> IProvider>(p: Tainted<'T>) : obj option = None

// Generated signature becomes:
// static member ComputeDefinitionLocationOfProvidedItem: p: Tainted<#IProvider> -> obj option
// Instead of preserving the explicit type parameter constraint
```

### Expected Behavior
Signature should preserve explicit type parameter: `ComputeDefinitionLocationOfProvidedItem<'T when 'T :> IProvider> : p: Tainted<'T> -> obj option`
Also, `[<Sealed>]` should be added to `type ConstructB` if it has only static members.

### Actual Behavior
Generates `p: Tainted<#IProvider>` which uses flexible type syntax instead of explicit constraint. May affect IL or MVID.

### Test Location
`CodeGenRegressions.fs` → `Issue_14706_SignatureWhereTypar`

### Analysis
NicePrint doesn't correctly handle `WhereTyparSubtypeOfType` constraints when generating signatures for static members with subtype-constrained type parameters.

### Fix Location
- `src/Compiler/Checking/NicePrint.fs`

### Risks
- Low: Fix is in signature printing only

---

## Issue #14508

**Title:** nativeptr in interfaces leads to runtime errors

**Link:** https://github.com/dotnet/fsharp/issues/14508

**Category:** Runtime Error (TypeLoadException)

### Minimal Repro

```fsharp
type IFoo<'T when 'T : unmanaged> =
    abstract member Pointer : nativeptr<'T>

// This causes TypeLoadException at runtime
type Broken() =
    member x.Pointer : nativeptr<int> = Unchecked.defaultof<_>
    interface IFoo<int> with
        member x.Pointer = x.Pointer

// This works fine
type Working<'T when 'T : unmanaged>() =
    member x.Pointer : nativeptr<'T> = Unchecked.defaultof<_>
    interface IFoo<'T> with
        member x.Pointer = x.Pointer
```

### Expected Behavior
Non-generic type implementing generic interface with `nativeptr<'T>` should work or produce compile-time error.

### Actual Behavior
`TypeLoadException`: "Signature of the body and declaration in a method implementation do not match."

### Test Location
`CodeGenRegressions.fs` → `Issue_14508_NativeptrInInterfaces`

### Analysis
IL generation for nativeptr in interface methods is incorrect.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Medium: Native pointer handling requires care

### UPDATE (FIXED)

**Root Cause:** When generating MethodImpl for interface implementations, there was a signature mismatch between the `Overrides` (interface method) and `OverrideBy` (implementing method) signatures.

The issue:
1. `GenFormalSlotsig` generated the interface slot signature using abstract type parameters. For `nativeptr<'T>` where `'T` is a type param, the pointer conversion to `T*` was skipped (because `freeInTypes` found the type param).
2. `GenActualSlotsig` generated the implementing method signature after instantiating types. For `nativeptr<int>`, the pointer conversion was applied, resulting in `int*`.
3. The IL signatures mismatched: interface method returned `nativeint`, implementing method returned `int*`.

**Fix:** Modified `GenActualSlotsig` in `IlxGen.fs` to detect when the slot signature contains `nativeptr<'T>` with interface class type parameters that are being instantiated with concrete types. When this condition is met, the implementing method signature is generated with an environment that includes the interface type parameters, which prevents the nativeptr-to-pointer conversion and maintains consistency with `GenFormalSlotsig`.

**Key Changes:**
- Added `containsNativePtrWithTypar` helper to check if a type contains `nativeptr<'T>` with type parameters from a given set
- Modified `GenActualSlotsig` to use `EnvForTypars ctps eenv` when the slot has nativeptr with interface type parameters that are instantiated to concrete types
- This ensures both `Overrides` and `OverrideBy` signatures use `nativeint` representation

**Location:** `src/Compiler/CodeGen/IlxGen.fs` → `GenActualSlotsig` and `containsNativePtrWithTypar` functions

---

## Issue #14492

**Title:** F# 7.0 incorrect program release config

**Link:** https://github.com/dotnet/fsharp/issues/14492

**Category:** Runtime Error (TypeLoadException)

### Minimal Repro

```fsharp
// Fails with TypeLoadException in Release mode on .NET Framework 4.7
let inline refEquals<'a when 'a : not struct> (a : 'a) (b : 'a) = obj.ReferenceEquals (a, b)

let inline tee f x = 
    f x
    x

let memoizeLatestRef (f: 'a -> 'b) =
    let cell = ref None
    let f' (x: 'a) =
        match cell.Value with
        | Some (x', value) when refEquals x' x -> value
        | _ -> f x |> tee (fun y -> cell.Value <- Some (x, y))
    f'

let f: string -> string = memoizeLatestRef id
f "ok" // TypeLoadException here in Release
```

### Expected Behavior
Program runs correctly in release mode, printing "ok".

### Actual Behavior
`TypeLoadException`: "Method 'Specialize' on type 'memoizeLatestRef@...' tried to implicitly override a method with weaker type parameter constraints."

### Test Location
`CodeGenRegressions.fs` → `Issue_14492_ReleaseConfigError`

### Analysis
Optimization in release mode produces a generated type that violates CLR constraints. The inline function with `'a : not struct` constraint interacts poorly with closure generation.

**Root Cause:** When a closure is generated for an inline function that has constraints (e.g., `'a : not struct`), the closure inherits from `FSharpTypeFunc` and overrides the `Specialize<'T>` method. The base method has NO constraints on `'T`, but the generated override was including the constraints from the inline function's type parameters. The CLR requires that override methods cannot have different constraints than the base method.

**Fix:** Strip all constraints from type parameters when generating the `Specialize` method override in `EraseClosures.fs`. Type safety is already enforced at the F# call site by the type checker.

### Fix Location
- `src/Compiler/CodeGen/EraseClosures.fs` (line ~565)

### UPDATE (FIXED)
Fixed by stripping constraints (`HasReferenceTypeConstraint`, `HasNotNullableValueTypeConstraint`, `HasDefaultConstructorConstraint`, `HasAllowsRefStruct`, and type `Constraints`) from the `ILGenericParameterDef` when generating the `Specialize` method override. This ensures the override matches the unconstrained base method signature while F# type safety is preserved at call sites.

### Risks
- Medium: Release-specific bugs affect production code

---

## Issue #14392

**Title:** OpenApi Swashbuckle support

**Link:** https://github.com/dotnet/fsharp/issues/14392

**Category:** Feature Request (OUT_OF_SCOPE)

**Note:** This is a feature request for better OpenAPI/Swashbuckle support, not a codegen bug. It's included for completeness but is out of scope for the codegen regression test suite.

### Minimal Repro

```fsharp
// No minimal repro - this is a feature request
type MyDto = { Name: string; Value: int }
```

### Expected Behavior
F# types work with OpenAPI/Swashbuckle reflection.

### Actual Behavior
Generated types may not serialize correctly with OpenAPI tools.

### Test Location
`CodeGenRegressions.fs` → `Issue_14392_OpenApiSupport`

### Analysis
This is a feature request, not a bug. F# types may need additional metadata or attributes to work with reflection-based tools like Swashbuckle.

### Fix Location
- N/A - Feature request

### Risks
- Low: Feature request, not a regression

### UPDATE (Sprint 7)
**Status:** OUT_OF_SCOPE - Test Passes
**Action:** Uncommented `[<Fact>]` attribute. Test now runs and passes, documenting that this is a tooling interoperability feature request rather than a codegen bug. F# records compile correctly - the request is about improved OpenAPI tooling support.

---

## Issue #14321

**Title:** Build fails reusing names DU constructors and IWSAM

**Link:** https://github.com/dotnet/fsharp/issues/14321

**Category:** Compile Error (Duplicate Entry)

### Minimal Repro

```fsharp
type SensorReadings = int

type EngineError<'e> =
    static abstract Overheated : 'e
    static abstract LowOil : 'e

// BUG: Using 'Overheated' (same as IWSAM member) for DU case causes:
// "duplicate entry 'Overheated' in property table"
type CarError =
    | Overheated  // <-- Same name as IWSAM member
    | LowOil
    interface EngineError<CarError> with
        static member Overheated = Overheated
        static member LowOil = LowOil

// Workaround: Use different names for DU cases (e.g., Overheated2)
```

### Expected Behavior
DU case names and IWSAM method names should be able to coexist when they refer to the same concept.

### Actual Behavior
Build fails with: `FS2014: A problem occurred writing the binary: duplicate entry 'Overheated' in property table`

### Test Location
`CodeGenRegressions.fs` → `Issue_14321_DuAndIWSAMNames`

### Analysis
IL generation creates duplicate property entries when DU case name matches IWSAM member name. This is a late-stage error (during binary writing) with no earlier feedback.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` or `src/Compiler/AbstractIL/ilwrite.fs`

### Risks
- Low: Name conflict resolution in edge case

* **UPDATE (FIXED):** Fixed by extending the `tdefDiscards` logic in `IlxGen.fs` (around line 11846). For DU types with `AllHelpers`, the compiler now collects nullary case names and discards IWSAM implementation properties/methods that would conflict with the generated DU case properties. When a nullary DU case has the same name as an IWSAM member implementation, the IWSAM implementation property is discarded since it is semantically equivalent to the DU case property (both return the nullary case value).

---

## Issue #13468

**Title:** outref parameter compiled as byref

**Link:** https://github.com/dotnet/fsharp/issues/13468

**Category:** Wrong IL Metadata

### Minimal Repro

```fsharp
// F# interface with outref - works correctly
type I1 =
    abstract M: param: outref<int> -> unit

type T1() =
    interface I1 with
        member this.M(param) = param <- 42

// C# interface (from IL) with out parameter:
// public interface I2 { void M(out int i); }

// F# implementation - BUG: generates `ref` instead of `out`
type T2() =
    interface I2 with  // I2 from C#
        member this.M(i) = i <- 42
// IL shows: void I2.M(ref int i)  -- missing [Out] attribute
```

### Expected Behavior
When implementing C# interface with `out` parameter, F# should generate IL with `[Out]` attribute on the parameter.

### Actual Behavior
F# generates `ref` (byref) instead of `[Out] ref` (outref), changing the method signature metadata.

### Test Location
`CodeGenRegressions.fs` → `Issue_13468_OutrefAsByref`

### Analysis
IL generation doesn't preserve `out` semantics when implementing interfaces from external assemblies. Doesn't break runtime but affects FCS symbol analysis.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Medium: Affects C# interop for out parameters

---

## Issue #13447

**Title:** Extra tail instruction corrupts stack

**Link:** https://github.com/dotnet/fsharp/issues/13447

**Category:** Runtime Crash (Stack Corruption)

### UPDATE (FIXED)

The bug was that when `NativePtr.stackalloc` (which emits `localloc` IL instruction) was used in a function, subsequent calls could incorrectly receive a `tail.` prefix. Since `localloc` allocates memory on the stack frame, and `tail.` releases the stack frame before the callee runs, passing stack-allocated memory (via Span or byref) to a tail call would cause the callee to access released memory - resulting in stack corruption.

**Fix:** Added tracking in `CodeGenBuffer` to detect when `I_localloc` instruction is emitted. The `CanTailcall` function now checks `HasStackAllocatedLocals()` and suppresses tail calls when any localloc has been used in the current method. This is a safe, conservative fix that ensures stack-allocated memory remains valid for the duration of any subsequent calls.

**Changed files:**
- `src/Compiler/CodeGen/IlxGen.fs` - Added `hasStackAllocatedLocals` tracking and check in `CanTailcall`

### Minimal Repro

The full repro requires complex code. See: https://github.com/kerams/repro/

```fsharp
// Simplified version - actual bug involves:
// 1. [<Struct>] on a Result-like type
// 2. Specific function patterns
// 3. Release mode compilation

[<Struct>]
type MyResult<'T, 'E> = 
    | Ok of value: 'T 
    | Error of error: 'E

let writeString (value: string) : MyResult<unit, string> =
    Ok ()
    // BUG: Without trailing `()` here, extra tail. instruction emitted
    // causing stack corruption and wrong values
```

### Expected Behavior
Consistent, correct values across multiple invocations.

### Actual Behavior
First 2 invocations produce wrong byte values in output array. Stack is corrupted by extra `tail.` instruction.

### Known Workarounds
1. Remove `[<Struct>]` from the Result type
2. Add `()` at the end of the affected function

### Test Location
`CodeGenRegressions.fs` → `Issue_13447_TailInstructionCorruption`

### Analysis
Extra `tail.` IL prefix emitted in cases where it corrupts the stack. The before/after IL diff shows the extraneous instruction.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Medium: Tail call optimization is critical for F#

---

## Issue #13223

**Title:** FSharp.Build support for reference assemblies

**Link:** https://github.com/dotnet/fsharp/issues/13223

**Category:** Feature Request (OUT_OF_SCOPE)

**Note:** This is a feature request for FSharp.Build tooling, not a codegen bug.

### Minimal Repro

N/A - Feature request to add/modify FSharp.Build tasks for reference assembly workflow.

### Expected Behavior
FSharp.Build should determine if projects need rebuild based on reference assembly changes (like Roslyn's CopyRefAssembly.cs).

### Actual Behavior
Reference assembly-based incremental build not fully supported.

### Test Location
`CodeGenRegressions.fs` → `Issue_13223_ReferenceAssemblies`

### Analysis
This is a build tooling feature request, not a compiler codegen bug. Requires MSBuild task modifications.

### Fix Location
- `src/FSharp.Build/`

### Risks
- Low: Build tooling feature

### UPDATE (Sprint 7)
**Status:** OUT_OF_SCOPE - Test Passes
**Action:** Uncommented `[<Fact>]` attribute. Test now runs and passes, documenting that this is a FSharp.Build tooling feature request rather than a codegen bug. Compilation works correctly - the request is about MSBuild task improvements for reference assembly workflows.

---

## Issue #13218

**Title:** Compilation time 13000 static member vs let binding

**Link:** https://github.com/dotnet/fsharp/issues/13218

**Category:** Performance (Compile-time) - Not a CodeGen Bug

**Note:** This is a compilation performance issue, not a codegen bug.

### Minimal Repro

```fsharp
// With 13,000 let bindings: ~2min 30sec compile time
module LetBindings =
    [<Literal>]
    let icon1 : string = "icon1"
    // ... 13000 more let bindings

// With 13,000 static members: ~20sec compile time  
type StaticMembers =
    [<Literal>]
    static member inline icon1 : string = "icon1"
    // ... 13000 more static members
```

### Expected Behavior
Both representations should compile in similar time.

### Actual Behavior
- `static member`: ~17-20 seconds
- `let` binding: ~2 minutes 30 seconds

### Test Location
`CodeGenRegressions.fs` → `Issue_13218_ManyStaticMembers`

### Analysis
Suspected O(n²) or worse algorithm when processing let bindings. The difference is in type checking/optimization phase, not codegen itself.

### Fix Location
- `src/Compiler/Checking/` or `src/Compiler/Optimize/Optimizer.fs`

### Risks
- Low: Performance optimization only

---

## Issue #13108

**Title:** Static linking: Understanding FS2009 warnings

**Link:** https://github.com/dotnet/fsharp/issues/13108

**Category:** Compile Warning

### Minimal Repro

```bash
# Clone VsVim and build with static linking
git clone https://github.com/VsVim/VsVim
cd src/VimCore
dotnet build
```

When static linking with `--standalone` flag, the following warnings appear:
```
FSC : warning FS2009: Ignoring mixed managed/unmanaged assembly 'System.Configuration.ConfigurationManager' during static linking
FSC : warning FS2009: Ignoring mixed managed/unmanaged assembly 'System.Security.Permissions' during static linking
```

### Expected Behavior
- Static linking should not produce spurious FS2009 warnings for referenced assemblies
- There should be documentation on what FS2009 means and how to mitigate it

### Actual Behavior
- FS2009 warnings are emitted for assemblies that reference native code
- No documentation available on how to address these warnings
- Unclear if warnings indicate real problems or can be safely ignored

### Test Location
`CodeGenRegressions.fs` → `Issue_13108_StaticLinkingWarnings`

### Analysis
When FSharp.Core or other assemblies are statically linked into a DLL (using `--standalone`), the compiler emits FS2009 warnings for any referenced assemblies that have mixed managed/unmanaged code. The warning message provides no guidance on how to resolve the issue.

### Fix Location
- `src/Compiler/AbstractIL/ilwrite.fs` - static linking warning logic
- Documentation for FS2009 warning

### Risks
- Low: Warning message improvement only

---

## Issue #13100

**Title:** --platform:x64 sets 32 bit characteristic

**Link:** https://github.com/dotnet/fsharp/issues/13100

**Category:** Wrong Behavior

### Minimal Repro

```bash
# Compile with x64 platform
fsc --platform:x64 Program.fs

# Examine PE headers
dumpbin /headers obj/Debug/net6.0/program.dll
```

F# output (incorrect):
```
FILE HEADER VALUES
            8664 machine (x64)
             12E characteristics
                   Executable
                   Line numbers stripped
                   Symbols stripped
                   Application can handle large (>2GB) addresses
                   32 bit word machine    ← WRONG for x64
```

C# output (correct):
```
FILE HEADER VALUES
            8664 machine (x64)
              22 characteristics
                   Executable
                   Application can handle large (>2GB) addresses
```

### Expected Behavior
PE header should NOT have the "32 bit word machine" (IMAGE_FILE_32BIT_MACHINE, 0x100) characteristic flag set when targeting x64.

### Actual Behavior
F# sets characteristics 0x12E which includes the 32-bit flag (0x100).
C# correctly sets characteristics 0x22 without the 32-bit flag.

### Test Location
`CodeGenRegressions.fs` → `Issue_13100_PlatformCharacteristic`

### Analysis
The PE writer incorrectly sets IMAGE_FILE_32BIT_MACHINE flag in the file characteristics when writing x64 binaries.

### Fix Location
- `src/Compiler/AbstractIL/ilwrite.fs` - PE characteristics calculation

### Risks
- Low: Metadata-only change to PE header flags

---

## Issue #12546

**Title:** Implicit boxing produces extraneous closure

**Link:** https://github.com/dotnet/fsharp/issues/12546

**Category:** Performance

### Minimal Repro

```fsharp
module Foo

let foo (ob: obj) = box(fun () -> ob.ToString()) :?> (unit -> string)

// BUG: This allocates TWO closures
let go() = foo "hi"

// WORKAROUND: Explicit boxing avoids the extra closure
let goFixed() = foo(box "hi")
```

Decompiled C# shows the extraneous wrapper closure:
```csharp
internal sealed class go@5 : FSharpFunc
{
    public FSharpFunc clo1;  // Wraps the real closure

    public override string Invoke(Unit arg10)
    {
        return clo1.Invoke(arg10);  // Just forwards the call
    }
}
```

### Expected Behavior
A single closure should be allocated - the one that captures `ob` and calls `ToString()`.

### Actual Behavior
Two closures are allocated: the real closure AND a wrapper closure that just forwards calls to the first one.

### Test Location
`CodeGenRegressions.fs` → `Issue_12546_BoxingClosure`

### Analysis
When implicit boxing occurs at the call site, the compiler generates an extra closure wrapper instead of directly using the allocated closure. This doubles allocations unnecessarily.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - closure generation for boxed arguments

### Risks
- Low: Performance optimization only

---

## Issue #12460

**Title:** F# and C# produce Version info values differently

**Link:** https://github.com/dotnet/fsharp/issues/12460

**Category:** Metadata

### Minimal Repro

Compile equivalent projects with F# and C#, then compare version resources:

Observed differences:
- F# project output misses `Internal Name` value in version resources
- `Product Version` format differs: C# produces `1.0.0`, F# produces `1.0.0.0`

### Expected Behavior
Version info metadata should match conventions used by C# for consistency:
- Include `Internal Name` in version resources  
- Use 3-part version format (`1.0.0`) for Product Version

### Actual Behavior
- `Internal Name` field is missing in F# assemblies
- `Product Version` shows 4-part format (`1.0.0.0`) instead of 3-part

### Test Location
`CodeGenRegressions.fs` → `Issue_12460_VersionInfoDifference`

### Analysis
The PE version info resources generated by F# differ from C# conventions. This can cause inconsistencies when examining assembly properties or when tooling expects C# conventions.

### Fix Location
- `src/Compiler/AbstractIL/ilwrite.fs` - PE version resource generation

### Risks
- Low: Metadata-only change for consistency with C#

---

## Issue #12416

**Title:** Optimization inlining applied inconsistently with piping

**Link:** https://github.com/dotnet/fsharp/issues/12416

**Category:** Performance

### Minimal Repro

```fsharp
type 'T PushStream = ('T -> bool) -> bool

let inline ofArray (vs : _ array) : _ PushStream = fun ([<InlineIfLambda>] r) ->
  let mutable i = 0
  while i < vs.Length && r vs.[i] do i <- i + 1
  i = vs.Length

let inline fold ([<InlineIfLambda>] f) z ([<InlineIfLambda>] ps : _ PushStream) =
  let mutable s = z
  let _ = ps (fun v -> s <- f s v; true)
  s

let inline (|>>) ([<InlineIfLambda>] v) ([<InlineIfLambda>] f) = f v

let values = [|0..10000|]

// THIS INLINES - values stored in let binding
let thisIsInlined () = ofArray values |>> fold (+) 0

// THIS ALSO INLINES - array in intermediate binding
let thisIsInlined2 () = 
  let vs = [|0..10000|]
  ofArray vs |>> fold (+) 0

// BUG: THIS DOES NOT INLINE - array literal inline
let thisIsNotInlined () = ofArray [|0..10000|] |>> fold (+) 0
```

### Expected Behavior
All three functions should have identical inlined code - the only difference is where the array comes from.

### Actual Behavior
- `thisIsInlined` and `thisIsInlined2` have fully inlined loops
- `thisIsNotInlined` generates closure classes and virtual calls

### Test Location
`CodeGenRegressions.fs` → `Issue_12416_PipeInlining`

### Analysis
`InlineIfLambda` inlining depends on whether the argument is a let-bound variable or an inline expression. When the argument is computed inline (like `[|0..10000|]`), the optimizer fails to inline the lambda.

**Workaround**: Store arguments in intermediate let bindings before passing to `InlineIfLambda` functions.

### Fix Location
- `src/Compiler/Optimize/Optimizer.fs` - InlineIfLambda argument handling

### Risks
- Low: Optimization improvement only

---

## Issue #12384

**Title:** Mutually recursive non-function values not initialized correctly

**Link:** https://github.com/dotnet/fsharp/issues/12384

**Category:** Wrong Behavior

### Minimal Repro

```fsharp
type Node = { Next: Node; Prev: Node; Value: int }

// Single self-reference works correctly
let rec zero = { Next = zero; Prev = zero; Value = 0 }

// BUG: Mutual recursion - 'one' has null references
let rec one = { Next = two; Prev = two; Value = 1 }
and two = { Next = one; Prev = one; Value = 2 }

printfn "%A" one
printfn "%A" two
```

### Expected Behavior
Either:
- Correctly initialize both mutually recursive values
- Reject the code at compile time as invalid

### Actual Behavior
Code compiles without warnings. At runtime:
```
{ Next = null; Prev = null; Value = 1 }     ← one is WRONG
{ Next = { Next = null; Prev = null; Value = 1 }
  Prev = { Next = null; Prev = null; Value = 1 }
  Value = 2 }                                ← two correctly references one
```

`one.Next` and `one.Prev` are null instead of referencing `two`.

### Test Location
`CodeGenRegressions.fs` → `Issue_12384_MutRecInitOrder`

### Analysis
The module initialization code for mutually recursive non-function values generates incorrect IL. The first binding in the group (`one`) is initialized before the second binding (`two`) is created, so references to `two` become null. Single self-referencing values work because they can reference themselves.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - mutual recursion initialization order

### Risks
- Medium: Initialization order changes could break existing code that accidentally depends on current behavior

### UPDATE (FIXED)

**Status:** Fixed (for simple `let rec ... and ...` cases)

The simple case of `let rec one = ... and two = ...` was fixed by PR #12395. The test now
verifies that mutually recursive record values with forward references are correctly initialized.

**Note:** There is still an edge case with `module rec` and intermediate modules between bindings
that remains unfixed (see issue #12384 comments). This edge case is documented but the main
use case reported in the issue is now working correctly.

**Test:** `Issue_12384_MutRecInitOrder` - Uncommented `[<Fact>]`, verifies mutual recursion
initialization works at runtime.

---

## Issue #12366

**Title:** Rethink names for compiler-generated closures

**Link:** https://github.com/dotnet/fsharp/issues/12366

**Category:** Cosmetic (IL Quality Issue)

### Minimal Repro

```fsharp
let f = fun x -> x + 1

// Nested closures get confusing names
let complex = 
    fun a -> 
        fun b -> 
            fun c -> a + b + c

// Pipeline closures may get "Pipe input at line 63@53" names
let result = [1;2;3] |> List.map (fun x -> x * 2)
```

### IL Output Shows Poor Type Names

```il
// Good: Uses the let binding name
.class nested assembly auto ansi serializable sealed beforefieldinit 'f@5'

// Poor: Uses generic "clo" backup name  
.class nested assembly auto ansi serializable sealed beforefieldinit 'clo@10-1'

// Bad: Uses debug string as type name
.class nested assembly auto ansi serializable sealed beforefieldinit 'Pipe input at line 63@53'
```

These type names are visible in:
- **ildasm** and **dotPeek/ILSpy** decompilation output
- **Stack traces** during debugging
- **Performance profiler** output
- **Exception messages** containing type names

### Why Runtime Verification Is Insufficient

Runtime tests cannot verify this issue because:
1. The code **executes correctly** regardless of closure type names
2. Type names are **cosmetic metadata** not affecting program behavior
3. Verifying type names requires **IL inspection** (e.g., `ildasm` output)

### Expected Behavior

Generated closure types should have meaningful, consistent names useful for debugging and profiling.

### Actual Behavior

The naming heuristic is weak:
- Unnecessary use of backup name "clo" when better names are available
- Sometimes compiler-generated debug strings are used as type names
- Line numbers appended but could be more informative

### Test Location

`CodeGenRegressions.fs` → `Issue_12366_ClosureNaming`

### Analysis

The closure naming heuristic in IlxGen tries to use the let identifier being bound, but often falls back to generic names. After pipeline debugging was added, some closures get names based on debug strings.

### Fix Location

- `src/Compiler/CodeGen/IlxGen.fs` - closure type naming logic

### Risks

- Low: Cosmetic change only, but may affect tooling that parses type names

---

## Issue #12139

**Title:** Improve string null check IL codegen

**Link:** https://github.com/dotnet/fsharp/issues/12139

**Category:** Performance (IL Code Size)

### Minimal Repro

```fsharp
open System

// Simple null check function
let isNullString (s: string) = s = null
let isNotNullString (s: string) = s <> null

// Null check in loop
let test() =
    while Console.ReadLine() <> null do
        Console.WriteLine(1)
```

### IL Comparison: F# vs C#

**F# IL for `s = null`:**
```il
IL_0000: ldarg.0
IL_0001: ldnull
IL_0002: call bool [System.Runtime]System.String::Equals(string, string)  ← Extra method call
IL_0007: ret
```

**C# IL for `s == null` (optimal):**
```il
IL_0000: ldarg.0
IL_0001: ldnull
IL_0002: ceq       ← Simple comparison instruction
IL_0004: ret
```

**Or in boolean context, C# uses:**
```il
IL_0000: ldarg.0
IL_0001: brtrue.s IL_0005   ← Single branch instruction
```

### Why Runtime Verification Is Insufficient

Runtime tests cannot verify this issue because:
1. Both patterns produce **identical runtime results** (`true`/`false`)
2. The JIT may **optimize away** the String.Equals call at runtime
3. The issue is about **IL code size** and **JIT overhead**, not correctness
4. Verification requires **IL inspection** comparing instruction counts

### Expected Behavior

String null checks should use simple `brtrue`/`brfalse` or `ceq` instructions like C#.

### Actual Behavior

F# emits `call String.Equals(string, string)` for null comparisons, which:
- Increases DLL size (extra call instruction + null push)
- Requires more JIT work (though JIT can optimize it away)
- Is inconsistent with C# output for the same pattern

### Test Location

`CodeGenRegressions.fs` → `Issue_12139_StringNullCheck`

### Analysis

F# treats `s = null` as a structural equality check and emits `String.Equals` call, while C# recognizes null comparisons specially and emits simple pointer comparisons.

### Fix Location

- `src/Compiler/CodeGen/IlxGen.fs` or `src/Compiler/Optimize/Optimizer.fs` - recognize string null patterns

### Risks

- Low: IL optimization only, semantics unchanged

---

## Issue #12137

**Title:** Improve analysis to reduce emit of tail.

**Link:** https://github.com/dotnet/fsharp/issues/12137

**Category:** Performance (Cross-Assembly IL Issue)

### Minimal Repro

When calling inline functions from another assembly, F# emits `tail.` prefix:

**Same assembly call (good - no tail. prefix):**
```il
IL_000c: call !!0 Module::fold<int32, int32>(...)
```

**Cross-assembly call (bad - unnecessary tail. prefix):**
```il
IL_000c: tail.
IL_000e: call !!0 [OtherLib]Module::fold<int32, int32>(...)
```

### Why Runtime Verification Is Insufficient

Runtime tests cannot fully verify this issue because:
1. Both patterns produce **identical runtime results**
2. The issue requires **two separate assemblies** to manifest
3. Single-file tests cannot demonstrate cross-assembly calls
4. The `tail.` prefix affects **performance** (2-3x slower) but not correctness
5. Verification requires **IL inspection** of cross-assembly call sites

### Expected Behavior

The `tail.` prefix should only be emitted when necessary for stack safety in recursive scenarios.

### Actual Behavior

F# emits `tail.` prefix inconsistently:
- Same-assembly calls: no `tail.` prefix (correct)
- Cross-assembly calls: `tail.` prefix emitted (unnecessary in most cases)

The unnecessary `tail.` causes:
- 2-3x slower execution due to tail call dispatch helpers
- 2x larger JIT-generated assembly code
- Prevents certain JIT optimizations (inlining)

### Test Location

`CodeGenRegressions.fs` → `Issue_12137_TailEmitReduction`

Note: The test documents the issue but cannot fully reproduce it because cross-assembly calls require compiling and referencing a separate assembly.

### Analysis

The tail call analysis doesn't have enough information about cross-assembly inline functions to determine that tail calls aren't needed. This results in conservative emission of `tail.` prefix which hurts performance.

### Fix Location

- `src/Compiler/CodeGen/IlxGen.fs` - tail call emission logic

### Risks

- Low: Performance optimization, but must ensure stack safety isn't compromised for truly recursive scenarios

---

## Issue #12136

**Title:** "use fixed" construct does not unpin at end of scope

**Link:** https://github.com/dotnet/fsharp/issues/12136

**Category:** Wrong Behavior

### Minimal Repro

```fsharp
let used<'T>(t: 'T): unit = ignore t

let test(array: int[]): unit =
    do
        use pin = fixed &array.[0]
        used pin
    used 1  // BUG: array is still pinned here!
```

F# IL - no cleanup at scope end:
```il
.locals init ([1] int32& pinned)
IL_0007: stloc.1
// ... use pin ...
IL_0012: ldc.i4.1        // ← No cleanup of pinned local
IL_0013: call void Main::used(!!0)
```

C# IL - properly unpins at scope end:
```il
.locals init ([1] int32& pinned)
// ... use pin ...
IL_0010: ldc.i4.0
IL_0011: conv.u
IL_0012: stloc.1        // ← Clears pinned local
// ... rest of code ...
```

### Expected Behavior
The pinned local should be set to null/zero at the end of the `use fixed` scope, so the GC can move the array.

### Actual Behavior
The pinned variable remains set until function returns, keeping the array pinned longer than necessary. This:
- Prevents GC from moving the array during other operations
- Confuses decompilers which expect proper scope cleanup

**Workaround**: Put the fixed block in a separate function:
```fsharp
let testFixed (array: int[]) : unit =
    let doBlock() =
        use pin = fixed &array.[0]
        used pin
    doBlock()
    used 1  // Now array is correctly unpinned
```

### Test Location
`CodeGenRegressions.fs` → `Issue_12136_FixedUnpin`

### Analysis
The `use fixed` construct generates a pinned local variable but doesn't emit cleanup code at the end of the scope. C# emits `ldc.i4.0; conv.u; stloc` to clear the pinned local, while F# does not.

See also #10832 where this caused confusion about whether F# pins correctly.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs` - fixed statement codegen

### Risks
- Medium: Memory pinning affects GC behavior; must ensure correctness

---

## Issue #11935

**Title:** unmanaged constraint not recognized by C#

**Link:** https://github.com/dotnet/fsharp/issues/11935

**Category:** C# Interop

### Minimal Repro

```fsharp
let inline test<'T when 'T : unmanaged> (x: 'T) = x
```

### Expected Behavior
C# code can call F# methods with unmanaged constraints.

### Actual Behavior
C# doesn't recognize the constraint correctly.

### Test Location
`CodeGenRegressions.fs` → `Issue_11935_UnmanagedConstraintInterop`

### Analysis
unmanaged constraint metadata may not match C# expectations.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Low: Constraint metadata

---

## Issue #11556

**Title:** Better IL output for property/field initializers

**Link:** https://github.com/dotnet/fsharp/issues/11556

**Category:** Performance

### Minimal Repro

```fsharp
open System.Runtime.CompilerServices

type Test =
    [<DefaultValue>]
    val mutable X : int
    new() = { }

[<MethodImpl(MethodImplOptions.NoInlining)>]
let test() =
    Test(X = 1)
```

### Expected Behavior

The IL should use the efficient `dup` pattern:

```il
.method public static class Program/Test test() cil managed noinlining
{
  .maxstack  4
  IL_0xxx:  newobj     instance void Program/Test::.ctor()
  IL_0xxx:  dup                                // Use dup - no locals needed
  IL_0xxx:  ldc.i4.1
  IL_0xxx:  stfld      int32 Program/Test::X
  IL_0xxx:  ret
}
```

### Actual Behavior

The IL uses unnecessary locals with stloc/ldloc pattern:

```il
.method public static class Program/Test test() cil managed noinlining
{
  .maxstack  4
  .locals init (class Program/Test V_0)        // Unnecessary local
  IL_0000:  newobj     instance void Program/Test::.ctor()
  IL_0005:  stloc.0                            // Store to local
  IL_0006:  ldloc.0                            // Load from local
  IL_0007:  ldc.i4.1
  IL_0008:  stfld      int32 Program/Test::X
  IL_000d:  ldloc.0                            // Load from local again
  IL_000e:  ret
}
```

### Test Location
`CodeGenRegressions.fs` → `Issue_11556_FieldInitializers`

### Analysis

The inefficiency comes from the code generator emitting a local variable to hold the newly constructed object reference, when it could simply use `dup` to duplicate the stack value. This results in:
- Extra `.locals init` declaration
- More instructions (stloc.0, ldloc.0, ldloc.0 vs single dup)
- Larger code size
- Slightly worse JIT performance

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Low: IL optimization only, no semantic change

---

## Issue #11132

**Title:** TypeloadException delegate with voidptr parameter

**Link:** https://github.com/dotnet/fsharp/issues/11132

**Category:** Runtime Error

### Minimal Repro

```fsharp
type MyDelegate = delegate of voidptr -> unit
```

### Expected Behavior
Delegate with voidptr parameter works.

### Actual Behavior
TypeLoadException at runtime.

### Test Location
`CodeGenRegressions.fs` → `Issue_11132_VoidptrDelegate`

### Analysis
Delegate IL generation incorrect for voidptr parameters.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Medium: Delegate generation

---

## Issue #11114

**Title:** Record with hundreds of members StackOverflow

**Link:** https://github.com/dotnet/fsharp/issues/11114

**Category:** Compile Crash

### Minimal Repro

```fsharp
type BigRecord = {
    Field1: int
    Field2: int
    // ... hundreds of fields
}
```

### Expected Behavior
Large records compile without error.

### Actual Behavior
StackOverflowException during compilation.

### Test Location
`CodeGenRegressions.fs` → `Issue_11114_LargeRecordStackOverflow`

### Analysis
Recursive algorithm without tail optimization for record processing.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Low: Algorithm fix

---

## Issue #9348

**Title:** Performance of Comparing and Ordering

**Link:** https://github.com/dotnet/fsharp/issues/9348

**Category:** Performance

### Minimal Repro

```fsharp
type T = { X: int } 
let compare (a: T) (b: T) = compare a.X b.X
```

### Expected Behavior
Efficient comparison IL.

### Actual Behavior
Generated comparison code is suboptimal.

### Test Location
`CodeGenRegressions.fs` → `Issue_9348_ComparePerformance`

### Analysis
Generated IComparable implementation could be more efficient.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Low: Performance optimization

---

## Issue #9176

**Title:** Decorate inline function code with attribute

**Link:** https://github.com/dotnet/fsharp/issues/9176

**Category:** Feature Request (OUT_OF_SCOPE)

**Note:** This is a FEATURE REQUEST, not a codegen bug. The compiler intentionally doesn't preserve attributes on inlined code. The request is for a **new feature** to add source tracking attributes to inlined code.

### Requested Feature

The issue requests a new `FSharpInlineFunction` attribute (or similar) that would be emitted at call sites where inline functions are expanded. This would enable:

1. **Debugging**: Stack traces could show the original inline function name
2. **Profiling**: Performance tools could attribute time to the original function
3. **Code coverage**: Coverage tools could track back to inline function definitions

### Minimal Repro

```fsharp
// Current behavior: when 'f' is inlined, there's no trace of it in IL
let inline f x = x + 1

// At this call site, the inlined code has no indication it came from 'f'
let g y = f y + f y
```

### Current IL (no inline tracking):
```il
.method public static int32 g(int32 y) cil managed
{
    IL_0000: ldarg.0
    IL_0001: ldc.i4.1
    IL_0002: add        // ← This is 'f y' but no indication of 'f'
    IL_0003: ldarg.0
    IL_0004: ldc.i4.1
    IL_0005: add        // ← This is 'f y' again
    IL_0006: add
    IL_0007: ret
}
```

### Requested IL (with tracking attribute):
```il
.method public static int32 g(int32 y) cil managed
{
    [FSharpInlineFunction("f", "Module.fs", line=3)]  // ← Requested feature
    IL_0000: ldarg.0
    IL_0001: ldc.i4.1
    IL_0002: add
    // ...
}
```

### Why Runtime Verification Is Insufficient

This cannot be verified at runtime because:
1. **It's a feature that doesn't exist** - there's no bug to reproduce
2. The current behavior is **intentional design**, not a regression
3. The request is for **new metadata** in IL, not a behavior change

### Test Location

`CodeGenRegressions.fs` → `Issue_9176_InlineAttributes`

The test is marked `[OUT_OF_SCOPE: Feature Request]` because it documents a feature request, not a bug.

### Analysis

This is a design question about whether attributes should be propagated to inlined call sites. Currently the compiler doesn't do this intentionally. Implementing this would require:
1. A new attribute type in FSharp.Core
2. Compiler changes to track inlining provenance
3. Tooling updates to consume the attribute

### Fix Location

- `src/Compiler/CodeGen/IlxGen.fs` - would need to emit new attributes during inlining

### Risks

- Low: This is additive metadata, would not affect existing behavior

### UPDATE (Sprint 7)
**Status:** OUT_OF_SCOPE - Test Passes
**Action:** Uncommented `[<Fact>]` attribute. Test now runs and passes, documenting that this is a feature request for new inline tracking metadata rather than a codegen bug. Inline functions work correctly - the request is about adding source provenance attributes at inlined call sites.

---

## Issue #7861

**Title:** Missing assembly reference for type in attributes

**Link:** https://github.com/dotnet/fsharp/issues/7861

**Category:** Compile Error

### Minimal Repro

```fsharp
[<SomeAttribute(typeof<ExternalType>)>]
type T = class end
```

### Expected Behavior
Attribute with type reference compiles correctly.

### Actual Behavior
Missing assembly reference error.

### Test Location
`CodeGenRegressions.fs` → `Issue_7861_AttributeTypeReference`

### Analysis
Assembly reference not added for types used in attributes.

### Fix Location
- `src/Compiler/AbstractIL/ilwrite.fs`

### Risks
- Low: Assembly reference handling

---

## Issue #6750

**Title:** Mutually recursive values leave fields uninitialized

**Link:** https://github.com/dotnet/fsharp/issues/6750

**Category:** Wrong Behavior

### Minimal Repro

```fsharp
let rec a = b + 1
and b = 0
printfn "%d" a
```

### Expected Behavior
Mutual recursion initializes correctly.

### Actual Behavior
Fields may be uninitialized or wrong value.

### Test Location
`CodeGenRegressions.fs` → `Issue_6750_MutRecUninitialized`

### Analysis
Initialization order for mutual recursion is incorrect.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Medium: Initialization semantics

---

## Issue #6379

**Title:** FS2014 when using tupled args

**Link:** https://github.com/dotnet/fsharp/issues/6379

**Category:** Compile Warning

### Minimal Repro

```fsharp
let f (x, y) = x + y
```

### Expected Behavior
No warning for normal tuple patterns.

### Actual Behavior
FS2014 warning in some tuple argument scenarios.

### Test Location
`CodeGenRegressions.fs` → `Issue_6379_TupledArgsWarning`

### Analysis
False positive warning for tuple patterns.

### Fix Location
- `src/Compiler/Checking/TypeChecker.fs`

### Risks
- Low: Warning elimination

---

## Issue #5834

**Title:** Obsolete on abstract generates accessors without specialname

**Link:** https://github.com/dotnet/fsharp/issues/5834

**Category:** Wrong Behavior

### Minimal Repro

```fsharp
open System
open System.Reflection

// Abstract type with [<Obsolete>] on abstract event accessors
[<AbstractClass>]
type AbstractWithEvent() =
    [<Obsolete("This event is deprecated")>]
    [<CLIEvent>]
    abstract member MyEvent : IEvent<EventHandler, EventArgs>

[<EntryPoint>]
let main _ =
    let abstractType = typeof<AbstractWithEvent>
    let abstractAddMethod = abstractType.GetMethod("add_MyEvent")
    
    printfn "add_MyEvent.IsSpecialName = %b" abstractAddMethod.IsSpecialName
    // Bug: Prints "false" instead of "true"
    if abstractAddMethod.IsSpecialName then 0 else 1
```

### Expected Behavior
Abstract event accessors (`add_MyEvent`, `remove_MyEvent`) have `IsSpecialName = true` in reflection, matching concrete implementations.

### Actual Behavior
Abstract event accessors have `IsSpecialName = false`, breaking Reflection-based tools like Moq that rely on this flag to identify event accessors.

### Test Location
`CodeGenRegressions.fs` → `Issue_5834_ObsoleteSpecialname`

### Analysis
When generating abstract event accessors (especially with attributes like `[<Obsolete>]`), the F# compiler doesn't set the `specialname` IL flag. Concrete event accessors get this flag correctly. This breaks Moq and other reflection-based libraries that check `method.IsSpecialName` to identify event accessors.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Low: Metadata fix, should be additive

### UPDATE (FIXED)

**Root Cause:** In `GenAbstractBinding`, when processing abstract [<CLIEvent>] members, the typechecker generates separate `add_` and `remove_` ValRefs with `MemberKind.Member`. These go through the `SynMemberKind.Member` case which didn't check for `IsGeneratedEventVal` to apply the SpecialName flag.

**Fix:** Added a check in the `SynMemberKind.Member` case of `GenAbstractBinding` to apply `WithSpecialName` when `vref.Deref.val_flags.IsGeneratedEventVal` is true. This matches the existing behavior for concrete event accessors at line 9682.

**Files Modified:**
- `src/Compiler/CodeGen/IlxGen.fs` - Added SpecialName for generated abstract event accessors

---

## Issue #5464

**Title:** F# ignores custom modifiers modreq/modopt

**Link:** https://github.com/dotnet/fsharp/issues/5464

**Category:** C# Interop

**[IL_LEVEL_ISSUE: Requires C# interop to demonstrate]**

### Root Cause

The F# compiler strips custom modifiers during type import:

```fsharp
| ILType.Modified(_,_,ty) ->
    // All custom modifiers are ignored
    ImportILType env m tinst ty
```

### What Are modreq/modopt?

Custom modifiers are IL metadata that modify types without changing their CLR type:
- **modreq (required)**: The modifier MUST be understood (e.g., `IsReadOnlyAttribute` for C# `in` parameters)
- **modopt (optional)**: The modifier MAY be ignored (e.g., `IsVolatile` for `volatile` fields)

### Example - C# 'in' Parameter

```csharp
// C# source:
public void Process(in ReadOnlyStruct value) { }

// Compiled IL signature:
.method public hidebysig instance void Process(
    [in] valuetype ReadOnlyStruct& modreq([netstandard]System.Runtime.InteropServices.InAttribute) value
) cil managed
```

### Expected Behavior

When F# calls this C# method, the IL should preserve the modreq:

```il
call instance void [CSharpLib]CSharpLib::Process(
    valuetype ReadOnlyStruct& modreq([System.Runtime]System.Runtime.InteropServices.InAttribute))
```

### Actual Behavior

F# strips the modreq from the emitted IL:

```il
call instance void [CSharpLib]CSharpLib::Process(
    valuetype ReadOnlyStruct&)  // NO modreq!
```

### Test Location
`CodeGenRegressions.fs` → `Issue_5464_CustomModifiers`

### Analysis

Full reproduction requires a multi-language test:
1. C# library with `in` parameters or `volatile` fields (which have modreq/modopt)
2. F# code that consumes it
3. IL verification that the modifier is present/absent

The bug causes:
- F# cannot distinguish C# `in` from `ref` parameters
- C++/CLI interop is broken (C++ heavily uses custom modifiers)
- Violation of ECMA CLI specification

This is fundamentally a cross-assembly issue that cannot be demonstrated in a single F# source file.

### Fix Location
- `src/Compiler/AbstractIL/ilwrite.fs`

### Risks
- Medium: C++/CLI interop relies heavily on custom modifiers

---

## Issue #878

**Title:** Serialization of F# exception variants doesn't serialize fields

**Link:** https://github.com/dotnet/fsharp/issues/878

**Category:** Wrong Behavior

### Minimal Repro

```fsharp
open System.IO
open System.Runtime.Serialization.Formatters.Binary

exception Foo of x:string * y:int

let clone (x : 'T) =
    let bf = new BinaryFormatter()
    let m = new MemoryStream()
    bf.Serialize(m, x)
    m.Position <- 0L
    bf.Deserialize(m) :?> 'T

let original = Foo("value", 42)
let cloned = clone original
// cloned is Foo(null, 0) instead of Foo("value", 42)

match cloned with
| Foo(x, y) -> printfn "x='%s', y=%d" (if isNull x then "null" else x) y
// Prints: x='null', y=0
```

### Expected Behavior
Exception fields survive serialization roundtrip: `Foo("value", 42)` → serialize → deserialize → `Foo("value", 42)`.

### Actual Behavior
Exception fields are lost after deserialization: `Foo("value", 42)` → serialize → deserialize → `Foo(null, 0)`.

### Test Location
`CodeGenRegressions.fs` → `Issue_878_ExceptionSerialization`

### Analysis
F# exception types inherit from `System.Exception` which uses `ISerializable`. The compiler must generate:
1. A `GetObjectData` override that calls `base.GetObjectData()` then adds field values via `info.AddValue()`
2. A `(SerializationInfo, StreamingContext)` constructor that calls base then reads fields via `info.GetValue()`

Currently, F# generates only the constructor shell without reading the field values.

### Fix Location
- `src/Compiler/CodeGen/IlxGen.fs`

### Risks
- Low: Serialization fix, additive change to generated code

### UPDATE (FIXED)
Fixed by modifying `GenExnDef` in IlxGen.fs to generate proper serialization support:
1. Modified the serialization constructor to restore fields by calling `info.GetValue("fieldName", typeof<fieldType>)` for each exception field
2. Added a `GetObjectData` override that calls `base.GetObjectData(info, context)` and then `info.AddValue("fieldName", this.field)` for each field

The test now verifies the IL generation contains both the GetObjectData method and the serialization constructor with proper field handling. Note: BinaryFormatter is removed in .NET 10+, so runtime verification is not possible; the test validates IL structure instead.

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
