// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module ComputationExpressionTests =
    [<Fact>]
    let ``A CE not using Zero does not require Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2;}
        """
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``A CE explicitly using Zero fails without a defined Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2;()}
        """
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 39, Line 10, Col 18, Line 10, Col 20, "The type 'ListBuilder' does not define the field, constructor or member 'Zero'.")
        |> ignore

    [<Fact>]
    let ``A CE explicitly using Zero succeeds with a defined Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Zero () = []
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2;()}
        """
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``A CE with a complete if-then expression does not require Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2; if true then 3 else 4;}
        """
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``A CE with a missing/empty else branch implicitly requires Zero``() =
        FSharp """
module ComputationExpressionTests
type ListBuilder () =
    member _.Combine (a: List<'T>, b) = a @ b
    member _.Yield  x = List.singleton x
    member _.Delay expr = expr () :  List<'T>

let lb = ListBuilder ()

let x = lb {1; 2; if true then 3;}
        """
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 708, Line 10, Col 19, Line 10, Col 31, "This control construct may only be used if the computation expression builder defines a 'Zero' method")
        |> ignore