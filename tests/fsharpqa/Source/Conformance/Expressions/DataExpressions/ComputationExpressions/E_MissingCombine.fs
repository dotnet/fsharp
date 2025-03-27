// #Regression #Conformance #DataExpressions #ComputationExpressions 
// Regression test for FSHARP1.0:6149
//<Expects status="error" span="(19,3-19,14)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'Combine' method$</Expects>

type R = S of string 
 
type T() = 
  member x.Bind(p: R, rest: (string -> R)) =  
    match p with 
    | S(s) -> rest s 
  member x.Zero() = S("l")
  member x.For(s : seq<int>, rest: (int -> unit)) = S("")

let t = new T()

let t' = t { 
  let a = 10
  for x in [1] do ()
  0 |> ignore
}
