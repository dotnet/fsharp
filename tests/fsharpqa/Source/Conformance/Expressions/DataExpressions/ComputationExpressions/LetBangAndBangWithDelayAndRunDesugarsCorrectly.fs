// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, .Delay; Return; Apply; Apply; Run.
//OUTPUT

open ApplicativeBuilderLib

module DelayAndRun =

    let () =
        let tracerWithDelayAndRun = TraceWithDelayAndRunBuilder()

        let ceResult : int Trace =
            tracerWithDelayAndRun {
                let! x = Trace 3
                and! y = Trace true
                return if y then x else -1
            }

        printfn "%+A, %+A" ceResult (tracerWithDelayAndRun.GetTrace ())