// #ErrorMessages
//<Expects id="FS3244" status="error" span="(9,14)">Pattern matching is not allowed on the left-hand side of the equals. 'use! ... anduse! ...' bindings must be of the form 'use! <var> = <expr>' or 'anduse! <var> = <expr>'.</Expects>

namespace ApplicativeComputationExpressions

module E_PatternOnLhsOfUseBangAndUseBangBinding =

    eventually {
        use! (x,_) = Eventually.Done (FakeDisposable 3, 19)
        anduse! y  = Eventually.Done (FakeDisposable 11)
        return x + y
    }
