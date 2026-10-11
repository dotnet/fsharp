// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Collections.Generic

/// Whether Enter submits what was typed or adds another line. The window asks on the UI thread, on
/// every Enter, so the judgement is lexical.
module internal SubmissionAnalysis =

    let private ordinalSet (items: string seq) =
        HashSet<string>(items, StringComparer.Ordinal)

    let private continuationTokens =
        [
            "= -> <- : , ; | || && + - * / % ** @ ^ |> <| >> << ->>"
            "then else elif do try with finally function fun begin match if let use and or in when as of new"
            "static member override abstract type module namespace open rec mutable yield return"
        ]
        |> Seq.collect _.Split(' ')
        |> ordinalSet

    let private opening = ordinalSet [ "("; "["; "{"; "[|"; "[<"; "{|" ]

    let private closing = ordinalSet [ ")"; "]"; "}"; "|]"; ">]"; "|}" ]

    let private (|Opening|Closing|Ordinary|) token =
        if opening.Contains token then Opening
        elif closing.Contains token then Closing
        else Ordinary

    type private Scan =
        {
            OpenBrackets: int
            InsideMultiLineConstruct: bool
            LastToken: string voption
        }

        member scanned.EndsWithTerminator =
            scanned.LastToken
            |> ValueOption.exists (fun token -> String.Equals(token, ";;", StringComparison.Ordinal))

    let private scan (scanners: ILexicalScannerFactory) (text: string) =
        let scanner = scanners.CreateScanner()
        let tokens = ResizeArray<LexicalToken>()
        let mutable openBrackets = 0
        let mutable lastToken = ValueNone

        for line in text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n') do
            tokens.Clear()
            scanner.ScanLine(line, tokens)

            for token in tokens do
                match token.Kind with
                | LexicalKind.Comment
                | LexicalKind.InactiveCode -> ()
                // Stands for any literal: its contents are not code, so the brackets and
                // terminators inside it must not count.
                | LexicalKind.String -> lastToken <- ValueSome "\"\""
                | _ when token.IsWithin line ->
                    let value = line.Substring(token.Start, token.Length)

                    if not (String.IsNullOrWhiteSpace value) then
                        match value with
                        | Opening -> openBrackets <- openBrackets + 1
                        | Closing -> openBrackets <- openBrackets - 1
                        | Ordinary -> ()

                        lastToken <- ValueSome value
                | _ -> ()

        {
            OpenBrackets = openBrackets
            InsideMultiLineConstruct = scanner.EndsInsideMultiLineConstruct
            LastToken = lastToken
        }

    let isComplete (scanners: ILexicalScannerFactory) (text: string) =
        if String.IsNullOrWhiteSpace text then
            // The window submits an empty one to start a session.
            true
        else
            match scan scanners text with
            | scanned when scanned.EndsWithTerminator -> true
            | { InsideMultiLineConstruct = true } -> false
            | scanned when scanned.OpenBrackets > 0 -> false
            | { LastToken = ValueSome token } -> not (continuationTokens.Contains token)
            | _ -> true

    let withTerminator (scanners: ILexicalScannerFactory) (text: string) =
        if (scan scanners text).EndsWithTerminator then
            text
        else
            $"{text}\n;;"
