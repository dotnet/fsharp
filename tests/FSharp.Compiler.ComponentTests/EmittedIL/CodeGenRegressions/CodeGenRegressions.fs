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

    // ====================================================================================
    // SPRINT 2: Issues #18319, #18263, #18140, #18135, #18125, #17692, #17641, #16565, #16546, #16378
    // ====================================================================================

    // ===== Issue #18319: Non-null constant literal of less-specific type generates invalid IL =====
    // https://github.com/dotnet/fsharp/issues/18319
    // Using a constant expression upcasted to a less-specific type (e.g., ValueType) generates
    // IL that's missing the box instruction, causing InvalidProgramException at runtime.
    // [<Fact>]
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
        |> shouldSucceed // This will fail with InvalidProgramException - missing box instruction
        |> ignore

    // ===== Issue #18263: DU .Is* properties causing compile time error =====
    // https://github.com/dotnet/fsharp/issues/18263
    // When DU case names share prefixes that produce identical .Is* property names after
    // normalization, compilation fails with "duplicate entry in method table".
    // [<Fact>]
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
        |> shouldSucceed // This will fail with "duplicate entry 'get_IsSZ' in method table" - bug exists
        |> ignore

    // ===== Issue #18140: Codegen causes ilverify errors - Callvirt on value type =====
    // https://github.com/dotnet/fsharp/issues/18140
    // The compiler generates callvirt on value type methods, which is incorrect IL
    // (should use constrained prefix or call instead). ILVerify reports this as an error.
    // [<Fact>]
    let ``Issue_18140_CallvirtOnValueType`` () =
        let source = """
module Test

// This demonstrates the pattern that causes callvirt on value type
// The issue manifests in the compiler's own code (Range, etc.) but can
// also occur in user code with custom IEqualityComparer on structs

[<Struct>]
type MyStruct =
    { Value: int }
    override this.GetHashCode() = this.Value

// Using struct in IEqualityComparer implementation pattern
type MyComparer() =
    interface System.Collections.Generic.IEqualityComparer<MyStruct> with
        member _.Equals(x, y) = x.Value = y.Value
        member _.GetHashCode(obj) = obj.GetHashCode()

let test() =
    let comparer = MyComparer() :> System.Collections.Generic.IEqualityComparer<MyStruct>
    let s = { Value = 42 }
    comparer.GetHashCode(s)
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // ILVerify would report: [CallVirtOnValueType] for the generated IL
        |> ignore

    // ===== Issue #18135: Can't compile static abstract with byref params =====
    // https://github.com/dotnet/fsharp/issues/18135
    // Static abstract interface members with byref parameters (inref, outref, byref)
    // fail to compile with a cryptic FS2014 error about MethodDefNotFound.
    // [<Fact>]
    let ``Issue_18135_StaticAbstractByrefParams`` () =
        let source = """
module Test

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
        |> shouldSucceed // This will fail with FS2014 MethodDefNotFound - bug exists
        |> ignore

    // ===== Issue #18125: Wrong StructLayoutAttribute.Size for struct unions =====
    // https://github.com/dotnet/fsharp/issues/18125
    // Struct unions with no data fields emit StructLayoutAttribute with Size=1,
    // but the actual size is 4 due to the compiler-generated _tag field.
    // [<Fact>]
    let ``Issue_18125_WrongStructLayoutSize`` () =
        let source = """
module Test

[<Struct>]
type ABC = A | B | C

// StructLayoutAttribute.Size should be >= sizeof<ABC> (which is 4)
// but the compiler emits Size=1

let check() =
    let actualSize = sizeof<ABC>
    let attr = 
        typeof<ABC>.GetCustomAttributes(typeof<System.Runtime.InteropServices.StructLayoutAttribute>, false)
        |> Array.head :?> System.Runtime.InteropServices.StructLayoutAttribute
    let declaredSize = attr.Size
    
    printfn "Actual size: %d, Declared size: %d" actualSize declaredSize
    
    if declaredSize < actualSize then
        failwithf "StructLayout.Size (%d) is less than actual size (%d)" declaredSize actualSize

check()
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed // This will fail - declared size is 1, actual is 4 - bug exists
        |> ignore

    // ===== Issue #17692: Mutual recursion codegen issue with duplicate param names =====
    // https://github.com/dotnet/fsharp/issues/17692
    // In mutually recursive functions, the compiler can generate duplicate 'self@'
    // parameter names in the IL, causing issues when the IL is round-tripped through ilasm.
    // [<Fact>]
    let ``Issue_17692_MutualRecursionDuplicateParamName`` () =
        let source = """
module Test

// Simplified mutual recursion pattern that triggers duplicate 'self@' param names
// The full repro involves more complex mutual recursion with closures

let rec caller x = callee (x - 1)
and callee y = if y > 0 then caller y else 0

// This produces valid runtime behavior but the generated IL has issues
// with duplicate parameter names that ilasm/ildasm round-trip catches

let result = caller 5
printfn "Result: %d" result
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        // The bug manifests as IL with duplicate 'self@' param names in constructors
        // which ilasm warns about and can cause issues in some scenarios
        |> ignore

    // ===== Issue #17641: IsMethod/IsProperty don't act as expected for generated members =====
    // https://github.com/dotnet/fsharp/issues/17641
    // When enumerating declarations in FSharpAssemblyContents, compiler-generated properties
    // like IsUnionCaseTester or methods like Equals have incorrect IsProperty/IsMethod flags.
    // [<Fact>]
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
    // [<Fact>]
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
        |> shouldSucceed // This will fail with "duplicate entry 'get_None' in method table" - bug exists
        |> ignore

    // ===== Issue #16546: NullReferenceException in Debug build with recursive reference =====
    // https://github.com/dotnet/fsharp/issues/16546
    // When using mutually recursive let bindings in a certain order, the Debug build
    // produces a NullReferenceException while Release works correctly.
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
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed // This will fail with NullReferenceException in Debug - bug exists
        |> ignore

    // ===== Issue #16378: Significant allocations logging F# types =====
    // https://github.com/dotnet/fsharp/issues/16378
    // Logging F# discriminated union values using Console.Logger causes ~20x more
    // memory allocation compared to serializing them first due to excessive boxing/allocations.
    // [<Fact>]
    let ``Issue_16378_DULoggingAllocations`` () =
        let source = """
module Test

open System

// This is a performance/allocation issue rather than a correctness bug
// When F# DU values are passed to logging methods that expect obj,
// the compiler generates excessive allocations compared to what's needed

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
    // This path causes excessive allocations
    String.Format("Error: {0}", sampleNotFound) |> ignore

let logSerialized() =
    // This path has minimal allocations
    String.Format("Error: {0}", sampleNotFound.SerializeError()) |> ignore

logDirect()
logSerialized()
printfn "Test completed"
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        // The bug is about allocation overhead, not correctness
        // Both paths work but logDirect allocates ~20x more memory
        |> ignore
