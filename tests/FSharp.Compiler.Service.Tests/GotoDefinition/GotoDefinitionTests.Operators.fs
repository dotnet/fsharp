module FSharp.Compiler.Service.Tests.GotoDefinitionOperatorsTests

open System
open Xunit

let private isOperatorChar c =
    not (isIdentChar c) && not (Char.IsWhiteSpace c)

let private markAfterOperator (source: string) (marker: string) =
    match source.IndexOf(marker, StringComparison.Ordinal) with
    | -1 -> failwithf "Marker %A not found in source" marker
    | i ->
        let mutable j = i

        while j < source.Length && isOperatorChar source.[j] do
            j <- j + 1

        source.Insert(j, "{caret}")

[<Fact>]
let ``Operators.TopLevel`` () =
    let source =
        """
                let (===) a b = a = b
                let _ = 1 === 2
                """

    assertGoToDefinitionOperatorOnLine "let (===) a b = a = b" "===" (markAfterOperator source "=== 2")

[<Fact>]
let ``Operators.Member`` () =
    let source =
        """
                type U = U
                    with
                    static member (+++) (U, U) = U
                let _ = U +++ U
                """

    assertGoToDefinitionOperatorOnLine "static member (+++) (U, U) = U" "+++" (markAfterOperator source "++ U")

let private simpleOperatorSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let (+) x _ = x (*loc-12*)"
          "  2 + 3 (*loc-11*)" ]

[<Fact(Skip = "Bug 2514 filed.")>]
let ``GotoDefinition.Simple.Binding.Operator`` () =
    assertGoToDefinitionOperatorOnLine
        "let (+) x _ = x (*loc-2*)"
        "+"
        (markAfterOperator simpleOperatorSource "+ 3 (*loc-11*)")
