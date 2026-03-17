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

        solution.Projects
        |> Seq.filter (fun project ->
            project.MetadataReferences
            |> Seq.exists (fun metaRef ->
                match metaRef with
                | :? PortableExecutableReference as peRef when not (isNull peRef.FilePath) ->
                    let refFileName = Path.GetFileName(peRef.FilePath)
                    String.Equals(refFileName, assemblyFileName, StringComparison.OrdinalIgnoreCase)
                | _ -> false))
        |> Seq.toList
