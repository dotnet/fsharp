namespace ApplicativeBuilderLib

/// Used for tracking what operations a Trace builder was asked to perform
[<RequireQualifiedAccess>]
type TraceOp =
    | ApplicativeBind
    | ApplicativeReturn
    | ApplicativeCombine
    | ApplicativeYield of obj
    | EnterUsing of resource : obj
    | StartUsingBody of resource : obj
    | EndUsingBody of resource : obj
    | ExitUsing of resource : obj
    | MergeSources
    | MonadicBind
    | MonadicReturn
    | Run
    | Delay

/// A pseudo identity functor
type Trace<'a> =
    | Trace of 'a
    override this.ToString () =
        sprintf "%+A" this

/// A builder which records what operations it is asked to perform
type TraceCore() =

    let mutable trace = ResizeArray<_>()

    member builder.GetTrace () = trace.ToArray()

    member builder.Trace x = trace.Add(x)

(*
    member builder.MapTryFinally(body, compensation) =
        try
            body ()
        finally
            compensation ()

    /// Doesn't actually do any disposing here, since we are just interesting
    /// in checking the order of events in this test
    member builder.ApplyUsing(resource, body) =
        builder.Trace TraceOp.EnterUsing resource
        let body' = fun () ->
            builder.Trace TraceOp.StartUsingBody resource
            let res = body resource
            builder.Trace TraceOp.EndUsingBody resource
            res
        builder.MapTryFinally(body', fun () ->
            builder.Trace TraceOp.ExitUsing resource
            (*resource.Dispose()*)
            ())
*)

type TraceApplicativeCore() =
    inherit TraceCore()

    member builder.MergeSources(Trace x1, Trace x2) =
        builder.Trace TraceOp.MergeSources
        Trace (x1, x2)

    member builder.Bind(Trace x, f) =
        builder.Trace TraceOp.ApplicativeBind
        Trace (f x)


type TraceApplicative() =
    inherit TraceCore()

    member builder.MergeSources(Trace x1, Trace x2) =
        builder.Trace TraceOp.MergeSources
        Trace (x1, x2)

    member builder.Bind(Trace x, f) =
        builder.Trace TraceOp.ApplicativeBind
        Trace (f x)

    member builder.Return(x) =
        builder.Trace TraceOp.ApplicativeReturn
        x
type TraceApplicativeMonoid() =
    inherit TraceApplicativeCore()

    member builder.Yield(x) =
        builder.Trace (TraceOp.ApplicativeYield x)
        [x]

    member builder.Combine(Trace x1, Trace x2) =
        builder.Trace TraceOp.ApplicativeCombine
        Trace (x1 @ x2)

type TraceApplicativeWithDelayAndRun() =
    inherit TraceApplicative()

    member builder.Run(x) =
        builder.Trace TraceOp.Run
        x

    member builder.Delay(thunk) =
        builder.Trace TraceOp.Delay
        thunk ()

type TraceApplicativeWithDelay() =
    inherit TraceApplicative()

    member builder.Delay(thunk) =
        builder.Trace TraceOp.Delay
        thunk ()

type TraceApplicativeWithRun() =
    inherit TraceApplicative()

    member builder.Run(x) =
        builder.Trace TraceOp.Run
        x

type TraceMonadic() =
    inherit TraceCore()

    member builder.MergeSources(Trace x1, Trace x2) =
        builder.Trace TraceOp.MergeSources
        Trace (x1, x2)

    member builder.Bind(x : 'a Trace, f : 'a -> 'b Trace) : 'b Trace =
        builder.Trace TraceOp.MonadicBind
        let (Trace x') = x
        f x'

    member builder.Return(x: 'T) : Trace<'T> =
        builder.Trace TraceOp.MonadicReturn
        Trace x

