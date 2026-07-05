module FSharp.Compiler.Service.Tests.GotoDefinitionRecordsTests

open System
open Xunit

let private recordSource =
    """
                //regression test for bug 2518
                type MyRec =
                  { myX (*MarkerXFieldDefinition*) : int
                    myY (*MarkerYFieldDefinition*) : int
                  }
                let rDefault =
                  { myX (*MarkerXField*) = 2
                    myY (*MarkerYField*) = 3
                  }
                """

[<Fact>]
let ``Record.Field.Definition`` () =
    assertGoToDefinitionOnLine
        "{ myX (*MarkerXFieldDefinition*) : int"
        (markCaretAfterLeadingIdent recordSource "myX (*MarkerXFieldDefinition*)")

[<Fact>]
let ``Record.Field.Usage`` () =
    assertGoToDefinitionOnLine
        " myY (*MarkerYFieldDefinition*) : int"
        (markCaretAfterLeadingIdent recordSource "myY (*MarkerYField*)")

let private simpleRecordSource =
    String.concat
        "\n"
        [ "type MyRec = (*loc-27*)"
          "  { myX : int (*loc-28*)"
          "    myY : int (*loc-29*)"
          "  }"
          "let rDefault ="
          "  { myX = 2 (*loc-30*)"
          "    myY = 3 (*loc-31*)"
          "  }"
          "let _ = { rDefault with myX = 7 } (*loc-32*)" ]

[<Theory>]
[<InlineData("type MyRec = (*loc-27*)", "MyRec = (*loc-27*)")>]
[<InlineData("{ myX : int (*loc-28*)", "myX : int (*loc-28*)")>]
[<InlineData("myY : int (*loc-29*)", "myY : int (*loc-29*)")>]
[<InlineData("{ myX : int (*loc-28*)", "myX = 2 (*loc-30*)")>]
[<InlineData("myY : int (*loc-29*)", "myY = 3 (*loc-31*)")>]
[<InlineData("{ myX : int (*loc-28*)", "myX = 7 } (*loc-32*)")>]
let ``GotoDefinition.Simple.Datatype.Record`` (definitionLine: string) (marker: string) =
    assertGoToDefinitionOnLine definitionLine (markCaretAfterLeadingIdent simpleRecordSource marker)
