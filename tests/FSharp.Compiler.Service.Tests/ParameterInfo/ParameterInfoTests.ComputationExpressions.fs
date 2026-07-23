module FSharp.Compiler.Service.Tests.ParameterInfoComputationExpressionsTests

open Xunit

[<Fact>]
let ``Regression.InsideWorkflow.6437`` () =
    assertParameterInfoContains ["count: int"] """
open System.IO
let computation2 =
    async { use file = File.Open("",FileMode.Open)
            let! buffer = file.AsyncRead({caret}0)
            return 0 }"""

[<Fact>]
let ``Regression.ParameterFirstTypeOpenParen.Bug90798`` () =
    assertParameterInfoOverloads [ ["'Arg -> Async<'T>"] ] """
let a = async {
        Async.AsBeginEnd({caret}
    }
let p = 10"""
