// #Regression #Conformance #DeclarationElements #Accessibility 
// Verify error associated with more accessible base class than super class
//<Expects id="FS0410" status="error" span="(10,6)">The type 'C1' is less accessible than the value, member or type 'C2' it is used in</Expects>

// Class inheritance:

type private C1() =
    member x.P = 1

type C2() =
    inherit C1()
    member x.Q = 1


let c = C2()
