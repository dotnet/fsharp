// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for Dev11:97473 - SPEC: sequence range expressions don't fully support units of measure
// Basically, the test is here to enforce the *by design* behavior.

//<Expects status="success"></Expects>

[<Measure>]
type foo

let m, n = 1<foo>, 10<foo>

[m .. 1<foo> .. n]         // OK
printfn "Finished"
