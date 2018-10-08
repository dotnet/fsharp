// #ErrorMessages
//<Expects id="FS3243" status="error" span="(11,9)">Expecting 'return', 'and!' or 'anduse!' but saw something else. Applicative computation expressions must be of the form 'let! <pat1> = <expr2> and! <pat2> = <expr2> and! ... and! <patN> = <exprN> return <exprBody>'.</Expects>

namespace ApplicativeComputationExpressions

module E_LetBangAndBangNotTerminatedWithReturn =

    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        let _ = 42 // Invalid: We expect a return to terminate the the let! ... and! ... sequence
        return x + y
    }