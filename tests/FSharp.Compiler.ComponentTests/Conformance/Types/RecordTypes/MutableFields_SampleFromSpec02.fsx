// #Conformance #TypesAndModules #Records 
// Sample found on the spec.
// Section 9.2 (second code snippet)
//<Expects status="success"></Expects>
#light

type R2 = 
    { mutable x : int; 
      mutable y : int } 
    member this.Move(dx,dy) = 
        this.x <- this.x + dx
        this.y <- this.y + dy
