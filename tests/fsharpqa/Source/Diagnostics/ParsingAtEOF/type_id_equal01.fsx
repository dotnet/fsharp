// #Regression #Diagnostics 
//<Expects status="error" span="(5,1-5,1)" id="FS0058">Possible incorrect indentation: this token is offside of context started at position \(4:1\)\. Try indenting this token further or using standard formatting conventions\.$</Expects>
//<Expects status="error" span="(4,6-4,8)" id="FS0547">A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'\.$</Expects>
type C() = 
