// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:2261
//<Expects id="FS0513" status="error">End of file in #if section begun at or after here</Expects>
#light

#if SOME_FLAG
let x = 1
