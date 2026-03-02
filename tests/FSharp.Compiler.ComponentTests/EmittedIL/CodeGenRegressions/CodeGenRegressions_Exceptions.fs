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

    // FSharp.Core has [assembly: SecurityTransparent] which prevents overriding
    // SecurityCritical methods like Exception.GetObjectData on .NET Framework.
    // Verify that FSharp.Core exceptions (MatchFailureException) still load and work,
    // have the deserialization ctor, but do NOT have a GetObjectData override.
    [<Fact>]
    let ``Issue_878_FSharpCoreExceptions_NoGetObjectDataOverride`` () =
        let source = """
module Test

// Force MatchFailureException to be loaded by triggering an incomplete match
let triggerMatch x =
    match x with
    | 1 -> "one"

// Verify FSharp.Core exceptions can be created and used without TypeLoadException
let test () =
    try
        triggerMatch 999 |> ignore
        failwith "Expected MatchFailureException"
    with
    | :? MatchFailureException as e ->
        // Verify the exception loaded successfully (no TypeLoadException from GetObjectData)
        printfn "MatchFailureException loaded OK: %s" e.Message

        // Check that deserialization ctor exists (it should — base ctor is SecuritySafeCritical)
        let ctorParams = [| typeof<System.Runtime.Serialization.SerializationInfo>; typeof<System.Runtime.Serialization.StreamingContext> |]
        let ctor = typeof<MatchFailureException>.GetConstructor(
            System.Reflection.BindingFlags.Instance ||| System.Reflection.BindingFlags.NonPublic,
            null, ctorParams, null)
        if ctor = null then failwith "Deserialization ctor missing on MatchFailureException"
        printfn "Deserialization ctor present"

        // GetObjectData should NOT be overridden on MatchFailureException
        // (FSharp.Core is SecurityTransparent, can't override SecurityCritical base)
        let godMethod = typeof<MatchFailureException>.GetMethod("GetObjectData",
            System.Reflection.BindingFlags.Instance ||| System.Reflection.BindingFlags.Public,
            null, ctorParams, null)
        if godMethod <> null && godMethod.DeclaringType = typeof<MatchFailureException> then
            failwith "GetObjectData should NOT be overridden on FSharp.Core exceptions"
        printfn "GetObjectData correctly not overridden"
        0

[<EntryPoint>]
let main _ = test ()
"""
        FSharp source
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore
