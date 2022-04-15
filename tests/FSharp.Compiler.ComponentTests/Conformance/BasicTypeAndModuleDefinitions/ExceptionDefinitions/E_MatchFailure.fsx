// #Regression #Conformance #TypesAndModules #Exceptions 
// Regression test for FSHARP1.0:3769
// Verify that F# exception MatchFailure is gone
//<Expects status="error" id="FS0039">The value or constructor 'MatchFailure' is not defined</Expects>

let p = MatchFailure("",1,2)

