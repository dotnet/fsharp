// #ErrorMessages
//<Expects id="FS0708" status="error" span="(11,13)">This control construct may only be used if the computation expression builder defines a 'ApplyUsing' method</Expects>

namespace ApplicativeComputationExpressions

module E_ApplyUsingNotDefinedOnBuilder =

    let example =
        eventuallyNoApplyUsing {
            let!    x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
            anduse! y = Eventually.Done (FakeDisposable -1)
            return x + y
        }