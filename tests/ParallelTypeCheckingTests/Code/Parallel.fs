module ParallelTypeCheckingTests.Parallel

#nowarn "1182"

open System
open System.Collections.Concurrent
open System.Threading

// TODO Could replace with MailboxProcessor+Tasks/Asyncs instead of BlockingCollection + Threads
// See http://www.fssnip.net/nX/title/Limit-degree-of-parallelism-using-an-agent
/// Process items in parallel, allow more work to be scheduled as a result of finished work,
/// limit parallelisation to 'parallelism' threads
let processInParallel
    (firstItems: 'Item[])
    (work: 'Item -> 'Item[])
    (parallelism: int)
    (shouldStop: int -> bool)
    (ct: CancellationToken)
    (_itemToString : 'Item -> string)
    : unit =
    let bc = new BlockingCollection<'Item>()
    firstItems |> Array.iter bc.Add
    let processedCountLock = Object()
    let mutable processedCount = 0

    let processItem item =
        printfn $"Processing {_itemToString item}"
        let toSchedule = work item

        let processedCount =
            lock processedCountLock (fun () ->
                processedCount <- processedCount + 1
                processedCount)
        let toScheduleString =
            toSchedule
            |> Array.map _itemToString
            |> fun names -> String.Join(", ", names)
        printfn $"Scheduling {toSchedule.Length} items: {toScheduleString}"
        toSchedule |> Array.iter bc.Add
        processedCount

    // TODO Could avoid workers with some semaphores
    let workerWork () : unit =
        for node in bc.GetConsumingEnumerable(ct) do
            if not ct.IsCancellationRequested then // improve
                let processedCount = processItem node
                if shouldStop processedCount then
                    bc.CompleteAdding()

    // TODO Do we need to handle cancellation given that workers do it already?
    Array.Parallel.map workerWork (Array.init parallelism (fun _ -> ())) |> ignore
