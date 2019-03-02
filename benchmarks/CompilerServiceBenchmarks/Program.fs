open System
open System.IO
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.SourceCodeServices
open System.Text

[<ClrJob(baseline = true)>]
type CompilerServiceParsing() =

    let mutable checkerOpt = None

    let mutable sourceOpt = None

    let parsingOptions =
        {
            SourceFiles = [|"TypeChecker.fs"|]
            ConditionalCompilationDefines = []
            ErrorSeverityOptions = FSharpErrorSeverityOptions.Default
            IsInteractive = false
            LightSyntax = None
            CompilingFsLib = false
            IsExe = false
        }

    [<GlobalSetup>]
    member __.Setup() =
        match checkerOpt with
        | None -> checkerOpt <- Some(FSharpChecker.Create())
        | _ -> ()

        match sourceOpt with
        | None ->
            let source = File.ReadAllText("""..\..\..\..\..\src\fsharp\TypeChecker.fs""")
            sourceOpt <- Some(source)
        | _ -> ()
    
    [<IterationSetup>]
    member __.ParsingSetup() =
        match checkerOpt with
        | None -> failwith "no checker"
        | Some(checker) ->
            checker.InvalidateAll()
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            checker.ParseFile("dummy.fs", "dummy", parsingOptions) |> Async.RunSynchronously |> ignore

    [<Benchmark>]
    member __.Parsing() =
        match checkerOpt, sourceOpt with
        | None, _ -> failwith "no checker"
        | _, None -> failwith "no source"
        | Some(checker), Some(source) ->
            let results = checker.ParseFile("TypeChecker.fs", source, parsingOptions) |> Async.RunSynchronously
            if results.ParseHadErrors then failwithf "parse had errors: %A" results.Errors

[<EntryPoint>]
let main argv =
    let _ = BenchmarkRunner.Run<CompilerServiceParsing>()
    0
