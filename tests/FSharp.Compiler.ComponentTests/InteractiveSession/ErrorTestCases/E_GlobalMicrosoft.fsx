// #Regression #NoMT #FSI 
// Regression test for FSharp1.0:5260 and FSHARP1.0:5270
// <Expects id="FS0039" status="error" span="(5,8)">The value or constructor 'Microsoft' is not defined</Expects>

global.Microsoft;;
exit 1;;
