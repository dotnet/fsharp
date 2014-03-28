// #Conformance #SignatureFiles 
#light

// Verify ability to use FSI files in conjunction with internal types

module InternalAccessibility01

type internal T() = 
    member x.P = 1


let f (x : T)   = x.P
let g (x : int) = x

exit 0
