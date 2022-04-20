// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
// Regression test for FSHARP1.0:5181

// DU
[<StructuralEquality>]
[<CustomComparison>]
type DU = 
    | A of int
    interface System.IComparable with
        override this.CompareTo(a:obj) = -1    // first always less than second


let a = A(1)
let b = A(2)

if not (a < b && b < a) then failwith "Failed: 1"
