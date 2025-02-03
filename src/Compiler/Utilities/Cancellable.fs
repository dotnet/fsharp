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
            tokenHolder.Value <- ValueSome ct
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

#if !FSHARPCORE_USE_PACKAGE
open FSharp.Core.CompilerServices.StateMachineHelpers
#endif

[<RequireQualifiedAccess; Struct>]
type ValueOrCancelled<'TResult> =
    | Value of result: 'TResult
    | Cancelled of ``exception``: OperationCanceledException

[<Struct>]
type Cancellable<'T> = Cancellable of (CancellationToken -> ValueOrCancelled<'T>)

module Cancellable =

    let inline run (ct: CancellationToken) (Cancellable oper) =
        if ct.IsCancellationRequested then
            ValueOrCancelled.Cancelled(OperationCanceledException ct)
        else
            try
                use _ = Cancellable.UsingToken(ct)
                oper ct
            with
            | :? OperationCanceledException as e when ct.IsCancellationRequested -> ValueOrCancelled.Cancelled e
            | :? OperationCanceledException as e -> InvalidOperationException("Wrong cancellation token", e) |> raise

    let fold f acc seq =
        Cancellable(fun ct ->
            let mutable acc = ValueOrCancelled.Value acc

            for x in seq do
                match acc with
                | ValueOrCancelled.Value accv -> acc <- run ct (f accv x)
                | ValueOrCancelled.Cancelled _ -> ()

            acc)

    let runWithoutCancellation comp =
        let res = run CancellationToken.None comp

        match res with
        | ValueOrCancelled.Cancelled _ -> failwith "unexpected cancellation"
        | ValueOrCancelled.Value r -> r

    let toAsync c =
        async {
            let! ct = Async.CancellationToken
            let res = run ct c

            return!
                Async.FromContinuations(fun (cont, _econt, ccont) ->
                    match res with
                    | ValueOrCancelled.Value v -> cont v
                    | ValueOrCancelled.Cancelled ce -> ccont ce)
        }

    let token () = Cancellable(ValueOrCancelled.Value)

type CancellableBuilder() =

    member inline _.Delay([<InlineIfLambda>] f) =
        Cancellable(fun ct ->
            let (Cancellable g) = f ()
            g ct)

    member inline _.Bind(comp, [<InlineIfLambda>] k) =
        Cancellable(fun ct ->
#if !FSHARPCORE_USE_PACKAGE
            __debugPoint ""
#endif

            match Cancellable.run ct comp with
            | ValueOrCancelled.Value v1 -> Cancellable.run ct (k v1)
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.BindReturn(comp, [<InlineIfLambda>] k) =
        Cancellable(fun ct ->
#if !FSHARPCORE_USE_PACKAGE
            __debugPoint ""
#endif

            match Cancellable.run ct comp with
            | ValueOrCancelled.Value v1 -> ValueOrCancelled.Value(k v1)
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.Combine(comp1, comp2) =
        Cancellable(fun ct ->
#if !FSHARPCORE_USE_PACKAGE
            __debugPoint ""
#endif

            match Cancellable.run ct comp1 with
            | ValueOrCancelled.Value() -> Cancellable.run ct comp2
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.TryWith(comp, [<InlineIfLambda>] handler) =
        Cancellable(fun ct ->
#if !FSHARPCORE_USE_PACKAGE
            __debugPoint ""
#endif

            let compRes =
                try
                    match Cancellable.run ct comp with
                    | ValueOrCancelled.Value res -> ValueOrCancelled.Value(Choice1Of2 res)
                    | ValueOrCancelled.Cancelled exn -> ValueOrCancelled.Cancelled exn
                with err ->
                    ValueOrCancelled.Value(Choice2Of2 err)

            match compRes with
            | ValueOrCancelled.Value res ->
                match res with
                | Choice1Of2 r -> ValueOrCancelled.Value r
                | Choice2Of2 err -> Cancellable.run ct (handler err)
            | ValueOrCancelled.Cancelled err1 -> ValueOrCancelled.Cancelled err1)

    member inline _.Using(resource, [<InlineIfLambda>] comp) =
        Cancellable(fun ct ->
#if !FSHARPCORE_USE_PACKAGE
            __debugPoint ""
#endif
            let body = comp resource

            let compRes =
                try
                    match Cancellable.run ct body with
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
        Cancellable(fun ct ->
#if !FSHARPCORE_USE_PACKAGE
            __debugPoint ""
#endif

            let compRes =
                try
                    match Cancellable.run ct comp with
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

    member inline _.ReturnFrom(v: Cancellable<'T>) = v

    member inline _.Zero() =
        Cancellable(fun _ -> ValueOrCancelled.Value())

[<AutoOpen>]
module CancellableAutoOpens =
    let cancellable = CancellableBuilder()
