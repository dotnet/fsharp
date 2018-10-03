// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, .Return; Apply; Apply.
//OUTPUT

open ApplicativeBuilderLib

module LetBang =

    let () =
        let tracer = TraceBuilder()

        let ceResult : int Trace =
            tracer {
                let! x = Trace 3
                and! y = Trace true
                return if y then x else -1
            }

        printfn "%+A, %+A" ceResult (tracer.GetTrace ())