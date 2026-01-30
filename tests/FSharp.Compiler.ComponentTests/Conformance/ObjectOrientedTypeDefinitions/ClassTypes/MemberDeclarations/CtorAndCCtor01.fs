// #Conformance #Regression
// It is just illegal to use '.ctor' or '.cctor' as member names
// The following code should compile just fine since we are not using .ctor incorrectly
//<Expects status="success"></Expects>

namespace N

type T1(``.ctor`` : char) = 
    let ``.ctor`` = 10
    static member ``.ctor ``(``.ctor`` : int) = ``.ctor`` + 1
    member __.``.ctoR``(?``.ctor``) = T1.``.ctor ``(10) + ``.ctor``.Value
    member __.``.ctoP`` with get() = 10
