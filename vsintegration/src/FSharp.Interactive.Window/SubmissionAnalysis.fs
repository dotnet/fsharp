// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Collections.Generic

open FSharp.Compiler.Tokenization

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

    let private tokenizer = FSharpSourceTokenizer([], Some "stdin.fsx", None)

    type private Scan =
        {
            /// Outside strings and comments.
            OpenBrackets: int
            InsideMultiLineConstruct: bool
            LastToken: string option
            EndsWithTerminator: bool
        }

    [<Literal>]
    let private probeIdentifier = "__fsharp_interactive_probe__"

    let private scan (text: string) =
        let lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n')

        let mutable state = FSharpTokenizerLexState.Initial
        let mutable openBrackets = 0
        let mutable lastToken = None

        let scanLine (line: string) (record: bool) =
            let lineTokenizer = tokenizer.CreateLineTokenizer line
            let mutable firstColor = None
            let mutable scanning = true

            while scanning do
                match lineTokenizer.ScanToken state with
                | Some token, nextState ->
                    state <- nextState

                    if firstColor.IsNone then
                        firstColor <- Some token.ColorClass

                    if record then
                        match token.ColorClass with
                        | FSharpTokenColorKind.Comment
                        | FSharpTokenColorKind.InactiveCode -> ()
                        | FSharpTokenColorKind.String ->
                            // A literal ends a submission as a number would, but its contents are
                            // not code: brackets and terminators inside it must not count.
                            lastToken <- Some "\"\""
                        | _ ->
                            let value =
                                if token.LeftColumn >= 0 && token.LeftColumn + token.FullMatchedLength <= line.Length then
                                    line.Substring(token.LeftColumn, token.FullMatchedLength)
                                else
                                    ""

                            if not (String.IsNullOrWhiteSpace value) then
                                if opening.Contains value then
                                    openBrackets <- openBrackets + 1
                                elif closing.Contains value then
                                    openBrackets <- openBrackets - 1

                                lastToken <- Some value
                | None, nextState ->
                    state <- nextState
                    scanning <- false

            firstColor

        for line in lines do
            scanLine line true |> ignore

        // The lexer state carries more than "inside a string or comment", so comparing it against
        // the initial state says nothing. Tokenizing an identifier with the state the text left
        // behind does: inside an unterminated string or comment the probe comes back coloured as
        // part of that construct.
        let insideMultiLineConstruct =
            match scanLine probeIdentifier false with
            | Some FSharpTokenColorKind.String
            | Some FSharpTokenColorKind.Comment
            | Some FSharpTokenColorKind.InactiveCode -> true
            | _ -> false

        {
            OpenBrackets = openBrackets
            InsideMultiLineConstruct = insideMultiLineConstruct
            LastToken = lastToken
            EndsWithTerminator =
                match lastToken with
                | Some token -> String.Equals(token, ";;", StringComparison.Ordinal)
                | None -> false
        }

    let endsWithTerminator (text: string) = (scan text).EndsWithTerminator

    /// An explicit `;;` always submits; without one, the submission goes when nothing is visibly
    /// left open.
    let isComplete (text: string) =
        if String.IsNullOrWhiteSpace text then
            // The window submits an empty one to start a session.
            true
        else
            let scanned = scan text

            if scanned.EndsWithTerminator then
                true
            elif scanned.InsideMultiLineConstruct || scanned.OpenBrackets > 0 then
                false
            else
                match scanned.LastToken with
                | Some token when continuationTokens.Contains token -> false
                | _ -> true

    let withTerminator (text: string) =
        if endsWithTerminator text then text else text + "\n;;"
