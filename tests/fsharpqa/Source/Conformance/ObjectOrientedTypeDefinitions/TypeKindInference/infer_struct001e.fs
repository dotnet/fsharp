// #Regression #Conformance #ObjectOrientedTypes #TypeInference 
#light
// attribute must match struct-end
//<Expects id="FS0927" status="error">kind.*does not match</Expects>
//<Expects id="FS0927" status="error">kind.*does not match</Expects>

//infer_interface001e.fs(16,6): error FS0191: The kind of the type specified by its attributes does not match the kind impl
//ied by its definition.
//
//infer_interface001e.fs(22,6): error FS0191: The kind of the type specified by its attributes does not match the kind imp
//lied by its definition.

module TypeInference 


[<Struct>]
type TK_S_000 =
 class
 end
 

[<Struct>]
type TK_S_001 =
  interface
  end
  

  
exit 1
