// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

/// Supported kinds of diagnostics by this service.
[<RequireQualifiedAccess>]
type FSharpDiagnosticKind =
    | AddIndexerDot
    | ReplaceWithSuggestion of suggestion:string

/// Exposes compiler diagnostic error messages.
module CompilerDiagnostics =

    /// Given a DiagnosticKind, returns the string representing the error message for that diagnostic.
    val GetErrorMessage: diagnosticKind: FSharpDiagnosticKind -> string