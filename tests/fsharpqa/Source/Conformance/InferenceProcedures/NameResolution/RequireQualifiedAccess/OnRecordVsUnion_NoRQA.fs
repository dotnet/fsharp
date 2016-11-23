// #Conformance #TypeInference #Attributes 
// Verify the access works on unions without RQA

//<Expects id="FS0002" status="error">This function takes too many arguments, or is used in a context where a function is not expected</Expects>

module A =
    type U = | C

    type C() =
        static member M() = ()

let x:A.U = A.C