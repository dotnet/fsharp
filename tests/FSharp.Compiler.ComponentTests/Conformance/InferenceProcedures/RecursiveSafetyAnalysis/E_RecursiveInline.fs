// #Regression #Conformance #TypeInference #Recursion 
// Regression test for FSharp1.0:3475 - ICE on rec inline function
//<Expects id="FS1114" span="(8,15-8,19)" status="error">The value 'E_RecursiveInline.test' was marked inline but was not bound in the optimization environment</Expects>
//<Expects id="FS1113" span="(7,16-7,20)" status="error">The value 'test' was marked inline but its implementation makes use of an internal or private function which is not sufficiently accessible$</Expects>
//<Expects id="FS1116" span="(8,15-8,19)" status="warning">A value marked as 'inline' has an unexpected value</Expects>
//<Expects id="FS1118" span="(8,15-8,19)" status="error">Failed to inline the value 'test' marked 'inline', perhaps because a recursive value was marked 'inline'</Expects>
let rec inline test x =
    if x then test false
    else 0
