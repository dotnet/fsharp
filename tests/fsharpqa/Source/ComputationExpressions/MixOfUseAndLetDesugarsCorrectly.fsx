// #Misc #AppCE

//<Expects status=success>val result : int ApplicativeBuilderLib.Trace = Trace 6</Expects>
//<Expects status=success>val trace : ApplicativeBuilderLib.TraceOp list =</Expects>
//<Expects status=success>  .Return; Apply; Apply; Apply; EnterUsing 1; StartUsingBody 1; EnterUsing 3;</Expects>
//<Expects status=success>   StartUsingBody 3; EndUsingBody 3; ExitUsing 3; EndUsingBody 1; ExitUsing 1.</Expects>

open ApplicativeBuilderLib

let result, trace =
    let tracer = TraceBuilder()

    let ceResult : int Trace =
        tracer {
            use!    x = Trace 1
            and!    y = Trace 2 // Explicitly _not_ treated as a resource to be managed
            anduse! z = Trace 3
            return 1 + 2 + 3
        }
    
    ceResult, tracer.GetTrace ()
;;
#q;;