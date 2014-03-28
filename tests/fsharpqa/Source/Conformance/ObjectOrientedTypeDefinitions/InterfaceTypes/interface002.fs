// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Verify properties
#light

 
//Interface with properties
type I_004<'a> =
 interface
  abstract Prop : 'a with get
 end 

{new I_004<int> with member x.Prop = 0 }.Prop |> exit


