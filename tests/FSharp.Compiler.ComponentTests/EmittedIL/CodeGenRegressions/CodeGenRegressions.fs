// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Tests for known CodeGen regressions/bugs that are documented but not yet fixed.
/// Each test is commented out with // [<Fact>] to prevent CI failures while keeping them buildable.
/// See CODEGEN_REGRESSIONS.md in the repository root for detailed analysis of each issue.
namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CodeGenRegressions =

    // ===== Issue #19075: CLR Crashes when running program using constrained calls =====
    // https://github.com/dotnet/fsharp/issues/19075
    // The combination of SRTP with IDisposable constraint and constrained call generates
    // invalid IL that causes a CLR crash (segfault) at runtime.
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
        |> shouldSucceed // This will fail with CLR crash - bug exists
        |> ignore

    // ===== Issue #19068: Object expression in struct generates byref field in a class =====
    // https://github.com/dotnet/fsharp/issues/19068
    // Using an object expression in a struct that depends on primary constructor parameters
    // results in a class being emitted with a byref field, causing TypeLoadException at runtime.
    // [<Fact>]
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
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed // This will fail with TypeLoadException - bug exists
        |> ignore

    // ===== Issue #19020: [<return: ...>] not respected on class members =====
    // https://github.com/dotnet/fsharp/issues/19020
    // The [<return: SomeAttribute>] syntax to attach an attribute to the return type of a
    // method does not work on class static/instance members (works on module functions).
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
        // Currently it's dropped for class members but works for module functions
        |> verifyIL [
            // This IL check would need to verify SomeAttribute on return for class members
            ".param [0]"
            ".custom instance void Test/SomeAttribute::.ctor()"
        ]
        |> ignore

    // ===== Issue #18956: Decimal constant causes InvalidProgramException for debug builds =====
    // https://github.com/dotnet/fsharp/issues/18956
    // A [<Literal>] decimal constant causes System.InvalidProgramException in debug builds
    // due to incorrect locals initialization in the generated IL.
    // [<Fact>]
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
        |> shouldSucceed // This will fail with InvalidProgramException in debug - bug exists
        |> ignore

    // ===== Issue #18953: Implicit Action/Func conversion captures extra expressions =====
    // https://github.com/dotnet/fsharp/issues/18953
    // When implicitly converting an F# function to Action/Func, the conversion incorrectly
    // re-evaluates expressions that should only be evaluated once.
    // [<Fact>]
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
// Actual: callCount = 2 (y() called twice due to incorrect conversion)
if callCount <> 1 then
    failwithf "Expected 1 call, got %d" callCount
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed // This will fail - y() is called twice - bug exists
        |> ignore

    // ===== Issue #18868: Error using [<CallerFilePath>] with caller info in delegates =====
    // https://github.com/dotnet/fsharp/issues/18868
    // Using [<CallerFilePath>] attribute in delegate definitions produces strange error
    // message and/or runtime failures.
    // [<Fact>]
    let ``Issue_18868_CallerInfoInDelegates`` () =
        let source = """
module Test

type A = delegate of [<System.Runtime.CompilerServices.CallerFilePath>] a: string -> unit
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed // This will fail with FS1246 - bug exists
        |> ignore

    // ===== Issue #18815: Can't define extensions for two same named types =====
    // https://github.com/dotnet/fsharp/issues/18815
    // Defining extensions for two types with the same simple name in a single module
    // causes a compilation error about duplicate entry in method table.
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
        |> shouldSucceed // This will fail with FS2014 duplicate entry - bug exists
        |> ignore

    // ===== Issue #18753: Inlining in CEs prevented by DU constructor in CE block =====
    // https://github.com/dotnet/fsharp/issues/18753
    // When using a CE, if a yielded item is constructed as a DU case in place,
    // it prevents inlining of subsequent yields in that CE, leading to suboptimal codegen.
    // [<Fact>]
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

// Both should produce equivalent, fully inlined code
// But test2 generates lambdas - bug exists
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        // The IL for test2 should be as clean as test1, but it's not
        |> ignore

    // ===== Issue #18672: Resumable code CE top level value doesn't work =====
    // https://github.com/dotnet/fsharp/issues/18672
    // When a CE using resumable code is created as a top-level value, it works in Debug
    // but returns null in Release mode.
    // [<Fact>]
    let ``Issue_18672_ResumableCodeTopLevelValue`` () =
        // This test requires the full resumable code infrastructure which is complex
        // For now we document that the bug exists - see issue for full repro
        let source = """
module Test

// Simplified test case - the actual bug requires resumable code infrastructure
// See https://github.com/dotnet/fsharp/issues/18672 for full repro

// The issue is that top-level CE values using resumable code return null in Release mode
// but work correctly in Debug mode

printfn "Test placeholder for Issue 18672"
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #18374: RuntimeWrappedException cannot be caught =====
    // https://github.com/dotnet/fsharp/issues/18374
    // When a non-Exception object is thrown (from CIL or other languages),
    // F# generates a catch handler that casts to Exception, which throws InvalidCastException.
    // [<Fact>]
    let ``Issue_18374_RuntimeWrappedExceptionCannotBeCaught`` () =
        // This test requires inline IL to throw a non-Exception object
        // The workaround is to use [<assembly: RuntimeCompatibility(WrapNonExceptionThrows=true)>]
        let source = """
module Test

// To properly test this, we need inline IL: let throwobj (x:obj) = (# "throw" x #)
// The bug is that the generated catch handler does:
//   catch [netstandard]System.Object { castclass [System.Runtime]System.Exception ... }
// This castclass throws InvalidCastException when a non-Exception is thrown

// Workaround exists: [<assembly: System.Runtime.CompilerServices.RuntimeCompatibility(WrapNonExceptionThrows=true)>]

printfn "Test placeholder for Issue 18374 - requires inline IL for full repro"
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore
