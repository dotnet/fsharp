// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System.IO
open FSharp.Compiler
open Microsoft.Build.Framework
open Microsoft.Build.Utilities

/// <summary>
/// MSBuild task that generates ILLink.Substitutions.xml file to remove F# metadata resources during IL linking.
/// </summary>
[<MSBuildMultiThreadableTask>]
type GenerateILLinkSubstitutions() =
    inherit Task()

    interface IMultiThreadableTask with
        member val TaskEnvironment = TaskEnvironment.Fallback with get, set

    /// <summary>
    /// Assembly name to use when generating resource names to be removed.
    /// </summary>
    [<Required>]
    member val AssemblyName = "" with get, set

    /// <summary>
    /// Intermediate output path for storing the generated file.
    /// </summary>
    [<Required>]
    member val IntermediateOutputPath = "" with get, set

    /// <summary>
    /// Generated embedded resource items.
    /// </summary>
    [<Output>]
    member val GeneratedItems = [||]: ITaskItem[] with get, set

    override this.Execute() =
        let rootedPath = TaskEnvironmentPaths.rootedPath this

        try
            let xmlContent =
                ILLinkSubstitutions.document this.AssemblyName (ILLinkSubstitutions.names this.AssemblyName)
                |> string

            let outputFileName =
                Path.Combine(this.IntermediateOutputPath, "ILLink.Substitutions.xml")

            Directory.CreateDirectory(rootedPath this.IntermediateOutputPath) |> ignore

            let outputPath = rootedPath outputFileName

            if not (File.Exists outputPath) || File.ReadAllText(outputPath) <> xmlContent then
                File.WriteAllText(outputPath, xmlContent)

            let item = TaskItem(outputFileName.Replace("%", "%25")) :> ITaskItem
            item.SetMetadata("LogicalName", "ILLink.Substitutions.xml")
            item.SetMetadata("Type", "Non-Resx")
            item.SetMetadata("WithCulture", "false")
            this.GeneratedItems <- [| item |]
            true
        with ex ->
            this.Log.LogErrorFromException(ex, true)
            false
