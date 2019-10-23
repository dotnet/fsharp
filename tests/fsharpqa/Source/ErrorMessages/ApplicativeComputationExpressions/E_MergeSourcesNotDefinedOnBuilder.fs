// #ErrorMessages
//<Expects id="FS0708" status="error" span="(11,22)">This control construct may only be used if the computation expression builder defines a 'MergeSources' method</Expects>


namespace ApplicativeComputationExpressions

module MergeSourcesNotDefinedOnBuilder =

    let example =
        eventuallyNoApply {
            let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
            and! y = Eventually.Done 6
            return x + y
        }