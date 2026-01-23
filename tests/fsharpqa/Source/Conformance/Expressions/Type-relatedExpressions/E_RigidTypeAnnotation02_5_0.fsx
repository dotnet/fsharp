// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Expressions
// Mismatching types (same as RigidTypeAnnotation02.fsx, but with errors)


[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type N = Kg * m / s ^ 2

let f0 x = (x : string) + 1
let f1 x = "" + (x : byte)
let f2 (x:int) = (x : float) + x
let f3 x = (x : float32<Kg>) + 1M<Kg>
let f4 x = (x : decimal<N*s*s/m>) + ""

exit 1

//<Expects id="FS0001" span="(11,27-11,28)" status="error">The type 'int' does not match the type 'string'</Expects>
//<Expects id="FS0043" span="(11,25-11,26)" status="error">The type 'int' does not match the type 'string'</Expects>
//<Expects id="FS0001" span="(12,18-12,26)" status="error">The type 'byte' does not match the type 'string'</Expects>
//<Expects id="FS0043" span="(12,15-12,16)" status="error">The type 'byte' does not match the type 'string'</Expects>
//<Expects id="FS0001" span="(13,32-13,33)" status="error">The type 'int' does not match the type 'float'</Expects>
//<Expects id="FS0043" span="(13,30-13,31)" status="error">The type 'int' does not match the type 'float'</Expects>
//<Expects status="error" span="(14,32-14,38)" id="FS0001">The type 'decimal<Kg>' does not match the type 'float32<Kg>'</Expects>
//<Expects status="error" span="(14,30-14,31)" id="FS0043">The type 'decimal<Kg>' does not match the type 'float32<Kg>'</Expects>
//<Expects status="error" span="(15,37-15,39)" id="FS0001">The type 'string' does not match the type 'decimal<N s \^ 2/m>'</Expects>
//<Expects status="error" span="(15,35-15,36)" id="FS0043">The type 'string' does not match the type 'decimal<N s \^ 2/m>'</Expects>
