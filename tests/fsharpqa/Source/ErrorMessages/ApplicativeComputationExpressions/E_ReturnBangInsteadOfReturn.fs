// #ErrorMessages
//<Expects id="FS3245" status="error" span="(12,9)">'return!' is not valid in this position in an applicative computation expression. Did you mean 'return' instead?</Expects>

namespace ApplicativeComputationExpressions

module E_LetBangAndBangNotTerminatedWithReturn =

    let f x y = Eventually.Done (x + y)
    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        return! f x y
    }