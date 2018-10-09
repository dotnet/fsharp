// #ErrorMessages
//<Expects id="FS0010" status="error" span="(13,9)">Unexpected keyword 'and!' in expression. Expected '}' or other token.</Expects>

namespace ApplicativeComputationExpressions

module E_DoBangInLetBangAndBangBlock =

    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        // Up to this point this is a valid monadic computation expression, with no reason to suspect it might be an applicative.
        do! Eventually.Done () // Still syntactically valid - immediate 'return' is forced by the type checker, not the parser
        and! z = Eventually.Done 4
        return x + y + z
    }