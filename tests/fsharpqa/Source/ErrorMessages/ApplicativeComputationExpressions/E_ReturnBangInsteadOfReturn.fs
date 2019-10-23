// #ErrorMessages
//<Expects id="FS0708" status="error" span="(12,9)">This control construct may only be used if the computation expression builder defines a 'ReturnFrom' method</Expects>

namespace ApplicativeComputationExpressions

module E_LetBangAndBangNotTerminatedWithReturn =

    let f x y = Eventually.Done (x + y)
    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        return! f x y
    }