// #ErrorMessages
//<Expects id="FS1141" status="error" span="(11,13)">Identifiers followed by '!' are reserved for future use</Expects>

namespace ApplicativeComputationExpressions

module E_ApplyUsingNotDefinedOnBuilder =

    let example =
        eventuallyNoApplyUsing {
            let!    x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
            anduse! y = Eventually.Done (FakeDisposable -1)
            return x + y
        }