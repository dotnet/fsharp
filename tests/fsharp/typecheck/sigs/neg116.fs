module Neg116

type Complex = unit

type Polynomial () =
    static member (*) (s: decimal, p: Polynomial) : Polynomial = failwith ""
    static member (*) (s: Complex, p: Polynomial) : Polynomial = failwith ""

module Foo =
    let test t (p: Polynomial) = (1.0 - t) * p
