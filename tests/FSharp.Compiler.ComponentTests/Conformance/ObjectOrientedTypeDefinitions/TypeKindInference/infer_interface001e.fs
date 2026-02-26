// #Regression #Conformance #ObjectOrientedTypes #TypeInference 
// attribute must match interface-end decl
//<Expects id="FS0927" status="error" span="(9,6)">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>
//<Expects id="FS0927" status="error" span="(14,6)">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>

module TypeInference 

[<Interface>]
type TK_I_000E =
 class
 end
 
[<Interface>]
type TK_I_001E =
 struct
 end
