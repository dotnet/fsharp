// #Conformance #TypesAndModules #Records 
// Sample found on the spec.
// Section 9.2
//<Expects status="success"></Expects>
#light

type R1 = 
    { x : int; 
      y: int } 
    member this.Sum = this.x + this.y 
