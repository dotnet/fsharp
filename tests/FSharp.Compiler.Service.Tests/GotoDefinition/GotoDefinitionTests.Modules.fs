module FSharp.Compiler.Service.Tests.GotoDefinitionModulesTests

open System
open Xunit

let private moduleDefSource =
    """
                //regression test for bug 2517
                module Foo (*MarkerModuleDefinition*) =
                  let x = ()
                """

[<Fact>]
let ``ModuleDefinition`` () =
    assertGoToDefinitionOnLine
        "module Foo (*MarkerModuleDefinition*) ="
        (markCaretAfterLeadingIdent moduleDefSource "Foo (*MarkerModuleDefinition*)")

let private moduleSource =
    String.concat
        "\n"
        [ "module Too = (*loc-55*)"
          "  let foo = 0 (*loc-56*)"
          "module Bar ="
          "  open Too (*loc-57*)"
          "let _ = Too.foo (*loc-58*)" ]

[<Theory>]
[<InlineData("module Too = (*loc-55*)", "Too = (*loc-55*)")>]
[<InlineData("let foo = 0 (*loc-56*)", "foo = 0 (*loc-56*)")>]
[<InlineData("module Too = (*loc-55*)", "Too.foo (*loc-58*)")>]
[<InlineData("let foo = 0 (*loc-56*)", "foo (*loc-58*)")>]
let ``GotoDefinition.Simple.Module`` (definitionLine: string) (marker: string) =
    assertGoToDefinitionOnLine definitionLine (markCaretAfterLeadingIdent moduleSource marker)

[<Fact>]
let ``GotoDefinition.Simple.Module.Open`` () =
    assertGoToDefinitionOnLine
        "module Too = (*loc-55*)"
        (markCaretAfterLeadingIdent moduleSource "Too (*loc-57*)")

[<Fact>]
let ``ModuleName.OnDefinitionSite.Bug2517`` () =
    let source =
        """
            namespace GotoDefinition
            module Foo(*Mark*) =
            let x = ()"""

    assertGoToDefinitionOnLine "module Foo(*Mark*) =" (markCaretAfterLeadingIdent source "(*Mark*)")
