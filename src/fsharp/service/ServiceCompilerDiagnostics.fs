// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

[<RequireQualifiedAccess>]
type FSharpDiagnosticKind =
    | AddIndexerDot
    | ReplaceWithSuggestion of suggestion:string

[<RequireQualifiedAccess>]
module CompilerDiagnostics =

    let GetErrorMessage diagnosticKind =
        match diagnosticKind with
        | FSharpDiagnosticKind.AddIndexerDot -> FSComp.SR.addIndexerDot()
        | FSharpDiagnosticKind.ReplaceWithSuggestion s -> FSComp.SR.replaceWithSuggestion(s)