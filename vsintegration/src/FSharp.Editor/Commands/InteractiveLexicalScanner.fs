// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.Tokenization

open Microsoft.VisualStudio.FSharp.Interactive

type internal FSharpLexicalScanner() =

    static let tokenizer = FSharpSourceTokenizer([], Some "stdin.fsx", None)

    static let probeIdentifier = "__fsharp_interactive_probe__"

    let mutable state = FSharpTokenizerLexState.Initial

    static let kindOf color =
        match color with
        | FSharpTokenColorKind.Keyword -> LexicalKind.Keyword
        | FSharpTokenColorKind.Comment -> LexicalKind.Comment
        | FSharpTokenColorKind.String -> LexicalKind.String
        | FSharpTokenColorKind.Number -> LexicalKind.Number
        | FSharpTokenColorKind.Operator -> LexicalKind.Operator
        | FSharpTokenColorKind.Identifier
        | FSharpTokenColorKind.UpperIdentifier -> LexicalKind.Identifier
        | FSharpTokenColorKind.PreprocessorKeyword -> LexicalKind.PreprocessorKeyword
        | FSharpTokenColorKind.InactiveCode -> LexicalKind.InactiveCode
        | _ -> LexicalKind.Other

    interface ILexicalScanner with

        member _.ScanLine(line, tokens) =
            let lineTokenizer = tokenizer.CreateLineTokenizer line
            let mutable scanning = true

            while scanning do
                let token, nextState = lineTokenizer.ScanToken state
                state <- nextState

                match token with
                | Some token ->
                    tokens.Add
                        {
                            Kind = kindOf token.ColorClass
                            Start = token.LeftColumn
                            Length = token.FullMatchedLength
                        }
                | None -> scanning <- false

        member _.EndsInsideMultiLineConstruct =
            match (tokenizer.CreateLineTokenizer probeIdentifier).ScanToken state with
            | Some token, _ ->
                match token.ColorClass with
                | FSharpTokenColorKind.String
                | FSharpTokenColorKind.Comment
                | FSharpTokenColorKind.InactiveCode -> true
                | _ -> false
            | None, _ -> false

type internal FSharpLexicalScannerFactory() =

    interface ILexicalScannerFactory with
        member _.CreateScanner() =
            FSharpLexicalScanner() :> ILexicalScanner
