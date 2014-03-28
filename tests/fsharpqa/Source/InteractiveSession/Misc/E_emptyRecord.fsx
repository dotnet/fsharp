// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5629
//<Expects status="error" span="(4,12)" id="FS0010">Unexpected symbol '}' in field declaration\. Expected identifier or other token\.$</Expects>
type R = { };;
exit 1;;
