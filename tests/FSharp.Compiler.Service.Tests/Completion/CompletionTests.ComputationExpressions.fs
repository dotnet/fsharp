module FSharp.Compiler.Service.Tests.CompletionComputationExpressionsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``AsyncExpression.CtrlSpaceSmokeTest3d`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                let x = async { for xxxxxx in [1;2;3] do xxx{caret} }"""

    assertHasItemWithNames [ "xxxxxx" ] info

[<Fact>]
let ``SequenceExpressions.SequenceExprWithWhileLoopSystematic`` () =
    let prefix = "\nmodule Test\nlet abbbbc = [| 1 |]\nlet aaaaaa = 0\n"

    let suffixes =
        [ ""
          " }"
          " } \nlet nextDefinition () = 1\n"
          " \nlet nextDefinition () = 1\n"
          " \ntype NextDefinition() = member x.P = 1\n" ]

    let lines =
        [ "BL1", "let f()  = seq { while abb(*C*)", [ "(*C*)", false, [ "abbbbc" ] ]
          "BL2", "let f()  = seq { while abbbbc(*D1*)", [ "(*D1*)", true, [ "Length" ] ]
          "BL3", "let f()  = seq { while abbbbc(*D1*) do (*C*)", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "abbbbc" ] ]
          "BL4", "let f()  = seq { while abbbbc(*D1*) do abb(*C*)", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "abbbbc" ] ]
          "BL5", "let f()  = seq { while abbbbc(*D1*) do abbbbc(*D2*)", [ "(*D1*)", true, [ "Length" ]; "(*D2*)", true, [ "Length" ] ]
          "BL6", "let f()  = seq { while abbbbc(*D1*) do abbbbc.[(*C*)", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "abbbbc"; "aaaaaa" ] ]
          "BL7", "let f()  = seq { while abbbbc(*D1*) do abbbbc.[aaa(*C*)", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "aaaaaa" ] ]
          "BL7a", "let f()  = seq { while abbbbc(*D1*) do abbbbc.[aaa(*C*)]", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "aaaaaa" ] ]
          "BL7b", "let f()  = seq { while abbbbc(*D1*) do abbbbc.[aaa(*C*)] <- ", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "aaaaaa" ] ]
          "BL7c", "let f()  = seq { while abbbbc(*D1*) do abbbbc.[aaa(*C*)] <- 1", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "aaaaaa" ] ]
          "BL7d", "let f()  = seq { while abbbbc(*D1*) do abbbbc.[ (*C*) ] <- 1", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "aaaaaa" ] ]
          "BL8", "let f()  = seq { while abbbbc(*D1*) do abbbbc.[aaaaaa]", [ "(*D1*)", true, [ "Length" ] ]
          "BL9", "let f()  = seq { while abbbbc(*D1*) do abbbbc.[aaaaaa] <- (*C*)", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "abbbbc"; "aaaaaa" ] ]
          "BL10", "let f()  = seq { while abbbbc(*D1*) do abbbbc.[aaaaaa] <- aaa(*C*)", [ "(*D1*)", true, [ "Length" ]; "(*C*)", false, [ "aaaaaa" ] ] ]

    for suffix in suffixes do
        for (lineName, lineText, checks) in lines do
            for (marker, dot, expected) in checks do
                let replacement = if dot then ".{caret}" else "{caret}"
                let markedSource = prefix + lineText.Replace(marker, replacement) + suffix
                let info = Checker.getCompletionInfo markedSource
                let itemNames = info.Items |> Array.map (fun i -> i.NameInCode)

                for name in expected do
                    if not (Array.contains name itemNames) then
                        failwithf
                            "suffix=%A line=%s marker=%s: expected %s but got [%s]"
                            suffix
                            lineName
                            marker
                            name
                            (String.concat ", " itemNames)

[<Fact>]
let ``ComputationExpression.LetBang`` () =
    let info =
        Checker.getCompletionInfo
            """let http(url:string) = 
  async { 
    let rnd = new System.Random()
    let! rsp = rnd.{caret}N"""

    assertHasItemWithNames [ "Next" ] info

[<Fact>]
let ``CompletionInDifferentEnvs3`` () =
    let info =
        Checker.getCompletionInfo
            """let mb1 = new MailboxProcessor<AsyncReplyChannel<int>>(fun inbox -> async { let! msg = inbox.Receive()
                                                                            do {caret}"""

    assertHasItemWithNames [ "msg" ] info

[<Fact>]
let ``CompletionInDifferentEnvs4`` () =
    let info1 =
        Checker.getCompletionInfo
            """async {
    let! x = i
    ({caret}
}"""

    assertHasItemWithNames [ "x" ] info1

    let info2 =
        Checker.getCompletionInfo
            """let q = 
    let a = 20
    let b = (fun i -> i) 40
    (({caret}"""

    assertHasItemWithNames [ "b" ] info2
    assertHasNoItemsWithNames [ "i" ] info2

[<Fact>]
let ``CompletionForAndBang_BaseLine0`` () =
    let info =
        Checker.getCompletionInfo
            """type Builder() =
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
builder {
    let! xxx3 = 2
    return x{caret}
}"""

    assertHasItemWithNames [ "xxx3" ] info

[<Fact>]
let ``CompletionForAndBang_BaseLine1`` () =
    let info =
        Checker.getCompletionInfo
            """type Builder() =
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
let xxx1 = 1
builder {
   let xxx2 = 1
   let! xxx3 = 1
   return (1 + x{caret})
}"""

    assertHasItemWithNames [ "xxx1"; "xxx2"; "xxx3" ] info

[<Fact>]
let ``CompletionForAndBang_BaseLine2`` () =
    let info =
        Checker.getCompletionInfo
            """type Builder() =
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
let yyy1 = 1
builder {
   let yyy2 = 1
   let! yyy3 = 1
   return (1 + y{caret})"""

    assertHasItemWithNames [ "yyy1"; "yyy2"; "yyy3" ] info

[<Fact>]
let ``CompletionForAndBang_BaseLine3`` () =
    let info =
        Checker.getCompletionInfo
            """type Builder() =
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
let zzz1 = 1
builder {
   let zzz2 = 1
   let! zzz3 = 1
   return (1 + z{caret}"""

    assertHasItemWithNames [ "zzz1"; "zzz2"; "zzz3" ] info

[<Fact>]
let ``CompletionForAndBang_BaseLine4`` () =
    let info =
        Checker.getCompletionInfo
            """type Builder() =
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
let zzz1 = 1
builder {
   let! zzz3 = 1
   return (1 + z{caret}"""

    assertHasItemWithNames [ "zzz1"; "zzz3" ] info

[<Fact>]
let ``CompletionForAndBang_Test_MergeSources_Bind_Return0`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "/langversion:preview" |]
            FSharpCodeCompletionOptions.Default
            """type Builder() =
    member x.MergeSources(a: 'T1, b: 'T2) = (a, b)
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
builder {
    let! xxx3 = 2
    and! xxx4 = 2
    return x{caret}
}"""

    assertHasItemWithNames [ "xxx3"; "xxx4" ] info

[<Fact>]
let ``CompletionForAndBang_Test_MergeSources_Bind_Return1`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "/langversion:preview" |]
            FSharpCodeCompletionOptions.Default
            """type Builder() =
    member x.MergeSources(a: 'T1, b: 'T2) = (a, b)
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
let xxx1 = 1
builder {
   let xxx2 = 1
   let! xxx3 = 1
   and! xxx4 = 1
   return (1 + x{caret})
}"""

    assertHasItemWithNames [ "xxx1"; "xxx2"; "xxx3"; "xxx4" ] info

[<Fact>]
let ``CompletionForAndBang_Test_MergeSources_Bind_Return2`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "/langversion:preview" |]
            FSharpCodeCompletionOptions.Default
            """type Builder() =
    member x.MergeSources(a: 'T1, b: 'T2) = (a, b)
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
let yyy1 = 1
builder {
   let yyy2 = 1
   let! yyy3 = 1
   and! yyy4 = 1
   return (1 + y{caret})"""

    assertHasItemWithNames [ "yyy1"; "yyy2"; "yyy3"; "yyy4" ] info

[<Theory>]
[<InlineData("""type Builder() =
    member x.MergeSources(a: 'T1, b: 'T2) = (a, b)
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
let zzz1 = 1
builder {
   let zzz2 = 1
   let! zzz3 = 1
   and! zzz4 = 1
   return (1 + z{caret}""",
             "zzz1 zzz2 zzz3 zzz4")>]
[<InlineData("""type Builder() =
    member x.MergeSources(a: 'T1, b: 'T2) = (a, b)
    member x.Bind(a: 'T1, f: 'T1 -> 'T2) = f a
    member x.Return(a: 'T) = a
let builder = Builder()
let zzz1 = 1
builder {
   let! zzz3 = 1
   and! zzz4 = 1
   return (1 + z{caret}""",
             "zzz1 zzz3 zzz4")>]
let ``CompletionForAndBang_Test_MergeSources_Bind_Return3and4`` (markedSource: string) (expectedNames: string) =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "/langversion:preview" |]
            FSharpCodeCompletionOptions.Default
            markedSource

    assertHasItemWithNames (expectedNames.Split(' ') |> List.ofArray) info

[<Fact>]
let ``CompletionForAndBang_Test_Bind2Return0`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "/langversion:preview" |]
            FSharpCodeCompletionOptions.Default
            """type Builder() =
    member x.Bind2Return(a: 'T1, b: 'T2, f: ('T1 * 'T2) -> 'T3) = f (a, b)
let builder = Builder()
builder {
    let! xxx3 = 2
    and! xxx4 = 2
    return x{caret}
}"""

    assertHasItemWithNames [ "xxx3"; "xxx4" ] info

[<Fact>]
let ``CompletionForAndBang_Test_Bind2Return1`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "/langversion:preview" |]
            FSharpCodeCompletionOptions.Default
            """type Builder() =
    member x.Bind2Return(a: 'T1, b: 'T2, f: ('T1 * 'T2) -> 'T3) = f (a, b)
let builder = Builder()
let xxx1 = 1
builder {
   let xxx2 = 1
   let! xxx3 = 1
   and! xxx4 = 1
   return (1 + x{caret})
}"""

    assertHasItemWithNames [ "xxx1"; "xxx2"; "xxx3"; "xxx4" ] info

[<Fact>]
let ``CompletionForAndBang_Test_Bind2Return2`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "/langversion:preview" |]
            FSharpCodeCompletionOptions.Default
            """type Builder() =
    member x.Bind2Return(a: 'T1, b: 'T2, f: ('T1 * 'T2) -> 'T3) = f (a, b)
let builder = Builder()
let yyy1 = 1
builder {
   let yyy2 = 1
   let! yyy3 = 1
   and! yyy4 = 1
   return (1 + y{caret})"""

    assertHasItemWithNames [ "yyy1"; "yyy2"; "yyy3"; "yyy4" ] info

[<Theory>]
[<InlineData("""type Builder() =
    member x.Bind2Return(a: 'T1, b: 'T2, f: ('T1 * 'T2) -> 'T3) = f (a, b)
let builder = Builder()
let zzz1 = 1
builder {
   let zzz2 = 1
   let! zzz3 = 1
   and! zzz4 = 1
   return (1 + z{caret}""",
             "zzz1 zzz2 zzz3 zzz4")>]
[<InlineData("""type Builder() =
    member x.Bind2Return(a: 'T1, b: 'T2, f: ('T1 * 'T2) -> 'T3) = f (a, b)
let builder = Builder()
let zzz1 = 1
builder {
   let! zzz3 = 1
   and! zzz4 = 1
   return (1 + z{caret}""",
             "zzz1 zzz3 zzz4")>]
let ``CompletionForAndBang_Test_Bind2Return3and4`` (markedSource: string) (expectedNames: string) =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "/langversion:preview" |]
            FSharpCodeCompletionOptions.Default
            markedSource

    assertHasItemWithNames (expectedNames.Split(' ') |> List.ofArray) info

[<Fact>]
let ``Expressions.Computation`` () =
    let info =
        Checker.getCompletionInfo
            """type FooBuilder() =
   member x.Return(a) = new System.Random()
let foo = FooBuilder()
(foo { return 0 }).{caret}"""

    assertHasItemWithNames [ "Next" ] info
    assertHasNoItemsWithNames [ "GetEnumerator" ] info

[<Fact>]
let ``ComputationExpressionLet`` () =
    let info =
        Checker.getCompletionInfo
            """let http(url:string) = 
  async { 
    let rnd = new System.Random()
    let rsp = rnd.{caret}N"""

    assertHasItemWithNames [ "Next" ] info

[<Fact(Skip = "69644 - no completion for an identifier when 'use'd inside an 'async' block")>]
let ``InAsyncAndUseBlock`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System.Text.RegularExpressions
                open System.IO
                let collectLinksAsync (url:string) : Async<string> =
                    async { do printfn "requesting %s" url
                            let! html =
                                async { use reader = new System.IO.StreamReader(new System.IO.FileStream("", FileMode.CreateNew))
                                        do printfn "reading %s" url
                                        return {caret}reader.ReadToEnd()  }  //<---- reader
                            let links = "a"
                            return links }
                """

    assertHasItemWithNames [ "reader" ] info

[<Fact(Skip = "This is still fail")>]
let ``ComputationExpression.WithClosingBrace`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                // regression test for bug 3879: intellisense glitch for computation expression
                // intellisense does not work in computation expression without the closing brace
                type System.Net.WebRequest with
                    member x.AsyncGetResponse() = Async.BuildPrimitive(x.BeginGetResponse, x.EndGetResponse)
                    member x.GetResponseAsync() = x.AsyncGetResponse()
                let http(url:string) =
                     async {let req = System.Net.WebRequest.Create("http://www.yahoo.com")
                            let! rsp = req.{caret}} """

    assertHasItemWithNames [ "AsyncGetResponse"; "GetResponseAsync"; "ToString" ] info

[<Fact(Skip = "This is still fail")>]
let ``ComputationExpression.WithoutClosingBrace`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest
                // regression test for bug 3879: intellisense glitch for computation expression
                // intellisense does not work in computation expression without the closing brace
                type System.Net.WebRequest with
                    member x.AsyncGetResponse() = Async.BuildPrimitive(x.BeginGetResponse, x.EndGetResponse)
                    member x.GetResponseAsync() = x.AsyncGetResponse()
                let http(url:string) =
                    async { let req = System.Net.WebRequest.Create("http://www.yahoo.com")
                            let! rsp = req.{caret}"""

    assertHasItemWithNames [ "AsyncGetResponse"; "GetResponseAsync"; "ToString" ] info

[<Fact(Skip = "aspirational: scope-aware Ctrl-space completion of let!-bound names not yet matched by FCS — https://github.com/dotnet/fsharp/issues/69654")>]
let ``AutoComplete.Bug69654_1`` () =
    let info1 =
        Checker.getCompletionInfo
            """let s = async {
            let! xxx = async { return 0 }
            xxx.Comp{caret}areTo |> ignore // the dot works
            xxx |> ignore // no xxx
            do xxx |> ignore // no xxx
            return xxx // no xxx
        }"""

    assertHasItemWithNames [ "CompareTo" ] info1

    let info2 =
        Checker.getCompletionInfo
            """let s = async {
            let! xxx = async { return 0 }
            x{caret}xx.CompareTo |> ignore // the dot works
            xxx |> ignore // no xxx
            do xxx |> ignore // no xxx
            return xxx // no xxx
        }"""

    assertHasItemWithNames [ "xxx" ] info2

    let info3 =
        Checker.getCompletionInfo
            """let s = async {
            let! xxx = async { return 0 }
            xxx.CompareTo |> ignore // the dot works
            x{caret}xx |> ignore // no xxx
            do xxx |> ignore // no xxx
            return xxx // no xxx
        }"""

    assertHasItemWithNames [ "xxx" ] info3

    let info4 =
        Checker.getCompletionInfo
            """let s = async {
            let! xxx = async { return 0 }
            xxx.CompareTo |> ignore // the dot works
            xxx |> ignore // no xxx
            do xx{caret}x |> ignore // no xxx
            return xxx // no xxx
        }"""

    assertHasItemWithNames [ "xxx" ] info4

    let info5 =
        Checker.getCompletionInfo
            """let s = async {
            let! xxx = async { return 0 }
            xxx.CompareTo |> ignore // the dot works
            xxx |> ignore // no xxx
            do xxx |> ignore // no xxx
            return xx{caret}x // no xxx
        }"""

    assertHasItemWithNames [ "xxx" ] info5

[<Fact(Skip = "aspirational: scope-aware Ctrl-space completion of use-bound names not yet matched by FCS — https://github.com/dotnet/fsharp/issues/69654")>]
let ``AutoComplete.Bug69654_2`` () =
    let info1 =
        Checker.getCompletionInfo
            """let s = async {
            use xxx = null
            xxx.Disp{caret}ose() // the dot works
            xxx |> ignore // no xxx
            do xxx |> ignore // no xxx
            return xxx // no xxx
        }"""

    assertHasItemWithNames [ "Dispose" ] info1

    let info2 =
        Checker.getCompletionInfo
            """let s = async {
            use xxx = null
            x{caret}xx.Dispose() // the dot works
            xxx |> ignore // no xxx
            do xxx |> ignore // no xxx
            return xxx // no xxx
        }"""

    assertHasItemWithNames [ "xxx" ] info2

    let info3 =
        Checker.getCompletionInfo
            """let s = async {
            use xxx = null
            xxx.Dispose() // the dot works
            x{caret}xx |> ignore // no xxx
            do xxx |> ignore // no xxx
            return xxx // no xxx
        }"""

    assertHasItemWithNames [ "xxx" ] info3

    let info4 =
        Checker.getCompletionInfo
            """let s = async {
            use xxx = null
            xxx.Dispose() // the dot works
            xxx |> ignore // no xxx
            do xx{caret}x |> ignore // no xxx
            return xxx // no xxx
        }"""

    assertHasItemWithNames [ "xxx" ] info4

    let info5 =
        Checker.getCompletionInfo
            """let s = async {
            use xxx = null
            xxx.Dispose() // the dot works
            xxx |> ignore // no xxx
            do xxx |> ignore // no xxx
            return xx{caret}x // no xxx
        }"""

    assertHasItemWithNames [ "xxx" ] info5

[<Fact(Skip = "harness-only: asserts background-thread unhandled-exception handling, not completion")>]
let ``EnsureThatUnhandledExceptionsCauseAnAssert`` () = ()
