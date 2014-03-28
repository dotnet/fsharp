// #Regression #Diagnostics 
// Used to be regression test for FSHARP1.0:1185
// to be compiled with --warnaserror+
//<Expects id="FS0554" span="(9,13-9,20)" status="error">Invalid declaration syntax</Expects>
//<Expects id="FS0547" span="(7,6-7,8)" status="error": A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'</Expects>

type T() =
     member this.X
       with set x,y = ()

exit 1
