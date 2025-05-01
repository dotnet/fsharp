// #Regression #Conformance #TypeInference #Recursion 
// Regression test for FSharp1.0:4275 - still allowing variations on recursive structs


[<Struct>]
type S2 (def : S2) =
  member s.XYZ = def
