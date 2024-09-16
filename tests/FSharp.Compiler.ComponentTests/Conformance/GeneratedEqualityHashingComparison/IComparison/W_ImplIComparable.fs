// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
// Verify warning when implementing IComparable on structs / classes
// See FSB 2653




[<CustomComparison>]
[<StructuralEquality>]
type S = struct
            interface System.IComparable with   
                member x.CompareTo(y:obj) = 0
         end

type C = class
            interface System.IComparable with   
                member x.CompareTo(y:obj) = 0
         end
