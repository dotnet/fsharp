// #Regression #Libraries #Operators 
// Regression test for FSHARP1.0:5436
// You should not be able to hash F# function values
// Note: most positive cases already covered under fsharp\typecheck\sigs
// I'm adding this simple one since I did not see it there.
//<Expects status="success"></Expects>
module TestModule

// This is ok (unchecked)
let p = Unchecked.hash id
