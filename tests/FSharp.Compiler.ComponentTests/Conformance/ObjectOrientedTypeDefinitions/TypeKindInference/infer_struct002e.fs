// #Regression #Conformance #ObjectOrientedTypes #TypeInference 

// attribute must match inferred type-kind
//<Expects id="FS0927" status="error">kind.*does not match</Expects>
//<Expects id="FS0926" status="error">multiple kinds</Expects>


//infer_struct002e.fs(13,6): error FS0191: The kind of the type specified by
//its attributes does not match the kind implied by its definition.
//
//infer_struct002e.fs(20,6): error FS0191: The attributes of this type specify multiple kinds for the type.

[<Interface>]
type TK_S_001 =
  struct
    val StructsMustContainAtLeastOneField: int
  end
  
[<Class>]
[<Struct>]
type TK_S_002 =
    val StructsMustContainAtLeastOneField: int

exit 1
