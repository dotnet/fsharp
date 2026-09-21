// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Collections.Generic

/// Decides whether pressing Enter submits what the user has typed or adds another line.
///
/// The window asks on every Enter, on the UI thread, so the judgement is lexical rather than a
/// type check.
module internal SubmissionAnalysis =

    let private ordinalSet (items: string seq) = HashSet<string>(items, StringComparer.Ordinal)

    /// Tokens after which more input is always expected.
    let private continuationTokens =
        ordinalSet
            [
                "="
                "->"
                "<-"
                ":"
                ","
                ";"
                "|"
                "||"
                "&&"
                "+"
                "-"
                "*"
                "/"
                "%"
                "**"
                "@"
                "^"
                "|>"
                "<|"
                ">>"
                "<<"
                "then"
                "else"
                "elif"
                "do"
                "try"
                "with"
                "finally"
                "function"
                "fun"
                "begin"
                "match"
                "if"
                "let"
                "use"
                "and"
                "or"
                "in"
                "when"
                "as"
                "of"
                "new"
                "static"
                "member"
                "override"
                "abstract"
                "type"
                "module"
                "namespace"
                "open"
                "rec"
                "mutable"
                "yield"
                "return"
                "->>"
            ]

    let private opening = ordinalSet [ "("; "["; "{"; "[|"; "[<"; "{|" ]

    let private closing = ordinalSet [ ")"; "]"; "}"; "|]"; ">]"; "|}" ]

    let private (|Opening|Closing|Ordinary|) token =
        if opening.Contains token then Opening
        elif closing.Contains token then Closing
        else Ordinary

    type private Scan =
        {
            /// Outside strings and comments.
            OpenBrackets: int
            InsideMultiLineConstruct: bool
            LastToken: string voption
            EndsWithTerminator: bool
        }

    let private scan (scanners: ILexicalScannerFactory) (text: string) =
        let lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n')

        let scanner = scanners.CreateScanner()
        let tokens = ResizeArray<LexicalToken>()
        let mutable openBrackets = 0
        let mutable lastToken = ValueNone

        for line in lines do
            tokens.Clear()
            scanner.ScanLine(line, tokens)

            for token in tokens do
                match token.Kind with
                | LexicalKind.Comment
                | LexicalKind.InactiveCode -> ()
                | LexicalKind.String ->
                    // A literal ends a submission as a number would, but its contents are
                    // not code: brackets and terminators inside it must not count.
                    lastToken <- ValueSome "\"\""
                | _ ->
                    let value =
                        if token.Start >= 0 && token.Start + token.Length <= line.Length then
                            line.Substring(token.Start, token.Length)
                        else
                            ""

                    if not (String.IsNullOrWhiteSpace value) then
                        match value with
                        | Opening -> openBrackets <- openBrackets + 1
                        | Closing -> openBrackets <- openBrackets - 1
                        | Ordinary -> ()

                        lastToken <- ValueSome value

        {
            OpenBrackets = openBrackets
            InsideMultiLineConstruct = scanner.EndsInsideMultiLineConstruct
            LastToken = lastToken
            EndsWithTerminator =
                match lastToken with
                | ValueSome token -> String.Equals(token, ";;", StringComparison.Ordinal)
                | ValueNone -> false
        }

    let endsWithTerminator (scanners: ILexicalScannerFactory) (text: string) =
        (scan scanners text).EndsWithTerminator

    /// An explicit `;;` always submits; without one, the submission goes when nothing is visibly
    /// left open.
    let isComplete (scanners: ILexicalScannerFactory) (text: string) =
        if String.IsNullOrWhiteSpace text then
            // The window submits an empty one to start a session.
            true
        else
            match scan scanners text with
            | { EndsWithTerminator = true } -> true
            | { InsideMultiLineConstruct = true } -> false
            | scanned when scanned.OpenBrackets > 0 -> false
            | { LastToken = ValueSome token } -> not (continuationTokens.Contains token)
            | _ -> true

    let withTerminator (scanners: ILexicalScannerFactory) (text: string) =
        if endsWithTerminator scanners text then text else $"{text}\n;;"
