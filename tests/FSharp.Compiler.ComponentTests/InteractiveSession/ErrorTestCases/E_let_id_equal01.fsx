// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5629
//<Expects status="error" span="(4,9)" id="FS0010">Incomplete structured construct at or before this point in binding$</Expects>
let x = ;;
exit 1;;
