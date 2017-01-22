open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open System
open System.IO
open LanguageServiceProfiling
open System.Diagnostics
open System.Collections.Generic

[<EntryPoint>]
let main argv = 
    let rootDir = argv.[0]
    let options = Options.FCS rootDir
    let checker = FSharpChecker.Create()
    let waste = new ResizeArray<int array>()
    let checkProject() =
        printfn "ParseAndCheckProject(FCS)..."
        let sw = Stopwatch.StartNew()
        let result = checker.ParseAndCheckProject(options) |> Async.RunSynchronously
        if result.HasCriticalErrors then
            printfn "Finished with ERRORS: %+A" result.Errors
        else printfn "Finished successfully in %O" sw.Elapsed
        ()

    let checkFile (relativePath: string) (fileVersion: int) =
        printfn "ParseAndCheckFileInProject(%s)..." relativePath
        let filePath = Path.Combine(rootDir, relativePath)
        let sw = Stopwatch.StartNew()
        let _, answer = checker.ParseAndCheckFileInProject(filePath, fileVersion, File.ReadAllText filePath, options) |> Async.RunSynchronously
        match answer with
        | FSharpCheckFileAnswer.Aborted ->
            printfn "Abortedin %O!" sw.Elapsed
        | FSharpCheckFileAnswer.Succeeded results ->
            if results.Errors |> Array.exists (fun x -> x.Severity = FSharpErrorSeverity.Error) then
                printfn "Finished with ERRORS in %O: %+A" sw.Elapsed results.Errors
            else printfn "Finished successfully in %O" sw.Elapsed
    
    let wasteMemory () =
        waste.Add(Array.zeroCreate (1024 * 1024 * 25))

    printfn """Press:

<C> for check the project
<G> for GC.Collect(2)
<F> for check TypeChecker.fs (you need to change it before rechecking)
<W> for wasting 100M of memory
<M> to reclaim waste <Enter> for exit."""

    let rec loop (fileVersion: int) =
        match Console.ReadKey().Key with
        | ConsoleKey.C -> 
            checkProject()
            loop fileVersion
        | ConsoleKey.G -> 
            printfn "GC is running..."
            let sw = Stopwatch.StartNew()
            GC.Collect 2
            printfn "GC is done in %O" sw.Elapsed
            loop fileVersion
        | ConsoleKey.F ->
            checkFile @"src\fsharp\TypeChecker.fs" fileVersion
            loop (fileVersion + 1)
        | ConsoleKey.W ->
            wasteMemory()
            loop fileVersion
        | ConsoleKey.M ->
            waste.Clear()
            loop fileVersion
        | ConsoleKey.Enter -> ()
        | _ -> loop fileVersion
    loop 0
    0