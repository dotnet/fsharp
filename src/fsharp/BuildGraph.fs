// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module FSharp.Compiler.BuildGraph

open System
open System.Threading
open System.Diagnostics
open System.Globalization
open FSharp.Compiler.ErrorLogger

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
type NodeCode<'T> = Node of Async<'T>

let wrapThreadStaticInfo computation =
    async {
        let errorLogger = CompileThreadStatic.ErrorLogger
        let phase = CompileThreadStatic.BuildPhase
        try
            return! computation 
        finally
            CompileThreadStatic.ErrorLogger <- errorLogger
            CompileThreadStatic.BuildPhase <- phase 
    }

type Async<'T> with

    static member AwaitNode(node: NodeCode<'T>) =
        match node with
        | Node(computation) -> wrapThreadStaticInfo computation

[<Sealed>]
type NodeCodeBuilder() =

    static let zero = Node(async.Zero())

    member _.Zero () : NodeCode<unit> = zero

    member _.Delay (f: unit -> NodeCode<'T>) = 
        Node(async.Delay(fun () -> match f() with Node(p) -> p))    

    member _.Return value = Node(async.Return(value))

    member _.ReturnFrom (computation: NodeCode<_>) = computation

    member _.Bind (Node(p): NodeCode<'a>, binder: 'a -> NodeCode<'b>) : NodeCode<'b> =
        Node(async.Bind(p, fun x -> match binder x with Node p -> p))

    member _.TryWith(Node(p): NodeCode<'T>, binder: exn -> NodeCode<'T>) : NodeCode<'T> =
        Node(async.TryWith(p, fun ex -> match binder ex with Node p -> p))

    member _.TryFinally(Node(p): NodeCode<'T>, binder: unit -> unit) : NodeCode<'T> =
        Node(async.TryFinally(p, binder))

    member _.For(xs: 'T seq, binder: 'T -> NodeCode<unit>) : NodeCode<unit> =
        Node(async.For(xs, fun x -> match binder x with Node p -> p))

    member _.Combine(Node(p1): NodeCode<unit>, Node(p2): NodeCode<'T>) : NodeCode<'T> =
        Node(async.Combine(p1, p2))

    member _.Using(value: CompilationGlobalsScope, binder: CompilationGlobalsScope -> NodeCode<'U>) =
        Node(
            async {
                CompileThreadStatic.ErrorLogger <- value.ErrorLogger
                CompileThreadStatic.BuildPhase <- value.Phase
                try
                    return! binder value |> Async.AwaitNode
                finally
                    (value :> IDisposable).Dispose()
            }
        )

let node = NodeCodeBuilder()

[<AbstractClass;Sealed>]
type NodeCode private () =

    static let cancellationToken =
        Node(wrapThreadStaticInfo Async.CancellationToken)

    static member RunImmediate (computation: NodeCode<'T>) =
        let errorLogger = CompileThreadStatic.ErrorLogger
        let phase = CompileThreadStatic.BuildPhase
        try
            async {
                CompileThreadStatic.ErrorLogger <- errorLogger
                CompileThreadStatic.BuildPhase <- phase
                return! computation |> Async.AwaitNode
            }
            |> Async.RunSynchronously
        finally
            CompileThreadStatic.ErrorLogger <- errorLogger
            CompileThreadStatic.BuildPhase <- phase

    static member StartAsTask (computation: NodeCode<'T>, ?ct: CancellationToken) =
        let errorLogger = CompileThreadStatic.ErrorLogger
        let phase = CompileThreadStatic.BuildPhase
        try
            let work =
                async {
                    CompileThreadStatic.ErrorLogger <- errorLogger
                    CompileThreadStatic.BuildPhase <- phase
                    return! computation |> Async.AwaitNode
                }
            Async.StartAsTask(work, cancellationToken=defaultArg ct CancellationToken.None)
        finally
            CompileThreadStatic.ErrorLogger <- errorLogger
            CompileThreadStatic.BuildPhase <- phase

    static member CancellationToken = cancellationToken

    static member AwaitAsync(computation: Async<'T>) = 
        Node(wrapThreadStaticInfo computation)

    static member AwaitWaitHandle(waitHandle: WaitHandle) =
        Node(wrapThreadStaticInfo (Async.AwaitWaitHandle(waitHandle)))

    static member Sequential(computations: NodeCode<'T> seq) =
        node {
            let results = ResizeArray()
            for computation in computations do
                let! res = computation
                results.Add(res)
            return results.ToArray()
        }

type private AgentMessage<'T> =
#if DEBUG
    | GetValue of AsyncReplyChannel<Result<'T, Exception>> * CancellationToken * stackTrace: string
#else
    | GetValue of AsyncReplyChannel<Result<'T, Exception>> * CancellationToken
#endif

type private AgentInstance<'T> = (MailboxProcessor<AgentMessage<'T>> * CancellationTokenSource)

[<RequireQualifiedAccess>]
type private AgentAction<'T> =
    | GetValue of AgentInstance<'T>
    | CachedValue of 'T

[<RequireQualifiedAccess>]
module GraphNode =

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
type GraphNode<'T> (computation: NodeCode<'T>) =

    let gate = obj ()
    let mutable computation = computation
    let mutable requestCount = 0
    let mutable cachedResult = ValueNone
    let mutable cachedResultNode = ValueNone

#if DEBUG
    let stackTrace = Environment.StackTrace
#endif

    let loop (agent: MailboxProcessor<AgentMessage<'T>>) =
        async {
            try
                while true do
                    match! agent.Receive() with
#if DEBUG
                    | GetValue (replyChannel, ct, _stackTrace) ->
#else
                    | GetValue (replyChannel, ct) ->
#endif
                        Thread.CurrentThread.CurrentUICulture <- GraphNode.culture
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
                                let! result = computation |> Async.AwaitNode
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
                            let! ct = NodeCode.CancellationToken
#if DEBUG
                            let! res = agent.PostAndAsyncReply(fun replyChannel -> GetValue(replyChannel, ct, stackTrace)) |> NodeCode.AwaitAsync
#else
                            let! res = agent.PostAndAsyncReply(fun replyChannel -> GetValue(replyChannel, ct)) |> GraphNode.AwaitAsync
#endif
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

       member _.HasValue = cachedResult.IsSome

       member _.RequestCount = requestCount