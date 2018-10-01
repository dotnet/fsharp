// #Misc #AppCE

//<Expects status=success>val result : Trace int = Trace 3</Expects>
//<Expects status=success>val trace : TraceOp list = [ Apply 2 ; Apply 1 ; Return ]</Expects>

namespace ComputationsExpressions.Test

module LetBangAndBangDesugarsCorrectly =

    let result, trace =
        let tracer = TraceBuilder()

        let ceResult : int Trace =
            tracer {
                let! x = Trace 1
                and! y = Trace 2
                return 1 + 2
            }

        ceResult, tracer.GetTrace ()