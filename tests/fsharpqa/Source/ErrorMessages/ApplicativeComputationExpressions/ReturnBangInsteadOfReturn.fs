// #Misc #AppCE

//#Expects: Success

namespace ApplicativeComputationExpressions

module LetBangAndBangNotTerminatedWithReturn =

    let f x y = Eventually.Done (x + y)
    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        return! f x y
    }