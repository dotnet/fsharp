// #Regression #Diagnostics #RequiresPowerPack 
// Regression test for FSHARP1.0:3214
// Used to be warning. Now (4/17/2009) is just an error
//<Expects id="FS0039" span="(6,28)" status="error">The namespace 'Notation' is not defined</Expects>
module M
open Microsoft.FSharp.Math.Notation

let twosV = vector [ 2.0; 2.0 ]
