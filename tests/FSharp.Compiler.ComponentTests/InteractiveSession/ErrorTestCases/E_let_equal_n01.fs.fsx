// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5629
//<Expects status="error" span="(4,5)" id="FS0010">Unexpected symbol '=' in binding$</Expects>
let = 1;;
exit 1;;
