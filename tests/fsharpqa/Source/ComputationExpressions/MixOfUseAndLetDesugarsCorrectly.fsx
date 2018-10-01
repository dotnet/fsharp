// #Misc #AppCE

//<Expects status=success>val result : Trace int = Trace 6</Expects>
//<Expects status=success>val trace : TraceOp list = [ Apply 2 ; Apply 1 ; Return ; EnterUsing 1 ; StartUsingBody 1 ; EndUsingBody 1 ; ExitUsing 1 ]</Expects>

open ComputationsExpressions.Test

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