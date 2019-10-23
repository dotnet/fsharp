// #Misc #AppCE

//#Expects: Success

namespace ApplicativeComputationExpressions

module LetBangAfterAndBang =

    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        let! z = Eventually.NotYetDone (fun () -> Eventually.Done 11)
        return x + y + z
    }