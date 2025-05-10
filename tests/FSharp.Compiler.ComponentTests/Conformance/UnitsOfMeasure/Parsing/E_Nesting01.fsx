// #Regression #Conformance #UnitsOfMeasure 
#light

// Regression test for FSHARP1.0:3456 - Unsupported syntax for units is not reported as an error
//<Expects id="FS0714" span="(12,19-12,20)" status="error">Anonymous unit-of-measure cannot be nested inside another unit-of-measure expression</Expects>
//<Expects id="FS0707" span="(17,17-17,21)" status="error">Unit-of-measure cannot be used in type constructor application</Expects>

open System
[<Measure>] type m
[<Measure>] type _m

let f(x : float<m*_>) = 1
let b(x : float<m _m>) = x * x
let x = f(1.)
let x' = b(2.01<_m m>) / 2.0<_m^2>

let g(x : float<m<m>>) = 1
let y = g(1.<m m>)
