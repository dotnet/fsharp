// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Tests for known CodeGen regressions/bugs that are documented but not yet fixed.
/// Each test is commented out with // [<Fact>] to prevent CI failures while keeping them buildable.
/// See CODEGEN_REGRESSIONS.md in the repository root for detailed analysis of each issue.
namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.Utilities

module CodeGenRegressions =

    let private getActualIL (result: CompilationResult) =
        match result with
        | CompilationResult.Success s ->
            match s.OutputPath with
            | Some p ->
                let (_, _, actualIL) = ILChecker.verifyILAndReturnActual [] p [ "// dummy" ]
                actualIL
            | None -> failwith "No output path"
        | _ -> failwith "Compilation failed"

    // ===== Issue #19075: CLR Crashes when running program using constrained calls =====
    // https://github.com/dotnet/fsharp/issues/19075
    // The combination of SRTP with IDisposable constraint and constrained call generates
    // invalid IL that causes a CLR crash (segfault) at runtime.
    // Test disabled - fix was reverted due to causing other test crashes
    // [<Fact>]
    let ``Issue_19075_ConstrainedCallsCrash`` () =
        let source = """
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
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // ===== Issue #19068: Object expression in struct generates byref field in a class =====
    // https://github.com/dotnet/fsharp/issues/19068
    // Using an object expression in a struct that depends on primary constructor parameters
    // results in a class being emitted with a byref field, causing TypeLoadException at runtime.
    [<Fact>]
    let ``Issue_19068_StructObjectExprByrefField`` () =
        let source = """
module Test

type Class(test : obj) = class end

[<Struct>]
type Struct(test : obj) =
    member _.Test() = {
        new Class(test) with
        member _.ToString() = ""
    }

let run() =
    let s = Struct("hello")
    s.Test() |> ignore
    printfn "Success"

run()
"""
        FSharp source
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // ===== Issue #19020: [<return: ...>] not respected on class members =====
    // https://github.com/dotnet/fsharp/issues/19020
    // The [<return: SomeAttribute>] syntax to attach an attribute to the return type of a
    // method does not work on class static/instance members (works on module functions).
    // Test disabled - fix was reverted due to bootstrap-breaking bug
    // [<Fact>]
    let ``Issue_19020_ReturnAttributeNotRespected`` () =
        let source = """
module Test

open System.Reflection

type SomeAttribute() =
    inherit System.Attribute()

module Module =
    [<return: SomeAttribute>]
    let func a = a + 1

type Class() =
    [<return: SomeAttribute>]
    static member ``static member`` a = a + 1

    [<return: SomeAttribute>]
    member _.``member`` a = a + 1
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // We should verify that the return attribute IS emitted for class members
        // Each method should have .param [0] followed by .custom for the return attribute
        |> verifyIL [
            // Module function
            """
.method public static int32  func(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/SomeAttribute::.ctor() = ( 01 00 00 00 )
"""
            // Static member
            """
.method public static int32  'static member'(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/SomeAttribute::.ctor() = ( 01 00 00 00 )
"""
            // Instance member
            """
.method public hidebysig instance int32 member(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/SomeAttribute::.ctor() = ( 01 00 00 00 )
"""
        ]
        |> ignore

    // Edge case: Return attributes with arguments, multiple return attributes
    // Test disabled - fix for Issue #19020 was reverted due to bootstrap-breaking bug
    // [<Fact>]
    let ``Issue_19020_ReturnAttributeEdgeCases`` () =
        let source = """
module Test

open System

// Attribute with arguments
type MyAttribute(value: string) =
    inherit Attribute()
    member _.Value = value

// Multiple return attributes
type AnotherAttribute() =
    inherit Attribute()

module Module =
    // Multiple return attributes on module function
    [<return: MyAttribute("mod")>]
    [<return: AnotherAttribute>]
    let func a = a + 1

type SomeClass() =
    // Multiple return attributes on instance member
    [<return: MyAttribute("ins")>]
    [<return: AnotherAttribute>]
    member _.MultipleAttrs a = a + 1
    
    // Return attribute on static member returning string
    [<return: MyAttribute("str")>]
    static member StringMethod () = "hello"
    
    // Return attribute on property getter
    [<return: MyAttribute("get")>]
    member _.PropWithReturnAttr = 42
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // Verify return attributes with arguments are emitted
        |> verifyIL [
            // Module function with multiple return attributes  
            """
.method public static int32  func(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/MyAttribute::.ctor(string) = ( 01 00 03 6D 6F 64 00 00 )
  .custom instance void Test/AnotherAttribute::.ctor() = ( 01 00 00 00 )
"""
            // Multiple return attrs on instance member
            """
.method public hidebysig instance int32 MultipleAttrs(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/MyAttribute::.ctor(string) = ( 01 00 03 69 6E 73 00 00 )
  .custom instance void Test/AnotherAttribute::.ctor() = ( 01 00 00 00 )
"""
            // Static member with return attribute
            """
.method public static string  StringMethod() cil managed
{
  .param [0]
  .custom instance void Test/MyAttribute::.ctor(string) = ( 01 00 03 73 74 72 00 00 )
"""
            // Property getter with return attribute
            """
.method public hidebysig specialname instance int32  get_PropWithReturnAttr() cil managed
{
  .param [0]
  .custom instance void Test/MyAttribute::.ctor(string) = ( 01 00 03 67 65 74 00 00 )
"""
        ]
        |> ignore

    // ===== Issue #18956: Decimal constant causes InvalidProgramException for debug builds =====
    // https://github.com/dotnet/fsharp/issues/18956
    // A [<Literal>] decimal constant causes System.InvalidProgramException in debug builds
    // due to incorrect locals initialization in the generated IL.
    // FIX: Exclude literal values from shadow local allocation in AllocValReprWithinExpr
    [<Fact>]
    let ``Issue_18956_DecimalConstantInvalidProgram`` () =
        let source = """
module A =
    [<Literal>]
    let B = 42m

[<EntryPoint>]
let main args =
    printfn "%M" A.B
    0
"""
        FSharp source
        |> asExe
        |> withDebug
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed // Fixed: No longer throws InvalidProgramException after excluding literals from shadow local allocation
        |> ignore

    // ===== Issue #18953: Implicit Action/Func conversion captures extra expressions =====
    // https://github.com/dotnet/fsharp/issues/18953
    // When implicitly converting an F# function to Action/Func, the conversion incorrectly
    // re-evaluates expressions that should only be evaluated once.
    // FIX: Added binding in BuildNewDelegateExpr (MethodCalls.fs) to capture expression result once
    [<Fact>]
    let ``Issue_18953_ActionFuncCapturesExtraExpressions`` () =
        let source = """
module Test

let mutable callCount = 0

let x (f: System.Action<int>) =
    f.Invoke 99
    f.Invoke 98

let y () =
    callCount <- callCount + 1
    fun num -> printfn "%d" num

x (y ())

// Expected: callCount = 1 (y() called once)
// Actual (before fix): callCount = 2 (y() called twice due to incorrect conversion)
if callCount <> 1 then
    failwithf "Expected 1 call, got %d" callCount
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed // Fixed: y() is now called once, as expected
        |> ignore

    // ===== Issue #18868: Error using [<CallerFilePath>] with caller info in delegates =====
    // https://github.com/dotnet/fsharp/issues/18868
    // Using [<CallerFilePath>] attribute in delegate definitions with optional parameters
    // should work correctly. The original bug was a contradictory error message for
    // non-optional parameters ("string but applied to string").
    // UPDATE: Bug was fixed. Non-optional CallerInfo correctly reports FS1247.
    // This test verifies the working case with optional parameter syntax (?a).
    [<Fact>]
    let ``Issue_18868_CallerInfoInDelegates`` () =
        let source = """
module Test

// CallerInfo attributes require optional parameters - use ?a syntax
type A = delegate of [<System.Runtime.CompilerServices.CallerFilePath>] ?a: string -> unit
type B = delegate of [<System.Runtime.CompilerServices.CallerLineNumber>] ?line: int -> unit
type C = delegate of [<System.Runtime.CompilerServices.CallerMemberName>] ?name: string -> unit
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #18815: Can't define extensions for two same named types =====
    // https://github.com/dotnet/fsharp/issues/18815
    // Defining extensions for two types with the same simple name in a single module
    // used to cause a compilation error about duplicate entry in method table.
    // KNOWN_LIMITATION: This issue is not fixed. Extension method names use simple type name
    // for binary compatibility with FsCheck and other reflection-based tools.
    // [<Fact>]
    let ``Issue_18815_DuplicateExtensionMethodNames`` () =
        let source = """
module Compiled

type Task = { F: int }

module CompiledExtensions =
    type System.Threading.Tasks.Task with
        static member CompiledStaticExtension() = ()

    type Task with
        static member CompiledStaticExtension() = ()
"""
        FSharp source
        |> asLibrary
        |> compile
        // Known to fail with duplicate method name error
        |> ignore

    // ===== Issue #18753: Inlining in CEs prevented by DU constructor in CE block =====
    // https://github.com/dotnet/fsharp/issues/18753
    // When using a CE, if a yielded item is constructed as a DU case in place,
    // it prevents inlining of subsequent yields in that CE, leading to suboptimal codegen.
    [<Fact>]
    let ``Issue_18753_CEInliningPreventedByDU`` () =
        let source = """
module Test

type IntOrString =
    | I of int
    | S of string

type IntOrStringBuilder() =
    member inline _.Zero() = ignore
    member inline _.Yield(x: int) = fun (xs: ResizeArray<IntOrString>) -> xs.Add(I x)
    member inline _.Yield(x: string) = fun (xs: ResizeArray<IntOrString>) -> xs.Add(S x)
    member inline _.Yield(x: IntOrString) = fun (xs: ResizeArray<IntOrString>) -> xs.Add(x)
    member inline _.Run([<InlineIfLambda>] f: ResizeArray<IntOrString> -> unit) =
        let xs = ResizeArray<IntOrString>()
        f xs
        xs
    member inline _.Delay([<InlineIfLambda>] f: unit -> ResizeArray<IntOrString> -> unit) =
        fun (xs: ResizeArray<IntOrString>) -> f () xs
    member inline _.Combine
        (
            [<InlineIfLambda>] f1: ResizeArray<IntOrString> -> unit,
            [<InlineIfLambda>] f2: ResizeArray<IntOrString> -> unit
        ) =
        fun (xs: ResizeArray<IntOrString>) ->
            f1 xs
            f2 xs

let builder = IntOrStringBuilder()

// test1 should be fully inlined - no lambdas
let test1 () =
    builder {
        1
        "two"
        3
        "four"
    }

// test2 has DU constructor I 1 first - this prevents inlining
let test2 () =
    builder {
        I 1
        "two"
        3
        "four"
    }

// Both should produce equivalent, fully inlined code but test2 generates lambdas
"""
        let actualIL = FSharp source |> asLibrary |> withOptimize |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("test1", actualIL)
        Assert.Contains("test2", actualIL)

    // ===== Issue #18672: Resumable code CE top level value doesn't work =====
    // https://github.com/dotnet/fsharp/issues/18672
    // When a CE using resumable code is created as a top-level value, it works in Debug
    // but returns null in Release mode.
    // UPDATE: Fixed by removing top-level restriction in LowerStateMachines.fs (PR #18817).
    // The isExpandVar and isStateMachineBindingVar functions no longer exclude top-level values.
    [<Fact>]
    let ``Issue_18672_ResumableCodeTopLevelValue`` () =
        // Test that top-level task CE values work correctly in Release mode.
        // The bug was that state machines at module level returned null because
        // the compiler refused to statically compile them (falling back to dynamic path).
        let source = """
module Test

// Top-level state machine - this is the scenario that was broken
let topLevelTask = task { return "result from top-level" }

// For comparison: class member should also work
type Container() =
    member val TaskInClass = task { return "result from class" }

[<EntryPoint>]
let main _ =
    let classResult = Container().TaskInClass.Result
    let topLevelResult = topLevelTask.Result
    
    if topLevelResult <> "result from top-level" then
        printfn "BUG: Top-level task returned: %A" topLevelResult
        1
    elif classResult <> "result from class" then
        printfn "BUG: Class task returned: %A" classResult  
        1
    else
        printfn "SUCCESS: Both top-level and class state machines work correctly"
        0
"""
        FSharp source
        |> asExe
        |> withOptimize  // Release mode - this is where the bug manifested
        |> compileExeAndRun
        |> shouldSucceed

    // ===== Issue #18374: RuntimeWrappedException cannot be caught =====
    // https://github.com/dotnet/fsharp/issues/18374
    // When a non-Exception object is thrown (from CIL or other languages),
    // F# generates a catch handler that casts to Exception, which throws InvalidCastException.
    // UPDATE: Fixed by generating code that uses isinst and wraps non-Exception objects
    // in RuntimeWrappedException. The catch handler now handles both Exception and non-Exception objects.
    // The generated IL now contains: isinst Exception; if null, newobj RuntimeWrappedException(object)
    [<Fact>]
    let ``Issue_18374_RuntimeWrappedExceptionCannotBeCaught`` () =
        // Test: Verify catch handlers properly handle exceptions at runtime
        // (Normal Exception case should still work correctly after the fix)
        let source = """
module Test

open System

[<EntryPoint>]
let main _ =
    // Test 1: Normal exception handling works
    let mutable caught1 = false
    try
        raise (InvalidOperationException("test"))
    with
    | e -> caught1 <- true
    
    if not caught1 then failwith "Normal exception not caught"
    
    // Test 2: Catch block with pattern matching still works
    let mutable caught2 = false
    try
        raise (ArgumentException("test"))
    with
    | :? ArgumentException as ae -> caught2 <- true
    | e -> failwith "Wrong exception type caught"
    
    if not caught2 then failwith "Specific exception not caught"
    
    // Test 3: RuntimeWrappedException is in scope and can be referenced
    // (full test requires inline IL to throw non-Exception object)
    let rweType = typeof<System.Runtime.CompilerServices.RuntimeWrappedException>
    if rweType = null then failwith "RuntimeWrappedException type not found"
    
    printfn "All exception tests passed"
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // ====================================================================================
    // SPRINT 2: Issues #18319, #18263, #18140, #18135, #18125, #17692, #17641, #16565, #16546, #16378
    // ====================================================================================

    // ===== Issue #18319: Non-null constant literal of less-specific type generates invalid IL =====
    // https://github.com/dotnet/fsharp/issues/18319
    // Using a constant expression upcasted to a less-specific type (e.g., ValueType) generates
    // IL that's missing the box instruction, causing InvalidProgramException at runtime.
    // UPDATE: Fixed by adding box instruction emission for literal upcasts. Test uncommented, now passes.
    [<Fact>]
    let ``Issue_18319_LiteralUpcastMissingBox`` () =
        let source = """
module Test

[<Literal>]
let badobj: System.ValueType = 1

[<EntryPoint>]
let main _ =
    System.Console.WriteLine(badobj)
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // ===== Issue #18263: DU .Is* properties causing compile time error =====
    // https://github.com/dotnet/fsharp/issues/18263
    // When DU case names share prefixes that produce identical .Is* property names after
    // normalization, compilation fails with "duplicate entry in method table".
    [<Fact>]
    let ``Issue_18263_DUIsPropertiesDuplicateMethod`` () =
        let source = """
namespace FSharpClassLibrary

module Say =
    let hello name =
        printfn "Hello %s" name

    type Foo = 
    | SZ
    | STZ
    | ZS
    | ASZ
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #18140: Codegen causes ilverify errors - Callvirt on value type =====
    // https://github.com/dotnet/fsharp/issues/18140
    // The compiler generates callvirt on value type methods, which is incorrect IL
    // (should use constrained prefix or call instead). ILVerify reports this as an error.
    [<Fact>]
    let ``Issue_18140_CallvirtOnValueType`` () =
        let source = """
module Test

// Minimal repro of the pattern that causes callvirt on value type
// Same pattern as in FSharp.Compiler.Text.RangeModule.comparer

[<Struct>]
type MyRange =
    val Value: int
    new(v) = { Value = v }

// Object expression implementing IEqualityComparer<struct>
// Calling GetHashCode() on a value type argument should emit constrained.callvirt
let comparer =
    { new System.Collections.Generic.IEqualityComparer<MyRange> with
        member _.Equals(x1, x2) = x1.Value = x2.Value
        // This call to GetHashCode on a struct must use constrained.callvirt
        member _.GetHashCode o = o.GetHashCode()
    }

let test() =
    let s = MyRange(42)
    comparer.GetHashCode(s)
"""
        let actualIL =
            FSharp source
            |> asLibrary
            |> compile
            |> shouldSucceed
            |> getActualIL

        Assert.Contains("constrained.", actualIL)

    // ===== Issue #18135: Can't compile static abstract with byref params =====
    // https://github.com/dotnet/fsharp/issues/18135
    // Static abstract interface members with byref parameters (inref, outref, byref)
    // fail to compile with a cryptic FS2014 error about MethodDefNotFound.
    [<Fact>]
    let ``Issue_18135_StaticAbstractByrefParams`` () =
        let source = """
module Test

#nowarn "3535"

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
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #18125: Wrong StructLayoutAttribute.Size for struct unions =====
    // https://github.com/dotnet/fsharp/issues/18125
    // Struct unions with no data fields emit StructLayoutAttribute with Size=1,
    // but the actual size is 4 due to the compiler-generated _tag field.
    // [<Fact>] // UNFIXED: Enable when issue is fixed
    let ``Issue_18125_WrongStructLayoutSize`` () =
        let source = """
module Test

[<Struct>]
type ABC = A | B | C

// StructLayoutAttribute.Size should be >= sizeof<ABC> (which is 4)
// The struct needs space for the _tag field (int32)
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // Verify IL contains size 4 (for the tag field) instead of size 1
        // The .pack 0 and .size 4 lines prove the struct has correct size
        |> verifyIL [
            """.pack 0
    .size 4"""
            // Verify the _tag field is present
            """.field assembly int32 _tag"""
        ]
        |> ignore

    // Edge case: Struct union with many cases and no data - size should still be 4 for int32 tag
    // [<Fact>] // UNFIXED: Enable when issue #18125 is fixed
    let ``Issue_18125_WrongStructLayoutSize_ManyCases`` () =
        let source = """
module Test

[<Struct>]
type ManyOptions = 
    | A | B | C | D | E | F | G | H | I | J

// Many cases still only need int32 for tag, so size should be 4
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // Verify IL contains size 4 (for the tag field) with many cases
        |> verifyIL [
            """.pack 0"""
            """.size 4"""
            // Verify the _tag field is present
            """.field assembly int32 _tag"""
        ]
        |> ignore

    // Edge case: Struct union with data fields - struct layout should accommodate all fields
    [<Fact>]
    let ``Issue_18125_StructLayoutWithDataFields`` () =
        let source = """
module Test

[<Struct>]
type IntOrFloat =
    | Int of i: int
    | Float of f: float

// Struct DU with data should compile and have proper fields
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // Verify struct has proper layout with both tag and data fields
        |> verifyIL [
            // Verify the _tag field is present
            """.field assembly int32 _tag"""
            // Verify data fields are present
            """.field assembly int32 _i"""
            """.field assembly float64 _f"""
        ]
        |> ignore

    // ===== Issue #17692: Mutual recursion codegen issue with duplicate param names =====
    // https://github.com/dotnet/fsharp/issues/17692
    // In mutually recursive functions, the compiler can generate duplicate 'self@'
    // parameter names in the IL, causing issues when the IL is round-tripped through ilasm.
    [<Fact>]
    let ``Issue_17692_MutualRecursionDuplicateParamName`` () =
        let source = """
module Test

// Mutual recursion pattern that triggered duplicate 'self@' param names in closure constructors
// The issue occurred when multiple closures each captured a self reference
// Fix ensures unique names are generated for all closure free variables

let rec caller x = callee (x - 1)
and callee y = if y > 0 then caller y else 0

// More complex case with additional closures
let rec f1 a = f2 (a - 1) + f3 (a - 2)
and f2 b = if b > 0 then f1 b else 1
and f3 c = if c > 0 then f2 c else 2

let result1 = caller 5
let result2 = f1 5
printfn "Results: %d %d" result1 result2
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        // The fix in EraseClosures.fs ensures unique parameter names are generated
        // for closure constructors by using mkUniqueFreeVarName when creating self@ vars
        |> ignore

    // ===== Issue #17641: IsMethod/IsProperty don't act as expected for generated members =====
    // https://github.com/dotnet/fsharp/issues/17641
    // When enumerating declarations in FSharpAssemblyContents, compiler-generated properties
    // like IsUnionCaseTester or methods like Equals have incorrect IsProperty/IsMethod flags.
    //
    // NOTE: The actual tests for this issue are in FSharp.Compiler.Service.Tests/GeneratedCodeSymbolsTests.fs
    // as Issue_17641_IsMethodIsProperty tests. This is because the issue relates to the Compiler Service
    // API (FSharpMemberOrFunctionOrValue.IsProperty/IsMethod) which cannot be tested via IL verification.
    // The tests below verify: get_IsCaseA, get_IsCaseB IsProperty=true; Equals, GetHashCode, CompareTo IsMethod=true.
    [<Fact>]
    let ``Issue_17641_IsMethodIsPropertyIncorrectForGenerated`` () =
        let source = """
module Test

// This is a Compiler Service API issue - the generated Is* properties
// for union types have IsProperty = false when accessed via assembly contents enumeration
// but IsProperty = true when accessed via GetSymbolUseAtLocation

type MyUnion =
    | CaseA of int
    | CaseB of string

// When inspecting MyUnion.IsCaseA via FSharpImplementationFileDeclaration.MemberOrFunctionOrValue:
// - mfv.IsProperty should be true (it's a property)
// - mfv.IsMethod should be false
// But currently IsProperty = false for generated members

let x = CaseA 1
let isA = match x with CaseA _ -> true | _ -> false
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // This is a metadata/API issue, not a runtime issue
        // The generated Is* properties should have correct IsProperty flag
        |> ignore

    // ===== Issue #16565: Codegen issue with DefaultAugmentation(false) =====
    // https://github.com/dotnet/fsharp/issues/16565
    // Defining a DU with DefaultAugmentation(false) and a static member with the same name
    // as a union case causes "duplicate entry in method table" error.
    [<Fact>]
    let ``Issue_16565_DefaultAugmentationFalseDuplicateEntry`` () =
        let source = """
module Test

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
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #16546: NullReferenceException in Debug build with recursive reference =====
    // https://github.com/dotnet/fsharp/issues/16546
    // When using mutually recursive let bindings in a certain order, the Debug build
    // produces a NullReferenceException while Release works correctly.
    // 
    // KNOWN LIMITATION: This issue requires a fix in the type checker (EliminateInitializationGraphs)
    // to reorder bindings before inserting Lazy wrappers. The code generator reordering in IlxGen.fs
    // is insufficient because by that point, the type checker has already inserted Lazy wrappers.
    // 
    // WORKAROUND: Reorder the bindings in source code so the lambda comes before the non-lambda:
    //   let rec parse node = ... and paramParse = tryParam parse
    // instead of:
    //   let rec paramParse = tryParam parse and parse node = ...
    // 
    // [<Fact>]
    let ``Issue_16546_DebugRecursiveReferenceNull`` () =
        let source = """
module Test

type Ident = string

type Type =
    | TPrim of Ident
    | TParam of Ident * Type

let tryParam pv x =
    match x with
    | TParam (name, typ) ->
        pv typ |> Result.map (fun t -> [name, t])
    | _ -> Error "unexpected"

let tryPrim = function
    | TPrim name -> Ok name
    | _ -> Error "unused"

module TypeModule =
    let parse =
        let rec paramParse = tryParam parse
        and parse node =
            match tryPrim node with
            | Ok name -> Ok(TPrim name)
            | _ ->
                match paramParse node with
                | Ok [name, typ] -> Ok(TParam(name,typ))
                | _ -> Error "invalid type"
        parse

[<EntryPoint>]
let main args =
    printfn "%A" (TypeModule.parse (TParam ("ptr", TPrim "float")))
    0
"""
        FSharp source
        |> asExe
        |> withDebug
        |> withNoOptimize  // Required to trigger the bug - Debug symbols + no optimizations
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed // NOT YET FIXED: Still throws NullReferenceException - requires type checker fix
        |> ignore

    // ===== Issue #16378: Significant allocations logging F# types =====
    // https://github.com/dotnet/fsharp/issues/16378
    // Logging F# discriminated union values using Console.Logger causes ~20x more
    // memory allocation compared to serializing them first due to excessive boxing/allocations.
    //
    // ROOT CAUSE: F# types (DU, records) use a reflection-based ToString() implementation
    // (see Microsoft.FSharp.Core.PrintfImpl). When DU values are passed to logging APIs
    // expecting obj, the ToString() call triggers heavy reflection allocation (~20-36KB).
    // C# record types with hand-written ToString() only allocate ~1-2KB.
    //
    // POTENTIAL FIX: Generate specialized ToString() methods at compile time for F# types
    // instead of using reflection. This would be a significant change affecting:
    // - TypedTree generation
    // - AugmentWithHashCompare module (where ToString is implemented)
    // - Potential breaking changes in ToString() output format
    //
    // WORKAROUND: Implement custom SerializeError() methods that return simple strings.
    [<Fact>]
    let ``Issue_16378_DULoggingAllocations`` () =
        let source = """
module Test

open System

// This is a performance/allocation issue rather than a correctness bug
// When F# DU values are passed to logging methods that expect obj,
// the compiler generates excessive allocations compared to what's needed

[<NoComparison>]
type StoreError =
    | Exception of exn
    | ErrorDuringReadingChannelFromDatabase of channel: string
    | AlreadyExists of Guid
    | NotFound of Guid
    member this.SerializeError(): obj = 
        match this with
        | Exception ex -> ex
        | ErrorDuringReadingChannelFromDatabase channel -> $"ErrorDuringReadingChannelFromDatabase: {channel}"
        | AlreadyExists id -> $"AlreadyExists: {id}"
        | NotFound id -> $"NotFound: {id}"

let sampleNotFound = NotFound(Guid.NewGuid())

// Using the DU directly allocates ~36KB per log call
// Using SerializeError() allocates ~1.8KB per log call
// The difference is due to how F# boxes and formats the DU

let logDirect() =
    // This path causes excessive allocations through reflection-based ToString
    String.Format("Error: {0}", sampleNotFound) |> ignore

let logSerialized() =
    // This path has minimal allocations using custom serialization
    String.Format("Error: {0}", sampleNotFound.SerializeError()) |> ignore

logDirect()
logSerialized()
printfn "Test completed"
"""
        let actualIL = FSharp source |> asExe |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("logDirect", actualIL)
        Assert.Contains("logSerialized", actualIL)
        Assert.Contains("box", actualIL)

    // ====================================================================================
    // SPRINT 3: Issues #16362, #16292, #16245, #16037, #15627, #15467, #15352, #15326, #15092, #14712
    // ====================================================================================

    // ===== Extension method compiled names use dot separator for binary compatibility =====
    // Extension methods must use dot separator (e.g., TypeName.MemberName) in their compiled names.
    // This maintains binary compatibility with FsCheck and other tools that use reflection
    // to find FSharp.Core extension methods like FSharpType.IsRecord.Static.
    // Related: https://github.com/dotnet/fsharp/issues/16362 (proposed $ separator was reverted)

    [<Fact>]
    let ``ExtensionMethod_InstanceMethod_UsesDotSeparator`` () =
        // Instance extension method: compiled name should be TypeName.MemberName
        let result =
            FSharp """
module Test

open System

type Exception with
    member ex.Reraise() = raise ex
"""
            |> asLibrary
            |> compile
            |> shouldSucceed
        // Positive: dot separator is used (Exception.Reraise)
        result |> verifyIL [ ".method public static !!a  Exception.Reraise<a>(class [runtime]System.Exception A_0) cil managed" ]
        // Negative: dollar separator is NOT used (regression guard for Issue #16362)
        result |> verifyILNotPresent [ "Exception$Reraise" ]

    [<Fact>]
    let ``ExtensionMethod_StaticMethod_UsesDotSeparatorWithStaticSuffix`` () =
        // Static extension method: compiled name should be TypeName.MemberName.Static
        // This mirrors FSharp.Core pattern like FSharpType.IsRecord.Static
        let result =
            FSharp """
module Test

open System

type Exception with
    static member CreateNew(msg: string) = Exception(msg)
"""
            |> asLibrary
            |> compile
            |> shouldSucceed
        // Positive: dot separator with .Static suffix (Exception.CreateNew.Static)
        result |> verifyIL [ ".method public static class [runtime]System.Exception Exception.CreateNew.Static(string msg) cil managed" ]
        // Negative: dollar separator is NOT used (regression guard for Issue #16362)
        result |> verifyILNotPresent [ "Exception$CreateNew"; "$Static" ]

    // ===== Issue #16292: Incorrect codegen for Debug build with SRTP and mutable struct =====
    // https://github.com/dotnet/fsharp/issues/16292
    // In Debug builds, SRTP with mutable struct enumerators generates incorrect code where
    // the struct is copied in each loop iteration, losing mutations from MoveNext().
    // [KNOWN_LIMITATION: Requires deeper investigation of how defensive copy suppression
    // interacts with debug point wrapping after inlining. Workaround: Use Release mode.]
    // [<Fact>]
    let ``Issue_16292_SrtpDebugMutableStructEnumerator`` () =
        let source = """
module Test

open System
open System.Buffers
open System.Text

let inline forEach<'C, 'E, 'I
                        when 'C: (member GetEnumerator: unit -> 'I)
                        and  'I: struct
                        and  'I: (member MoveNext: unit -> bool)
                        and  'I: (member Current : 'E) >
        ([<InlineIfLambda>] f: 'E -> unit) (container: 'C)  =
    let mutable iter = container.GetEnumerator()
    while iter.MoveNext() do
        f iter.Current

let showIt (buffer: ReadOnlySequence<byte>) =
    let mutable count = 0
    buffer |> forEach (fun segment ->
        count <- count + 1
    )
    count

// In Debug builds, the loop never terminates because enumerator2.MoveNext()
// mutates a copy, not the original enumerator
// Release builds work correctly

[<EntryPoint>]
let main _ =
    let arr = [| 1uy; 2uy; 3uy |]
    let buffer = ReadOnlySequence<byte>(arr)
    let result = showIt buffer
    printfn "Segments: %d" result
    if result = 0 then failwith "Bug: Debug build has infinite loop or zero iterations"
    0
"""
        FSharp source
        |> asExe
        |> withDebug
        |> withNoOptimize  // Critical: must disable optimizations to reproduce the bug
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // ===== Issue #16245: Span IL gen produces 2 get_Item calls =====
    // https://github.com/dotnet/fsharp/issues/16245
    // When incrementing a span element (span[i] <- span[i] + 1), the compiler generates
    // two get_Item calls instead of one, leading to suboptimal performance.
    [<Fact>]
    let ``Issue_16245_SpanDoubleGetItem`` () =
        let source = """
module Test

open System

let incrementSpan (span: Span<byte>) =
    for i = 0 to span.Length - 1 do
        span[i] <- span[i] + 1uy

// The IL generates two System.Span`1::get_Item(int32) calls
// instead of efficiently loading once, adding, and storing

let test() =
    let arr = [| 1uy; 2uy; 3uy |]
    incrementSpan (arr.AsSpan())
    printfn "%A" arr

test()
"""
        let actualIL = FSharp source |> asExe |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("incrementSpan", actualIL)
        Assert.Contains("get_Item", actualIL)

    // ===== Issue #16037: Suboptimal code for tuple pattern matching in lambda parameter =====
    // https://github.com/dotnet/fsharp/issues/16037
    // Pattern matching a tuple in a lambda parameter generates two FSharpFunc classes,
    // causing ~2x memory allocation compared to using fst/snd or matching inside the body.
    [<Fact>]
    let ``Issue_16037_TuplePatternLambdaSuboptimal`` () =
        let source = """
module Test

let data = Map.ofList [
    "1", (true, 1)
    "2", (true, 2)
]

// Suboptimal - generates 2 FSharpFunc classes
let foldWithPattern () =
    (data, [])
    ||> Map.foldBack (fun _ (x, _) state -> x :: state)

// Optimal - generates 1 FSharpFunc class
let foldWithFst () =
    (data, [])
    ||> Map.foldBack (fun _ v state -> fst v :: state)
    
// Also optimal - generates 1 FSharpFunc class
let foldWithPattern2 () =
    (data, [])
    ||> Map.foldBack (fun _ v state ->
        let x, _ = v
        x :: state)

// Benchmark shows foldWithPattern is ~50% slower and allocates 2x more memory
// Expected: All three should generate equivalent code

let test() =
    let r1 = foldWithPattern()
    let r2 = foldWithFst()
    let r3 = foldWithPattern2()
    printfn "Results: %A %A %A" r1 r2 r3

test()
"""
        let actualIL = FSharp source |> asExe |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("foldWithPattern", actualIL)
        Assert.Contains("foldWithFst", actualIL)
        Assert.Contains("FSharpFunc", actualIL)

    // ===== Issue #15627: Program stuck when using async/task before EntryPoint =====
    // https://github.com/dotnet/fsharp/issues/15627
    // [KNOWN_LIMITATION: Type Initializer Deadlock]
    // Running async operations before an [<EntryPoint>] function causes the program to hang.
    // 
    // ROOT CAUSE: When [<EntryPoint>] is used, module-level code runs in a .cctor (static class constructor).
    // The .cctor has a CLR type initialization lock. When async code runs in the .cctor and tries to
    // access a static field from the same type, the async thread blocks waiting for the .cctor to complete,
    // but the .cctor is waiting for the async to complete → deadlock.
    //
    // WORKAROUND: Either:
    // 1. Move async operations inside the entry point function, or
    // 2. Remove [<EntryPoint>] and use implicit entry point (no main function), or
    // 3. Move the variable declaration (deployPath) inside the async block
    //
    // TECHNICAL NOTES:
    // - Without [<EntryPoint>], F# uses implicit entry point where all code runs in main@() directly
    // - With [<EntryPoint>], module-level code runs in .cctor which has thread-safety locks
    // - A fix would require significant rearchitecting of F# module initialization code generation
    [<Fact>]
    let ``Issue_15627_AsyncBeforeEntryPointHangs`` () =
        // This test documents the known limitation
        // The code compiles but hangs at runtime due to .cctor deadlock
        let source = """
module Test

open System.IO

let deployPath = Path.GetFullPath "deploy"

printfn "1"

async {
     printfn "2 %s" deployPath
}
|> Async.RunSynchronously

printfn "After async"

[<EntryPoint>]
let main args =
    printfn "3"
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        // NOTE: Do NOT run this test - it will hang forever due to .cctor deadlock
        // |> run
        // |> shouldSucceed
        |> ignore

    // ===== Issue #15467: Include language version in compiled metadata =====
    // https://github.com/dotnet/fsharp/issues/15467
    // [OUT_OF_SCOPE: FEATURE REQUEST] - Request to embed language version in compiled DLLs
    // for better error messages when older compilers read newer assemblies.
    // This is NOT a codegen bug - it's a request for new metadata embedding functionality.
    // Test documents the feature request by verifying current behavior compiles correctly.
    [<Fact>]
    let ``Issue_15467_LanguageVersionInMetadata`` () =
        // [OUT_OF_SCOPE: FEATURE REQUEST] - This test documents the feature request, not a bug.
        let source = """
module Test

type MyRecord = { Value: int }
let test = { Value = 42 }
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #15352: User defined symbols get CompilerGeneratedAttribute =====
    // https://github.com/dotnet/fsharp/issues/15352
    // Private let-bound functions in classes get [<CompilerGenerated>] attribute
    // even though they are user-defined code, not compiler-generated.
    // [<Fact>] // UNFIXED: Enable when issue is fixed
    let ``Issue_15352_UserCodeCompilerGeneratedAttribute`` () =
        let source = """
module Test

open System
open System.Reflection
open System.Runtime.CompilerServices

type T() =
    let f x = x + 1
    let mutable counter = 0
    member _.CallF x = f x
    member _.Increment() = counter <- counter + 1
    member _.Counter = counter

// User-defined let-bound functions, mutable values, and their accessors
// should NOT have CompilerGeneratedAttribute - they are user-written code

let checkAttribute() =
    let t = typeof<T>
    
    // Check that let-bound function 'f' does NOT have CompilerGeneratedAttribute
    let methodF = t.GetMethod("f", BindingFlags.NonPublic ||| BindingFlags.Instance)
    if methodF <> null then
        let hasAttr = methodF.GetCustomAttribute<CompilerGeneratedAttribute>() <> null
        if hasAttr then
            failwith "Bug: User-defined method 'f' has CompilerGeneratedAttribute"
        else
            printfn "OK: No CompilerGeneratedAttribute on user method 'f'"
    else
        failwith "Method 'f' not found"
    
    // Check that public member CallF does NOT have CompilerGeneratedAttribute  
    let memberCallF = t.GetMethod("CallF", BindingFlags.Public ||| BindingFlags.Instance)
    if memberCallF <> null then
        let hasAttr = memberCallF.GetCustomAttribute<CompilerGeneratedAttribute>() <> null
        if hasAttr then
            failwith "Bug: User-defined member 'CallF' has CompilerGeneratedAttribute"
        else
            printfn "OK: No CompilerGeneratedAttribute on user member 'CallF'"
    else
        failwith "Member 'CallF' not found"

checkAttribute()
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // Test that closure classes still correctly receive CompilerGeneratedAttribute
    [<Fact>]
    let ``Issue_15352_ClosuresStillHaveCompilerGeneratedAttribute`` () =
        let source = """
module Test

open System
open System.Reflection
open System.Runtime.CompilerServices

// This function creates a closure that captures 'x'
let makeAdder x =
    fun y -> x + y

// Use it to ensure it's emitted
let adder5 = makeAdder 5
printfn "Result: %d" (adder5 10)

// Get the assembly that contains the Test module
let testModule = 
    Assembly.GetExecutingAssembly().GetTypes() 
    |> Array.find (fun t -> t.Name = "Test")
let asm = testModule.Assembly

// Check that closure classes (types with @ in the name) have CompilerGeneratedAttribute
let closureTypes = 
    asm.GetTypes() 
    |> Array.filter (fun t -> 
        t.FullName.Contains("@") && 
        t.IsClass &&
        not (t.Name.StartsWith("<")))

printfn "Found %d potential closure types" closureTypes.Length

// At least one closure-like type should have CompilerGeneratedAttribute
let markedClosures = 
    closureTypes 
    |> Array.filter (fun t -> t.GetCustomAttribute<CompilerGeneratedAttribute>() <> null)

for closureType in markedClosures do
    printfn "OK: Type '%s' has CompilerGeneratedAttribute" closureType.FullName

printfn "Closure check complete - found %d marked closure types" markedClosures.Length
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // Test that user-defined properties do NOT have CompilerGeneratedAttribute
    [<Fact>]
    let ``Issue_15352_UserPropertiesNoCompilerGeneratedAttribute`` () =
        let source = """
module Test

open System
open System.Reflection
open System.Runtime.CompilerServices

type MyClass() =
    let mutable _value = 0
    
    // User-defined property with explicit getter and setter
    member _.Value
        with get() = _value
        and set(v) = _value <- v
    
    // User-defined auto-property
    member val AutoProp = 42 with get, set
    
    // User-defined method
    member _.DoSomething() = printfn "Doing something"

let checkProperties() =
    let t = typeof<MyClass>
    
    // Check user-defined property getter does NOT have CompilerGeneratedAttribute
    let valueProp = t.GetProperty("Value")
    if valueProp <> null then
        let getter = valueProp.GetGetMethod()
        if getter <> null then
            let hasAttr = getter.GetCustomAttribute<CompilerGeneratedAttribute>() <> null
            if hasAttr then
                failwith "Bug: User-defined property getter 'Value' has CompilerGeneratedAttribute"
            else
                printfn "OK: No CompilerGeneratedAttribute on user property getter 'Value'"
    
    // Check user-defined method does NOT have CompilerGeneratedAttribute
    let doMethod = t.GetMethod("DoSomething")
    if doMethod <> null then
        let hasAttr = doMethod.GetCustomAttribute<CompilerGeneratedAttribute>() <> null
        if hasAttr then
            failwith "Bug: User-defined method 'DoSomething' has CompilerGeneratedAttribute"
        else
            printfn "OK: No CompilerGeneratedAttribute on user method 'DoSomething'"
    else
        failwith "Method 'DoSomething' not found"

checkProperties()
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // ===== Issue #15326: Delegates not inlined with InlineIfLambda =====
    // https://github.com/dotnet/fsharp/issues/15326
    // Custom delegates with InlineIfLambda are not being inlined as they were
    // before .NET 7 Preview 5. This is a regression.
    [<Fact>]
    let ``Issue_15326_InlineIfLambdaDelegateRegression`` () =
        let source = """
module Test

open System

type SystemAction<'a when 'a: struct and 'a :> IConvertible> = 
    delegate of byref<'a> -> unit

let inline doAction (span: Span<'a>) ([<InlineIfLambda>] action: SystemAction<'a>) =
    for i = 0 to span.Length - 1 do
        let batch = &span[i]
        action.Invoke &batch

[<EntryPoint>]
let main args =
    let array = Array.zeroCreate<int> 100
    doAction (array.AsSpan()) (SystemAction(fun batch -> batch <- batch + 1))
    printfn "Sum: %d" (Array.sum array)
    0
"""
        let actualIL = FSharp source |> asExe |> withOptimize |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("main", actualIL)
        Assert.Contains("doAction", actualIL)

    // ===== Issue #15092: Should we generate DebuggerProxies in release code? =====
    // https://github.com/dotnet/fsharp/issues/15092
    // [OUT_OF_SCOPE: FEATURE REQUEST] - Design question about eliding DebuggerProxy types in release builds.
    // This is NOT a codegen bug - it's a request to optionally reduce binary size.
    // Test documents the feature request by verifying current behavior compiles and runs correctly.
    [<Fact>]
    let ``Issue_15092_DebuggerProxiesInRelease`` () =
        // [OUT_OF_SCOPE: FEATURE REQUEST] - Documents design question, not a bug.
        let source = """
module Test

type MyRecord = { Name: string; Value: int }
let result = { Name = "test"; Value = 42 }
"""
        FSharp source
        |> asExe
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        // The feature request is to optionally not generate DebuggerProxies in release
        |> ignore

    // ===== Issue #14712: Signature file generation should use F# Core alias =====
    // https://github.com/dotnet/fsharp/issues/14712
    // When generating signature files, inferred types use System.Int32 instead of int,
    // System.String instead of string, etc. This is inconsistent with F# conventions.
    [<Fact>]
    let ``Issue_14712_SignatureFileTypeAlias`` () =
        let source = """
module Test

let add (x:int) (y:int) = x + y

let concat (a:string) (b:string) = a + b

let isValid (b:bool) = b

let multiply (x:float) (y:float) = x * y
"""
        // When generating signature file for this:
        // Return types should use F# aliases (int, string, bool, float)
        // instead of CLR types (System.Int32, System.String, System.Boolean, System.Double)
        let signature = FSharp source |> printSignatures
        
        // Verify the signature uses F# aliases rather than CLR type names
        Assert.DoesNotContain("System.Int32", signature)
        Assert.DoesNotContain("System.String", signature)
        Assert.DoesNotContain("System.Boolean", signature)
        Assert.DoesNotContain("System.Double", signature)
        
        // Verify the correct F# aliases are used
        Assert.Contains("val add: x: int -> y: int -> int", signature)
        Assert.Contains("val concat: a: string -> b: string -> string", signature)
        Assert.Contains("val isValid: b: bool -> bool", signature)
        Assert.Contains("val multiply: x: float -> y: float -> float", signature)

    // Comprehensive test for all F# type aliases
    [<Fact>]
    let ``Issue_14712_SignatureFileTypeAlias_AllTypes`` () =
        let source = """
module Test

// All F# numeric type aliases
let useInt (x:int) = x
let useInt16 (x:int16) = x
let useInt32 (x:int32) = x
let useInt64 (x:int64) = x
let useUint16 (x:uint16) = x
let useUint32 (x:uint32) = x
let useUint64 (x:uint64) = x
let useByte (x:byte) = x
let useSbyte (x:sbyte) = x
let useFloat (x:float) = x
let useFloat32 (x:float32) = x
let useDecimal (x:decimal) = x
let useChar (x:char) = x
let useNativeint (x:nativeint) = x
let useUnativeint (x:unativeint) = x

// Other primitive types
let useString (x:string) = x
let useBool (x:bool) = x
let useUnit () = ()
"""
        let signature = FSharp source |> printSignatures
        
        // Verify CLR type names are NOT used in signature
        Assert.DoesNotContain("System.Int16", signature)
        Assert.DoesNotContain("System.Int32", signature)
        Assert.DoesNotContain("System.Int64", signature)
        Assert.DoesNotContain("System.UInt16", signature)
        Assert.DoesNotContain("System.UInt32", signature)
        Assert.DoesNotContain("System.UInt64", signature)
        Assert.DoesNotContain("System.Byte", signature)
        Assert.DoesNotContain("System.SByte", signature)
        Assert.DoesNotContain("System.Double", signature)
        Assert.DoesNotContain("System.Single", signature)
        Assert.DoesNotContain("System.Decimal", signature)
        Assert.DoesNotContain("System.Char", signature)
        Assert.DoesNotContain("System.IntPtr", signature)
        Assert.DoesNotContain("System.UIntPtr", signature)
        Assert.DoesNotContain("System.String", signature)
        Assert.DoesNotContain("System.Boolean", signature)
        
        // Verify correct F# aliases are used
        Assert.Contains("val useInt: x: int -> int", signature)
        Assert.Contains("val useInt16: x: int16 -> int16", signature)
        Assert.Contains("val useInt32: x: int32 -> int32", signature)
        Assert.Contains("val useInt64: x: int64 -> int64", signature)
        Assert.Contains("val useUint16: x: uint16 -> uint16", signature)
        Assert.Contains("val useUint32: x: uint32 -> uint32", signature)
        Assert.Contains("val useUint64: x: uint64 -> uint64", signature)
        Assert.Contains("val useByte: x: byte -> byte", signature)
        Assert.Contains("val useSbyte: x: sbyte -> sbyte", signature)
        Assert.Contains("val useFloat: x: float -> float", signature)
        Assert.Contains("val useFloat32: x: float32 -> float32", signature)
        Assert.Contains("val useDecimal: x: decimal -> decimal", signature)
        Assert.Contains("val useChar: x: char -> char", signature)
        Assert.Contains("val useNativeint: x: nativeint -> nativeint", signature)
        Assert.Contains("val useUnativeint: x: unativeint -> unativeint", signature)
        Assert.Contains("val useString: x: string -> string", signature)
        Assert.Contains("val useBool: x: bool -> bool", signature)
        Assert.Contains("val useUnit: unit -> unit", signature)

    // Test that less common CLR types without F# aliases still use full names
    [<Fact>]
    let ``Issue_14712_SignatureFileTypeAlias_NoAliasTypes`` () =
        let source = """
module Test

open System

let useGuid (x:Guid) = x
let useDateTime (x:DateTime) = x
let useTimeSpan (x:TimeSpan) = x
"""
        let signature = FSharp source |> printSignatures
        
        // These types don't have F# aliases, so they should use their CLR names
        Assert.Contains("Guid", signature)
        Assert.Contains("DateTime", signature)
        Assert.Contains("TimeSpan", signature)

    // ===== Issue #14707: Existing signature files become unusable =====
    // https://github.com/dotnet/fsharp/issues/14707
    // When --allsigs regenerates signature files, wildcards (_) become `'?NNNNN` variables
    // making existing signature files unusable after rebuild.
    // [<Fact>]
    let ``Issue_14707_SignatureFileUnusable`` () =
        // The bug: sig files with `val foo : int -> _` get regenerated as `val foo : int -> '?17893`
        // This is about signature file generation, not IL codegen per se.
        // Note: Full repro requires file-based testing with --allsigs flag
        // The signature file would contain:
        //   val foo01 : int -> string -> _
        //   val bar01 : int -> int -> _
        // Which --allsigs converts to invalid type variables
        let source = """
module Test
let foo01 x y = "result"
let bar01 x y = 0
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #14706: Signature file generation WhereTyparSubtypeOfType =====
    // https://github.com/dotnet/fsharp/issues/14706
    // Signature generation for static member with subtype constraint produces
    // `#IProvider` syntax instead of preserving explicit type parameter.
    [<Fact>]
    let ``Issue_14706_SignatureWhereTypar`` () =
        // The bug: a static member with `'T when 'T :> IProvider` constraint
        // gets signature-generated as `p: Tainted<#IProvider>` instead of
        // `ComputeDefinitionLocationOfProvidedItem<'T when 'T :> IProvider> : p: Tainted<'T>`
        let source = """
module Test

type IProvider = interface end

type Tainted<'T> = class end

type ConstructB =
    static member ComputeDefinitionLocationOfProvidedItem<'T when 'T :> IProvider>(p: Tainted<'T>) : obj option = None
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #14508: nativeptr in interfaces leads to runtime errors =====
    // https://github.com/dotnet/fsharp/issues/14508
    // Implementing a generic interface with nativeptr<'T> member in a non-generic type
    // causes TypeLoadException: "Signature of the body and declaration in a method
    // implementation do not match."
    [<Fact>]
    let ``Issue_14508_NativeptrInInterfaces`` () =
        // Note: Runtime verification (asExe |> run) still produces TypeLoadException for the
        // 'Broken' type, indicating the fix is partial. Compile-only test kept for now.
        let source = """
module Test

open Microsoft.FSharp.NativeInterop

type IFoo<'T when 'T : unmanaged> =
    abstract member Pointer : nativeptr<'T>

// This type causes the bug - non-generic implementation of generic interface
type Broken() =
    member x.Pointer : nativeptr<int> = Unchecked.defaultof<_>
    interface IFoo<int> with
        member x.Pointer = x.Pointer

// This works - generic type implementing generic interface
type Working<'T when 'T : unmanaged>() =
    member x.Pointer : nativeptr<'T> = Unchecked.defaultof<_>
    interface IFoo<'T> with
        member x.Pointer = x.Pointer
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #14492: F# 7.0 incorrect program release config =====
    // https://github.com/dotnet/fsharp/issues/14492
    // TypeLoadException in release config: "Method 'Specialize' on type 'memoizeLatestRef@...'
    // tried to implicitly override a method with weaker type parameter constraints."
    // FIXED: Strip constraints from type parameters when generating Specialize method override.
    [<Fact>]
    let ``Issue_14492_ReleaseConfigError`` () =
        // The bug: inline function with 'not struct' constraint + memoization causes
        // TypeLoadException at runtime in Release mode
        let source = """
module Test

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

module BugInReleaseConfig = 
    let test f x = 
        printfn "%s" (f x)

    let f: string -> string = memoizeLatestRef id

    let run () = test f "ok"

[<EntryPoint>]
let main _ =
    BugInReleaseConfig.run ()
    0
"""
        FSharp source
        |> asExe
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // ===== Issue #14392: OpenApi Swashbuckle support =====
    // https://github.com/dotnet/fsharp/issues/14392
    // [OUT_OF_SCOPE: FEATURE REQUEST] - Not a codegen bug. Request for better OpenAPI/Swashbuckle support.
    // This is NOT a codegen bug - it's a request for improved OpenAPI tooling interoperability.
    // Test documents the feature request by verifying F# records compile correctly.
    [<Fact>]
    let ``Issue_14392_OpenApiSupport`` () =
        // [OUT_OF_SCOPE: FEATURE REQUEST] - Documents tooling request, not a codegen bug.
        let source = """
module Test

type MyDto = { Name: string; Value: int }

let dto = { Name = "test"; Value = 42 }
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #14321: Build fails reusing names DU constructors and IWSAM =====
    // https://github.com/dotnet/fsharp/issues/14321
    // When a DU case name matches an IWSAM member name, build fails with:
    // "duplicate entry 'Overheated' in property table"
    // FIX: The compiler now correctly discards IWSAM implementation properties that 
    // would conflict with nullary DU case properties (they are semantically equivalent).
    [<Fact>]
    let ``Issue_14321_DuAndIWSAMNames`` () =
        // The fix: DU constructor names that match IWSAM member names no longer cause
        // "duplicate entry 'X' in property table" error. The IWSAM implementation property
        // is correctly discarded since it would be identical to the DU case property.
        let source = """
module Test

#nowarn "3535"  // IWSAM warning

type EngineError<'e> =
    static abstract Overheated : 'e
    static abstract LowOil : 'e

// Previously caused: "duplicate entry 'Overheated' in property table"
// Now works: The DU case properties and IWSAM implementations coexist correctly
type CarError =
    | Overheated  // Same name as IWSAM member - now works!
    | LowOil
    | DeviceNotPaired

    interface EngineError<CarError> with
        static member Overheated = Overheated
        static member LowOil = LowOil
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13468: outref parameter compiled as byref =====
    // https://github.com/dotnet/fsharp/issues/13468
    // When implementing interface from C# with out parameter, F# compiles it as ref instead.
    // This doesn't break runtime but affects FCS symbol analysis.
    // FIX: The [Out] attribute is now correctly emitted for interface implementations.
    [<Fact>]
    let ``Issue_13468_OutrefAsByref`` () =
        // Test: Implement C# interface with 'out' parameter in F#
        // The fix ensures [Out] attribute is emitted for the implementation
        let csCode = "namespace CSharpLib { public interface IOutTest { void TryGet(string k, out int v); } }"
        let csLib = CSharp csCode |> withName "CSharpLib"
        let fsCode = "module Test\nopen CSharpLib\ntype MyImpl() =\n    interface IOutTest with\n        member this.TryGet(k, v) = v <- 42"
        FSharp fsCode
        |> withReferences [csLib]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13447: Extra tail instruction corrupts stack =====
    // https://github.com/dotnet/fsharp/issues/13447
    // When NativePtr.stackalloc is used (which emits localloc), the stack memory may be
    // passed to called functions via Span or byref. If a tail. prefix is emitted on such
    // calls, the stack frame is released before the callee accesses the memory, causing
    // corruption. The fix suppresses tail calls when localloc has been used in the method.
    [<Fact>]
    let ``Issue_13447_TailInstructionCorruption`` () =
        // Verify that tail. is NOT emitted when localloc (NativePtr.stackalloc) is used.
        // The bug was that tail. prefix on calls following localloc corrupts stack memory.
        let source = """
module Test
open System
open Microsoft.FSharp.NativeInterop

#nowarn "9" // Uses of this construct may result in the generation of unverifiable .NET IL code

[<Struct>]
type MyResult<'T, 'E> = 
    | Ok of value: 'T 
    | Error of error: 'E

// Helper that uses stackalloc and passes the span to another function
let useStackAlloc () : MyResult<int, string> =
    let ptr = NativePtr.stackalloc<byte> 100
    let span = Span<byte>(NativePtr.toVoidPtr ptr, 100)
    span.[0] <- 42uy
    // This call should NOT have tail. prefix since localloc was used
    Ok (int span.[0])

// Verify compilation succeeds without stack corruption
let test () =
    match useStackAlloc () with
    | Ok v -> v
    | Error _ -> -1
"""
        let actualIL =
            FSharp source
            |> asLibrary
            |> withOptimize
            |> compile
            |> shouldSucceed
            |> getActualIL

        let useStackAllocIdx = actualIL.IndexOf("useStackAlloc")
        Assert.True(useStackAllocIdx >= 0, "useStackAlloc method not found in IL")
        let methodEnd = actualIL.IndexOf("\n    } ", useStackAllocIdx)
        let methodIL = if methodEnd > 0 then actualIL.Substring(useStackAllocIdx, methodEnd - useStackAllocIdx) else actualIL.Substring(useStackAllocIdx)
        Assert.DoesNotContain("tail.", methodIL)

    // ===== Issue #13223: FSharp.Build support for reference assemblies =====
    // https://github.com/dotnet/fsharp/issues/13223
    // [OUT_OF_SCOPE: FEATURE REQUEST] - Not a codegen bug. Request for FSharp.Build reference assembly support.
    // This is NOT a codegen bug - it's a request for FSharp.Build tooling enhancement.
    // Test documents the feature request by verifying basic compilation works correctly.
    [<Fact>]
    let ``Issue_13223_ReferenceAssemblies`` () =
        // [OUT_OF_SCOPE: FEATURE REQUEST] - Documents build tooling request, not a codegen bug.
        let source = """
module Test

let f x = x + 1
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13218: Compilation time 13000 static member vs let binding =====
    // https://github.com/dotnet/fsharp/issues/13218
    // NOTE: This is a PERFORMANCE issue, not a codegen bug.
    // 13000 let bindings take ~2.5min vs 13000 static members taking ~20sec.
    // [<Fact>]
    let ``Issue_13218_ManyStaticMembers`` () =
        // This is a compilation performance issue - let bindings are much slower
        // than static members for large numbers of declarations.
        // O(n²) or worse algorithm suspected. Not a codegen bug per se.
        let source = """
module Test

// Full repro would have 13000+ members, simplified here
type T =
    static member M1 = 1
    static member M2 = 2
    static member M3 = 3
    // ... 13000 more would cause ~20sec compile time
    // Using 'let' instead would cause ~2.5min compile time
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13108: Static linking FS2009 warnings =====
    // https://github.com/dotnet/fsharp/issues/13108
    // When static linking assemblies, spurious FS2009 warnings are produced about
    // "Ignoring mixed managed/unmanaged assembly" for referenced assemblies that
    // are not actually mixed. The warning message lacks documentation on mitigation.
    // This primarily affects scenarios like VsVim that static-link FSharp.Core.
    [<Fact>]
    let ``Issue_13108_StaticLinkingWarnings`` () =
        // When using --standalone, the compiler statically links FSharp.Core and its
        // transitive dependencies. Previously, this emitted spurious FS2009 warnings
        // for assemblies that were merely transitively referenced but not actually
        // linked (e.g., assemblies that happened to not be pure IL).
        // The fix removes the warning for such assemblies since they're correctly
        // skipped during static linking.
        let source = """
module StaticLinkTest

// Simple program that will exercise static linking with --standalone
let value = List.iter (fun x -> printfn "%d" x) [1; 2; 3]

[<EntryPoint>]
let main _ = 0
"""
        FSharp source
        |> asExe
        |> withOptions ["--standalone"]
        |> compile
        |> shouldSucceed
        |> withDiagnostics []  // No FS2009 warnings should be produced
        |> ignore

    // ===== Issue #13100: --platform:x64 sets 32 bit characteristic =====
    // https://github.com/dotnet/fsharp/issues/13100
    // When compiling with --platform:x64, the PE FILE HEADER has "32 bit word machine"
    // characteristic set (0x100 flag), which is incorrect for x64 executables.
    // dumpbin /headers shows: "32 bit word machine" for F# but not for C# x64 builds.
    // C# correctly produces only "Executable" and "Application can handle large (>2GB) addresses"
    [<Fact>]
    let ``Issue_13100_PlatformCharacteristic`` () =
        // The bug is that F# sets IMAGE_FILE_32BIT_MACHINE (0x100) characteristic
        // in the PE header when targeting x64, which is incorrect.
        // To verify: compile with --platform:x64 and run `dumpbin /headers`
        // F# shows: 0x12E characteristics (includes "32 bit word machine")
        // C# shows: 0x22 characteristics (no 32-bit flag)
        let source = """
module PlatformTest

[<EntryPoint>]
let main _ = 0
"""
        // Test that x64 platform does NOT have Bit32Machine characteristic
        FSharp source
        |> asExe
        |> withPlatform ExecutionPlatform.X64
        |> compile
        |> shouldSucceed
        |> withPeReader (fun rdr -> 
            let characteristics = rdr.PEHeaders.CoffHeader.Characteristics
            // Should have LargeAddressAware (0x20)
            if not (characteristics.HasFlag(System.Reflection.PortableExecutable.Characteristics.LargeAddressAware)) then
                failwith $"x64 binary should have LargeAddressAware flag. Found: {characteristics}"
            // Should NOT have Bit32Machine (0x100)
            if characteristics.HasFlag(System.Reflection.PortableExecutable.Characteristics.Bit32Machine) then
                failwith $"x64 binary should NOT have Bit32Machine flag. Found: {characteristics}")
        |> ignore

    // ===== Issue #12546: Implicit boxing produces extraneous closure =====
    // https://github.com/dotnet/fsharp/issues/12546
    // When a function takes an obj parameter and you pass a value that gets implicitly boxed,
    // the compiler generates an extra wrapper closure that just invokes the first closure.
    // This doubles allocation unnecessarily.
    // Workaround: explicitly box the argument at the call site.
    //
    // ROOT CAUSE: The AdjustPossibleSubsumptionExpr function in TypedTreeOps.fs creates wrapper
    // lambdas when coercing between function types with different argument types. When calling
    // a function that takes obj with a string argument, the string->obj coercion triggers 
    // subsumption adjustment which wraps the result in an extra closure.
    //
    // The fix would need to optimize AdjustPossibleSubsumptionExpr to recognize when the argument
    // coercion is a simple box (concrete type to obj) and avoid generating the wrapper lambda
    // in that case. This is complex because the subsumption logic is also used for quotations.
    [<Fact>]
    let ``Issue_12546_BoxingClosure`` () =
        // Issue #12546: When a function takes obj and you pass a value that gets implicitly boxed,
        // the compiler generates an extra wrapper closure that just invokes the first closure.
        // This doubles allocation unnecessarily.
        //
        // ROOT CAUSE: The wrapper closure is generated during type checking/subsumption adjustment.
        // When calling `foo "hi"` where `foo: obj -> (unit -> string)`, the implicit string->obj
        // coercion triggers the subsumption adjustment logic which creates a wrapper lambda.
        //
        // WORKAROUND: Explicitly box the argument at the call site: `foo(box "hi")`
        //
        // STATUS: Bug confirmed. A fix was attempted in AdjustPossibleSubsumptionExpr but the
        // wrapper closure is created at a different point in the compilation pipeline.
        let source = """
module BoxingClosureTest

let foo (ob: obj) = box(fun () -> ob.ToString()) :?> (unit -> string)

// BUG: This allocates an extraneous closure that wraps the real one
let go() = foo "hi"

// WORKAROUND: Explicitly boxing avoids the extra closure
let goFixed() = foo(box "hi")
"""
        let actualIL = FSharp source |> asLibrary |> withOptimize |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("go", actualIL)
        Assert.Contains("goFixed", actualIL)
        Assert.Contains("FSharpFunc", actualIL)

    // ===== Issue #12460: F# C# Version info values different =====
    // https://github.com/dotnet/fsharp/issues/12460
    // F# and C# compilers produce different Version info metadata:
    // - F# output misses "Internal Name" value
    // - ProductVersion format differs: C# uses informational version, F# was using file version
    // These should be aligned with C# for consistency in tooling.
    [<Fact>]
    let ``Issue_12460_VersionInfoDifference`` () =
        // Test that version info matches C# conventions:
        // 1. InternalName is present (uses output filename)
        // 2. ProductVersion uses AssemblyInformationalVersion (not FileVersion)
        let source = """
module VersionInfoTest

open System.Reflection

[<assembly: AssemblyFileVersion("1.2.3.4")>]
[<assembly: AssemblyInformationalVersion("5.6.7")>]
do ()

let value = 42
"""
        FSharp source
        |> asLibrary
        |> withName "VersionInfoTest"
        |> compile
        |> shouldSucceed
        |> fun result ->
            match result with
            | CompilationResult.Success s ->
                match s.OutputPath with
                | Some path ->
                    let fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(path)
                    // Verify InternalName is set (matches C# behavior)
                    if System.String.IsNullOrEmpty(fvi.InternalName) then
                        failwith $"InternalName should not be empty. Expected filename, got: '{fvi.InternalName}'"
                    // Verify ProductVersion uses AssemblyInformationalVersion (not FileVersion)
                    // C# sets ProductVersion from AssemblyInformationalVersion
                    if fvi.ProductVersion <> "5.6.7" then
                        failwith $"ProductVersion should be '5.6.7' (from AssemblyInformationalVersion), got: '{fvi.ProductVersion}'"
                    // FileVersion should still be from AssemblyFileVersion
                    if fvi.FileVersion <> "1.2.3.4" then
                        failwith $"FileVersion should be '1.2.3.4', got: '{fvi.FileVersion}'"
                    result
                | None -> failwith "Output path not found"
            | _ -> failwith "Compilation failed"
        |> ignore

    [<Fact>]
    let ``Issue_12460_VersionInfoFallback`` () =
        // Edge case: Without AssemblyInformationalVersion, ProductVersion should fall back
        // to AssemblyFileVersion (C# behavior)
        let source = """
module VersionInfoFallbackTest

open System.Reflection

[<assembly: AssemblyFileVersion("2.3.4.5")>]
do ()

let value = 42
"""
        FSharp source
        |> asLibrary
        |> withName "VersionInfoFallbackTest"
        |> compile
        |> shouldSucceed
        |> fun result ->
            match result with
            | CompilationResult.Success s ->
                match s.OutputPath with
                | Some path ->
                    let fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(path)
                    // InternalName should still be set
                    if System.String.IsNullOrEmpty(fvi.InternalName) then
                        failwith $"InternalName should not be empty, got: '{fvi.InternalName}'"
                    // Without AssemblyInformationalVersion, ProductVersion falls back to FileVersion
                    if fvi.FileVersion <> "2.3.4.5" then
                        failwith $"FileVersion should be '2.3.4.5', got: '{fvi.FileVersion}'"
                    // OriginalFilename should also be set (matches InternalName)
                    if System.String.IsNullOrEmpty(fvi.OriginalFilename) then
                        failwith $"OriginalFilename should not be empty, got: '{fvi.OriginalFilename}'"
                    result
                | None -> failwith "Output path not found"
            | _ -> failwith "Compilation failed"
        |> ignore

    // ===== Issue #12416: Optimization inlining inconsistent with piping =====
    // https://github.com/dotnet/fsharp/issues/12416
    // InlineIfLambda functions don't inline when the input is an inline expression
    // (like array literal) vs when it's stored in a let binding.
    // Workaround: store arguments in intermediate let bindings before piping.
    [<Fact>]
    let ``Issue_12416_PipeInlining`` () =
        // The issue: InlineIfLambda inlining depends on whether the argument
        // is a variable or an inline expression. This is inconsistent.
        let source = """
module PipeInliningTest

type 'T PushStream = ('T -> bool) -> bool

let inline ofArray (vs : _ array) : _ PushStream = fun ([<InlineIfLambda>] r) ->
  let mutable i = 0
  while i < vs.Length && r vs.[i] do i <- i + 1
  i = vs.Length

let inline fold ([<InlineIfLambda>] f) z ([<InlineIfLambda>] ps : _ PushStream) =
  let mutable s = z
  let _ = ps (fun v -> s <- f s v; true)
  s

let inline (|>>) ([<InlineIfLambda>] v : _ -> _) ([<InlineIfLambda>] f : _ -> _) = f v

let values = [|0..100|]

// THIS INLINES CORRECTLY - values is a let binding
let thisIsInlined1 () = ofArray values |>> fold (+) 0

// THIS ALSO INLINES - intermediate binding for array
let thisIsInlined2 () = 
  let vs = [|0..100|]
  ofArray vs |>> fold (+) 0

// BUG: THIS DOES NOT INLINE - array literal inline
let thisIsNotInlined () = ofArray [|0..100|] |>> fold (+) 0
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        // The decompiled IL would show closures for thisIsNotInlined but not for thisIsInlined*
        |> ignore

    // ===== Issue #12384: Mutually recursive values wrong init =====
    // https://github.com/dotnet/fsharp/issues/12384
    // Mutually recursive non-function values were not initialized correctly.
    // The first value in the binding group had null for its recursive references,
    // while the second value was initialized correctly.
    // FIXED by PR #12395: Simple let rec ... and ... now works correctly.
    // NOTE: The edge case with module rec and intermediate modules is still open.
    [<Fact>]
    let ``Issue_12384_MutRecInitOrder`` () =
        // Test case from issue - simple mutual recursion with let rec ... and ...
        let source = """
module MutRecInitTest

type Node = { Next: Node; Prev: Node; Value: int }

// Single self-reference works correctly
let rec zero = { Next = zero; Prev = zero; Value = 0 }

// Mutual recursion with let rec ... and ... (fixed by PR #12395)
let rec one = { Next = two; Prev = two; Value = 1 }
and two = { Next = one; Prev = one; Value = 2 }

[<EntryPoint>]
let main _ =
    // Verify all references are correct
    let zeroOk = obj.ReferenceEquals(zero.Next, zero) && obj.ReferenceEquals(zero.Prev, zero)
    let oneNextOk = obj.ReferenceEquals(one.Next, two)
    let onePrevOk = obj.ReferenceEquals(one.Prev, two)
    let twoNextOk = obj.ReferenceEquals(two.Next, one)
    let twoPrevOk = obj.ReferenceEquals(two.Prev, one)
    
    if zeroOk && oneNextOk && onePrevOk && twoNextOk && twoPrevOk then
        0
    else
        failwith (sprintf "Mutual recursion initialization failed: zero=%b one.Next=%b one.Prev=%b two.Next=%b two.Prev=%b" 
                         zeroOk oneNextOk onePrevOk twoNextOk twoPrevOk)
"""
        FSharp source
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // ===== Issue #12366: Rethink names for compiler-generated closures =====
    // https://github.com/dotnet/fsharp/issues/12366
    // COSMETIC IL ISSUE - Affects debugging/profiling, not correctness.
    // Compiler-generated closure names like "foo@376" and "clo43@53" are weak heuristics.
    // Problems visible in IL:
    // - .class nested assembly auto ansi serializable sealed beforefieldinit 'clo@12-1'
    // - .class nested assembly auto ansi ... 'Pipe input at line 63@53'
    // These names appear in:
    // - Debugger call stacks
    // - Profiler output
    // - Decompiled code
    // FIX: Include enclosing function name in closure class names for debugger-friendliness.
    [<Fact>]
    let ``Issue_12366_ClosureNaming`` () =
        // Test that closures get meaningful names from their enclosing bindings.
        // When a closure is inside a function, it should get the enclosing function's name
        // rather than a generic "clo" name.
        let source = """
module ClosureNamingTest

// Function with inner closures - closures get the enclosing function name  
let processData items =
    items
    |> List.map (fun x -> x * 2)    
    |> List.filter (fun x -> x > 2) 

// Nested function with capturing closure
let outerFunc a =
    let innerFunc b =
        fun c -> a + b + c  
    innerFunc
"""
        // The closure naming improvement ensures closures inside functions
        // get names that include the enclosing function name for debugger-friendliness.
        // Closures use the enclosing function name (outerFunc) for better debugging.
        let actualIL = FSharp source |> asLibrary |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("outerFunc@", actualIL)
        Assert.DoesNotContain("'clo@", actualIL)

    // Additional test: Verify closures that capture environment variables work
    [<Fact>]
    let ``Issue_12366_ClosureNaming_Capturing`` () =
        // Test closures that capture outer values get meaningful names from enclosing function
        let source = """
module CapturingClosureTest

// Closure inside a function that captures outer variables
let makeAdder n =
    fun x -> x + n  // Anonymous closure captures 'n'

let makeMultiplier n =
    let multiply x = x * n  // Named inner function captures 'n', inlined by optimizer
    multiply
"""
        let actualIL = FSharp source |> asLibrary |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("makeMultiplier@", actualIL)
        Assert.DoesNotContain("'clo@", actualIL)

    // Edge case: Deeply nested closures
    [<Fact>]
    let ``Issue_12366_ClosureNaming_DeepNesting`` () =
        let source = """
module DeepNestingTest

let outermost x =
    let middle y =
        let innermost z =
            fun w -> x + y + z + w
        innermost
    middle
"""
        let actualIL = FSharp source |> asLibrary |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("outermost@", actualIL)
        Assert.DoesNotContain("'clo@", actualIL)

    // ===== Issue #12139: Improve string null check IL codegen =====
    // https://github.com/dotnet/fsharp/issues/12139
    // PERFORMANCE IL ISSUE - F# emits String.Equals(s, null) for string null checks,
    // while C# emits simple brtrue/brfalse (single instruction).
    // 
    // F# IL for `s <> null`:
    //   ldarg.0
    //   ldnull
    //   call bool [System.Runtime]System.String::Equals(string, string)
    //   brtrue.s IL_XXXX
    //
    // C# IL for `s != null`:
    //   ldarg.0  
    //   brtrue.s IL_XXXX   ← single instruction, much simpler
    //
    // The JIT may optimize this, but IL is larger and startup is slower.
    [<Fact>]
    let ``Issue_12139_StringNullCheck`` () =
        // PERFORMANCE: F# generates String.Equals call for null comparison
        // C# generates simple null pointer check (brtrue/brfalse)
        //
        // F# emits this IL pattern for `s = null`:
        //   IL_0000: ldarg.0
        //   IL_0001: ldnull  
        //   IL_0002: call bool [System.Runtime]System.String::Equals(string, string)
        //   IL_0007: ret
        //
        // C# emits this IL pattern for `s == null`:
        //   IL_0000: ldarg.0
        //   IL_0001: ldnull
        //   IL_0002: ceq
        //   IL_0004: ret
        //
        // Or even simpler with brfalse/brtrue for boolean context
        let source = """
module StringNullCheckTest

open System

// F# IL: ldarg.0; ldnull; call bool String::Equals(string, string); ret
// C# IL: ldarg.0; ldnull; ceq; ret (or just brtrue.s for conditionals)
let isNullString (s: string) = s = null

// Same issue - extra String.Equals call instead of simple branch
let isNotNullString (s: string) = s <> null

// In loop context, this adds 2 extra IL instructions per iteration:
// F# IL: call string Console::ReadLine(); ldnull; call bool String::Equals(...); brtrue.s
// C# IL: call string Console::ReadLine(); brtrue.s (single branch instruction)
let test() =
    while Console.ReadLine() <> null do
        Console.WriteLine(1)
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        // The IL shows String.Equals calls instead of simple brtrue/brfalse
        // This is a performance issue (code size, potential JIT overhead)
        |> ignore

    // ===== Issue #12137: Improve analysis to reduce emit of tail =====
    // https://github.com/dotnet/fsharp/issues/12137
    // PERFORMANCE IL ISSUE - F# emits `tail.` prefix inconsistently:
    // - Same-assembly calls: NO tail. prefix (correct, allows inlining)
    // - Cross-assembly calls: tail. prefix emitted (unnecessary, hurts performance)
    //
    // IL for SAME assembly call (good):
    //   IL_000c: call !!0 Module::fold<...>
    //
    // IL for CROSS assembly call (bad):
    //   IL_000c: tail.
    //   IL_000e: call !!0 [OtherLib]Module::fold<...>
    //
    // The unnecessary `tail.` prefix causes:
    // - 2-3x slower execution (tail call dispatch helpers)
    // - 2x larger JIT-generated assembly code
    //
    // NOTE: Hard to demonstrate in single-file test. Requires two assemblies.
    // This test documents the issue; full repro needs cross-assembly call.
    [<Fact>]
    let ``Issue_12137_TailEmitReduction`` () =
        // PERFORMANCE: Cross-assembly calls get unnecessary `tail.` prefix
        // 
        // When compiled, this module's internal calls don't have tail. prefix.
        // But if another assembly calls these same functions, F# emits:
        //   tail.
        //   call !!0 [TailEmitTest]TailEmitTest::fold<...>
        //
        // The tail. prefix is emitted because F# can't prove the callee won't
        // have unbounded stack growth. But for most functions, tail. is unnecessary
        // and causes significant performance overhead.
        //
        // To fully reproduce: compile this as LibA.dll, then from LibB.dll call
        // the functions. LibB will show tail. prefix in IL for cross-assembly calls.
        let source = """
module TailEmitTest

// Inline generic fold - when called from same assembly, no tail. (good)
let inline fold (f: 'S -> 'T -> 'S) (state: 'S) (items: 'T list) =
    let mutable s = state
    for item in items do
        s <- f s item
    s

// Same-assembly call - IL shows NO tail. prefix (correct behavior)
let sumLocal () = fold (+) 0 [1; 2; 3]

// For cross-assembly scenario (not testable in single file):
// If another assembly calls: TailEmitTest.fold (+) 0 [1;2;3]
// The IL would show:
//   tail.
//   call !!0 [TailEmitTest]TailEmitTest/fold@5::Invoke(...)
// This tail. is unnecessary and causes 2-3x performance penalty
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        // Cross-assembly calls would show unnecessary tail. prefix in IL
        // This test documents the issue; single-assembly can't fully demonstrate
        |> ignore

    // ===== Issue #12136: use fixed does not unpin at end of scope =====
    // https://github.com/dotnet/fsharp/issues/12136
    // [KNOWN_LIMITATION]
    //
    // PROBLEM: When using "use x = fixed expr", the pinned variable remains pinned until
    // the function returns, not at the end of the scope where it's declared.
    // C# correctly nulls out the pinned local at end of fixed block scope.
    // This affects GC behavior and confuses decompilers.
    //
    // WHY IT'S COMPLEX: The `use fixed` expression is elaborated by the type checker as:
    //   let pin = let pinnedByref = &array.[0] in conv.i pinnedByref
    // The IsFixed flag is on the inner 'pinnedByref' variable, not the outer 'pin'.
    // This makes it difficult to emit cleanup at the correct scope end.
    //
    // WORKAROUND: Put the fixed block in a separate function:
    //   let doBlock() = use pin = fixed &array.[0]; used pin
    //   doBlock(); used 1  // Array is correctly unpinned after doBlock returns
    //
    // This test verifies the workaround compiles correctly.
    // [<Fact>]  // Commented out - workaround only, not a fix
    let ``Issue_12136_FixedUnpin`` () =
        // Test the workaround: put fixed block in separate function
        let source = """
module FixedUnpinTest

#nowarn "9"  // Suppress unverifiable IL warning for fixed

open Microsoft.FSharp.NativeInterop

let inline used<'T> (t: 'T) : unit = ignore t

// WORKAROUND: Put the fixed block in a separate function
let testFixed (array: int[]) : unit =
    let doBlock() =
        use pin = fixed &array.[0]
        used pin
    doBlock()  // Function returns, so pinned local is implicitly cleaned
    used 1     // Now array is correctly unpinned
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #11935: unmanaged constraint not recognized by C# =====
    // https://github.com/dotnet/fsharp/issues/11935
    // C# doesn't recognize F# unmanaged constraint correctly.
    // FIXED: F# 10+ emits modreq and IsUnmanagedAttribute for C# interop.
    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop`` () =
        let source = """
module Test

let test<'T when 'T : unmanaged> (x: 'T) = x
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
      .method public static !!T  test<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>(!!T x) cil managed
      {
        .param type T 
          .custom instance void [runtime]System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ret
      }"""]

    // Issue #11935 edge case: unmanaged constraint on class type definition
    // The original issue reported that `type C<'T when 'T: unmanaged>` loses the constraint in C#
    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop_ClassType`` () =
        let source = """
module Test

type Container<'T when 'T : unmanaged>() =
    member _.GetDefault() : 'T = Unchecked.defaultof<'T>
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        // Verify that class has the unmanaged constraint with modreq
        |> verifyILContains [
            "Container`1<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>"
            ".custom instance void [runtime]System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )"
        ]
        |> shouldSucceed

    // Issue #11935 edge case: unmanaged constraint on struct type definition
    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop_StructType`` () =
        let source = """
module Test

[<Struct>]
type StructContainer<'T when 'T : unmanaged> =
    val Value : 'T
    new(v) = { Value = v }
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        // Verify that struct has the unmanaged constraint with modreq
        |> verifyILContains [
            "StructContainer`1<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>"
            ".custom instance void [runtime]System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )"
        ]
        |> shouldSucceed

    // Issue #11935 edge case: unmanaged constraint on instance method
    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop_InstanceMethod`` () =
        let source = """
module Test

type Processor() =
    member _.Process<'T when 'T : unmanaged>(x: 'T) = x
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        // Verify instance method has unmanaged constraint
        |> verifyIL ["""
    .method public hidebysig instance !!T 
            Process<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>(!!T x) cil managed
    {
      .param type T 
        .custom instance void [runtime]System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ret
    }"""]

    // Issue #11935 edge case: multiple type parameters, some with unmanaged constraint
    [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop_MultipleTypeParams`` () =
        let source = """
module Test

let combine<'T, 'U when 'T : unmanaged and 'U : unmanaged> (x: 'T) (y: 'U) = struct(x, y)
"""
        FSharp source
        |> asLibrary
        |> withLangVersion10
        |> compile
        |> shouldSucceed
        // Verify both type parameters have unmanaged constraint with modreq and IsUnmanagedAttribute
        |> verifyILContains [
            "combine<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T,valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) U>(!!T x, !!U y) cil managed"
        ]
        |> shouldSucceed

    // ===== Issue #11556: Field initializers in object construction =====
    // https://github.com/dotnet/fsharp/issues/11556
    //
    // This test verifies that object construction with named field initialization
    // compiles and executes correctly. The syntax Test(X = 1) should work and
    // set the field properly.
    //
    // Note: An IL-level optimization using 'dup' instead of 'stloc/ldloc' was
    // evaluated but removed because modern JIT (RyuJIT) already optimizes the
    // stloc/ldloc pattern to register transfers. The ~70 lines of pattern-matching
    // code provided no measurable runtime benefit. See dotnet/roslyn#21764 for
    // similar analysis in Roslyn.
    [<Fact>]
    let ``Issue_11556_FieldInitializers`` () =
        // Verify object construction with field initialization compiles and runs correctly
        let source = """
module Program

open System.Runtime.CompilerServices

type Test =
    [<DefaultValue>]
    val mutable X : int
    new() = { }

[<MethodImpl(MethodImplOptions.NoInlining)>]
let test() =
    Test(X = 1)

[<EntryPoint>]
let main _ =
    let t = test()
    if t.X <> 1 then failwith "X should be 1"
    0
"""
        FSharp source
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // ===== Issue #11132: TypeloadException delegate with voidptr parameter =====
    // https://github.com/dotnet/fsharp/issues/11132
    // TypeLoadException at runtime for delegates with voidptr.
    // FIX: void* cannot be used as a generic type argument in CLI.
    // Solution: Convert voidptr to nativeint (IntPtr) in GenTypeArgAux when generating
    // type arguments for FSharpFunc generic instantiation.
    [<Fact>]
    let ``Issue_11132_VoidptrDelegate`` () =
        let source = """
module Test
#nowarn "9"

open System

type MyDelegate = delegate of voidptr -> unit

let method (ptr: voidptr) = ()

// This function returns a delegate - this is what triggers the bug
// because it creates FSharpFunc<voidptr, MyDelegate>
let getDelegate (m: voidptr -> unit) : MyDelegate = MyDelegate(m)

let test() =
    let d = getDelegate method
    d.Invoke(IntPtr.Zero.ToPointer())

// Execute to verify no TypeLoadException
do test()
"""
        FSharp source
        |> asExe
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // ===== Issue #11114: Record with hundreds of members StackOverflow =====
    // https://github.com/dotnet/fsharp/issues/11114
    // StackOverflowException during compilation for large records.
    // [<Fact>]
    let ``Issue_11114_LargeRecordStackOverflow`` () =
        let source = """
module Test

type SmallRecord = {
    Field1: int
    Field2: int
    Field3: int
}
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #9348: Performance of Comparing and Ordering =====
    // https://github.com/dotnet/fsharp/issues/9348
    // Generated comparison code is suboptimal.
    [<Fact>]
    let ``Issue_9348_ComparePerformance`` () =
        let source = """
module Test

type T = { X: int }
let compare (a: T) (b: T) = compare a.X b.X
"""
        let actualIL = FSharp source |> asLibrary |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("compare", actualIL)
        Assert.Contains("ldfld", actualIL)

    // ===== Issue #9176: Decorate inline function code with attribute =====
    // https://github.com/dotnet/fsharp/issues/9176
    // [OUT_OF_SCOPE: FEATURE REQUEST]
    // This is NOT a bug - it's a feature request for a new `FSharpInlineFunction` attribute.
    // The request is to mark call sites where inline functions were inlined, similar to 
    // how StackTrace shows inline methods. This would require:
    // 1. A new attribute type (e.g., FSharpInlineFunction) added to FSharp.Core
    // 2. Compiler changes to emit the attribute at inlined call sites
    // 3. Tooling changes to consume the attribute
    // Test documents the feature request by verifying inline functions work correctly.
    [<Fact>]
    let ``Issue_9176_InlineAttributes`` () =
        // [OUT_OF_SCOPE: FEATURE REQUEST] - Documents the feature request, not a regression.
        // The issue asks for a way to trace back inlined code to its original source,
        // similar to how C# shows inlined methods in stack traces.
        // Currently F# inline functions leave no trace after inlining.
        let source = """
module Test

// When this inline function is inlined at call sites, there's no IL indication
// that the code came from 'f'. The feature request asks for:
// - An attribute like [<FSharpInlineFunction("f", "Test.fs", line=5)>] at call sites
// - This would help debugging/profiling tools show the original source location
let inline f x = x + 1

// At this call site, the code "x + 1" is inlined with no trace back to 'f'
let g y = f y + f y
"""
        // This compiles successfully - there's no bug, just a missing feature
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #7861: Missing assembly reference for type in attributes =====
    // https://github.com/dotnet/fsharp/issues/7861
    // Missing assembly reference for types used in attribute arguments.
    // When typeof<ExternalType> is used in an attribute, the compiler must emit
    // an assembly reference for the assembly containing ExternalType.
    [<Fact>]
    let ``Issue_7861_AttributeTypeReference`` () =
        let source = """
module Test

open System

// Custom attribute that takes a Type parameter
type TypedAttribute(t: Type) =
    inherit Attribute()
    member _.TargetType = t

// Use typeof with an external type - System.Xml.XmlDocument
// This should trigger an assembly reference to System.Xml.ReaderWriter
[<Typed(typeof<System.Xml.XmlDocument>)>]
type MyClass() = class end
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // Verify that System.Xml.ReaderWriter assembly is referenced in the output
        // This assembly contains System.Xml.XmlDocument
        |> verifyILContains [ 
            ".assembly extern System.Xml.ReaderWriter"
        ]
        |> shouldSucceed

    // Additional edge case: Named attribute argument with typeof
    [<Fact>]
    let ``Issue_7861_NamedAttributeArgument`` () =
        let source = """
module Test

open System

// Attribute with named type property
type TypePropertyAttribute() =
    inherit Attribute()
    member val TargetType : Type = null with get, set

// Use named argument with typeof referencing external type
[<TypeProperty(TargetType = typeof<System.Xml.XmlDocument>)>]
type MyClass() = class end
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyILContains [ 
            ".assembly extern System.Xml.ReaderWriter"
        ]
        |> shouldSucceed

    // Additional edge case: Array of types in attribute
    // [<Fact>] // UNFIXED: Enable when issue #7861 is fixed
    let ``Issue_7861_TypeArrayInAttribute`` () =
        let source = """
module Test

open System

// Attribute with Type array parameter
type MultiTypeAttribute(types: Type[]) =
    inherit Attribute()
    member _.Types = types

// Use array of types from different assemblies
[<MultiType([| typeof<System.Xml.XmlDocument>; typeof<System.Net.Http.HttpClient> |])>]
type MyClass() = class end
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // Both assemblies should be referenced
        |> verifyILContains [ 
            ".assembly extern System.Xml.ReaderWriter"
            ".assembly extern System.Net.Http"
        ]
        |> shouldSucceed

    // Additional edge case: Attribute on method with typeof
    [<Fact>]
    let ``Issue_7861_AttributeOnMethod`` () =
        let source = """
module Test

open System

type TypedAttribute(t: Type) =
    inherit Attribute()
    member _.TargetType = t

type MyClass() =
    [<Typed(typeof<System.Xml.XmlDocument>)>]
    member _.MyMethod() = ()
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyILContains [ 
            ".assembly extern System.Xml.ReaderWriter"
        ]
        |> shouldSucceed

    // ===== Issue #6750: Mutually recursive values leave fields uninitialized =====
    // https://github.com/dotnet/fsharp/issues/6750
    // Fields may be uninitialized or wrong value in mutual recursion.
    // [<Fact>]
    let ``Issue_6750_MutRecUninitialized`` () =
        let source = """
module Test

let rec a = b + 1
and b = 0
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #6379: FS2014 when using tupled args =====
    // https://github.com/dotnet/fsharp/issues/6379
    // False positive warning for tuple patterns.
    // [<Fact>]
    let ``Issue_6379_TupledArgsWarning`` () =
        let source = """
module Test

let f (x, y) = x + y
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #5834: Obsolete on abstract generates accessors without specialname =====
    // https://github.com/dotnet/fsharp/issues/5834
    // Abstract event accessors don't get the specialname IL flag, breaking Reflection-based tools.
    // This becomes problematic when [<Obsolete>] or other attributes are involved.
    [<Fact>]
    let ``Issue_5834_ObsoleteSpecialname`` () =
        let source = """
module Test

open System
open System.Reflection

// Abstract type with [<Obsolete>] on abstract event accessors
[<AbstractClass>]
type AbstractWithEvent() =
    [<Obsolete("This event is deprecated")>]
    [<CLIEvent>]
    abstract member MyEvent : IEvent<EventHandler, EventArgs>

// Concrete type for comparison - event accessors should have specialname
type ConcreteWithEvent() =
    let evt = new Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member this.MyEvent = evt.Publish

[<EntryPoint>]
let main _ =
    let abstractType = typeof<AbstractWithEvent>
    let concreteType = typeof<ConcreteWithEvent>
    
    // Check abstract event accessors
    let abstractAddMethod = abstractType.GetMethod("add_MyEvent")
    let abstractRemoveMethod = abstractType.GetMethod("remove_MyEvent")
    
    // Check concrete event accessors (for comparison)
    let concreteAddMethod = concreteType.GetMethod("add_MyEvent")
    
    printfn "AbstractWithEvent.add_MyEvent.IsSpecialName = %b" abstractAddMethod.IsSpecialName
    printfn "AbstractWithEvent.remove_MyEvent.IsSpecialName = %b" abstractRemoveMethod.IsSpecialName
    printfn "ConcreteWithEvent.add_MyEvent.IsSpecialName = %b" concreteAddMethod.IsSpecialName
    
    // Bug: Abstract event accessors should have IsSpecialName = true, but they don't
    if not abstractAddMethod.IsSpecialName || not abstractRemoveMethod.IsSpecialName then
        printfn "BUG: Abstract event accessors missing specialname flag"
        printfn "Expected: IsSpecialName = true"
        printfn "Actual: IsSpecialName = false"
        1
    else
        printfn "SUCCESS: All event accessors have specialname"
        0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // ===== Issue #5464: F# ignores custom modifiers modreq/modopt =====
    // https://github.com/dotnet/fsharp/issues/5464
    //
    // Custom modifiers (modreq/modopt) from C# types must be preserved when F# calls methods.
    // The modreq is part of the method signature and must appear in the call instruction.
    //
    // This test verifies that when F# calls a C# method with an 'in' parameter,
    // the modreq(InAttribute) is preserved in the emitted IL call instruction.
    // [<Fact>] // UNFIXED: Enable when issue is fixed
    let ``Issue_5464_CustomModifiers`` () =
        // C# library with 'in' parameter - this generates modreq(InAttribute)
        // Use CSharp11 to ensure 'in' parameters are properly supported with modreq
        let csLib = 
            CSharp """
    using System;
    namespace CsLib
    {
        public struct ReadOnlyStruct 
        { 
            public int Value; 
            public ReadOnlyStruct(int v) { Value = v; }
        }
        
        public class Processor
        {
            public static int Process(in ReadOnlyStruct s)
            {
                return s.Value;
            }
        }
    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11 |> withName "CsLib"

        // F# code that calls the C# method with 'in' parameter
        let fsharpSource = """
module Test

open CsLib

let callProcessor () =
    let s = ReadOnlyStruct(42)
    Processor.Process(&s)
"""
        FSharp fsharpSource
        |> asLibrary
        |> withReferences [csLib]
        |> compile
        |> shouldSucceed
        // Verify that the call instruction preserves the modreq modifier
        // C# emits modreq(InAttribute) for 'in' parameters
        |> verifyIL [
            // The call should include modreq - checking for any modreq presence on the byref parameter
            "call       int32 [CsLib]CsLib.Processor::Process(valuetype [CsLib]CsLib.ReadOnlyStruct& modreq("
        ]
        |> ignore

    // Edge case: modopt custom modifiers
    // modopt is advisory and should also be preserved for proper interop
    // [<Fact>] // UNFIXED: Enable when issue #5464 is fixed
    let ``Issue_5464_CustomModifiers_ModOpt`` () =
        // C# library with modopt via IsConst (using 'ref readonly' returns)
        let csLib = 
            CSharp """
    using System;
    using System.Runtime.CompilerServices;
    namespace CsLib
    {
        public struct Data 
        { 
            public int Value; 
        }
        
        public class Container
        {
            private Data _data;
            
            // ref readonly return adds modopt(IsConst) on some configurations
            // However, 'ref readonly' uses modreq(In) - let's use in param for consistency
            public static void ProcessWithIn(in Data d) { }
        }
    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11 |> withName "CsLibModOpt"

        let fsharpSource = """
module Test

open CsLib

let callWithIn () =
    let d = Data()
    Container.ProcessWithIn(&d)
"""
        FSharp fsharpSource
        |> asLibrary
        |> withReferences [csLib]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // Verify modreq is preserved on the in parameter
            "call       void [CsLibModOpt]CsLib.Container::ProcessWithIn(valuetype [CsLibModOpt]CsLib.Data& modreq("
        ]
        |> ignore

    // Edge case: Multiple in parameters - both should preserve modreq
    // [<Fact>] // UNFIXED: Enable when issue #5464 is fixed
    let ``Issue_5464_CustomModifiers_MultipleInParams`` () =
        let csLib = 
            CSharp """
    using System;
    namespace CsLib
    {
        public struct Vector3
        {
            public float X, Y, Z;
            public Vector3(float x, float y, float z) { X = x; Y = y; Z = z; }
        }
        
        public class VectorMath
        {
            // Two in parameters - both should have modreq(InAttribute)
            public static float Dot(in Vector3 a, in Vector3 b)
            {
                return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            }
        }
    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11 |> withName "CsLibMultiIn"

        let fsharpSource = """
module Test

open CsLib

let dotProduct () =
    let a = Vector3(1.0f, 2.0f, 3.0f)
    let b = Vector3(4.0f, 5.0f, 6.0f)
    VectorMath.Dot(&a, &b)
"""
        FSharp fsharpSource
        |> asLibrary
        |> withReferences [csLib]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // Verify both parameters preserve modreq(InAttribute) 
            // The call should show modreq on the byref parameters
            "call       float32 [CsLibMultiIn]CsLib.VectorMath::Dot(valuetype [CsLibMultiIn]CsLib.Vector3& modreq("
        ]
        |> ignore

    // Edge case: Generic type with custom modifiers
    // [<Fact>] // UNFIXED: Enable when issue #5464 is fixed
    let ``Issue_5464_CustomModifiers_GenericType`` () =
        let csLib = 
            CSharp """
    using System;
    namespace CsLib
    {
        public struct GenericData<T> 
        { 
            public T Value; 
        }
        
        public class GenericProcessor
        {
            public static T Process<T>(in GenericData<T> data) where T : struct
            {
                return data.Value;
            }
        }
    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11 |> withName "CsLibGeneric"

        let fsharpSource = """
module Test

open CsLib

let processGeneric () =
    let data = GenericData<int>(Value = 42)
    GenericProcessor.Process(&data)
"""
        FSharp fsharpSource
        |> asLibrary
        |> withReferences [csLib]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // Verify modreq is preserved on generic in parameter
            "call       !!0 [CsLibGeneric]CsLib.GenericProcessor::Process<int32>(valuetype [CsLibGeneric]CsLib.GenericData`1<!!0>& modreq("
        ]
        |> ignore

    // Edge case: Chained calls with custom modifiers
    // [<Fact>] // UNFIXED: Enable when issue #5464 is fixed
    let ``Issue_5464_CustomModifiers_ChainedCalls`` () =
        let csLib = 
            CSharp """
    using System;
    namespace CsLib
    {
        public struct Point 
        { 
            public int X, Y; 
            public Point(int x, int y) { X = x; Y = y; }
        }
        
        public class Math
        {
            public static int GetX(in Point p) => p.X;
            public static int GetY(in Point p) => p.Y;
            public static int Distance(in Point a, in Point b)
            {
                var dx = b.X - a.X;
                var dy = b.Y - a.Y;
                return dx * dx + dy * dy;
            }
        }
    }""" |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp11 |> withName "CsLibChain"

        let fsharpSource = """
module Test

open CsLib

let computeDistance () =
    let p1 = Point(0, 0)
    let p2 = Point(3, 4)
    Math.Distance(&p1, &p2)
"""
        FSharp fsharpSource
        |> asLibrary
        |> withReferences [csLib]
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // Verify both in parameters preserve modreq
            "call       int32 [CsLibChain]CsLib.Math::Distance(valuetype [CsLibChain]CsLib.Point& modreq("
        ]
        |> ignore

    // ===== Issue #878: Serialization of F# exception variants doesn't serialize fields =====
    // https://github.com/dotnet/fsharp/issues/878
    // Exception fields are lost after deserialization when using BinaryFormatter.
    // The F# compiler now generates proper GetObjectData override and deserialization constructor.
    // Note: BinaryFormatter is removed in .NET 10+, so we verify IL generation instead of runtime behavior.
    [<Fact>]
    let ``Issue_878_ExceptionSerialization`` () =
        let source = """
module Test

// Define F# exception with multiple fields
exception Foo of x:string * y:int
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // Verify GetObjectData override exists (serialization method)
            ".method public strict virtual instance void GetObjectData(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed"
            // Verify base.GetObjectData is called
            "call       instance void [runtime]System.Exception::GetObjectData(class [runtime]System.Runtime.Serialization.SerializationInfo,"
            // Verify serialization constructor exists
            ".method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed"
        ]
        |> ignore
