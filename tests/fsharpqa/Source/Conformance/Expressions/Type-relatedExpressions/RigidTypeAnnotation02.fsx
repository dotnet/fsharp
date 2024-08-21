// #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Expressions
//
// Rigid type annotation
// expr : ty
//
// rigid type annotation used in the let-binding (function definition + function invocation)
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

let f0 x = (x : string) + x
let f1 x = (x : byte) + x
let f2 x = (x : float) + x
let f3 x = (x : float32<Kg>) + x
let f4 x = (x : decimal<N*s*s/m>) + x

let f5 x = (x : string) + (x : string)   // ok
let f6a x = (x : string) + (x : _)       // ok
let f6b x = (x : _) + (x : string)       // ok

let _ = f0 @"\"
let _ = f1 e1
let _ = f2 e2
let _ = f3 e3
let _ = f4 e4

exit 0
