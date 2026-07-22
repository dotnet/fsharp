module FSharp.Compiler.Service.Tests.GotoDefinitionMiscTests

open System
open Xunit

let private nestedLetRecSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let     x = ()"
          "  let rec x = (*loc-9*)"
          "    fun y -> (*loc-10*)"
          "      x{caret} y (*loc-8*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Binding.NestedLetWithXRec`` () =
    assertGoToDefinitionOnLine
        "let rec x = (*loc-9*)"
        nestedLetRecSource

let private asPatternSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let foo          = ()"
          "  let f (_ as foo) = (*loc-35*)"
          "    foo (*loc-36*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.AsPatLHS`` () =
    assertGoToDefinitionOnLine
        "let f (_ as foo) = (*loc-35*)"
        (markCaretAfterLeadingIdent asPatternSource "foo) = (*loc-35*)")

[<Fact>]
let ``GotoDefinition.Simple.Tricky.AsPatRHS`` () =
    assertGoToDefinitionOnLine
        "let f (_ as foo) = (*loc-35*)"
        (markCaretAfterLeadingIdent asPatternSource "foo (*loc-36*)")

let private lambdaMultiBindSource =
    String.concat
        "\n"
        [ "let _ ="
          "  fun x{caret} (*loc-37*)"
          "      x -> (*loc-38*)"
          "    x (*loc-39*)" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.LambdaMultBind1`` () =
    assertGoToDefinitionOnLine
        "fun x (*loc-37*)"
        lambdaMultiBindSource

let private quotedKeywordSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let rec ``let{caret}`` = (*loc-74*)"
          "    function 0 -> 1"
          "           | n -> n * ``let`` (n - 1) (*loc-75*)" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.QuotedKeyword`` () =
    assertGoToDefinitionOnLine
        "let rec ``let`` = (*loc-74*)"
        quotedKeywordSource

let private structConstructorSource =
    String.concat
        "\n"
        [ ""
          "[<Struct>]"
          "type Astruct(x:int, y:int) ="
          "  [<DefaultValue()>]"
          "  val mutable a : int"
          "  new(a) = Astruct(a, a)"
          "type AS = Astruct"
          "let a1 = Astruct(0)"
          "let b1 = Astruct(0, 1)"
          "let c1 = Astruct()"
          "let a2 = AS(0)"
          "let b2 = AS(0, 1)"
          "let c2 = AS()" ]

[<Fact>]
let ``GotoDefinition.ObjectOriented.StructConstructor`` () =
    assertGoToDefinitionOnLine
        "new(a) = Astruct(a, a)"
        (markCaretAfterLeadingIdent structConstructorSource "Astruct(0)")

    assertGoToDefinitionOnLine
        "type Astruct(x:int, y:int) ="
        (markCaretAfterLeadingIdent structConstructorSource "Astruct(0, 1)")

    assertGoToDefinitionOnLine
        "type Astruct(x:int, y:int) ="
        (markCaretAfterLeadingIdent structConstructorSource "Astruct()")

    assertGoToDefinitionOnLine
        "new(a) = Astruct(a, a)"
        (markCaretAfterLeadingIdent structConstructorSource "AS(0)")

    assertGoToDefinitionOnLine
        "type Astruct(x:int, y:int) ="
        (markCaretAfterLeadingIdent structConstructorSource "AS(0, 1)")

    assertGoToDefinitionOnLine
        "type Astruct(x:int, y:int) ="
        (markCaretAfterLeadingIdent structConstructorSource "AS()")

[<Fact>]
let ``GotoDefinition.Abbreviation.Bug193064`` () =
    let source =
        """
            type X = int
            let f (x:X) = x{caret}(*Marker*) """

    assertGoToDefinitionOnLine "let f (x:X) = x(*Marker*)" source

[<Fact>]
let ``GotoDefinition.UnitOfMeasure.Bug193064`` () =
    let source =
        """
            open Microsoft.FSharp.Data.UnitSystems.SI
            UnitSymbols.A{caret}(*Marker*)"""

    assertGoToDefinitionToExternalLine "type A = ampere" source
