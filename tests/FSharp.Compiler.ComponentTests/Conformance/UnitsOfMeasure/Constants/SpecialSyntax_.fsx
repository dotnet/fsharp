// #Regression #Conformance #UnitsOfMeasure #Constants 
// Regression test for FSHARP1.0:2918
// Special syntax _
// Basically <_> means that this is something the compiler should be able to infer
module M
[<Measure>] type kg

let x : float<kg> = 2.0<_>
let y : float<kg> = 0.0<_> 
let w = 1.0<  1 >
let w2 = 1.0<kg/kg>
let w3 = -1.2M<kg>
