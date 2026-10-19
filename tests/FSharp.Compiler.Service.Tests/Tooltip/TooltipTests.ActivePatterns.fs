module FSharp.Compiler.Service.Tests.TooltipActivePatternsTests

open Xunit

let private lazyActivePatternSource =
    """let (|Lazy|) x = x
                             match 0 with | Lazy y -> ()"""

[<Fact>]
let ``ActivePatterns.Declaration`` () =
    assertTooltipContains "int -> Choice" (markAtEndOfMarker "let ( |One|Two| ) x = One(x+1)" "ne|Tw")

[<Fact>]
let ``ActivePatterns.Result`` () =
    assertTooltipContains "active pattern result One: int -> Choice" (markAtEndOfMarker "let ( |One|Two| ) x = One(x+1)" "= On")

[<Fact>]
let ``ActivePatterns.Value`` () =
    let source =
        """let ( |One|Two| ) x = One(x+1)
             let patval = (|One|Two|) // use"""

    assertTooltipContains "int -> Choice" (markAtEndOfMarker source "= (|On")

[<Fact>]
let ``Regression.ActivePatterns.Bug4100a`` () =
    assertTooltipDoesNotContain "'?" (markAtEndOfMarker lazyActivePatternSource "with | Laz")
    assertTooltipContains "Lazy" (markAtEndOfMarker lazyActivePatternSource "with | Laz")

[<Fact>]
let ``Regression.ActivePatterns.Bug4100b`` () =
    let source =
        """let Some (a:int) = a
match None with
| Some _ -> ()
| _ -> ()

let (|NSome|) (a:int) = a
let NSome (a:int) = a.ToString()
match 0 with
| NSome _ -> ()"""

    assertTooltipDoesNotContain "int -> int" (markAtEndOfMarker source "| Som")
    assertTooltipContains "Option.Some" (markAtEndOfMarker source "| Som")
    assertTooltipDoesNotContain "int -> string" (markAtEndOfMarker source "| NSom")
    assertTooltipContains "active recognizer NSome" (markAtEndOfMarker source "| NSom")

[<Fact>]
let ``Regression.ActivePatterns.Bug4103`` () =
    let marked = markAtEndOfMarker lazyActivePatternSource "(|Laz"
    assertTooltipDoesNotContain "Control.Lazy" marked
    assertTooltipContains "|Lazy|" marked

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_5`` () =
    assertCompletionItemTooltipContainsInOrder
        "Pattern"
        [ "active recognizer Pattern: int"; "Pattern comment" ]
        """module Module =
    /// Pattern comment
    let (|Pattern|) = 0

let x() =
    Module.{caret}"""
