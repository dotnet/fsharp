// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify error when defining an Active Pattern with more than seven 'values'
// This is regression test for FSHARP1.0:3562


let (|One|Two|Three|Four|Five|Six|Seven|Eight|) x = One

