// #Conformance #ObjectOrientedTypes #TypeInference 
#light
// Verify the use of struct/end
module TypeInference 

type TK_S_002 =
  struct
    val StructsMustContainAtLeastOneField: int
  end
  
let mutable a = false
try 
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_S_002")).IsValueType
with 
  | _ as e -> 
    printfn "%A" e
    a <- false
    
(if (a) then 0 else 1) |> exit


