open System
open System.IO
open System.Text
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open CodeAnalysis.Text
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

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

[<MemoryDiagnoser>]
type CompilerService() =

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

    let mutable assembliesOpt = None

    let readerOptions =
        {
            pdbDirPath = None
            ilGlobals = mkILGlobals ILScopeRef.Local
            reduceMemoryUsage = ReduceMemoryFlag.No
            metadataOnly = MetadataOnlyFlag.Yes
            tryGetMetadataSnapshot = fun _ -> None
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

        match assembliesOpt with
        | None -> 
            assembliesOpt <- 
                System.AppDomain.CurrentDomain.GetAssemblies()
                |> Array.map (fun x -> (x.Location))
                |> Some
        | _ -> ()
    
    [<IterationSetup(Target = "Parsing")>]
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

    [<IterationSetup(Target = "ILReading")>]
    member __.ILReadingSetup() =
        // With caching, performance increases an order of magnitude when re-reading an ILModuleReader.
        // Clear it for benchmarking.
        ClearAllILModuleReaderCache()

    [<Benchmark>]
    member __.ILReading() =
        match assembliesOpt with
        | None -> failwith "no assemblies"
        | Some(assemblies) ->
            // We try to read most of everything in the assembly that matter, mainly types with their properties, methods, and fields.
            // CustomAttrs and SecurityDecls are lazy until you call them, so we call them here for benchmarking.
            assemblies
            |> Array.iter (fun (fileName) ->
                let reader = OpenILModuleReader fileName readerOptions

                let ilModuleDef = reader.ILModuleDef

                let ilAssemblyManifest = ilModuleDef.Manifest.Value

                ilAssemblyManifest.CustomAttrs |> ignore
                ilAssemblyManifest.SecurityDecls |> ignore
                ilAssemblyManifest.ExportedTypes.AsList
                |> List.iter (fun x ->
                   x.CustomAttrs |> ignore
                )

                ilModuleDef.CustomAttrs |> ignore
                ilModuleDef.TypeDefs.AsArray
                |> Array.iter (fun ilTypeDef ->
                    ilTypeDef.CustomAttrs |> ignore
                    ilTypeDef.SecurityDecls |> ignore

                    ilTypeDef.Methods.AsArray
                    |> Array.iter (fun ilMethodDef ->
                        ilMethodDef.CustomAttrs |> ignore
                        ilMethodDef.SecurityDecls |> ignore
                    )

                    ilTypeDef.Fields.AsList
                    |> List.iter (fun ilFieldDef ->
                        ilFieldDef.CustomAttrs |> ignore
                    )

                    ilTypeDef.Properties.AsList
                    |> List.iter (fun ilPropertyDef ->
                        ilPropertyDef.CustomAttrs |> ignore
                    )
                )
            )

[<EntryPoint>]
let main argv =
    let _ = BenchmarkRunner.Run<CompilerService>()
    0
