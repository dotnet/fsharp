namespace ApplicativeBuilderLib

/// Used for tracking what operations a Trace builder was asked to perform
type TraceOp =
    | Apply
    | Return
    | EnterUsing of resource : obj
    | StartUsingBody of resource : obj
    | EndUsingBody of resource : obj
    | ExitUsing of resource : obj
    | Bind
    | Run
    | Delay

/// A pseudo identity functor
type 'a Trace =
    | Trace of 'a
    override this.ToString () =
        sprintf "%+A" this

/// A builder which records what operations it is asked to perform
type TraceBuilder =

    val mutable trace : TraceOp list
    new () = { trace = [] }

    member __.GetTrace () = List.rev __.trace

    member __.Apply(Trace f, Trace x) =
        __.trace <- Apply :: __.trace
        Trace (f x)

    member __.Return(x) =
        __.trace <- Return :: __.trace
        Trace x

    member __.MapTryFinally(body, compensation) =
        try
            body ()
        finally
            compensation ()

    /// Doesn't actually do any disposing here, since we are just interesting
    /// in checking the order of events in this test
    member __.ApplyUsing(resource(*:#System.IDisposable*), body) =
        __.trace <- EnterUsing resource :: __.trace
        let body' = fun () ->
            __.trace <- StartUsingBody resource :: __.trace
            let res = body resource
            __.trace <- EndUsingBody resource :: __.trace
            res
        __.MapTryFinally(body', fun () ->
            __.trace <- ExitUsing resource :: __.trace
            (*resource.Dispose()*)
            ())

type TraceWithDelayAndRunBuilder() =
    inherit TraceBuilder()

    member __.Run(x) =
        __.trace <- Run :: __.trace
        x

    member __.Delay(thunk) =
        __.trace <- Delay :: __.trace
        thunk ()

type TraceWithDelayBuilder() =
    inherit TraceBuilder()

    member __.Delay(thunk) =
        __.trace <- Delay :: __.trace
        thunk ()

type TraceWithRunBuilder() =
    inherit TraceBuilder()

    member __.Run(x) =
        __.trace <- Run :: __.trace
        x

type MonadicTraceBuilder() =
    inherit TraceBuilder()

    member __.Bind(x : 'a Trace, f : 'a -> 'b Trace) : 'b Trace =
        __.trace <- Bind :: __.trace
        let (Trace x') = x
        f x'
