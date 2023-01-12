/// Parallel processing of graph of work items with dependencies
module internal FSharp.Compiler.GraphChecking.TypeCheckingGraphProcessing

open GraphProcessing
open System.Collections.Generic
open System.Threading

// TODO Do we need to suppress some error logging if we
// TODO apply the same partial results multiple times?
// TODO Maybe we can enable logging only for the final fold
/// <summary>
/// Combine type-checking results of dependencies needed to type-check a 'higher' node in the graph
/// </summary>
/// <param name="emptyState">Initial state</param>
/// <param name="deps">Direct dependencies of a node</param>
/// <param name="transitiveDeps">Transitive dependencies of a node</param>
/// <param name="folder">A way to fold a single result into existing state</param>
/// <remarks>
/// Similar to 'processFileGraph', this function is generic yet specific to the type-checking process.
/// </remarks>
let private combineResults
    (emptyState: 'State)
    (deps: ProcessedNode<'Item, 'State * Finisher<'State, 'FinalFileResult>>[])
    (transitiveDeps: ProcessedNode<'Item, 'State * Finisher<'State, 'FinalFileResult>>[])
    (folder: 'State -> Finisher<'State, 'FinalFileResult> -> 'State)
    : 'State =
    match deps with
    | [||] -> emptyState
    | _ ->
        // Instead of starting with empty state,
        // reuse state produced by the the dependency with the biggest number of transitive dependencies.
        // This is to reduce the number of folds required to achieve the final state.
        let biggestDependency =
            let sizeMetric (node: ProcessedNode<_, _>) = node.Info.TransitiveDeps.Length
            deps |> Array.maxBy sizeMetric

        let firstState = biggestDependency.Result |> fst

        // Find items not already included in the state.
        // Note: Ordering is not preserved due to reusing results of the biggest child
        // rather than starting with empty state
        let itemsPresent =
            let set = HashSet(biggestDependency.Info.TransitiveDeps)
            set.Add biggestDependency.Info.Item |> ignore
            set

        let resultsToAdd =
            transitiveDeps
            |> Array.filter (fun dep -> itemsPresent.Contains dep.Info.Item = false)
            |> Array.distinctBy (fun dep -> dep.Info.Item)
            |> Array.map (fun dep -> dep.Result |> snd)

        // Fold results not already included and produce the final state
        let state = Array.fold folder firstState resultsToAdd
        state

// TODO This function and its parameters are quite specific to type-checking despite using generic types.
// Perhaps we should make it either more specific and remove type parameters, or more generic.
/// <summary>
/// Process a graph of items.
/// A version of 'GraphProcessing.processGraph' with a signature slightly specific to type-checking.
/// </summary>
let processTypeCheckingGraph<'Item, 'ChosenItem, 'State, 'FinalFileResult when 'Item: equality and 'Item: comparison>
    (graph: Graph<'Item>)
    (work: 'Item -> 'State -> Finisher<'State, 'FinalFileResult>)
    (folder: 'State -> Finisher<'State, 'FinalFileResult> -> 'FinalFileResult * 'State)
    // Decides whether a result for an item should be included in the final state, and how to map the item if it should.
    (finalStateChooser: 'Item -> 'ChosenItem option)
    (emptyState: 'State)
    (ct: CancellationToken)
    : ('ChosenItem * 'FinalFileResult) list * 'State =

    let workWrapper
        (getProcessedNode: 'Item -> ProcessedNode<'Item, 'State * Finisher<'State, 'FinalFileResult>>)
        (node: NodeInfo<'Item>)
        : 'State * Finisher<'State, 'FinalFileResult> =
        let folder x y = folder x y |> snd
        let deps = node.Deps |> Array.except [| node.Item |] |> Array.map getProcessedNode

        let transitiveDeps =
            node.TransitiveDeps
            |> Array.except [| node.Item |]
            |> Array.map getProcessedNode

        let inputState = combineResults emptyState deps transitiveDeps folder
        let singleRes = work node.Item inputState
        let state = folder inputState singleRes
        state, singleRes

    let results = processGraph graph workWrapper ct

    let finalFileResults, state: ('ChosenItem * 'FinalFileResult) list * 'State =
        results
        |> Array.choose (fun (item, res) ->
            match finalStateChooser item with
            | Some item -> Some(item, res)
            | None -> None)
        |> Array.fold
            (fun (fileResults, state) (item, (_, itemRes)) ->
                let fileResult, state = folder state itemRes
                (item, fileResult) :: fileResults, state)
            ([], emptyState)

    finalFileResults, state
