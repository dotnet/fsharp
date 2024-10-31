// #Regression #Conformance #LexicalAnalysis #ReqNOMT 
// Regression test for FSHARP1.0:6044

#nowarn "44" "49"

[<System.Obsolete("Dummy attribute to trigger a warning")>]
let obsoleteIdentifier = 12

let f UpperCaseArgument = obsoleteIdentifier

exit 0
