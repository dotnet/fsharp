// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5675
//<Expects status="success">val ``A\.B`` : bool = true</Expects>
//<Expects status="success">val ``+`` : bool = true</Expects>
//<Expects status="success">val ``\.\.`` : bool = true</Expects>
//<Expects status="success">val ``\.\. \.\.`` : bool = true</Expects>
//<Expects status="success">val ``(+)`` : bool = true</Expects>
//<Expects status="success">val ``land`` : bool = true</Expects>
//<Expects status="success">val ``type`` : bool = true</Expects>
//<Expects status="success">val ``or`` : bool = true</Expects>
//<Expects status="success">val ``params`` : bool = true</Expects>
//<Expects status="success">val A: bool = true</Expects>
//<Expects status="success">val ``'A`` : bool = true</Expects>
//<Expects status="success">val A' : bool = true</Expects>
//<Expects status="success">val ``0A`` : bool = true</Expects>
//<Expects status="success">val A0 : bool = true</Expects>
//<Expects status="success">val ``A-B`` : bool = true</Expects>
//<Expects status="success">val ``A B`` : bool = true</Expects>
//<Expects status="success">val ``base`` : bool = true</Expects>
//<Expects status="success">val ``or`` : bool = true</Expects>

let ``A.B``   = true;;
let ``+``   = true;;
let ``..``   = true;;
let ``.. ..``   = true;;
let ``(+)``   = true;;
let ``land``   = true;;
let ``type``   = true;;
let ``or``   = true;;
let ``params``   = true;;
let ``A``   = true;;
let ``'A``   = true;;
let ``A'``   = true;;
let ``0A``   = true;;
let ``A0``   = true;;
let ``A-B``   = true;;
let ``A B``   = true;;
let ``base``   = true;;
let ``or``   = true;;

exit 0;;

