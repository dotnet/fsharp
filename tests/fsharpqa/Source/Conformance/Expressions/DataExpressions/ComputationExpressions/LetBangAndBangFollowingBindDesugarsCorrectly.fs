// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, .Bind; Return; Apply; Apply.
//OUTPUT

open ApplicativeBuilderLib

module LetBangAndBangAfterLetBang =

    let () =
        let tracer = TraceBuilder()

        let ceResult : int Trace =
            tracer {
                let foo = Trace "foo"
                match! foo with
                | "bar" ->
                    return 0
                | _ ->
                    let! x = Trace 3
                    and! y = foo
                    return if y = "foo" then x else -1
            }

        printfn "%+A, %+A" ceResult (tracer.GetTrace ())