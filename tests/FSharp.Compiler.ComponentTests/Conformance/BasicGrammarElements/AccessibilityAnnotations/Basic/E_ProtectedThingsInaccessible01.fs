// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSharp1.0:4227 - protected access checks are not correctly implemented


type C1() as this =
    inherit System.MarshalByRefObject()
    member x.M(c:C1) = c.MemberwiseClone(false)         // System.MarshalByRefObject.MemberwiseClone is just a protected method - nothing special about it

type C2() as this =
    inherit System.MarshalByRefObject()
    member x.M(c:C1) = c.MemberwiseClone(false)
