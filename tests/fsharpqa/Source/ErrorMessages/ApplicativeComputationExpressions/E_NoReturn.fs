// #ErrorMessages
//<Expects id="FS3246" status="error" span="(9,9)">No body given after the applicative bindings. Expected a 'return' to terminate this applicative computation expression.</Expects>

namespace ApplicativeComputationExpressions

module E_NoReturn =

    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
    }