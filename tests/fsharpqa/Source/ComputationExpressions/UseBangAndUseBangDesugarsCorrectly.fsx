// #Misc #AppCE

//<Expects status=success>val result : int ApplicativeBuilderLib.Trace = Trace 3</Expects>
//<Expects status=success>val trace : ApplicativeBuilderLib.TraceOp list =</Expects>
//<Expects status=success>  .Return; Apply; Apply; EnterUsing 1; StartUsingBody 1; EnterUsing 2;</Expects>
//<Expects status=success>   StartUsingBody 2; EndUsingBody 2; ExitUsing 2; EndUsingBody 1; ExitUsing 1.</Expects>

open ApplicativeBuilderLib

let result, trace =
    let tracer = TraceBuilder()

    let ceResult : int Trace =
        tracer {
            use!    x = Trace 1
            anduse! y = Trace 2
            return 1 + 2
        }

    ceResult, tracer.GetTrace ()
;;
#q;;
