// #Regression #Conformance #LexFilter 
// Regression test for FSHARP1.0:5399
//<Expects status="error" id="FS1161" span="(5,1-5,2)">TABs are not allowed in F# code unless the #indent "off" option is used</Expects>

	(* <- invisible tab! *) type T = class end
