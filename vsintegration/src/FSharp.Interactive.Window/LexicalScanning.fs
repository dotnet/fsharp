// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

[<RequireQualifiedAccess>]
type internal LexicalKind =
    | Keyword
    | Comment
    | String
    | Number
    | Operator
    | Identifier
    | PreprocessorKeyword
    | InactiveCode
    | Other

[<Struct>]
type internal LexicalToken =
    {
        Kind: LexicalKind
        Start: int
        Length: int
    }

    member token.IsWithin(line: string) =
        token.Start >= 0 && token.Start + token.Length <= line.Length

type internal ILexicalScanner =

    abstract ScanLine: line: string * tokens: ResizeArray<LexicalToken> -> unit

    abstract EndsInsideMultiLineConstruct: bool

type internal ILexicalScannerFactory =
    abstract CreateScanner: unit -> ILexicalScanner
