// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, [|MonadicBind; MergeSources; MonadicBind; MonadicReturn|]
//OUTPUT

module LetBangAndBangAfterMonadicOperations

open ApplicativeBuilderLib


let () =
    let tracer = TraceMonadic()

    let ceResult : int Trace =
        tracer {
            let fb = Trace "foobar"
            match! fb with
            | "bar" ->
                let! bar = fb
                return String.length bar
            | _ ->
                let! x = Trace 3
                and! y = Trace true
                return if y then x else -1
        }

    printfn "%+A, %+A" ceResult (tracer.GetTrace ())