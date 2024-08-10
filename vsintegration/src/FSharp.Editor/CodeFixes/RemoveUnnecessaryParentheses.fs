// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Composition
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor.Extensions
open CancellableTasks

[<AutoOpen>]
module private Patterns =
    let inline toPat f x = if f x then ValueSome() else ValueNone

    /// Starts with //.
    [<return: Struct>]
    let (|StartsWithSingleLineComment|_|) (s: string) =
        if s.AsSpan().TrimStart(' ').StartsWith("//".AsSpan()) then
            ValueSome StartsWithSingleLineComment
        else
            ValueNone

    /// Starts with match, e.g.,
    ///
    ///     (match … with
    ///     | … -> …)
    [<return: Struct>]
    let (|StartsWithMatch|_|) (s: string) =
        let s = s.AsSpan().TrimStart ' '

        if s.StartsWith("match".AsSpan()) && (s.Length = 5 || s[5] = ' ') then
            ValueSome StartsWithMatch
        else
            ValueNone

    [<AutoOpen>]
    module Char =
        [<return: Struct>]
        let inline (|LetterOrDigit|_|) c = toPat Char.IsLetterOrDigit c

        [<return: Struct>]
        let inline (|Punctuation|_|) c = toPat Char.IsPunctuation c

        [<return: Struct>]
        let inline (|Symbol|_|) c = toPat Char.IsSymbol c

    [<AutoOpen>]
    module SourceText =
        /// E.g., something like:
        ///
        ///     let … = (␤
        ///     …
        ///     )
        [<return: Struct>]
        let (|TrailingOpen|_|) (span: TextSpan) (sourceText: SourceText) =
            let linePosition = sourceText.Lines.GetLinePosition span.Start
            let line = (sourceText.Lines.GetLineFromPosition span.Start).ToString()

            if
                line.AsSpan(0, linePosition.Character).LastIndexOfAnyExcept(' ', '(') >= 0
                && line.AsSpan(linePosition.Character).IndexOfAnyExcept('(', ' ') < 0
            then
                ValueSome TrailingOpen
            else
                ValueNone

[<NoEquality; NoComparison>]
type private InnerOffsides =
    /// We haven't found an inner construct yet.
    | NoneYet

    /// The start column of the first inner construct we find.
    /// This may not be on the same line as the open paren.
    | FirstLine of col: int

    /// The leftmost start column of an inner construct on a line
    /// following the first inner construct we found.
    /// We keep the first column of the first inner construct for comparison at the end.
    | FollowingLine of firstLine: int * followingLine: int

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RemoveUnnecessaryParentheses); Shared; Sealed>]
type internal FSharpRemoveUnnecessaryParenthesesCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.RemoveUnnecessaryParentheses()
    static let fixableDiagnosticIds = ImmutableArray.Create "FS3583"

    override _.FixableDiagnosticIds = fixableDiagnosticIds

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    override this.GetFixAllProvider() =
        this.RegisterFsharpFixAll(fun diagnostics ->
            // There may be pairs of diagnostics with nested spans
            // for which it would be valid to apply either but not both, e.g.,
            // (x.M(y)).N → (x.M y).N ↮ x.M(y).N
            let builder = ImmutableArray.CreateBuilder diagnostics.Length

            let spans =
                SortedSet
                    { new IComparer<TextSpan> with
                        member _.Compare(x, y) =
                            if x.IntersectsWith y then 0 else x.CompareTo y
                    }

            for i in 0 .. diagnostics.Length - 1 do
                let diagnostic = diagnostics[i]

                if spans.Add diagnostic.Location.SourceSpan then
                    builder.Add diagnostic

            builder.ToImmutable())

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            assert (context.Span.Length >= 3) // (…)

            cancellableTask {
                let! sourceText = context.GetSourceTextAsync()
                let txt = sourceText.ToString(TextSpan(context.Span.Start, context.Span.Length))

                let firstChar = txt[0]
                let lastChar = txt[txt.Length - 1]

                match firstChar, lastChar with
                | '(', ')' ->
                    /// Trim only spaces from the start if there is something else
                    /// before the open paren on the same line (or else we could move
                    /// the whole inner expression up a line); otherwise trim all whitespace
                    /// from start and end.
                    let (|Trim|) (sourceText: SourceText) =
                        let linePosition = sourceText.Lines.GetLinePosition context.Span.Start
                        let line = (sourceText.Lines.GetLineFromPosition context.Span.Start).ToString()

                        if line.AsSpan(0, linePosition.Character).LastIndexOfAnyExcept(' ', '(') >= 0 then
                            fun (s: string) -> s.TrimEnd().TrimStart ' '
                        else
                            fun (s: string) -> s.Trim()

                    let (|ShiftLeft|NoShift|ShiftRight|) (sourceText: SourceText) =
                        let startLinePosition = sourceText.Lines.GetLinePosition context.Span.Start
                        let endLinePosition = sourceText.Lines.GetLinePosition context.Span.End
                        let startLineNo = startLinePosition.Line
                        let endLineNo = endLinePosition.Line

                        if startLineNo = endLineNo then
                            NoShift
                        else
                            let outerOffsides = startLinePosition.Character

                            let rec loop innerOffsides lineNo startCol =
                                if lineNo <= endLineNo then
                                    let line = sourceText.Lines[lineNo].ToString()

                                    match line.AsSpan(startCol).IndexOfAnyExcept(' ', ')') with
                                    | -1 -> loop innerOffsides (lineNo + 1) 0
                                    | i ->
                                        match line[i + startCol ..] with
                                        | StartsWithMatch
                                        | StartsWithSingleLineComment -> loop innerOffsides (lineNo + 1) 0
                                        | _ ->
                                            match innerOffsides with
                                            | NoneYet -> loop (FirstLine(i + startCol)) (lineNo + 1) 0

                                            | FirstLine inner -> loop (FollowingLine(inner, i + startCol)) (lineNo + 1) 0

                                            | FollowingLine(firstLine, innerOffsides) ->
                                                loop (FollowingLine(firstLine, min innerOffsides (i + startCol))) (lineNo + 1) 0
                                else
                                    innerOffsides

                            match loop NoneYet startLineNo (startLinePosition.Character + 1) with
                            | NoneYet -> NoShift
                            | FirstLine innerOffsides when innerOffsides < outerOffsides -> ShiftRight(outerOffsides - innerOffsides)
                            | FirstLine innerOffsides -> ShiftLeft(innerOffsides - outerOffsides)
                            | FollowingLine(firstLine, followingLine) ->
                                match firstLine - outerOffsides with
                                | 0 -> NoShift
                                | 1 when firstLine < followingLine -> NoShift
                                | primaryOffset when primaryOffset < 0 -> ShiftRight -primaryOffset
                                | primaryOffset -> ShiftLeft primaryOffset

                    let adjusted =
                        match sourceText with
                        | TrailingOpen context.Span -> txt[1 .. txt.Length - 2].TrimEnd()
                        | Trim trim & NoShift -> trim txt[1 .. txt.Length - 2]
                        | Trim trim & ShiftLeft spaces -> trim (txt[1 .. txt.Length - 2].Replace("\n" + String(' ', spaces), "\n"))
                        | Trim trim & ShiftRight spaces -> trim (txt[1 .. txt.Length - 2].Replace("\n", "\n" + String(' ', spaces)))

                    let newText =
                        let (|ShouldPutSpaceBefore|_|) (s: string) =
                            match s with
                            | StartsWithMatch -> None
                            | _ ->
                                // ……(……)
                                // ↑↑ ↑
                                match sourceText[max (context.Span.Start - 2) 0], sourceText[max (context.Span.Start - 1) 0], s[0] with
                                | _, _, ('\n' | '\r') -> None
                                | '[', '|', (Punctuation | LetterOrDigit) -> None
                                | _, '[', '<' -> Some ShouldPutSpaceBefore
                                | _, ('(' | '[' | '{'), _ -> None
                                | _, '>', _ -> Some ShouldPutSpaceBefore
                                | ' ', '=', _ -> Some ShouldPutSpaceBefore
                                | _, '=', ('(' | '[' | '{') -> None
                                | _, '=', (Punctuation | Symbol) -> Some ShouldPutSpaceBefore
                                | _, ('_' | LetterOrDigit), '(' -> None
                                | _, ('_' | LetterOrDigit | '`'), _ -> Some ShouldPutSpaceBefore
                                | _, (Punctuation | Symbol), (Punctuation | Symbol) -> Some ShouldPutSpaceBefore
                                | _ -> None

                        let (|ShouldPutSpaceAfter|_|) (s: string) =
                            // (……)…
                            //   ↑ ↑
                            match s[s.Length - 1], sourceText[min context.Span.End (sourceText.Length - 1)] with
                            | '>', ('|' | ']') -> Some ShouldPutSpaceAfter
                            | _, (')' | ']' | '[' | '}' | '.' | ';' | ',' | '|') -> None
                            | _, ('+' | '-' | '%' | '&' | '!' | '~') -> None
                            | (Punctuation | Symbol), (Punctuation | Symbol | LetterOrDigit) -> Some ShouldPutSpaceAfter
                            | ('_' | LetterOrDigit), ('_' | LetterOrDigit) -> Some ShouldPutSpaceAfter
                            | _ -> None

                        let (|WouldTurnInfixIntoPrefix|_|) (s: string) =
                            // (……)…
                            //   ↑ ↑
                            match s[s.Length - 1], sourceText[min context.Span.End (sourceText.Length - 1)] with
                            | (Punctuation | Symbol), ('+' | '-' | '%' | '&' | '!' | '~') ->
                                let linePos = sourceText.Lines.GetLinePosition context.Span.End
                                let line = sourceText.Lines[linePos.Line].ToString()

                                // (……)+…
                                //      ↑
                                match line.AsSpan(linePos.Character).IndexOfAnyExcept("*/%-+:^@><=!|$.?".AsSpan()) with
                                | -1 -> None
                                | i when line[linePos.Character + i] <> ' ' -> Some WouldTurnInfixIntoPrefix
                                | _ -> None
                            | _ -> None

                        match adjusted with
                        | WouldTurnInfixIntoPrefix -> ValueNone
                        | ShouldPutSpaceBefore & ShouldPutSpaceAfter -> ValueSome(" " + adjusted + " ")
                        | ShouldPutSpaceBefore -> ValueSome(" " + adjusted)
                        | ShouldPutSpaceAfter -> ValueSome(adjusted + " ")
                        | adjusted -> ValueSome adjusted

                    return
                        newText
                        |> ValueOption.map (fun newText ->
                            {
                                Name = CodeFix.RemoveUnnecessaryParentheses
                                Message = title
                                Changes = [ TextChange(context.Span, newText) ]
                            })

                | notParens ->
                    System.Diagnostics.Debug.Fail $"%A{notParens} <> ('(', ')')"
                    return ValueNone
            }
