// #Regression #Diagnostics
//<Expects status="error" span="(7,1-7,1)" id="FS0058">.*</Expects>
//<Expects status="error" span="(6,6-6,8)" id="FS0547">A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'\.$</Expects>


type C() =
