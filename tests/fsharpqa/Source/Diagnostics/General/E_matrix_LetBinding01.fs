// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3175
//<Expects id="FS0039" span="(6,18-6,24)" status="error">The type 'matrix' is not defined in 'Microsoft.FSharp.Math'\.$</Expects>


let f ( m : Math.matrix) = 12
