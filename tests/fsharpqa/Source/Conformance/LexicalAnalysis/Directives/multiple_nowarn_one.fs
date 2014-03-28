// #Regression #Conformance #LexicalAnalysis #ReqNOMT 
// Regression test for FSHARP1.0:6044
// Just one #nowarn
#nowarn "49"

[<System.Obsolete("Dummy attribute to trigger a warning")>]
let obsoleteIdentifier = 12

exit 0
