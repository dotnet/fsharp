open Microsoft.FSharp.Compiler.SourceCodeServices
open System
open LanguageServiceProfiling
open System.Diagnostics

[<EntryPoint>]
let main argv = 
    let options = Options.FCS argv.[0]
    let checker = FSharpChecker.Create()

    let check() =
        printfn "ParseAndCheckProject(FCS)..."
        let sw = Stopwatch.StartNew()
        let result = checker.ParseAndCheckProject(options) |> Async.RunSynchronously
        if result.HasCriticalErrors then
            printfn "Finished with ERRORS: %+A" result.Errors
        else printfn "Finished successfully in %O" sw.Elapsed
        ()
    
    printfn "Press <C> for check the project, <G> for GC.Collect(2), <Enter> for exit."
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
        | ConsoleKey.Enter -> ()
        | _ -> loop()
    loop()
    0