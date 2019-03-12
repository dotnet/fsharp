// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, .Return; Apply; Apply; Run.
//OUTPUT

open ApplicativeBuilderLib

module DelayAndRun =

    let () =
        let tracerWithRun = TraceWithRunBuilder()

        let ceResult : int Trace =
            tracerWithRun {
                let! x = Trace 3
                and! y = Trace true
                return if y then x else -1
            }

        printfn "%+A, %+A" ceResult (tracerWithRun.GetTrace ())