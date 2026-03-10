// #Conformance #TypeInference #Attributes 
// Verify the RequireQualifiedAccess attribute works on unions

module A =
    [<RequireQualifiedAccess>]
    type U = | C

    type C() =
        static member M() = ()

let x = A.C.M()