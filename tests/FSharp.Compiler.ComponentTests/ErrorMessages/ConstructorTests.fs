// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

module ``Constructor`` =

    [<Fact>]
    let ``Invalid Record``() =
        CompilerAssert.TypeCheckWithErrors
            """
type Record = {field1:int; field2:int}
let doSomething (xs) = List.map (fun {field1=x} -> x) xs
doSomething {Record.field1=0; field2=0}
            """
            [|
                FSharpErrorSeverity.Error, 1, (4, 13, 4, 40), "This expression was expected to have type\n    'Record list'    \nbut here has type\n    'Record'    "
                FSharpErrorSeverity.Warning, 20, (4, 1, 4, 40), "The result of this expression has type 'int list' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'."
            |]

    [<Fact>]
    let ``Comma In Rec Ctor``() =
        CompilerAssert.TypeCheckWithErrors
            """
type Person = { Name : string; Age : int; City : string }
let x = { Name = "Isaac", Age = 21, City = "London" }
            """
            [|
                FSharpErrorSeverity.Error, 1, (3, 18, 3, 52), "This expression was expected to have type\n    'string'    \nbut here has type\n    ''a * 'b * 'c'    " + System.Environment.NewLine + "A ';' is used to separate field values in records. Consider replacing ',' with ';'."
                FSharpErrorSeverity.Error, 764, (3, 9, 3, 54), "No assignment given for field 'Age' of type 'Test.Person'"
            |]

    [<Fact>]
    let ``Missing Comma In Ctor``() =
        CompilerAssert.TypeCheckWithErrors
            """
type Person() =
    member val Name = "" with get,set
    member val Age = 0 with get,set

let p =
    Person(Name = "Fred"
           Age = 18)
            """
            [|
                FSharpErrorSeverity.Error, 39, (7, 12, 7, 16), "The value or constructor 'Name' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   nan"
                FSharpErrorSeverity.Warning, 20, (7, 12, 7, 25), "The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'."
                FSharpErrorSeverity.Error, 39, (8, 12, 8, 15), "The value or constructor 'Age' is not defined."
                FSharpErrorSeverity.Error, 501, (7, 5, 8, 21), "The object constructor 'Person' takes 0 argument(s) but is here given 1. The required signature is 'new : unit -> Person'. If some of the arguments are meant to assign values to properties, consider separating those arguments with a comma (',')."
            |]

    [<Fact>]
    let ``Missing Ctor Value``() =
        CompilerAssert.TypeCheckSingleError
            """
type Person(x:int) =
    member val Name = "" with get,set
    member val Age = x with get,set

let p =
    Person(Name = "Fred",
           Age = 18)
            """
            FSharpErrorSeverity.Error
            496
            (7, 5, 8, 21)
            "The member or object constructor 'Person' requires 1 argument(s). The required signature is 'new : x:int -> Person'."

    [<Fact>]
    let ``Extra Argument In Ctor``() =
        CompilerAssert.TypeCheckSingleError
            """
type Person() =
    member val Name = "" with get,set
    member val Age = 0 with get,set

let p =
    Person(1)
            """
            FSharpErrorSeverity.Error
            501
            (7, 5, 7, 14)
            "The object constructor 'Person' takes 0 argument(s) but is here given 1. The required signature is 'new : unit -> Person'."

    [<Fact>]
    let ``Extra Argument In Ctor2``() =
        CompilerAssert.TypeCheckSingleError
            """
type Person() =
    member val Name = "" with get,set
    member val Age = 0 with get,set

let b = 1

let p =
    Person(1=b)
            """
            FSharpErrorSeverity.Error
            501
            (9, 5, 9, 16)
            "The object constructor 'Person' takes 0 argument(s) but is here given 1. The required signature is 'new : unit -> Person'."

    [<Fact>]
    let ``Valid Comma In Rec Ctor``() =
        CompilerAssert.Pass
            """
type Person = { Name : string * bool * bool }
let Age = 22
let City = "London"
let x = { Name = "Isaac", Age = 21, City = "London" }
            """
