// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.ProjectSystem 

module internal SourceMovement =
    
    type Movement =
        | Add of string
        | Remove of string[]
        | Rename of (string * string) seq
        | Reorder
    
    /// Try to re-order/insert/remove files from 'oldSources' (i.e., what we
    /// last computed with ComputeSourcesAndFlags) based on our
    /// 'newCompileItems' (i.e., all descendants of the project node that are
    /// marked as 'Compile').
    /// 'oldSources' and 'newCompileItems' can sometimes mismatch due to
    /// MSBuild tasks/the compiler adding files to be compiled that aren't
    /// shown in the Solution Explorer. In the vast majority of cases, these
    /// additional sources are either prefixed or suffixed to the array that
    /// we get from the Solution Explorer.
    let newSources (oldSources : string[]) (newCompileItems : string[]) (movement : Movement) : string[] option =

        let normalisePath (source: string) =
            try System.IO.Path.GetFullPath (source.ToLowerInvariant())
            with _ -> source.ToLowerInvariant()
        
        let renamer =
            match movement with
            | Rename names ->
                let normalisedNewNameMap =
                    names
                    |> Seq.map (fun (oldName, newName) ->
                        normalisePath oldName, normalisePath newName
                    )
                    |> Map.ofSeq

                fun oldName ->
                    match normalisedNewNameMap |> Map.tryFind oldName with
                    | None -> oldName
                    | Some newName -> newName

            | _ -> id

        let oldSources =
            oldSources
            |> Array.map (normalisePath >> renamer)

        let newCompileItems = newCompileItems |> Array.map normalisePath

        let extra =
            match movement with
            | Remove s -> s |> Array.map normalisePath
            | _ -> [||]

        let searchTargets = Array.append newCompileItems extra

        let prefixed =
            oldSources
            |> Array.takeWhile (fun s -> not (Array.contains s searchTargets))
        
        let suffixed =
            oldSources
            |> Array.skip prefixed.Length
            |> Array.skipWhile (fun s -> Array.contains s searchTargets)

        let newSources =
            [|
                yield! prefixed
                yield! newCompileItems
                yield! suffixed
            |]

        let lengthDelta =
            match movement with
            | Add _ -> 1
            | Remove files -> -files.Length
            | Rename _ | Reorder -> 0

        if prefixed.Length + suffixed.Length = oldSources.Length then
            None
        elif newSources.Length = oldSources.Length + lengthDelta then
            Some (Array.ofSeq newSources)
        else
            None
