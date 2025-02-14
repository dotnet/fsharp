module internal FSharp.Compiler.ReuseTcResults

open System.Collections.Generic
open System.IO

open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.GraphChecking
open FSharp.Compiler.IO
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TypedTree
open CompilerImports
open FSharp.Compiler.AbstractIL.IL

type TcData =
    {
        CmdLine: string array
        Graph: string array
        References: string array
    }

[<Sealed>]
type CachingDriver(tcConfig: TcConfig) =

    let outputDir = tcConfig.outputDir |> Option.defaultValue ""
    let tcDataFilePath = Path.Combine(outputDir, FSharpTcDataResourceName)

    [<Literal>]
    let CmdLineHeader = "CMDLINE"

    [<Literal>]
    let GraphHeader = "GRAPH"

    [<Literal>]
    let ReferencesHeader = "REFERENCES"

    let writeThisTcData (tcData: TcData) =
        use tcDataFile = FileSystem.OpenFileForWriteShim tcDataFilePath

        let lines = ResizeArray<string>()
        lines.Add $"BEGIN {CmdLineHeader}"
        lines.AddRange tcData.CmdLine
        lines.Add $"BEGIN {GraphHeader}"
        lines.AddRange tcData.Graph
        lines.Add $"BEGIN {ReferencesHeader}"
        lines.AddRange tcData.References

        tcDataFile.WriteAllLines lines

    let readPrevTcData () =
        if FileSystem.FileExistsShim tcDataFilePath then
            use tcDataFile = FileSystem.OpenFileForReadShim tcDataFilePath

            let cmdLine = ResizeArray<string>()
            let graph = ResizeArray<string>()
            let refs = ResizeArray<string>()

            let mutable currentHeader = ""

            tcDataFile.ReadLines()
            |> Seq.iter (fun line ->
                match line with
                | line when line.StartsWith "BEGIN" -> currentHeader <- line.Split ' ' |> Array.last
                | line ->
                    match currentHeader with
                    | CmdLineHeader -> cmdLine.Add line
                    | GraphHeader -> graph.Add line
                    | ReferencesHeader -> refs.Add line
                    | _ -> invalidOp "broken tc cache")

            Some
                {
                    CmdLine = cmdLine.ToArray()
                    Graph = graph.ToArray()
                    References = refs.ToArray()
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

        let list = List<string>()

        for KeyValue(idx, _) in graph do
            let fileName = sourceFiles[idx].FileName
            let lastWriteTime = FileSystem.GetLastWriteTimeShim fileName
            list.Add(sprintf "%i,%s,%i" idx fileName lastWriteTime.Ticks)

        for KeyValue(idx, deps) in graph do
            for depIdx in deps do
                list.Add $"%i{idx} --> %i{depIdx}"

        list.ToArray()

    let getThisCompilationReferences = Seq.map formatAssemblyReference >> Seq.toArray

    member _.CanReuseTcResults inputs =
        let prevTcDataOpt = readPrevTcData ()

        let thisTcData =
            {
                CmdLine = getThisCompilationCmdLine tcConfig.cmdLineArgs
                Graph = getThisCompilationGraph inputs
                References = getThisCompilationReferences tcConfig.referencedDLLs
            }

        match prevTcDataOpt with
        | Some prevTcData ->
            use _ = Activity.start Activity.Events.reuseTcResultsCachePresent []

            if prevTcData = thisTcData then
                use _ = Activity.start Activity.Events.reuseTcResultsCacheHit []
                true
            else
                use _ = Activity.start Activity.Events.reuseTcResultsCacheMissed []
                writeThisTcData thisTcData
                false

        | None ->
            use _ = Activity.start Activity.Events.reuseTcResultsCacheAbsent []
            writeThisTcData thisTcData
            false

    member _.ReuseTcResults inputs (tcInitialState: TcState) =

        let bytes = File.ReadAllBytes("tc")
        let memory = ByteMemory.FromArray(bytes)
        let byteReaderA () = ReadOnlyByteMemory(memory)

        let byteReaderB = None

        let tcInfo =
            GetTypecheckingData(
                "", // assembly.FileName,
                ILScopeRef.Local, // assembly.ILScopeRef,
                None, //assembly.RawMetadata.TryGetILModuleDef(),
                byteReaderA,
                byteReaderB
            )

        let rawData = tcInfo.RawData

        let topAttrs: TopAttribs =
            {
                mainMethodAttrs = rawData.MainMethodAttrs
                netModuleAttrs = rawData.NetModuleAttrs
                assemblyAttrs = rawData.AssemblyAttrs
            }

        // need to understand if anything can be used here, pickling state is hard
        tcInitialState,
        topAttrs,
        rawData.DeclaredImpls,
        // this is quite definitely wrong, need to figure out what to do with the environment
        tcInitialState.TcEnvFromImpls

    member _.CacheTcResults(tcState: TcState, topAttrs: TopAttribs, declaredImpls, tcEnvAtEndOfLastFile, inputs, tcGlobals, outfile) =
        let thisTcData =
            {
                CmdLine = getThisCompilationCmdLine tcConfig.cmdLineArgs
                Graph = getThisCompilationGraph inputs
                References = getThisCompilationReferences tcConfig.referencedDLLs
            }

        writeThisTcData thisTcData

        let tcInfo =
            {
                MainMethodAttrs = topAttrs.mainMethodAttrs
                NetModuleAttrs = topAttrs.netModuleAttrs
                AssemblyAttrs = topAttrs.assemblyAttrs
                DeclaredImpls = declaredImpls
            }

        let encodedData =
            EncodeTypecheckingData(tcConfig, tcGlobals, tcState.Ccu, outfile, false, tcInfo)

        let resource = encodedData[0].GetBytes().ToArray()
        File.WriteAllBytes("tc", resource)
