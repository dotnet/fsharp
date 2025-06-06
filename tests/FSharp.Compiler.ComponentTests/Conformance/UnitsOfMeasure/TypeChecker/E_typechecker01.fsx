// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
// Regression test for FSHARP1.0:2279
// The following code should give a nicer error message, not an ICE
//<Expects id="FS0687" span="(8,1-8,2)" status="error">This value, type or method expects 2 type parameter\(s\) but was given 1</Expects>

let f x y = ()

f<int>
