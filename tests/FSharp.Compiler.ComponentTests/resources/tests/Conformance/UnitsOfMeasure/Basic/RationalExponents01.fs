// #Conformance #UnitsOfMeasure 
// Rational exponents feature

[<Measure>]
type kg

[<Measure>]
type s

[<Measure>]
type m

// Simple fractions
let test01() = 1.0<kg^(1/2)>
let test02() = 2.0<kg^(1/4)>
let test03() = sqrt (test01()) + test02()

// Negative fractions
let test04() = 4.0<s^-(4/6)>
let test05() = 5.0<s^(-1/3)>
let test06() = 1.0 / (test04() * test05()) + 3.0<s>

// More complex expressions
let test07() = 2.0</(kg s^2)^(3/4)>
let test08() = 4.0<(s^6 kg^3)^(1/4)>
let test09() = test07() * test08() + 3.0

// Generics
let test10(x:float<'u>) (y:float<'u^(1/2)>) = sqrt x + y
let test11(x:float<'u^-(1/4)>) (y:float<'u^(3/4)>) : float</'u> = (x*x*x + 1.0/y) * x
let test12(x:float<'u^(1/2)>) (y:float<'v^2>) :float<'u 'v> = x*x*sqrt y
let test13() = test12 4.0<s^(1/4)> 2.0<kg> + 3.0<(kg s)^(1/2)>
printfn "Finished"
