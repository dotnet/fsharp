// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
// Verify warning when implementing IComparable on structs / classes
// See FSB 2653

//<Expects id="FS0342" span="(10,6)" status="warning">The type 'S' implements 'System\.IComparable'\. Consider also adding an explicit override for 'Object\.Equals'</Expects>
//<Expects id="FS0343" span="(15,6)" status="warning">The type 'C' implements 'System\.IComparable' explicitly but provides no corresponding override for 'Object\.Equals'\. An implementation of 'Object\.Equals' has been automatically provided, implemented via 'System\.IComparable'\. Consider implementing the override 'Object\.Equals' explicitly</Expects>

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
