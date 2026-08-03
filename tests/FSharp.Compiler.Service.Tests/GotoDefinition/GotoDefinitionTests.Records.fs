module FSharp.Compiler.Service.Tests.GotoDefinitionRecordsTests

open System
open Xunit

let private simpleRecordSource =
    String.concat
        "\n"
        [ "type MyRec{caret1} = (*loc-27*)"
          "  { myX{caret2} : int (*loc-28*)"
          "    myY{caret3} : int (*loc-29*)"
          "  }"
          "let rDefault ="
          "  { myX{caret4} = 2 (*loc-30*)"
          "    myY{caret5} = 3 (*loc-31*)"
          "  }"
          "let _ = { rDefault with myX{caret6} = 7 } (*loc-32*)" ]

[<Fact>]
let ``GotoDefinition.Simple.Datatype.Record`` () =
    simpleRecordSource
    |> assertGoToDefinitionOnLines
        [ "type MyRec = (*loc-27*)"
          "{ myX : int (*loc-28*)"
          "myY : int (*loc-29*)"
          "{ myX : int (*loc-28*)"
          "myY : int (*loc-29*)"
          "{ myX : int (*loc-28*)" ]
