module FSharp.Compiler.Service.Tests.CompletionPrintfFormatTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``TupledArgsInLambda.Completion.Bug312557_1`` () =
    let info =
        Checker.getCompletionInfo
            """[(1,2);(1,2);(1,2)]
|> Seq.iter (fun (xxx,yyy) -> printfn "%d" {caret}
                              printfn "%d" 1)"""

    assertHasItemWithNames [ "xxx"; "yyy" ] info

[<Fact>]
let ``CtrlSpaceInWhiteSpace.Bug133112`` () =
    let info =
        Checker.getCompletionInfo
            """
            type Foo =
                static member A = 1
                static member B = 2
            printfn "%d %d" Foo.A {caret} """

    assertHasItemWithNames [ "AbstractClassAttribute" ] info
    assertHasNoItemsWithNames [ "A"; "B" ] info

[<Fact>]
let ``BY_DESIGN.ExplicitlyCloseTheParens.Bug73940`` () =
    let info =
        Checker.getCompletionInfo
            """
                    let g lam =
                        lam true |> printfn "%b"
                        sprintf "%s"
                    let r =
                        ["1"]
                           |> List.map (fun s -> s.{caret}  )   // user types close paren here to avoid paren mismatch
                           |> g     // regardless of whatever is down here now, it won't affect the type of 's' above
                """

    assertHasItemWithNames [ "Chars" ] info

[<Fact>]
let ``BY_DESIGN.MismatchedParenthesesAreHardToRecoverFromAndHereIsWhy.Bug73940`` () =
    let info =
        Checker.getCompletionInfo
            """
                    let g lam =
                        lam true |> printfn "%b"
                        sprintf "%s"
                    let r =
                        ["1"]
                           |> List.map (fun s -> s.{caret}   // it looks like s is a string here, but it's not!
                           |> g     // parser recovers as though there is a right-paren here
                """

    assertHasItemWithNames [ "CompareTo" ] info
    assertHasNoItemsWithNames [ "Chars" ] info

[<Fact>]
let ``Identifier.AfterParenthesis.Bug6484_2`` () =
    let info =
        Checker.getCompletionInfo
            """for x = 1 to 10 do
    printfn "%s" (x.{caret} """

    assertHasItemWithNames [ "CompareTo" ] info
