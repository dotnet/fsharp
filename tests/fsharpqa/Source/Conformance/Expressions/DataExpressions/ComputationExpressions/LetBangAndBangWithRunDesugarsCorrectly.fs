// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, [|MergeSources; ApplicativeBind; ApplicativeReturn; Run|]
//OUTPUT

module ApplicativeWithRun

open ApplicativeBuilderLib

let () =
    let tracerWithRun = TraceApplicativeWithRun()

    let ceResult : int Trace =
        tracerWithRun {
            let! x = Trace 3
            and! y = Trace true
            return if y then x else -1
        }

    printfn "%+A, %+A" ceResult (tracerWithRun.GetTrace ())