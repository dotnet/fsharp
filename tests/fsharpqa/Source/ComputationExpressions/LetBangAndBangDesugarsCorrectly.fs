// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, \\[Return; Apply; Apply\\]
//OUTPUT

open ApplicativeBuilderLib

module LetBang =

    let () =
        let tracer = TraceBuilder()

        let ceResult : int Trace =
            tracer {
                let! x = Trace 1
                and! y = Trace 2
                return 1 + 2
            }

        printf "%+A, %+A" ceResult (tracer.GetTrace ())