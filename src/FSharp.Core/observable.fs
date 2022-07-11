// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

open System
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Control

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Observable =

    let inline protect f succeed fail =
        match
            (try
                Choice1Of2(f ())
             with e ->
                 Choice2Of2 e)
        with
        | Choice1Of2 x -> (succeed x)
        | Choice2Of2 e -> (fail e)

    [<AbstractClass>]
    type BasicObserver<'T>() =

        let mutable stopped = false

        abstract Next: value: 'T -> unit

        abstract Error: error: exn -> unit

        abstract Completed: unit -> unit

        interface IObserver<'T> with

            member x.OnNext value =
                if not stopped then
                    x.Next value

            member x.OnError e =
                if not stopped then
                    stopped <- true
                    x.Error e

            member x.OnCompleted() =
                if not stopped then
                    stopped <- true
                    x.Completed()

    [<CompiledName("Map")>]
    let map mapping (source: IObservable<'T>) =
        { new IObservable<'U> with
            member x.Subscribe(observer) =
                source.Subscribe
                    { new BasicObserver<'T>() with

                        member x.Next(v) =
                            protect (fun () -> mapping v) observer.OnNext observer.OnError

                        member x.Error(e) =
                            observer.OnError(e)

                        member x.Completed() =
                            observer.OnCompleted()
                    }
        }

    [<CompiledName("Choose")>]
    let choose chooser (source: IObservable<'T>) =
        { new IObservable<'U> with
            member x.Subscribe(observer) =
                source.Subscribe
                    { new BasicObserver<'T>() with

                        member x.Next(v) =
                            protect
                                (fun () -> chooser v)
                                (function
                                | None -> ()
                                | Some v2 -> observer.OnNext v2)
                                observer.OnError

                        member x.Error(e) =
                            observer.OnError(e)

                        member x.Completed() =
                            observer.OnCompleted()
                    }
        }

    [<CompiledName("Filter")>]
    let filter predicate (source: IObservable<'T>) =
        choose (fun x -> if predicate x then Some x else None) source

    [<CompiledName("Partition")>]
    let partition predicate (source: IObservable<'T>) =
        filter predicate source, filter (predicate >> not) source

    [<CompiledName("Scan")>]
    let scan collector state (source: IObservable<'T>) =
        { new IObservable<'U> with
            member x.Subscribe(observer) =
                let mutable state = state

                source.Subscribe
                    { new BasicObserver<'T>() with

                        member x.Next(v) =
                            let z = state

                            protect
                                (fun () -> collector z v)
                                (fun z ->
                                    state <- z
                                    observer.OnNext z)
                                observer.OnError

                        member x.Error(e) =
                            observer.OnError(e)

                        member x.Completed() =
                            observer.OnCompleted()
                    }
        }

    [<CompiledName("Add")>]
    let add callback (source: IObservable<'T>) =
        source.Add(callback)

    [<CompiledName("Subscribe")>]
    let subscribe (callback: 'T -> unit) (source: IObservable<'T>) =
        source.Subscribe(callback)

    [<CompiledName("Pairwise")>]
    let pairwise (source: IObservable<'T>) : IObservable<'T * 'T> =
        { new IObservable<_> with
            member x.Subscribe(observer) =
                let mutable lastArgs = None

                source.Subscribe
                    { new BasicObserver<'T>() with

                        member x.Next(args2) =
                            match lastArgs with
                            | None -> ()
                            | Some args1 -> observer.OnNext(args1, args2)

                            lastArgs <- Some args2

                        member x.Error(e) =
                            observer.OnError(e)

                        member x.Completed() =
                            observer.OnCompleted()
                    }
        }

    [<CompiledName("Merge")>]
    let merge (source1: IObservable<'T>) (source2: IObservable<'T>) =
        { new IObservable<_> with
            member x.Subscribe(observer) =
                let mutable stopped = false
                let mutable completed1 = false
                let mutable completed2 = false

                let h1 =
                    source1.Subscribe
                        { new IObserver<'T> with
                            member x.OnNext(v) =
                                if not stopped then
                                    observer.OnNext v

                            member x.OnError(e) =
                                if not stopped then
                                    stopped <- true
                                    observer.OnError(e)

                            member x.OnCompleted() =
                                if not stopped then
                                    completed1 <- true

                                    if completed1 && completed2 then
                                        stopped <- true
                                        observer.OnCompleted()
                        }

                let h2 =
                    source2.Subscribe
                        { new IObserver<'T> with
                            member x.OnNext(v) =
                                if not stopped then
                                    observer.OnNext v

                            member x.OnError(e) =
                                if not stopped then
                                    stopped <- true
                                    observer.OnError(e)

                            member x.OnCompleted() =
                                if not stopped then
                                    completed2 <- true

                                    if completed1 && completed2 then
                                        stopped <- true
                                        observer.OnCompleted()
                        }

                { new IDisposable with
                    member x.Dispose() =
                        h1.Dispose()
                        h2.Dispose()
                }
        }

    [<CompiledName("Split")>]
    let split (splitter: 'T -> Choice<'U1, 'U2>) (source: IObservable<'T>) =
        choose
            (fun v ->
                match splitter v with
                | Choice1Of2 x -> Some x
                | _ -> None)
            source,
        choose
            (fun v ->
                match splitter v with
                | Choice2Of2 x -> Some x
                | _ -> None)
            source
