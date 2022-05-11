// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Diagnostics

open FSharp.Compiler.DiagnosticResolutionHints

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

    let GetSuggestedNames (suggestionsF: FSharp.Compiler.DiagnosticsLogger.Suggestions) (unresolvedIdentifier: string) =
        let buffer = SuggestionBuffer(unresolvedIdentifier)
        if buffer.Disabled then
            Seq.empty
        else
            suggestionsF buffer.Add
            buffer :> seq<string>