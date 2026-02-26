// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.Utilities

module CodeGenRegressions_Exceptions =

    let private getActualIL (result: CompilationResult) =
        match result with
        | CompilationResult.Success s ->
            match s.OutputPath with
            | Some p ->
                let (_, _, actualIL) = ILChecker.verifyILAndReturnActual [] p [ "// dummy" ]
                actualIL
            | None -> failwith "No output path"
        | _ -> failwith "Compilation failed"

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
            ".custom instance void [runtime]System.Security.SecurityCriticalAttribute::.ctor() = ( 01 00 00 00 )"
            "call       instance void [runtime]System.Exception::GetObjectData(class [runtime]System.Runtime.Serialization.SerializationInfo,"
            ".method family specialname rtspecialname instance void  .ctor(class [runtime]System.Runtime.Serialization.SerializationInfo info, valuetype [runtime]System.Runtime.Serialization.StreamingContext context) cil managed"
        ]
        |> ignore

        let actualIL = getActualIL result
        Assert.Contains("AddValue", actualIL)

    // https://github.com/dotnet/fsharp/issues/878

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
