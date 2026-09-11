// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Xunit

open Microsoft.VisualStudio.FSharp.Editor.SnippetIndentation

/// Every case here is a real insertion that came out wrong at some point, recorded as the columns the
/// expansion engine left behind and the columns the result should have.
module SnippetIndentationTests =

    /// The indentation each line ends up at, which is what a reader can check against F# they know.
    let private columnsAfter placement lines =
        let moved = deltas placement lines

        List.map2 (fun line delta -> line.Indent + delta) lines moved

    let private template indent = { Kind = Template; Indent = indent }

    let private directive indent =
        {
            Kind = RootLevelDirective
            Indent = indent
        }

    let private selectedFirst indent =
        {
            Kind = SelectedFirst
            Indent = indent
        }

    let private selectedRest indent =
        { Kind = SelectedRest; Indent = indent }

    [<Fact>]
    let ``Surround With for over two lines nests both under the loop`` () =
        // fields = [
        //     yield Define.Field …        <- the two selected lines, at column 12
        //     yield Define.AsyncField …
        // Template is `for $item$ in $collection$ do` / `    $selected$$end$`, so the engine leaves the
        // first selected line at 4 + 12 and the second at its own 12.
        let lines = [ template 0; selectedFirst 16; selectedRest 12 ]

        Assert.Equal<int list>([ 12; 16; 16 ], columnsAfter (AroundSelection(12, 4)) lines)

    [<Fact>]
    let ``Surround With async keeps the wrapper at the code's column`` () =
        // `async {` and `}` are the snippet's own lines and belong at the wrapped code's column, not at
        // the column 0 the verbatim insertion left them at.
        let lines = [ template 0; selectedFirst 24; template 0 ]

        Assert.Equal<int list>([ 20; 24; 20 ], columnsAfter (AroundSelection(20, 4)) lines)

    [<Fact>]
    let ``Surround With a directive pair pins it to column zero and does not nest`` () =
        // A directive wrapper - `#if`/`#endif`, or the scoped `#nowarn`/`#warnon` pair - wraps code
        // without indenting it, so `$selected$` sits at template column 0 and the wrapped lines keep
        // the columns they had.
        let lines = [ directive 0; selectedFirst 20; directive 0 ]

        Assert.Equal<int list>([ 0; 20; 0 ], columnsAfter (AroundSelection(20, 0)) lines)

    [<Theory>]
    [<InlineData "#if DEBUG">]
    [<InlineData "#else">]
    [<InlineData "#endif">]
    [<InlineData "#nowarn 0040">]
    [<InlineData "        #warnon 0040">]
    let ``A directive is recognized wherever the engine left it`` (line: string) = Assert.True(isRootLevelDirective line)

    [<Theory>]
    [<InlineData "async {">]
    [<InlineData "| _ -> ()">]
    let ``Code is not mistaken for a directive`` (line: string) = Assert.False(isRootLevelDirective line)

    [<Fact>]
    let ``Insert Snippet leaves the opening line where the caret put it`` () =
        // The caret positioned `async {`; the body and the closing brace follow its column.
        let lines = [ template 8; template 4; template 0 ]

        Assert.Equal<int list>([ 8; 12; 8 ], columnsAfter (AtCaret 8) lines)

    [<Fact>]
    let ``Insert Snippet still pins a directive to column zero`` () =
        let lines = [ directive 8; template 4; directive 0 ]

        Assert.Equal<int list>([ 0; 12; 0 ], columnsAfter (AtCaret 8) lines)

    [<Fact>]
    let ``A blank line is left alone`` () =
        let lines = [ template 0; { Kind = Blank; Indent = 0 }; template 0 ]

        Assert.Equal<int list>([ 20; 0; 20 ], columnsAfter (AroundSelection(20, 4)) lines)

    [<Fact>]
    let ``A selection keeps its own internal shape`` () =
        // A deeper second line stays one level deeper than the first.
        let lines = [ template 0; selectedFirst 16; selectedRest 16 ]

        Assert.Equal<int list>([ 12; 16; 20 ], columnsAfter (AroundSelection(12, 4)) lines)
