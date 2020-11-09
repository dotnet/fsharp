// #Regression #Conformance #UnitsOfMeasure #Diagnostics 
// Regression test for FSHARP1.0:2914. FSHARP1.0:3795
//<Expects status="error" id="FS0634" span="(7,15-7,17)">Non-zero constants cannot have generic units\. For generic zero, write 0\.0<_></Expects>
//<Expects status="error" id="FS0634" span="(8,16-8,18)">Non-zero constants cannot have generic units\. For generic zero, write 0\.0<_></Expects>
//<Expects status="error" id="FS0634" span="(9,16-9,18)">Non-zero constants cannot have generic units\. For generic zero, write 0\.0<_></Expects>
#light
let p1 = -2.0<'a>
let p2 = -2.0m<'b >
let p3 = -2.0f<'c  >

let q1 = -0.0<_>
let q2 = -0.0m<_ >
let q3 = -0.0f<_  >

