// #Regression #Conformance #UnitsOfMeasure #Constants 
// Constants with measures
// ieee32
// Loosely related to regression tests for FSHARP1.0:2918
module M

[<Measure>] type kg

let decimal_M   = -21.0M<kg>
let decimal_m   = -11.0m<kg>
