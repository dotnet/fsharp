// #Misc #AppCE

//#Expects: Success: val result : int ApplicativeBuilderLib.Trace = Trace 3
//#Expects: Success: val trace : ApplicativeBuilderLib.TraceOp list = .Return; Apply; Apply.

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