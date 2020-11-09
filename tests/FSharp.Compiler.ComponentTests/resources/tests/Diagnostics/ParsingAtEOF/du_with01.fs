// #Regression #Diagnostics 
//<Expects status="warning" span="(9,1-9,7)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(6:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="warning" span="(9,7-9,7)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(6:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
// Note: this is now a positive test (see DevDiv:258510)

type C =
  | A 
  | B
  with