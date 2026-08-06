module FSharp.Compiler.Service.Tests.GotoDefinitionDiscriminatedUnionsTests

open Xunit

let private discUnionSource =
    """
                type DiscUnion =
                    | Alpha of string
                    | Beta of decimal * unit
                    | Gamma

                let valueX = Beta{caret2}(1.0M, ())(*GotoTypeDef*)
                let valueY = valueX{caret1}
                """

[<Fact>]
let ``GotoDefinition.DiscriminatedUnion`` () =
    discUnionSource
    |> assertGoToDefinitionOnLines
        [ "let valueX = Beta(1.0M, ())(*GotoTypeDef*)"
          "| Beta of decimal * unit" ]

let private simpleDatatypeSource =
    String.concat
        "\n"
        [ "type Zero = (*loc-13*)"
          "let foo (_ : Zero{caret1}) : 'a = failwith \"hi\""
          "type One{caret3} = (*loc-16*)"
          "  One{caret2} (*loc-15*)"
          "let f (x : One{caret5}) ="
          "  One{caret4}"
          "type Nat{caret6} = (*loc-19*)"
          "  | Suc of Nat{caret7} (*loc-20*)"
          "  | Zro (*loc-21*)"
          "let rec plus m n = (*loc-23*)"
          "  match m with (*loc-22*)"
          "  | Zro{caret8}   ->"
          "      n"
          "  | Suc{caret9} m -> (*loc-25*)"
          "      Suc (plus m{caret10} n{caret11})" ]

[<Fact>]
let ``GotoDefinition.Simple.Datatype`` () =
    simpleDatatypeSource
    |> assertGoToDefinitionOnLines
        [ "type Zero = (*loc-13*)"
          "One (*loc-15*)"
          "type One = (*loc-16*)"
          "One (*loc-15*)"
          "type One = (*loc-16*)"
          "type Nat = (*loc-19*)"
          "type Nat = (*loc-19*)"
          "| Zro (*loc-21*)"
          "| Suc of Nat (*loc-20*)"
          "| Suc m -> (*loc-25*)"
          "let rec plus m n = (*loc-23*)" ]
