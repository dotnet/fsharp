module FSharp.Compiler.Service.Tests.code

open System.Collections.Generic
open FSharp.Compiler.Service.Tests.Graph
open FSharp.Compiler.Service.Tests.Utils
open FSharp.Compiler.Service.Tests2
open FSharp.Compiler.Syntax
type AST = FSharp.Compiler.Syntax.ParsedInput

type FileName = FileName of string
    with member this.FileName = match this with FileName fileName -> fileName

/// Input from the compiler after parsing
[<CustomEquality; NoComparison>]
type SourceFile = 
    {
        Name : string
        AST : AST
    }
    override this.Equals other =
        match other with
        | :? SourceFile as p -> p.Name.Equals this.Name
        | _ -> false
    override this.GetHashCode () = this.Name.GetHashCode()

type SourceFiles = SourceFile[]

type File =
    {
        Name : FileName
        AST : AST
        Idx : FileIdx
        FsiBacked : bool
    }
type Files = File[]

let gatherBackingInfo (files : SourceFiles) : Files =
    let seenSigFiles = HashSet<string>()
    files
    |> Array.mapi (fun i f ->
        let fsiBacked =
            match f.AST with
            | ParsedInput.SigFile _ ->
                false
            | ParsedInput.ImplFile _ ->
                let fsiName = System.IO.Path.ChangeExtension(f.Name, "fsi")
                let fsiBacked = seenSigFiles.Contains fsiName
                fsiBacked
        {
            Name = f.Name |> FileName
            AST = f.AST
            Idx = FileIdx.make i
            FsiBacked = fsiBacked
        }
    )
    
type EdgeTrimmer = File -> File -> bool

type FileData =
    {
        Idx : FileIdx
        Name : FileName
        ModuleRefs : LongIdent[]
        Tops : LongIdent[]
        ContainsModuleAbbreviations : bool
        IsFsiBacked : bool
    }

type FileGraph = Graph<File>

let gatherFileData (file : File) : FileData =
    let moduleRefs, containsModuleAbbreviations = ASTVisit.findModuleRefs file.AST
    let tops = ASTVisit.topModuleOrNamespaces file.AST
    // TODO As a perf optimisation we can skip top-level ids scanning for FsiBacked .fs files
    // However, it is unlikely to give a noticable speedup (citation needed)
    {
        Idx = file.Idx
        Name = file.Name
        ModuleRefs = moduleRefs
        Tops = tops
        ContainsModuleAbbreviations = containsModuleAbbreviations
        IsFsiBacked = file.FsiBacked
    }

let calcFileGraph (files : SourceFiles) : FileGraph =
    failwith ""

/// Used for processing
type NodeInfo<'Item> =
    {
        Item : 'Item
        Deps : 'Item[]
        TransitiveDeps : 'Item[]
        Dependants : 'Item[]
        ProcessedDepsCount : int
    }
type Node<'Item, 'State, 'Result> =
    {
        Info : NodeInfo<'Item>
        Result : ('State * 'Result) option
    }

// TODO Do we need to suppress some error logging if we
// TODO apply the same partial results multiple times?
// TODO Maybe we can enable logging only for the final fold
/// <summary>
/// Combine results of dependencies needed to type-check a 'higher' node in the graph 
/// </summary>
/// <param name="deps">Direct dependencies of a node</param>
/// <param name="transitiveDeps">Transitive dependencies of a node</param>
/// <param name="folder">A way to fold a single result into existing state</param>
let combineResults
    (deps : Node<_,_,_>[])
    (transitiveDeps : Node<_,_,_>[])
    (folder : 'State -> 'Result -> 'State)
    : 'State
    =
    let biggestDep =
        let sizeMetric node =
            // Could also use eg. total file size/AST size
            node.Info.TransitiveDeps.Length
        deps
        |> Array.maxBy sizeMetric
    let extractResultOrFail node =
        node.Result
        |> Option.defaultWith (fun () -> failwith "Unexpected lack of result")
        |> snd
    let firstState = extractResultOrFail biggestDep
    
    // TODO Potential perf optimisation: Keep transDeps in a HashSet from the start,
    // avoiding reconstructing the HashSet here
    
    // Add single-file results of remaining transitive deps one-by-one using folder
    // Note: Good to preserve order here so that folding happens in file order
    let included = HashSet(firstState.Info.TransitiveDeps)
    let resultsToAdd =
        transitiveDeps 
        |> Array.filter (fun dep -> included.Contains dep = false)
    let state = Array.fold folder firstState resultsToAdd
    state
    
 let processInParallel
     (firstItems : 'Item[])
     (work : 'Item -> 'Item[])
     (parallelism : int)
     (stop : int -> bool)
     (ct)
     : unit async
     =
     let bc = BlockingCollection(firstItems)
     let mutable processedCount = 0
     let processItem item =
         let toSchedule = work item
         lock processedCount (fun () -> processedCount++)
         toSchedule |> Array.iter bc.Add
     // Could avoid workers with some semaphores
     let workerWork () =
         for node in bc.Get... do
             if not ct.Cancelled then // improve
                 processNode node
             if stop () then
                 bc.CompleteAdding() // avoid doing multiple times?

     Array.Parallel.map
         parallelism workerWork // use cancellation
     
let processGraph
    (graph : FileGraph)
    (doWork : 'Item -> 'State -> 'Result * 'State)
    (folder : 'State -> 'Result -> 'State)
    (parallelism : int)
    : 'State
    =
    let transitiveDeps = graph |> calcTransitiveGraph
    let dependants = graph |> reverseGraph
    let nodes = graph.Keys |> Seq.map ...
    let leaves = nodes |> Seq.filter ...
    let work
        (node : Node<'Item, 'State, 'Result>)
        : Node<'Item, 'State, 'Result>[]
        =
        let inputState = combineResults node.Deps node.TransitiveDeps folder
        let res = doWork node.Info.Item
        node.Result <- res
        let unblocked =
            node.Info.Dependants
            |> Array.filter (fun x -> 
                let pdc =
                    lock x (fun () ->
                    x.Info.ProcessedDepsCount++
                    x.Info.PrcessedDepsCount
                )
                pdc = node.Info.Deps.Length
            )
         |> Array.map (fun x -> nodes[x])
     unblocked
     
    processInParallel
        leaves
        work
        parallelism
        (fun processedCount -> processedCount = nodes.Length)

    let state = combineResults nodes nodes addCheckResultsToTcState 
    state

type TcState
type SingleResult

let typeCheckFile (file : File) (state : TcState)
    : SingleResult * TcState
    =
    ...

let typeCheckGraph (graph : FileGraph) : TcState =
    let parallelism = 4 // cpu count?
    let state =
        processGraph
            graph
            typeCheckFile
            addCheckResultsToTcState
            parallelism
     state
    
let typeCheck (files : SourceFiles) : TcState =
    let graph = calcFileGraph files
    let state = typeCheckGraph graph
    state