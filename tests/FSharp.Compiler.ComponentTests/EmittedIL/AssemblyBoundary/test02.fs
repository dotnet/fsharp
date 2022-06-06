// #Regression #CodeGen #Optimizations #Assemblies 
//	Do not inline of anything that uses �protected� things
//	Do not inline of anything that uses explicitly internal values/types/methods/properties/values/union-cases/record-fields/exceptions. This includes 
//   - generic things instantiated at internal types
//   - typeof/sizeof etc. on internal types
//	Do not inline of anything that uses implicitly internal values such as �let f() = ...� in a class
// Regression test for FSHARP1.0:5849
//<Expects status="success"></Expects>

(new N.L2.T()).F(10) |> ignore
