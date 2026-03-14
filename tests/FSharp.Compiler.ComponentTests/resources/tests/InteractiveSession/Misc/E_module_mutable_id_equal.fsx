// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5629
//<Expects status="error" span="(5,8)" id="FS0010">Unexpected start of structured construct in interaction\. Expected identifier, 'global' or other token\.$</Expects>

module mutable M = ;;
exit 1;;
