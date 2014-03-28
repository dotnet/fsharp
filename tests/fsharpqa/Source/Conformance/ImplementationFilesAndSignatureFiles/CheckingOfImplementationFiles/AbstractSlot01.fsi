// #Conformance #SignatureFiles 
#light

module AbstractSlot01

type Foo =
    new : unit -> Foo
    abstract AbstractMethod : unit -> int
    default AbstractMethod : unit -> int

type Bar =
    inherit Foo
    new : unit -> Bar
    override AbstractMethod : unit -> int
    
