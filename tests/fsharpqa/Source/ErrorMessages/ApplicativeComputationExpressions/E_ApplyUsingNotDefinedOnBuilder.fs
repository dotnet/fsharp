// #ErrorMessages
//<Expects id="FS12345" status="error" span="(9,9)">'ApplyUsing' is not defined on the computation expression builder</Expects>

namespace ApplicativeComputationExpressions

module E_ApplyUsingNotDefinedOnBuilder =

    eventuallyNoApplyUsing {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        return x + y
    }