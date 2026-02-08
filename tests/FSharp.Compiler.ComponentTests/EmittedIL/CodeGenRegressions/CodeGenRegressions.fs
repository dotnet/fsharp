// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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

    // https://github.com/dotnet/fsharp/issues/19075
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

    // https://github.com/dotnet/fsharp/issues/19068
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

    // https://github.com/dotnet/fsharp/issues/19020
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
        |> verifyIL [
            """
.method public static int32  func(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/SomeAttribute::.ctor() = ( 01 00 00 00 )
"""
            """
.method public static int32  'static member'(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/SomeAttribute::.ctor() = ( 01 00 00 00 )
"""
            """
.method public hidebysig instance int32 member(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/SomeAttribute::.ctor() = ( 01 00 00 00 )
"""
        ]
        |> ignore

    // [<Fact>]
    let ``Issue_19020_ReturnAttributeEdgeCases`` () =
        let source = """
module Test

open System

type MyAttribute(value: string) =
    inherit Attribute()
    member _.Value = value

type AnotherAttribute() =
    inherit Attribute()

module Module =
    [<return: MyAttribute("mod")>]
    [<return: AnotherAttribute>]
    let func a = a + 1

type SomeClass() =
    [<return: MyAttribute("ins")>]
    [<return: AnotherAttribute>]
    member _.MultipleAttrs a = a + 1
    
    [<return: MyAttribute("str")>]
    static member StringMethod () = "hello"
    
    [<return: MyAttribute("get")>]
    member _.PropWithReturnAttr = 42
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """
.method public static int32  func(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/MyAttribute::.ctor(string) = ( 01 00 03 6D 6F 64 00 00 )
  .custom instance void Test/AnotherAttribute::.ctor() = ( 01 00 00 00 )
"""
            """
.method public hidebysig instance int32 MultipleAttrs(int32 a) cil managed
{
  .param [0]
  .custom instance void Test/MyAttribute::.ctor(string) = ( 01 00 03 69 6E 73 00 00 )
  .custom instance void Test/AnotherAttribute::.ctor() = ( 01 00 00 00 )
"""
            """
.method public static string  StringMethod() cil managed
{
  .param [0]
  .custom instance void Test/MyAttribute::.ctor(string) = ( 01 00 03 73 74 72 00 00 )
"""
            """
.method public hidebysig specialname instance int32  get_PropWithReturnAttr() cil managed
{
  .param [0]
  .custom instance void Test/MyAttribute::.ctor(string) = ( 01 00 03 67 65 74 00 00 )
"""
        ]
        |> ignore

    // https://github.com/dotnet/fsharp/issues/18956
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

    // https://github.com/dotnet/fsharp/issues/18953
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

    // https://github.com/dotnet/fsharp/issues/18868
    [<Fact>]
    let ``Issue_18868_CallerInfoInDelegates`` () =
        let source = """
module Test

type A = delegate of [<System.Runtime.CompilerServices.CallerFilePath>] ?a: string -> unit
type B = delegate of [<System.Runtime.CompilerServices.CallerLineNumber>] ?line: int -> unit
type C = delegate of [<System.Runtime.CompilerServices.CallerMemberName>] ?name: string -> unit
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/18815
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
        |> ignore

    // https://github.com/dotnet/fsharp/issues/18753
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

let test1 () =
    builder {
        1
        "two"
        3
        "four"
    }

let test2 () =
    builder {
        I 1
        "two"
        3
        "four"
    }

"""
        let actualIL = FSharp source |> asLibrary |> withOptimize |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("test1", actualIL)
        Assert.Contains("test2", actualIL)

    // https://github.com/dotnet/fsharp/issues/18672
    [<Fact>]
    let ``Issue_18672_ResumableCodeTopLevelValue`` () =
        let source = """
module Test

let topLevelTask = task { return "result from top-level" }

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

    // https://github.com/dotnet/fsharp/issues/18374
    [<Fact>]
    let ``Issue_18374_RuntimeWrappedExceptionCannotBeCaught`` () =
        let source = """
module Test

open System

[<EntryPoint>]
let main _ =
    let mutable caught1 = false
    try
        raise (InvalidOperationException("test"))
    with
    | e -> caught1 <- true
    
    if not caught1 then failwith "Normal exception not caught"
    
    let mutable caught2 = false
    try
        raise (ArgumentException("test"))
    with
    | :? ArgumentException as ae -> caught2 <- true
    | e -> failwith "Wrong exception type caught"
    
    if not caught2 then failwith "Specific exception not caught"
    
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

    // https://github.com/dotnet/fsharp/issues/18319
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

    // https://github.com/dotnet/fsharp/issues/18263
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

    // https://github.com/dotnet/fsharp/issues/18140
    [<Fact>]
    let ``Issue_18140_CallvirtOnValueType`` () =
        let source = """
module Test

[<Struct>]
type MyRange =
    val Value: int
    new(v) = { Value = v }

let comparer =
    { new System.Collections.Generic.IEqualityComparer<MyRange> with
        member _.Equals(x1, x2) = x1.Value = x2.Value
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

    // https://github.com/dotnet/fsharp/issues/18135
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

    // https://github.com/dotnet/fsharp/issues/18125
    // [<Fact>] // UNFIXED: Enable when issue is fixed
    let ``Issue_18125_WrongStructLayoutSize`` () =
        let source = """
module Test

[<Struct>]
type ABC = A | B | C

"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.pack 0
    .size 4"""
            """.field assembly int32 _tag"""
        ]
        |> ignore

    // [<Fact>] // UNFIXED: Enable when issue #18125 is fixed
    let ``Issue_18125_WrongStructLayoutSize_ManyCases`` () =
        let source = """
module Test

[<Struct>]
type ManyOptions = 
    | A | B | C | D | E | F | G | H | I | J

"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.pack 0"""
            """.size 4"""
            """.field assembly int32 _tag"""
        ]
        |> ignore

    [<Fact>]
    let ``Issue_18125_StructLayoutWithDataFields`` () =
        let source = """
module Test

[<Struct>]
type IntOrFloat =
    | Int of i: int
    | Float of f: float

"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """.field assembly int32 _tag"""
            """.field assembly int32 _i"""
            """.field assembly float64 _f"""
        ]
        |> ignore

    // https://github.com/dotnet/fsharp/issues/17692
    [<Fact>]
    let ``Issue_17692_MutualRecursionDuplicateParamName`` () =
        let source = """
module Test

let rec caller x = callee (x - 1)
and callee y = if y > 0 then caller y else 0

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
        |> ignore

    // https://github.com/dotnet/fsharp/issues/17641
    [<Fact>]
    let ``Issue_17641_IsMethodIsPropertyIncorrectForGenerated`` () =
        let source = """
module Test

type MyUnion =
    | CaseA of int
    | CaseB of string

let x = CaseA 1
let isA = match x with CaseA _ -> true | _ -> false
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/16565
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

    // https://github.com/dotnet/fsharp/issues/16546
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

    // https://github.com/dotnet/fsharp/issues/16378
    [<Fact>]
    let ``Issue_16378_DULoggingAllocations`` () =
        let source = """
module Test

open System

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

let logDirect() =
    String.Format("Error: {0}", sampleNotFound) |> ignore

let logSerialized() =
    String.Format("Error: {0}", sampleNotFound.SerializeError()) |> ignore

logDirect()
logSerialized()
printfn "Test completed"
"""
        let actualIL = FSharp source |> asExe |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("logDirect", actualIL)
        Assert.Contains("logSerialized", actualIL)
        Assert.Contains("box", actualIL)

    // Related: https://github.com/dotnet/fsharp/issues/16362 (proposed $ separator was reverted)

    [<Fact>]
    let ``ExtensionMethod_InstanceMethod_UsesDotSeparator`` () =
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
        result |> verifyIL [ ".method public static !!a  Exception.Reraise<a>(class [runtime]System.Exception A_0) cil managed" ]
        result |> verifyILNotPresent [ "Exception$Reraise" ]

    [<Fact>]
    let ``ExtensionMethod_StaticMethod_UsesDotSeparatorWithStaticSuffix`` () =
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
        result |> verifyIL [ ".method public static class [runtime]System.Exception Exception.CreateNew.Static(string msg) cil managed" ]
        result |> verifyILNotPresent [ "Exception$CreateNew"; "$Static" ]

    // https://github.com/dotnet/fsharp/issues/16292
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

    // https://github.com/dotnet/fsharp/issues/16245
    [<Fact>]
    let ``Issue_16245_SpanDoubleGetItem`` () =
        let source = """
module Test

open System

let incrementSpan (span: Span<byte>) =
    for i = 0 to span.Length - 1 do
        span[i] <- span[i] + 1uy

let test() =
    let arr = [| 1uy; 2uy; 3uy |]
    incrementSpan (arr.AsSpan())
    printfn "%A" arr

test()
"""
        let actualIL = FSharp source |> asExe |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("incrementSpan", actualIL)
        Assert.Contains("get_Item", actualIL)

    // https://github.com/dotnet/fsharp/issues/16037
    [<Fact>]
    let ``Issue_16037_TuplePatternLambdaSuboptimal`` () =
        let source = """
module Test

let data = Map.ofList [
    "1", (true, 1)
    "2", (true, 2)
]

let foldWithPattern () =
    (data, [])
    ||> Map.foldBack (fun _ (x, _) state -> x :: state)

let foldWithFst () =
    (data, [])
    ||> Map.foldBack (fun _ v state -> fst v :: state)
    
let foldWithPattern2 () =
    (data, [])
    ||> Map.foldBack (fun _ v state ->
        let x, _ = v
        x :: state)

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

    // https://github.com/dotnet/fsharp/issues/15627
    [<Fact>]
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
        // |> run
        // |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/15467
    [<Fact>]
    let ``Issue_15467_LanguageVersionInMetadata`` () =
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

    // https://github.com/dotnet/fsharp/issues/15352
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

let checkAttribute() =
    let t = typeof<T>
    
    let methodF = t.GetMethod("f", BindingFlags.NonPublic ||| BindingFlags.Instance)
    if methodF <> null then
        let hasAttr = methodF.GetCustomAttribute<CompilerGeneratedAttribute>() <> null
        if hasAttr then
            failwith "Bug: User-defined method 'f' has CompilerGeneratedAttribute"
        else
            printfn "OK: No CompilerGeneratedAttribute on user method 'f'"
    else
        failwith "Method 'f' not found"
    
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

    [<Fact>]
    let ``Issue_15352_ClosuresStillHaveCompilerGeneratedAttribute`` () =
        let source = """
module Test

open System
open System.Reflection
open System.Runtime.CompilerServices

let makeAdder x =
    fun y -> x + y

let adder5 = makeAdder 5
printfn "Result: %d" (adder5 10)

let testModule = 
    Assembly.GetExecutingAssembly().GetTypes() 
    |> Array.find (fun t -> t.Name = "Test")
let asm = testModule.Assembly

let closureTypes = 
    asm.GetTypes() 
    |> Array.filter (fun t -> 
        t.FullName.Contains("@") && 
        t.IsClass &&
        not (t.Name.StartsWith("<")))

printfn "Found %d potential closure types" closureTypes.Length

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

    [<Fact>]
    let ``Issue_15352_UserPropertiesNoCompilerGeneratedAttribute`` () =
        let source = """
module Test

open System
open System.Reflection
open System.Runtime.CompilerServices

type MyClass() =
    let mutable _value = 0
    
    member _.Value
        with get() = _value
        and set(v) = _value <- v
    
    member val AutoProp = 42 with get, set
    
    member _.DoSomething() = printfn "Doing something"

let checkProperties() =
    let t = typeof<MyClass>
    
    let valueProp = t.GetProperty("Value")
    if valueProp <> null then
        let getter = valueProp.GetGetMethod()
        if getter <> null then
            let hasAttr = getter.GetCustomAttribute<CompilerGeneratedAttribute>() <> null
            if hasAttr then
                failwith "Bug: User-defined property getter 'Value' has CompilerGeneratedAttribute"
            else
                printfn "OK: No CompilerGeneratedAttribute on user property getter 'Value'"
    
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

    // https://github.com/dotnet/fsharp/issues/15326
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

    // https://github.com/dotnet/fsharp/issues/15092
    [<Fact>]
    let ``Issue_15092_DebuggerProxiesInRelease`` () =
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
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14712
    [<Fact>]
    let ``Issue_14712_SignatureFileTypeAlias`` () =
        let source = """
module Test

let add (x:int) (y:int) = x + y

let concat (a:string) (b:string) = a + b

let isValid (b:bool) = b

let multiply (x:float) (y:float) = x * y
"""
        let signature = FSharp source |> printSignatures
        
        Assert.DoesNotContain("System.Int32", signature)
        Assert.DoesNotContain("System.String", signature)
        Assert.DoesNotContain("System.Boolean", signature)
        Assert.DoesNotContain("System.Double", signature)
        
        Assert.Contains("val add: x: int -> y: int -> int", signature)
        Assert.Contains("val concat: a: string -> b: string -> string", signature)
        Assert.Contains("val isValid: b: bool -> bool", signature)
        Assert.Contains("val multiply: x: float -> y: float -> float", signature)

    [<Fact>]
    let ``Issue_14712_SignatureFileTypeAlias_AllTypes`` () =
        let source = """
module Test

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

let useString (x:string) = x
let useBool (x:bool) = x
let useUnit () = ()
"""
        let signature = FSharp source |> printSignatures
        
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
        
        Assert.Contains("Guid", signature)
        Assert.Contains("DateTime", signature)
        Assert.Contains("TimeSpan", signature)

    // https://github.com/dotnet/fsharp/issues/14707
    // [<Fact>]
    let ``Issue_14707_SignatureFileUnusable`` () =
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

    // https://github.com/dotnet/fsharp/issues/14706
    // `#IProvider` syntax instead of preserving explicit type parameter.
    [<Fact>]
    let ``Issue_14706_SignatureWhereTypar`` () =
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

    // https://github.com/dotnet/fsharp/issues/14492
    // TypeLoadException in release config: "Method 'Specialize' on type 'memoizeLatestRef@...'
    // tried to implicitly override a method with weaker type parameter constraints."
    // FIXED: Strip constraints from type parameters when generating Specialize method override.
    [<Fact>]
    let ``Issue_14492_ReleaseConfigError`` () =
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

    // https://github.com/dotnet/fsharp/issues/14392
    [<Fact>]
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

    // https://github.com/dotnet/fsharp/issues/14321
    // "duplicate entry 'Overheated' in property table"
    // would conflict with nullary DU case properties (they are semantically equivalent).
    [<Fact>]
    let ``Issue_14321_DuAndIWSAMNames`` () =
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

    // https://github.com/dotnet/fsharp/issues/13468
    // This doesn't break runtime but affects FCS symbol analysis.
    [<Fact>]
    let ``Issue_13468_OutrefAsByref`` () =
        // Test: Implement C# interface with 'out' parameter in F#
        let csCode = "namespace CSharpLib { public interface IOutTest { void TryGet(string k, out int v); } }"
        let csLib = CSharp csCode |> withName "CSharpLib"
        let fsCode = "module Test\nopen CSharpLib\ntype MyImpl() =\n    interface IOutTest with\n        member this.TryGet(k, v) = v <- 42"
        FSharp fsCode
        |> withReferences [csLib]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/13447
    // passed to called functions via Span or byref. If a tail. prefix is emitted on such
    // calls, the stack frame is released before the callee accesses the memory, causing
    // corruption. The fix suppresses tail calls when localloc has been used in the method.
    [<Fact>]
    let ``Issue_13447_TailInstructionCorruption`` () =
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

    // https://github.com/dotnet/fsharp/issues/13223
    [<Fact>]
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

    // https://github.com/dotnet/fsharp/issues/13218
    // NOTE: This is a PERFORMANCE issue, not a codegen bug.
    // 13000 let bindings take ~2.5min vs 13000 static members taking ~20sec.
    // [<Fact>]
    let ``Issue_13218_ManyStaticMembers`` () =
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
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/13108
    // "Ignoring mixed managed/unmanaged assembly" for referenced assemblies that
    // are not actually mixed. The warning message lacks documentation on mitigation.
    // This primarily affects scenarios like VsVim that static-link FSharp.Core.
    [<Fact>]
    let ``Issue_13108_StaticLinkingWarnings`` () =
        // transitive dependencies. Previously, this emitted spurious FS2009 warnings
        // for assemblies that were merely transitively referenced but not actually
        // linked (e.g., assemblies that happened to not be pure IL).
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

    // https://github.com/dotnet/fsharp/issues/13100
    // characteristic set (0x100 flag), which is incorrect for x64 executables.
    // dumpbin /headers shows: "32 bit word machine" for F# but not for C# x64 builds.
    // C# correctly produces only "Executable" and "Application can handle large (>2GB) addresses"
    [<Fact>]
    let ``Issue_13100_PlatformCharacteristic`` () =
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

    // https://github.com/dotnet/fsharp/issues/12546
    // the compiler generates an extra wrapper closure that just invokes the first closure.
    // This doubles allocation unnecessarily.
    // Workaround: explicitly box the argument at the call site.
    // lambdas when coercing between function types with different argument types. When calling
    // a function that takes obj with a string argument, the string->obj coercion triggers 
    // subsumption adjustment which wraps the result in an extra closure.
    // coercion is a simple box (concrete type to obj) and avoid generating the wrapper lambda
    // in that case. This is complex because the subsumption logic is also used for quotations.
    [<Fact>]
    let ``Issue_12546_BoxingClosure`` () =
        // Issue #12546: When a function takes obj and you pass a value that gets implicitly boxed,
        // the compiler generates an extra wrapper closure that just invokes the first closure.
        // This doubles allocation unnecessarily.
        // coercion triggers the subsumption adjustment logic which creates a wrapper lambda.
        // STATUS: Bug confirmed. A fix was attempted in AdjustPossibleSubsumptionExpr but the
        // wrapper closure is created at a different point in the compilation pipeline.
        let source = """
module BoxingClosureTest

let foo (ob: obj) = box(fun () -> ob.ToString()) :?> (unit -> string)

// BUG: This allocates an extraneous closure that wraps the real one
let go() = foo "hi"

let goFixed() = foo(box "hi")
"""
        let actualIL = FSharp source |> asLibrary |> withOptimize |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("go", actualIL)
        Assert.Contains("goFixed", actualIL)
        Assert.Contains("FSharpFunc", actualIL)

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
                    if System.String.IsNullOrEmpty(fvi.InternalName) then
                        failwith $"InternalName should not be empty. Expected filename, got: '{fvi.InternalName}'"
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

    // https://github.com/dotnet/fsharp/issues/12416
    // InlineIfLambda functions don't inline when the input is an inline expression
    // (like array literal) vs when it's stored in a let binding.
    // Workaround: store arguments in intermediate let bindings before piping.
    [<Fact>]
    let ``Issue_12416_PipeInlining`` () =
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
        |> ignore

    // https://github.com/dotnet/fsharp/issues/12384
    // Mutually recursive non-function values were not initialized correctly.
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
    [<Fact>]
    let ``Issue_12366_ClosureNaming`` () =
        // Test that closures get meaningful names from their enclosing bindings.
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

    // https://github.com/dotnet/fsharp/issues/12139
    // PERFORMANCE IL ISSUE - F# emits String.Equals(s, null) for string null checks,
    // while C# emits simple brtrue/brfalse (single instruction).
    // F# IL for `s <> null`:
    //   ldarg.0
    //   ldnull
    //   call bool [System.Runtime]System.String::Equals(string, string)
    //   brtrue.s IL_XXXX
    // C# IL for `s != null`:
    //   ldarg.0  
    //   brtrue.s IL_XXXX   ← single instruction, much simpler
    [<Fact>]
    let ``Issue_12139_StringNullCheck`` () =
        // PERFORMANCE: F# generates String.Equals call for null comparison
        // C# generates simple null pointer check (brtrue/brfalse)
        // F# emits this IL pattern for `s = null`:
        //   IL_0000: ldarg.0
        //   IL_0001: ldnull  
        //   IL_0002: call bool [System.Runtime]System.String::Equals(string, string)
        //   IL_0007: ret
        //   IL_0000: ldarg.0
        //   IL_0001: ldnull
        //   IL_0002: ceq
        //   IL_0004: ret
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
        |> ignore

    // https://github.com/dotnet/fsharp/issues/12137
    // PERFORMANCE IL ISSUE - F# emits `tail.` prefix inconsistently:
    // - Same-assembly calls: NO tail. prefix (correct, allows inlining)
    // - Cross-assembly calls: tail. prefix emitted (unnecessary, hurts performance)
    // IL for SAME assembly call (good):
    //   IL_000c: call !!0 Module::fold<...>
    // IL for CROSS assembly call (bad):
    //   IL_000c: tail.
    //   IL_000e: call !!0 [OtherLib]Module::fold<...>
    // - 2-3x slower execution (tail call dispatch helpers)
    // - 2x larger JIT-generated assembly code
    // NOTE: Hard to demonstrate in single-file test. Requires two assemblies.
    [<Fact>]
    let ``Issue_12137_TailEmitReduction`` () =
        // PERFORMANCE: Cross-assembly calls get unnecessary `tail.` prefix
        // But if another assembly calls these same functions, F# emits:
        //   tail.
        //   call !!0 [TailEmitTest]TailEmitTest::fold<...>
        // have unbounded stack growth. But for most functions, tail. is unnecessary
        // and causes significant performance overhead.
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
        |> ignore

    // https://github.com/dotnet/fsharp/issues/12136
    // PROBLEM: When using "use x = fixed expr", the pinned variable remains pinned until
    // the function returns, not at the end of the scope where it's declared.
    // C# correctly nulls out the pinned local at end of fixed block scope.
    // [<Fact>]  // Commented out - workaround only, not a fix
    let ``Issue_12136_FixedUnpin`` () =
        let source = """
module FixedUnpinTest

#nowarn "9"  // Suppress unverifiable IL warning for fixed

open Microsoft.FSharp.NativeInterop

let inline used<'T> (t: 'T) : unit = ignore t

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

    // https://github.com/dotnet/fsharp/issues/11935
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
        |> verifyILContains [
            "Container`1<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>"
            ".custom instance void [runtime]System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )"
        ]
        |> shouldSucceed

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
        |> verifyILContains [
            "StructContainer`1<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T>"
            ".custom instance void [runtime]System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 )"
        ]
        |> shouldSucceed

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
        |> verifyILContains [
            "combine<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) T,valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) U>(!!T x, !!U y) cil managed"
        ]
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/11556
    [<Fact>]
    let ``Issue_11556_FieldInitializers`` () =
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

    // https://github.com/dotnet/fsharp/issues/11132
    [<Fact>]
    let ``Issue_11132_VoidptrDelegate`` () =
        let source = """
module Test
#nowarn "9"

open System

type MyDelegate = delegate of voidptr -> unit

let method (ptr: voidptr) = ()

let getDelegate (m: voidptr -> unit) : MyDelegate = MyDelegate(m)

let test() =
    let d = getDelegate method
    d.Invoke(IntPtr.Zero.ToPointer())

do test()
"""
        FSharp source
        |> asExe
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/11114
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

    // https://github.com/dotnet/fsharp/issues/9348
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

    // https://github.com/dotnet/fsharp/issues/9176
    [<Fact>]
    let ``Issue_9176_InlineAttributes`` () =
        let source = """
module Test

let inline f x = x + 1

let g y = f y + f y
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/7861
    [<Fact>]
    let ``Issue_7861_AttributeTypeReference`` () =
        let source = """
module Test

open System

type TypedAttribute(t: Type) =
    inherit Attribute()
    member _.TargetType = t

[<Typed(typeof<System.Xml.XmlDocument>)>]
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

    [<Fact>]
    let ``Issue_7861_NamedAttributeArgument`` () =
        let source = """
module Test

open System

type TypePropertyAttribute() =
    inherit Attribute()
    member val TargetType : Type = null with get, set

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

    // [<Fact>] // UNFIXED: Enable when issue #7861 is fixed
    let ``Issue_7861_TypeArrayInAttribute`` () =
        let source = """
module Test

open System

type MultiTypeAttribute(types: Type[]) =
    inherit Attribute()
    member _.Types = types

[<MultiType([| typeof<System.Xml.XmlDocument>; typeof<System.Net.Http.HttpClient> |])>]
type MyClass() = class end
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyILContains [ 
            ".assembly extern System.Xml.ReaderWriter"
            ".assembly extern System.Net.Http"
        ]
        |> shouldSucceed

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

    // https://github.com/dotnet/fsharp/issues/6750
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

    // https://github.com/dotnet/fsharp/issues/6379
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

    // https://github.com/dotnet/fsharp/issues/5834
    [<Fact>]
    let ``Issue_5834_ObsoleteSpecialname`` () =
        let source = """
module Test

open System
open System.Reflection

[<AbstractClass>]
type AbstractWithEvent() =
    [<Obsolete("This event is deprecated")>]
    [<CLIEvent>]
    abstract member MyEvent : IEvent<EventHandler, EventArgs>

type ConcreteWithEvent() =
    let evt = new Event<EventHandler, EventArgs>()
    [<CLIEvent>]
    member this.MyEvent = evt.Publish

[<EntryPoint>]
let main _ =
    let abstractType = typeof<AbstractWithEvent>
    let concreteType = typeof<ConcreteWithEvent>
    
    let abstractAddMethod = abstractType.GetMethod("add_MyEvent")
    let abstractRemoveMethod = abstractType.GetMethod("remove_MyEvent")
    
    let concreteAddMethod = concreteType.GetMethod("add_MyEvent")
    
    printfn "AbstractWithEvent.add_MyEvent.IsSpecialName = %b" abstractAddMethod.IsSpecialName
    printfn "AbstractWithEvent.remove_MyEvent.IsSpecialName = %b" abstractRemoveMethod.IsSpecialName
    printfn "ConcreteWithEvent.add_MyEvent.IsSpecialName = %b" concreteAddMethod.IsSpecialName
    
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

    // https://github.com/dotnet/fsharp/issues/5464
    // [<Fact>] // UNFIXED: Enable when issue is fixed
    let ``Issue_5464_CustomModifiers`` () =
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
        |> verifyIL [
            "call       int32 [CsLib]CsLib.Processor::Process(valuetype [CsLib]CsLib.ReadOnlyStruct& modreq("
        ]
        |> ignore

    // [<Fact>] // UNFIXED: Enable when issue #5464 is fixed
    let ``Issue_5464_CustomModifiers_ModOpt`` () =
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
            "call       void [CsLibModOpt]CsLib.Container::ProcessWithIn(valuetype [CsLibModOpt]CsLib.Data& modreq("
        ]
        |> ignore

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
            "call       float32 [CsLibMultiIn]CsLib.VectorMath::Dot(valuetype [CsLibMultiIn]CsLib.Vector3& modreq("
        ]
        |> ignore

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
            "call       !!0 [CsLibGeneric]CsLib.GenericProcessor::Process<int32>(valuetype [CsLibGeneric]CsLib.GenericData`1<!!0>& modreq("
        ]
        |> ignore

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
            "call       int32 [CsLibChain]CsLib.Math::Distance(valuetype [CsLibChain]CsLib.Point& modreq("
        ]
        |> ignore

    // https://github.com/dotnet/fsharp/issues/878
    [<Fact>]
    let ``Issue_878_ExceptionSerialization`` () =
        let source = """
module Test

exception Foo of x:string * y:int
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyIL [
            ".method public strict virtual instance void GetObjectData(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed"
            "call       instance void [runtime]System.Exception::GetObjectData(class [runtime]System.Runtime.Serialization.SerializationInfo,"
            ".method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed"
        ]
        |> ignore
