namespace FSharp.Compiler

open System
open System.Threading

[<Sealed>]
type Cancellable =
    static let tokenHolder = AsyncLocal<CancellationToken voption>()

    static let guard =
        String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DISABLE_CHECKANDTHROW_ASSERT"))

    static let ensureToken msg =
        tokenHolder.Value
        |> ValueOption.defaultWith (fun () -> if guard then failwith msg else CancellationToken.None)

    static member HasCancellationToken = tokenHolder.Value.IsSome

    static member Token = ensureToken "Token not available outside of Cancellable computation."

    static member UseToken() =
        async {
            let! ct = Async.CancellationToken
            return Cancellable.UsingToken ct
        }

    static member UsingToken(ct) =
        let oldCt = tokenHolder.Value
        tokenHolder.Value <- ValueSome ct

        { new IDisposable with
            member _.Dispose() = tokenHolder.Value <- oldCt
        }

    static member CheckAndThrow() =
        let token = ensureToken "CheckAndThrow invoked outside of Cancellable computation."
        token.ThrowIfCancellationRequested()

    static member TryCheckAndThrow() =
        match tokenHolder.Value with
        | ValueNone -> ()
        | ValueSome token -> token.ThrowIfCancellationRequested()

namespace Internal.Utilities.Library

open System
open System.Threading
open FSharp.Compiler

open FSharp.Core.CompilerServices.StateMachineHelpers

[<RequireQualifiedAccess; Struct>]
type ValueOrCancelled<'TResult> =
    | Value of result: 'TResult
    | Cancelled of ``exception``: OperationCanceledException

[<Struct>]
type Cancellable<'T> = Cancellable of (CancellationToken * int -> ValueOrCancelled<'T>)

module Cancellable =

    let maxDepth = 100

    let handleCheckAndThrow (ct: CancellationToken, depth) oper =
        try
            oper (ct, depth)
        with
        | :? OperationCanceledException as e when ct.IsCancellationRequested -> ValueOrCancelled.Cancelled e
        | :? OperationCanceledException as e -> InvalidOperationException("Wrong cancellation token", e) |> raise

    let run (ct: CancellationToken, depth: int) (Cancellable oper) =
        if ct.IsCancellationRequested then
            ValueOrCancelled.Cancelled(OperationCanceledException ct)
        else if depth % maxDepth = 0 then
            async {
                do! Async.SwitchToNewThread()
                return handleCheckAndThrow (ct, depth + 1) oper
            }
            |> Async.RunSynchronously
        else
            handleCheckAndThrow (ct, depth + 1) oper

    let fold f acc seq =
        Cancellable(fun state ->
            let mutable acc = ValueOrCancelled.Value acc

            for x in seq do
                match acc with
                | ValueOrCancelled.Value accv -> acc <- run state (f accv x)
                | ValueOrCancelled.Cancelled _ -> ()

            acc)

    let runWithoutCancellation comp =
        use _ = Cancellable.UsingToken CancellationToken.None
        let res = run (CancellationToken.None, 1) comp

        match res with
        | ValueOrCancelled.Cancelled _ -> failwith "unexpected cancellation"
        | ValueOrCancelled.Value r -> r

    let toAsync c =
        async {
            let! ct = Async.CancellationToken
            use! _holder = Cancellable.UseToken()
            let res = run (ct, 1) c

            return!
                Async.FromContinuations(fun (cont, _econt, ccont) ->
                    match res with
                    | ValueOrCancelled.Value v -> cont v
                    | ValueOrCancelled.Cancelled ce -> ccont ce)
        }

    let token () =
        Cancellable(fun (ct, _) -> ValueOrCancelled.Value ct)

type CancellableBuilder() =

    member inline _.Delay([<InlineIfLambda>] f) =
        Cancellable(fun state ->
            let (Cancellable g) = f ()
            g state)

    member inline _.Bind(comp, [<InlineIfLambda>] k) =
        Cancellable(fun state ->

            __debugPoint ""

            match Cancellable.run state comp with
            | ValueOrCancelled.Value v1 -> Cancellable.run state (k v1)
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.BindReturn(comp, [<InlineIfLambda>] k) =
        Cancellable(fun state ->

            __debugPoint ""

            match Cancellable.run state comp with
            | ValueOrCancelled.Value v1 -> ValueOrCancelled.Value(k v1)
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.Combine(comp1, comp2) =
        Cancellable(fun state ->

            __debugPoint ""

            match Cancellable.run state comp1 with
            | ValueOrCancelled.Value() -> Cancellable.run state comp2
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.TryWith(comp, [<InlineIfLambda>] handler) =
        Cancellable(fun state ->

            __debugPoint ""

            let compRes =
                try
                    match Cancellable.run state comp with
                    | ValueOrCancelled.Value res -> ValueOrCancelled.Value(Choice1Of2 res)
                    | ValueOrCancelled.Cancelled exn -> ValueOrCancelled.Cancelled exn
                with err ->
                    ValueOrCancelled.Value(Choice2Of2 err)

            match compRes with
            | ValueOrCancelled.Value res ->
                match res with
                | Choice1Of2 r -> ValueOrCancelled.Value r
                | Choice2Of2 err -> Cancellable.run state (handler err)
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.Using(resource, [<InlineIfLambda>] comp) =
        Cancellable(fun state ->

            __debugPoint ""

            let body = comp resource

            let compRes =
                try
                    match Cancellable.run state body with
                    | ValueOrCancelled.Value res -> ValueOrCancelled.Value(Choice1Of2 res)
                    | ValueOrCancelled.Cancelled exn -> ValueOrCancelled.Cancelled exn
                with err ->
                    ValueOrCancelled.Value(Choice2Of2 err)

            match compRes with
            | ValueOrCancelled.Value res ->
                Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicFunctions.Dispose resource

                match res with
                | Choice1Of2 r -> ValueOrCancelled.Value r
                | Choice2Of2 err -> raise err
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.TryFinally(comp, [<InlineIfLambda>] compensation) =
        Cancellable(fun state ->

            __debugPoint ""

            let compRes =
                try
                    match Cancellable.run state comp with
                    | ValueOrCancelled.Value res -> ValueOrCancelled.Value(Choice1Of2 res)
                    | ValueOrCancelled.Cancelled exn -> ValueOrCancelled.Cancelled exn
                with err ->
                    ValueOrCancelled.Value(Choice2Of2 err)

            match compRes with
            | ValueOrCancelled.Value res ->
                compensation ()

                match res with
                | Choice1Of2 r -> ValueOrCancelled.Value r
                | Choice2Of2 err -> raise err
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.Return v =
        Cancellable(fun _ -> ValueOrCancelled.Value v)

    member inline _.ReturnFrom(v: Cancellable<'T>) =
        Cancellable(fun state -> Cancellable.run state v)

    member inline _.Zero() =
        Cancellable(fun _ -> ValueOrCancelled.Value())

[<AutoOpen>]
module CancellableAutoOpens =
    let cancellable = CancellableBuilder()
