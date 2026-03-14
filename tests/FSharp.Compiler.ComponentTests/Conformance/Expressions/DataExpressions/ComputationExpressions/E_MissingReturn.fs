// #Regression #Conformance #DataExpressions #ComputationExpressions 
// Regression test for FSHARP1.0:6149
//<Expects status="error" span="(16,3-16,9)" id="FS0708">This control construct may only be used if the computation expression builder defines a 'Return' method$</Expects>

type R = S of string 
 
type T() = 
  member x.Bind(p: R, rest: (string -> R)) =  
    match p with 
    | S(s) -> rest s 

let t = new T()

let t' = t { 
  let! x = S("1")
  return S("2")
}
