// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
open Microsoft.CodeAnalysis

module internal ProjectFiltering =

    /// #10227: Filters projects to those referencing a specific assembly file.
    /// Used to optimize Find All References for external DLL symbols.
    let getProjectsReferencingAssembly (assemblyFilePath: string) (solution: Solution) =
        let assemblyFileName = Path.GetFileName(assemblyFilePath)

        let sameFileName (path: string) =
            not (String.IsNullOrEmpty path)
            && String.Equals(Path.GetFileName path, assemblyFileName, StringComparison.OrdinalIgnoreCase)

        let samePath (path: string) (other: string) =
            not (String.IsNullOrEmpty path)
            && String.Equals(path, other, StringComparison.OrdinalIgnoreCase)

        let outputsPerProjectFile =
            solution.Projects
            |> Seq.filter (fun project -> sameFileName project.OutputFilePath)
            |> Seq.groupBy _.FilePath
            |> Seq.map (fun (_, targets) ->
                targets
                |> Seq.collect (fun target -> [ target.OutputFilePath; target.OutputRefFilePath ])
                |> Seq.toArray)
            |> Seq.toArray

        // Consumers reference copies of the assembly, so its file name identifies it unless several project files produce that name.
        let isTheAssembly =
            if outputsPerProjectFile.Length < 2 then
                sameFileName
            else
                let declaringOutputs =
                    outputsPerProjectFile
                    |> Array.tryFindV (Array.exists (samePath assemblyFilePath))
                    |> ValueOption.defaultValue [| assemblyFilePath |]

                fun path -> declaringOutputs |> Array.exists (samePath path)

        solution.Projects
        |> Seq.filter (fun project ->
            project.MetadataReferences
            |> Seq.exists (fun metaRef ->
                match metaRef with
                | :? PortableExecutableReference as peRef -> isTheAssembly peRef.FilePath
                | _ -> false))
        |> Seq.toList
