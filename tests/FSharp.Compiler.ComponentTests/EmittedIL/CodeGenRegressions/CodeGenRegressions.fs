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
        |> shouldSucceed
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
        |> shouldSucceed
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
        |> withOptimize
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

    // https://github.com/dotnet/fsharp/issues/18374
    // Tests that non-Exception objects thrown from IL are wrapped in RuntimeWrappedException
    [<Fact>]
    let ``Issue_18374_RuntimeWrappedExceptionNonExceptionThrow`` () =
        let csCode = """
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CSharpHelper {
    public static class NonExceptionThrower {
        public static Action CreateStringThrower() {
            var dm = new DynamicMethod("ThrowString", typeof(void), Type.EmptyTypes);
            var il = dm.GetILGenerator();
            il.Emit(OpCodes.Ldstr, "non-exception payload");
            il.Emit(OpCodes.Throw);
            il.Emit(OpCodes.Ret);
            return (Action)dm.CreateDelegate(typeof(Action));
        }
    }
}"""
        let csLib = CSharp csCode |> withName "CSharpHelper"
        let fsCode = """
module Test
open System.Runtime.CompilerServices

[<EntryPoint>]
let main _ =
    let thrower = CSharpHelper.NonExceptionThrower.CreateStringThrower()
    let mutable caught = false
    try
        thrower.Invoke()
    with
    | :? RuntimeWrappedException as rwe ->
        caught <- true
        let payload = rwe.WrappedException :?> string
        if payload <> "non-exception payload" then
            failwithf "Wrong payload: %s" payload
    | e ->
        caught <- true
        printfn "Caught as %s: %s" (e.GetType().Name) e.Message

    if not caught then failwith "Non-exception throw was not caught"
    printfn "SUCCESS: Non-exception throw caught correctly"
    0
"""
        FSharp fsCode
        |> withReferences [csLib]
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/18374
    [<Fact>]
    let ``Issue_18374_RuntimeWrappedExceptionIL`` () =
        let source = """
module Test
let catchAll () =
    try
        failwith "test"
    with
    | e -> e.Message
"""
        let actualIL = FSharp source |> asLibrary |> compile |> shouldSucceed |> getActualIL
        Assert.Contains("isinst", actualIL)
        Assert.DoesNotContain("castclass", actualIL)

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

[<EntryPoint>]
let main _ =
    f<T>()
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/18125
    // Tests letrec lambda reordering: lambdas must be allocated before non-lambda bindings
    // that reference them, to avoid null closure references in mutual recursion.
    [<Fact>]
    let ``Issue_18125_LetRecLambdaReordering`` () =
        let source = """
module Test

// Mutual recursion where lambdas reference record values and vice versa.
// The compiler must reorder bindings so closures are allocated before records that capture them.
[<NoComparison; NoEquality>]
type Node = { Value: int; GetLabel: unit -> string }

let test () =
    let rec a = { Value = 1; GetLabel = labelA }
    and b = { Value = 2; GetLabel = labelB }
    and labelA () = sprintf "A(%d)->B(%d)" a.Value b.Value
    and labelB () = sprintf "B(%d)->A(%d)" b.Value a.Value
    
    if a.GetLabel() <> "A(1)->B(2)" then failwithf "Expected A(1)->B(2), got %s" (a.GetLabel())
    if b.GetLabel() <> "B(2)->A(1)" then failwithf "Expected B(2)->A(1), got %s" (b.GetLabel())
    printfn "SUCCESS"

[<EntryPoint>]
let main _ =
    test()
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
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

    // https://github.com/dotnet/fsharp/issues/14706
    [<Fact>]
    let ``Issue_14706_SignatureWhereTypar`` () =
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
    [<Fact>]
    let ``Issue_14508_NativeptrInInterfaces_CompileOnly`` () =
        let source = """
module Test

open Microsoft.FSharp.NativeInterop

type IFoo<'T when 'T : unmanaged> =
    abstract member Pointer : nativeptr<'T>

type Broken() =
    member x.Pointer : nativeptr<int> = Unchecked.defaultof<_>
    interface IFoo<int> with
        member x.Pointer = x.Pointer

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

    // https://github.com/dotnet/fsharp/issues/14508
    [<Fact>]
    let ``Issue_14508_NativeptrInInterfaces_RuntimeWorking`` () =
        let source = """
open Microsoft.FSharp.NativeInterop

type IFoo<'T when 'T : unmanaged> =
    abstract member Pointer : nativeptr<'T>

type Working<'T when 'T : unmanaged>() =
    member x.Pointer : nativeptr<'T> = Unchecked.defaultof<_>
    interface IFoo<'T> with
        member x.Pointer = x.Pointer

printfn "Working type loaded successfully"
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14492
    [<Fact>]
    let ``Issue_14492_ReleaseConfigError`` () =
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
    [<Fact>]
    let ``Issue_14321_DuAndIWSAMNames`` () =
        let source = """
module Test

#nowarn "3535"  // IWSAM warning

type EngineError<'e> =
    static abstract Overheated : 'e
    static abstract LowOil : 'e

type CarError =
    | Overheated
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

    // https://github.com/dotnet/fsharp/issues/14321
    // Runtime test: type must load without "duplicate entry in method table"
    [<Fact>]
    let ``Issue_14321_DuAndIWSAMNames_Runtime`` () =
        let source = """
module Test

#nowarn "3535"

type EngineError<'e> =
    static abstract Overheated : 'e
    static abstract LowOil : 'e

type CarError =
    | Overheated
    | LowOil
    | DeviceNotPaired

    interface EngineError<CarError> with
        static member Overheated = Overheated
        static member LowOil = LowOil

[<EntryPoint>]
let main _ =
    let err = CarError.Overheated
    match err with
    | Overheated -> printfn "Got Overheated"
    | LowOil -> printfn "Got LowOil"
    | DeviceNotPaired -> printfn "Got DeviceNotPaired"
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/13468
    [<Fact>]
    let ``Issue_13468_OutrefAsByref_IL`` () =
        let csCode = "namespace CSharpLib { public interface IOutTest { void TryGet(string k, out int v); } }"
        let csLib = CSharp csCode |> withName "CSharpLib"
        let fsCode = "module Test\nopen CSharpLib\ntype MyImpl() =\n    interface IOutTest with\n        member this.TryGet(k, v) = v <- 42"
        let actualIL =
            FSharp fsCode
            |> withReferences [csLib]
            |> asLibrary
            |> compile
            |> shouldSucceed
            |> getActualIL
        Assert.Contains("[out]", actualIL)

    // https://github.com/dotnet/fsharp/issues/13468
    [<Fact>]
    let ``Issue_13468_OutrefAsByref_Runtime`` () =
        let csCode = """
namespace CSharpLib {
    public interface IOutTest { bool TryGet(string k, out int v); }
    public static class OutTestHelper {
        public static string Run(IOutTest impl) {
            int v;
            bool ok = impl.TryGet("key", out v);
            return ok ? v.ToString() : "fail";
        }
    }
}"""
        let csLib = CSharp csCode |> withName "CSharpLib"
        let fsCode = """
module Test
open CSharpLib
type MyImpl() =
    interface IOutTest with
        member this.TryGet(k, v) = v <- 42; true

[<EntryPoint>]
let main _ =
    let result = OutTestHelper.Run(MyImpl())
    if result <> "42" then failwithf "Expected 42, got %s" result
    printfn "Success: %s" result
    0
"""
        FSharp fsCode
        |> withReferences [csLib]
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/13447
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

let useStackAlloc () : MyResult<int, string> =
    let ptr = NativePtr.stackalloc<byte> 100
    let span = Span<byte>(NativePtr.toVoidPtr ptr, 100)
    span.[0] <- 42uy
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

    // https://github.com/dotnet/fsharp/issues/13108
    [<Fact>]
    let ``Issue_13108_StaticLinkingWarnings`` () =
        let source = """
module StaticLinkTest

let value = List.iter (fun x -> printfn "%d" x) [1; 2; 3]

[<EntryPoint>]
let main _ = 0
"""
        FSharp source
        |> asExe
        |> withOptions ["--standalone"]
        |> compile
        |> shouldSucceed
        |> withDiagnostics []
        |> ignore

    // https://github.com/dotnet/fsharp/issues/13100
    [<Fact>]
    let ``Issue_13100_PlatformCharacteristic`` () =
        let source = """
module PlatformTest

[<EntryPoint>]
let main _ = 0
"""
        FSharp source
        |> asExe
        |> withPlatform ExecutionPlatform.X64
        |> compile
        |> shouldSucceed
        |> withPeReader (fun rdr -> 
            let characteristics = rdr.PEHeaders.CoffHeader.Characteristics
            if not (characteristics.HasFlag(System.Reflection.PortableExecutable.Characteristics.LargeAddressAware)) then
                failwith $"x64 binary should have LargeAddressAware flag. Found: {characteristics}"
            if characteristics.HasFlag(System.Reflection.PortableExecutable.Characteristics.Bit32Machine) then
                failwith $"x64 binary should NOT have Bit32Machine flag. Found: {characteristics}")
        |> ignore

    // https://github.com/dotnet/fsharp/issues/12546
    [<Fact>]
    let ``Issue_12546_BoxingClosure`` () =
        let source = """
module BoxingClosureTest

let foo (ob: obj) = box(fun () -> ob.ToString()) :?> (unit -> string)

let go() = foo "hi"

let goFixed() = foo(box "hi")
"""
        let actualIL = FSharp source |> asLibrary |> withOptimize |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("go", actualIL)
        Assert.Contains("goFixed", actualIL)
        Assert.Contains("FSharpFunc", actualIL)

    // https://github.com/dotnet/fsharp/issues/12460
    [<Fact>]
    let ``Issue_12460_VersionInfoDifference`` () =
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
                    if fvi.ProductVersion <> "5.6.7" then
                        failwith $"ProductVersion should be '5.6.7' (from AssemblyInformationalVersion), got: '{fvi.ProductVersion}'"
                    if fvi.FileVersion <> "1.2.3.4" then
                        failwith $"FileVersion should be '1.2.3.4', got: '{fvi.FileVersion}'"
                    result
                | None -> failwith "Output path not found"
            | _ -> failwith "Compilation failed"
        |> ignore

    [<Fact>]
    let ``Issue_12460_VersionInfoFallback`` () =
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
                    if System.String.IsNullOrEmpty(fvi.InternalName) then
                        failwith $"InternalName should not be empty, got: '{fvi.InternalName}'"
                    if fvi.FileVersion <> "2.3.4.5" then
                        failwith $"FileVersion should be '2.3.4.5', got: '{fvi.FileVersion}'"
                    if System.String.IsNullOrEmpty(fvi.OriginalFilename) then
                        failwith $"OriginalFilename should not be empty, got: '{fvi.OriginalFilename}'"
                    result
                | None -> failwith "Output path not found"
            | _ -> failwith "Compilation failed"
        |> ignore

    // https://github.com/dotnet/fsharp/issues/12416
    [<Fact>]
    let ``Issue_12416_PipeInlining`` () =
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

let thisIsInlined1 () = ofArray values |>> fold (+) 0

let thisIsInlined2 () = 
  let vs = [|0..100|]
  ofArray vs |>> fold (+) 0

let thisIsNotInlined () = ofArray [|0..100|] |>> fold (+) 0
"""
        FSharp source
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/12384
    [<Fact>]
    let ``Issue_12384_MutRecInitOrder`` () =
        let source = """
module MutRecInitTest

type Node = { Next: Node; Prev: Node; Value: int }

let rec zero = { Next = zero; Prev = zero; Value = 0 }

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
    [<Fact>]
    let ``Issue_12366_ClosureNaming`` () =
        let source = """
module ClosureNamingTest

let processData items =
    items
    |> List.map (fun x -> x * 2)    
    |> List.filter (fun x -> x > 2) 

let outerFunc a =
    let innerFunc b =
        fun c -> a + b + c  
    innerFunc
"""
        let actualIL = FSharp source |> asLibrary |> compile |> shouldSucceed |> getActualIL

        Assert.Contains("outerFunc@", actualIL)
        Assert.DoesNotContain("'clo@", actualIL)

    [<Fact>]
    let ``Issue_12366_ClosureNaming_Capturing`` () =
        let source = """
module CapturingClosureTest

let makeAdder n =
    fun x -> x + n

let makeMultiplier n =
    let multiply x = x * n
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
    [<Fact>]
    let ``Issue_12139_StringNullCheck`` () =
        let source = """
module StringNullCheckTest

open System

let isNullString (s: string) = s = null

let isNotNullString (s: string) = s <> null

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
    [<Fact>]
    let ``Issue_12137_TailEmitReduction`` () =
        let source = """
module TailEmitTest

let inline fold (f: 'S -> 'T -> 'S) (state: 'S) (items: 'T list) =
    let mutable s = state
    for item in items do
        s <- f s item
    s

let sumLocal () = fold (+) 0 [1; 2; 3]
"""
        FSharp source
        |> asLibrary
        |> withOptimize
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

    // https://github.com/dotnet/fsharp/issues/878
    [<Fact>]
    let ``Issue_878_ExceptionSerialization`` () =
        let source = """
module Test

exception Foo of x:string * y:int
"""
        let result =
            FSharp source
            |> asLibrary
            |> compile
            |> shouldSucceed

        result
        |> verifyIL [
            ".method public strict virtual instance void GetObjectData(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed"
            "call       instance void [runtime]System.Exception::GetObjectData(class [runtime]System.Runtime.Serialization.SerializationInfo,"
            ".method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed"
        ]
        |> ignore

        let actualIL = getActualIL result
        Assert.Contains("AddValue", actualIL)

    // https://github.com/dotnet/fsharp/issues/878
    // Runtime roundtrip test using SerializationInfo directly (BinaryFormatter removed in .NET 10)
    [<Fact>]
    let ``Issue_878_ExceptionSerialization_Roundtrip`` () =
        let source = """
module Test
open System
open System.Runtime.Serialization

#nowarn "44" // Serialization types are obsolete but needed for testing ISerializable
#nowarn "67"

exception Foo of x:string * y:int

let roundtrip (e: Exception) =
    let info = SerializationInfo(e.GetType(), FormatterConverter())
    let ctx = StreamingContext(StreamingContextStates.All)
    e.GetObjectData(info, ctx)
    let ctor =
        e.GetType().GetConstructor(
            System.Reflection.BindingFlags.Instance ||| System.Reflection.BindingFlags.NonPublic ||| System.Reflection.BindingFlags.Public,
            null,
            [| typeof<SerializationInfo>; typeof<StreamingContext> |],
            null)
    if ctor = null then failwith "Deserialization constructor not found"
    ctor.Invoke([| info :> obj; ctx :> obj |]) :?> Exception

[<EntryPoint>]
let main _ =
    let original = Foo("value", 42)
    // Check GetObjectData actually writes our fields
    let info = SerializationInfo(original.GetType(), FormatterConverter())
    let ctx = StreamingContext(StreamingContextStates.All)
    original.GetObjectData(info, ctx)
    let xVal = info.GetString("x")
    let yVal = info.GetInt32("y")
    if xVal <> "value" then failwithf "GetObjectData: Expected x='value', got '%s'" xVal
    if yVal <> 42 then failwithf "GetObjectData: Expected y=42, got %d" yVal
    
    // Check full roundtrip
    let cloned = roundtrip original
    // Access fields via internal backing fields using reflection
    let xField = cloned.GetType().GetField("x@", System.Reflection.BindingFlags.Instance ||| System.Reflection.BindingFlags.NonPublic)
    let yField = cloned.GetType().GetField("y@", System.Reflection.BindingFlags.Instance ||| System.Reflection.BindingFlags.NonPublic)
    if xField = null then failwith "Field x@ not found"
    if yField = null then failwith "Field y@ not found"
    let xCloned = xField.GetValue(cloned) :?> string
    let yCloned = yField.GetValue(cloned) :?> int
    if xCloned <> "value" then failwithf "Roundtrip: Expected x='value', got '%s'" xCloned
    if yCloned <> 42 then failwithf "Roundtrip: Expected y=42, got %d" yCloned
    printfn "SUCCESS: Foo(value, 42) roundtripped correctly"
    0
"""
        FSharp source
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore
