// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662
// Make sure we can use ( and ) in Units of Measure
//<Expects status="error" span="(10,26-10,27)" id="FS0010">Unexpected symbol '\)' in expression$</Expects>

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s

let velocity12 = 1.0<m / ) >  // Err
