// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, .Bind; Return; Apply; Apply.
//OUTPUT

open ApplicativeBuilderLib

module LetBangAndBangAfterMonadicOperations =

    let () =
        let tracer = MonadicTraceBuilder()

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