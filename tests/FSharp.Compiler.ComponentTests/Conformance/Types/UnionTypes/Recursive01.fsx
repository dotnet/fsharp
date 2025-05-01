// #Conformance #TypesAndModules #Unions 
// DU - with recursive definition
// Note: don't try to make much sense of this code.
// It's a rather (intentionally) convoluted code.
//<Expects status="success"></Expects>
#light

type E = | Sum of E * E
         | Mul of E * E
         | E of T
         member e.Eval() = match e with
                           | Sum(e1,e2) -> e1.Eval() + e2.Eval()
                           | Mul(e1,e2) -> e1.Eval() * e2.Eval()
                           | E(t) -> t.Eval()
and
    T = | T of E
        | N of int
        member t.Eval() = match t with
                          | T(e) -> e.Eval()
                          | N(i) -> i
                          
let c = E(T(Sum(E(N(1)),E(N(2)))))

if c.Eval() = 3 then 0 else failwith "Failed: 1"

