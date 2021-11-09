// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Constructor`` =

    [<Fact>]
    let ``Invalid Record``() =
        FSharp """
type Record = {field1:int; field2:int}
let doSomething (xs) = List.map (fun {field1=x} -> x) xs
doSomething {Record.field1=0; field2=0}
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error   1,  Line 4, Col 13, Line 4, Col 40, "This expression was expected to have type\n    'Record list'    \nbut here has type\n    'Record'    ")
            (Warning 20, Line 4, Col 1,  Line 4, Col 40, "The result of this expression has type 'int list' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")]

    [<Fact>]
    let ``Comma In Rec Ctor``() =
        FSharp """
type Person = { Name : string; Age : int; City : string }
let x = { Name = "Isaac", Age = 21, City = "London" }
        """
        |> withLangVersion50
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1,   Line 3, Col 18, Line 3, Col 52, "This expression was expected to have type\n    'string'    \nbut here has type\n    ''a * 'b * 'c'    " + System.Environment.NewLine + "A ';' is used to separate field values in records. Consider replacing ',' with ';'.")
            (Error 764, Line 3, Col 9,  Line 3, Col 54, "No assignment given for field 'Age' of type 'Test.Person'")]

    [<Fact>]
    let ``Missing Comma In Ctor``() =
        FSharp """
type Person() =
    member val Name = "" with get,set
    member val Age = 0 with get,set

let p =
    Person(Name = "Fred"
           Age = 18)
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error   39,  Line 7, Col 12, Line 7, Col 16, "The value or constructor 'Name' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   nameof" + System.Environment.NewLine + "   nan")
            (Warning 20,  Line 7, Col 12, Line 7, Col 25, "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'.")
            (Error   39,  Line 8, Col 12, Line 8, Col 15, "The value or constructor 'Age' is not defined.")
            (Error   501, Line 7, Col 5,  Line 8, Col 21, "The object constructor 'Person' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> Person'. If some of the arguments are meant to assign values to properties, consider separating those arguments with a comma (',').")]

    [<Fact>]
    let ``Missing Ctor Value``() =
        FSharp """
type Person(x:int) =
    member val Name = "" with get,set
    member val Age = x with get,set

let p =
    Person(Name = "Fred",
           Age = 18)
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 496, Line 7, Col 5, Line 8, Col 21, "The member or object constructor 'Person' requires 1 argument(s). The required signature is 'new: x: int -> Person'.")

    [<Fact>]
    let ``Extra Argument In Ctor``() =
        FSharp """
type Person() =
    member val Name = "" with get,set
    member val Age = 0 with get,set

let p =
    Person(1)
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 501, Line 7, Col 5, Line 7, Col 14,
                                 "The object constructor 'Person' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> Person'.")

    [<Fact>]
    let ``Extra Argument In Ctor2``() =
        FSharp """
type Person() =
    member val Name = "" with get,set
    member val Age = 0 with get,set

let b = 1

let p =
    Person(1=b)
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 501, Line 9, Col 5, Line 9, Col 16,
                                 "The object constructor 'Person' takes 0 argument(s) but is here given 1. The required signature is 'new: unit -> Person'.")

    [<Fact>]
    let ``Valid Comma In Rec Ctor``() =
        FSharp """
type Person = { Name : string * bool * bool }
let Age = 22
let City = "London"
let x = { Name = "Isaac", Age = 21, City = "London" }
        """ |> typecheck |> shouldSucceed
