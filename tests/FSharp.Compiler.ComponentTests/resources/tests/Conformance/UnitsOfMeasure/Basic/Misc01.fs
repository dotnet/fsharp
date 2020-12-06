// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2869
#light

// Polymorphic recursion
let rec prodlists<[<Measure>] 'u,[<Measure>] 'v>(xs:float<'u> list,ys:float<'v> list) : float<'u 'v> list = 
  match xs,ys with
  | x::xs,y::ys -> x*y :: prodlists<'v,'u>(ys,xs)
  | _ -> []
