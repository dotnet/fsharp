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
open BenchmarkDotNet.Order
open BenchmarkDotNet.Mathematics

[<AutoOpen>]
module private CascadeProjectHelpers =
        
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
let myFunc%i{number} () = myFunc%i{number-1}()
//$COMMENTAREA$"""

[<MemoryDiagnoser>]
[<LongRunJob>]
[<Orderer(SummaryOrderPolicy.FastestToSlowest)>]
[<RankColumn(NumeralSystem.Roman)>]
type FileCascadeBenchmarks() = 
    let mutable project : FSharpProjectOptions option = None

    let getProject() = project.Value
    
    let checker = FSharpChecker.Create(projectCacheSize = 5, enableParallelCheckingWithSignatureFiles = true, parallelReferenceResolution = true)
    let filesToCreate = 64

    let mutable finalFileContents = SourceText.ofString ""
        
    [<GlobalSetup>]
    member _.Setup() =
        let projectFolder = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),"CascadeBenchmarkProject"))                    
        if(projectFolder.Exists) then
            do projectFolder.Delete(recursive=true)
        do Directory.CreateDirectory(projectFolder.FullName) |> ignore

        let inProjectFolder fileName = Path.Combine(projectFolder.FullName,fileName)

        File.WriteAllText(inProjectFolder "Benchmark0.fs",baselineModule)
        for i = 1 to filesToCreate do
            File.WriteAllText(inProjectFolder $"Benchmark%i{i}.fs", generateSourceCode i)

        let dllFileName = inProjectFolder "CascadingBenchMark.dll"
        let allSourceCodeFiles = [| for i in 0 .. filesToCreate -> inProjectFolder $"Benchmark%i{i}.fs"|]
        let x = createProject allSourceCodeFiles dllFileName
        project <- Some x
        finalFileContents <- generateSourceCode filesToCreate |> SourceText.ofString


    member x.ChangeFile(fileIndex:int, action) = 
        let project = getProject()
        let fileName = project.SourceFiles.[fileIndex]
        let fullOriginalSource = File.ReadAllText(fileName)
        try
            File.WriteAllText(fileName, fullOriginalSource.Replace("$COMMENTAREA$","$FILEMODIFIED"))
            action()
        finally
            File.WriteAllText(fileName,fullOriginalSource)

    member x.CheckFinalFile () =
        let project = getProject()
        let lastFile = project.SourceFiles |> Array.last
        checker.ParseAndCheckFileInProject(lastFile,999,finalFileContents,project)
               
    [<Benchmark>]
    member x.ParseProjectAsIs() =
        x.CheckFinalFile()

    [<Benchmark(Baseline=true)>]
    member x.ParseProjectWithFullCacheClear() =
        checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
        checker.ClearCache([getProject()])
        checker.InvalidateConfiguration(getProject())
        x.CheckFinalFile()

    [<Benchmark>]
    member x.ParseProjectWithChangingFirstFile() =
        x.ChangeFile(fileIndex=0, action= fun () -> x.CheckFinalFile())  

    [<Benchmark>]
    member x.ParseProjectWithChanging25thPercentileFile() =
        x.ChangeFile(fileIndex=filesToCreate/4, action= fun () -> x.CheckFinalFile())  

    [<Benchmark>]
    member x.ParseProjectWithChangingMiddleFile() =
        x.ChangeFile(fileIndex=filesToCreate/2, action= fun () -> x.CheckFinalFile())  