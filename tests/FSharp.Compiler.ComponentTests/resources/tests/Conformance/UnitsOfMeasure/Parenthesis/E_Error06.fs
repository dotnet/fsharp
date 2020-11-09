// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662,2746
// Make sure we can use ( and ) in Units of Measure
//<Expects id="FS0620" span="(11,20-11,23)" status="error">Unexpected integer literal in unit-of-measure expression</Expects>
#light

[<Measure>] type Kg
[<Measure>] type m
[<Measure>] type s

let v = 1.0< (s)/( 255 ) >  // Error
