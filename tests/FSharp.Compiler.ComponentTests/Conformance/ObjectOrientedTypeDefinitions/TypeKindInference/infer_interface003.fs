// #Conformance #ObjectOrientedTypes #TypeInference 
// Verify the use of interface/end
module TypeInference 

type TK_I_002 =
 interface
 end
  
let mutable a = false

try 
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_I_002")).IsInterface
with 
  | _ as e -> 
    printfn "%A" e
    a <- false
    
(if a then 0 else 1) |> exit


