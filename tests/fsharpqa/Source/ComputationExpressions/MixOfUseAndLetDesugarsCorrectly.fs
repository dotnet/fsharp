// #Misc #AppCE

//#Expects: Success
// << OUTPUT
//Trace 6, \\[Return; Apply; Apply; Apply; EnterUsing 1; StartUsingBody 1; EnterUsing 3;
// StartUsingBody 3; EndUsingBody 3; ExitUsing 3; EndUsingBody 1; ExitUsing 1\\]
//OUTPUT

open ApplicativeBuilderLib

module UseAndLet =

    let () =
        let tracer = TraceBuilder()

        let ceResult : int Trace =
            tracer {
                use!    x = Trace 1
                and!    y = Trace 2 // Explicitly _not_ treated as a resource to be managed
                anduse! z = Trace 3
                return 1 + 2 + 3
            }
        
        printf "%+A, %+A" ceResult (tracer.GetTrace ())