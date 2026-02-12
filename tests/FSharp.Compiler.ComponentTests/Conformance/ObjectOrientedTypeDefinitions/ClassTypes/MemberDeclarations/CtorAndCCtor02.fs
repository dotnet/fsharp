// #Conformance #Regression
// It is just illegal to use '.ctor' or '.cctor' as member names
// The following code should compile just fine since we are not using .cctor incorrectly
//<Expects status="success"></Expects>

namespace N

type T2(``.cctor`` : char) = 
    let ``.cctor`` = 10
    static member ``.cctor ``(``.cctor`` : int) = ``.cctor`` + 1
    member __.``.cctoR``(?``.cctor``) = T2.``.cctor ``(10) + ``.cctor``.Value
    member __.``.cctoP`` with get() = 10
