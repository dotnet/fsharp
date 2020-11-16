// #Regression #Conformance #UnitsOfMeasure #Constants 
// Constants with measures
// unsupported types 
// Loosely related to regression tests for FSHARP1.0:2918
//<Expects id="FS0636" status="error">Units-of-measure are only supported on float, float32, decimal, and integer types.</Expects>
//<Expects id="FS0636" status="error">Units-of-measure are only supported on float, float32, decimal, and integer types.</Expects>
module M
[<Measure>] type kg

// OK
let _ = 4.0f<kg>
let _ = 4.0<kg>
let ieee32_   = 1.f<kg>
let _ = 4s<kg>
let _ = 4y<kg>
let _ = 4<kg>
let _ = 0x4<kg>
let _ = 0b0100<kg>
let _ = 4L<kg>

// Err
let _ = 4I<kg>
let _ = 4N<kg>
