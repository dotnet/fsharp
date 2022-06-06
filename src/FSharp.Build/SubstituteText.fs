// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.IO
open Microsoft.Build.Framework

type SubstituteText() =

    let mutable _buildEngine: IBuildEngine MaybeNull = null
    let mutable _hostObject: ITaskHost MaybeNull = null

    let mutable copiedFiles = new ResizeArray<ITaskItem>()
    let mutable embeddedResources: ITaskItem[] = [||]

    [<Required>]
    member _.EmbeddedResources
        with get () = embeddedResources
        and set (value) = embeddedResources <- value

    [<Output>]
    member _.CopiedFiles = copiedFiles.ToArray()

    interface ITask with
        member _.BuildEngine
            with get () = _buildEngine
            and set (value) = _buildEngine <- value

        member _.HostObject
            with get () = _hostObject
            and set (value) = _hostObject <- value

        member _.Execute() =
            copiedFiles.Clear()

            if not (isNull embeddedResources) then
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
                                let mutable contents = File.ReadAllText(sourcePath)

                                if not (String.IsNullOrWhiteSpace(pattern1)) then
                                    let replacement = item.GetMetadata("Replacement1")
                                    contents <- contents.Replace(pattern1, replacement)

                                if not (String.IsNullOrWhiteSpace(pattern2)) then
                                    let replacement = item.GetMetadata("Replacement2")
                                    contents <- contents.Replace(pattern2, replacement)

                                let directory = Path.GetDirectoryName(targetPath)

                                if not (Directory.Exists(directory)) then
                                    Directory.CreateDirectory(directory) |> ignore

                                File.WriteAllText(targetPath, contents)
                            with _ ->
                                ()

                    copiedFiles.Add(item)

            true
