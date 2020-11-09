// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662
// Make sure we can use ( and ) in Units of Measure
//<Expects id="FS0010" span="(11,34-11,35)" status="error">Unexpected symbol '_' in binding</Expects>
#light

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s

let velocity11 = 1.0<m / ( (s) / _)>  // Err
