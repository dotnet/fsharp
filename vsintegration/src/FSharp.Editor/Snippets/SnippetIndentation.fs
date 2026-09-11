// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

/// Where the lines of an inserted snippet belong, as arithmetic over columns.
///
/// The expansion engine inserts a snippet verbatim: the opening line lands at the insertion column
/// and every later line at the column its template spells, with the text substituted into
/// `$selected$` carrying whatever indentation it had in the buffer. C# survives that because Roslyn's
/// formatter reflows the result; F# has no formatter, so the columns are computed here instead.
///
/// This module is deliberately free of editor types so that it can be tested directly - the policy
/// is where the mistakes live, not the buffer edit that applies it.
module internal SnippetIndentation =

    /// What an inserted line is, which is what decides how it moves.
    type LineKind =
        /// The snippet's own text. Takes the column of the code it wraps.
        | Template
        /// A compiler directive, which reads at the left margin whatever it wraps.
        | RootLevelDirective
        /// The first line of the text substituted into `$selected$`. The template already placed it.
        | SelectedFirst
        /// A later line of that text. It starts its own buffer line at its original column.
        | SelectedRest
        /// Whitespace only; left alone so the snippet does not leave trailing spaces behind.
        | Blank

    type Line = { Kind: LineKind; Indent: int }

    /// How the snippet got there, which is what supplies the column to align to.
    type Placement =
        /// Insert Snippet. The caret already positioned the opening line; the rest follow it.
        | AtCaret of column: int
        /// Surround With over a whole-line selection, so the insertion began at column 0.
        /// `column` is the column the wrapped block sat at, `fieldIndent` the template's own
        /// indentation around `$selected$` - the one nesting level the wrapper contributes.
        | AroundSelection of column: int * fieldIndent: int

    let private rootLevelDirectives =
        [| "#if"; "#else"; "#endif"; "#nowarn"; "#warnon" |]

    /// Whether a snippet line is a compiler directive rather than code. Those wrappers belong at the
    /// left margin whatever they wrap, so the code they cover keeps the column it had. `#nowarn` and
    /// `#warnon` are scoped, but they read as directives all the same.
    let isRootLevelDirective (lineText: string) =
        let text = lineText.TrimStart()

        rootLevelDirectives
        |> Array.exists (fun directive -> text.StartsWith(directive, StringComparison.Ordinal))

    /// How far each line has to move. Positive inserts, negative removes, zero leaves it alone.
    let deltas placement (lines: Line list) =
        lines
        |> List.mapi (fun index line ->
            match line.Kind, placement with
            | Blank, _ -> 0
            | RootLevelDirective, _ -> -line.Indent
            | Template, AtCaret column -> if index = 0 then 0 else column
            | Template, AroundSelection(column, _) -> column
            | SelectedFirst, _ -> 0
            | SelectedRest, AroundSelection(_, fieldIndent) -> fieldIndent
            | SelectedRest, AtCaret _ -> 0)
