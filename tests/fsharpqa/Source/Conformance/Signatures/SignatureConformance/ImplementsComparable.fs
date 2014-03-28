// #Conformance #SignatureFiles 
module A
type C() = 
    interface System.IComparable with 
       member x.CompareTo(yobj) = 0
