// #Regression #TypeProvider #Arrays
// This is regression test for DevDiv:213995
// TP limit array rank to 4
//<Expects status="error" span="(6,6-6,7)" id="FS3138">F# supports a maximum array rank of 4$</Expects>

type X = N.T<"System.String", 5>

let _ = typeof<X>

exit 0



