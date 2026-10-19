module FSharp.Compiler.Service.Tests.CompletionIndexingSlicingTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Theory>]
[<InlineData(".[]")>]
[<InlineData(".[]<-")>]
[<InlineData(".[,]<-")>]
[<InlineData(".[,,]<-")>]
[<InlineData(".[,,,]<-")>]
[<InlineData(".[,,,]")>]
[<InlineData(".[,,]")>]
[<InlineData(".[,]")>]
[<InlineData(".[..]")>]
[<InlineData(".[..,..]")>]
[<InlineData(".[..,..,..]")>]
[<InlineData(".[..,..,..,..]")>]
let ``AdjacentToDot.Positive`` (op: string) =
    let info = Checker.getCompletionInfo (markAtEndOfMarker ("System.Console" + op) "System.Console.")
    assertHasItemWithNames [ "BackgroundColor" ] info

[<Theory>]
[<InlineData(".[]")>]
[<InlineData(".[]<-")>]
[<InlineData(".[,]<-")>]
[<InlineData(".[,,]<-")>]
[<InlineData(".[,,,]<-")>]
[<InlineData(".[,,,]")>]
[<InlineData(".[,,]")>]
[<InlineData(".[,]")>]
[<InlineData(".[..]")>]
[<InlineData(".[..,..]")>]
[<InlineData(".[..,..,..]")>]
[<InlineData(".[..,..,..,..]")>]
let ``AdjacentToDot.Negative`` (op: string) =
    let info = Checker.getCompletionInfo ("System.Console" + op + "{caret}")
    assertHasItemWithNames [ "abs" ] info
    assertHasNoItemsWithNames [ "BackgroundColor" ] info

[<Fact>]
let ``DotOff.Parenthesized.Expr`` () =
    let info =
        Checker.getCompletionInfo
            """let string_of_int (x:int) = x.ToString()
let strs = Array.init 10 string_of_int
let x = (strs.[1]).{caret}"""

    assertHasItemWithNames [ "Substring"; "GetHashCode" ] info

[<Fact>]
let ``DotOff.ArrayIndexerNotation`` () =
    let info =
        Checker.getCompletionInfo
            """let string_of_int (x:int) = x.ToString()
let strs = Array.init 10 string_of_int
let test1 = strs.[1].{caret}"""

    assertHasItemWithNames [ "Substring"; "GetHashCode" ] info

[<Fact>]
let ``DotOff.ArraySliceNotation`` () =
    """let string_of_int (x:int) = x.ToString()
let strs = Array.init 10 string_of_int
let test2 = strs.[1..].{caret1}
let test3 = strs.[..1].{caret2}
let test4 = strs.[1..1].{caret3}"""
    |> SourceContext.extractOrderedMarkedSources
    |> List.iter (fun source -> assertHasItemWithNames [ "Length" ] (Checker.getCompletionInfo source))

[<Fact>]
let ``DotOff.DictionaryIndexer`` () =
    let info =
        Checker.getCompletionInfo
            """let dict = new System.Collections.Generic.Dictionary<int,string>()
let test5 = dict.[1].{caret}"""

    assertHasItemWithNames [ "Length" ] info

[<Fact>]
let ``Identifier.FuzzyDefined.Bug67133`` () =
    let info =
        Checker.getCompletionInfo
            """let gDateTime (arr: System.DateTime[]) =
    arr.[0].{caret}"""

    assertHasItemWithNames [ "AddDays" ] info

[<Fact>]
let ``Identifier.FuzzyDefined.Bug67133.Negative`` () =
    let info =
        Checker.getCompletionInfo
            """let gDateTime (arr: DateTime[]) =
    arr.[0].{caret}"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Type.Indexers.Bug4898_1`` () =
    let info =
        Checker.getCompletionInfo
            """type Foo(len) =
    member this.Value = [1 .. len]
type Bar =
    static member ParamProp with get len = new Foo(len)
let n = Bar.ParamProp.{caret}"""

    assertHasItemWithNames [ "ToString" ] info
    assertHasNoItemsWithNames [ "Value" ] info

[<Fact>]
let ``Type.Indexers.Bug4898_2`` () =
    let info =
        Checker.getCompletionInfo
            """type mytype() =
    let instanceArray2 = [|[| "A"; "B" |]; [| "A"; "B" |] |]
    let instanceArray = [| "A"; "B" |]
    member x.InstanceIndexer
        with get(idx) = instanceArray.[idx]
    member x.InstanceIndexer2
        with get(idx1,idx2) = instanceArray2.[idx1].[idx2]
let a = mytype()
a.InstanceIndexer2.{caret}"""

    assertHasItemWithNames [ "ToString" ] info
    assertHasNoItemsWithNames [ "Chars" ] info

[<Fact>]
let ``Expression.ListItem`` () =
    let info =
        Checker.getCompletionInfo
            """
                let a = [1;2;3]
                a.[1].{caret}
                """

    assertHasItemWithNames [ "CompareTo"; "ToString" ] info

[<Fact>]
let ``Expression.2DArray`` () =
    let info =
        Checker.getCompletionInfo
            """
                let (a2: int[,]) = Array2.zero_create 10 10
                a2.[1,2].{caret}
                """

    assertHasItemWithNames [ "ToString" ] info

[<Theory>]
[<InlineData("Chars;Split", true)>]
[<InlineData("IsReadOnly;Rank", false)>]
let ``Expression.ArrayItem`` (names: string, shouldContain: bool) =
    let info =
        Checker.getCompletionInfo
            """
                //regression test for bug 1001
                let str1 = Array.init 10 string
                str1.[1].{caret}"""

    let names = names.Split(';') |> List.ofArray

    assertItemsWithNames shouldContain names info

[<Fact(Skip = "Re-enable this test --- https://github.com/dotnet/fsharp/issues/5238")>]
let ``Identifier.In#Statement`` () =
    let info =
        Checker.getCompletionInfo
            """
                # 29 "original-test-file.fs"
                let argv = System.Environment.GetCommandLineArgs()
                let SetCulture() =
                  if argv.{caret}Length > 2 && argv.[1] = "--culture" then
                    let cultureString = argv.[2]
                """

    assertHasItemWithNames [ "Length"; "Clone"; "ToString" ] info
