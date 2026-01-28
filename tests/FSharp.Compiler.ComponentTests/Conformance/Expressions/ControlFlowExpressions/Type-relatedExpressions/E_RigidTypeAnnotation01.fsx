// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Expressions
// Mismatching types (same as RigidTypeAnnotation01.fsx, but with errors)
//<Expects id="FS0001" span="(14,10-14,13)" status="error">This expression was expected to have type.    'sbyte'    .but here has type.    'byte'</Expects>
//<Expects id="FS0001" span="(15,10-15,16)" status="error">This expression was expected to have type.    'float32'    .but here has type.    'float<'u>'</Expects>
//<Expects id="FS0001" span="(16,10-16,16)" status="error">This expression was expected to have type.    'float32<'u>'    .but here has type.    'float'</Expects>
//<Expects id="FS0001" span="(17,10-17,17)" status="error">This expression was expected to have type.    'decimal<N s \^ 2/m>'    .but here has type.    'float<Kg>'</Expects>

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type N = Kg * m / s ^ 2

let e1 = 1uy : sbyte
let e2 = 1.0<_> : float32
let e3 = 1.0<1> : float32<_>
let e4 = 1.0<Kg> : decimal<N*s*s/m>

