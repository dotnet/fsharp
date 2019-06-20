// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open Newtonsoft.Json.Linq
open Newtonsoft.Json

// Interfaces as defined at https://microsoft.github.io/language-server-protocol/specification.  The properties on
// these types are camlCased to match the underlying JSON properties to avoid attributes on every field:
//   [<JsonProperty("camlCased")>]

/// Represents a zero-based line and column of a text document.
type Position =
    { line: int
      character: int }

type Range =
    { start: Position
      ``end``: Position }

type DocumentUri = string

type Location =
    { uri: DocumentUri
      range: Range }

type DiagnosticRelatedInformation =
    { location: Location
      message: string }

type Diagnostic =
    { range: Range
      severity: int option
      code: string
      source: string option // "F#"
      message: string
      relatedInformation: DiagnosticRelatedInformation[] option }
    static member Error = 1
    static member Warning = 2
    static member Information = 3
    static member Hint = 4

type PublishDiagnosticsParams =
    { uri: DocumentUri
      diagnostics: Diagnostic[] }

type ClientCapabilities =
    { workspace: JToken option // TODO: WorkspaceClientCapabilities
      textDocument: JToken option // TODO: TextDocumentCapabilities
      experimental: JToken option
      supportsVisualStudioExtensions: bool option }

[<JsonConverter(typeof<JsonDUConverter>)>]
type Trace =
    | Off
    | Messages
    | Verbose

type WorkspaceFolder =
    { uri: DocumentUri
      name: string }

/// Note, this type has many more optional values that can be expanded as support is added.
type ServerCapabilities =
    { hoverProvider: bool }
    static member DefaultCapabilities() =
        { ServerCapabilities.hoverProvider = true }

type InitializeResult =
    { capabilities: ServerCapabilities }

[<JsonConverter(typeof<JsonDUConverter>)>]
type MarkupKind =
    | PlainText
    | Markdown

type MarkupContent =
    { kind: MarkupKind
      value: string }

type Hover =
    { contents: MarkupContent
      range: Range option }

type TextDocumentIdentifier =
    { uri: DocumentUri }
