module Module

type A = A

type B =
    | B
    member this.M1 = 1
    member this.M2 = 2
    member this.M3 = 3
    module M1 = begin end
    module M2 = begin end
    module M3 = begin end
    type M4() = class end
    type M5() = class end
    type M6() = class end
    
type C = C