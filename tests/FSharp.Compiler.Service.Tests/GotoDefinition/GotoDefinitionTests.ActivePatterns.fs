module FSharp.Compiler.Service.Tests.GotoDefinitionActivePatternsTests

open System
open Xunit

let private overlapSource =
    String.concat
        "\n"
        [ "module Overlap ="
          "  type Parity = Even | Odd"
          "  let (|Even{caret1}|Odd|) x = (*loc-59*)"
          "    if x % 0 = 0"
          "       then Even{caret2} (*loc-60*)"
          "       else Odd"
          "  let foo (x : int) ="
          "    match x with"
          "    | Even{caret3} -> 1 (*loc-61*)"
          "    | Odd  -> 0"
          "  let patval = (|Even{caret4}|Odd|) (*loc-61b*)" ]

[<Fact>]
let ``GotoDefinition.Simple.ActivePat`` () =
    overlapSource
    |> assertGoToDefinitionOnLines (List.replicate 4 "let (|Even|Odd|) x = (*loc-59*)")
