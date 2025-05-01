// #Regression #Conformance #DeclarationElements #LetBindings 
// Regression test for FSharp1.0:4488
// fsc throws ICE on source code error [was TABs caused ICE]
//<Expects status="warning" span="(10,1-10,5)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(8:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="error" span="(10,6-10,7)" id="FS0010">Unexpected start of structured construct in expression$</Expects>
//<Expects status="error" span="(9,5-9,6)" id="FS0583">Unmatched '\('$</Expects>
//<Expects status="error" span="(10,16-10,17)" id="FS0010">Unexpected symbol '\)' in implementation file$</Expects>
let x = 
    (   try 123
with e -> 1234 )

do ()

[< >]
do ()
