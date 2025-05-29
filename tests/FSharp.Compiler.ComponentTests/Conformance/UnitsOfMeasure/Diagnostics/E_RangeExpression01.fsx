// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for Dev11:97473 - SPEC: sequence range expressions don't fully support units of measure
// Basically, the test is here to enforce the *by design* behavior.
//<Expects status="error" span="(11,2-11,3)" id="FS0001">The type 'int<foo>' does not match the type 'int'$</Expects>

[<Measure>]
type foo

let m, n = 1<foo>, 10<foo>

[m .. n] |> ignore              // Error

