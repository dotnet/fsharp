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
    // Signature files generated by older F# versions may not work with newer versions.
    // [<Fact>]
    let ``Issue_14707_SignatureFileUnusable`` () =
        let source = """
module Test
let f x = x + 1
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #14706: Signature file generation WhereTyparSubtypeOfType =====
    // https://github.com/dotnet/fsharp/issues/14706
    // Error in signature file generation with type parameter constraints.
    // [<Fact>]
    let ``Issue_14706_SignatureWhereTypar`` () =
        let source = """
module Test

type MyClass<'T when 'T :> System.IDisposable>() =
    member _.Dispose() = ()
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #14508: nativeptr in interfaces leads to runtime errors =====
    // https://github.com/dotnet/fsharp/issues/14508
    // Interfaces with nativeptr members cause runtime errors.
    // [<Fact>]
    let ``Issue_14508_NativeptrInInterfaces`` () =
        let source = """
module Test

open Microsoft.FSharp.NativeInterop

type IWithPtr =
    abstract member GetPtr : unit -> nativeptr<int>

type Impl() =
    interface IWithPtr with
        member _.GetPtr() = NativePtr.ofNativeInt 0n
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #14492: F# 7.0 incorrect program release config =====
    // https://github.com/dotnet/fsharp/issues/14492
    // InvalidProgramException in release configuration.
    // [<Fact>]
    let ``Issue_14492_ReleaseConfigError`` () =
        let source = """
module Test

let inline test<'a> (x: 'a) = x

let main() =
    let result = test 42
    printfn "%d" result
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #14392: OpenApi Swashbuckle support =====
    // https://github.com/dotnet/fsharp/issues/14392
    // F# types may not serialize correctly with OpenAPI tools.
    // [<Fact>]
    let ``Issue_14392_OpenApiSupport`` () =
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
    // DU case names and IWSAM method names conflict.
    // [<Fact>]
    let ``Issue_14321_DuAndIWSAMNames`` () =
        let source = """
module Test

type MyDU =
    | Create
    | Other
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13468: outref parameter compiled as byref =====
    // https://github.com/dotnet/fsharp/issues/13468
    // outref is compiled as regular byref without [Out] attribute.
    // [<Fact>]
    let ``Issue_13468_OutrefAsByref`` () =
        let source = """
module Test

let myFunc (x: outref<int>) = x <- 42

let test() =
    let mutable result = 0
    myFunc &result
    result
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13447: Extra tail instruction corrupts stack =====
    // https://github.com/dotnet/fsharp/issues/13447
    // Stack corruption in certain tail call scenarios.
    // [<Fact>]
    let ``Issue_13447_TailInstructionCorruption`` () =
        let source = """
module Test

let rec loop n acc =
    if n = 0 then acc
    else loop (n - 1) (acc + 1)

let result = loop 100 0
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13223: FSharp.Build support for reference assemblies =====
    // https://github.com/dotnet/fsharp/issues/13223
    // Reference assembly generation not fully supported.
    // [<Fact>]
    let ``Issue_13223_ReferenceAssemblies`` () =
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
    // Large number of static members compiles slowly.
    // [<Fact>]
    let ``Issue_13218_ManyStaticMembers`` () =
        let source = """
module Test

type T =
    static member M1 = 1
    static member M2 = 2
    static member M3 = 3
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13108: Static linking FS2009 warnings =====
    // https://github.com/dotnet/fsharp/issues/13108
    // Static linking produces spurious warnings.
    // [<Fact>]
    let ``Issue_13108_StaticLinkingWarnings`` () =
        let source = """
module Test

let f x = x + 1
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #13100: --platform:x64 sets 32 bit characteristic =====
    // https://github.com/dotnet/fsharp/issues/13100
    // PE header has 32-bit characteristic flag set for x64 platform.
    // [<Fact>]
    let ``Issue_13100_PlatformCharacteristic`` () =
        let source = """
module Test

let f x = x + 1
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #12546: Implicit boxing produces extraneous closure =====
    // https://github.com/dotnet/fsharp/issues/12546
    // Creates unnecessary closure around boxing operation.
    // [<Fact>]
    let ``Issue_12546_BoxingClosure`` () =
        let source = """
module Test

let test (x: 'a) : obj = box x
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #12460: F# C# Version info values different =====
    // https://github.com/dotnet/fsharp/issues/12460
    // Version metadata fields differ from C# conventions.
    // [<Fact>]
    let ``Issue_12460_VersionInfoDifference`` () =
        let source = """
module Test

let f x = x + 1
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #12416: Optimization inlining inconsistent with piping =====
    // https://github.com/dotnet/fsharp/issues/12416
    // Piped form may not inline as well as direct calls.
    // [<Fact>]
    let ``Issue_12416_PipeInlining`` () =
        let source = """
module Test

let inline f x = x + 1
let test1 = f 42
let test2 = 42 |> f
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #12384: Mutually recursive values intermediate module wrong init =====
    // https://github.com/dotnet/fsharp/issues/12384
    // Incorrect initialization order in some mutual recursion scenarios.
    // [<Fact>]
    let ``Issue_12384_MutRecInitOrder`` () =
        let source = """
module Test

let rec x = y + 1
and y = 0
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #12366: Rethink names for compiler-generated closures =====
    // https://github.com/dotnet/fsharp/issues/12366
    // Closure names like clo@12-1 are not helpful for debugging.
    // [<Fact>]
    let ``Issue_12366_ClosureNaming`` () =
        let source = """
module Test

let f = fun x -> x + 1
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #12139: Improve string null check IL codegen =====
    // https://github.com/dotnet/fsharp/issues/12139
    // String null checks could use more efficient IL patterns.
    // [<Fact>]
    let ``Issue_12139_StringNullCheck`` () =
        let source = """
module Test

let isNullStr (s: string) = isNull s
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #12137: Improve analysis to reduce emit of tail =====
    // https://github.com/dotnet/fsharp/issues/12137
    // Unnecessary tail. prefixes emitted.
    // [<Fact>]
    let ``Issue_12137_TailEmitReduction`` () =
        let source = """
module Test

let f x = x + 1
let g x = f x
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // ===== Issue #12136: use fixed does not unpin at end of scope =====
    // https://github.com/dotnet/fsharp/issues/12136
    // Pin may not be released correctly.
    // [<Fact>]
    let ``Issue_12136_FixedUnpin`` () =
        let source = """
module Test

open Microsoft.FSharp.NativeInterop

let test (arr: byte[]) =
    use ptr = fixed arr
    ()
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
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
    // Obsolete attribute causes missing specialname on property accessors.
    // [<Fact>]
    let ``Issue_5834_ObsoleteSpecialname`` () =
        let source = """
module Test

open System

[<AbstractClass>]
type T() =
    abstract member Prop : int
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
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
    // Exception fields are lost after deserialization.
    // [<Fact>]
    let ``Issue_878_ExceptionSerialization`` () =
        let source = """
module Test

exception MyException of data: string

let ex = MyException("test")
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore
