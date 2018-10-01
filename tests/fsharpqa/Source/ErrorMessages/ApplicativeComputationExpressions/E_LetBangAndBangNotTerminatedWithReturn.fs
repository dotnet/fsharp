// #ErrorMessages
//<Expects id="FS3243" status="error" span="(16,19)">Expecting 'return' but saw something else. </Expects>
//<Expects>Applicative computation expressions must be of the form 'let! <pat1> = <expr2> </Expects>
//<Expects>and! <pat2> = <expr2> and! ... and! <patN> = <exprN> return <exprBody>'.</Expects>

namespace ApplicativeComputationExpressions

    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        let _ = 42 // Invalid: We expect a return to terminate the the let! ... and! ... sequence
        return x + y
    }