// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System.Collections.Generic

open FSharp.Compiler.ErrorResolutionHints

module ErrorResolutionHints =
    let getSuggestedNames (namesToCheck: string[]) (unresolvedIdentifier: string) =
        let res = FilterPredictions (fun () -> HashSet<string>(namesToCheck)) unresolvedIdentifier |> List.map snd
        match res with
        | [] -> None
        | _ -> Some res