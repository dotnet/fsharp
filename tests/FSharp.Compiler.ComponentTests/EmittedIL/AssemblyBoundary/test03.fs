// #Regression #CodeGen #Optimizations #Assemblies 
//	Do not inline of anything that uses �protected� things
//	Do not inline of anything that uses explicitly internal values/types/methods/properties/values/union-cases/record-fields/exceptions. This includes 
//   - generic things instantiated at internal types
//   - typeof/sizeof etc. on internal types
//	Do not inline of anything that uses implicitly internal values such as �let f() = ...� in a class
// Regression test for FSHARP1.0:5849
//<Expects status="success"></Expects>

try
  N.L3.f1(box 1) |> ignore
with
  | _ -> ()

try
  N.L3.f1(box 2) |> ignore
with
  | _ -> ()

N.L3.k1() |> ignore
N.L3.k2() |> ignore
N.L3.k3() |> ignore
N.L3.s1() |> ignore
N.L3.s2() |> ignore
N.L3.s3() |> ignore
