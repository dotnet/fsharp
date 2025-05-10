// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:3248
// Verify we can use Units of Measure in a quotation
// All these functions have hidden unit of measure variables
#light

let _ = <@ sqrt 1.0 @>
let _ = <@ atan2 1.0 2.0 @>
let _ = <@ sign nan @>
let _ = <@ abs -infinityf @>
let _ = <@ op_Addition 1.0 2.0 @>
let _ = <@ op_Multiply 1.0 2.0 @>
let _ = <@ op_Division 1.0 2.0 @>
let _ = <@ op_UnaryNegation 1.0 @>
