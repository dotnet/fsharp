module FSharp.Compiler.Service.Tests.GotoDefinitionOperatorsTests

open Xunit

[<Fact>]
let ``Operators.TopLevel`` () =
    let source =
        """
                let (===) a b = a = b
                let _ = 1 ==={caret} 2
                """

    assertGoToDefinitionOperatorOnLine "let (===) a b = a = b" "===" source

[<Fact>]
let ``Operators.Member`` () =
    let source =
        """
                type U = U
                    with
                    static member (+++) (U, U) = U
                let _ = U +++{caret} U
                """

    assertGoToDefinitionOperatorOnLine "static member (+++) (U, U) = U" "+++" source

let private simpleOperatorSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let (+) x _ = x (*loc-12*)"
          "  2 +{caret} 3 (*loc-11*)" ]

[<Fact(Skip = "Bug 2514 filed.")>]
let ``GotoDefinition.Simple.Binding.Operator`` () =
    assertGoToDefinitionOperatorOnLine
        "let (+) x _ = x (*loc-2*)"
        "+"
        simpleOperatorSource
