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
        /// Returns true if the given span contains an expression
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
        let containsSensitiveIndentation (span: TextSpan) (sourceText: SourceText) =
            let startLinePosition = sourceText.Lines.GetLinePosition span.Start
            let endLinePosition = sourceText.Lines.GetLinePosition span.End
            let startLine = startLinePosition.Line
            let startCol = startLinePosition.Character
            let endLine = endLinePosition.Line

            if startLine = endLine then
                false
            else
                let rec loop offsides lineNo startCol =
                    if lineNo <= endLine then
                        let line = sourceText.Lines[ lineNo ].ToString()

                        match offsides with
                        | ValueNone ->
                            let i = line.AsSpan(startCol).IndexOfAnyExcept(' ', ')')

                            if i >= 0 then
                                loop (ValueSome(i + startCol)) (lineNo + 1) 0
                            else
                                loop offsides (lineNo + 1) 0

                        | ValueSome offsidesCol ->
                            let i = line.AsSpan(0, min offsidesCol line.Length).IndexOfAnyExcept(' ', ')')
                            i <= offsidesCol || loop offsides (lineNo + 1) 0
                    else
                        false

                loop ValueNone startLine startCol

        let hasPrecedingConstructOnSameLine (span: TextSpan) (sourceText: SourceText) =
            let linePosition = sourceText.Lines.GetLinePosition span.Start
            let line = (sourceText.Lines.GetLineFromPosition span.Start).ToString()
            line.AsSpan(0, linePosition.Character).LastIndexOfAnyExcept(' ', '(') >= 0

        let followingLineMovesOffsidesRightward (span: TextSpan) (sourceText: SourceText) =
            let startLinePosition = sourceText.Lines.GetLinePosition span.Start
            let startLine = startLinePosition.Line
            let endLinePosition = sourceText.Lines.GetLinePosition span.End
            let endLine = endLinePosition.Line
            let offsides = startLinePosition.Character

            let rec loop lineNo =
                if lineNo <= endLine then
                    let line = sourceText.Lines[ lineNo ].ToString().AsSpan()
                    let i = line.IndexOfAnyExcept("*/%-+:^@><=!|0$.?) ".AsSpan())
                    i > offsides || loop (lineNo + 1)
                else
                    false

            loop (startLine + 1)

        [<return: Struct>]
        let (|ContainsSensitiveIndentation|_|) span sourceText =
            toPat (containsSensitiveIndentation span) sourceText

        [<return: Struct>]
        let (|HasPrecedingConstructOnSameLine|_|) span sourceText =
            toPat (hasPrecedingConstructOnSameLine span) sourceText

        [<return: Struct>]
        let (|FollowingLineMovesOffsidesRightward|_|) span sourceText =
            toPat (followingLineMovesOffsidesRightward span) sourceText

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
                    let (|ShouldPutSpaceBefore|_|) (s: string) =
                        // "……(……)"
                        //  ↑↑ ↑
                        match sourceText[max (context.Span.Start - 2) 0], sourceText[max (context.Span.Start - 1) 0], s[1] with
                        | _, _, ('\n' | '\r') -> None
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
                        match s[s.Length - 2], sourceText[min context.Span.End (sourceText.Length - 1)] with
                        | _, (')' | ']' | '[' | '}' | '.' | ';') -> None
                        | (Punctuation | Symbol), (Punctuation | Symbol | LetterOrDigit) -> Some ShouldPutSpaceAfter
                        | LetterOrDigit, LetterOrDigit -> Some ShouldPutSpaceAfter
                        | _ -> None

                    let (|NewOffsidesOnFirstLine|_|) (s: string) =
                        let s = s.AsSpan 1 // (…
                        let newline = s.IndexOfAny('\n', '\r')

                        if newline < 0 || s.Slice(0, newline).IndexOfAnyExcept(@"\r\n ".AsSpan()) >= 0 then
                            Some NewOffsidesOnFirstLine
                        else
                            None

                    let newText =
                        match txt, sourceText with
                        | ShouldPutSpaceBefore & ShouldPutSpaceAfter, _ -> " " + txt[1 .. txt.Length - 2] + " "
                        | ShouldPutSpaceBefore, _ -> " " + txt[1 .. txt.Length - 2]
                        | ShouldPutSpaceAfter, _ -> txt[1 .. txt.Length - 2] + " "
                        | NewOffsidesOnFirstLine,
                          ContainsSensitiveIndentation context.Span & (HasPrecedingConstructOnSameLine context.Span | FollowingLineMovesOffsidesRightward context.Span) ->
                            txt[ 1 .. txt.Length - 2 ].Replace("\n ", "\n")
                        | NewOffsidesOnFirstLine, ContainsSensitiveIndentation context.Span -> " " + txt[1 .. txt.Length - 2]
                        | _ -> txt[1 .. txt.Length - 2]

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
