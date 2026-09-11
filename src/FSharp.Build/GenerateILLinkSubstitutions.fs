// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.IO
open System.Text
open System.Xml.Linq
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

    member val EmbeddedResources = [||]: ITaskItem[] with get, set

    [<Output>]
    member val ReplacedItems = [||]: ITaskItem[] with get, set

    override this.Execute() =
        try
            let substitutionsName = "ILLink.Substitutions.xml"

            let existing =
                this.EmbeddedResources
                |> Array.filter (fun item ->
                    let logicalName = item.GetMetadata("LogicalName")

                    let name =
                        if logicalName = "" then
                            item.GetMetadata("ManifestResourceName")
                        else
                            logicalName

                    String.Equals(name, substitutionsName, StringComparison.OrdinalIgnoreCase))

            if existing.Length > 1 then
                invalidOp $"Only one embedded {substitutionsName} resource is supported."

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
            let assemblyName = System.Security.SecurityElement.Escape this.AssemblyName
            sb.AppendLine($"  <assembly fullname=\"{assemblyName}\">") |> ignore

            // Add each resource entry with proper closing tag on the same line
            for prefix in resourcePrefixes do
                sb.AppendLine($"    <resource name=\"{prefix}.{assemblyName}\" action=\"remove\"></resource>")
                |> ignore

            // Close assembly and linker tags
            sb.AppendLine("  </assembly>") |> ignore
            sb.AppendLine("</linker>") |> ignore

            let xmlContent =
                match existing with
                | [| resource |] ->
                    let document = XElement.Load(resource.ItemSpec, LoadOptions.PreserveWhitespace)

                    if document.Name <> XName.Get "linker" then
                        invalidOp $"{resource.ItemSpec} must have a <linker> root element."

                    document.Add(XElement.Parse(sb.ToString()).Elements())
                    document.ToString(SaveOptions.DisableFormatting)
                | _ -> sb.ToString()

            // Create a file in the intermediate output path
            let outputFileName = Path.Combine(this.IntermediateOutputPath, substitutionsName)

            Directory.CreateDirectory(this.IntermediateOutputPath) |> ignore

            if
                not (File.Exists outputFileName)
                || File.ReadAllText(outputFileName) <> xmlContent
            then
                File.WriteAllText(outputFileName, xmlContent)

            // Create a TaskItem for the generated file
            let item = TaskItem(outputFileName)

            if existing.Length = 1 then
                existing[0].CopyMetadataTo(item)

            item.SetMetadata("LogicalName", substitutionsName)

            this.GeneratedItems <- [| item :> ITaskItem |]
            this.ReplacedItems <- existing
            true
        with ex ->
            this.Log.LogErrorFromException(ex, true)
            false
