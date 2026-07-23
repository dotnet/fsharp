module FSharp.Compiler.Service.Tests.TooltipModulesTests

open System
open Xunit
open FSharp.Compiler.EditorServices

[<Fact>]
let ``ModuleDefinition.ModuleNoNewLines`` () =
    let source =
        """module XXX
type t = C3
module YYY =
    type t = C4
///Doc
module ZZZ =
    type t = C5 """

    assertTooltipContains "module XXX" (markAtEndOfMarker source "XX")
    assertTooltipContainsInOrder [ "module YYY"; "from XXX" ] (markAtEndOfMarker source "YY")
    assertTooltipContainsInOrder [ "module ZZZ"; "from XXX"; "Doc" ] (markAtEndOfMarker source "ZZ")

[<Fact>]
let ``TypeAndModuleReferences`` () =
    let source =
        String.concat
            "\n"
            [ "let test1 = List.length"
              "let test2 = List.Empty"
              "let test3 = (\"1\").Length"
              "let test3b = (id \"1\").Length" ]

    walk source "let test1 = " "List" "module List"
    walk source "let test1 = List." "length" "length"
    walk source "let test2 = " "List" "Collections.List"
    walk source "let test2 = List." "Empty" "List.Empty"
    walk source "let test3 = (\"1\")." "Length" "String.Length"
    walk source "let test3b = (id \"1\")." "Length" "String.Length"

[<Fact>]
let ``ModuleNameAndMisc`` () =
    let source =
        String.concat
            "\n"
            [ "module (*test3q*)MM3 ="
              "    let y = 2"
              "let test4 = lock"
              "let (*test5*) ffff xx = xx + 1" ]

    walk source "module (*test3q*)" "MM3" "module MM3"
    walk source "let test4 = " "lock" "lock"
    walk source "let (*test5*) " "ffff" "ffff"

[<Fact>]
let ``Regression.ModuleAlias.Bug3790a`` () =
    let source =
        """module ``Some`` = Microsoft.FSharp.Collections.List
module None = Microsoft.FSharp.Collections.List"""

    assertTooltipContains "module List" (markAtEndOfMarker source "module ``So")
    assertTooltipContains "module List" (markAtEndOfMarker source "module No")
    assertTooltipDoesNotContain "Option" (markAtEndOfMarker source "module ``So")
    assertTooltipDoesNotContain "Option" (markAtEndOfMarker source "module No")

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_2`` () =
    assertTooltipContainsInOrder
        [ "module Inner"; "from"; "Outer"; "Comment" ]
        """module Outer =
    /// Comment
    module Inner =
        let x = 1

let _ = Outer.Inn{caret}er.x"""

[<Fact>]
let ``Automation.Regression.ModuleIdentifier.Bug2937`` () =
    let source = "module XXX{caret}\ntype t = C3"
    assertTooltipContains "module XXX" source

    for description in groupMainDescriptions (Checker.getTooltip source) do
        if description.Contains "module XXX" && description.Contains "\n" then
            failwithf "Expected the module identifier tooltip to be a single line, but it contained a newline:\n%s" description

[<Fact>]
let ``Automation.Regression.QuotedIdentifier.Bug3790`` () =
    let source =
        String.concat
            "\n"
            [ "module Test"
              "module ``Some``(*Marker1*) = Microsoft.FSharp.Collections.List"
              "let _ = ``Some``(*Marker2*).append [] []" ]

    assertTooltipContains "module List" (markAtStartOfMarker source "``(*Marker1*)")
    assertTooltipDoesNotContain "Option.Some" (markAtStartOfMarker source "``(*Marker1*)")
    assertTooltipContains "module List" (markAtStartOfMarker source "``(*Marker2*)")
    assertTooltipDoesNotContain "Option.Some" (markAtStartOfMarker source "``(*Marker2*)")
