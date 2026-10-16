module FSharp.Compiler.Service.Tests.ParameterInfoIndexingSlicingTests

open Xunit

[<Fact(Skip = "FSharp1.0:5245")>]
let ``Single.DotNet.IndexerParameter`` () =
    assertParameterInfoOverloads [ ["index: int"] ] """
let alist = System.Collections.ArrayList(2)
alist.[{caret}0] |> ignore"""

[<Fact>]
let ``LocationOfParams.UnmatchedParens.Bug91609.OtherCases.Open`` () =
    assertHasParameterInfo """
let arr = Array.create 4 1
arr.[1] <- System.Int32.Parse({caret}
open System"""

[<Fact>]
let ``LocationOfParams.UnmatchedParens.Bug91609.OtherCases.Module`` () =
    assertHasParameterInfo """
let arr = Array.create 4 1
arr.[1] <- System.Int32.Parse({caret}
module Foo =
    let x = 42"""

[<Fact>]
let ``LocationOfParams.UnmatchedParens.Bug91609.OtherCases.Namespace`` () =
    assertHasParameterInfo """
namespace Foo
module Bar =
    let arr = Array.create 4 1
    arr.[1] <- System.Int32.Parse({caret}
namespace Other"""
