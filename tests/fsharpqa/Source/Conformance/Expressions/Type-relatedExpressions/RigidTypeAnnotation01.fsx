// #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Expressions
//
// Rigid type annotation
// expr : ty
//
// rigid type annotation used in the let-binding
//
//<Expects status="success"></Expects>
#light

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type N = Kg * m / s ^ 2

let e1 = 1uy : byte
let e2 = 1.0<_> : float
let e3 = 1.0f<Kg> : float32<_>
let e4 = 1.0M<Kg> : decimal<N*s*s/m>

exit 0
