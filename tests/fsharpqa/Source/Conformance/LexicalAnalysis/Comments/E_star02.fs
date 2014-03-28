// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:5444
// (*) in comments
//<Expects status="error" id="FS0010" span="(8,3-8,4)">Unexpected symbol '\*' in implementation file$</Expects>

(*
let a2 = (*)
*)*)
let b2 = ()
