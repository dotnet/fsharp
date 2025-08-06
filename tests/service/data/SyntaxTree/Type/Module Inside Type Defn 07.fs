module Module

type A = A

module M1 = begin end
module M2 = begin end
module M3 = begin end

type B =
    | B
    module M4 = begin end
    module M5 = begin end
    module M6 = begin end

type C = C