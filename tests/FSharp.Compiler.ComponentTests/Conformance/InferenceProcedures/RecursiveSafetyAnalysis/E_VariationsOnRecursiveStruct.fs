// #Regression #Conformance #TypeInference #Recursion 
// Regression test for FSharp1.0:4275 - still allowing variations on recursive structs
//<Expects id="FS0954" span="(6,6-6,8)" status="error">This type definition involves an immediate cyclic reference through a struct field or inheritance relation</Expects>

[<Struct>]
type S2 (def : S2) =
  member s.XYZ = def
