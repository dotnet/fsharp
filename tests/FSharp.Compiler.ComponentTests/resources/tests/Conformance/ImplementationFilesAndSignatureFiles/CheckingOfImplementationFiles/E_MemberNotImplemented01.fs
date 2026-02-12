// #Conformance #SignatureFiles 
module Test

[<Sealed>]
type C1() = 
//  static member op_Explicit (x:C1) = 1
    static member op_Explicit (x:C1) = 1.0
