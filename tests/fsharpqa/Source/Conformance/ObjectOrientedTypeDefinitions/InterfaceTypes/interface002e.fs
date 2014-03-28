// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Verify that there may be no circular references between  inherits declarations based on the named types of the inherited interface types. 
//<Expects span="(7,6)" id="FS0954" status="error">This type definition involves an immediate cyclic reference through a struct field or inheritance relation</Expects>
//<Expects span="(13,5)" id="FS0954" status="error">This type definition involves an immediate cyclic reference through a struct field or inheritance relation</Expects>
//<Expects span="(18,5)" id="FS0954" status="error">This type definition involves an immediate cyclic reference through a struct field or inheritance relation</Expects>

type I_000 = 
  interface 
    inherit I_001
    abstract Mem : unit -> int
  end

and I_001 =
 interface
  inherit I_002
 end 

and I_002 =
  interface
    inherit I_000
  end

