// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
// Regression test for FSHARP1.0:5181

// Struct
[<StructuralEquality>]
[<CustomComparison>]
type S(x:int) = struct
                 interface System.IComparable with
                   override this.CompareTo(a:obj) = -1       // first always less than second
                end

let a = S(1)
let b = S(2)

if not (a < b && b < a) then failwith "Failed: 1"
