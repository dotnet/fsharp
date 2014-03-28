// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662
// Make sure we can use ( and ) in Units of Measure
//<Expects id="FS0010" span="(11,18-11,19)" status="error">Unexpected symbol '\(' in binding\. Expected integer literal, '-' or other token</Expects>
#light

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s

let v = 1.0< (s)^(2) >  //error FS0010: Unexpected symbol '(' in binding. Expected integer literal, '-' or other token
