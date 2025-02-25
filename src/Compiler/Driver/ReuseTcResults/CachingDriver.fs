module internal FSharp.Compiler.ReuseTcResults.CachingDriver

#nowarn "3261"

open System.IO

open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.GraphChecking
open FSharp.Compiler.IO
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.ReuseTcResults.TcResultsImport
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.ReuseTcResults.TcResultsPickle
open FSharp.Compiler.TypedTree

type TcCompilationData =
    {
        CmdLine: string array
        References: string array
    }

type GraphFileLine =
    {
        Index: int
        FileName: string
        Stamp: int64
    }

type Graph =
    {
        Files: GraphFileLine list
        Dependencies: string list
    }

type GraphComparisonResult = (ParsedInput * bool) list

[<RequireQualifiedAccess>]
type TcCacheState =
    | Empty
    | Present of GraphComparisonResult

type TcResult =
    {
        Input: ParsedInput
        DeclaredImpl: CheckedImplFile
        State: TcState
    }

[<Sealed>]
type CachingDriver(tcConfig: TcConfig) =

    let outputDir = tcConfig.outputDir |> Option.defaultValue ""
    let tcDataFilePath = Path.Combine(outputDir, FSharpTcDataResourceName)
    let graphFilePath = Path.Combine(outputDir, "tcGraph")
    let tcSharedDataFilePath = Path.Combine(outputDir, "tcSharedData")
    let tcInputFilePath = Path.Combine(outputDir, "tcInput")
    let tcStateFilePath = Path.Combine(outputDir, "tcState")

    [<Literal>]
    let CmdLineHeader = "CMDLINE"

    [<Literal>]
    let ReferencesHeader = "REFERENCES"

    let writeThisTcData tcData =
        use tcDataFile = FileSystem.OpenFileForWriteShim tcDataFilePath

        let lines = ResizeArray<string>()
        lines.Add $"BEGIN {CmdLineHeader}"
        lines.AddRange tcData.CmdLine
        lines.Add $"BEGIN {ReferencesHeader}"
        lines.AddRange tcData.References

        tcDataFile.WriteAllLines lines

    let readPrevTcData () =
        if FileSystem.FileExistsShim tcDataFilePath then
            use tcDataFile = FileSystem.OpenFileForReadShim tcDataFilePath

            let cmdLine = ResizeArray<string>()
            let refs = ResizeArray<string>()

            let mutable currentHeader = ""

            tcDataFile.ReadLines()
            |> Seq.iter (fun line ->
                match line with
                | line when line.StartsWith "BEGIN" -> currentHeader <- line.Split ' ' |> Array.last
                | line ->
                    match currentHeader with
                    | CmdLineHeader -> cmdLine.Add line
                    | ReferencesHeader -> refs.Add line
                    | _ -> invalidOp "broken tc cache")

            Some
                {
                    CmdLine = cmdLine.ToArray()
                    References = refs.ToArray()
                }

        else
            None

    let writeThisGraph graph =
        use tcDataFile = FileSystem.OpenFileForWriteShim graphFilePath

        let formatGraphFileLine l =
            sprintf "%i,%s,%i" l.Index l.FileName l.Stamp

        (graph.Files |> List.map formatGraphFileLine) @ graph.Dependencies
        |> tcDataFile.WriteAllLines

    let readPrevGraph () =
        if FileSystem.FileExistsShim graphFilePath then
            use graphFile = FileSystem.OpenFileForReadShim graphFilePath

            let parseGraphFileLine (l: string) =
                let parts = l.Split(',') |> Array.toList

                {
                    Index = int parts[0]
                    FileName = parts[1]
                    Stamp = int64 parts[2]
                }

            let depLines, fileLines =
                graphFile.ReadAllLines()
                |> Array.toList
                |> List.partition (fun l -> l.Contains "-->")

            Some
                {
                    Files = fileLines |> List.map parseGraphFileLine
                    Dependencies = depLines
                }
        else
            None

    let formatAssemblyReference (r: AssemblyReference) =
        let fileName = r.Text
        let lastWriteTime = FileSystem.GetLastWriteTimeShim fileName
        sprintf "%s,%i" fileName lastWriteTime.Ticks

    let getThisCompilationCmdLine args = args

    // maybe split into two things?
    let getThisCompilationGraph inputs =
        let sourceFiles =
            inputs
            |> Seq.toArray
            |> Array.mapi (fun idx (input: ParsedInput) ->
                {
                    Idx = idx
                    FileName = input.FileName
                    ParsedInput = input
                })

        let filePairs = FilePairMap sourceFiles
        let graph, _ = DependencyResolution.mkGraph filePairs sourceFiles

        let graphFileLines =
            [
                for KeyValue(idx, _) in graph do
                    let fileName = sourceFiles[idx].FileName
                    let lastWriteTime = FileSystem.GetLastWriteTimeShim fileName

                    let graphFileLine =
                        {
                            Index = idx
                            FileName = fileName
                            Stamp = lastWriteTime.Ticks
                        }

                    yield graphFileLine
            ]

        let dependencies =
            [
                for KeyValue(idx, deps) in graph do
                    for depIdx in deps do
                        yield $"%i{idx} --> %i{depIdx}"
            ]

        {
            Files = graphFileLines
            Dependencies = dependencies
        }

    let getThisCompilationReferences = Seq.map formatAssemblyReference >> Seq.toArray

    // TODO: don't ignore dependencies
    let compareGraphs (inputs: ParsedInput list) thisGraph baseGraph : GraphComparisonResult =

        let isPresentInBaseGraph thisLine =
            baseGraph.Files
            |> Seq.tryFind (fun baseLine -> baseLine.FileName = thisLine.FileName)
            |> Option.exists (fun baseLine -> baseLine.Stamp = thisLine.Stamp)

        // TODO: make this robust
        let findMatchingInput thisLine =
            inputs
            |> Seq.where (fun input -> input.FileName = thisLine.FileName)
            |> Seq.exactlyOne

        thisGraph.Files
        |> List.map (fun thisLine ->
            let input = findMatchingInput thisLine
            let canReuse = isPresentInBaseGraph thisLine
            input, canReuse)

    member _.GetTcCacheState inputs =
        let prevTcDataOpt = readPrevTcData ()

        let thisTcData =
            {
                CmdLine = getThisCompilationCmdLine tcConfig.cmdLineArgs
                References = getThisCompilationReferences tcConfig.referencedDLLs
            }

        match prevTcDataOpt with
        | Some prevTcData ->
            use _ = Activity.start Activity.Events.reuseTcResultsCachePresent []

            if prevTcData = thisTcData then
                match readPrevGraph () with
                | Some graph ->
                    let thisGraph = getThisCompilationGraph inputs

                    let graphComparisonResult = graph |> compareGraphs inputs thisGraph

                    // we'll need more events do distinguish scenarios here
                    use _ =
                        if graphComparisonResult |> Seq.forall (fun (_file, canUse) -> canUse) then
                            Activity.start Activity.Events.reuseTcResultsCacheHit []
                        else
                            Activity.start Activity.Events.reuseTcResultsCacheMissed []

                    TcCacheState.Present graphComparisonResult
                | None ->
                    use _ = Activity.start Activity.Events.reuseTcResultsCacheMissed []
                    TcCacheState.Empty
            else
                use _ = Activity.start Activity.Events.reuseTcResultsCacheMissed []
                TcCacheState.Empty

        | None ->
            use _ = Activity.start Activity.Events.reuseTcResultsCacheAbsent []
            TcCacheState.Empty

    member private _.ReuseSharedData() =
        let bytes = File.ReadAllBytes(tcSharedDataFilePath)
        let memory = ByteMemory.FromArray(bytes)
        let byteReaderA () = ReadOnlyByteMemory(memory)

        let data =
            GetSharedData(
                "", // assembly.FileName,
                ILScopeRef.Local, // assembly.ILScopeRef,
                None, //assembly.RawMetadata.TryGetILModuleDef(),
                byteReaderA,
                None
            )

        data.RawData

    member private _.ReuseDeclaredImpl(implFile: ParsedInput) =
        let fileName = Path.GetFileNameWithoutExtension(implFile.FileName)
        let bytes = File.ReadAllBytes($"{tcInputFilePath}{fileName}")
        let memory = ByteMemory.FromArray(bytes)
        let byteReaderA () = ReadOnlyByteMemory(memory)

        let data =
            GetCheckedImplFile(
                "", // assembly.FileName,
                ILScopeRef.Local, // assembly.ILScopeRef,
                None, //assembly.RawMetadata.TryGetILModuleDef(),
                byteReaderA,
                None
            )

        data.RawData

    member private _.ReuseTcState(name: string) : TcState =
        let bytes = File.ReadAllBytes($"{tcStateFilePath}{name}")
        let memory = ByteMemory.FromArray(bytes)
        let byteReaderA () = ReadOnlyByteMemory(memory)

        let data =
            GetTypecheckingDataTcState(
                "", // assembly.FileName,
                ILScopeRef.Local, // assembly.ILScopeRef,
                None, //assembly.RawMetadata.TryGetILModuleDef(),
                byteReaderA,
                None
            )

        data.RawData

    member this.ReuseTcResults inputs =
        let sharedData = this.ReuseSharedData()
        let declaredImpls = inputs |> List.map this.ReuseDeclaredImpl

        let lastInput = inputs |> List.last
        let fileName = Path.GetFileNameWithoutExtension(lastInput.FileName)
        let lastState = this.ReuseTcState fileName

        lastState, sharedData.TopAttribs, declaredImpls, lastState.TcEnvFromImpls

    member private _.CacheSharedData(tcState: TcState, sharedData, tcGlobals, outfile) =
        let encodedData =
            EncodeSharedData(tcConfig, tcGlobals, tcState.Ccu, outfile, false, sharedData)

        let resource = encodedData[0].GetBytes().ToArray()
        File.WriteAllBytes(tcSharedDataFilePath, resource)

    member private _.CacheDeclaredImpl(fileName: string, tcState: TcState, impl, tcGlobals, outfile) =
        let encodedData =
            EncodeCheckedImplFile(tcConfig, tcGlobals, tcState.Ccu, outfile, false, impl)

        let resource = encodedData[0].GetBytes().ToArray()
        File.WriteAllBytes($"{tcInputFilePath}{fileName}", resource)

    member private _.CacheTcState(fileName: string, tcState: TcState, tcGlobals, outfile) =
        let encodedData =
            EncodeTypecheckingDataTcState(tcConfig, tcGlobals, tcState.Ccu, outfile, false, tcState)

        let resource = encodedData[0].GetBytes().ToArray()
        File.WriteAllBytes($"{tcStateFilePath}{fileName}", resource)

    member this.CacheTcResults(tcResults, topAttribs, _tcEnvAtEndOfLastFile, tcGlobals, outfile) =
        let thisTcData =
            {
                CmdLine = getThisCompilationCmdLine tcConfig.cmdLineArgs
                References = getThisCompilationReferences tcConfig.referencedDLLs
            }

        writeThisTcData thisTcData

        let inputs = tcResults |> List.map (fun r -> r.Input)
        let thisGraph = getThisCompilationGraph inputs
        writeThisGraph thisGraph

        let sharedData = { TopAttribs = topAttribs }

        let lastState = tcResults |> List.map (fun r -> r.State) |> List.last
        this.CacheSharedData(lastState, sharedData, tcGlobals, outfile)

        tcResults
        |> List.iter (fun r ->
            // TODO: bare file name is not enough
            let fileName = Path.GetFileNameWithoutExtension(r.Input.FileName)
            this.CacheDeclaredImpl(fileName, r.State, r.DeclaredImpl, tcGlobals, outfile)
            this.CacheTcState(fileName, r.State, tcGlobals, outfile))
