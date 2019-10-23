// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, [|MergeSources; ApplicativeBind; ApplicativeReturn|]
//OUTPUT

module LetBangAndBang
open ApplicativeBuilderLib


let () =
    let tracer = TraceApplicative()

    let ceResult : Trace<int> =
        tracer {
            let! x = Trace 3
            and! y = Trace true
            return if y then x else -1
        }

    printfn "%+A, %+A" ceResult (tracer.GetTrace ())