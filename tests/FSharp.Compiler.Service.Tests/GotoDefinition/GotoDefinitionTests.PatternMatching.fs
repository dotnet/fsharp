module FSharp.Compiler.Service.Tests.GotoDefinitionPatternMatchingTests

open System
open Xunit

let private nestedLetSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let     x = ()"
          "  let rec x = (*loc-9*)"
          "    fun y -> (*loc-10*)"
          "      x y (*loc-8*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Binding.NestedLetWithXRecParam`` () =
    assertGoToDefinitionOnLine
        "fun y -> (*loc-10*)"
        (markCaretAfterLeadingIdent nestedLetSource "y (*loc-8*)")

let private lambdaMultiBindSource =
    String.concat
        "\n"
        [ "let _ ="
          "  fun x (*loc-37*)"
          "      x -> (*loc-38*)"
          "    x (*loc-39*)" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.LambdaMultBind2`` () =
    assertGoToDefinitionOnLine
        "x -> (*loc-38*)"
        (markCaretAfterLeadingIdent lambdaMultiBindSource "x -> (*loc-38*)")

[<Fact>]
let ``GotoDefinition.Simple.Tricky.LambdaMultBindBody`` () =
    assertGoToDefinitionOnLine
        "x -> (*loc-38*)"
        (markCaretAfterLeadingIdent lambdaMultiBindSource "x (*loc-39*)")

let private functionPatternSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let f = () (*loc-40*)"
          "  let f = (*loc-41*)"
          "    function f -> (*loc-42*)"
          "      f (*loc-43*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.LotsOfFsPat`` () =
    assertGoToDefinitionOnLine
        "function f -> (*loc-42*)"
        (markCaretAfterLeadingIdent functionPatternSource "f -> (*loc-42*)")

[<Fact>]
let ``GotoDefinition.Simple.Tricky.LotsOfFsUse`` () =
    assertGoToDefinitionOnLine
        "function f -> (*loc-42*)"
        (markCaretAfterLeadingIdent functionPatternSource "f (*loc-43*)")

let private andPatternSource =
    String.concat
        "\n"
        [ "type Nat = Suc of Nat | Zro"
          "let _ ="
          "  let f x ="
          "    match x with"
          "    | Suc y & z -> (*loc-47*)"
          "        y (*loc-46*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.AndPat`` () =
    assertGoToDefinitionOnLine
        "| Suc y & z -> (*loc-47*)"
        (markCaretAfterLeadingIdent andPatternSource "y (*loc-46*)")

let private consPatternSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let f xs ="
          "    match xs with"
          "    | x :: xs -> (*loc-49*)"
          "        x (*loc-48*)"
          "    | _       -> []"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.ConsPat`` () =
    assertGoToDefinitionOnLine
        "| x :: xs -> (*loc-49*)"
        (markCaretAfterLeadingIdent consPatternSource "x (*loc-48*)")

let private pairPatternSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let f x ="
          "    match x with"
          "    | (y : int, z) -> (*loc-51*)"
          "         y (*loc-50*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.PairPat`` () =
    assertGoToDefinitionOnLine
        "| (y : int, z) -> (*loc-51*)"
        (markCaretAfterLeadingIdent pairPatternSource "y (*loc-50*)")

let private consWhenSource =
    String.concat
        "\n"
        [ "let _ ="
          "  let f xs ="
          "    match xs with"
          "    | x :: xs (*loc-54*)"
          "      when xs <> [] -> (*loc-52*)"
          "        x :: xs (*loc-53*)"
          "  ()" ]

[<Fact>]
let ``GotoDefinition.Simple.Tricky.ConsPatWhenClauseInWhen`` () =
    assertGoToDefinitionOnLine
        "| x :: xs (*loc-54*)"
        (markCaretAfterLeadingIdent consWhenSource "xs <> [] -> (*loc-52*)")
