module FSharp.Compiler.Service.Tests.Parallel

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading

/// The agent handles two kind of messages - the 'Start' message is sent
/// when the caller wants to start a new work item. The 'Finished' message
/// is sent (by the agent itself) when one work item is completed.
type LimitAgentMessage =
    | Start of Async<unit>
    | Finished

/// A function that takes the limit - the maximal number of operations it
/// will run in parallel - and returns an agent that accepts new
/// tasks via the 'Start' message
let threadingLimitAgent limit (ct : CancellationToken) =
    let act (inbox : MailboxProcessor<LimitAgentMessage>)  =
        async {
            // Keep number of items running & queue of items to run later
            // NOTE: We keep an explicit queue, so that we can e.g. start dropping
            // items if there are too many requests (or do something else)
            // NOTE: The loop is only accessed from one thread at each time
            // so we can just use non-thread-safe queue & mutation
            let queue = Queue<_>()
            let mutable count = 0

            while true do
                let! msg = inbox.Receive()
                // When we receive Start, add the work to the queue
                // When we receive Finished, do count--
                match msg with
                | Start work -> queue.Enqueue(work)
                | Finished -> count <- count + 1 
                // After something happened, we check if we can
                // start a next task from the queue
                if count < limit && queue.Count > 0 then
                    count <- count + 1
                    let work = queue.Dequeue()
                    // Start it in a thread pool (on background)
                    Async.Start(
                        async {
                            do! work
                            inbox.Post(Finished)
                        }
                    )
        }
    MailboxProcessor.Start(act, ct)

// TODO Test this version
/// Untested version that uses MailboxProcessor.
/// See http://www.fssnip.net/nX/title/Limit-degree-of-parallelism-using-an-agent for implementation
let processInParallelUsingMailbox
    (firstItems : 'Item[])
    (work : 'Item -> Async<'Item[]>)
    (parallelism : int)
    (notify : int -> unit)
    (ct : CancellationToken)
    : unit
    =
    let processedCountLock = Object()
    let mutable processedCount = 0
    let agent = threadingLimitAgent 10 ct
    let rec processItem item =
        async {
            let! toSchedule = work item
            let pc = lock processedCountLock (fun () -> processedCount <- processedCount + 1; processedCount)
            notify pc
            toSchedule |> Array.iter (fun x -> agent.Post(Start(processItem x)))   
        }
    firstItems |> Array.iter (fun x -> agent.Post(Start(processItem x)))
    ()    
    
// TODO Could replace with MailboxProcessor+Tasks/Asyncs instead of BlockingCollection + Threads
// See http://www.fssnip.net/nX/title/Limit-degree-of-parallelism-using-an-agent
/// Process items in parallel, allow more work to be scheduled as a result of finished work,
/// limit parallelisation to 'parallelism' threads
let processInParallel
    (firstItems : 'Item[])
    (work : 'Item -> 'Item[])
    (parallelism : int)
    (stop : int -> bool)
    (ct : CancellationToken)
    : unit
    =
    let bc = new BlockingCollection<'Item>()
    firstItems |> Array.iter bc.Add
    let processedCountLock = Object()
    let mutable processedCount = 0
    let processItem item =
        // printfn $"Processing {item}"
        let toSchedule = work item
        let processedCount = lock processedCountLock (fun () -> processedCount <- processedCount + 1; processedCount)
        toSchedule
        |> Array.iter (
            fun next -> bc.Add(next)
        )
        processedCount
    
    // TODO Could avoid workers with some semaphores
    let workerWork () : unit =
        for node in bc.GetConsumingEnumerable(ct) do
            if not ct.IsCancellationRequested then // improve
                let processedCount = processItem node
                if stop processedCount then
                    bc.CompleteAdding()

    Array.Parallel.map workerWork (Array.init parallelism (fun _ -> ())) |> ignore // use cancellation
    ()

let test () =
    // Create an agent that can run at most 2 tasks in parallel
    // and send 10 work items that take 1 second to the queue
    use cts = new CancellationTokenSource()
    let agent = threadingLimitAgent 2 cts.Token

    for i in 0..10 do
        agent.Post(
            Start(
                async {
                    do! Async.Sleep(1000)
                    printfn $"Finished: %d{i}"
                }
            )
        )
