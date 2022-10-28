module FSharp.Compiler.Service.Tests.code

open System.Collections.Generic
open FSharp.Compiler.Service.Tests.Graph
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
    // custom check - compare against CustomerId only
    override this.Equals other =
        match other with
        | :? SourceFile as p -> p.Name.Equals this.Name
        | _ -> false
    // custom hash check
    override this.GetHashCode () = this.Name.GetHashCode()
    
type SourceFiles = SourceFile[]
type FsiInfo =
    | FsiBacked
    | NotBacked
        with member this.IsFsiBacked =
                match this with
                | FsiBacked -> true
                | NotBacked -> false
 
type File =
    {
        File : SourceFile
        FsiInfo : FsiInfo
    }
type Files = File[]
type FileGraph = Graph<File>

let gatherBackingInfo (files : SourceFiles) : Files =
    let seenSigFiles = HashSet<string>()
    files
    |> Array.map (fun f ->
        let fsiInfo =
            match f.AST with
            | ParsedInput.SigFile _ ->
                NotBacked
            | ParsedInput.ImplFile _ ->
                let fsiName = System.IO.Path.ChangeExtension(f.Name, "fsi")
                match seenSigFiles.Contains fsiName with
                | true -> FsiBacked
                | false -> NotBacked
        {
            File = f
            FsiInfo = fsiInfo
        }
    )
    
type EdgeTrimmer = File -> File -> bool

type FileData =
    {
        ModuleRefs : ModuleRef[]
        Structure : FileStructure
    }

let gatherFileData (file : File) =

let calcFileGraph (files : SourceFiles) : FileGraph =
    let fsFsiTrimmer =
        let files =
            gatherBackingInfo files
            ... to dict
        fun file dep -> not files[dep].FsiInfo.IsFsiBacked
    let 

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

// Do we need to suppress some error logging if we
// apply the same partial results multiple times?
// Maybe we can enable logging only for the final fold
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
        |> Array.maxBy sizeMetrix
    let firstState =    snd biggestDep.Result
    // Perf: Keep transDeps in a HashSet from the start
    let included = HashSet(firstState.Info.TransitiveDeps)
    let toAdd =
        transitiveDeps
        |> Array.filter (fun dep -> included.Add dep)
    let state = Array.fold folder firstState toAdd
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