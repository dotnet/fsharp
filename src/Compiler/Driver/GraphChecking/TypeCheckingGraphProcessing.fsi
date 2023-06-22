/// Parallel processing of a type-checking file graph.
module internal FSharp.Compiler.GraphChecking.TypeCheckingGraphProcessing

open System.Threading

/// <summary>
/// Process a graph of items.
/// A version of 'GraphProcessing.processGraph' with a signature slightly specific to type-checking.
/// </summary>
val processTypeCheckingGraph<'Item, 'ChosenItem, 'State, 'FinalFileResult when 'Item: equality and 'Item: comparison> :
    graph: Graph<'Item> ->
    work: ('Item -> 'State -> Finisher<'State, 'FinalFileResult>) ->
    folder: ('State -> Finisher<'State, 'FinalFileResult> -> 'FinalFileResult * 'State) ->
    finalStateChooser: ('Item -> 'ChosenItem option) ->
    emptyState: 'State ->
    ct: CancellationToken ->
        ('ChosenItem * 'FinalFileResult) list * 'State
