// #Misc #AppCE

//<Expects status=success>^$</Expects>
//<Expects status=success>val result : int ApplicativeBuilderLib.Trace = Trace 3</Expects>
//<Expects status=success>val trace : ApplicativeBuilderLib.TraceOp list = .Return; Apply; Apply.</Expects>

open ApplicativeBuilderLib

let result, trace =
    let tracer = TraceBuilder()

    let ceResult : int Trace =
        tracer {
            let! x = Trace 1
            and! y = Trace 2
            return 1 + 2
        }

    ceResult, tracer.GetTrace ()
;;
#q;;