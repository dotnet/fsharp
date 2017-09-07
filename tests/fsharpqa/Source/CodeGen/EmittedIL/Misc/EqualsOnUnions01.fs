// #Regression #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for FSHARP1.0:5543
// Inefficiency on .Equals()
// This test has nothing to do with debugging spans, etc...
// It's a pure IL verification
type U = | A 
         | B of int
