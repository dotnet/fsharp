// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662
// Make sure we can use ( and ) in Units of Measure
//<Expects id="FS0010" span="(11,28-11,29)" status="error">Unexpected symbol '\)' in binding</Expects>
#light

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s

let velocity12 = 1.0<m / ( )>  // Err
