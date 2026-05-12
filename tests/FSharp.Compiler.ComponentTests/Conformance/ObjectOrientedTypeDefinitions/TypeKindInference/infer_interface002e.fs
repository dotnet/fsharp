// #Regression #Conformance #ObjectOrientedTypes #TypeInference 
// attribute must match inferred type
//<Expects id="FS0927" span="(16,6-16,15)" status="error">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>
//<Expects id="FS0931" span="(20,12-20,20)" status="error">Structs, interfaces, enums and delegates cannot inherit from other types</Expects>
//<Expects id="FS0946" span="(20,12-20,20)" status="error">Cannot inherit from interface type\. Use interface \.\.\. with instead</Expects>
//<Expects id="FS0927" span="(28,6-28,15)" status="error">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>
//<Expects id="FS0931" span="(32,12-32,20)" status="error">Structs, interfaces, enums and delegates cannot inherit from other types</Expects>
//<Expects id="FS0946" span="(32,12-32,20)" status="error">Cannot inherit from interface type\. Use interface \.\.\. with instead</Expects>

// An interface
type TK_I_003 = interface 
                end


[<Struct>]
type TK_I_004a = TK_I_003

[<Struct>]
type TK_I_004b = 
   inherit TK_I_003
  
// Another interface 
type TK_I_005 =
  abstract M  : unit -> unit


[<Struct>]
type TK_I_006a = TK_I_005
  
[<Struct>]
type TK_I_006b = 
   inherit TK_I_005
