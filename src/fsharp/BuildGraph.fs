// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Compiler.BuildGraph

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Features
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text
open FSharp.Compiler.ErrorLogger
open System
open System.Threading
open System.Globalization

/// This represents the thread-local state established as each task function runs as part of the build.
///
/// Use to reset error and warning handlers.
type CompilationGlobalsScope(errorLogger: ErrorLogger, phase: BuildPhase) = 
    let unwindEL = PushErrorLoggerPhaseUntilUnwind(fun _ -> errorLogger)
    let unwindBP = PushThreadBuildPhaseUntilUnwind phase

    member _.ErrorLogger = errorLogger
    member _.Phase = phase

    // Return the disposable object that cleans up
    interface IDisposable with
        member d.Dispose() =
            unwindBP.Dispose()         
            unwindEL.Dispose()

[<NoEquality;NoComparison>]
type GraphNode<'T> = Node of Async<'T>

type Async<'T> with

    static member AwaitGraphNode(node: GraphNode<'T>) =
        match node with
        | Node(computation) -> computation

[<Sealed>]
type GraphNodeBuilder() =

    member _.Zero () : GraphNode<unit> = 
        Node(
            async { 
                ()
            }
        )

    member _.Delay (f: unit -> GraphNode<'T>) = f()   

    member _.Return value =
        Node(
            async { 
                return value
            }
        )

    member _.ReturnFrom (computation:GraphNode<_>) = computation

    member _.Bind (computation: GraphNode<'a>, binder: 'a -> GraphNode<'b>) : GraphNode<'b> =
        Node(
            async {
                let errorLogger = CompileThreadStatic.ErrorLogger
                let phase = CompileThreadStatic.BuildPhase
                try
                    let! res = computation |> Async.AwaitGraphNode
                    CompileThreadStatic.ErrorLogger <- errorLogger
                    CompileThreadStatic.BuildPhase <- phase
                    return! binder res |> Async.AwaitGraphNode
                finally
                    CompileThreadStatic.ErrorLogger <- errorLogger
                    CompileThreadStatic.BuildPhase <- phase
            }
        )

    member _.TryWith(computation: GraphNode<'T>, binder: exn -> GraphNode<'T>) : GraphNode<'T> =
        Node(
            async {
                let errorLogger = CompileThreadStatic.ErrorLogger
                let phase = CompileThreadStatic.BuildPhase
                try
                    try
                        return! computation |> Async.AwaitGraphNode
                    with
                    | ex ->  
                        CompileThreadStatic.ErrorLogger <- errorLogger
                        CompileThreadStatic.BuildPhase <- phase
                        return! binder ex |> Async.AwaitGraphNode
                finally
                    CompileThreadStatic.ErrorLogger <- errorLogger
                    CompileThreadStatic.BuildPhase <- phase
            }
        )

    member _.TryFinally(computation: GraphNode<'T>, binder: unit -> unit) : GraphNode<'T> =
        Node(
            async {
                let errorLogger = CompileThreadStatic.ErrorLogger
                let phase = CompileThreadStatic.BuildPhase
                try
                    return! computation |> Async.AwaitGraphNode
                finally
                    CompileThreadStatic.ErrorLogger <- errorLogger
                    CompileThreadStatic.BuildPhase <- phase
                    binder()
            }
        )

    member _.For(xs: 'T seq, binder: 'T -> GraphNode<unit>) : GraphNode<unit> =
        Node(
            async {
                for x in xs do
                    do! binder x |> Async.AwaitGraphNode
            }
        )

    member _.Combine(x1: GraphNode<unit>, x2: GraphNode<'T>) : GraphNode<'T> =
        Node(
            async {
                do! x1 |> Async.AwaitGraphNode
                return! x2 |> Async.AwaitGraphNode
            }
        )

    member _.Using(value: CompilationGlobalsScope, binder: CompilationGlobalsScope -> GraphNode<'U>) =
        Node(
            async {
                CompileThreadStatic.ErrorLogger <- value.ErrorLogger
                CompileThreadStatic.BuildPhase <- value.Phase

                try
                    return! binder value |> Async.AwaitGraphNode
                finally
                    (value :> IDisposable).Dispose()
            }
        )

let node = GraphNodeBuilder()

[<AbstractClass;Sealed>]
type GraphNode =

    static member RunSynchronously (computation: GraphNode<'T>) =
        let errorLogger = CompileThreadStatic.ErrorLogger
        let phase = CompileThreadStatic.BuildPhase
        try
            async {
                CompileThreadStatic.ErrorLogger <- errorLogger
                CompileThreadStatic.BuildPhase <- phase
                return! computation |> Async.AwaitGraphNode
            }
            |> Async.RunSynchronously
        finally
            CompileThreadStatic.ErrorLogger <- errorLogger
            CompileThreadStatic.BuildPhase <- phase

    static member StartAsTask (computation: GraphNode<'T>, ?ct: CancellationToken) =
        let errorLogger = CompileThreadStatic.ErrorLogger
        let phase = CompileThreadStatic.BuildPhase
        try
            let work =
                async {
                    CompileThreadStatic.ErrorLogger <- errorLogger
                    CompileThreadStatic.BuildPhase <- phase
                    return! computation |> Async.AwaitGraphNode
                }
            Async.StartAsTask(work, cancellationToken=defaultArg ct CancellationToken.None)
        finally
            CompileThreadStatic.ErrorLogger <- errorLogger
            CompileThreadStatic.BuildPhase <- phase

    static member CancellationToken = Node(async { return! Async.CancellationToken })

    static member AwaitAsync(computation: Async<'T>) = Node(computation)

    static member AwaitWaitHandle(waitHandle: WaitHandle) =
        Node(
            async {
                return! Async.AwaitWaitHandle(waitHandle)
            }
        )

    static member Sequential(computations: GraphNode<'T> seq) =
        node {
            let results = ResizeArray()
            for computation in computations do
                let! res = computation
                results.Add(res)
            return results.ToArray()
        }

type private AgentMessage<'T> =
    | GetValue of AsyncReplyChannel<Result<'T, Exception>> * CancellationToken

type private AgentInstance<'T> = (MailboxProcessor<AgentMessage<'T>> * CancellationTokenSource)

[<RequireQualifiedAccess>]
type private AgentAction<'T> =
    | GetValue of AgentInstance<'T>
    | CachedValue of 'T

[<RequireQualifiedAccess>]
module LazyGraphNode =

    // We need to store the culture for the VS thread that is executing now, 
    // so that when the agent in the async lazy object picks up thread from the thread pool we can set the culture
    let mutable culture = CultureInfo(CultureInfo.CurrentUICulture.Name)

    let SetPreferredUILang (preferredUiLang: string option) = 
        match preferredUiLang with
        | Some s -> 
            culture <- CultureInfo s
#if FX_RESHAPED_GLOBALIZATION
            CultureInfo.CurrentUICulture <- culture
#else
            Thread.CurrentThread.CurrentUICulture <- culture
#endif
        | None -> ()

[<Sealed>]
type LazyGraphNode<'T> (computation: GraphNode<'T>) =

    let gate = obj ()
    let mutable computation = computation
    let mutable requestCount = 0
    let mutable cachedResult = ValueNone
    let mutable cachedResultNode = ValueNone

    let loop (agent: MailboxProcessor<AgentMessage<'T>>) =
        async {
            try
                while true do
                    match! agent.Receive() with
                    | GetValue (replyChannel, ct) ->
                        Thread.CurrentThread.CurrentUICulture <- LazyGraphNode.culture
                        try
                            use _reg = 
                                // When a cancellation has occured, notify the reply channel to let the requester stop waiting for a response.
                                ct.Register (fun () -> 
                                    let ex = OperationCanceledException() :> exn
                                    replyChannel.Reply (Result.Error ex)
                                )

                            ct.ThrowIfCancellationRequested ()

                            match cachedResult with
                            | ValueSome result ->
                                replyChannel.Reply (Ok result)
                            | _ ->
                                // This computation can only be canceled if the requestCount reaches zero.
                                let! result = computation |> Async.AwaitGraphNode
                                cachedResult <- ValueSome result
                                cachedResultNode <- ValueSome (Node(async { return result }))
                                computation <- Unchecked.defaultof<_>
                                if not ct.IsCancellationRequested then
                                    replyChannel.Reply (Ok result)
                        with 
                        | ex ->
                            replyChannel.Reply (Result.Error ex)
            with
            | _ -> 
                ()
        }

    let mutable agentInstance: AgentInstance<'T> option = None

    member _.GetValue() =
        // fast path
        match cachedResultNode with
        | ValueSome resultNode -> resultNode
        | _ ->
            node {
                match cachedResult with
                | ValueSome result -> return result
                | _ ->
                    let action =
                        lock gate <| fun () ->
                            // We try to get the cached result after the lock so we don't spin up a new mailbox processor.
                            match cachedResult with
                            | ValueSome result -> AgentAction<'T>.CachedValue result
                            | _ ->
                                requestCount <- requestCount + 1
                                match agentInstance with
                                | Some agentInstance -> AgentAction<'T>.GetValue agentInstance
                                | _ ->
                                    try
                                        let cts = new CancellationTokenSource()
                                        let agent = new MailboxProcessor<_>(loop, cancellationToken = cts.Token)
                                        let newAgentInstance = (agent, cts)
                                        agentInstance <- Some newAgentInstance
                                        agent.Start()
                                        AgentAction<'T>.GetValue newAgentInstance
                                    with
                                    | ex ->
                                        agentInstance <- None
                                        raise ex

                    match action with
                    | AgentAction.CachedValue result -> return result
                    | AgentAction.GetValue(agent, cts) ->                       
                        try
                            let! ct = GraphNode.CancellationToken
                            let! res = agent.PostAndAsyncReply(fun replyChannel -> GetValue(replyChannel, ct)) |> GraphNode.AwaitAsync
                            match res with
                            | Ok result -> return result
                            | Result.Error ex -> return raise ex
                        finally
                            lock gate <| fun () ->
                                requestCount <- requestCount - 1
                                if requestCount = 0 then
                                        cts.Cancel() // cancel computation when all requests are cancelled
                                        try (agent :> IDisposable).Dispose () with | _ -> ()
                                        cts.Dispose()
                                        agentInstance <- None
            }

       member _.TryGetValue() = cachedResult

       member _.RequestCount = requestCount