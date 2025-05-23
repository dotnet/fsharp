// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.IO
open System.Text
open Microsoft.Build.Framework
open Microsoft.Build.Utilities

/// <summary>
/// MSBuild task that generates ILLink.Substitutions.xml file to remove F# metadata resources during IL linking.
/// </summary>
type GenerateILLinkSubstitutions() =
    inherit Task()

    let mutable _assemblyName = ""
    let mutable _intermediateOutputPath = ""
    let mutable _generatedItems: ITaskItem[] = [||]

    /// <summary>
    /// Assembly name to use when generating resource names to be removed.
    /// </summary>
    [<Required>]
    member _.AssemblyName
        with get () = _assemblyName
        and set value = _assemblyName <- value

    /// <summary>
    /// Intermediate output path for storing the generated file.
    /// </summary>
    [<Required>]
    member _.IntermediateOutputPath
        with get () = _intermediateOutputPath
        and set value = _intermediateOutputPath <- value

    /// <summary>
    /// Generated embedded resource items.
    /// </summary>
    [<Output>]
    member _.GeneratedItems
        with get () = _generatedItems
        and set value = _generatedItems <- value

    override this.Execute() =
        try
            // Define the resource prefixes that need to be removed
            let resourcePrefixes = [|
                "FSharpSignatureData"
                "FSharpSignatureDataB"
                "FSharpSignatureCompressedData"
                "FSharpSignatureCompressedDataB"
                "FSharpOptimizationData"
                "FSharpOptimizationDataB"
                "FSharpOptimizationCompressedData"
                "FSharpOptimizationCompressedDataB"
                "FSharpOptimizationInfo"
                "FSharpSignatureInfo"
            |]

            // Generate the XML content
            let sb = StringBuilder()
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>") |> ignore
            sb.AppendLine("<linker>") |> ignore
            sb.AppendLine($"  <assembly fullname=\"{_assemblyName}\">") |> ignore
            
            // Add each resource entry
            for prefix in resourcePrefixes do
                sb.AppendLine($"    <resource name=\"{prefix}.{_assemblyName}\" action=\"remove\">") |> ignore
            
            // Close all resource tags
            for _ in resourcePrefixes do
                sb.Append("</resource>") |> ignore
            
            // Close assembly and linker tags
            sb.AppendLine("</assembly>") |> ignore
            sb.AppendLine("</linker>") |> ignore
            
            let xmlContent = sb.ToString()
            
            // Create a file in the intermediate output path
            let outputFileName = Path.Combine(_intermediateOutputPath, "ILLink.Substitutions.xml")
            Directory.CreateDirectory(_intermediateOutputPath) |> ignore
            File.WriteAllText(outputFileName, xmlContent)
            
            // Create a TaskItem for the generated file
            let item = TaskItem(outputFileName) :> ITaskItem
            item.SetMetadata("LogicalName", "ILLink.Substitutions.xml")
            
            _generatedItems <- [| item |]
            true
        with ex ->
            this.Log.LogErrorFromException(ex, true)
            false