// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662
// Make sure we can use ( and ) in Units of Measure
//<Expects status="error" span="(11,26-11,27)" id="FS0010">Unexpected symbol '\)' in binding$</Expects>
//<Expects status="error" span="(11,24-11,25)" id="FS0010">Unexpected token '/' or incomplete expression</Expects>

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s

let velocity12 = 1.0<m / ) >  // Err
