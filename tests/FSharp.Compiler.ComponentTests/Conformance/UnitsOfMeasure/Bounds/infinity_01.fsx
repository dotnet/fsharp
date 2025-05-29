// #Regression #Conformance #UnitsOfMeasure 
// Regression tests for FSHARP1.0:2343
// Units: ICE when trying to use Units on NaN and Infinity
//<Expects id="FS0717" span="(12,18-12,27)" status="error">Unexpected type arguments</Expects>
//<Expects id="FS0717" span="(13,18-13,26)" status="error">Unexpected type arguments</Expects>
//<Expects id="FS0717" span="(14,19-14,28)" status="error">Unexpected type arguments</Expects>
//<Expects id="FS0717" span="(15,19-15,27)" status="error">Unexpected type arguments</Expects>
#light

[<Measure>] type kg

let veryheavy1 = infinityf<kg>
let veryheavy2 = infinity<kg>
let veryheavy3 = -infinityf<kg>
let veryheavy4 = -infinity<kg>
