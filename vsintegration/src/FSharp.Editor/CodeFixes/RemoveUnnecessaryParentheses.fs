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

        /// Trim only spaces from the start if there is something else
        /// before the open paren on the same line (or else we could move
        /// the whole inner expression up a line); otherwise trim all whitespace
        // from start and end.
        let (|Trim|) (span: TextSpan) (sourceText: SourceText) =
            let linePosition = sourceText.Lines.GetLinePosition span.Start
            let line = (sourceText.Lines.GetLineFromPosition span.Start).ToString()

            if line.AsSpan(0, linePosition.Character).LastIndexOfAnyExcept(' ', '(') >= 0 then
                fun (s: string) -> s.TrimEnd().TrimStart ' '
            else
                fun (s: string) -> s.Trim()

        /// Returns the offsides diff if the given span contains an expression
        /// whose indentation would be made invalid if the open paren
        /// were removed (because the offside line would be shifted), e.g.,
        ///
        ///     // Valid.
        ///     (let x = 2
        ///      x)
        ///
        ///     // Invalid.
        ///    ←let x = 2
        ///      x◌
        ///
        ///     // Valid.
        ///     ◌let x = 2
        ///      x◌
        [<return: Struct>]
        let (|OffsidesDiff|_|) (span: TextSpan) (sourceText: SourceText) =
            let startLinePosition = sourceText.Lines.GetLinePosition span.Start
            let endLinePosition = sourceText.Lines.GetLinePosition span.End
            let startLineNo = startLinePosition.Line
            let endLineNo = endLinePosition.Line

            if startLineNo = endLineNo then
                ValueNone
            else
                let rec loop innerOffsides lineNo startCol =
                    if lineNo <= endLineNo then
                        let line = sourceText.Lines[lineNo].ToString()

                        match line.AsSpan(startCol).IndexOfAnyExcept(' ', ')') with
                        | -1 -> loop innerOffsides (lineNo + 1) 0
                        | i -> loop (i + startCol) (lineNo + 1) 0
                    else
                        ValueSome(startLinePosition.Character - innerOffsides)

                loop startLinePosition.Character startLineNo (startLinePosition.Character + 1)

        let (|ShiftLeft|NoShift|ShiftRight|) n =
            if n < 0 then ShiftLeft -n
            elif n = 0 then NoShift
            else ShiftRight n

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
                    let adjusted =
                        match sourceText with
                        | TrailingOpen context.Span -> txt[1 .. txt.Length - 2].TrimEnd()

                        | Trim context.Span trim & OffsidesDiff context.Span spaces ->
                            match spaces with
                            | NoShift -> trim txt[1 .. txt.Length - 2]
                            | ShiftLeft spaces -> trim (txt[1 .. txt.Length - 2].Replace("\n" + String(' ', spaces), "\n"))
                            | ShiftRight spaces -> trim (txt[1 .. txt.Length - 2].Replace("\n", "\n" + String(' ', spaces)))

                        | _ -> txt[1 .. txt.Length - 2].Trim()

                    let newText =
                        let (|ShouldPutSpaceBefore|_|) (s: string) =
                            // "……(……)"
                            //  ↑↑ ↑
                            match sourceText[max (context.Span.Start - 2) 0], sourceText[max (context.Span.Start - 1) 0], s[0] with
                            | _, _, ('\n' | '\r') -> None
                            | '[', '|', (Punctuation | LetterOrDigit) -> None
                            | _, '[', '<' -> Some ShouldPutSpaceBefore
                            | _, ('(' | '[' | '{'), _ -> None
                            | _, '>', _ -> Some ShouldPutSpaceBefore
                            | ' ', '=', _ -> Some ShouldPutSpaceBefore
                            | _, '=', ('(' | '[' | '{') -> None
                            | _, '=', (Punctuation | Symbol) -> Some ShouldPutSpaceBefore
                            | _, LetterOrDigit, '(' -> None
                            | _, (LetterOrDigit | '`'), _ -> Some ShouldPutSpaceBefore
                            | _, (Punctuation | Symbol), (Punctuation | Symbol) -> Some ShouldPutSpaceBefore
                            | _ -> None

                        let (|ShouldPutSpaceAfter|_|) (s: string) =
                            // "(……)…"
                            //    ↑ ↑
                            match s[s.Length - 1], sourceText[min context.Span.End (sourceText.Length - 1)] with
                            | '>', ('|' | ']') -> Some ShouldPutSpaceAfter
                            | _, (')' | ']' | '[' | '}' | '.' | ';' | ',' | '|') -> None
                            | (Punctuation | Symbol), (Punctuation | Symbol | LetterOrDigit) -> Some ShouldPutSpaceAfter
                            | LetterOrDigit, LetterOrDigit -> Some ShouldPutSpaceAfter
                            | _ -> None

                        match adjusted with
                        | ShouldPutSpaceBefore & ShouldPutSpaceAfter -> " " + adjusted + " "
                        | ShouldPutSpaceBefore -> " " + adjusted
                        | ShouldPutSpaceAfter -> adjusted + " "
                        | adjusted -> adjusted

                    return
                        ValueSome
                            {
                                Name = CodeFix.RemoveUnnecessaryParentheses
                                Message = title
                                Changes = [ TextChange(context.Span, newText) ]
                            }

                | notParens ->
                    System.Diagnostics.Debug.Fail $"%A{notParens} <> ('(', ')')"
                    return ValueNone
            }
