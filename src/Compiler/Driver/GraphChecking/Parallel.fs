module FSharp.Compiler.GraphChecking.Parallel

open System
open System.Collections.Concurrent
open System.Threading

/// Process items in parallel, allow more work to be scheduled as a result of finished work,
/// limit parallelisation to 'parallelism' threads
let processInParallel
    (firstItems: 'Item[])
    (work: 'Item -> 'Item[])
    (parallelism: int)
    (shouldStop: int -> bool)
    (ct: CancellationToken)
    (_itemToString: 'Item -> string)
    : unit =
    let bc = new BlockingCollection<'Item>()
    firstItems |> Array.iter bc.Add
    let processedCountLock = Object()
    let mutable processedCount = 0

    let processItem item =
        let toSchedule = work item

        let processedCount =
            lock processedCountLock (fun () ->
                processedCount <- processedCount + 1
                processedCount)

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
