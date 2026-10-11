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

        let samePath (path: string) (other: string) =
            String.Equals(path, other, StringComparison.OrdinalIgnoreCase)

        // The output paths of each project that produces an assembly of this name. The target frameworks of a
        // multi-targeted project are projects of their own that share its project file: they are one producer,
        // writing the assembly once per target.
        let producers =
            solution.Projects
            |> Seq.filter (fun project -> sameFileName project.OutputFilePath)
            |> Seq.groupBy _.FilePath
            |> Seq.map (fun (_, targets) -> targets |> Seq.map _.OutputFilePath |> Seq.toArray)
            |> Seq.toArray

        // A consumer references the copy in its own output rather than the file the producer writes, so the
        // name is what identifies the assembly. It stops identifying it once another project of the solution
        // produces one named the same, and then only the paths the declaring project writes to will do.
        let nameIsAmbiguous = producers.Length > 1

        let declaringOutputs =
            producers
            |> Array.tryFindV (Array.exists (samePath assemblyFilePath))
            |> ValueOption.defaultValue [| assemblyFilePath |]

        let isTheAssembly (path: string) =
            if nameIsAmbiguous then
                declaringOutputs |> Array.exists (samePath path)
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
