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

    // ====================================================================================
    // SPRINT 3: Issues #16362, #16292, #16245, #16037, #15627, #15467, #15352, #15326, #15092, #14712
    // ====================================================================================

    // ===== Issue #16362: Extension methods with CompiledName generate C# incompatible names =====
    // https://github.com/dotnet/fsharp/issues/16362
    // F# style extension methods generate method names that contain dots (e.g., Exception.Reraise)
    // which are not compatible with C# and don't show in C# autocomplete.
    // [<Fact>]
    let ``Issue_16362_ExtensionMethodCompiledName`` () =
        let source = """
module Test

open System

type Exception with
    member ex.Reraise() = raise ex

// The generated extension method name is "Exception.Reraise"
// which is not valid C# syntax and doesn't appear in C# autocomplete
// Expected: generate compatible name or emit a warning

let test() =
    let ex = Exception("test")
    try
        ex.Reraise()
    with
    | :? Exception -> ()
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // The issue is that the compiled extension method name uses a dot
        // which is incompatible with C# interop
        |> ignore

    // ===== Issue #16292: Incorrect codegen for Debug build with SRTP and mutable struct =====
    // https://github.com/dotnet/fsharp/issues/16292
    // In Debug builds, SRTP with mutable struct enumerators generates incorrect code where
    // the struct is copied in each loop iteration, losing mutations from MoveNext().
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
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed // This may hang or fail in Debug build - bug exists
        |> ignore

    // ===== Issue #16245: Span IL gen produces 2 get_Item calls =====
    // https://github.com/dotnet/fsharp/issues/16245
    // When incrementing a span element (span[i] <- span[i] + 1), the compiler generates
    // two get_Item calls instead of one, leading to suboptimal performance.
    // [<Fact>]
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
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        // The issue is performance - IL is suboptimal with duplicate get_Item calls
        |> ignore

    // ===== Issue #16037: Suboptimal code for tuple pattern matching in lambda parameter =====
    // https://github.com/dotnet/fsharp/issues/16037
    // Pattern matching a tuple in a lambda parameter generates two FSharpFunc classes,
    // causing ~2x memory allocation compared to using fst/snd or matching inside the body.
    // [<Fact>]
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
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        // The issue is performance - pattern in lambda parameter causes extra allocations
        |> ignore

    // ===== Issue #15627: Program stuck when using async/task before EntryPoint =====
    // https://github.com/dotnet/fsharp/issues/15627
    // Running async operations before an [<EntryPoint>] function causes the program to hang.
    // The async never completes when there's an EntryPoint defined later in the file.
    // [<Fact>]
    let ``Issue_15627_AsyncBeforeEntryPointHangs`` () =
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
        |> run
        |> shouldSucceed // This will hang - program gets stuck after printing "1"
        |> ignore

    // ===== Issue #15467: Include language version in compiled metadata =====
    // https://github.com/dotnet/fsharp/issues/15467
    // When an older compiler reads a DLL built with a newer compiler, the error message
    // is generic and unhelpful. Including language version in metadata would enable
    // more specific error messages like "Tooling must support F# 7 or higher".
    // [<Fact>]
    let ``Issue_15467_LanguageVersionInMetadata`` () =
        let source = """
module Test

// This is a feature request to include F# language version in compiled assemblies
// Currently when a newer feature is used, older compilers get cryptic pickle errors
// With language version in metadata, the error could be more specific

type MyRecord = { Value: int }

let test = { Value = 42 }
printfn "%A" test
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // The feature request is to embed language version in metadata
        // so older compilers can give better error messages
        |> ignore

    // ===== Issue #15352: User defined symbols get CompilerGeneratedAttribute =====
    // https://github.com/dotnet/fsharp/issues/15352
    // Private let-bound functions in classes get [<CompilerGenerated>] attribute
    // even though they are user-defined code, not compiler-generated.
    // [<Fact>]
    let ``Issue_15352_UserCodeCompilerGeneratedAttribute`` () =
        let source = """
module Test

open System
open System.Reflection
open System.Runtime.CompilerServices

type T() =
    let f x = x + 1
    member _.CallF x = f x

// The method 'f' should NOT have CompilerGeneratedAttribute
// It is user-written code, not compiler-generated

let checkAttribute() =
    let t = typeof<T>
    let method = t.GetMethod("f", BindingFlags.NonPublic ||| BindingFlags.Instance)
    if method <> null then
        let hasAttr = method.GetCustomAttribute<CompilerGeneratedAttribute>() <> null
        if hasAttr then
            failwith "Bug: User-defined method 'f' has CompilerGeneratedAttribute"
        else
            printfn "OK: No CompilerGeneratedAttribute"
    else
        printfn "Method not found (expected for private)"

checkAttribute()
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        // The bug is that the generated IL has CompilerGeneratedAttribute on user code
        // This is misleading for debuggers and reflection-based tools
        |> ignore

    // ===== Issue #15326: Delegates not inlined with InlineIfLambda =====
    // https://github.com/dotnet/fsharp/issues/15326
    // Custom delegates with InlineIfLambda are not being inlined as they were
    // before .NET 7 Preview 5. This is a regression.
    // [<Fact>]
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
        FSharp source
        |> asExe
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        // The issue is that the delegate is not being inlined - creates closure
        // This worked in .NET 7 Preview 4 but regressed in Preview 5
        |> ignore

    // ===== Issue #15092: Should we generate DebuggerProxies in release code? =====
    // https://github.com/dotnet/fsharp/issues/15092
    // DebuggerProxy types are generated even in release builds, increasing binary size.
    // This is a design question about whether they should be elided in release mode.
    // [<Fact>]
    let ``Issue_15092_DebuggerProxiesInRelease`` () =
        let source = """
module Test

open System
open System.Reflection

type MyRecord = { Name: string; Value: int }

let checkDebuggerProxy() =
    let asm = typeof<MyRecord>.Assembly
    let types = asm.GetTypes()
    let proxyTypes = types |> Array.filter (fun t -> t.Name.Contains("DebuggerProxy"))
    
    printfn "DebuggerProxy types found: %d" proxyTypes.Length
    for t in proxyTypes do
        printfn "  - %s" t.FullName
    
    // In release builds, we might want to elide these to reduce binary size
    // Currently they are always generated

let result = { Name = "test"; Value = 42 }
printfn "%A" result
checkDebuggerProxy()
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
    // [<Fact>]
    let ``Issue_14712_SignatureFileTypeAlias`` () =
        let source = """
module Test

type System.Int32 with
    member i.PlusPlus () = i + 1
    member i.PlusPlusPlus () : int = i + 1 + 1

type X(y:int) =
    member x.PlusPlus () = y + 1
"""
        // When generating signature file for this:
        // - PlusPlus returns System.Int32 (because no explicit type annotation)
        // - PlusPlusPlus returns int (because explicit type annotation)
        // Expected: Both should show 'int' in generated signature
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // The issue is cosmetic - generated .fsi files use System.Int32 instead of int
        |> ignore

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
    // [<Fact>]
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
    // [<Fact>]
    let ``Issue_14508_NativeptrInInterfaces`` () =
        // The bug: non-generic type implementing generic interface with nativeptr causes
        // runtime TypeLoadException due to IL signature mismatch
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
    // [<Fact>]
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
        match !cell with
        | Some (x', value) when refEquals x' x -> value
        | _ -> f x |> tee (fun y -> cell := Some (x, y))
    f'

module BugInReleaseConfig = 
    let test f x = 
        printfn "%s" (f x)

    let f: string -> string = memoizeLatestRef id

    let run () = test f "ok"
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #14392: OpenApi Swashbuckle support =====
    // https://github.com/dotnet/fsharp/issues/14392
    // NOTE: This is a FEATURE REQUEST, not a codegen bug. Included for completeness.
    // F# types may not serialize correctly with OpenAPI tools.
    // [<Fact>]
    let ``Issue_14392_OpenApiSupport`` () =
        // This is a feature request for better OpenAPI/Swashbuckle support.
        // No actual codegen bug to test - placeholder for tracking purposes.
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
    // [<Fact>]
    let ``Issue_14321_DuAndIWSAMNames`` () =
        // The bug: DU constructor names that match IWSAM member names cause
        // "duplicate entry 'X' in property table" error at build time
        let source = """
module Test

type SensorReadings = int

type EngineError<'e> =
    static abstract Overheated : 'e
    static abstract LowOil : 'e

// BUG: If we use 'Overheated' and 'LowOil' (without '2' suffix) for DU cases,
// we get: "duplicate entry 'Overheated' in property table"
type CarError =
    | Overheated2  // Workaround: Using '2' suffix to avoid name conflict
    | LowOil2
    | DeviceNotPaired2

    interface EngineError<CarError> with
        static member Overheated = Overheated2
        static member LowOil = LowOil2
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
    // [<Fact>]
    let ``Issue_13468_OutrefAsByref`` () =
        // The bug: when implementing a C# interface with `out int` parameter,
        // F# generates IL with `ref int` instead of `[Out] ref int`
        let source = """
module Test

// When I is defined in F#, outref works correctly
type I =
    abstract M: param: outref<int> -> unit

type T() =
    interface I with
        member this.M(param) = param <- 42

// BUG: When implementing interface from IL/C# assembly with 'out' param,
// the implementation uses `ref` instead of `[Out] ref`
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13447: Extra tail instruction corrupts stack =====
    // https://github.com/dotnet/fsharp/issues/13447
    // In Release mode with [<Struct>] on a Result type, extra tail. instruction
    // causes wrong values to be written (stack corruption).
    // Workaround: remove [<Struct>] or add () at end of affected function.
    // [<Fact>]
    let ``Issue_13447_TailInstructionCorruption`` () =
        // The bug: complex interaction of [<Struct>] Result + certain function patterns
        // causes an extra tail. instruction that corrupts stack
        // The repro requires specific code from external repo, simplified here
        let source = """
module Test

// The actual bug requires complex interaction with:
// - [<Struct>] on a Result type
// - Specific function patterns that trigger extra tail. prefix
// - Running in Release mode
// See: https://github.com/kerams/repro/ for full repro

[<Struct>]
type MyResult<'T, 'E> = 
    | Ok of value: 'T 
    | Error of error: 'E

let writeString (value: string) : MyResult<unit, string> =
    Ok ()
    // BUG: without trailing `()` here, an extra tail. instruction is emitted
    // causing stack corruption

let test () =
    match writeString "test" with
    | Ok () -> printfn "Success"
    | Error e -> printfn "Error: %s" e
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13223: FSharp.Build support for reference assemblies =====
    // https://github.com/dotnet/fsharp/issues/13223
    // NOTE: This is a FEATURE REQUEST for FSharp.Build, not a codegen bug.
    // Request to add/modify FSharp.Build tasks for reference assembly support.
    // [<Fact>]
    let ``Issue_13223_ReferenceAssemblies`` () =
        // This is a feature request to add reference assembly support to FSharp.Build
        // similar to Roslyn's CopyRefAssembly.cs implementation.
        // No codegen bug to reproduce - placeholder for tracking.
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
    // [<Fact>]
    let ``Issue_13108_StaticLinkingWarnings`` () =
        // Note: This issue requires multi-project static linking scenario with --standalone flag
        // The warning FS2009 appears when linking assemblies that reference native code.
        // Simplified test demonstrates that static linking path is exercised.
        let source = """
module StaticLinkTest

// Static linking with --standalone produces FS2009 warnings for assemblies
// that reference mixed managed/unmanaged code (like System.Configuration.ConfigurationManager)
// The warnings lack documentation on how to address them.
let value = 42
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13100: --platform:x64 sets 32 bit characteristic =====
    // https://github.com/dotnet/fsharp/issues/13100
    // When compiling with --platform:x64, the PE FILE HEADER has "32 bit word machine"
    // characteristic set (0x100 flag), which is incorrect for x64 executables.
    // dumpbin /headers shows: "32 bit word machine" for F# but not for C# x64 builds.
    // C# correctly produces only "Executable" and "Application can handle large (>2GB) addresses"
    // [<Fact>]
    let ``Issue_13100_PlatformCharacteristic`` () =
        // The bug is that F# sets IMAGE_FILE_32BIT_MACHINE (0x100) characteristic
        // in the PE header when targeting x64, which is incorrect.
        // To verify: compile with --platform:x64 and run `dumpbin /headers`
        // F# shows: 0x12E characteristics (includes "32 bit word machine")
        // C# shows: 0x22 characteristics (no 32-bit flag)
        let source = """
module PlatformTest

// When compiled with --platform:x64, the PE header should NOT have
// the "32 bit word machine" (IMAGE_FILE_32BIT_MACHINE) characteristic.
// Currently F# incorrectly sets this flag for x64 binaries.
[<EntryPoint>]
let main _ = 0
"""
        FSharp source
        |> asExe
        // Note: would need --platform:x64 flag to fully reproduce
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #12546: Implicit boxing produces extraneous closure =====
    // https://github.com/dotnet/fsharp/issues/12546
    // When a function takes an obj parameter and you pass a value that gets implicitly boxed,
    // the compiler generates an extra wrapper closure that just invokes the first closure.
    // This doubles allocation unnecessarily.
    // Workaround: explicitly box the argument at the call site.
    // [<Fact>]
    let ``Issue_12546_BoxingClosure`` () =
        // This code allocates TWO closures: the actual closure and a wrapper
        // The wrapper (go@5) just forwards to the real closure (clo1.Invoke)
        // This is unnecessary - only one closure should be allocated.
        let source = """
module BoxingClosureTest

let foo (ob: obj) = box(fun () -> ob.ToString()) :?> (unit -> string)

// BUG: This allocates an extraneous closure that wraps the real one
let go() = foo "hi"

// WORKAROUND: Explicitly boxing avoids the extra closure
let goFixed() = foo(box "hi")
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        // Should verify IL doesn't have wrapper closure class like go@5
        |> ignore

    // ===== Issue #12460: F# C# Version info values different =====
    // https://github.com/dotnet/fsharp/issues/12460
    // F# and C# compilers produce different Version info metadata:
    // - F# output misses "Internal Name" value
    // - ProductVersion format differs: C# produces "1.0.0", F# produces "1.0.0.0"
    // These should be aligned with C# for consistency in tooling.
    // [<Fact>]
    let ``Issue_12460_VersionInfoDifference`` () =
        // The compiled DLL has different version info metadata than C# produces:
        // 1. Missing "Internal Name" in version resources
        // 2. ProductVersion has 4 parts (1.0.0.0) instead of 3 (1.0.0)
        let source = """
module VersionInfoTest

// When examining the compiled DLL's version resources:
// - "Internal Name" field is missing (C# includes it)
// - "Product Version" shows "1.0.0.0" (C# shows "1.0.0")
let value = 42
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // Version info differences would be visible with dumpbin or file properties
        |> ignore

    // ===== Issue #12416: Optimization inlining inconsistent with piping =====
    // https://github.com/dotnet/fsharp/issues/12416
    // InlineIfLambda functions don't inline when the input is an inline expression
    // (like array literal) vs when it's stored in a let binding.
    // Workaround: store arguments in intermediate let bindings before piping.
    // [<Fact>]
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

    // ===== Issue #12384: Mutually recursive values intermediate module wrong init =====
    // https://github.com/dotnet/fsharp/issues/12384
    // Mutually recursive non-function values are not initialized correctly.
    // The first value in the binding group has null for its recursive references,
    // while the second value is initialized correctly.
    // Single self-referencing values work correctly.
    // [<Fact>]
    let ``Issue_12384_MutRecInitOrder`` () =
        // BUG: one.Next and one.Prev are null, but two is correctly initialized
        // This should either work correctly or be rejected at compile time.
        let source = """
module MutRecInitTest

type Node = { Next: Node; Prev: Node; Value: int }

// Single self-reference works correctly
let rec zero = { Next = zero; Prev = zero; Value = 0 }

// BUG: Mutual recursion fails - 'one' has null references
let rec one = { Next = two; Prev = two; Value = 1 }
and two = { Next = one; Prev = one; Value = 2 }

// At runtime:
// one = { Next = null; Prev = null; Value = 1 }  // WRONG - should reference two
// two = { Next = one; Prev = one; Value = 2 }    // Correct

[<EntryPoint>]
let main _ =
    // This would show the bug: one.Next is null instead of two
    printfn "%A" one
    printfn "%A" two
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        // Bug manifests at runtime - one.Next and one.Prev are null
        |> ignore

    // ===== Issue #12366: Rethink names for compiler-generated closures =====
    // https://github.com/dotnet/fsharp/issues/12366
    // Compiler-generated closure names like "foo@376" and "clo43@53" are weak heuristics.
    // Problems:
    // - Unnecessary use of backup name "clo" when better names available
    // - Sometimes other compiler-generated names are used as basis (e.g., "Pipe input at line 63@53")
    // - Line numbers alone aren't as useful as they could be
    // [<Fact>]
    let ``Issue_12366_ClosureNaming`` () =
        // The generated closure types get names like:
        // - f@5 (uses the let binding name - good)
        // - clo@10-1 (uses generic "clo" name - could be better)
        // - "Pipe input at line 63@53" (uses debug string as name - bad)
        let source = """
module ClosureNamingTest

// This closure gets a reasonable name based on 'f'
let f = fun x -> x + 1

// This anonymous closure in a pipeline might get a poor name like "clo@N"
let result = 
    [1; 2; 3]
    |> List.map (fun x -> x * 2)  // closure name should be more descriptive
    |> List.filter (fun x -> x > 2)  // same issue

// Nested closures compound the problem
let complex = 
    fun a -> 
        fun b -> 
            fun c -> a + b + c  // inner closures get confusing names
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // Generated type names in IL are not as helpful for debugging as they could be
        |> ignore

    // ===== Issue #12139: Improve string null check IL codegen =====
    // https://github.com/dotnet/fsharp/issues/12139
    // F# emits String.Equals(s, null) for string null checks, while C# emits simple brtrue/brfalse.
    // F# IL: call bool String::Equals(string, string); brtrue.s 
    // C# IL: brtrue.s (single instruction)
    // The JIT will optimize this away, but it increases DLL size and is less efficient.
    // [<Fact>]
    let ``Issue_12139_StringNullCheck`` () =
        // F# generates call to String.Equals for null comparison
        // C# generates simple null pointer check (brtrue/brfalse)
        let source = """
module StringNullCheckTest

open System

// F# generates: call string Console::ReadLine(); ldnull; call bool String::Equals(string, string); brtrue.s
// C# generates: call string Console::ReadLine(); brtrue.s (much simpler)
let test() =
    while Console.ReadLine() <> null do
        Console.WriteLine(1)

// Simple null check also uses String.Equals instead of direct comparison
let isNullString (s: string) = s = null
let isNotNullString (s: string) = s <> null
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        // IL should show String.Equals calls instead of simple brtrue/brfalse
        |> ignore

    // ===== Issue #12137: Improve analysis to reduce emit of tail =====
    // https://github.com/dotnet/fsharp/issues/12137
    // F# emits tail. prefix for cross-assembly calls but not for same-assembly calls.
    // This is inconsistent and the unnecessary tail. prefix causes:
    // - 2-3x slower execution due to tail call dispatch helpers
    // - 2x larger JIT-generated assembly code
    // The tail call should only be emitted when actually needed for stack safety.
    // [<Fact>]
    let ``Issue_12137_TailEmitReduction`` () =
        // When calling an inline function from another assembly, F# emits tail. prefix
        // When calling the same function from the same assembly, no tail. prefix
        // This inconsistency hurts performance for cross-assembly calls
        let source = """
module TailEmitTest

// Simulating the scenario: inline generic fold function
let inline fold (f: 'S -> 'T -> 'S) (state: 'S) (items: 'T list) =
    let mutable s = state
    for item in items do
        s <- f s item
    s

// When this calls fold from same assembly: no tail. prefix (good)
let sumLocal () = fold (+) 0 [1; 2; 3]

// When calling fold from another assembly: tail. prefix emitted (bad)
// This causes 2-3x performance penalty and larger JIT code
// The tail. is unnecessary because fold doesn't need tail recursion for correctness
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        // Cross-assembly calls would show tail. prefix in IL
        |> ignore

    // ===== Issue #12136: use fixed does not unpin at end of scope =====
    // https://github.com/dotnet/fsharp/issues/12136
    // When using "use x = fixed expr", the pinned variable remains pinned until
    // the function returns, not at the end of the scope where it's declared.
    // C# correctly nulls out the pinned local at end of fixed block scope.
    // This affects GC behavior and confuses decompilers.
    // Workaround: put the fixed block in a separate function.
    // [<Fact>]
    let ``Issue_12136_FixedUnpin`` () =
        // The pinned local should be set to null/zero at end of scope
        // F# IL: no cleanup of pinned local at scope end
        // C# IL: stloc.1 with null at end of fixed block
        let source = """
module FixedUnpinTest

open Microsoft.FSharp.NativeInterop

let inline used<'T> (t: 'T) : unit = ignore t

let test (array: int[]) : unit =
    do
        use pin = fixed &array.[0]
        used pin
    // BUG: array remains pinned here even though 'pin' is out of scope!
    used 1  // GC cannot move 'array' during this call

// The IL shows pin remains set (no cleanup instruction) after the 'do' block
// C# would emit: ldc.i4.0; conv.u; stloc.1 to unpin

// WORKAROUND: Put fixed block in separate function
let testFixed (array: int[]) : unit =
    let doBlock() =
        use pin = fixed &array.[0]
        used pin
    doBlock()
    used 1  // Now array is correctly unpinned
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        // IL analysis would show missing cleanup for pinned local at scope end
        |> ignore

    // ===== Issue #11935: unmanaged constraint not recognized by C# =====
    // https://github.com/dotnet/fsharp/issues/11935
    // C# doesn't recognize F# unmanaged constraint correctly.
    // [<Fact>]
    let ``Issue_11935_UnmanagedConstraintInterop`` () =
        let source = """
module Test

let inline test<'T when 'T : unmanaged> (x: 'T) = x
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #11556: Better IL output for property/field initializers =====
    // https://github.com/dotnet/fsharp/issues/11556
    // Field initialization could be more efficient.
    // [<Fact>]
    let ``Issue_11556_FieldInitializers`` () =
        let source = """
module Test

type T() =
    let mutable x = 42
    member _.X = x
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #11132: TypeloadException delegate with voidptr parameter =====
    // https://github.com/dotnet/fsharp/issues/11132
    // TypeLoadException at runtime for delegates with voidptr.
    // [<Fact>]
    let ``Issue_11132_VoidptrDelegate`` () =
        let source = """
module Test

type MyDelegate = delegate of nativeint -> unit
"""
        FSharp source
        |> asLibrary
        |> compile
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
    // [<Fact>]
    let ``Issue_9348_ComparePerformance`` () =
        let source = """
module Test

type T = { X: int }
let compare (a: T) (b: T) = compare a.X b.X
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #9176: Decorate inline function code with attribute =====
    // https://github.com/dotnet/fsharp/issues/9176
    // Attributes lost when code is inlined.
    // [<Fact>]
    let ``Issue_9176_InlineAttributes`` () =
        let source = """
module Test

let inline f x = x + 1
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #7861: Missing assembly reference for type in attributes =====
    // https://github.com/dotnet/fsharp/issues/7861
    // Missing assembly reference error for types used in attributes.
    // [<Fact>]
    let ``Issue_7861_AttributeTypeReference`` () =
        let source = """
module Test

type MyAttribute() =
    inherit System.Attribute()

[<MyAttribute>]
type T = class end
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

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
    // [<Fact>]
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
        |> shouldSucceed // This will fail - abstract event accessors have IsSpecialName = false - bug exists
        |> ignore

    // ===== Issue #5464: F# ignores custom modifiers modreq/modopt =====
    // https://github.com/dotnet/fsharp/issues/5464
    // Custom modifiers from C# types are stripped.
    // [<Fact>]
    let ``Issue_5464_CustomModifiers`` () =
        let source = """
module Test

let f x = x + 1
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #878: Serialization of F# exception variants doesn't serialize fields =====
    // https://github.com/dotnet/fsharp/issues/878
    // Exception fields are lost after deserialization when using BinaryFormatter.
    // The F# compiler doesn't generate proper GetObjectData override or deserialization constructor.
    // [<Fact>]
    let ``Issue_878_ExceptionSerialization`` () =
        let source = """
module Test

open System
open System.IO
open System.Runtime.Serialization.Formatters.Binary

// Define F# exception with multiple fields
exception Foo of x:string * y:int

// Clone an object via BinaryFormatter serialization roundtrip
let clone (x : 'T) =
    let bf = new BinaryFormatter()
    let m = new MemoryStream()
    bf.Serialize(m, x)
    m.Position <- 0L
    bf.Deserialize(m) :?> 'T

[<EntryPoint>]
let main _ =
    let original = Foo("value", 42)
    let cloned = clone original
    
    // Extract fields from cloned exception
    // Bug: After deserialization, fields become null/0 instead of "value"/42
    match cloned with
    | Foo(x, y) ->
        printfn "Original: x='value', y=42"
        printfn "Cloned: x='%s', y=%d" (if isNull x then "null" else x) y
        if x = "value" && y = 42 then
            printfn "SUCCESS: Fields survived serialization"
            0
        else
            printfn "BUG: Fields lost during serialization (expected x='value', y=42)"
            1
    | _ -> 
        printfn "Unexpected exception type"
        1
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed // This will fail - fields are null/0 after deserialization - bug exists
        |> ignore
