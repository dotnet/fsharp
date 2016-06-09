// #Regression #NoMT #FSI 
// Regression test for FSharp1.0:4164 - FSI emit "please report to fsbugs" error on malformed usage of .. operator

// <Expects status="notin">fsbug</Expects>
// <Expects status="notin">nonTerminalId\.GetTag</Expects>
//<Expects status="error" span="(8,5)" id="FS0010">Unexpected symbol '\.\.' in interaction\. Expected incomplete structured construct at or before this point, ';', ';;' or other token\.$</Expects>

aaaa..;;

