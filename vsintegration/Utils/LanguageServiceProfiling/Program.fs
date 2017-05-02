(*

Background check a project "CXGSCGS" (Check, Flush, GC, Stats, Check, GC, Stats)

  .\Release\net40\bin\LanguageServiceProfiling.exe ..\FSharp.Compiler.Service CXGSCGS 

Foreground check new versions of multiple files in an already-checked project and keep intellisense info "CGSFGS" (Check, GC, Stats, File, GC, Stats)

  .\Release\net40\bin\LanguageServiceProfiling.exe ..\FSharp.Compiler.Service CGSFGS 



Use the following to collect memory usage stats, appending to the existing stats files:

  git clone http://github.com/fsharp/FSharp.Compiler.Service  tests\scripts\tmp\FSharp.Compiler.Service
  pushd tests\scripts\tmp\FSharp.Compiler.Service
  git checkout  2d51df21ca1d86d4d6676ead3b1fb125b2a0d1ba 
  .\build Build.NetFx
  popd

  git rev-parse HEAD > gitrev.txt
  set /P gitrev=<gitrev.txt

  echo %gitrev%

  .\Release\net40\bin\LanguageServiceProfiling.exe ..\FSharp.Compiler.Service SCGS   %gitrev% >> tests\scripts\service-fcs-project-check-mem-results.txt
  .\Release\net40\bin\LanguageServiceProfiling.exe ..\FSharp.Compiler.Service CGSFGS %gitrev% >> tests\scripts\service-fcs-file-check-mem-results.txt

Results look like this:

  Background FCS project:
    master:     TimeDelta: 24.70     MemDelta:  318     G0:  880     G1:  696     G2:    8

  Background FCS project (with keepAllBackgroundResolutions=true) :
    master:     TimeDelta: 23.84     MemDelta:  448     G0:  877     G1:  702     G2:    6

  Multiple foreground files from FCS project keeping all results:
    statistics:     TimeDelta: 8.58     MemDelta:  143     G0:  260     G1:  192     G2:    4



*)

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
        | FSharpToolTipElement.Group(xs) -> xs |> List.map (fun item -> item.MainDescription) |> String.concat "\n"
        | FSharpToolTipElement.CompositionError(error) -> error
    elements |> List.map (parseElement) |> String.concat "\n" |> normalizeLineEnds

[<EntryPoint>]
let main argv = 
    let rootDir = argv.[0]
    let scriptOpt = if argv.Length >= 2 then Some argv.[1] else None
    let msgOpt = if argv.Length >= 3 then Some argv.[2] else None
    let useConsole = scriptOpt.IsNone
    let options = Options.get rootDir
    let getFileLines () = File.ReadAllLines(options.FileToCheck)
    let getFileText () = File.ReadAllText(options.FileToCheck)
    let getLine line = (getFileLines ()).[line]

    eprintfn "Found options for %s." options.Options.ProjectFileName
    let checker = FSharpChecker.Create(projectCacheSize = 200, keepAllBackgroundResolutions = false)
    let waste = new ResizeArray<int array>()
    
    let checkProject() : Async<FSharpCheckProjectResults option> =
        async {
            eprintfn "ParseAndCheckProject(%s)..." (Path.GetFileName options.Options.ProjectFileName)
            let sw = Stopwatch.StartNew()
            let! result = checker.ParseAndCheckProject(options.Options)
            if result.HasCriticalErrors then
                eprintfn "Finished with ERRORS: %+A" result.Errors
                return None
            else 
                eprintfn "Finished successfully in %O" sw.Elapsed
                return Some result
        }

    let checkFile (fileVersion: int) : Async<FSharpCheckFileResults option> =
        async {
            eprintfn "ParseAndCheckFileInProject(%s)..." options.FileToCheck
            let sw = Stopwatch.StartNew()
            let! _, answer = checker.ParseAndCheckFileInProject(options.FileToCheck, fileVersion, File.ReadAllText options.FileToCheck, options.Options)
            match answer with
            | FSharpCheckFileAnswer.Aborted ->
                eprintfn "Abortedin %O!" sw.Elapsed
                return None
            | FSharpCheckFileAnswer.Succeeded results ->
                if results.Errors |> Array.exists (fun x -> x.Severity = FSharpErrorSeverity.Error) then
                    eprintfn "Finished with ERRORS in %O: %+A" sw.Elapsed results.Errors
                    return None
                else 
                    eprintfn "Finished successfully in %O" sw.Elapsed
                    return Some results
        }
    
    let checkFiles (fileVersion: int) =
        async {
            eprintfn "multiple ParseAndCheckFileInProject(...)..." 
            let sw = Stopwatch.StartNew()
            let answers = 
               options.FilesToCheck |> List.map (fun file -> 
                   eprintfn "doing %s" file
                   checker.ParseAndCheckFileInProject(file, fileVersion, File.ReadAllText file, options.Options) |> Async.RunSynchronously)
            for _,answer in answers do 
                match answer with
                | FSharpCheckFileAnswer.Aborted ->
                    eprintfn "Aborted!" 
                | FSharpCheckFileAnswer.Succeeded results ->
                    if results.Errors |> Array.exists (fun x -> x.Severity = FSharpErrorSeverity.Error) then
                        eprintfn "Finished with ERRORS: %+A" results.Errors
            eprintfn "Finished in %O" sw.Elapsed
        }
    
    let findAllReferences (fileVersion: int) : Async<FSharpSymbolUse[]> =
        async {
            eprintfn "Find all references (symbol = '%s', file = '%s')" options.SymbolText options.FileToCheck
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
                        eprintfn "Found symbol %s" symbolUse.Symbol.FullName
                        let sw = Stopwatch.StartNew()
                        let! symbolUses = projectResults.GetUsesOfSymbol(symbolUse.Symbol)
                        eprintfn "Found %d symbol uses in %O" symbolUses.Length sw.Elapsed
                        return symbolUses
                    | None -> 
                        eprintfn "Symbol '%s' was not found at (%d, %d) in %s" options.SymbolText options.SymbolPos.Line options.SymbolPos.Column options.FileToCheck
                        return [||]
                | None -> 
                    eprintfn "No file check results for %s" options.FileToCheck
                    return [||]
             | None -> 
                eprintfn "No project results for %s" options.Options.ProjectFileName
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
                        eprintfn "querying %A %s" completion.QualifyingNames completion.PartialName
                        let! listInfo =
                            fileResults.GetDeclarationListInfo(
                                Some parseResult,
                                completion.Position.Line,
                                completion.Position.Column,
                                getLine (completion.Position.Line),
                                completion.QualifyingNames,
                                completion.PartialName,
                                fun() -> [])
                           
                        for i in listInfo.Items do
                            eprintfn "%s" (getQuickInfoText i.DescriptionText)

                | None -> eprintfn "no declarations"
            | None -> eprintfn "no declarations"
        }
    
    let wasteMemory () =
        waste.Add(Array.zeroCreate (1024 * 1024 * 25))

    if useConsole then 
        eprintfn """Press:

<C> for check the project
<G> for GC.Collect(2)
<F> for check TypeChecker.fs (you need to change it before rechecking)
<R> for find all references
<L> for completion lists
<W> for wasting 100M of memory
<X> to clear all caches
<P> to pause for key entry
<M> to reclaim waste
<Enter> for exit."""

    let mutable tPrev = None
    let stats() = 
        // Note that timing calls are relatively expensive on the startup path so we don't
        // make this call unless showTimes has been turned on.
        let uproc = System.Diagnostics.Process.GetCurrentProcess()
        let timeNow = uproc.UserProcessorTime.TotalSeconds
        let maxGen = System.GC.MaxGeneration
        let gcNow = [| for i in 0 .. maxGen -> System.GC.CollectionCount(i) |]
        let wsNow = uproc.WorkingSet64/1000000L

        match tPrev with
        | Some (timePrev, gcPrev:int[], wsPrev)->
            let spanGC = [| for i in 0 .. maxGen -> System.GC.CollectionCount(i) - gcPrev.[i] |]
            printfn "%s     TimeDelta: %4.2f     MemDelta: %4d     G0: %4d     G1: %4d     G2: %4d" 
                (match msgOpt with Some msg -> msg | None -> "statistics:")
                (timeNow - timePrev) 
                (wsNow - wsPrev)
                spanGC.[min 0 maxGen] spanGC.[min 1 maxGen] spanGC.[min 2 maxGen]

        | _ -> ()
        tPrev <- Some (timeNow, gcNow, wsNow)

    let processCmd (fileVersion: int) c =
        match c with 
        | 'C' -> 
            checkProject() |> Async.RunSynchronously |> ignore
            fileVersion
        | 'G' -> 
            eprintfn "GC is running..."
            let sw = Stopwatch.StartNew()
            GC.Collect 2
            eprintfn "GC is done in %O" sw.Elapsed
            fileVersion
        | 'F' ->
            checkFiles fileVersion |> Async.RunSynchronously |> ignore
            (fileVersion + 1)
        | 'R' ->
            findAllReferences fileVersion |> Async.RunSynchronously |> ignore
            fileVersion
        | 'L' ->
            getDeclarations fileVersion |> Async.RunSynchronously
            fileVersion
        | 'W' ->
            wasteMemory()
            fileVersion
        | 'M' ->
            waste.Clear()
            fileVersion
        | 'S' ->
            stats()
            fileVersion
        | 'X' ->
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients() 
            fileVersion
        | 'P' ->
            eprintfn "pausing (press any key)...";
            System.Console.ReadKey() |> ignore
            fileVersion
        | _ -> fileVersion

    let rec console fileVersion = 
        match Console.ReadKey().Key with
        | ConsoleKey.C -> processCmd fileVersion 'C' |> console
        | ConsoleKey.G -> processCmd fileVersion 'G' |> console
        | ConsoleKey.F -> processCmd fileVersion 'F' |> console
        | ConsoleKey.R -> processCmd fileVersion 'R' |> console
        | ConsoleKey.L -> processCmd fileVersion 'L' |> console
        | ConsoleKey.W -> processCmd fileVersion 'W' |> console            
        | ConsoleKey.M -> processCmd fileVersion 'M' |> console
        | ConsoleKey.X -> processCmd fileVersion 'X' |> console
        | ConsoleKey.P -> processCmd fileVersion 'P' |> console
        | ConsoleKey.Enter -> ()
        | _ -> console fileVersion

    let runScript (script:string)  = 
        (0,script) ||> Seq.fold processCmd |> ignore

    match scriptOpt with 
    | None ->  console 0
    | Some s -> runScript  s
    0