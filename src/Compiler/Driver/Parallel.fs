module FSharp.Compiler.Service.Driver.Parallel

#nowarn "1182"

open System
open System.Collections.Concurrent
open System.Threading
open FSharp.Compiler.Diagnostics
open Internal.Utilities.Library.Extras

/// <summary>
/// Process items in parallel, allow more work to be scheduled as a result of finished work,
/// limit parallelisation to 'parallelism' threads.
/// </summary>
/// <remarks>
/// Could be replace with MailboxProcessor+Tasks/Asyncs instead of BlockingCollection + Threads.
/// See http://www.fssnip.net/nX/title/Limit-degree-of-parallelism-using-an-agent for an example.
/// </remarks>
let processInParallel
    (opName: string)
    (firstItems: 'Item[])
    (work: 'Item -> 'Item[])
    (parallelism: int)
    (shouldStop: unit -> bool)
    (ct: CancellationToken)
    (_itemToString: 'Item -> string)
    : unit =

    use _ =
        Activity.start
            $"processInParallel - {opName}"
            [|
                "firstItemsCount", firstItems.Length.ToString()
                "parallelism", parallelism.ToString()
            |]

    let toProcess = new BlockingCollection<'Item>()
    firstItems |> Array.iter toProcess.Add

    let processItem item =
        // printfn $"[{Thread.CurrentThread.ManagedThreadId}] Processing {_itemToString item}"
        use _ = Activity.start "processItem" [| "item", _itemToString item |]
        let toSchedule = work item

        // printfn $"[{Thread.CurrentThread.ManagedThreadId}] Finished {_itemToString item}"
        // printfn $"[{Thread.CurrentThread.ManagedThreadId}] Scheduling {toSchedule.Length} items: {toScheduleString}"
        toSchedule |> Array.iter toProcess.Add

    let worker () : unit =
        for node in toProcess.GetConsumingEnumerable(ct) do
            if not ct.IsCancellationRequested then
                processItem node

                if shouldStop () then
                    toProcess.CompleteAdding()

    let parallelism = min parallelism Environment.ProcessorCount
    ArrayParallel.iter worker (Array.create parallelism ())
