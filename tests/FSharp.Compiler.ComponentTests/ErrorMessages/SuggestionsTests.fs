// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

module Suggestions =

    [<Fact>]
    let ``Field Suggestion`` () =
        CompilerAssert.TypeCheckSingleError
            """
type Person = { Name : string; }

let x = { Person.Names = "Isaac" }
            """
            FSharpErrorSeverity.Error
            39
            (4, 18, 4, 23)
            ("The type 'Person' does not define the field, constructor or member 'Names'. Maybe you want one of the following:" + System.Environment.NewLine + "   Name")


    [<Fact>]
    let ``Suggest Array Module Functions`` () =
        CompilerAssert.TypeCheckSingleError
            """
let f =
    Array.blt
            """
            FSharpErrorSeverity.Error
            39
            (3, 11, 3, 14)
            ("The value, constructor, namespace or type 'blt' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   blit")


    [<Fact>]
    let ``Suggest Async Module`` () =
        CompilerAssert.TypeCheckSingleError
            """
let f =
    Asnc.Sleep 1000
            """
            FSharpErrorSeverity.Error
            39
            (3, 5, 3, 9)
            ("The value, namespace, type or module 'Asnc' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   Async" + System.Environment.NewLine + "   async" + System.Environment.NewLine + "   asin" + System.Environment.NewLine + "   snd")


    [<Fact>]
    let ``Suggest Attribute`` () =
        CompilerAssert.TypeCheckSingleError
            """
[<AbstractClas>]
type MyClass<'Bar>() =
   abstract M<'T> : 'T -> 'T
   abstract M2<'T> : 'T -> 'Bar
            """
            FSharpErrorSeverity.Error
            39
            (2, 3, 2, 15)
            ("The type 'AbstractClas' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   AbstractClass" + System.Environment.NewLine + "   AbstractClassAttribute")


    [<Fact>]
    let ``Suggest Double Backtick Identifiers`` () =
        CompilerAssert.TypeCheckSingleError
            """
module N =
    let ``longer name`` = "hallo"

let x = N.``longe name``
            """
            FSharpErrorSeverity.Error
            39
            (5, 11, 5, 25)
            ("The value, constructor, namespace or type 'longe name' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   longer name")


    [<Fact>]
    let ``Suggest Double Backtick Unions`` () =
        CompilerAssert.TypeCheckSingleError
            """
module N =
    type MyUnion =
    | ``My Case1``
    | Case2

open N

let x = N.MyUnion.``My Case2``
            """
            FSharpErrorSeverity.Error
            39
            (9, 19, 9,31)
            ("The type 'MyUnion' does not define the field, constructor or member 'My Case2'. Maybe you want one of the following:" + System.Environment.NewLine + "   My Case1" + System.Environment.NewLine + "   Case2")


    [<Fact>]
    let ``Suggest Fields In Constructor`` () =
        CompilerAssert.TypeCheckSingleError
            """
type MyClass() =
    member val MyProperty = "" with get, set
    member val MyProperty2 = "" with get, set
    member val ABigProperty = "" with get, set

let c = MyClass(Property = "")
            """
            FSharpErrorSeverity.Error
            495
            (7, 17, 7, 25)
            ("The object constructor 'MyClass' has no argument or settable return property 'Property'. The required signature is new : unit -> MyClass. Maybe you want one of the following:" + System.Environment.NewLine + "   MyProperty" + System.Environment.NewLine + "   MyProperty2" + System.Environment.NewLine + "   ABigProperty")


    [<Fact>]
    let ``Suggest Generic Type`` () =
        CompilerAssert.TypeCheckSingleError
            """
type T = System.Collections.Generic.Dictionary<int11,int>
            """
            FSharpErrorSeverity.Error
            39
            (2, 48, 2, 53)
            ("The type 'int11' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   int16" + System.Environment.NewLine + "   int16`1" + System.Environment.NewLine + "   int8" + System.Environment.NewLine + "   uint16" + System.Environment.NewLine + "   int")


    [<Fact>]
    let ``Suggest Methods`` () =
        CompilerAssert.TypeCheckSingleError
            """
module Test2 =
    type D() =

       static let x = 1

       member x.Method1() = 10

    D.Method2()
            """
            FSharpErrorSeverity.Error
            39
            (9, 7, 9, 14)
            ("The type 'D' does not define the field, constructor or member 'Method2'. Maybe you want one of the following:" + System.Environment.NewLine + "   Method1")


    [<Fact>]
    let ``Suggest Modules`` () =
        CompilerAssert.TypeCheckSingleError
            """
module Collections =

    let f () = printfn "%s" "Hello"

open Collectons
            """
            FSharpErrorSeverity.Error
            39
            (6, 6, 6, 16)
            ("The namespace or module 'Collectons' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   Collections")


    [<Fact>]
    let ``Suggest Namespaces`` () =
        CompilerAssert.TypeCheckSingleError
            """
open System.Collectons
            """
            FSharpErrorSeverity.Error
            39
            (2, 13, 2, 23)
            "The namespace 'Collectons' is not defined."


    [<Fact>]
    let ``Suggest Record Labels`` () =
        CompilerAssert.TypeCheckSingleError
            """
type MyRecord = { Hello: int; World: bool}

let r = { Hello = 2 ; World = true}

let x = r.ello
            """
            FSharpErrorSeverity.Error
            39
            (6, 11, 6, 15)
            ("The type 'MyRecord' does not define the field, constructor or member 'ello'. Maybe you want one of the following:" + System.Environment.NewLine + "   Hello")


    [<Fact>]
    let ``Suggest Record Type for RequireQualifiedAccess Records`` () =
        CompilerAssert.TypeCheckSingleError
            """
[<RequireQualifiedAccess>]
type MyRecord = {
    Field1: string
    Field2: int
}

let r = { Field1 = "hallo"; Field2 = 1 }
            """
            FSharpErrorSeverity.Error
            39
            (8, 11, 8, 17)
            ("The record label 'Field1' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   MyRecord.Field1")


    [<Fact>]
    let ``Suggest To Use Indexer`` () =
        CompilerAssert.TypeCheckWithErrors
            """
let d = [1,1] |> dict
let y = d[1]

let z = d[|1|]

let f() = d
let a = (f())[1]
            """
            [|
                FSharpErrorSeverity.Error, 3217, (3, 9, 3, 10), "This value is not a function and cannot be applied. Did you intend to access the indexer via d.[index] instead?"
                FSharpErrorSeverity.Error, 3, (5, 9, 5, 10), "This value is not a function and cannot be applied."
                FSharpErrorSeverity.Error, 3217, (8, 10, 8, 13), "This expression is not a function and cannot be applied. Did you intend to access the indexer via expr.[index] instead?"
            |]


    [<Fact>]
    let ``Suggest Type Parameters`` () =
        CompilerAssert.TypeCheckSingleError
            """
[<AbstractClass>]
type MyClass<'Bar>() =
   abstract M<'T> : 'T -> 'T
   abstract M2<'T> : 'T -> 'Bar
   abstract M3<'T> : 'T -> 'B
            """
            FSharpErrorSeverity.Error
            39
            (6, 28, 6, 30)
            "The type parameter 'B is not defined."


    [<Fact>]
    let ``Suggest Types in Module`` () =
        CompilerAssert.TypeCheckSingleError
            """
let x : System.Collections.Generic.Lst = ResizeArray()
            """
            FSharpErrorSeverity.Error
            39
            (2, 36, 2, 39)
            ("The type 'Lst' is not defined in 'System.Collections.Generic'. Maybe you want one of the following:" + System.Environment.NewLine + "   List" + System.Environment.NewLine + "   IList" + System.Environment.NewLine + "   List`1")

    [<Fact>]
    let ``Suggest Types in Namespace`` () =
        CompilerAssert.TypeCheckSingleError
            """
let x = System.DateTie.MaxValue
            """
            FSharpErrorSeverity.Error
            39
            (2, 16, 2, 23)
            ("The value, constructor, namespace or type 'DateTie' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   DateTime" + System.Environment.NewLine + "   DateTimeKind" + System.Environment.NewLine + "   DateTimeOffset" + System.Environment.NewLine + "   Data")


    [<Fact>]
    let ``Suggest Union Cases`` () =
        CompilerAssert.TypeCheckSingleError
            """
type MyUnion =
| ASimpleCase
| AnotherCase of int

let u = MyUnion.AntherCase
            """
            FSharpErrorSeverity.Error
            39
            (6, 17, 6, 27)
            ("The type 'MyUnion' does not define the field, constructor or member 'AntherCase'. Maybe you want one of the following:" + System.Environment.NewLine + "   AnotherCase")


    [<Fact>]
    let ``Suggest Union Type for RequireQualifiedAccess Unions`` () =
        CompilerAssert.TypeCheckSingleError
            """
[<RequireQualifiedAccess>]
type MyUnion =
| MyCase1
| MyCase2 of string

let x : MyUnion = MyCase1
            """
            FSharpErrorSeverity.Error
            39
            (7, 19, 7, 26)
            ("The value or constructor 'MyCase1' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   MyUnion.MyCase1")


    [<Fact>]
    let ``Suggest Unions in PatternMatch`` () =
        CompilerAssert.TypeCheckSingleError
            """
[<RequireQualifiedAccess>]
type MyUnion =
| Case1
| Case2

let y = MyUnion.Case1

let x =
    match y with
    | MyUnion.Cas1 -> 1
    | _ -> 2
            """
            FSharpErrorSeverity.Error
            39
            (11, 15, 11, 19)
            ("The type 'MyUnion' does not define the field, constructor or member 'Cas1'. Maybe you want one of the following:" + System.Environment.NewLine + "   Case1" + System.Environment.NewLine + "   Case2")
