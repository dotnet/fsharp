// #Conformance #SignatureFiles 
#light

// Verify error if you FSI has methods listed as internal / private but implementation does not

namespace Cake

type Cake(description : string) =
    member this.Description = description

type EvenMoreDeliciousCake() =
    inherit Cake("")

    member this.Bake () = ()

    static member Eat () = ()

