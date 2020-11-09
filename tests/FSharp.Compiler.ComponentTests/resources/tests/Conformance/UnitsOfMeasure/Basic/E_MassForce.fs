// #Regression #Conformance #UnitsOfMeasure 
//<Expects id="FS0001" span="(15,27-15,32)" status="error">The unit of measure 'N' does not match the unit of measure 'kg'</Expects>
//<Expects id="FS0043" span="(15,25-15,26)" status="error">The unit of measure 'N' does not match the unit of measure 'kg'</Expects>
#light

[<Measure>] type kg
[<Measure>] type m
[<Measure>] type s
[<Measure>] type N = kg * m / (s*s)

let g = 9.81<m/s^2>              // acceleration due to gravity
let me = 65.0<kg>
let apple = 1.0<N>

let meHoldingApple = me + apple                     // error!
