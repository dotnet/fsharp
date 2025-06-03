// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:3457
// Units in prefix form should be banned
//<Expects id="FS0706" span="(9,12-9,27)" status="error">Units-of-measure cannot be used as prefix arguments to a type\. Rewrite as postfix arguments in angle brackets</Expects>
#light
[<Measure>] type m

let f0(x : float<m ^ -0x01>) = 1 // OK
let f1(x : m ^ -0x01 float) = 1  // Err
