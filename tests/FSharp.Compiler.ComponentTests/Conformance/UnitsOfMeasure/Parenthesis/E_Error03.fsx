// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662
// Make sure we can use ( and ) in Units of Measure
//<Expects id="FS0010" span="(11,31-11,32)" status="error">Unexpected symbol '\)' in binding</Expects>
#light

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s

let velocity7 = 1.0<m / ( s / )>  // Error
