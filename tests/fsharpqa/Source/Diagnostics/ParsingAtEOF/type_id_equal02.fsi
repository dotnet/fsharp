// #Regression #Diagnostics
//<Expects status="error" span="(8,1-8,1)" id="FS0058">.*</Expects>
//<Expects status="error" span="(7,6-7,9)" id="FS0547">A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'\.$</Expects>
//<Expects status="error" span="(4,1-4,1)" id="FS0058">.*</Expects>
//<Expects status="error" span="(3,6-3,8)" id="FS0547">A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'\.$</Expects>
module M
type C =
