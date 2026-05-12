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
    let ``Delegate with OptionalArgument and CallerLineNumber`` () =
        FSharp """open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type TestDelegate = delegate of [<OptionalArgument; CallerLineNumber>] line: int option -> unit
let f = fun (line: int option) -> 
    match line with
    | Some l -> printf "line: %d" l
    | None -> printf "no line"
let d = TestDelegate f
d.Invoke()"""
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutput "line: 9"
        
    [<Fact>]
    let ``Delegate with OptionalArgument and CallerFilePath`` () =
        FSharp """open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type TestDelegate = delegate of [<OptionalArgument; CallerFilePath>] path: string option -> unit
let f = fun (path: string option) -> 
    match path with
    | Some p -> printf "has path"
    | None -> printf "no path"
let d = TestDelegate f
d.Invoke()"""
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutput "has path"

    [<Fact>]
    let ``Delegate with OptionalArgument and CallerMemberName`` () =
        FSharp """open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type TestDelegate = delegate of [<OptionalArgument; CallerMemberName>] memberName: string option -> unit
let f = fun (memberName: string option) -> 
    match memberName with
    | Some m -> printf "has member"
    | None -> printf "no member"
let d = TestDelegate f
d.Invoke()"""
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutput "has member"

    [<Fact>]
    let ``Delegate with nested option type`` () =
        FSharp """type TestDelegate = delegate of x: int option option -> unit
let f = fun (x: int option option) -> 
    match x with
    | Some (Some v) -> printfn "value: %d" v
    | Some None -> printfn "inner none"
    | None -> printfn "outer none"
let d = TestDelegate f
d.Invoke(Some (Some 42))
d.Invoke(Some None)
d.Invoke(None)"""
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyOutputContains [| "value: 42"; "inner none"; "outer none" |] 
