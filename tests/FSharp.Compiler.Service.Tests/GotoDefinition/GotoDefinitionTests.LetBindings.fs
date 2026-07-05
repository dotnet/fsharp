module FSharp.Compiler.Service.Tests.GotoDefinitionLetBindingsTests

open System
open Xunit

[<Fact>]
let ``PrimitiveType`` () =
    let source =
        """
                // Can't goto def on an int literal
                let bi = 123456I"""

    assertGoToDefinitionFails (markCaretAfterLeadingIdent source "123456I")

[<Fact>]
let ``GotoDefinition.NoIdentifierAtLocation`` () =
    let useCases =
        [ "let x = 1", "1"
          "let x = 1.2", ".2"
          "let x = \"123\"", "2" ]

    for source, marker in useCases do
        assertGoToDefinitionFails (markCaretAfterLeadingIdent source marker)

let private trivialLetSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x = () (*loc-2*)"
          "  x (*loc-1*)" ]

[<Fact>]
let ``GotoDefinition.Simple.Binding.TrivialLetRHSToRight`` () =
    assertGoToDefinitionOnLine
        "let x = () (*loc-2*)"
        (markCaretAfterLeadingIdent trivialLetSource " (*loc-1*)")

[<Fact>]
let ``GotoDefinition.Simple.Binding.TrivialLetLHS`` () =
    assertGoToDefinitionOnLine
        "let x = () (*loc-2*)"
        (markCaretAfterLeadingIdent trivialLetSource "x = () (*loc-2*)")

let private nestedSameNameSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x = () (*loc-5*)"
          "  let x = () (*loc-3*)"
          "  x (*loc-4*)" ]

[<Theory>]
[<InlineData("let x = () (*loc-3*)", "x (*loc-4*)")>]
[<InlineData("let x = () (*loc-3*)", "x = () (*loc-3*)")>]
[<InlineData("let x = () (*loc-5*)", "x = () (*loc-5*)")>]
let ``GotoDefinition.Simple.Binding.NestedLetWithSameName`` (definitionLine: string) (marker: string) =
    assertGoToDefinitionOnLine definitionLine (markCaretAfterLeadingIdent nestedSameNameSource marker)

let private nestedXIsXSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x = () (*loc-7*)"
          "  let x ="
          "    x (*loc-6*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Binding.NestedLetWithXIsX`` () =
    assertGoToDefinitionOnLine
        "let x = () (*loc-7*)"
        (markCaretAfterLeadingIdent nestedXIsXSource "x (*loc-6*)")

let private lotsOfFsFuncSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let f = () (*loc-40*)"
          "  let f = (*loc-41*)"
          "    function f -> (*loc-42*)"
          "      f (*loc-43*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.LotsOfFsFunc`` () =
    assertGoToDefinitionOnLine
        "let f = (*loc-41*)"
        (markCaretAfterLeadingIdent lotsOfFsFuncSource "f = (*loc-41*)")
