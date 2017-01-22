open Microsoft.FSharp.Compiler.SourceCodeServices
open System
open LanguageServiceProfiling
open System.Diagnostics
open System.Collections.Generic

[<EntryPoint>]
let main argv = 
    let options = Options.FCS argv.[0]
    let checker = FSharpChecker.Create()
    let waste = new ResizeArray<int array>()
    let check() =
        printfn "ParseAndCheckProject(FCS)..."
        let sw = Stopwatch.StartNew()
        let result = checker.ParseAndCheckProject(options) |> Async.RunSynchronously
        if result.HasCriticalErrors then
            printfn "Finished with ERRORS: %+A" result.Errors
        else printfn "Finished successfully in %O" sw.Elapsed
        ()
    
    let wasteMemory () =
        waste.Add(Array.zeroCreate (1024 * 1024 * 25))

    printfn "Press <C> for check the project, <G> for GC.Collect(2), <W> for wasting memory, <M> to reclaim waste <Enter> for exit."
    let rec loop() =
        match Console.ReadKey().Key with
        | ConsoleKey.C -> 
            check()
            loop()
        | ConsoleKey.G -> 
            printfn "GC is running..."
            let sw = Stopwatch.StartNew()
            GC.Collect 2
            printfn "GC is done in %O" sw.Elapsed
            loop()
        | ConsoleKey.W ->
            wasteMemory()
        | ConsoleKey.M ->
            waste.Clear()
        | ConsoleKey.Enter -> ()
        | _ -> loop()
    loop()
    0