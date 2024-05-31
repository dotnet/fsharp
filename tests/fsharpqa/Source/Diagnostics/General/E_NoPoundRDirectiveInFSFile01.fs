// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3203
//<Expects id="FS0076" span="(6,1-6,22)" status="error">This directive may only be used in F# script files \(extensions \.fsx or \.fsscript\)\.</Expects>
module M

#r "DoesNotExist.dll"


let x = 1.0<Microsoft.FSharp.Math.SI.A>

exit 0
