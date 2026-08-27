module FSharp.Compiler.Service.Tests.ParameterInfoTypeExtensionsTests

open Xunit

[<Fact>]
let ``ExtensionMethod.Overloads`` () =
    assertParameterInfoOverloads [ ["a: string"]; ["a: int"] ] """
module MyCode =
    type A() =
        member this.Method(a:string) = ""
module MyExtension =
    type MyCode.A with
        member this.Method(a:int) = ""

open MyCode
open MyExtension
let foo = A()
foo.Method({caret}"""

[<Fact(Skip = "Parameterinfo not retrieved properly for indexed properties by test infra")>]
let ``ExtensionProperty.Overloads`` () =
    assertParameterInfoOverloads [ ["string"]; ["int"] ] """
module MyCode =
    type A() =
        member this.Prop with get(a:string) = ""
module MyExtension =
    type MyCode.A with
        member this.Prop with get(a:int) = ""

open MyCode
open MyExtension
let foo = A()
foo.Prop({caret}"""
