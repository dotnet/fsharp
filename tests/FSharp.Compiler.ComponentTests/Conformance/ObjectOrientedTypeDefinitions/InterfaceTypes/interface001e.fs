// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Verify that the inherited type must be an interface
//<Expects id="FS0887" span="(15,3-15,16)" status="error">The type 'C_000' is not an interface type$</Expects>
 
type I_000 = 
  abstract Mem : unit -> int
  
type C_000 =
  class
    interface I_000 with member x.Mem() = 0
  end

type I_001<'a> =
 interface
  inherit C_000
 end 
