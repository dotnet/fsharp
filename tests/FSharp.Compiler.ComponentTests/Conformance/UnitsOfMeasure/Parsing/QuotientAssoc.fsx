// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:4124 - Units of measure: division should be left-associative in both types and constants
// Regression test for FSHARP1.0:4188 - Type parameters for units of measure parsed inconsistently.

#light

[<Measure>] type s
[<Measure>] type m
[<Measure>] type A   = m / s / s
[<Measure>] type AA  = m / s^2
[<Measure>] type B   = m^2 / m s / s^2 / s^-1
[<Measure>] type B'  = m / s / s / s / s / s / s / s / s^-5
[<Measure>] type C   = m / s * s / s * s / s / s

let (x : float<m / s / s>) = 1.0<m / s / s>
let y = 2.0<m / s^2>
let z = x+y
let a = 1.0<A>
let aa = 2.0<AA>
let (b : float<m^2/m s/s^2/s^-1>) = 3.0<B>
let b' = 4.0<B'>
let (c : float<m/s*s/s*s/s/s>) = 5.0<C>

let res = a + aa + b + b' + c
