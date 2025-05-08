// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// Regression test for FSHARP1.0:4945
// Unexpected "Value Restriction" error message with overloading & units-of-measure

let differentiate (h:float<_>) f x = (f(x+h) - f(x-h)) / (2.0*h)

let f (x : float<_>) = x * x

if (differentiate 2.0<1> f 2.0) <> 4.0 then exit 1

exit 0
