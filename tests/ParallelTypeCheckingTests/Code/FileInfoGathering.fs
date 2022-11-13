/// Allows extracting necessary data from a sequence of project source files
module ParallelTypeCheckingTests.FileInfoGathering

open System.Collections.Generic
open ParallelTypeCheckingTests.Types
open ParallelTypeCheckingTests.Utils
open ParallelTypeCheckingTests
open FSharp.Compiler.Syntax

let internal gatherBackingInfo (files: SourceFiles) : Files =
    let seenSigFiles = HashSet<string>()

    files
    |> Array.mapi (fun i f ->
        let fsiBacked =
            match f.AST with
            | ParsedInput.SigFile _ ->
                seenSigFiles.Add f.QualifiedName |> ignore
                false
            | ParsedInput.ImplFile _ ->
                let fsiBacked = seenSigFiles.Contains f.QualifiedName
                fsiBacked

        {
            Idx = FileIdx.make i
            AST = ASTOrFsix.AST f.AST
            FsiBacked = fsiBacked
        })

type ExtractedData =
    {
        Tops: SimpleId[]
        Abbreviations: Abbreviation[]
        /// All partial module references found in this file's AST
        ModuleRefs: SimpleId[]
    }

/// All the data about a single file needed for the dependency resolution algorithm
type FileData =
    {
        File: File
        Data: ExtractedData
    }

    member this.CodeSize = this.File.CodeSize

let private gatherFileData (ast: ParsedInput) : ExtractedData =
    let moduleRefs, abbreviations = ASTVisit.findModuleRefs ast
    let tops = TopModulesExtraction.topModuleOrNamespaces ast
    // TODO As a perf optimisation we can skip top-level ids scanning for FsiBacked .fs files
    // However, it is unlikely to give a noticable speedup due to parallelism (citation needed)
    {
        ModuleRefs = moduleRefs
        Tops = tops
        Abbreviations = abbreviations
    }

/// Extract necessary information from all files in parallel - top-level items and all (partial) module references
let gatherForAllFiles (files: SourceFiles) =
    let files = gatherBackingInfo files

    let nodes =
        files
        // TODO Proper async with cancellation
        |> Array.Parallel.map (fun f ->
            let ast =
                match f.AST with
                | ASTOrFsix.AST ast -> ast
                | Fsix _ -> failwith "Unexpected X item"

            let data = gatherFileData ast
            { File = f; Data = data })

    nodes
