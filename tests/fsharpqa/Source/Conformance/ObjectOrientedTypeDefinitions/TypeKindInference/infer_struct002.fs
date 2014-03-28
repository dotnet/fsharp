// #Conformance #ObjectOrientedTypes #TypeInference 
#light
// Verify the use of Type Kind Attributes
module TypeInference 

[<StructAttribute>]
type TK_S_001 =
  val StructsMustContainAtLeastOneField: int

  
let mutable a = false
try 
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_S_001")).IsValueType
with 
  | _ as e -> 
    printfn "%A" e
    a <- false
    
(if (a) then 0 else 1) |> exit


