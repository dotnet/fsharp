module FSharp.Compiler.Service.Tests.CompletionQuotationsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``Regression3225.Identifier.InQuotation`` () =
    let info =
        Checker.getCompletionInfo
            """
                let _ = <@ let x = "foo"
                           x.{caret} @>"""

    assertHasItemWithNames [ "Chars"; "Length" ] info

[<Fact>]
let ``ReOpenNameSpace.FsharpQuotation`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                open Microsoft.FSharp.Quotations
                open Microsoft.FSharp.Quotations
                Expr.{caret}
                """

    assertHasItemWithNames [ "Value" ] info

[<Theory>]
[<InlineData(true, "Attributes;CallingConvention;ContainsGenericParameters")>]
[<InlineData(false, "Head;ToInt")>]
let ``Identifier.InActivePattern`` (shouldContain: bool) (names: string) =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                // regression test for bug 3223  No intellisense at point
                open Microsoft.FSharp.Quotations.Patterns
                open Microsoft.FSharp.Quotations.DerivedPatterns
                let test1 = <@ 1 + 1 @>
                let _ =
                    match test1 with
                    | Call(None, methInfo, args) ->
                        if methInfo.{caret}
                """

    let expected = names.Split(';') |> List.ofArray

    assertItemsWithNames shouldContain expected info
