// #Conformance #TypeInference #Attributes 
// Verify the access works on unions without RQA

module A =
    type U = | C

    type C() =
        static member M() = ()

let x = A.C