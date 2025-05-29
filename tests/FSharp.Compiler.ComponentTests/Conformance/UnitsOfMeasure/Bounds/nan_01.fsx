// #Regression #Conformance #UnitsOfMeasure 
// Regression tests for FSHARP1.0:2343
// Units: ICE when trying to use Units on NaN and Infinity
//<Expects id="FS0717" span="(7,19-7,22)" status="error">Unexpected type arguments</Expects>
#light
[<Measure>] type kg
let mysterymass = nan<kg>
