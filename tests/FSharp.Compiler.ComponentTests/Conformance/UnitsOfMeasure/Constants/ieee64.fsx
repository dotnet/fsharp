// #Regression #Conformance #UnitsOfMeasure #Constants 
// Constants with measures
// ieee64
// Loosely related to regression tests for FSHARP1.0:2918
module M

[<Measure>] type kg

let ieee64    = -1.0e27<kg>
let ieee64_LF = 0x0001LF<kg>
let ieee64_   = -1.<kg>
