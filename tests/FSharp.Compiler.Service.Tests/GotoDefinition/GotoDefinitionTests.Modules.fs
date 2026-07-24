module FSharp.Compiler.Service.Tests.GotoDefinitionModulesTests

open System
open Xunit

let private moduleDefSource =
    """
                //regression test for bug 2517
                module Foo{caret} (*MarkerModuleDefinition*) =
                  let x = ()
                """

[<Fact>]
let ``ModuleDefinition`` () =
    assertGoToDefinitionOnLine
        "module Foo (*MarkerModuleDefinition*) ="
        moduleDefSource

let private moduleSource =
    String.concat
        "\n"
        [ "module Too{caret1} = (*loc-55*)"
          "  let foo{caret2} = 0 (*loc-56*)"
          "module Bar ="
          "  open Too{caret5} (*loc-57*)"
          "let _ = Too{caret3}.foo{caret4} (*loc-58*)" ]

[<Fact>]
let ``GotoDefinition.Simple.Module`` () =
    moduleSource
    |> assertGoToDefinitionOnLines
        [ "module Too = (*loc-55*)"
          "let foo = 0 (*loc-56*)"
          "module Too = (*loc-55*)"
          "let foo = 0 (*loc-56*)"
          "module Too = (*loc-55*)" ]

[<Fact>]
let ``ModuleName.OnDefinitionSite.Bug2517`` () =
    let source =
        """
            namespace GotoDefinition
            module Foo{caret}(*Mark*) =
            let x = ()"""

    assertGoToDefinitionOnLine "module Foo(*Mark*) =" source
