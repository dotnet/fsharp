// #ErrorMessages
//<Expects id="FS0708" status="error" span="(11,13)">This control construct may only be used if the computation expression builder defines a 'Apply' method</Expects>

namespace ApplicativeComputationExpressions

module E_ApplyNotDefinedOnBuilder =

    let example =
        eventuallyNoApply {
            let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
            and! y = Eventually.Done 6
            return x + y
        }