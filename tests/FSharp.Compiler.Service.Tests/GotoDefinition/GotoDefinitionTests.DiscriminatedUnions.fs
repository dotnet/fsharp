module FSharp.Compiler.Service.Tests.GotoDefinitionDiscriminatedUnionsTests

open System
open Xunit

let private discUnionSource =
    """
                type DiscUnion =
                    | Alpha of string
                    | Beta of decimal * unit
                    | Gamma

                let valueX = Beta(1.0M, ())(*GotoTypeDef*)
                let valueY = valueX (*GotoValDef*)
                """

[<Fact>]
let ``Value`` () =
    assertGoToDefinitionOnLine
        "let valueX = Beta(1.0M, ())(*GotoTypeDef*)"
        (markCaretAfterLeadingIdent discUnionSource "valueX (*GotoValDef*)")

[<Fact>]
let ``DisUnionMember`` () =
    assertGoToDefinitionOnLine
        "| Beta of decimal * unit"
        (markCaretAfterLeadingIdent discUnionSource "Beta(1.0M, ())(*GotoTypeDef*)")

let private simpleDatatypeSource =
    String.concat
        "\n"
        [ "type Zero = (*loc-13*)"
          "let foo (_ : Zero) : 'a = failwith \"hi\" (*loc-14*)"
          "type One = (*loc-16*)"
          "  One (*loc-15*)"
          "let f (x : One) = (*loc-17*)"
          "  One (*loc-18*)"
          "type Nat = (*loc-19*)"
          "  | Suc of Nat (*loc-20*)"
          "  | Zro (*loc-21*)"
          "let rec plus m n = (*loc-23*)"
          "  match m with (*loc-22*)"
          "  | Zro   -> (*loc-24*)"
          "      n"
          "  | Suc m -> (*loc-25*)"
          "      Suc (plus m n) (*loc-26*)" ]

[<Theory>]
[<InlineData("type Zero = (*loc-13*)", "Zero) : 'a = failwith \"hi\" (*loc-14*)")>]
[<InlineData("One (*loc-15*)", "One (*loc-15*)")>]
[<InlineData("type One = (*loc-16*)", "One = (*loc-16*)")>]
[<InlineData("One (*loc-15*)", "One (*loc-18*)")>]
[<InlineData("type One = (*loc-16*)", "One) = (*loc-17*)")>]
[<InlineData("type Nat = (*loc-19*)", "Nat = (*loc-19*)")>]
[<InlineData("type Nat = (*loc-19*)", "Nat (*loc-20*)")>]
[<InlineData("| Zro (*loc-21*)", "Zro   -> (*loc-24*)")>]
[<InlineData("| Suc of Nat (*loc-20*)", "Suc m -> (*loc-25*)")>]
[<InlineData("| Suc m -> (*loc-25*)", "m n) (*loc-26*)")>]
[<InlineData("let rec plus m n = (*loc-23*)", "n) (*loc-26*)")>]
let ``GotoDefinition.Simple.Datatype`` (definitionLine: string) (marker: string) =
    assertGoToDefinitionOnLine definitionLine (markCaretAfterLeadingIdent simpleDatatypeSource marker)
