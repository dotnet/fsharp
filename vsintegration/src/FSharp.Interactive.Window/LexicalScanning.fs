// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

/// How the lexer classifies a token, reduced to what the window colours or reasons about.
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

/// A token on one line: where it starts and how many characters it spans.
[<Struct>]
type internal LexicalToken =
    {
        Kind: LexicalKind
        Start: int
        Length: int
    }

/// Scans text a line at a time, carrying the lexer's state from one line to the next.
type internal ILexicalScanner =

    /// Appends the tokens of the next line to <paramref name="tokens"/>.
    abstract ScanLine: line: string * tokens: ResizeArray<LexicalToken> -> unit

    /// Whether what has been scanned so far ends inside a string or comment that goes on into the
    /// next line.
    abstract EndsInsideMultiLineConstruct: bool

/// The source of scanners. The window colours text and judges when a submission is complete, but
/// leaves the lexer to the host: the F# tooling in Visual Studio already runs on one, and the
/// session's own compiler is the SDK's, started as a separate process.
type internal ILexicalScannerFactory =
    abstract CreateScanner: unit -> ILexicalScanner
