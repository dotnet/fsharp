// #Regression #Diagnostics 
//<Expects status="warning" span="(10,1-10,1)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(9:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="warning" span="(10,1-10,1)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(9:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="error" span="(9,6-9,9)" id="FS0547">A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'\.$</Expects>
//<Expects status="warning" span="(4,1-4,1)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(3:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="warning" span="(4,1-4,1)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(3:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="error" span="(3,6-3,8)" id="FS0547">A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'\.$</Expects>
module M
type C =
