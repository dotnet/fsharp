// #Regression #Conformance #DataExpressions #ComputationExpressions 
// Regression test for FSHARP1.0:6149
//<Expects status="error" span="(17,3-17,10)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'ReturnFrom' method$</Expects>

type R = S of string 
 
type T() = 
  member x.Bind(p: R, rest: (string -> R)) =  
    match p with 
    | S(s) -> rest s 
  member x.Zero() = S("0")

let t = new T()

let t' = t { 
  let! x = S("1")
  return! S("2")
}
