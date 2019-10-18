// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

/// Supported kinds of diagnostics by this service.
type DiagnosticKind =
    | AddIndexerDot
    | ReplaceWithSuggestion of suggestion:string

/// Exposes compiler diagnostic error messages.
module CompilerDiagnostics =
    /// Given a DiagnosticKind, returns the string representing the error message for that diagnostic.
    val getErrorMessage: diagnosticKind: DiagnosticKind -> string