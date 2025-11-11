// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test.Compiler

module DelegateDefinition =

    [<Fact>]
    let ``Delegate definition with primary constructor and argument.`` () =
        FSharp
            """
namespace FSharpTest
    type T(x: int) =
        delegate of int -> int
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 552, Line 3, Col 11, Line 3, Col 19, "Only class types may take value arguments")
        ]
        
    [<Fact>]
    let ``Delegate definition with primary constructor no argument.`` () =
        FSharp
            """
namespace FSharpTest
    type T() =
        delegate of int -> int
        """
        |> compile
        |> shouldFail
        |> shouldFail
        |> withDiagnostics [
            (Error 552, Line 3, Col 11, Line 3, Col 13, "Only class types may take value arguments")
        ]

    [<Fact>]
    let ``Delegate definition`` () =
        FSharp
            """
namespace FSharpTest
    type T =
        delegate of int -> int
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Delegate with optional parameter`` () =
        FSharp """open System.Runtime.CompilerServices
type A = delegate of [<CallerLineNumber>] ?a: int -> unit
let f = fun (a: int option) -> defaultArg a 100 |> printf "line: %d"
let a = A f
a.Invoke()"""
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutput "line: 5"
        
    [<Fact>]
    let ``Delegate with struct optional parameter`` () =
        FSharp """type A = delegate of [<Struct>] ?a: int -> unit
let f = fun (a: int voption) -> defaultValueArg a 100 |> printf "line: %d"
let a = A f
a.Invoke(5)"""
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutput "line: 5"

    [<Fact>]
    let ``Delegate with OptionalArgument and CallerFilePath`` () =
        FSharp """open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type TestDelegate = delegate of [<OptionalArgument; CallerFilePath>] path: string option -> unit
let f = fun (path: string option) -> 
    match path with
    | Some p -> if p.Contains("test") then printfn "SUCCESS" else printfn "FAIL: %s" p
    | None -> printfn "FAIL: None"
let d = TestDelegate f
d.Invoke()"""
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutput "SUCCESS"

    [<Fact>]
    let ``Delegate with OptionalArgument and CallerLineNumber`` () =
        FSharp """open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type TestDelegate = delegate of [<OptionalArgument; CallerLineNumber>] line: int option -> unit
let f = fun (line: int option) -> 
    match line with
    | Some l -> if l > 0 then printfn "SUCCESS: line %d" l else printfn "FAIL"
    | None -> printfn "FAIL: None"
let d = TestDelegate f
d.Invoke()"""
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Delegate with OptionalArgument and CallerMemberName`` () =
        FSharp """open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type TestDelegate = delegate of [<OptionalArgument; CallerMemberName>] memberName: string option -> unit
let f = fun (memberName: string option) -> 
    match memberName with
    | Some m -> printfn "member: %s" m
    | None -> printfn "FAIL"
let d = TestDelegate f
d.Invoke()"""
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Delegate with CallerFilePath without optional should fail`` () =
        FSharp """namespace Test
open System.Runtime.CompilerServices
type TestDelegate = delegate of [<CallerFilePath>] path: string -> unit"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1247, Line 3, Col 41, Line 3, Col 45, "'CallerFilePath' can only be applied to optional arguments")
        ]

    [<Fact>]
    let ``Delegate with CallerFilePath on wrong type should fail`` () =
        FSharp """namespace Test
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type TestDelegate = delegate of [<OptionalArgument; CallerFilePath>] x: int option -> unit"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1246, Line 4, Col 69, Line 4, Col 70, "'CallerFilePath' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'")
        ]

    [<Fact>]
    let ``Delegate with CallerLineNumber on wrong type should fail`` () =
        FSharp """namespace Test
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type TestDelegate = delegate of [<OptionalArgument; CallerLineNumber>] x: string option -> unit"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1246, Line 4, Col 73, Line 4, Col 74, "'CallerLineNumber' must be applied to an argument of type 'int', but has been applied to an argument of type 'string'")
        ]

