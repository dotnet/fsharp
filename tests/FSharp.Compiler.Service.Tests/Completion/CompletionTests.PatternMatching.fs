module FSharp.Compiler.Service.Tests.CompletionPatternMatchingTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``TupledArgsInLambda.Completion.Bug312557_2`` () =
    """(1,2) |> (fun (aaa,bbb) ->
    printfn "hi"
    printfn "%d%d" b{caret1} a{caret3}
    printfn "%d%d" a{caret2} b{caret4}   ) """
    |> SourceContext.extractOrderedMarkedSources
    |> List.iter (fun source -> assertHasItemWithNames [ "aaa"; "bbb" ] (Checker.getCompletionInfo source))

[<Fact>]
let ``DotCompletionInPatternsPartOfLambda`` () =
    let info = Checker.getCompletionInfo "let _ = fun x .{caret} -> x + 1"
    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``DotCompletionInPatterns`` () =
    let assertEmpty (markedSource: string) =
        let info = Checker.getCompletionInfo markedSource
        Assert.Equal(0, info.Items.Length)

    assertEmpty "let (x, y .{caret}) = 1, 2"
    assertEmpty "let run (o : obj) = match o with | :? int as i .{caret} -> 1 | _ -> 0"
    assertEmpty "let (``x.y``, ``y.z`` .{caret}) = 1, true"
    assertEmpty "let ``x`` .{caret} = 1"

[<Fact>]
let ``MatchStatement.WhenClause.Bug2519`` () =
    let info =
        Checker.getCompletionInfo
            """type DU = X of int
let timefilter pkt =
    match pkt with
    | X(hdr) when (*aaa*)hdr.{caret}
    | _ -> ()"""

    assertHasItemWithNames [ "CompareTo"; "GetHashCode" ] info

[<Fact>]
let ``Bug229433.AfterMismatchedParensCauseWeirdParseTreeAndExceptionDuringTypecheck`` () =
    let info =
        Checker.getCompletionInfo
            """
            type T() =
                member this.Bar() = ()
                member val X = "foo" with get,set
                static member Id(x) = x
            [1]
            |> Seq.iter (fun x ->
                let user = x
                ["foo"]
                |> List.iter (fun m ->
                    let xyz = new T()
                    xyz.X <- null
                    T.Id((*here*)xyz.{caret}  // no intellisense here after .
                    )
                printfn ""
                )  """

    assertHasItemWithNames [ "Bar"; "X" ] info

[<Fact>]
let ``Identifer.InMatchStatement.Bug72595`` () =
    let info =
        Checker.getCompletionInfo
            """
                    type C() =
                        let someValue = "abc"
                        member _.M() =
                          let x = 1
                          match someValue.{caret} with
                            let x = 1
                            match 1 with
                            | _ -> 2
                    type D() =
                        member x.P = 1
                    [<assembly:Microsoft.FSharp.Core.CompilerServices.TypeProviderAssembly("Samples.DataStore.Freebase.DesignTime")>]
                    do()
                """

    assertHasItemWithNames [ "Chars" ] info

[<Theory>]
[<InlineData("""let p4 = 
   let isPalindrome x = 
       let chars = (string_of_int x).ToCharArray()
       let len = chars.{caret}
       chars 
       |> Array.mapi (fun i c ->""")>]
[<InlineData("""let p4 = 
   let isPalindrome x = 
       let chars = (string_of_int x).ToCharArray()
       let len = chars.{caret}
       chars 
       |> Array.mapi (fun i c ->
let p5 = 1""")>]
let ``LambdaExpression.WithoutClosing.Bug1346`` (markedSource: string) =
    let info = Checker.getCompletionInfo markedSource
    assertHasItemWithNames [ "Length" ] info

[<Fact>]
let ``IncompleteStatement.Match_A`` () =
    let info =
        Checker.getCompletionInfo
            """let x = "1"
let test2 = match (x).{caret}"""

    assertHasItemWithNames [ "Contains" ] info

[<Fact>]
let ``IncompleteStatement.Match_C`` () =
    let info =
        Checker.getCompletionInfo
            """let x = "1"
let test2 = match (x).{caret}
let y = 2"""

    assertHasItemWithNames [ "Contains" ] info

[<Fact>]
let ``WithinMatchClause.Bug1603`` () =
    let info =
        Checker.getCompletionInfo
            """let rec f l =
    match l with
    | [] ->
        let xx = System.DateTime.Now
        let y = xx.{caret}
    | x :: xs -> f xs"""

    assertHasItemWithNames [ "AddMilliseconds" ] info

[<Fact>]
let ``MatchStatement.Clause.AfterLetBinds.Bug1603`` () =
    let info =
        Checker.getCompletionInfo
            """let rec f l =
    match l with
    | [] ->
        let xx = System.DateTime.Now
        let y = xx
    | x :: xs -> f xs.{caret}"""

    assertHasItemWithNames [ "Head"; "Tail" ] info

    let headTail =
        info.Items |> Array.filter (fun i -> i.NameInCode = "Head" || i.NameInCode = "Tail")

    if headTail.Length <> 2 then
        failwithf
            "Expected exactly 2 items named Head/Tail but found %d: [%s]"
            headTail.Length
            (headTail |> Array.map _.NameInCode |> String.concat ", ")

    for item in headTail do
        if item.Glyph <> FSharpGlyph.Property then
            failwithf "Item %A has glyph %A but expected Property" item.NameInCode item.Glyph

[<Fact>]
let ``BestMatch.Bug4320a`` () =
    let info = Checker.getCompletionInfo " let x = System.{caret}"
    assertHasItemWithNames [ "GC"; "GCCollectionMode" ] info
    assertPrefixIsNotUnique "G" false info
    assertPrefixIsUnique "GCC" false info

[<Fact>]
let ``BestMatch.Bug4320b`` () =
    let info = Checker.getCompletionInfo " let x = List.{caret}"
    assertHasItemWithNames [ "empty" ] info
    assertPrefixIsNotUnique "e" false info
    assertPrefixIsUnique "em" false info

[<Fact>]
let ``BestMatch.Bug5131`` () =
    let info = Checker.getCompletionInfo "System.Environment.{caret}"
    assertHasItemWithNames [ "OSVersion" ] info
    assertPrefixIsUnique "o" true info

[<Fact>]
let ``Identifier.InMatchStatement`` () =
    let info =
        Checker.getCompletionInfo
            """
let x = 1
match x.{caret} with
    |1 -> 1*1
    |2 -> 2*2
"""

    assertHasItemWithNames [ "ToString"; "Equals" ] info

[<Fact>]
let ``Identifier.InMatchClause`` () =
    let info =
        Checker.getCompletionInfo
            """
let rec f l =
    match l with
    | [] ->
        let xx = System.DateTime.Now
        let y = xx.{caret}
        ()
    | x :: xs -> f xs
"""

    assertHasItemWithNames [ "Add"; "Date" ] info

[<Fact>]
let ``Keywords.Match`` () =
    let info =
        Checker.getCompletionInfo
            """
                match.{caret} a with
                    | pattern -> exp"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Identifier.InMatch.UnderScore`` () =
    let info =
        Checker.getCompletionInfo
            """
                let x = 1
                match x with
                    |1 -> 1*2
                    |2 -> 2*2
                    |_.{caret} -> 0 """

    Assert.Equal(0, info.Items.Length)

[<Fact(Skip = "this no longer works, but I'm unclear why - now you get all the top-level completions")>]
let ``Identifier.InFunctionMatch`` () =
    let info =
        Checker.getCompletionInfo
            """
                let f5 = function
                    | 1.{caret} -> printfn "1"
                    | 2 -> printfn "2" """

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Expression.InMatchWhenClause`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                type DU = X of int
                let timefilter pkt =
                    match pkt with
                    | X(hdr) when hdr.{caret} -> ()
                    | _ -> ()
                """

    assertHasItemWithNames [ "CompareTo"; "ToString" ] info
