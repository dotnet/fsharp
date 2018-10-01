// #Misc #AppCE

//<Expects status=success>val result : Trace int = Trace 3</Expects>
//<Expects status=success>val trace : TraceOp list = [ Apply 2 ; Apply 1 ; Return ; EnterUsing 1 ; EnterUsing 2 ; StartUsingBody 1 ; StartUsingBody 2 ; EndUsingBody 2 ; EndUsingBody 1 ; ExitUsing 2 ; ExitUsing 1 ]</Expects>

open ComputationsExpressions.Test

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
