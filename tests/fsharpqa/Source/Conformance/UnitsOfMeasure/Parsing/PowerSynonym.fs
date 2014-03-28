// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:4125
// Units of measure: implicit product + powers in measure type synonyms

#light

[<Measure>] type s
[<Measure>] type m
[<Measure>] type a = m s ^-2
[<Measure>] type aa = m / s^2
[<Measure>] type aaa = m * s^-2
[<Measure>] type b = m * s^-1 * s^-1
[<Measure>] type bb = 1^3 / s * 1^2 / s * m

let x = 1.0<a>
let y = 2.0<aa>
let z = 3.0<aaa>
let x' = 1.0<b>
let y' = 2.0<bb>
let a = x + y + z + x' + y'
