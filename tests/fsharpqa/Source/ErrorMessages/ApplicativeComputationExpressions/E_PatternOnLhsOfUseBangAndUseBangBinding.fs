// #ErrorMessages
//<Expects id="FS3243" status="error" span="(16,19)">Pattern matching is not allowed on the left-hand side of the equals. </Expects>
//<Expects>'use! ... anduse! ...' bindings must be of the form </Expects>
//<Expects>'use! <var> = <expr>' or 'anduse! <var> = <expr>'.</Expects>

namespace ApplicativeComputationExpressions

    eventually {
        use! (x,_) = Eventually.Done (FakeDisposable 3, 19)
        anduse! y  = Eventually.Done (FakeDisposable 11)
        return x + y
    }
