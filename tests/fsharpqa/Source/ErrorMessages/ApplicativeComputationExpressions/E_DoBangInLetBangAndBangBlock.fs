// #ErrorMessages
//<Expects id="FS0010" status="error" span="(12,9)">Unexpected keyword 'and!' in expression</Expects>

namespace ApplicativeComputationExpressions

module E_DoBangInLetBangAndBangBlock =

    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        // Up to this point this is a valid monadic computation expression, with no reason to suspect it might be an applicative.
        do! Eventually.Done ()
        and! z = Eventually.Done 4
        return x + y + z
    }