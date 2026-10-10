// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

open FSharp.Compiler.Tokenization

/// The expansion engine inserts a snippet verbatim: the opening line lands at the insertion column
/// and every later line at the column its template spells, with the text substituted into
/// `$selected$` carrying whatever indentation it had in the buffer. C# survives that because Roslyn's
/// formatter reflows the result; F# has no formatter, so the columns are computed here instead.
module internal SnippetIndentation =

    type LineKind =
        /// The snippet's own text. Takes the column of the code it wraps.
        | Template
        /// A compiler directive, which reads at the left margin whatever it wraps.
        | RootLevelDirective
        /// The first line of the text substituted into `$selected$`. The template already placed it.
        | SelectedFirst
        /// A later line of that text. It starts its own buffer line at its original column.
        | SelectedRest
        /// A later line of that text which begins inside a string literal continued from an earlier
        /// selected line - its whitespace is part of the string's value, not its layout.
        | InsideString
        /// Whitespace only; left alone so the snippet does not leave trailing spaces behind.
        | Blank

    /// `Indent` is a visual column, so a tab counts as the width it renders at.
    type Line = { Kind: LineKind; Indent: int }

    type Placement =
        /// Insert Snippet. The caret already positioned the opening line; the rest follow it.
        | AtCaret of column: int
        /// Surround With over a whole-line selection, so the insertion began at column 0.
        /// `column` is the column the wrapped block sat at, `fieldIndent` the template's own
        /// indentation around `$selected$` - the one nesting level the wrapper contributes.
        | AroundSelection of column: int * fieldIndent: int

    let advanceColumn tabSize column character =
        if character = '\t' then
            column + tabSize - column % tabSize
        else
            column + 1

    /// The public list of lexer states is missing some of the string ones, so the state is asked how it reads code.
    let private isInsideString (tokenizer: FSharpSourceTokenizer) lexState =
        match (tokenizer.CreateLineTokenizer "x").ScanToken lexState with
        | Some token, _ -> token.ColorClass = FSharpTokenColorKind.String
        | None, _ -> false

    let rec private leadingTokenAndEndState (tokenizer: FSharpLineTokenizer) lexState leadingToken =
        match tokenizer.ScanToken lexState, leadingToken with
        | (None, endState), _ -> struct (leadingToken, endState)
        | (Some token, afterToken), ValueNone when token.ColorClass <> FSharpTokenColorKind.Default ->
            leadingTokenAndEndState tokenizer afterToken (ValueSome token)
        | (Some _, afterToken), _ -> leadingTokenAndEndState tokenizer afterToken leadingToken

    let private originOf selectedLines index =
        match selectedLines with
        | ValueSome(first, _) when index = first -> SelectedFirst
        | ValueSome(first, last) when index > first && index <= last -> SelectedRest
        | _ -> Template

    let private classifyLine (tokenizer: FSharpSourceTokenizer) tabSize origin lexState (text: string) =
        let struct (leadingToken, endState) =
            leadingTokenAndEndState (tokenizer.CreateLineTokenizer text) lexState ValueNone

        let kind =
            match leadingToken with
            | _ when String.IsNullOrWhiteSpace text -> Blank
            | _ when isInsideString tokenizer lexState -> InsideString
            | ValueSome token when token.ColorClass = FSharpTokenColorKind.PreprocessorKeyword -> RootLevelDirective
            | _ -> origin

        {
            Kind = kind
            Indent = text |> Seq.takeWhile Char.IsWhiteSpace |> Seq.fold (advanceColumn tabSize) 0
        },
        endState

    /// `selectedLines` are the first and last index filled from `$selected$`. Those lines are lexed apart from the
    /// snippet's own, so a directive or string one of them leaves open does not reach into the other - except a
    /// string the snippet itself opens around the field, which the selected lines are the content of.
    let classify tabSize selectedLines (lines: string list) =
        let tokenizer = FSharpSourceTokenizer([], None, None)

        lines
        |> List.indexed
        |> List.mapFold
            (fun struct (template, selection) (index, text) ->
                let origin = originOf selectedLines index

                if origin.IsTemplate || isInsideString tokenizer template then
                    let line, template = classifyLine tokenizer tabSize origin template text
                    line, struct (template, selection)
                else
                    let line, selection = classifyLine tokenizer tabSize origin selection text
                    line, struct (template, selection))
            struct (FSharpTokenizerLexState.Initial, FSharpTokenizerLexState.Initial)
        |> fst

    /// How far each line has to move. Positive inserts, negative removes, zero leaves it alone.
    let deltas placement (lines: Line list) =
        lines
        |> List.mapi (fun index line ->
            match line.Kind, placement with
            | Blank, _
            | InsideString, _ -> 0
            | RootLevelDirective, _ -> -line.Indent
            | Template, AtCaret column -> if index = 0 then 0 else column
            | Template, AroundSelection(column, _) -> column
            | SelectedFirst, _ -> 0
            | SelectedRest, AroundSelection(_, fieldIndent) -> fieldIndent
            | SelectedRest, AtCaret _ -> 0)
