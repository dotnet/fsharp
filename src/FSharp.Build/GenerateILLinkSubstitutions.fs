// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System.IO
open FSharp.Compiler
open Microsoft.Build.Framework
open Microsoft.Build.Utilities

/// <summary>
/// MSBuild task that generates ILLink.Substitutions.xml file to remove F# metadata resources during IL linking.
/// </summary>
type GenerateILLinkSubstitutions() =
    inherit Task()

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
        try
            let xmlContent =
                ILLinkSubstitutions.document this.AssemblyName (ILLinkSubstitutions.names this.AssemblyName)
                |> string

            let outputFileName =
                Path.Combine(this.IntermediateOutputPath, "ILLink.Substitutions.xml")

            Directory.CreateDirectory(this.IntermediateOutputPath) |> ignore

            if
                not (File.Exists outputFileName)
                || File.ReadAllText(outputFileName) <> xmlContent
            then
                File.WriteAllText(outputFileName, xmlContent)

            let item = TaskItem(outputFileName.Replace("%", "%25")) :> ITaskItem
            item.SetMetadata("LogicalName", "ILLink.Substitutions.xml")
            item.SetMetadata("Type", "Non-Resx")
            item.SetMetadata("WithCulture", "false")
            this.GeneratedItems <- [| item |]
            true
        with ex ->
            this.Log.LogErrorFromException(ex, true)
            false
