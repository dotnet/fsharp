// #Conformance #Regression
// It is just illegal to use '.ctor' or '.cctor' as member names
// The following code should NOT compile
//<Expects status="error" span="(12,19-12,28)" id="FS3066">Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$</Expects>
//<Expects status="error" span="(13,15-13,24)" id="FS3066">Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$</Expects>
//<Expects status="error" span="(16,19-16,29)" id="FS3066">Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$</Expects>
//<Expects status="error" span="(17,15-17,25)" id="FS3066">Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$</Expects>

namespace N

type T1() = 
    static member ``.ctor``() = ()
    member __.``.ctor``() = ()

type T2() = 
    static member ``.cctor``() = ()
    member __.``.cctor``() = ()
