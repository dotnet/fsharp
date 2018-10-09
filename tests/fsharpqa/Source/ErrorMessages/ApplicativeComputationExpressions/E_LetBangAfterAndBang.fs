// #ErrorMessages
//<Expects id="FS3243" status="error" span="(11,9)">Expecting 'and!', 'anduse!' or 'return' but saw something else. Applicative computation expressions must be of the form 'let! <pat1> = <expr2> and! <pat2> = <expr2> and! ... and! <patN> = <exprN> return <exprBody>'.</Expects>

namespace ApplicativeComputationExpressions

module E_LetBangAfterAndBang =

    eventually {
        let! x = Eventually.NotYetDone (fun () -> Eventually.Done 4)
        and! y = Eventually.Done 6
        let! z = Eventually.NotYetDone (fun () -> Eventually.Done 11)
        return x + y + z
    }