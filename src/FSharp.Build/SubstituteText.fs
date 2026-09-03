// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.IO
open Microsoft.Build.Framework
open Microsoft.Build.Utilities

[<MSBuildMultiThreadableTask>]
type SubstituteText(taskEnvironment: TaskEnvironment) =
    inherit Task()

    let mutable copiedFiles = new ResizeArray<ITaskItem>()
    let mutable embeddedResources: ITaskItem[] = [||]
    let mutable _taskEnvironment = taskEnvironment

    // Every File/Directory API consuming a (possibly relative) path must go through this so paths
    // are resolved against this task instance's TaskEnvironment rather than the ambient process
    // current directory. Original relative strings are preserved for item.ItemSpec.
    let rootedPath (path: string) =
        _taskEnvironment.GetAbsolutePath(path).Value

    new() = SubstituteText(TaskEnvironment.Fallback)

    interface IMultiThreadableTask with
        member _.TaskEnvironment
            with get () = _taskEnvironment
            and set (value) = _taskEnvironment <- value

    [<Required>]
    member _.EmbeddedResources
        with get () = embeddedResources
        and set (value) = embeddedResources <- value

    [<Output>]
    member _.CopiedFiles = copiedFiles.ToArray()

    override _.Execute() =
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
                            let mutable contents = File.ReadAllText(rootedPath sourcePath)

                            if not (String.IsNullOrWhiteSpace(pattern1)) then
                                let replacement = item.GetMetadata("Replacement1")
                                contents <- contents.Replace(pattern1, replacement)

                            if not (String.IsNullOrWhiteSpace(pattern2)) then
                                let replacement = item.GetMetadata("Replacement2")
                                contents <- contents.Replace(pattern2, replacement)

                            let directory = Path.GetDirectoryName(targetPath)

                            if not (Directory.Exists(rootedPath directory)) then
                                Directory.CreateDirectory(rootedPath directory) |> ignore

                            File.WriteAllText(rootedPath targetPath, contents)
                        with _ ->
                            ()

                copiedFiles.Add(item)

        true
