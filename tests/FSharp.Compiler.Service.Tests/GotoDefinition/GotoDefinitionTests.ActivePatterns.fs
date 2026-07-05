module FSharp.Compiler.Service.Tests.GotoDefinitionActivePatternsTests

open System
open Xunit

let private overlapSource =
    String.concat
        "\n"
        [ "module Overlap ="
          "  type Parity = Even | Odd"
          "  let (|Even|Odd|) x = (*loc-59*)"
          "    if x % 0 = 0"
          "       then Even (*loc-60*)"
          "       else Odd"
          "  let foo (x : int) ="
          "    match x with"
          "    | Even -> 1 (*loc-61*)"
          "    | Odd  -> 0"
          "  let patval = (|Even|Odd|) (*loc-61b*)" ]

[<Theory>]
[<InlineData("Even|Odd|) x = (*loc-59*)")>]
[<InlineData("Even (*loc-60*)")>]
[<InlineData("Even -> 1 (*loc-61*)")>]
[<InlineData("en|Odd|) (*loc-61b*)")>]
let ``GotoDefinition.Simple.ActivePat`` (marker: string) =
    assertGoToDefinitionOnLine
        "let (|Even|Odd|) x = (*loc-59*)"
        (markCaretAfterLeadingIdent overlapSource marker)
