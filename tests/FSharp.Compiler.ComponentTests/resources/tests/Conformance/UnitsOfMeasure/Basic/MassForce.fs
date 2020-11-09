// #Conformance #UnitsOfMeasure 
#light

[<Measure>] type kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type N = kg * m / (s*s)

let g = 9.81<m/s^2>              // acceleration due to gravity
let me = 65.0<kg>
let apple = 1.0<N>

let meHoldingApple = g*me + apple    // ok!
