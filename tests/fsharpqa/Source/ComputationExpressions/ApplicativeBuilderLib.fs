namespace ApplicativeBuilderLib

/// Used for tracking what operations a Trace builder was asked to perform
type TraceOp =
    | Apply
    | Return
    | EnterUsing of resource : obj
    | StartUsingBody of resource : obj
    | EndUsingBody of resource : obj
    | ExitUsing of resource : obj

/// A pseudo identity functor
type 'a Trace =
    Trace of 'a
    with
    override this.ToString () =
        sprintf "%+A" this

/// A builder which records what operations it is asked to perform
type TraceBuilder() =

    let mutable trace : TraceOp list = []

    member __.GetTrace () = List.rev trace

    member __.Apply((Trace f) as fTrace, (Trace x) as xTrace) =
        trace <- Apply :: trace
        Trace (f x)

    member __.Return(x) =
        trace <- Return :: trace
        Trace x

    member __.MapTryFinally(body, compensation) =
        try
            body ()
        finally
            compensation ()

    /// Doesn't actually do any disposing here, since we are just interesting
    /// in checking the order of events in this test
    member __.MapUsing(resource(*:#System.IDisposable*), body) =
        trace <- EnterUsing resource :: trace
        let body' = fun () ->
            trace <- StartUsingBody resource :: trace
            let res = body resource
            trace <- EndUsingBody resource :: trace
            res
        __.MapTryFinally(body', fun () ->
            trace <- ExitUsing resource :: trace
            (*resource.Dispose()*)
            ())
