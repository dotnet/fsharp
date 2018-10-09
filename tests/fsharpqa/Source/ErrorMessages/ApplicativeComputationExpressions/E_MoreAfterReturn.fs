// #ErrorMessages
//<Expects id="FS3245" status="error" span="(12,9)">Saw unexpected expression after 'return'. Applicative computation expressions must be terminated with a 'return'.</Expects>

namespace ApplicativeComputationExpressions

module E_MoreAfterReturn =

    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        return x + y
        let w = 42
        let! a = Eventually.Done -1
        and! b = Eventually.Done 30
        return a * b
    }