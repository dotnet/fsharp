// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify error when defining an Active Pattern with more than seven 'values'
// This is regression test for FSHARP1.0:3562
//<Expects id="FS0265" span="(6,53)" status="error">Active patterns cannot return more than 7 possibilities$</Expects>

let (|One|Two|Three|Four|Five|Six|Seven|Eight|) x = One

