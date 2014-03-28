// #Regression #Conformance #Quotations 
#light  

// FSB 1075, TOp_asm in pattern match

type T = | A of float

[<ReflectedDefinition>]
let foo v =
  match v with 
  | A(1.0) -> 0
  | _ -> 1

// Previously was compile error
exit 0
