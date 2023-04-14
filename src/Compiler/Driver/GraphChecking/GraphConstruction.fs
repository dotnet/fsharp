module internal FSharp.Compiler.GraphChecking.GraphConstructor

open System.Linq

open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.IO
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open Internal.Utilities.Library

[<RequireQualifiedAccess>]
type NodeToTypeCheck =
    | PhysicalFile of fileIndex: FileIndex
    | ArtificialImplFile of signatureFileIndex: FileIndex

    member this.FileIndex =
        match this with
        | PhysicalFile idx -> idx
        | ArtificialImplFile idx -> idx

let constructGraphs (tcConfig: TcConfig option) (sourceFiles: FileInProject array) =
    let filePairs = FilePairMap(sourceFiles)

    let compilingFSharpCore =
        tcConfig
        |> Option.map (fun c -> c.compilingFSharpCore)
        |> Option.defaultValue false

    let graph = DependencyResolution.mkGraph compilingFSharpCore filePairs sourceFiles

    let nodeGraph =
        let mkArtificialImplFile n = NodeToTypeCheck.ArtificialImplFile n
        let mkPhysicalFile n = NodeToTypeCheck.PhysicalFile n

        /// Map any signature dependencies to the ArtificialImplFile counterparts,
        /// unless the signature dependency is the backing file of the current (implementation) file.
        let mapDependencies idx deps =
            Array.map
                (fun dep ->
                    if filePairs.IsSignature dep then
                        let implIdx = filePairs.GetImplementationIndex dep

                        if implIdx = idx then
                            // This is the matching signature for the implementation.
                            // Retain the direct dependency onto the signature file.
                            mkPhysicalFile dep
                        else
                            mkArtificialImplFile dep
                    else
                        mkPhysicalFile dep)
                deps

        // Transform the graph to include ArtificialImplFile nodes when necessary.
        graph
        |> Seq.collect (fun (KeyValue (fileIdx, deps)) ->
            if filePairs.IsSignature fileIdx then
                // Add an additional ArtificialImplFile node for the signature file.
                [|
                    // Mark the current file as physical and map the dependencies.
                    mkPhysicalFile fileIdx, mapDependencies fileIdx deps
                    // Introduce a new node that depends on the signature.
                    mkArtificialImplFile fileIdx, [| mkPhysicalFile fileIdx |]
                |]
            else
                [| mkPhysicalFile fileIdx, mapDependencies fileIdx deps |])
        |> Graph.make

    // Persist the graph to a Mermaid diagram if specified.
    tcConfig
    |> Option.iter (fun config ->
        if config.typeCheckingConfig.DumpGraph then
            config.outputFile
            |> Option.iter (fun outputFile ->
                let outputFile = FileSystem.GetFullPathShim(outputFile)
                let graphFile = FileSystem.ChangeExtensionShim(outputFile, ".graph.md")

                graph
                |> Graph.map (fun idx ->
                    let friendlyFileName =
                        sourceFiles[idx]
                            .FileName.Replace(config.implicitIncludeDir, "")
                            .TrimStart([| '\\'; '/' |])

                    (idx, friendlyFileName))
                |> Graph.serialiseToMermaid graphFile))

    nodeGraph

let getDependencyGraph (inputs: ParsedInput list) : Graph<int * string> =

    let sourceFiles = FileInProject.FromFileInProject inputs
    let nodeGraph = constructGraphs None sourceFiles

    let findFileName idx =
        sourceFiles |> Array.find (fun f -> f.Idx = idx) |> (fun f -> f.FileName)

    let graph =
        nodeGraph.Select(fun a ->
            let idx = a.Key.FileIndex
            let name = findFileName idx
            let v = a.Value |> Array.map (fun x -> x.FileIndex, findFileName x.FileIndex)
            (idx, name), v)

    graph |> Graph.make
