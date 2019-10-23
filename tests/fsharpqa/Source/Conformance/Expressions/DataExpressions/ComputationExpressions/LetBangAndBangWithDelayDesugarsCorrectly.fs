// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, [|Delay; MergeSources; ApplicativeBind; ApplicativeReturn|]
//OUTPUT

module Delay 

open ApplicativeBuilderLib

let () =
    let tracerWithDelay = TraceApplicativeWithDelay()

    let ceResult : int Trace =
        tracerWithDelay {
            let! x = Trace 3
            and! y = Trace true
            return if y then x else -1
        }

    printfn "%+A, %+A" ceResult (tracerWithDelay.GetTrace ())