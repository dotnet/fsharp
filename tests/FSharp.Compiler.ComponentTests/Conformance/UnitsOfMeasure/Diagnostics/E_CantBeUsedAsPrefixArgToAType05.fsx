// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:3457
// Units in prefix form should be banned
//<Expects id="FS0704" span="(10,12-10,13)" status="error">Expected type, not unit-of-measure</Expects>
//<Expects id="FS0706" span="(10,14-10,21)" status="error">Units-of-measure cannot be used as prefix arguments to a type\. Rewrite as postfix arguments in angle brackets</Expects>
//<Expects id="FS0716" span="(10,12-10,21)" status="error">Unexpected \/ in type</Expects>
[<Measure>] type m

let f0(x : float<m/m>) = 1  // OK
let f1(x : m/m float) = 1   // Err

