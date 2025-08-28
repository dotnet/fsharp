namespace Internal.Utilities.Library

open System
open System.Threading
open System.Threading.Tasks

#nowarn 3513

type Async2 =
    static let token = AsyncLocal<CancellationToken>()
    static member UseToken ct = token.Value <- ct
    static member val Token = token.Value

module internal Async2Implementation =

    open FSharp.Core.CompilerServices.StateMachineHelpers

    open Microsoft.FSharp.Core.CompilerServices
    open System.Runtime.CompilerServices
    open System.Runtime.ExceptionServices

    /// A structure that looks like an Awaiter
    type Awaiter<'Awaiter, 'TResult
        when 'Awaiter :> ICriticalNotifyCompletion
        and 'Awaiter: (member get_IsCompleted: unit -> bool)
        and 'Awaiter: (member GetResult: unit -> 'TResult)> = 'Awaiter

    type Awaitable<'Awaitable, 'Awaiter, 'TResult
        when 'Awaitable: (member GetAwaiter: unit -> Awaiter<'Awaiter, 'TResult>)> = 'Awaitable

    module Awaiter =
        let inline isCompleted (awaiter: ^Awaiter) : bool
            when ^Awaiter : (member get_IsCompleted : unit -> bool) =
                awaiter.get_IsCompleted()

        let inline getResult (awaiter: ^Awaiter) : ^TResult
            when ^Awaiter : (member GetResult : unit -> ^TResult) =
                awaiter.GetResult()

        let inline onCompleted (awaiter: ^Awaiter) (continuation: Action) : unit
            when ^Awaiter :> INotifyCompletion =
                awaiter.OnCompleted continuation

        let inline unsafeOnCompleted (awaiter: ^Awaiter) (continuation: Action) : unit
            when ^Awaiter :> ICriticalNotifyCompletion =
                awaiter.UnsafeOnCompleted continuation

    type Trampoline private () =
    
        let ownerThreadId = Thread.CurrentThread.ManagedThreadId
        
        static let holder = new ThreadLocal<_>(fun () -> Trampoline())
    
        let mutable pending : Action voption = ValueNone
        let mutable running = false
    
        let start (action: Action) =
            try 
                running <- true
                action.Invoke()
                while pending.IsSome do
                    let next = pending.Value
                    pending <- ValueNone
                    next.Invoke()
            finally
                running <- false
    
        let set action =
            assert (Thread.CurrentThread.ManagedThreadId = ownerThreadId)
            assert pending.IsNone
            if running then
                pending <- ValueSome action
            else
                start action
    
        interface ICriticalNotifyCompletion with
            member _.OnCompleted(continuation) = set continuation
            member _.UnsafeOnCompleted(continuation) = set continuation
    
        static member Current = holder.Value
    
    module Trampoline =
        let Awaiter : ICriticalNotifyCompletion = Trampoline.Current
        let AwaiterRef = ref Awaiter
    
    module BindContext =
        [<Literal>]
        let bindLimit = 100
    
        let bindCount = new ThreadLocal<int>()
      
        let inline IncrementBindCount () =
            bindCount.Value <- bindCount.Value + 1
            bindCount.Value % bindLimit = 0
    
    module ExceptionCache =
        let store = ConditionalWeakTable<exn, ExceptionDispatchInfo>()
    
        let inline CaptureOrRetrieve (exn: exn) =
            match store.TryGetValue exn with
            | true, edi when edi.SourceException = exn -> edi
            | _ ->
                let edi = ExceptionDispatchInfo.Capture exn
    
                try store.Add(exn, edi) with _ -> ()
    
                edi
    
        let inline Throw(exn: exn) =
            let edi = CaptureOrRetrieve exn
            edi.Throw()
            Unchecked.defaultof<_>
    
        let inline GetResultOrThrow awaiter = try Awaiter.getResult awaiter with exn -> Throw exn

    [<Struct; NoComparison>]
    type internal Async2<'T> (start: bool -> Task<'T>) =
    
        member _.Start() = start false
        member _.GetAwaiter() =
            let hijack = BindContext.IncrementBindCount()
            (start hijack).GetAwaiter()
    
    [<Struct>]
    type Async2Data<'t> =
        [<DefaultValue(false)>]
        val mutable Result: 't
    
        [<DefaultValue(false)>]
        val mutable MethodBuilder: AsyncTaskMethodBuilder<'t>
    
        [<DefaultValue(false)>]
        val mutable Hijack: bool
    
    type Async2StateMachine<'TOverall> = ResumableStateMachine<Async2Data<'TOverall>>
    type IAsync2StateMachine<'TOverall> = IResumableStateMachine<Async2Data<'TOverall>>
    type Async2ResumptionFunc<'TOverall> = ResumptionFunc<Async2Data<'TOverall>>
    type Async2ResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<Async2Data<'TOverall>>
    type Async2Code<'TOverall, 'T> = ResumableCode<Async2Data<'TOverall>, 'T>  
    
    [<AutoOpen>]
    module Async2Code =
        let inline filterCancellation (catch: exn -> Async2Code<_, _>) (exn: exn) =
            Async2Code(fun sm ->
                match exn with
                | :? OperationCanceledException as oce when oce.CancellationToken = Async2.Token -> raise exn
                | _ -> (catch exn).Invoke(&sm))
    
        let inline throwIfCancellationRequested (code: Async2Code<_, _>) =
            Async2Code(fun sm ->
                Async2.Token.ThrowIfCancellationRequested()
                code.Invoke(&sm))
    
        [<Struct>]
        type DynamicState =
            | InitialYield
            | Running
            | SetResult
            | SetException of ExceptionDispatchInfo
    
        [<Struct>]
        type DynamicContinuation =
            | Stop
            | Immediate
            | Await of ICriticalNotifyCompletion
    
        let inline yieldOnBindLimit () =
            Async2Code<_, _>(fun sm ->
                if sm.Data.Hijack then
                    let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                    if not __stack_yield_fin then
                        sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(Trampoline.AwaiterRef, &sm)
                    __stack_yield_fin
                else
                    true
            )
    
    type Async2Builder() =
    
        member inline _.Delay(generator: unit -> Async2Code<'TOverall, 'T>) : Async2Code<'TOverall, 'T> =
            ResumableCode.Delay(fun () -> generator () |> throwIfCancellationRequested)
    
        [<DefaultValue>]
        member inline _.Zero() : Async2Code<'TOverall, unit> = ResumableCode.Zero()
    
        member inline _.Return(value: 'T) = Async2Code(fun sm -> sm.Data.Result <- value; true)
    
        member inline _.Combine
            (code1: Async2Code<'TOverall, unit>, code2: Async2Code<'TOverall, 'T>)
            : Async2Code<'TOverall, 'T> =
            ResumableCode.Combine(code1, code2)
    
        member inline _.While
            ([<InlineIfLambda>] condition: unit -> bool, body: Async2Code<'TOverall, unit>)
            : Async2Code<'TOverall, unit> =
            ResumableCode.While(condition, throwIfCancellationRequested body)
    
        member inline _.TryWith
            (body: Async2Code<'TOverall, 'T>, catch: exn -> Async2Code<'TOverall, 'T>)
            : Async2Code<'TOverall, 'T> =
            ResumableCode.TryWith(body, filterCancellation catch)
    
        member inline _.TryFinally
            (body: Async2Code<'TOverall, 'T>, [<InlineIfLambda>] compensation: unit -> unit)
            : Async2Code<'TOverall, 'T> =
            ResumableCode.TryFinally(
                body,
                ResumableCode<_, _>(fun _sm ->
                    compensation ()
                    true)
            )
    
        member inline _.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable | null>
            (resource: 'Resource, body: 'Resource -> Async2Code<'TOverall, 'T>)
            : Async2Code<'TOverall, 'T> =
            ResumableCode.Using(resource, body)
    
        member inline _.For(sequence: seq<'T>, body: 'T -> Async2Code<'TOverall, unit>) : Async2Code<'TOverall, unit> =
            ResumableCode.For(sequence, fun x -> body x |> throwIfCancellationRequested)
    
        [<NoEagerConstraintApplication>]
        static member inline BindDynamic(sm: byref<Async2StateMachine<_>>, awaiter, continuation: _ -> Async2Code<_, _>) =
            if Awaiter.isCompleted awaiter then
                (Awaiter.getResult awaiter |> continuation).Invoke(&sm)
            else
                let resumptionFunc = Async2ResumptionFunc(fun sm ->
                    let result = ExceptionCache.GetResultOrThrow awaiter
                    (continuation result).Invoke(&sm))
                sm.ResumptionDynamicInfo.ResumptionFunc <- resumptionFunc
                sm.ResumptionDynamicInfo.ResumptionData <- awaiter :> ICriticalNotifyCompletion
                false
    
        [<NoEagerConstraintApplication>]
        member inline _.Bind
            (awaiter, continuation: 'U -> Async2Code<'Data, 'T>)
            : Async2Code<'Data, 'T> =
                Async2Code(fun sm ->
                    if __useResumableCode then
                        if Awaiter.isCompleted awaiter then
                            continuation(ExceptionCache.GetResultOrThrow awaiter).Invoke(&sm)
                        else
                            let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                            if __stack_yield_fin then
                                continuation(ExceptionCache.GetResultOrThrow awaiter).Invoke(&sm)
                            else
                                let mutable __stack_awaiter = awaiter
                                sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&__stack_awaiter, &sm)
                                false
                    else
                        Async2Builder.BindDynamic(&sm, awaiter, continuation)
                )
    
        [<NoEagerConstraintApplication>]
        member inline this.ReturnFrom(awaiter) : Async2Code<'T, 'T> = this.Bind(awaiter, this.Return)
    
        static member inline RunDynamic(code: Async2Code<'T, 'T>) : Async2<'T> =
            let initialResumptionFunc = Async2ResumptionFunc<'T>(fun sm -> code.Invoke &sm)
    
            let resumptionInfo () =
                let mutable state = InitialYield
                { new Async2ResumptionDynamicInfo<'T>(initialResumptionFunc) with
                    member info.MoveNext(sm) =
                        let mutable continuation = Stop
    
                        let hijackCheck = if sm.Data.Hijack then Await Trampoline.Awaiter else Immediate
    
                        let current = state
                        match current with
                        | InitialYield ->
                            state <- Running
                            continuation <- hijackCheck
                        | Running ->
                            try
                                let step =  info.ResumptionFunc.Invoke(&sm)
                                if step then
                                    state  <- SetResult
                                    continuation <-  hijackCheck
                                else
                                    match info.ResumptionData with
                                    | :? ICriticalNotifyCompletion as awaiter -> continuation <- Await awaiter
                                    | _ -> failwith "invalid awaiter"
                            with exn ->
                                state <- SetException (ExceptionCache.CaptureOrRetrieve exn)
                                continuation <-  hijackCheck
                        | SetResult -> sm.Data.MethodBuilder.SetResult sm.Data.Result
                        | SetException edi -> sm.Data.MethodBuilder.SetException(edi.SourceException)
    
                        match continuation with
                        | Await awaiter ->
                            sm.ResumptionDynamicInfo.ResumptionData <- null
                            let mutable awaiter = awaiter
                            sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                        | Immediate -> info.MoveNext &sm
                        | Stop -> ()
    
                    member _.SetStateMachine(sm, state) =
                        sm.Data.MethodBuilder.SetStateMachine(state)
                }

            Async2(fun hijack ->
                let mutable copy = Async2StateMachine()
                copy.ResumptionDynamicInfo <- resumptionInfo ()
                copy.Data <- Async2Data()
                copy.Data.Hijack <- hijack
                copy.Data.MethodBuilder <- AsyncTaskMethodBuilder<'T>.Create()
                copy.Data.MethodBuilder.Start(&copy)
                copy.Data.MethodBuilder.Task)
    
        member inline _.Run(code: Async2Code<'T, 'T>) : Async2<'T> =
            if __useResumableCode then
                __stateMachine<Async2Data<_>, _>
    
                    (MoveNextMethodImpl<_>(fun sm ->
                        __resumeAt sm.ResumptionPoint
                        let mutable error = ValueNone
    
                        let __stack_go1 = yieldOnBindLimit().Invoke(&sm)
                        if __stack_go1 then
                            try
                                let __stack_code_fin = code.Invoke(&sm)
                                if __stack_code_fin then
                                    let __stack_go2 = yieldOnBindLimit().Invoke(&sm)
                                    if __stack_go2 then
                                        sm.Data.MethodBuilder.SetResult(sm.Data.Result)
                            with exn ->
                                error  <- ValueSome (ExceptionCache.CaptureOrRetrieve exn)
    
                            if error.IsSome then
                                let __stack_go2 =  yieldOnBindLimit().Invoke(&sm)
                                if __stack_go2 then
                                    sm.Data.MethodBuilder.SetException(error.Value.SourceException)
                    ))
    
                    (SetStateMachineMethodImpl<_>(fun sm state -> sm.Data.MethodBuilder.SetStateMachine state))
    
                    (AfterCode<_, _>(fun sm ->
                        let mutable copy = sm
                        Async2(fun hijack ->
                            copy.Data <- Async2Data()
                            copy.Data.Hijack <- hijack
                            copy.Data.MethodBuilder <- AsyncTaskMethodBuilder<'T>.Create()
                            copy.Data.MethodBuilder.Start(&copy)
                            copy.Data.MethodBuilder.Task)
                    ))
            else
                Async2Builder.RunDynamic(code)

        member inline _.Source(code: Async2<_>) = code.GetAwaiter()
    
    [<AutoOpen>]
    module SourceExtensions =
        type Async2Builder with      
            member inline _.Source(awaitable: Awaitable<_, _, _>) = awaitable.GetAwaiter()
            member inline _.Source(task: Task) = task.GetAwaiter()
            member inline _.Source(items: #seq<_>) : seq<_> = upcast items  
    
[<AutoOpen>]
module internal Async2AutoOpens =
    open Async2Implementation

    let async2 = Async2Builder()

type Async2<'t> = Async2Implementation.Async2<'t>

module internal Async2 =
    open Async2Implementation

    let run ct (code: Async2<'t>) =
        Async2.UseToken ct
        if isNull SynchronizationContext.Current && TaskScheduler.Current = TaskScheduler.Default then
            code.Start().GetAwaiter().GetResult()
        else       
            Task.Run<'t>(code.Start).GetAwaiter().GetResult()

    let startAsTask (code: Async2<'t>) = code.Start()

    let runWithoutCancellation code = run CancellationToken.None code
