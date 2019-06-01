// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

type DiagnosticKind =
    | AddIndexerDot
    | ReplaceWithSuggestion of suggestion:string

[<RequireQualifiedAccess>]
module CompilerDiagnostics =
    let getErrorMessage diagnosticKind =
        match diagnosticKind with
        | AddIndexerDot -> FSComp.SR.addIndexerDot()
        | ReplaceWithSuggestion s -> FSComp.SR.replaceWithSuggestion(s)