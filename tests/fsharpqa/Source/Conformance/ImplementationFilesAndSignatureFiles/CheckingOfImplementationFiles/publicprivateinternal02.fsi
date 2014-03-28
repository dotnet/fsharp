// #Conformance #SignatureFiles 
#light

// Verify error if you FSI has methods listed as internal / private but implementation does not

namespace Cake

[<Class>]
type Cake =
    
    member Description : string

[<Class>]
type EvenMoreDeliciousCake =
    inherit Cake

    member private Bake : unit -> unit
    static member internal Eat  : unit -> unit

