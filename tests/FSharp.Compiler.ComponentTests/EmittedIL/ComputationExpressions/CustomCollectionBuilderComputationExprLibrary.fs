namespace ComputationExpressions

#nowarn 77

open System

[<AutoOpen>]
module Library =
    type CollectionBuilderCode<'T> = delegate of byref<'T> -> unit

    type CollectionBuilder () =
        member inline _.Combine ([<InlineIfLambda>] f1 : CollectionBuilderCode<_>, [<InlineIfLambda>] f2 : CollectionBuilderCode<_>) =
            CollectionBuilderCode (fun sm -> f1.Invoke &sm; f2.Invoke &sm)

        member inline _.Delay ([<InlineIfLambda>] f : unit -> CollectionBuilderCode<_>) =
            CollectionBuilderCode (fun sm -> (f ()).Invoke &sm)

        member inline _.Zero () = CollectionBuilderCode (fun _ -> ())

        member inline _.While ([<InlineIfLambda>] condition : unit -> bool, [<InlineIfLambda>] body : CollectionBuilderCode<_>) =
            CollectionBuilderCode (fun sm ->
                while condition () do
                    body.Invoke &sm)

        member inline _.TryWith ([<InlineIfLambda>] body : CollectionBuilderCode<_>, [<InlineIfLambda>] handle : exn -> CollectionBuilderCode<_>) =
            CollectionBuilderCode (fun sm ->
                try body.Invoke &sm
                with e -> (handle e).Invoke &sm)

        member inline _.TryFinally ([<InlineIfLambda>] body : CollectionBuilderCode<_>, compensation : unit -> unit) =
            CollectionBuilderCode (fun sm ->
                try body.Invoke &sm
                with _ ->
                    compensation ()
                    reraise ()
                compensation ())

        member inline builder.Using (disposable : #IDisposable, [<InlineIfLambda>] body : #IDisposable -> CollectionBuilderCode<_>) =
            builder.TryFinally ((fun sm -> (body disposable).Invoke &sm), (fun () -> if not (isNull (box disposable)) then disposable.Dispose ()))

        member inline _.For (resizeArray : ResizeArray<_>, [<InlineIfLambda>] body : _ -> CollectionBuilderCode<_>) =
            CollectionBuilderCode (fun sm ->
                for i in 0 .. resizeArray.Count - 1 do
                    (body resizeArray[i]).Invoke &sm)

        member inline _.Yield x = CollectionBuilderCode (fun sm -> ignore (^a : (member Add : ^b -> _) (sm, x)))

        member inline builder.YieldFrom xs = CollectionBuilderCode (fun sm -> ignore (^a : (member AddRange : ^b -> _) (sm, xs)))

    [<Sealed>]
    type ResizeArrayBuilder<'T> () =
        inherit CollectionBuilder ()
        static member val Instance = ResizeArrayBuilder<'T> ()
        member inline _.Run ([<InlineIfLambda>] f : CollectionBuilderCode<_>) =
            let mutable sm = ResizeArray<'T> ()
            f.Invoke &sm
            sm

    let resizeArray<'T> = ResizeArrayBuilder<'T>.Instance
