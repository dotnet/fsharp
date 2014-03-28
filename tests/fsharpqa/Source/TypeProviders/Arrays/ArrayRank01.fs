// #Regression #TypeProvider #Arrays
// This is regression test for DevDiv:213995
// TP limit array rank to 4
//<Expects status="success"></Expects>

type X = N.T<"System.String", 4>

let _ = typeof<X>

exit 0



