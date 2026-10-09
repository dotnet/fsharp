type T =
    | T of int
    static member op_Implicit (T t) = U t

and U =
    | U of int

type R1 = { A : T }
type R2 = { A : U }

#nowarn 3391

let r1 : R1 = { A = T 3 }
let r2 : R2 = { ...r1 }
