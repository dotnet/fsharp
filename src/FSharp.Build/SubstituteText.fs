// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.IO
open Microsoft.Build.Framework
open Microsoft.Build.Utilities

[<MSBuildMultiThreadableTask>]
type SubstituteText() =
    inherit Task()

    let mutable copiedFiles = new ResizeArray<ITaskItem>()
    let mutable embeddedResources: ITaskItem[] = [||]

    interface IMultiThreadableTask with
        member val TaskEnvironment = TaskEnvironment.Fallback with get, set

    [<Required>]
    member _.EmbeddedResources
        with get () = embeddedResources
        and set (value) = embeddedResources <- value

    [<Output>]
    member _.CopiedFiles = copiedFiles.ToArray()

    override this.Execute() =
        let rootedPath = TaskEnvironmentPaths.rootedPath this
        copiedFiles.Clear()

        if not (isNull (box embeddedResources)) then // this check can't fail, the type is non-nullable
            for item in embeddedResources do
                // Update ITaskItem metadata to point to new location
                let sourcePath = item.GetMetadata("FullPath")

                let pattern1 = item.GetMetadata("Pattern1")
                let pattern2 = item.GetMetadata("Pattern2")

                // Is there any replacement to do?
                if not (String.IsNullOrWhiteSpace(pattern1) && String.IsNullOrWhiteSpace(pattern2)) then
                    if not (String.IsNullOrWhiteSpace(sourcePath)) then
                        try
                            let getTargetPathFrom key =
                                let md = item.GetMetadata(key)
                                let path = Path.GetDirectoryName(md)
                                let fileName = Path.GetFileName(md)
                                let target = Path.Combine(path, @"..\resources", fileName)
                                target

                            // Copy from the location specified in Identity
                            let sourcePath = item.GetMetadata("Identity")

                            // Copy to the location specified in TargetPath unless no TargetPath is provided, then use Identity
                            let targetPath =
                                let identityPath = getTargetPathFrom "Identity"
                                let intermediateTargetPath = item.GetMetadata("IntermediateTargetPath")

                                if not (String.IsNullOrWhiteSpace(intermediateTargetPath)) then
                                    let fileName = Path.GetFileName(identityPath)
                                    let target = Path.Combine(intermediateTargetPath, fileName)
                                    target
                                else
                                    identityPath

                            item.ItemSpec <- targetPath

                            // Transform file
                            let replaceFromMetadata pattern replacementName (contents: string) =
                                if String.IsNullOrWhiteSpace pattern then
                                    contents
                                else
                                    contents.Replace(pattern, item.GetMetadata replacementName)

                            let contents =
                                File.ReadAllText(rootedPath sourcePath)
                                |> replaceFromMetadata pattern1 "Replacement1"
                                |> replaceFromMetadata pattern2 "Replacement2"

                            let directory = Path.GetDirectoryName(targetPath)
                            let rootedDirectory = rootedPath directory

                            if not (Directory.Exists rootedDirectory) then
                                Directory.CreateDirectory rootedDirectory |> ignore

                            File.WriteAllText(rootedPath targetPath, contents)
                        with _ ->
                            ()

                copiedFiles.Add(item)

        true
