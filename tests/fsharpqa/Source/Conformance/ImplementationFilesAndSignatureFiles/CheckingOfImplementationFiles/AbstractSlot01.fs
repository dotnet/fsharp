// #Conformance #SignatureFiles 
#light

// Check FSI signature files for abstract slots
module AbstractSlot01

type Foo() =
    abstract AbstractMethod : unit -> int
    default this.AbstractMethod () = 1

type Bar() =
    inherit Foo()
    override this.AbstractMethod () = 2
    
let t = new Foo()
if t.AbstractMethod() <> 1 then exit 1

let b = new Bar()
if b.AbstractMethod() <> 2 then exit 1

let d = b :> Foo
if d.AbstractMethod() <> 2 then exit 1

exit 0
