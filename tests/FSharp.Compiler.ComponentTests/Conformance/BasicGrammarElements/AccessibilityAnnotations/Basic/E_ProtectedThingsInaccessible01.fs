// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSharp1.0:4227 - protected access checks are not correctly implemented
//<Expects id="FS0629" span="(11,24-11,41)" status="error">Method 'MemberwiseClone' is not accessible from this code location</Expects>

type C1() as this =
    inherit System.MarshalByRefObject()
    member x.M(c:C1) = c.MemberwiseClone(false)         // System.MarshalByRefObject.MemberwiseClone is just a protected method - nothing special about it

type C2() as this =
    inherit System.MarshalByRefObject()
    member x.M(c:C1) = c.MemberwiseClone(false)
