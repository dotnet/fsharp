#PARALLEL_COMPILATION_NAMEOFTHEGROUPLIKE_UNITTESTS
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
let myFunc0 () = returnValue
type MyType0 = MyType0 of string
type MyOtherType0 = MyOtherType0 of int"""

    let baselineFsi = 
        """
module Benchmark0

val returnValue: int

val myFunc0: unit -> int

type MyType0 = | MyType0 of string

type MyOtherType0 = | MyOtherType0 of int"""

    let generateSourceCode number =
        $"""
module Benchmark%i{number}
open Benchmark%i{number-1}
let myFunc%i{number} () = myFunc%i{number-1}()
type MyType{number} = MyType{number} of string
type MyOtherType{number} = MyOtherType{number} of int
type MyFunctionType{number} = MyType{number} -> MyOtherType{number}

let processFunc{number} (x) (func:MyFunctionType{number}) = 
    async {{
        return func(x)
    }}
//$COMMENTAREA$"""

/// Code create using FSharpCheckFileResults.GenerateSignature()
    let generateFsi number = 
        $"""
module Benchmark{number}

val myFunc{number}: unit -> int

type MyType{number} = | MyType6 of string

type MyOtherType{number} = | MyOtherType{number} of int

type MyFunctionType{number} = MyType{number} -> MyOtherType{number}

val processFunc{number}: x: MyType{number} -> func: MyFunctionType{number} -> Async<MyOtherType{number}>"""

[<MemoryDiagnoser>]
[<ShortRunJob>]
[<Orderer(SummaryOrderPolicy.FastestToSlowest)>]
[<RankColumn(NumeralSystem.Roman)>]
type FileCascadeBenchmarks() = 
    let mutable project : FSharpProjectOptions option = None   
    let filesToCreate = 128
    member val FinalFileContents = SourceText.ofString "" with get,set

    [<ParamsAllValues>]
    member val PartialCheck = true with get,set
    [<ParamsAllValues>]
    member val ParaChecking = true with get,set
    [<ParamsAllValues>]
    member val GenerateFSI = true with get,set

    member val Checker = Unchecked.defaultof<FSharpChecker> with get, set
    member this.GetProject() = project.Value
        
    [<GlobalSetup>]
    member this.Setup() =
        printfn $"Running Setup(). Partial = {this.PartialCheck}, Para = {this.ParaChecking}, FSIGen = {this.GenerateFSI}"
        this.Checker <- FSharpChecker.Create(
                projectCacheSize = 5, 
                enablePartialTypeChecking = this.PartialCheck,
                enableParallelCheckingWithSignatureFiles = this.ParaChecking)

        let projectFolder = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(),"CascadeBenchmarkProject"))                    
        if(projectFolder.Exists) then
            do projectFolder.Delete(recursive=true)
        do Directory.CreateDirectory(projectFolder.FullName) |> ignore

        let inProjectFolder fileName = Path.Combine(projectFolder.FullName,fileName)
        
        if this.GenerateFSI then
            File.WriteAllText(inProjectFolder "Benchmark0.fsi",baselineFsi)
        File.WriteAllText(inProjectFolder "Benchmark0.fs",baselineModule)

        for i = 1 to filesToCreate do            
            if this.GenerateFSI then
                File.WriteAllText(inProjectFolder $"Benchmark%i{i}.fsi", generateFsi i)
            File.WriteAllText(inProjectFolder $"Benchmark%i{i}.fs", generateSourceCode i)

        let dllFileName = inProjectFolder "CascadingBenchMark.dll"
        let allSourceCodeFiles = 
            [| for i in 0 .. filesToCreate do                
                if this.GenerateFSI then
                    yield (inProjectFolder $"Benchmark%i{i}.fsi")
                yield (inProjectFolder $"Benchmark%i{i}.fs")
            |]
        let x = createProject allSourceCodeFiles dllFileName
        project <- Some x
        this.FinalFileContents <- generateSourceCode filesToCreate |> SourceText.ofString                

        ()


    member x.ChangeFile(fileIndex:int, action) = 
        let project = x.GetProject()
        let fileIndexAdapted = if x.GenerateFSI then fileIndex * 2 else fileIndex
        let fileName = project.SourceFiles.[fileIndexAdapted]
        let fullOriginalSource = File.ReadAllText(fileName)
        try
            File.WriteAllText(fileName, fullOriginalSource.Replace("$COMMENTAREA$","$FILEMODIFIED"))
            action()
        finally
            File.WriteAllText(fileName,fullOriginalSource)

    member x.ParseAndCheckLastFileInTheProject () =
        let project = x.GetProject()
        let lastFile = project.SourceFiles |> Array.last
        let pr,cr = x.Checker.ParseAndCheckFileInProject(lastFile,999,x.FinalFileContents,project) |> Async.RunSynchronously
        printfn $"ParseError = {pr.ParseHadErrors}; Check = {match cr with | FSharpCheckFileAnswer.Succeeded _ -> '+' | _ -> '-' }"
        for e in pr.Diagnostics do
            printfn "Error:= %s" (e.ToString())
               
    [<Benchmark>]
    member x.ParseAndCheckLastFileProjectAsIs() =
        x.ParseAndCheckLastFileInTheProject()

    [<Benchmark(Baseline=true)>]
    member x.ParseProjectWithFullCacheClear() =
        x.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
        x.Checker.ClearCache([x.GetProject()])
        x.Checker.InvalidateConfiguration(x.GetProject())     
        x.ParseAndCheckLastFileInTheProject()

    [<Benchmark>]
    member x.ParseProjectWithChangingFirstFile() =
        x.ChangeFile(fileIndex=0, action= fun () -> x.ParseAndCheckLastFileInTheProject())  

    [<Benchmark>]
    member x.ParseProjectWithChangingMiddleFile() =
        x.ChangeFile(fileIndex=filesToCreate/2, action= fun () -> x.ParseAndCheckLastFileInTheProject())        

    [<Benchmark>]
    member x.ParseProjectWithChangingPenultimateFile() =
        x.ChangeFile(fileIndex=(filesToCreate-2), action= fun () -> x.ParseAndCheckLastFileInTheProject()) 