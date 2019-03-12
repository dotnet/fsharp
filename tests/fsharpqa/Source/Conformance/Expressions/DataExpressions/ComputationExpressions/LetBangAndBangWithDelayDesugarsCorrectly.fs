// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, .Delay; Return; Apply; Apply.
//OUTPUT

open ApplicativeBuilderLib

module DelayAndRun =

    let () =
        let tracerWithDelay = TraceWithDelayBuilder()

        let ceResult : int Trace =
            tracerWithDelay {
                let! x = Trace 3
                and! y = Trace true
                return if y then x else -1
            }

        printfn "%+A, %+A" ceResult (tracerWithDelay.GetTrace ())