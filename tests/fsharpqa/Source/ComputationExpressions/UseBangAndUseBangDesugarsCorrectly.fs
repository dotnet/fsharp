// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 3, \\[Return; Apply; Apply; EnterUsing 1; StartUsingBody 1; EnterUsing 2;
// StartUsingBody 2; EndUsingBody 2; ExitUsing 2; EndUsingBody 1; ExitUsing 1\\]
//OUTPUT

open ApplicativeBuilderLib

module UseBang =

    let () =
        let tracer = TraceBuilder()

        let ceResult : int Trace =
            tracer {
                use!    x = Trace 1
                anduse! y = Trace 2
                return 1 + 2
            }

        printf "%+A, %+A" ceResult (tracer.GetTrace ())