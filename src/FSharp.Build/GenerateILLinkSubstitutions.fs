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
            // Define the resource prefixes that need to be removed
            let resourcePrefixes =
                [|
                    // Signature variants
                    yield!
                        [|
                            for dataType in [| "Data"; "DataB" |] do
                                for compression in [| ""; "Compressed" |] do
                                    yield $"FSharpSignature{compression}{dataType}"
                        |]

                    // Optimization variants
                    yield!
                        [|
                            for dataType in [| "Data"; "DataB" |] do
                                for compression in [| ""; "Compressed" |] do
                                    yield $"FSharpOptimization{compression}{dataType}"
                        |]

                    // Info variants
                    yield "FSharpOptimizationInfo"
                    yield "FSharpSignatureInfo"
                |]

            // Generate the XML content
            let sb = StringBuilder(4096) // pre-allocate capacity
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>") |> ignore
            sb.AppendLine("<linker>") |> ignore
            sb.AppendLine($"  <assembly fullname=\"{this.AssemblyName}\">") |> ignore

            // Add each resource entry with proper closing tag on the same line
            for prefix in resourcePrefixes do
                sb.AppendLine($"    <resource name=\"{prefix}.{this.AssemblyName}\" action=\"remove\"></resource>")
                |> ignore

            // Close assembly and linker tags
            sb.AppendLine("  </assembly>") |> ignore
            sb.AppendLine("</linker>") |> ignore

            let xmlContent = sb.ToString()

            // Create a file in the intermediate output path
            let outputFileName =
                Path.Combine(this.IntermediateOutputPath, "ILLink.Substitutions.xml")

            Directory.CreateDirectory(this.IntermediateOutputPath) |> ignore
            File.WriteAllText(outputFileName, xmlContent)

            // Create a TaskItem for the generated file
            let item = TaskItem(outputFileName) :> ITaskItem
            item.SetMetadata("LogicalName", "ILLink.Substitutions.xml")

            this.GeneratedItems <- [| item |]
            true
        with ex ->
            this.Log.LogErrorFromException(ex, true)
            false
