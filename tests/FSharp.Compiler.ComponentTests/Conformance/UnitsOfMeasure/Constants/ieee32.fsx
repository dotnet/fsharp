// #Regression #Conformance #UnitsOfMeasure #Constants 
// Constants with measures
// ieee32
// Loosely related to regression tests for FSHARP1.0:2918

module M
[<Measure>] type kg

let ieee32_f = -1.0e27f<kg>
let ieee32_F = -1.0e27F<kg>
let ieee32_lf = 0x0001lf<kg>
let ieee32_   = -1.f<kg>
