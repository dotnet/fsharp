// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.SnippetIndentationTests

open Xunit

open Microsoft.VisualStudio.FSharp.Editor.SnippetIndentation

/// Each line as its kind, the indentation the engine left it at, and the indentation it should end up at.
let private scenarios =
    [
        "Surround With for over two lines nests both under the loop",
        AroundSelection(12, 4),
        [ Template, 0, 12; SelectedFirst, 16, 16; SelectedRest, 12, 16 ]

        "Surround With async keeps the wrapper at the code's column",
        AroundSelection(20, 4),
        [ Template, 0, 20; SelectedFirst, 24, 24; Template, 0, 20 ]

        "Surround With a directive pair pins it to column zero and does not nest",
        AroundSelection(20, 0),
        [ RootLevelDirective, 0, 0; SelectedFirst, 20, 20; RootLevelDirective, 0, 0 ]

        "Insert Snippet leaves the opening line where the caret put it", AtCaret 8, [ Template, 8, 8; Template, 4, 12; Template, 0, 8 ]

        "Insert Snippet after a tab nests the body by the tab's width",
        AtCaret(advanceColumn 4 0 '\t'),
        [ Template, 4, 4; Template, 4, 8; Template, 0, 4 ]

        "Insert Snippet still pins a directive to column zero",
        AtCaret 8,
        [ RootLevelDirective, 8, 0; Template, 4, 12; RootLevelDirective, 0, 0 ]

        "A blank line is left alone", AroundSelection(20, 4), [ Template, 0, 20; Blank, 0, 0; Template, 0, 20 ]

        "A selection keeps its own internal shape", AroundSelection(12, 4), [ Template, 0, 12; SelectedFirst, 16, 16; SelectedRest, 16, 20 ]

        "A line inside a string carried over from the selection is left alone",
        AroundSelection(20, 4),
        [ Template, 0, 20; SelectedFirst, 0, 0; InsideString, 0, 0 ]
    ]

let scenarioNames: obj[][] = [| for name, _, _ in scenarios -> [| name |] |]

let private kindsOf selectedLines lines =
    classify 4 selectedLines lines |> List.map _.Kind

[<Theory; MemberData(nameof scenarioNames)>]
let ``Each line moves to where its kind and the placement put it`` (name: string) =
    let _, placement, lines =
        scenarios |> List.find (fun (scenario, _, _) -> scenario = name)

    let engine =
        lines |> List.map (fun (kind, indent, _) -> { Kind = kind; Indent = indent })

    let expected = lines |> List.map (fun (_, _, indent) -> indent)

    let actual =
        deltas placement engine
        |> List.map2 (fun (line: Line) delta -> line.Indent + delta) engine

    Assert.Equal<int list>(expected, actual)

[<Theory>]
[<InlineData(4, 0, '\t', 4)>]
[<InlineData(4, 1, '\t', 4)>]
[<InlineData(4, 4, '\t', 8)>]
[<InlineData(4, 0, 'a', 1)>]
let ``A tab runs on to the next tab stop and any other character takes one column``
    (tabSize: int, column: int, character: char, expected: int)
    =
    Assert.Equal<int>(expected, advanceColumn tabSize column character)

[<Fact>]
let ``Indentation is measured in visual columns`` () =
    Assert.Equal<int list>([ 6 ], classify 4 ValueNone [ "\t  x" ] |> List.map _.Indent)

[<Fact>]
let ``A directive is recognized wherever the engine left it`` () =
    let lines, kinds =
        [
            "#if DEBUG", RootLevelDirective
            "    code", Template
            "    #elif TRACE", RootLevelDirective
            "    #else", RootLevelDirective
            "    code", Template
            "#endif", RootLevelDirective
            "#nowarn 0040", RootLevelDirective
            "        #warnon 0040", RootLevelDirective
        ]
        |> List.unzip

    Assert.Equal<LineKind list>(kinds, kindsOf ValueNone lines)

[<Theory>]
[<InlineData "async {">]
[<InlineData "| _ -> ()">]
[<InlineData "printfn \"#endif\"">]
let ``Code is not mistaken for a directive`` (line: string) =
    Assert.Equal<LineKind list>([ Template ], kindsOf ValueNone [ line ])

[<Fact>]
let ``A blank line stays blank inside a branch the lexer skips`` () =
    Assert.Equal<LineKind list>([ RootLevelDirective; Blank; RootLevelDirective ], kindsOf ValueNone [ "#if A"; "    "; "#endif" ])

[<Theory>]
[<InlineData("let s = \"a", "b\"")>]
[<InlineData("let s = @\"a", "  b\"")>]
[<InlineData("let s = \"\"\"a", "#if DEBUG")>]
[<InlineData("let s = \"\"\"a", "\"\"\" |> ignore")>]
[<InlineData("let s = $\"\"\"a {1}", "b\"\"\"")>]
[<InlineData("let s = $$\"\"\"a", "b\"\"\"")>]
let ``A selected line that continues a string is left alone, even inside a directive wrapper`` (opening: string, continuation: string) =
    Assert.Equal<LineKind list>(
        [ RootLevelDirective; SelectedFirst; InsideString; RootLevelDirective ],
        kindsOf (ValueSome(1, 2)) [ "#if DEBUG"; opening; continuation; "#endif" ]
    )

[<Theory>]
[<InlineData("let text = \"\"\"", "\"\"\"")>]
[<InlineData("let text = @\"", "\"")>]
let ``Selected lines a snippet puts inside its own string are that string's content`` (opening: string, closing: string) =
    Assert.Equal<LineKind list>(
        [ Template; InsideString; InsideString; InsideString; Template ],
        kindsOf (ValueSome(1, 2)) [ opening; "    ()"; "()"; closing; "()" ]
    )

[<Theory>]
[<InlineData("printfn \"a\"", "\"b\" |> printfn \"%s\"")>]
[<InlineData("let s = $\"\"\"a {", "  1 } b\"\"\"")>]
let ``A selected line that only starts with a string, or sits in an interpolation hole, is code`` (first: string, rest: string) =
    Assert.Equal<LineKind list>([ SelectedFirst; SelectedRest ], kindsOf (ValueSome(0, 1)) [ first; rest ])

[<Theory>]
[<InlineData "    #endif">]
[<InlineData "    #if NET">]
let ``A directive the selection leaves unbalanced does not move the wrapper's own`` (selected: string) =
    let kinds =
        kindsOf (ValueSome(1, 2)) [ "#if DEBUG"; "    let b = 3"; selected; "#endif" ]

    Assert.Equal<LineKind>(RootLevelDirective, List.last kinds)
