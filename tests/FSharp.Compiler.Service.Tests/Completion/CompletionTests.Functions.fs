module FSharp.Compiler.Service.Tests.CompletionFunctionsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``DotAfterApplication1`` () =
    let info =
        Checker.getCompletionInfo
            """let g a = new System.Random()
(g []).{caret}"""

    assertHasItemWithNames [ "Next" ] info

[<Fact>]
let ``DotAfterApplication2`` () =
    let info =
        Checker.getCompletionInfo
            """let g a = new System.Random()
g [].{caret}"""

    assertHasItemWithNames [ "Head" ] info

[<Fact(Skip = "Re-enable this test --- https://github.com/dotnet/fsharp/issues/5238")>]
let ``CurriedArguments.Regression1`` () =
    let info =
        Checker.getCompletionInfo
            """let f{caret}ffff x y = 1
let ggggg  = 1
let test1 = fffff "a" ggggg
let test2 = fffff 1 ggggg
let test3 = fffff ggggg ggggg"""

    assertHasItemWithNames [ "fffff" ] info

[<Theory>]
[<InlineData("""let fffff x y = 1
let ggggg  = 1
let test1 = f{caret}ffff "a" ggggg
let test2 = fffff 1 ggggg
let test3 = fffff ggggg ggggg""", "fffff")>]
[<InlineData("""let fffff x y = 1
let ggggg  = 1
let test1 = fffff "a" gg{caret}ggg
let test2 = fffff 1 ggggg
let test3 = fffff ggggg ggggg""", "ggggg")>]
[<InlineData("""let fffff x y = 1
let ggggg  = 1
let test1 = fffff "a" ggggg
let test2 = fffff 1 gg{caret}ggg
let test3 = fffff ggggg ggggg""", "ggggg")>]
[<InlineData("""let fffff x y = 1
let ggggg  = 1
let test1 = fffff "a" ggggg
let test2 = fffff 1 ggggg
let test3 = fffff gg{caret}ggg ggggg""", "ggggg")>]
[<InlineData("""let fffff x y = 1
let ggggg  = 1
let test1 = fffff "a" ggggg
let test2 = fffff 1 ggggg
let test3 = fffff ggggg gg{caret}ggg""", "ggggg")>]
let ``CurriedArguments.Regression`` (markedSource: string) (expected: string) =
    let info = Checker.getCompletionInfo markedSource

    assertHasItemWithNames [ expected ] info

[<Fact>]
let ``StringFunctions`` () =
    let info =
        Checker.getCompletionInfo
            """let y = String.{caret}
let f x = 0"""

    assertHasItemWithNames [ "collect"; "concat"; "exists" ] info

    for item in info.Items do
        Assert.Equal(FSharpGlyph.Method, item.Glyph)

[<Fact>]
let ``NotShowInfo.FunctionParameter.Bug3602`` () =
    let info =
        Checker.getCompletionInfo
            """let foo s.{caret} = s + "Hello world"
                            ()"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``IncompleteIfClause.Bug4594`` () =
    let info =
        Checker.getCompletionInfo
            """let Bar(xyz) =
  let hello = 
    if x{caret}"""

    assertHasItemWithNames [ "xyz" ] info

[<Fact>]
let ``ListFunctions`` () =
    let info =
        Checker.getCompletionInfo
            """let y = List.{caret}
let f x = 0"""

    assertHasItemWithNames [ "map"; "filter"; "fold" ] info

    for item in info.Items do
        match item.NameInCode, item.Glyph with
        | "Cons", FSharpGlyph.Method -> ()
        | "Empty", FSharpGlyph.Property -> ()
        | "empty", _ -> ()
        | _, FSharpGlyph.Method -> ()
        | name, glyph -> Assert.Fail(sprintf "Unexpected item %s with glyph %A" name glyph)

[<Fact>]
let ``Expression.Function`` () =
    let info =
        Checker.getCompletionInfo
            """
                let func(mm) = 100
                func(x + y).{caret}
                """

    assertHasItemWithNames [ "CompareTo"; "ToString" ] info

[<Theory>]
[<InlineData("DayOfWeek", true)>]
[<InlineData("Chars", false)>]
let ``RedefinedIdentifier.DiffScope.InScope`` (item: string) (shouldBePresent: bool) =
    let info =
        Checker.getCompletionInfo
            """
                let identifierBothScope = ""
                let functionScope () =
                    let identifierBothScope = System.DateTime.Now
                    identifierBothScope.{caret}
                identifierBothScope(*MarkerShowLastOneWhenOutscoped*)"""

    if shouldBePresent then
        assertHasItemWithNames [ item ] info
    else
        assertHasNoItemsWithNames [ item ] info

[<Fact>]
let ``RedefinedIdentifier.DiffScope.OutScope.Positive`` () =
    let info =
        Checker.getCompletionInfo
            """
                let identifierBothScope = ""
                let functionScope () =
                    let identifierBothScope = System.DateTime.Now
                    identifierBothScope(*MarkerShowLastOneWhenInScope*)
                identifierBothScope.{caret}"""

    assertHasItemWithNames [ "Chars" ] info

[<Fact>]
let ``Identifier.AsFunctionName.InInitial`` () =
    let info =
        Checker.getCompletionInfo
            """let f2.{caret} x = x+1 """

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Identifier.AsParameter.InInitial`` () =
    let info =
        Checker.getCompletionInfo
            """ let f3 x.{caret} = x+1"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Basic.Completion.UnfinishedLet`` () =
    let info =
        Checker.getCompletionInfo
            """
                let g(x) = x+1
                let f() =
                    let r = g(4).{caret} """

    assertHasItemWithNames [ "CompareTo" ] info

[<Fact(Skip = "VS bug 65730 (aspirational): member completion on a generic un-annotated parameter `x.` is not yet offered by FCS; assertion preserved (Equals present)")>]
let ``AutoComplete.Bug65730`` () =
    let info =
        Checker.getCompletionInfo
            """let f x y = x.{caret}Equals(y)"""

    assertHasItemWithNames [ "Equals" ] info

[<Fact(Skip = "disabled upstream; exclusion assertion preserved")>]
let ``AutoComplete.Bug72596_B`` () =
    let info =
        Checker.getCompletionInfo
            """let f() =
    let foo = fo{caret}"""

    assertHasNoItemsWithNames [ "foo" ] info
