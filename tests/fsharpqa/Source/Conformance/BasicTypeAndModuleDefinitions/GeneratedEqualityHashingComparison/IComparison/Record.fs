// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing #Attributes 
// Regression test for FSHARP1.0:5181

// Record
[<StructuralEquality>]
[<CustomComparison>]
type R = { f : int } with
           interface System.IComparable with
              override this.CompareTo(a:obj) = -1       // first always less than second

let a = { f = 1 }
let b = { f = 2 }

(if a < b && b < a then 0 else 1) |> exit
