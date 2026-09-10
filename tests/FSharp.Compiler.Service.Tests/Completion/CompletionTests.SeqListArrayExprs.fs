module FSharp.Compiler.Service.Tests.CompletionSeqListArrayExprsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``Array.Length.InForRange`` () =
    let info =
        Checker.getCompletionInfo
            """
let a = [|1;2;3|]
for i in 0..a.{caret}"""

    assertHasItemWithNames [ "Length" ] info

[<Fact>]
let ``Identifier.Array.AfterassertKeyword`` () =
    let info =
        Checker.getCompletionInfo
            """
let x = [1;2;3] 
assert x.{caret}"""

    assertHasItemWithNames [ "Head" ] info
    assertHasNoItemsWithNames [ "Listeners" ] info

[<Fact>]
let ``CtrlSpaceCompletion.Bug294974.Case2`` () =
    let info =
        Checker.getCompletionInfo
            """
              let xxx {caret}= [1]
              xxx .IsEmpty // Ctrl-J just before the '.' """

    assertHasItemWithNames [ "AbstractClassAttribute" ] info
    assertHasNoItemsWithNames [ "IsEmpty" ] info

[<Theory>]
[<InlineData("Popup")>]
[<InlineData("CtrlSpace")>]
let ``PopupsVersusCtrlSpaceOnDotDot.FirstDot`` (_trigger: string) =
    let info = Checker.getCompletionInfo "System.Console.{caret}.BackgroundColor"

    assertHasItemWithNames [ "BackgroundColor" ] info
    assertHasNoItemsWithNames [ "abs" ] info

[<Fact>]
let ``Identifier.OnWhiteSpace.AtTopLevel`` () =
    let info = Checker.getCompletionInfo "(*marker*) {caret} "

    assertHasItemWithNames [ "System"; "Array2D" ] info
    assertHasNoItemsWithNames [ "Int32" ] info

[<Fact>]
let ``Identifier.AfterDefined.Bug1545`` () =
    let info =
        Checker.getCompletionInfo
            """
let x = [|"hello"|]
x.{caret}"""

    assertHasItemWithNames [ "Length" ] info

[<Fact>]
let ``Residues1`` () =
    let info = Checker.getCompletionInfo "System   .   Int32   .   M{caret}"

    assertHasItemWithNames [ "MaxValue"; "MinValue" ] info
    assertHasNoItemsWithNames [ "MailboxProcessor"; "Map" ] info

[<Fact>]
let ``BY_DESIGN.CommonScenarioThatBegsTheQuestion.Bug73940`` () =
    let info =
        Checker.getCompletionInfo
            """
                    let r =
                        ["1"]
                           |> List.map (fun s -> s.{caret}     // user previous had e.g. '(fun s -> s)' here, but he erased after 's' to end-of-line and hit '.' e.g. to eventually type '.Substring(5))'
                           |> List.filter (fun s -> s.Length > 5)  // parser recover assumes close paren is here, and type inference goes wacky-useless with such a parse
                """

    assertHasNoItemsWithNames [ "Chars" ] info

[<Fact>]
let ``Identifier.AfterParenthesis.Bug835276`` () =
    let info =
        Checker.getCompletionInfo
            """
let f ( s : string ) =
   let x = 10 + s.Length
   for i in 1..10 do
     let ok = 10 + s.Length // dot here did work
     let y = 10 +(s.{caret}"""

    assertHasItemWithNames [ "Length" ] info

[<Fact>]
let ``Identifier.AfterParenthesis.Bug6484_1`` () =
    let info =
        Checker.getCompletionInfo
            """
for x in 1..10 do
    printfn "%s" (x.{caret} """

    assertHasItemWithNames [ "CompareTo" ] info

[<Fact>]
let ``Array`` () =
    let info = Checker.getCompletionInfo "let arr = [| for i in 1..10 -> i |].{caret}"

    assertHasItemWithNames [ "Clone"; "IsFixedSize" ] info

[<Fact>]
let ``List`` () =
    let info = Checker.getCompletionInfo "let lst = [ for i in 1..10 -> i].{caret}"

    assertHasItemWithNames [ "Head"; "Tail" ] info

[<Fact>]
let ``Expression.List`` () =
    let info = Checker.getCompletionInfo "[1;2].{caret}   "

    assertHasItemWithNames [ "Head"; "Item" ] info

[<Fact>]
let ``Array.InitialUsing..`` () =
    let info = Checker.getCompletionInfo "let x1 = [| 0.0 .. 0.1 .. 10.0 |].{caret}"

    assertHasItemWithNames [ "Length"; "Clone"; "ToString" ] info

[<Fact>]
let ``BadCompletionAfterQuicklyTyping`` () =
    let info = Checker.getCompletionInfo "[1].{caret}"

    assertHasItemWithNames [ "Length" ] info
    assertHasNoItemsWithNames [ "AbstractClassAttribute" ] info
