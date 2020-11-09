// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSHARP1.0:1561
// The OCaml (** *) feature is now gone, so this test will be a negative test from now on.
//<Expects status="warning" span="(15,1)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(10:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="warning" span="(15,1)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(10:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="error" span="(15,1)" id="FS0010">Incomplete structured construct at or before this point in binding$</Expects>
//<Expects status="error" span="(10,10)" id="FS0516">End of file in comment begun at or before here$</Expects>


let y6 = (** This is a comment with (***) 6 (**)

if y6 <> 6 then exit 1

exit 1
