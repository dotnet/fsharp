// #ErrorMessages
//<Expects id="FS12345" status="error" span="(9,9)">'Apply' is not defined on the computation expression builder</Expects>

namespace ApplicativeComputationExpressions

module E_ApplyNotDefinedOnBuilder =

    eventuallyNoApply {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        return x + y
    }