// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5675
//<Expects status="success">val \( A\.B \) : bool = true</Expects>
//<Expects status="success">val \( \.\. \) : bool = true</Expects>
//<Expects status="success">val A : bool = true</Expects>

let ``A.B``   = true;;
let ``..``   = true;;
let ``A``   = true;;

exit 0;;

