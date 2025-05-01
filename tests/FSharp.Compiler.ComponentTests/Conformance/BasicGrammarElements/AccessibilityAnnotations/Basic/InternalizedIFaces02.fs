// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSHARP1.0:5042
// It is *ok* to implement internalized interfaces
//<Expects status="success"></Expects>

module N.M
 type StdGen = A | B

 type IGen = 

   abstract AsGenObject : Gen<obj>

 and Gen<'a> = 
  | Gen of (int -> StdGen -> 'a) 
  member x.Map f = failwith ""
  interface IGen with
    member x.AsGenObject = x.Map box
