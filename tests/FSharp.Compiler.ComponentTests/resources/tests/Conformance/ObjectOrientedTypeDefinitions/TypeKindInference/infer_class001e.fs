// #Regression #Conformance #ObjectOrientedTypes #TypeInference 
// attribute must match struct-end
//<Expects id="FS0927" status="error" span="(9,6)">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>
//<Expects id="FS0927" status="error" span="(14,6)">The kind of the type specified by its attributes does not match the kind implied by its definition</Expects>

module TypeInference 

[<Class>]
type TK_C_000 =
 struct
 end

[<Class>]
type TK_C_001 =
 interface
 end
