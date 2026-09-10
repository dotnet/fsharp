module FSharp.Compiler.Service.Tests.ParameterInfoSeqListArrayExprsTests

open Xunit

[<Fact(Skip = "FSharp1.0:2394")>]
let ``Single.DotNet.ParameterArray`` () =
    assertParameterInfoOverloads
        [ ["format"; "args"]
          ["format"; "arg0"]
          ["provider"; "format"; "args"]
          ["format"; "arg0"; "arg1"]
          ["format"; "arg0"; "arg1"; "arg2"] ] """
let x = "a"
System.String.Format("[{0}] for [{1}]", x.ToUpperInvariant(){caret}, x)"""

[<Fact>]
let ``ParameterInfo.LocationOfParams.Bug112688`` () =
    assertNoParameterInfo """
let f x y = ()
module MailboxProcessorBasicTests =
    do f 0
         0
    {caret}let zz = 42
    for timeout in [0; 10] do
      ()"""

[<Fact>]
let ``Multi.Function.AsParameter`` () =
    assertParameterInfoOverloads [ ["int list"] ] """
let isLessThanZero x = (x < 0)
let containsNegativeNumbers intList =
    let filteredList = List.filter isLessThanZero intList
    if List.length filteredList > 0
    then Some(filteredList)
    else None
let _ = Option.get(containsNegativeNumber{caret}s [6; 20; 8; 45; 5])"""
