// #Conformance #TypeInference #Attributes 
// Verify the access works on unions where type name is case name

module A =
    [<RequireQualifiedAccess>]
    type C =
    | B 
    | C

let x = A.C