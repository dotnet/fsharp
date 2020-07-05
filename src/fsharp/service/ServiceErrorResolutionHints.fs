// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler.ErrorResolutionHints

module ErrorResolutionHints =

    let getSuggestedNames (suggestionsF: FSharp.Compiler.ErrorLogger.Suggestions) (unresolvedIdentifier: string) =
        let buffer = SuggestionBuffer(unresolvedIdentifier)
        if buffer.Disabled then
            Seq.empty
        else
            suggestionsF buffer.Add
            buffer :> seq<string>