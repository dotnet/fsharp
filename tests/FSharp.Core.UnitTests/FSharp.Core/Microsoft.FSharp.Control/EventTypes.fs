// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Control event types

namespace FSharp.Core.UnitTests.Control

open System
open System.Reflection
open Xunit

module private EventTypes =
    type MultiArgDelegate = delegate of obj * obj[] -> unit

    let getListeners event =
        let eventType = event.GetType()
        let multicastField =
            eventType
                .GetField("multicast", BindingFlags.NonPublic ||| BindingFlags.Instance)
                .GetValue event
                :?> System.Delegate

        if not (isNull multicastField) then
            let multicastType = typeof<System.MulticastDelegate>
            let listeners =
                multicastType
                    .GetMethod("GetInvocationList")
                    .Invoke(multicastField, [||])
                    :?> System.Delegate []
            Some listeners
        else
            None

type EventTypes() =

    [<Literal>]
    let RunsCount = 100

    let runAddRemoveHandlers (event: IDelegateEvent<_>) handlerInitializer =
        seq {
            for _ in 1 .. RunsCount do
                async {
                    let h = handlerInitializer()
                    event.AddHandler(h)
                    event.RemoveHandler(h)
                }
        } |> Async.Parallel |> Async.RunSynchronously |> ignore

    let checkHasSubscribers (hasSubscribers: unit -> bool) (event: IDelegateEvent<_>) handlerInitializer =
        let first = handlerInitializer()
        let second = handlerInitializer()
        Assert.False(hasSubscribers ())
        event.AddHandler(first)
        Assert.True(hasSubscribers ())
        event.AddHandler(second)
        event.RemoveHandler(first)
        Assert.True(hasSubscribers ())
        event.RemoveHandler(second)
        Assert.False(hasSubscribers ())

    [<Fact>]
    member this.``Adding/removing handlers to published Event<'T> is thread-safe``() =
        let event = new Event<int>()
        let listenersBefore = EventTypes.getListeners event
        runAddRemoveHandlers (event.Publish) (fun _ -> new Handler<_>(fun sender args -> ()))
        let listenersAfter = EventTypes.getListeners event

        Assert.True(listenersBefore.IsNone)
        Assert.True(listenersAfter.IsNone)

    [<Fact>]
    member this.``Adding/removing handlers to published DelegateEvent is thread-safe``() =
        let event = new DelegateEvent<_>()
        let listenersBefore = EventTypes.getListeners event
        runAddRemoveHandlers (event.Publish) (fun _ -> EventTypes.MultiArgDelegate(fun sender args -> ()))
        let listenersAfter = EventTypes.getListeners event

        Assert.True(listenersBefore.IsNone)
        Assert.True(listenersAfter.IsNone)

    [<Fact>]
    member this.``Adding/removing handlers to published Event<'D,'A> is thread-safe``() =
        let event = new Event<_, _>()
        let listenersBefore = EventTypes.getListeners event
        runAddRemoveHandlers (event.Publish) (fun _ -> EventTypes.MultiArgDelegate(fun sender args -> ()))
        let listenersAfter = EventTypes.getListeners event

        Assert.True(listenersBefore.IsNone)
        Assert.True(listenersAfter.IsNone)

    [<Fact>]
    member this.``HasSubscribers of Event<'T> follows its handlers``() =
        let event = new Event<int>()
        checkHasSubscribers (fun () -> event.HasSubscribers) event.Publish (fun _ -> new Handler<_>(fun sender args -> ()))

    [<Fact>]
    member this.``HasSubscribers of DelegateEvent follows its handlers``() =
        let event = new DelegateEvent<EventTypes.MultiArgDelegate>()
        checkHasSubscribers (fun () -> event.HasSubscribers) event.Publish (fun _ -> EventTypes.MultiArgDelegate(fun sender args -> ()))

    [<Fact>]
    member this.``HasSubscribers of Event<'D,'A> follows its handlers``() =
        let event = new Event<EventTypes.MultiArgDelegate, obj[]>()
        checkHasSubscribers (fun () -> event.HasSubscribers) event.Publish (fun _ -> EventTypes.MultiArgDelegate(fun sender args -> ()))

    [<Fact>]
    member this.``HasSubscribers of Event<'T> follows its subscriptions``() =
        let event = new Event<int>()
        let subscription = event.Publish.Subscribe(fun _ -> ())
        Assert.True(event.HasSubscribers)
        subscription.Dispose()
        Assert.False(event.HasSubscribers)

    [<Fact>]
    member this.``HasSubscribers of Event<'D,'A> follows its subscriptions``() =
        let event = new Event<EventTypes.MultiArgDelegate, obj[]>()
        let subscription = event.Publish.Subscribe(fun _ -> ())
        Assert.True(event.HasSubscribers)
        subscription.Dispose()
        Assert.False(event.HasSubscribers)
