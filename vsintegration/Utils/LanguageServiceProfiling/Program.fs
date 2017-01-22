open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open System
open System.IO
open LanguageServiceProfiling
open System.Diagnostics
open System.Collections.Generic
// ---- taken from QuickInfoProviderTests.fs
let normalizeLineEnds (s: string) = s.Replace("\r\n", "\n").Replace("\n\n", "\n")
let internal getQuickInfoText (FSharpToolTipText.FSharpToolTipText elements) : string =
    let rec parseElement = function
        | FSharpToolTipElement.None -> ""
        | FSharpToolTipElement.Single(text, _) -> text
        | FSharpToolTipElement.SingleParameter(text, _, _) -> text
        | FSharpToolTipElement.Group(xs) -> xs |> List.map fst |> String.concat "\n"
        | FSharpToolTipElement.CompositionError(error) -> error
    elements |> List.map (parseElement) |> String.concat "\n" |> normalizeLineEnds

[<EntryPoint>]
let main argv = 
    let rootDir = argv.[0]
    let options = Options.get rootDir
    let getFileLines () = File.ReadAllLines(options.FileToCheck)
    let getFileText () = File.ReadAllText(options.FileToCheck)
    let getLine line = (getFileLines ()).[line]

    printfn "Found options for %s." options.Options.ProjectFileName
    let checker = FSharpChecker.Create()
    let waste = new ResizeArray<int array>()
    
    let checkProject() : Async<FSharpCheckProjectResults option> =
        async {
            printfn "ParseAndCheckProject(%s)..." (Path.GetFileName options.Options.ProjectFileName)
            let sw = Stopwatch.StartNew()
            let! result = checker.ParseAndCheckProject(options.Options)
            if result.HasCriticalErrors then
                printfn "Finished with ERRORS: %+A" result.Errors
                return None
            else 
                printfn "Finished successfully in %O" sw.Elapsed
                return Some result
        }

    let checkFile (fileVersion: int) : Async<FSharpCheckFileResults option> =
        async {
            printfn "ParseAndCheckFileInProject(%s)..." options.FileToCheck
            let sw = Stopwatch.StartNew()
            let! _, answer = checker.ParseAndCheckFileInProject(options.FileToCheck, fileVersion, File.ReadAllText options.FileToCheck, options.Options)
            match answer with
            | FSharpCheckFileAnswer.Aborted ->
                printfn "Abortedin %O!" sw.Elapsed
                return None
            | FSharpCheckFileAnswer.Succeeded results ->
                if results.Errors |> Array.exists (fun x -> x.Severity = FSharpErrorSeverity.Error) then
                    printfn "Finished with ERRORS in %O: %+A" sw.Elapsed results.Errors
                    return None
                else 
                    printfn "Finished successfully in %O" sw.Elapsed
                    return Some results
        }
    
    let findAllReferences (fileVersion: int) : Async<FSharpSymbolUse[]> =
        async {
            printfn "Find all references (symbol = '%s', file = '%s')" options.SymbolText options.FileToCheck
            let! projectResults = checkProject()
            match projectResults with
            | Some projectResults ->
                let! fileResults = checkFile fileVersion
                match fileResults with
                | Some fileResults ->
                    let! symbolUse =
                        fileResults.GetSymbolUseAtLocation(
                            options.SymbolPos.Line, 
                            options.SymbolPos.Column, 
                            getLine(options.SymbolPos.Line),
                            [options.SymbolText])
                    match symbolUse with
                    | Some symbolUse ->
                        printfn "Found symbol %s" symbolUse.Symbol.FullName
                        let sw = Stopwatch.StartNew()
                        let! symbolUses = projectResults.GetUsesOfSymbol(symbolUse.Symbol)
                        printfn "Found %d symbol uses in %O" symbolUses.Length sw.Elapsed
                        return symbolUses
                    | None -> 
                        printfn "Symbol '%s' was not found at (%d, %d) in %s" options.SymbolText options.SymbolPos.Line options.SymbolPos.Column options.FileToCheck
                        return [||]
                | None -> 
                    printfn "No file check results for %s" options.FileToCheck
                    return [||]
             | None -> 
                printfn "No project results for %s" options.Options.ProjectFileName
                return [||]
        }
    let getDeclarations (fileVersion: int) =
        async {
            let! projectResults = checkProject()
            match projectResults with
            | Some projectResults ->
                let! fileResults = checkFile fileVersion
                match fileResults with
                | Some fileResults ->
                    let! parseResult = checker.ParseFileInProject(options.FileToCheck, getFileText(), options.Options) 
                    for completion in options.CompletionPositions do
                        printfn "querying %A %s" completion.QualifyingNames completion.PartialName
                        let! listInfo =
                            fileResults.GetDeclarationListInfo(
                                Some parseResult
                                , completion.Position.Line
                                , completion.Position.Column
                                , getLine (completion.Position.Line)
                                , completion.QualifyingNames
                                , completion.PartialName)
                           
                        for i in listInfo.Items do
                            printfn "%s" (getQuickInfoText i.DescriptionText)

                | None -> printfn "no declarations"
            | None -> printfn "no declarations"
        }
    
    let wasteMemory () =
        waste.Add(Array.zeroCreate (1024 * 1024 * 25))

    printfn """Press:

<C> for check the project
<G> for GC.Collect(2)
<F> for check TypeChecker.fs (you need to change it before rechecking)
<R> for find all references
<L> for completion lists
<W> for wasting 100M of memory
<M> to reclaim waste
<Enter> for exit."""

    let rec loop (fileVersion: int) =
        match Console.ReadKey().Key with
        | ConsoleKey.C -> 
            checkProject() |> Async.RunSynchronously |> ignore
            loop fileVersion
        | ConsoleKey.G -> 
            printfn "GC is running..."
            let sw = Stopwatch.StartNew()
            GC.Collect 2
            printfn "GC is done in %O" sw.Elapsed
            loop fileVersion
        | ConsoleKey.F ->
            checkFile fileVersion |> Async.RunSynchronously |> ignore
            loop (fileVersion + 1)
        | ConsoleKey.R ->
            findAllReferences fileVersion |> Async.RunSynchronously |> ignore
            loop fileVersion
        | ConsoleKey.L ->
            getDeclarations fileVersion |> Async.RunSynchronously
            loop fileVersion
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