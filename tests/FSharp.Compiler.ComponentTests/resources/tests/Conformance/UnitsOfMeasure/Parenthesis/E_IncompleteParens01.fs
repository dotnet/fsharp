// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2662
// Make sure we can use ( and ) in Units of Measure


//<Expects status="error" span="(11,1-11,1)" id="FS0010">Incomplete structured construct at or before this point in expression\. Expected '\)' or other token\.$</Expects>
//<Expects status="error" span="(10,26-10,27)" id="FS0583">Unmatched '\('$</Expects>

[<Measure>] type m
let velocity12 = 1.0<m / ( >  // Err
