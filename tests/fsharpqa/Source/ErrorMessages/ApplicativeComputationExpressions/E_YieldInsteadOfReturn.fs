// #ErrorMessages
//<Expects id="FS0708" status="error" span="(11,9)">This control construct may only be used if the computation expression builder defines a 'Yield' method</Expects>

namespace ApplicativeComputationExpressions

module E_LetBangAndBangNotTerminatedWithReturn =

    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        yield x + y
    }