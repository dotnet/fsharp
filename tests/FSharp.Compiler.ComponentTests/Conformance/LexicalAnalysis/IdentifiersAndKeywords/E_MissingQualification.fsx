// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSharp1.0:3420 - parsing glitch allows trailing "." in file
//<Expects id="FS0599" span="(8,2-8,3)" status="error">Missing qualification after '\.'</Expects>

#light

let x = 1
x.
