// #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints  
// Regression test for FSharp1.0:6391 - Unitized structs apparently does not satisfy struct constraint

[<Measure>]
type ms
type x = System.Nullable<int<ms>>

let x_obj = Unchecked.defaultof<x>

match x_obj.HasValue with
| false -> ignore 0
| _     -> ignore 1