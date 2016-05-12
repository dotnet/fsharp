// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Expressions
// Mismatching types (same as RigidTypeAnnotation03.fsx, but with errors)
[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type N = Kg * m / s ^ 2

type T = class
            static member M(a:byte) = 1
            static member M(a:float) = 2
            static member M(a:float32<Kg>) = 3
            static member M(a:decimal<Kg>) = 4
            static member M(a:string) = 0
         end

let _ = T.M(1uy : sbyte)
let _ = T.M(1.0<_> : float32)
let _ = T.M(1.0M<s> : float32<_>)
let _ = T.M(1.0M<Kg> : decimal<N*s*s>)
let _ = T.M( @"\" : char )

exit 1
// Way more errors are reported, but this is a good enough list.
//<Expects id="FS0001" span="(17,13-17,16)" status="error">This expression was expected to have type.    'sbyte'    .but here has type.    'byte'</Expects>
//<Expects id="FS0041" span="(17,9-17,25)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>
//<Expects id="FS0001" span="(18,13-18,19)" status="error">This expression was expected to have type.    'float32'    .but here has type.    'float<'u>'</Expects>
//<Expects id="FS0041" span="(18,9-18,30)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>
//<Expects id="FS0001" span="(19,13-19,20)" status="error">This expression was expected to have type.    'float32<'u>'    .but here has type.    'decimal<s>'</Expects>
//<Expects id="FS0001" span="(20,13-20,21)" status="error">Type mismatch\. Expecting a.    'decimal<N s \^ 2>'    .but given a.    'decimal<Kg>'</Expects>
//<Expects id="FS0041" span="(20,9-20,39)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>
//<Expects id="FS0001" span="(21,14-21,18)" status="error">This expression was expected to have type.    'char'    .but here has type.    'string'</Expects>
//<Expects id="FS0041" span="(21,9-21,27)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>
