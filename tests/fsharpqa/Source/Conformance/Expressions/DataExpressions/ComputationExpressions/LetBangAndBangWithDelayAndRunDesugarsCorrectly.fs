// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, [|Delay; MergeSources; ApplicativeBind; ApplicativeReturn; Run|]
//OUTPUT

module DelayAndRun

open ApplicativeBuilderLib

let () =
    let tracerWithDelayAndRun = TraceApplicativeWithDelayAndRun()

    let ceResult : int Trace =
        tracerWithDelayAndRun {
            let! x = Trace 3
            and! y = Trace true
            return if y then x else -1
        }

    printfn "%+A, %+A" ceResult (tracerWithDelayAndRun.GetTrace ())