namespace FSharp.Compiler.UnitTests

open NUnit.Framework

/// Used for tracking what operations a Trace builder was asked to perform
type TraceOp =
    | Apply of arg : obj // We only capture the arg here, because function equality is awkward
    | Return // Similarly, we don't capture the arg here for reasons of function equality pain
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

    member __.GetTrace () = trace

    member __.Apply((Trace f), (Trace x) as xTrace) =
        trace <- Apply xTrace :: trace
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

[<TestFixture>]
module ApplicativeComputationExpressionDesugaring =

    [<Test>]
    let ``let! ... and! ... return ... desugars to calls to builder.Apply and builder.Return to compute its result`` () =
        let trace = TraceBuilder()

        let ceResult : int Trace =
            trace {
                let! x = Trace 1
                and! y = Trace 2
                return 1 + 2
            }

        Assert.IsTrue(
            trace.GetTrace () =
                [
                    Apply 2
                    Apply 1
                    Return
                ]
        )

        Assert.areEqual(Trace 3, ceResult)

    [<Test>]
    let ``use! ... anduse! ... return ... desugars to calls to builder.Apply, builder.Return and builder.MapUsing to compute its result`` () =
        let trace = TraceBuilder()

        let ceResult : int Trace =
            trace {
                use!    x = Trace 1
                anduse! y = Trace 2
                return 1 + 2
            }

        Assert.IsTrue(
            trace.GetTrace () =
                [
                    Apply 2
                    Apply 1
                    Return
                    EnterUsing 1
                    EnterUsing 2
                    StartUsingBody 1
                    StartUsingBody 2
                    EndUsingBody 2
                    EndUsingBody 1
                    ExitUsing 2
                    ExitUsing 1
                ]
        )

        Assert.areEqual(Trace 3, ceResult)

    [<Test>]
    let ``Mixed use of managed resource args and not desugars to calls to MapUsing only where implied by the CE syntax`` () =
        let trace = TraceBuilder()

        let ceResult : int Trace =
            trace {
                use!    x = Trace 1
                and!    y = Trace 2 // Explicitly _not_ treated as a resource to be managed
                anduse! z = Trace 3
                return 1 + 2 + 3
            }

        Assert.IsTrue(
            trace.GetTrace () =
                [
                    Apply 3
                    Apply 2
                    Apply 1
                    Return
                    EnterUsing 1
                    //EnterUsing 2
                    EnterUsing 3
                    StartUsingBody 1
                    //StartUsingBody 2
                    StartUsingBody 3
                    EndUsingBody 3
                    //EndUsingBody 2
                    EndUsingBody 1
                    ExitUsing 3
                    //ExitUsing 2
                    ExitUsing 1
                ]
        )

        Assert.areEqual(Trace 6, ceResult)
