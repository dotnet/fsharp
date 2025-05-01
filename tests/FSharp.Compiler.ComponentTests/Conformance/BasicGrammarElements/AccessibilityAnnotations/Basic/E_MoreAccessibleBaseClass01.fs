// #Regression #Conformance #DeclarationElements #Accessibility 
// Verify error associated with more accessible base class than super class


// Class inheritance:

type private C1() =
    member x.P = 1

type C2() =
    inherit C1()
    member x.Q = 1


let c = C2()
