module FSharp.Compiler.Service.Tests.CompletionQueriesTests

open Xunit

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.CompletionInJoinOn`` () =
    let info =
        Checker.getCompletionInfo
            """
query {
    for a in [1] do
    join b in [2] on (a.{caret})
    select (a + b)
}"""

    assertHasItemWithNames [ "GetHashCode"; "CompareTo" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.GroupJoin.CompletionInIncorrectJoinRelations`` () =
    let info =
        Checker.getCompletionInfo
            """
let t =
    query {
        for x in [1] do
        groupJoin y in [""] on (x.{caret} ?=? y.) into g
        select 1  }"""

    assertHasItemWithNames [ "CompareTo" ] info
    assertHasNoItemsWithNames [ "abs" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.Join.CompletionInIncorrectJoinRelations`` () =
    let info =
        Checker.getCompletionInfo
            """
let t =
    query {
        for x in [1] do
        join y in [""] on (x.{caret} ?=? y.)
        select 1  }"""

    assertHasItemWithNames [ "CompareTo" ] info
    assertHasNoItemsWithNames [ "abs" ] info

[<Fact(Skip = "Re-enable this test --- https://github.com/dotnet/fsharp/issues/5238")>]
let ``Query.ForKeywordCanCompleteIntoIdentifier`` () =
    let info =
        Checker.getCompletionInfo
            """
let form = 42
let t =
    query {
        for{caret}
    }"""

    assertHasItemWithNames [ "form" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.CtrlSpaceSmokeTest0`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = si{caret}"""

    assertHasItemWithNames [ "sin" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.CtrlSpaceSmokeTest0b`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = qu{caret}"""

    assertHasItemWithNames [ "query" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.CtrlSpaceSmokeTest1`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = query { for x in [1;2;3] do sel{caret}"""

    assertHasItemWithNames [ "select" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.CtrlSpaceSmokeTest1b`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = query { for x in [1;2;3] do {caret}"""

    assertHasItemWithNames [ "select" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.CtrlSpaceSmokeTest2`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = query { for x in [1;2;3] do sel{caret} }"""

    assertHasItemWithNames [ "select" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.CtrlSpaceSmokeTest3`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = query { for xxxxxx in [1;2;3] do xxx{caret}"""

    assertHasItemWithNames [ "xxxxxx" ] info

[<Theory>]
[<InlineData("""
module BasicTest
let x = seq { for xxxxxx in [1;2;3] do xxx{caret}""")>]
[<InlineData("""
module BasicTest
let x = async { for xxxxxx in [1;2;3] do xxx{caret}""")>]
let ``QueryExpression.CtrlSpaceSmokeTest3b_3c`` (markedSource: string) =
    let info = Checker.getCompletionInfo markedSource

    assertHasItemWithNames [ "xxxxxx" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.CtrlSpaceSystematic1`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = query { for x in [1;2;3] do sel{caret}"""

    assertHasItemWithNames [ "select" ] info

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``QueryExpressions.QueryAndSequenceExpressionWithForYieldLoopSystematic`` () =
    let info =
        Checker.getCompletionInfo
            """
module Test
let aaaaaa = [| "1" |]
let v = query { for bbbb in [ aaaaaa ] do yield {caret}"""

    assertHasItemWithNames [ "aaaaaa"; "bbbb" ] info

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``QueryAndOtherExpressions.WordByWordSystematicJoinQueryOnSingleLine`` () =
    let info =
        Checker.getCompletionInfo
            """
module Test
let abbbbc = [| 1 |]
let aaaaaa = 0
let x = query { for bbbb in abbbbc do join cccc in abbb{caret}"""

    assertHasItemWithNames [ "abbbbc" ] info

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``QueryAndOtherExpressions.WordByWordSystematicJoinQueryOnMultipleLine`` () =
    let info =
        Checker.getCompletionInfo
            """
module Test
let abbbbc = [| 1 |]
let aaaaaa = 0
let x = query { for bbbb in abbbbc do
                join cccc in abbb{caret}"""

    assertHasItemWithNames [ "abbbbc" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.CtrlSpaceSystematic2`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = query { for x in [1;2;3] do {caret}"""

    assertHasItemWithNames [ "select"; "where" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.Auto.InNestedQuery`` () =
    let info =
        Checker.getCompletionInfo
            """
let tuples = [ (1, 8, 9); (56, 45, 3)]
let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]
let foo =
    query {
        for n in numbers do
        let maxNumber = query {for x in tuples do ma{caret}}
        select n }"""

    assertHasItemWithNames [ "maxBy"; "maxByNullable" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.Auto.OffSetFromPreviousLine`` () =
    let info =
        Checker.getCompletionInfo
            """
let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]
let foo =
    query {
        for n in numbers do
            gro{caret}
      }"""

    assertHasItemWithNames [ "groupBy"; "groupJoin"; "groupValBy" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.DotCompletionSmokeTest1`` () =
    let info =
        Checker.getCompletionInfo
            """
module Basic
let x2 = query { for x in ["1";"2";"3"] do
                 select x.{caret}"""

    assertHasItemWithNames [ "Chars"; "Length" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.DotCompletionSmokeTest2`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = query { for x in ["1";"2";"3"] do select x.{caret}"""

    assertHasItemWithNames [ "Chars"; "Length" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.DotCompletionSmokeTest0`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = seq { for x in ["1";"2";"3"] do yield x.{caret} }"""

    assertHasItemWithNames [ "Chars"; "Length" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.DotCompletionSmokeTest3`` () =
    let info =
        Checker.getCompletionInfo
            """
module BasicTest
let x = query { for x in ["1";"2";"3"] do select x.{caret} }"""

    assertHasItemWithNames [ "Chars"; "Length" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.DotCompletionSystematic1`` () =
    let info =
        Checker.getCompletionInfo
            """
module Simple
let x2 = query { for x in ["1";"2";"3"] do
                 select x.{caret}"""

    assertHasItemWithNames [ "Chars"; "Length" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QueryExpression.InsideJoin.Bug204147`` () =
    let info =
        Checker.getCompletionInfo
            """
module Simple
type T() =
     member x.GetCollection() = [1;2;3;4]
let q =
    query {
       for e in [1..10] do
       join b in T().{caret}
       select b
    }"""

    assertHasItemWithNames [ "GetCollection" ] info

[<Fact(Skip = "196230")>]
let ``Query.HasErrors.Bug196230`` () =
    let info =
        Checker.getCompletionInfo
            """
open DataSource
let products = Products.getProductList()
let sortedProducts =
    query {
        for p in products do
        let x = p.ProductID + "a"
        sortBy p.{caret}
        select p
    }"""

    assertHasItemWithNames [ "ProductID"; "ProductName" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.HasErrors2`` () =
    let info =
        Checker.getCompletionInfo
            """
open DataSource
let products = Products.getProductList()
let sortedProducts =
    query {
        for p in products do
        orderBy (p.{caret})
    }"""

    assertHasItemWithNames [ "ProductID"; "ProductName" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.ShadowedVariables`` () =
    let info =
        Checker.getCompletionInfo
            """
open DataSource
let products = Products.getProductList()
let p = 12
let sortedProducts =
    query {
        for p in products do
        select p.{caret}
    }"""

    assertHasItemWithNames [ "Category"; "ProductName" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.InNestedQuery`` () =
    let info =
        Checker.getCompletionInfo
            """
let tuples = [ (1, 8, 9); (56, 45, 3)]
let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]
let foo =
    query {
        for n in numbers do
        let maxNumber = query {for x in tuples do maxBy x.{caret}}
        select (n, query {for y in numbers do minBy y}) }"""

    assertHasItemWithNames [ "Equals"; "GetType" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``Query.NestedExpressionWithinLamda`` () =
    let info =
        Checker.getCompletionInfo
            """
let numbers = [ 1;2; 8; 9; 15; 23; 3; 42; 4;0; 55;]
let f (x : string) = ()
let foo =
    query {
        for n in numbers do
        let x = 42 |> ignore; numbers |> List.iter( fun n -> f ("1" + "1").{caret})
        skipWhile (n < 30)
         }"""

    assertHasItemWithNames [ "Chars"; "Length" ] info
