module FSharp.Compiler.Service.Tests.GotoDefinitionLetBindingsTests

open System
open Xunit

[<Fact>]
let ``PrimitiveType`` () =
    let source =
        """
                // Can't goto def on an int literal
                let bi = 123456I{caret}"""

    assertGoToDefinitionFails source

[<Fact>]
let ``GotoDefinition.NoIdentifierAtLocation`` () =
    let markedSources =
        [ "let x = 1{caret}"
          "let x = 1{caret}.2"
          "let x = \"12{caret}3\"" ]

    for markedSource in markedSources do
        assertGoToDefinitionFails markedSource

let private trivialLetSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x{caret2} = () (*loc-2*)"
          "  x{caret1} (*loc-1*)" ]

[<Fact>]
let ``GotoDefinition.Simple.Binding.TrivialLet`` () =
    trivialLetSource
    |> assertGoToDefinitionOnLines (List.replicate 2 "let x = () (*loc-2*)")

let private nestedSameNameSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x{caret3} = () (*loc-5*)"
          "  let x{caret2} = () (*loc-3*)"
          "  x{caret1} (*loc-4*)" ]

[<Fact>]
let ``GotoDefinition.Simple.Binding.NestedLetWithSameName`` () =
    nestedSameNameSource
    |> assertGoToDefinitionOnLines
        [ "let x = () (*loc-3*)"
          "let x = () (*loc-3*)"
          "let x = () (*loc-5*)" ]

let private nestedXIsXSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let x = () (*loc-7*)"
          "  let x ="
          "    x{caret} (*loc-6*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Binding.NestedLetWithXIsX`` () =
    assertGoToDefinitionOnLine
        "let x = () (*loc-7*)"
        nestedXIsXSource

let private lotsOfFsFuncSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let f = () (*loc-40*)"
          "  let f{caret} = (*loc-41*)"
          "    function f -> (*loc-42*)"
          "      f (*loc-43*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.LotsOfFsFunc`` () =
    assertGoToDefinitionOnLine
        "let f = (*loc-41*)"
        lotsOfFsFuncSource
