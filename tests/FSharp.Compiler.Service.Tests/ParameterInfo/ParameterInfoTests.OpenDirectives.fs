module FSharp.Compiler.Service.Tests.ParameterInfoOpenDirectivesTests

open Xunit

[<Fact>]
let ``Single.Constructor2`` () =
    assertHasParameterInfo """
open System
new DateTime({caret}"""

[<Fact>]
let ``Regression.NoParameterInfoTriggeredByOpenBrace.Bug3878`` () =
    assertParameterInfoContains ["value: string"] """
module ParameterInfo
let x = 1 + 2

let _ = System.Console.WriteLin{caret}e ()

let y = 1"""

[<Fact(Skip = "Bug 95862")>]
let ``BasicBehavior.WithReference`` () =
    assertParameterInfoContains ["System.Type"; "System.Uri []"] """
open System.ServiceModel
let serviceHost = new ServiceHost({caret})"""
