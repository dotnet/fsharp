// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
open Microsoft.CodeAnalysis

module internal ProjectFiltering =

    /// #10227: Filters projects to those referencing a specific assembly file.
    /// Used to optimize Find All References for external DLL symbols.
    let getProjectsReferencingAssembly (assemblyFilePath: string) (solution: Solution) =
        let assemblyFileName = Path.GetFileName assemblyFilePath

        let sameFileName (path: string) =
            not (String.IsNullOrEmpty path)
            && String.Equals(Path.GetFileName path, assemblyFileName, StringComparison.OrdinalIgnoreCase)

        // A consumer references the copy in its own output rather than the file the producer writes, so the
        // name is what identifies the assembly. It stops identifying it once another project of the solution
        // produces one named the same, and then only the path the declaring project writes to will do.
        let nameIsAmbiguous =
            solution.Projects
            |> Seq.filter (fun project -> sameFileName project.OutputFilePath)
            |> Seq.truncate 2
            |> Seq.length > 1

        let isTheAssembly (path: string) =
            if nameIsAmbiguous then
                String.Equals(path, assemblyFilePath, StringComparison.OrdinalIgnoreCase)
            else
                sameFileName path

        solution.Projects
        |> Seq.filter (fun project ->
            project.MetadataReferences
            |> Seq.exists (fun metaRef ->
                match metaRef with
                | :? PortableExecutableReference as peRef -> isTheAssembly peRef.FilePath
                | _ -> false))
        |> Seq.toList
