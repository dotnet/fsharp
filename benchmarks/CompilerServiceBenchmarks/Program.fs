open System
open System.IO
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Text
open System.Text
open Microsoft.CodeAnalysis.Text

module private SourceText =

    open System.Runtime.CompilerServices

    let weakTable = ConditionalWeakTable<SourceText, ISourceText>()

    let create (sourceText: SourceText) =

        let sourceText =
            { new ISourceText with
            
                member __.Item with get index = sourceText.[index]

                member __.GetLineString(lineIndex) =
                    sourceText.Lines.[lineIndex].ToString()

                member __.GetLineCount() =
                    sourceText.Lines.Count

                member __.GetLastCharacterPosition() =
                    if sourceText.Lines.Count > 0 then
                        (sourceText.Lines.Count, sourceText.Lines.[sourceText.Lines.Count - 1].Span.Length)
                    else
                        (0, 0)

                member __.GetSubTextString(start, length) =
                    sourceText.GetSubText(TextSpan(start, length)).ToString()

                member __.SubTextEquals(target, startIndex) =
                    if startIndex < 0 || startIndex >= sourceText.Length then
                        raise (ArgumentOutOfRangeException("startIndex"))

                    if String.IsNullOrEmpty(target) then
                        raise (ArgumentException("Target is null or empty.", "target"))

                    let lastIndex = startIndex + target.Length
                    if lastIndex <= startIndex || lastIndex >= sourceText.Length then
                        raise (ArgumentException("Target is too big.", "target"))

                    let mutable finished = false
                    let mutable didEqual = true
                    let mutable i = 0
                    while not finished && i < target.Length do
                        if target.[i] <> sourceText.[startIndex + i] then
                            didEqual <- false
                            finished <- true // bail out early                        
                        else
                            i <- i + 1

                    didEqual

                member __.ContentEquals(sourceText) =
                    match sourceText with
                    | :? SourceText as sourceText -> sourceText.ContentEquals(sourceText)
                    | _ -> false

                member __.Length = sourceText.Length

                member __.CopyTo(sourceIndex, destination, destinationIndex, count) =
                    sourceText.CopyTo(sourceIndex, destination, destinationIndex, count)
            }

        sourceText

type SourceText with

    member this.ToFSharpSourceText() =
        SourceText.weakTable.GetValue(this, Runtime.CompilerServices.ConditionalWeakTable<_,_>.CreateValueCallback(SourceText.create))

[<ClrJob; MemoryDiagnoser>]
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
            sourceOpt <- Some <| SourceText.From(File.OpenRead("""..\..\..\..\..\src\fsharp\TypeChecker.fs"""), Encoding.Default, SourceHashAlgorithm.Sha1, true)
        | _ -> ()
    
    [<IterationSetup>]
    member __.ParsingSetup() =
        match checkerOpt with
        | None -> failwith "no checker"
        | Some(checker) ->
            checker.InvalidateAll()
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            checker.ParseFile("dummy.fs", SourceText.ofString "dummy", parsingOptions) |> Async.RunSynchronously |> ignore

    [<Benchmark>]
    member __.Parsing() =
        match checkerOpt, sourceOpt with
        | None, _ -> failwith "no checker"
        | _, None -> failwith "no source"
        | Some(checker), Some(source) ->
            let results = checker.ParseFile("TypeChecker.fs", source.ToFSharpSourceText(), parsingOptions) |> Async.RunSynchronously
            if results.ParseHadErrors then failwithf "parse had errors: %A" results.Errors

[<EntryPoint>]
let main argv =
    let _ = BenchmarkRunner.Run<CompilerServiceParsing>()
    0