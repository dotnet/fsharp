module FSharp.Compiler.Service.Tests.GotoDefinitionMiscTests

open Xunit

let private nestedLetRecSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let     x = ()"
          "  let rec x = (*loc-9*)"
          "    fun y -> (*loc-10*)"
          "      x{caret} y"
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
          "  let f (_ as foo{caret1}) = (*loc-35*)"
          "    foo{caret2}"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.AsPat`` () =
    asPatternSource
    |> assertGoToDefinitionOnLines (List.replicate 2 "let f (_ as foo) = (*loc-35*)")

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
          "let a1 = Astruct{caret1}(0)"
          "let b1 = Astruct{caret2}(0, 1)"
          "let c1 = Astruct{caret3}()"
          "let a2 = AS{caret4}(0)"
          "let b2 = AS{caret5}(0, 1)"
          "let c2 = AS{caret6}()" ]

[<Fact>]
let ``GotoDefinition.ObjectOriented.StructConstructor`` () =
    structConstructorSource
    |> assertGoToDefinitionOnLines
        [ "new(a) = Astruct(a, a)"
          "type Astruct(x:int, y:int) ="
          "type Astruct(x:int, y:int) ="
          "new(a) = Astruct(a, a)"
          "type Astruct(x:int, y:int) ="
          "type Astruct(x:int, y:int) =" ]

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
            UnitSymbols.A{caret}"""

    assertGoToDefinitionToExternalLine "type A = ampere" source
