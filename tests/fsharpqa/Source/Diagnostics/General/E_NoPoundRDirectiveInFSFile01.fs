// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3203
//<Expects id="FS0076" span="(6,1-6,22)" status="error">#r directives may only occur in F# script files \(extensions \.fsx or \.fsscript\)\. Either move this code to a script file, add a '-r' compiler option for this reference or delimit the directive with '#if INTERACTIVE'/'#endif'</Expects>
module M

#r "DoesNotExist.dll"


let x = 1.0<Microsoft.FSharp.Math.SI.A>

exit 0
