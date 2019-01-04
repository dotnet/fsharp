// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System.Collections.Generic

open Microsoft.FSharp.Compiler.ErrorResolutionHints

module ErrorResolutionHints =
    let getSuggestedNames (symbolUses: FSharpSymbolUse[]) (unresolvedIdentifier: string) =
        let candidates =
            symbolUses
            |> Array.map (fun s -> s.Symbol.DisplayName)
            |> HashSet<string>

        let res = FilterPredictions (fun () -> candidates) unresolvedIdentifier |> List.map snd
        match res with
        | [] -> None
        | _ -> Some res