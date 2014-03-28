// #Regression #Conformance #SignatureFiles 
#light

// Regression test for FSHARP1.0:4155 - combined accessibilities internal --> internal give "private"

module internal M =

    let internal x = 1

    type internal T() =
        member internal x.X () = 2
        member private x.Y    = 3

let a = M.x
let b = (M.T()).X()

exit 0
