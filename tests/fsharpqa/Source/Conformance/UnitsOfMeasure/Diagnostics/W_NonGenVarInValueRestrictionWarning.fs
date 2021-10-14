// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:4969
// Non-generalized unit-of-measure variables should display with "_" in value restriction warning
//<Expects status="error" span="(6,5-6,6)" id="FS0030">.+val x: float<'_u> list ref</Expects>
//<Expects status="error" span="(7,5-7,6)" id="FS0030">.+val y: '_a list ref</Expects>
let x = ref ([] : float<_> list)
let y = ref ([] : _ list)
