// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module DotLambdaTests =

    [<Fact>]
    let ``Script: _.ToString()`` () =
        Fsx "_.ToString()"
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``Simple anonymous unary function shorthands compile`` () =
        FSharp """
module One
let a1 : {| Foo : {| Bar : string |}|} -> string = _.Foo.Bar
let a2 : {| Foo : int array |} -> int = _.Foo.[5]
let a3 : {| Foo : int array |} -> int = _.Foo[5]
let a4 : {| Foo : unit -> string |} -> string = _.Foo()
let a5 : {| Foo : int -> {| X : string |} |} -> string = _.Foo(5).X

open System
let a6 = [1] |> List.map _.ToString()
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``Nested anonymous unary function shorthands compile`` () =
        FSharp """
module One
let a : string = {| Inner =  (fun x -> x.ToString()) |} |> _.Inner([5] |> _.[0])
        """
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3546, Line 3, Col 75, Line 3, Col 76, "Discard is ambiguous")
        
    [<Fact>]
    let ``Anonymous unary function shorthand with conflicting wild argument`` () =
        FSharp """
module One
let a : string -> string = (fun _ -> 5 |> _.ToString())
let b : int -> int -> string = function |5 -> (fun _ -> "Five") |_ -> _.ToString()
let c : string = let _ = "test" in "asd" |> _.ToString()
        """
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 3546, Line 3, Col 43, Line 3, Col 44, "Discard is ambiguous")