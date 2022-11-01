/// Allows extracting necessary data from a sequence of project source files 
module FSharp.Compiler.Service.Tests.FileInfoGathering

open System.Collections.Generic
open FSharp.Compiler.Service.Tests.Types
open FSharp.Compiler.Service.Tests.Utils
open FSharp.Compiler.Service.Tests2
open FSharp.Compiler.Syntax

let internal gatherBackingInfo (files : SourceFiles) : Files =
    let seenSigFiles = HashSet<string>()
    files
    |> Array.mapi (fun i f ->
        let fsiBacked =
            match f.AST with
            | ParsedInput.SigFile _ ->
                // TODO Use QualifiedNameOfFile
                seenSigFiles.Add f.AST.FileName |> ignore
                false
            | ParsedInput.ImplFile _ ->
                let fsiName = System.IO.Path.ChangeExtension(f.QualifiedName, "fsi")
                let fsiBacked = seenSigFiles.Contains fsiName
                fsiBacked
        {
            Idx = FileIdx.make i
            Code = "no code here" // TODO
            AST = ASTOrX.AST f.AST
            FsiBacked = fsiBacked
        }
    )
    
type ExtractedData =
    {
        /// Order of the file in the project. Files with lower number cannot depend on files with higher number
        Tops : LongIdent[]
        ContainsModuleAbbreviations : bool
        /// All partial module references found in this file's AST
        ModuleRefs : LongIdent[]
    }
    
/// All the data about a single file needed for the dependency resolution algorithm
type FileData =
    {
        File : File
        Data : ExtractedData
    }
    with member this.CodeSize = this.File.CodeSize

let private gatherFileData (ast : ParsedInput) : ExtractedData =
    let moduleRefs, containsModuleAbbreviations = ASTVisit.findModuleRefs ast
    let tops = ASTVisit.topModuleOrNamespaces ast
    // TODO As a perf optimisation we can skip top-level ids scanning for FsiBacked .fs files
    // However, it is unlikely to give a noticable speedup due to parallelism (citation needed)
    {
        ModuleRefs = moduleRefs
        Tops = tops
        ContainsModuleAbbreviations = containsModuleAbbreviations
    }

/// Extract necessary information from all files in parallel - top-level items and all (partial) module references
let gatherForAllFiles (files : SourceFiles) =
    let files = gatherBackingInfo files
    let nodes =
        files
        // TODO Proper async with cancellation
        |> Array.Parallel.map (fun f ->
            let ast = match f.AST with ASTOrX.AST ast -> ast | X _ -> failwith "Unexpected X item"
            let data = gatherFileData ast
            {
                File = f
                Data = data
            }
        )
    nodes
