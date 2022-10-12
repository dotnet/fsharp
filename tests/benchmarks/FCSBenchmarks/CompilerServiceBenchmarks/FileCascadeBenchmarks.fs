namespace FSharp.Compiler.Benchmarks

open System
open System.IO
open System.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open BenchmarkDotNet.Attributes
open FSharp.Compiler.Benchmarks
open Microsoft.CodeAnalysis.Text

[<AutoOpen>]
module private Helpers =
        
    let createProject files projectFilename =       
        {
            ProjectFileName = projectFilename
            ProjectId = None
            SourceFiles = files
            OtherOptions =  [|"--optimize+" |] 
            ReferencedProjects = [||]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = DateTime()
            UnresolvedReferences = None
            OriginalLoadReferences = []
            Stamp = Some 0L (* set the stamp to 0L on each run so we don't evaluate the whole project again *)
        }

    let baselineModule =
        $"""
module Benchmark0
let returnValue = 5
let myFunc0 () = 5"""

    let generateSourceCode number =
        $"""
module Benchmark%i{number}
open Benchmark%i{number-1}
let myFunc%i{number} () = myFunc%i{number-1}()"""

[<MemoryDiagnoser>]
type FileCascadeBenchmarks() = 
        let mutable project : FSharpProjectOptions option = None

        let filesToCreate = 100
        
        [<GlobalSetup>]
        member _.Setup() =
                match project with
                | Some p -> p
                | None ->
                    let projectFolder = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),"CascadeBenchmarkProject"))                    
                    if(projectFolder.Exists) then
                        do projectFolder.Delete(recursive=true)
                    do Directory.CreateDirectory(projectFolder.FullName) |> ignore

                    let inProjectFolder fileName = Path.Combine(projectFolder.FullName,fileName)

                    File.WriteAllText(inProjectFolder "Benchmark0.fs",baselineModule)
                    for i = 1 to filesToCreate do
                        File.WriteAllText(inProjectFolder $"Benchmark%i{i}.fs", generateSourceCode i)

                    let dllFileName = inProjectFolder "CascadingBenchMark.dll"
                    let allSourceCodeFiles = [| for i in 0 .. 100 -> inProjectFolder $"Benchmark%i{i}.fs"|]
                    let x = createProject allSourceCodeFiles dllFileName
                    project <- Some x
                    x
                


