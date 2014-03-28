// #Regression #Libraries #Operators 
// Regression test for FSHARP1.0:5436
// You should not be able to hash F# function values)
// Note: most positive cases already covered under fsharp\typecheck\sigs
// I'm adding this simple one since I did not see it there.
//<Expects status="error" id="FS0001" span="(8,14-8,16)">The type '\('a -> 'a\)' does not support the 'equality' constraint because it is a function type</Expects>

let q = hash id
